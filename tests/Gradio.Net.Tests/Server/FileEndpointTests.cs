using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The file loop as a client sees it: what an upload answers, what a file is served as, and what
/// the application refuses to serve.
/// </summary>
/// <remarks>
/// <para>
/// Every test drives the endpoints over HTTP against a real host, because that is the seam the
/// frontend and <c>gradio_client</c> use: a test that called the reader or the writer directly
/// could pass while the route around it was wrong.
/// </para>
/// <para>
/// The application's upload folder is the test's own directory rather than the process default, so
/// a test can look at what was written — and so two tests cannot see each other's files.
/// </para>
/// </remarks>
public sealed class FileEndpointTests
{
    [Fact]
    public async Task An_uploaded_file_is_stored_in_the_application_folder_and_comes_back_as_a_path()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        var content = "hello upload"u8.ToArray();
        using var body = Multipart(("greeting.txt", content));
        using var response = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paths = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!.AsArray();
        var path = Assert.Single(paths)!.GetValue<string>();

        // The answer is the list of paths, and the path is the one the file endpoint will serve:
        // inside the application's folder, under the name the client gave.
        Assert.True(MediaCache.IsInFolder(path, folder.Folder));
        Assert.Equal("greeting.txt", Path.GetFileName(path));
        Assert.Equal(content, await System.IO.File.ReadAllBytesAsync(path, cancellationToken));

        // The file is the application's to reclaim, which is what the upload endpoint records.
        Assert.Contains(Path.GetFullPath(path), host.Services.GetRequiredService<Blocks>().TempFiles);
    }

    [Fact]
    public async Task An_upload_of_two_files_answers_with_both_paths_in_the_order_they_arrived()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        using var body = Multipart(("first.txt", "1"u8.ToArray()), ("second.txt", "2"u8.ToArray()));
        using var response = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        var paths = JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!.AsArray();

        Assert.Equal(2, paths.Count);
        Assert.Equal("first.txt", Path.GetFileName(paths[0]!.GetValue<string>()));
        Assert.Equal("second.txt", Path.GetFileName(paths[1]!.GetValue<string>()));
    }

    [Fact]
    public async Task Uploading_the_same_bytes_twice_names_one_file()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        var content = "the same bytes"u8.ToArray();

        using var first = Multipart(("same.txt", content));
        using var firstResponse = await host.Client.PostAsync("/gradio_api/upload", first, cancellationToken);

        using var second = Multipart(("same.txt", content));
        using var secondResponse = await host.Client.PostAsync("/gradio_api/upload", second, cancellationToken);

        var a = await SinglePathAsync(firstResponse, cancellationToken);
        var b = await SinglePathAsync(secondResponse, cancellationToken);

        // The address is the content's hash, so the second upload is the first file rather than a
        // copy of it — which is what makes uploading the same file twice cheap.
        Assert.Equal(a, b);
        Assert.Single(folder.Files());
    }

    [Fact]
    public async Task A_request_that_carries_no_multipart_body_is_refused()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        using var content = new StringContent("not a file", Encoding.UTF8, "text/plain");
        using var response = await host.Client.PostAsync("/gradio_api/upload", content, cancellationToken);

        // The refusal is a `detail` document, which is how the Python version answers a request it refuses
        // before it reads anything — the plain text is for a parser failure.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Invalid content type.",
            JsonNode.Parse(await response.Content.ReadAsStringAsync(cancellationToken))!["detail"]!.GetValue<string>());
        Assert.Empty(folder.Files());
    }

    [Fact]
    public async Task More_files_than_an_upload_may_carry_are_refused()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        var files = Enumerable.Range(0, 1001).Select(index => ($"f{index}.txt", new byte[] { 1 })).ToArray();
        using var body = Multipart(files);
        using var response = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains(
            "Too many files. Maximum number of files is 1000.",
            await response.Content.ReadAsStringAsync(cancellationToken),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_file_larger_than_the_server_would_allow_by_default_is_accepted()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        // The server caps a request body at 30 MB unless it is told otherwise, and an application
        // that set no limit accepts a file of any size. The cap has to be lifted for that to be
        // true — this file is past it.
        var content = new byte[31 * 1024 * 1024];
        using var body = Multipart(("large.bin", content));
        using var response = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var path = await SinglePathAsync(response, cancellationToken);
        Assert.Equal(content.Length, new FileInfo(path).Length);
    }

    [Fact]
    public async Task A_part_whose_headers_are_too_long_is_refused_as_too_large()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        // Headers are read into memory before the file is, so they have a limit of their own —
        // and the Python version answers that limit with the same status as a file's, because both are
        // "too large".
        using var body = Multipart((new string('a', 9000) + ".txt", "content"u8.ToArray()));
        using var response = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
        Assert.Equal(
            "Headers exceeded maximum allowed size of 8192 bytes.",
            await response.Content.ReadAsStringAsync(cancellationToken));
        Assert.Empty(folder.Files());
    }

    [Fact]
    public async Task A_file_over_the_limit_is_refused_and_nothing_of_it_is_kept()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken, maxFileSize: 16);

        using var body = Multipart(("too-big.bin", new byte[1024]));
        using var response = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        Assert.Equal(HttpStatusCode.RequestEntityTooLarge, response.StatusCode);
        Assert.Contains("16 bytes", await response.Content.ReadAsStringAsync(cancellationToken), StringComparison.Ordinal);

        // The limit is applied while the file is being read, so what was written before the limit
        // was crossed is not left behind as a half file.
        Assert.Empty(folder.Files());
    }

    [Fact]
    public async Task A_file_that_fits_the_limit_is_accepted()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken, maxFileSize: 1024);

        using var body = Multipart(("fits.bin", new byte[1024]));
        using var response = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(folder.Files());
    }

    [Fact]
    public async Task A_file_is_written_to_disk_while_it_is_still_arriving()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        // The body is a multipart upload whose content stops halfway and waits for the test. What
        // is asserted while it waits is the point of the test: the bytes that have arrived are on
        // disk, so a file of any size costs one read's worth of memory rather than its own size.
        var head = new byte[128 * 1024];
        var tail = new byte[128 * 1024];
        Random.Shared.NextBytes(head);
        Random.Shared.NextBytes(tail);

        using var body = new GatedUpload(head.Length, ("big.bin", [.. head, .. tail]));
        var upload = host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);

        var written = await Eventually.IsTrueAsync(
            () => Task.FromResult(TemporaryFiles(folder).Any(file => new FileInfo(file).Length > 0)),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(written, "the server did not write any of the file to disk while it was arriving");

        // Part of the file, not all of it: the rest has not been sent yet.
        var partial = TemporaryFiles(folder).Sum(file => new FileInfo(file).Length);
        Assert.InRange(partial, 1, head.Length + tail.Length - 1);

        body.Release();

        using var response = await upload;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var path = await SinglePathAsync(response, cancellationToken);
        var stored = await System.IO.File.ReadAllBytesAsync(path, cancellationToken);
        Assert.Equal(head.Length + tail.Length, stored.Length);
        Assert.Equal(head, stored[..head.Length]);
        Assert.Equal(tail, stored[head.Length..]);

        // The temporary file the bytes were written to is gone once the file has its address.
        Assert.Empty(TemporaryFiles(folder));
    }

    [Fact]
    public async Task The_progress_of_an_upload_is_reported_on_its_own_stream()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        var head = new byte[128 * 1024];
        var tail = new byte[128 * 1024];

        using var body = new GatedUpload(head.Length, ("reported.bin", [.. head, .. tail]));
        var upload = host.Client.PostAsync("/gradio_api/upload?upload_id=watch-me", body, cancellationToken);

        // The client opens the stream before it starts sending, so the stream may arrive first and
        // wait for the upload to be announced.
        await using var progress = await QueueSession.OpenAsync(
            host.Client,
            "/gradio_api/upload_progress?upload_id=watch-me",
            cancellationToken);

        var update = await progress.UntilAsync(
            message => QueueSession.Kind(message) == "update",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(update);
        Assert.Equal("reported.bin", update["orig_name"]!.GetValue<string>());
        Assert.True(update["chunk_size"]!.GetValue<long>() > 0);

        body.Release();

        var done = await progress.UntilAsync(
            message => QueueSession.Kind(message) == "done",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        // The upload says when it is over, which is what tells the client to stop drawing progress.
        Assert.NotNull(done);

        using var response = await upload;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task An_upload_of_several_files_reports_one_file_at_a_time()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        var first = new byte[128 * 1024];
        var second = new byte[128 * 1024];

        // Both files are sent and the last one stops part-way, so by the time the test reads the
        // stream both have been through the tracker and neither has been reported yet.
        using var body = new GatedUpload(second.Length, ("first.bin", first), ("second.bin", second));
        var upload = host.Client.PostAsync("/gradio_api/upload?upload_id=batch", body, cancellationToken);

        var bothStored = await Eventually.IsTrueAsync(
            () => Task.FromResult(
                folder.Files().Any(file => Path.GetFileName(file) == "first.bin")
                && TemporaryFiles(folder).Any(file => new FileInfo(file).Length > 0)),
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.True(bothStored, "both files should have been read before the stream was opened");

        await using var progress = await QueueSession.OpenAsync(
            host.Client,
            "/gradio_api/upload_progress?upload_id=batch",
            cancellationToken);

        var update = await progress.UntilAsync(
            message => QueueSession.Kind(message) == "update",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(update);

        // The file being reported is the one still arriving. The one before it is not reported
        // after it: a client told "the second file is at 128 KB" must not then be told "the first
        // file is at 128 KB", which is what keeping both would do.
        Assert.Equal("second.bin", update["orig_name"]!.GetValue<string>());

        body.Release();

        var done = await progress.UntilAsync(
            message => QueueSession.Kind(message) == "done",
            TimeSpan.FromSeconds(15),
            cancellationToken);

        Assert.NotNull(done);

        // Nothing was ever said about the file that had already arrived.
        Assert.DoesNotContain(
            progress.Messages,
            message => message["orig_name"]?.GetValue<string>() == "first.bin");

        using var response = await upload;
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task A_progress_request_that_names_no_upload_is_missing()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        using var response = await host.Client.GetAsync("/gradio_api/upload_progress", cancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task A_progress_request_for_an_upload_nobody_is_making_is_missing()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        using var response = await host.Client.GetAsync(
            "/gradio_api/upload_progress?upload_id=nobody",
            cancellationToken);

        // It waits for an upload to be announced, and gives up rather than waiting for ever.
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task A_file_is_served_at_the_address_its_value_carries()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = folder.Write("served.txt", "served content");

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("served content", await response.Content.ReadAsStringAsync(cancellationToken));
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);

        // A text type carries its charset, which is what the HTTP layer the Python version is built on
        // appends; a browser told "text/plain" without one guesses.
        Assert.Equal("utf-8", response.Content.Headers.ContentType?.CharSet);
        Assert.Equal("inline", response.Content.Headers.ContentDisposition?.DispositionType);
    }

    [Fact]
    public async Task A_file_is_served_by_the_path_form_of_the_route_too()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = folder.Write("both-forms.txt", "same file");

        using var response = await host.Client.GetAsync(
            "/gradio_api/file/" + Uri.EscapeDataString(path).Replace("%2F", "/", StringComparison.Ordinal),
            cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("same file", await response.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task A_file_whose_name_has_a_space_in_it_is_served_by_the_address_its_value_carries()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = folder.Write("a file with spaces.txt", "spaces");

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("spaces", await response.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task A_file_whose_name_is_not_ascii_survives_the_upload_and_is_served()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        // A browser sends a non-ASCII name as UTF-8 bytes inside a quoted string, and the header
        // parser reads header bytes as Latin-1 — so the name arrives spelled as its bytes. The
        // upload decodes it back, which is why the stored file has the name the client meant.
        using var body = Multipart(("naïve.txt", "accented"u8.ToArray()));
        using var upload = await host.Client.PostAsync("/gradio_api/upload", body, cancellationToken);
        var path = await SinglePathAsync(upload, cancellationToken);

        Assert.Equal("naïve.txt", Path.GetFileName(path));

        using var served = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        Assert.Equal(HttpStatusCode.OK, served.StatusCode);
        Assert.Equal("accented", await served.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task A_file_whose_name_holds_a_percent_escape_is_served_by_its_own_address()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        // "%41" in a name is three characters, not an escape for "A". The address encodes them
        // once and the route decodes them once, so the file that comes back is the one that went in
        // — decoding twice would look for a file called "aAb.txt".
        var path = folder.Write("a%41b.txt", "literal percent");

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("literal percent", await response.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task A_range_request_gets_the_bytes_it_asked_for()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = folder.Write("ranged.txt", "0123456789abcdef");

        using var request = new HttpRequestMessage(HttpMethod.Get, MediaCache.UrlOf(path));
        request.Headers.Range = new RangeHeaderValue(4, 9);

        using var response = await host.Client.SendAsync(request, cancellationToken);

        Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);
        Assert.Equal("456789", await response.Content.ReadAsStringAsync(cancellationToken));
        Assert.Equal("bytes 4-9/16", response.Content.Headers.ContentRange?.ToString());
        Assert.Equal("bytes", response.Headers.AcceptRanges.Single());
    }

    [Fact]
    public async Task A_range_that_starts_past_the_end_is_refused_with_the_size()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = folder.Write("short.txt", "0123456789");

        using var request = new HttpRequestMessage(HttpMethod.Get, MediaCache.UrlOf(path));
        request.Headers.Range = new RangeHeaderValue(100, 200);

        using var response = await host.Client.SendAsync(request, cancellationToken);

        // The Python version answers 416 and says how long the file is. (The Python version 6.30.0 clamps the end to
        // the length instead, which is one byte too far and leaves the response unfinished; this is
        // the rule the release after it fixed it to.)
        Assert.Equal(HttpStatusCode.RequestedRangeNotSatisfiable, response.StatusCode);
        Assert.Equal("bytes */10", response.Content.Headers.ContentRange?.ToString());
    }

    [Fact]
    public async Task A_range_that_ends_past_the_end_is_clamped_to_the_last_byte()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = folder.Write("clamped.txt", "0123456789");

        using var request = new HttpRequestMessage(HttpMethod.Get, MediaCache.UrlOf(path));
        request.Headers.Range = new RangeHeaderValue(8, 200);

        using var response = await host.Client.SendAsync(request, cancellationToken);

        Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);
        Assert.Equal("89", await response.Content.ReadAsStringAsync(cancellationToken));
        Assert.Equal("bytes 8-9/10", response.Content.Headers.ContentRange?.ToString());
    }

    [Fact]
    public async Task A_head_request_answers_with_the_size_and_no_body()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = folder.Write("headed.txt", "0123456789");

        using var request = new HttpRequestMessage(HttpMethod.Head, MediaCache.UrlOf(path));
        using var response = await host.Client.SendAsync(request, cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(10, response.Content.Headers.ContentLength);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync(cancellationToken));
    }

    [Fact]
    public async Task A_file_outside_the_application_folder_is_refused()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = elsewhere.Write("secret.txt", "not the application's");

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Contains("File not allowed", await response.Content.ReadAsStringAsync(cancellationToken), StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_path_that_names_a_location_is_refused_rather_than_followed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        foreach (var path in new[] { "/etc/passwd", "http://example.com/a.txt", "//example.com/a.txt" })
        {
            using var response = await host.Client.GetAsync(
                "/gradio_api/file=" + Uri.EscapeDataString(path),
                cancellationToken);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }

    [Fact]
    public async Task A_directory_is_refused()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(folder.Folder), cancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task A_file_a_browser_would_run_is_served_as_a_download()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = folder.Write("page.html", "<script>alert(1)</script>");

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        // An inline text/html is a script the application's own origin runs, so a type that is not
        // one of the safe ones is served as a download of octet-stream instead.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/octet-stream", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("attachment", response.Content.Headers.ContentDisposition?.DispositionType);
        Assert.Equal("page.html", response.Content.Headers.ContentDisposition?.FileName?.Trim('"'));
    }

    [Fact]
    public async Task A_name_that_cannot_be_written_as_it_is_is_carried_in_the_encoded_form()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);
        var path = folder.Write("résumé.html", "cv");

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        // The header has to be ASCII, so a name with an accent goes in the RFC 5987 form, which a
        // browser reads back as the same name.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            "filename*=utf-8''",
            response.Content.Headers.ContentDisposition?.ToString() ?? string.Empty,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task A_file_an_application_allowed_is_served_from_outside_its_own_folder()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken, allowedPaths: [elsewhere.Folder]);
        var path = elsewhere.Write("shared.txt", "an operator said this is fine");

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("an operator said this is fine", await response.Content.ReadAsStringAsync(cancellationToken));
    }

    [Fact]
    public async Task A_file_an_application_blocked_is_refused_even_inside_its_own_folder()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        var kept = Path.Combine(folder.Folder, "kept");
        Directory.CreateDirectory(kept);

        await using var host = await StartAsync(folder, cancellationToken, blockedPaths: [kept]);
        var path = Path.Combine(kept, "private.txt");
        await System.IO.File.WriteAllTextAsync(path, "not for the browser", cancellationToken);

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task A_path_that_is_allowed_and_blocked_at_once_is_refused()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        using var elsewhere = new TempFolder();

        // The same directory on both lists. The Python version's block takes precedence, because a block that
        // an allow could undo would not be a block.
        await using var host = await StartAsync(
            folder,
            cancellationToken,
            allowedPaths: [elsewhere.Folder],
            blockedPaths: [elsewhere.Folder]);

        var path = elsewhere.Write("contested.txt", "which list wins");

        using var response = await host.Client.GetAsync(MediaCache.UrlOf(path), cancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task A_proxy_url_is_refused_because_nothing_was_loaded()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var folder = new TempFolder();
        await using var host = await StartAsync(folder, cancellationToken);

        using var response = await host.Client.GetAsync("/gradio_api/proxy=http://example.com/a.txt", cancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("cannot be proxied", await response.Content.ReadAsStringAsync(cancellationToken), StringComparison.Ordinal);
    }

    private static async Task<GradioTestHost> StartAsync(
        TempFolder folder,
        CancellationToken cancellationToken,
        long? maxFileSize = null,
        string[]? allowedPaths = null,
        string[]? blockedPaths = null) =>
        await GradioTestHost.StartStandaloneAsync(null, cancellationToken, blocks =>
        {
            blocks.UploadFolder = folder.Folder;
            blocks.MaxFileSize = maxFileSize;
            blocks.AllowedPaths = allowedPaths ?? [];
            blocks.BlockedPaths = blockedPaths ?? [];
        });

    /// <summary>The one path an upload answered with.</summary>
    /// <param name="response">The upload's answer.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The path.</returns>
    private static async Task<string> SinglePathAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return Assert.Single(JsonNode.Parse(body)!.AsArray())!.GetValue<string>();
    }

    /// <summary>The files the upload reader is writing into the folder right now.</summary>
    /// <param name="folder">The application's upload folder.</param>
    /// <returns>The temporary files, by path.</returns>
    private static IReadOnlyList<string> TemporaryFiles(TempFolder folder) =>
        [.. folder.Files().Where(file => Path.GetFileName(file).StartsWith("tmp", StringComparison.Ordinal))];

    /// <summary>A multipart body carrying files in the field an upload reads.</summary>
    /// <param name="files">The name and content of each file.</param>
    /// <returns>The body.</returns>
    private static MultipartFormDataContent Multipart(params (string Name, byte[] Content)[] files)
    {
        var body = new MultipartFormDataContent();

        foreach (var (name, content) in files)
        {
            var part = new ByteArrayContent(content);
            part.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            body.Add(part, "files", name);
        }

        return body;
    }

    /// <summary>
    /// A multipart upload that stops part-way and waits, so that what the server did with the bytes
    /// that had arrived can be observed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Written by hand rather than assembled from <see cref="MultipartFormDataContent"/> because the
    /// body has to be produced in two parts with a pause between them, and a content that writes
    /// itself is the only way to pause inside one.
    /// </para>
    /// <para>
    /// The pause is inside the last file, so a body of several files can be used to observe what
    /// happens once the first one is complete: by the time the pause is reached, every file before
    /// the last has been read and the last has been read up to the pause.
    /// </para>
    /// </remarks>
    private sealed class GatedUpload : HttpContent
    {
        private const string Boundary = "gradio-net-test-boundary";

        private readonly (string Name, byte[] Content)[] _files;
        private readonly int _gateAfter;
        private readonly TaskCompletionSource _released = new(TaskCreationOptions.RunContinuationsAsynchronously);

        internal GatedUpload(int gateAfter, params (string Name, byte[] Content)[] files)
        {
            Assert.NotEmpty(files);

            _files = files;
            _gateAfter = gateAfter;

            Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data")
            {
                Parameters = { new NameValueHeaderValue("boundary", Boundary) },
            };
        }

        /// <summary>Lets the rest of the body through.</summary>
        internal void Release() => _released.TrySetResult();

        /// <inheritdoc />
        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            for (var index = 0; index < _files.Length; index++)
            {
                var (name, content) = _files[index];

                await stream.WriteAsync(PartHeader(name));

                var last = index == _files.Length - 1;
                var gate = last ? Math.Clamp(_gateAfter, 0, content.Length) : content.Length;

                await stream.WriteAsync(content.AsMemory(0, gate));

                if (last)
                {
                    await stream.FlushAsync();
                    await _released.Task;
                }

                await stream.WriteAsync(content.AsMemory(gate));
                await stream.WriteAsync("\r\n"u8.ToArray());
            }

            await stream.WriteAsync(Encoding.UTF8.GetBytes($"--{Boundary}--\r\n"));
        }

        /// <inheritdoc />
        /// <returns>
        /// <see langword="false"/>, which makes the request a chunked one: the server is told no
        /// length up front, so it cannot decide to hold the body somewhere until it has all of it.
        /// </returns>
        protected override bool TryComputeLength(out long length)
        {
            length = 0;

            return false;
        }

        /// <summary>The bytes that open one part.</summary>
        /// <param name="name">The name the client gives the file.</param>
        /// <returns>The part's header, boundary included.</returns>
        private static byte[] PartHeader(string name) =>
            Encoding.UTF8.GetBytes(
                $"--{Boundary}\r\n"
                + $"Content-Disposition: form-data; name=\"files\"; filename=\"{name}\"\r\n"
                + "Content-Type: application/octet-stream\r\n\r\n");
    }
}
