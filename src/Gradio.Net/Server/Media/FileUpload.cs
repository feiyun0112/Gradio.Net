using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Gradio.Net;
/// <summary>
/// Reads an upload request: the multipart body, one file at a time, straight to disk.
/// </summary>
/// <remarks>
/// <para>
/// The Python version vendors Starlette's multipart parser to change three things about it, and all
/// three are why this is a reader of its own rather than a call to <c>Request.ReadFormAsync</c>:
/// the file is written to a temporary file in the upload directory rather than to memory, its
/// content hash is computed as it arrives, and every read is reported to the progress tracker. A
/// form reader that buffered the body would hold a file of any size in memory, which is exactly
/// what an upload of a large file must not do.
/// </para>
/// <para>
/// The file's address is the hash of its content, so the same bytes uploaded twice are stored once.
/// That is the Python version's scheme, and it is why the temporary file has to be hashed while it
/// is written rather than afterwards: the destination is not known until the last byte has arrived.
/// </para>
/// </remarks>
internal static class FileUpload
{
    /// <summary>The form field an uploaded file has to be sent as.</summary> <remarks>The Python
    /// version reads <c>form.getlist("files")</c> and ignores every other field.</remarks>
    private const string FileField = "files";

    /// <summary>The most files one request may carry.</summary>
    private const int MaxFiles = 1000;

    /// <summary>The most a part's headers may take, in bytes.</summary>
    /// <remarks>
    /// A header is read into memory before the file it describes is, so it is the one part of an
    /// upload that a client could use to make the server allocate. The Python version's limit, and
    /// its reason.
    /// </remarks>
    private const int HeadersLengthLimit = 8 * 1024;

    /// <summary>How much of a body is read at a time.</summary>
    /// <remarks>
    /// Large enough that a big file is not a million syscalls, small enough that the memory a
    /// request holds does not grow with the file. It is the size the Python version's parser reads
    /// in as well.
    /// </remarks>
    private const int ChunkSize = 64 * 1024;

    /// <summary>What reading a request produced: the files, or why it was refused.</summary>
    /// <param name="Paths">The files, in the order they arrived.</param>
    /// <param name="Refusal">What to tell the client, or <see langword="null"/> when nothing was refused.</param>
    /// <param name="Status">The status a refusal is answered with.</param>
    internal sealed record Outcome(IReadOnlyList<string> Paths, string? Refusal, int Status)
    {
        /// <summary>Whether the request was served.</summary>
        internal bool Accepted => Refusal is null;

        /// <summary>A refusal, with the status the Python version answers it with.</summary>
        internal static Outcome Refuse(string detail, int status) => new([], detail, status);
    }

    /// <summary>Whether a request carries a multipart form, which is the only shape an upload has.</summary>
    /// <param name="request">The request.</param>
    /// <returns>Whether it declares a multipart body with a boundary.</returns>
    /// <remarks>
    /// Asked before anything is read, because the refusal is a different answer from the ones the
    /// reader gives: the Python version refuses a request of the wrong shape with a <c>detail</c>
    /// document rather than with the plain text a parser failure is reported as.
    /// </remarks>
    internal static bool IsMultipart(HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return TryReadBoundary(request.ContentType, out _);
    }

    /// <summary>Reads a request's files into an application's upload folder.</summary>
    /// <param name="request">The request being read.</param>
    /// <param name="folder">The directory the files are stored in.</param>
    /// <param name="maxFileSize">The largest file the application accepts, or <see langword="null"/> for no limit.</param>
    /// <param name="progress">Where the progress of the upload is reported.</param>
    /// <param name="uploadId">The id the client asked to be told about, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The files, or the refusal.</returns>
    /// <remarks>
    /// The caller owns the progress tracker's lifetime: this records what arrives and says when the
    /// request is over, but a refusal it returns is the caller's to answer with, and the stream the
    /// client is reading ends when the caller says so.
    /// </remarks>
    internal static async Task<Outcome> ReadAsync(
        HttpRequest request,
        string folder,
        long? maxFileSize,
        UploadProgress progress,
        string? uploadId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(progress);

        if (!TryReadBoundary(request.ContentType, out var boundary))
        {
            return Outcome.Refuse("Invalid content type.", StatusCodes.Status400BadRequest);
        }

        Directory.CreateDirectory(folder);

        var reader = new MultipartReader(boundary, request.Body)
        {
            HeadersLengthLimit = HeadersLengthLimit,
        };

        var paths = new List<string>();
        var files = 0;

        try
        {
            MultipartSection? section;
            while ((section = await reader.ReadNextSectionAsync(cancellationToken).ConfigureAwait(false)) is not null)
            {
                if (!TryReadFile(section, out var fileName))
                {
                    continue;
                }

                files++;
                if (files > MaxFiles)
                {
                    return Outcome.Refuse(
                        $"Too many files. Maximum number of files is {MaxFiles}.",
                        StatusCodes.Status400BadRequest);
                }

                var stored = await StoreAsync(
                    section.Body,
                    folder,
                    fileName,
                    maxFileSize,
                    progress,
                    uploadId,
                    cancellationToken).ConfigureAwait(false);

                if (stored.Refusal is not null)
                {
                    return stored;
                }

                paths.AddRange(stored.Paths);
            }
        }
        catch (IOException error)
        {
            return Outcome.Refuse(
                $"Could not parse multipart request: {error.Message}",
                StatusCodes.Status400BadRequest);
        }
        catch (InvalidDataException error) when (IsHeaderLimit(error))
        {
            // A part's headers are read into memory, so they have a limit of their own; the Python version
            // answers that limit with the same status as a file's, because both are "too large".
            return Outcome.Refuse(
                $"Headers exceeded maximum allowed size of {HeadersLengthLimit} bytes.",
                StatusCodes.Status413PayloadTooLarge);
        }
        catch (InvalidDataException error)
        {
            return Outcome.Refuse(error.Message, StatusCodes.Status400BadRequest);
        }
        finally
        {
            if (uploadId is not null)
            {
                progress.Complete(uploadId);
            }
        }

        return new Outcome(paths, Refusal: null, StatusCodes.Status200OK);
    }

    /// <summary>Writes one file section to the upload folder and returns where it landed.</summary>
    /// <param name="body">The section's content.</param>
    /// <param name="folder">The upload directory.</param>
    /// <param name="fileName">The name the client gave the file.</param>
    /// <param name="maxFileSize">The largest file accepted, or <see langword="null"/>.</param>
    /// <param name="progress">Where the progress is reported.</param>
    /// <param name="uploadId">The id to report it under, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    private static async Task<Outcome> StoreAsync(
        Stream body,
        string folder,
        string fileName,
        long? maxFileSize,
        UploadProgress progress,
        string? uploadId,
        CancellationToken cancellationToken)
    {
        var temporary = Path.Combine(folder, "tmp" + Convert.ToHexString(RandomNumberGenerator.GetBytes(8)).ToLowerInvariant());

        try
        {
            long size = 0;
            using var hash = MediaCache.CreateContentHash();
            using (var file = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                var buffer = new byte[ChunkSize];
                int read;

                while ((read = await body.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    size += read;

                    if (maxFileSize is { } limit && size > limit)
                    {
                        return Outcome.Refuse(
                            $"File size exceeded maximum allowed size of {limit} bytes.",
                            StatusCodes.Status413PayloadTooLarge);
                    }

                    await file.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                    hash.AppendData(buffer, 0, read);

                    if (uploadId is not null)
                    {
                        progress.Append(uploadId, fileName, read);
                    }
                }
            }

            var name = MediaCache.SanitizeFileName(fileName);
            if (name is "" or "..")
            {
                // The Python version lets its safe join refuse the name, which is what makes ".." an invalid
                // file name rather than a file called "..". The rule is the same; the message says
                // which name was refused.
                return Outcome.Refuse(
                    $"Invalid file name: {name}",
                    StatusCodes.Status400BadRequest);
            }

            var destination = Path.Combine(MediaCache.DirectoryFor(folder, MediaCache.Complete(hash)), name);

            // The same bytes uploaded twice name the same file. The Python version renames over it, which is
            // why a second upload of one file does not fail and does not leave a second copy.
            System.IO.File.Move(temporary, destination, overwrite: true);

            return new Outcome([destination], Refusal: null, StatusCodes.Status200OK);
        }
        finally
        {
            if (System.IO.File.Exists(temporary))
            {
                System.IO.File.Delete(temporary);
            }
        }
    }

    /// <summary>Whether a section is an uploaded file, and what it is called.</summary>
    /// <param name="section">The section.</param>
    /// <param name="fileName">The name the client gave it.</param>
    /// <returns>Whether the section is a file sent in the field an upload uses.</returns>
    private static bool TryReadFile(MultipartSection section, out string fileName)
    {
        fileName = string.Empty;

        if (section.ContentDisposition is not { } disposition
            || !ContentDispositionHeaderValue.TryParse(disposition, out var parsed)
            || !parsed.IsFileDisposition())
        {
            return false;
        }

        var name = parsed.Name.Value;
        if (!string.Equals(name, FileField, StringComparison.Ordinal))
        {
            return false;
        }

        fileName = DecodeFileName(parsed.FileName.HasValue ? parsed.FileName.Value : parsed.FileNameStar.Value);

        return true;
    }

    /// <summary>
    /// The file name a header carries, as the client wrote it.
    /// </summary>
    /// <param name="raw">The name as the header parser read it.</param>
    /// <returns>The name, decoded from UTF-8 when the client sent UTF-8 bytes.</returns>
    /// <remarks>
    /// A browser sends a non-ASCII name as UTF-8 bytes inside a quoted string, and the header parser
    /// reads header bytes as Latin-1 — so a name with an accent arrives as its bytes spelled as
    /// characters. Re-encoding those characters and decoding them as UTF-8 recovers the name; a name
    /// that was ASCII, or that is not valid UTF-8, is left as it arrived.
    /// </remarks>
    private static string DecodeFileName(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
        {
            return string.Empty;
        }

        if (raw.StartsWith("utf-8''", StringComparison.OrdinalIgnoreCase))
        {
            return Uri.UnescapeDataString(raw[7..]);
        }

        try
        {
            var bytes = Encoding.Latin1.GetBytes(raw);
            var decoded = Encoding.UTF8.GetString(bytes);

            return decoded.Contains('\uFFFD', StringComparison.Ordinal) ? raw : decoded;
        }
        catch (ArgumentException)
        {
            return raw;
        }
    }

    /// <summary>Whether a parser failure is the part-header limit being exceeded.</summary>
    /// <param name="error">The failure.</param>
    /// <returns>Whether it is about the headers rather than about the body.</returns>
    /// <remarks>
    /// The reader reports the limit as a message rather than as a type of its own, so the message
    /// is what is left to read — which is also how the Python version decides between 413 and 400:
    /// it looks for "maximum allowed size" in the failure it caught.
    /// </remarks>
    private static bool IsHeaderLimit(InvalidDataException error) =>
        error.Message.Contains("headers length limit", StringComparison.OrdinalIgnoreCase);

    /// <summary>The multipart boundary a content type declares.</summary>
    /// <param name="contentType">The request's content type.</param>
    /// <param name="boundary">The boundary, when there is one.</param>
    /// <returns>Whether the request is a multipart form with a boundary.</returns>
    private static bool TryReadBoundary(string? contentType, out string boundary)
    {
        boundary = string.Empty;

        if (!MediaTypeHeaderValue.TryParse(contentType, out var mediaType)
            || !mediaType.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase)
            || !mediaType.Boundary.HasValue)
        {
            return false;
        }

        boundary = HeaderUtilities.RemoveQuotes(mediaType.Boundary).Value ?? string.Empty;

        return boundary.Length > 0;
    }
}
