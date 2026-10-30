using System.Text;
using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// What a chat records when a user rates a message: one row per like or dislike, in a log the
/// application keeps.
/// </summary>
/// <remarks>
/// <para>
/// The Python version's <c>ChatCSVLogger</c>, and it is a different file from the one an
/// <see cref="Interface"/> writes: a chat has no sample to record, only a conversation and the
/// position in it that was rated, so the row says which conversation, which message, how it was
/// rated and when. The file is <c>log.csv</c> in the flagging directory, and its header is written
/// once.
/// </para>
/// <para>
/// The conversation is written as JSON in one cell, which is what makes the log readable by a
/// program rather than only by a person: the messages keep their structure, so a tool that reads
/// the log back gets the same conversation the browser showed.
/// </para>
/// </remarks>
public sealed class ChatCsvLogger
{
    private readonly object _gate = new();
    private string _directory = string.Empty;

    /// <summary>The directory the log goes into, once <see cref="Setup"/> has been given one.</summary>
    public string FlaggingDir => _directory;

    /// <summary>The file the rows are being written to, once one has been created.</summary>
    public string LogFile => Path.Combine(_directory, "log.csv");

    /// <summary>Tells the logger where the log goes, and makes sure the directory is there.</summary>
    /// <param name="directory">The directory the application asked for.</param>
    /// <exception cref="ArgumentException"><paramref name="directory"/> is empty.</exception>
    public void Setup(string directory)
    {
        ArgumentException.ThrowIfNullOrEmpty(directory);

        _directory = directory;
        Directory.CreateDirectory(directory);
    }

    /// <summary>Records one rating of one message.</summary>
    /// <param name="like">Which message was rated, and how.</param>
    /// <param name="messages">The conversation as it stood when it was rated.</param>
    /// <returns>How many rows the log holds, header excluded.</returns>
    /// <exception cref="InvalidOperationException">No directory has been given.</exception>
    /// <remarks>
    /// <para>
    /// The rating is spelled the way the feedback options are: <c>Like</c> and <c>Dislike</c> for the
    /// two a chat offers by default, and the option's own value for any other — which is why the
    /// value is written as it arrives rather than as a flag.
    /// </para>
    /// <para>
    /// The Python version writes four cells under five headers — the timestamp column has no value
    /// of its own in the row — and this writes four as well. It is a defect, and it is reproduced
    /// because the file is the artifact: a log written here has to be the log an application
    /// ported from the Python version already has, or the port changes its data.
    /// </para>
    /// </remarks>
    public int Flag(LikeData like, object? messages)
    {
        ArgumentNullException.ThrowIfNull(like);

        if (_directory.Length == 0)
        {
            throw new InvalidOperationException(
                "A chat's flagging logger has no directory; call Setup before a rating can be recorded.");
        }

        lock (_gate)
        {
            var path = LogFile;
            var isNew = !System.IO.File.Exists(path);

            if (isNew)
            {
                System.IO.File.AppendAllText(
                    path,
                    CsvText.Row(["conversation", "index", "value", "flag", "timestamp"]),
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            }

            System.IO.File.AppendAllText(
                path,
                CsvText.Row(
                [
                    Conversation(messages),
                    like.Index,
                    Feedback(like.Liked),
                    CsvText.Timestamp(),
                ]),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            return RowCount(path);
        }
    }

    /// <summary>The conversation as one cell: the messages as the browser sent them.</summary>
    /// <param name="messages">The conversation.</param>
    /// <returns>The JSON text.</returns>
    private static string Conversation(object? messages) => messages switch
    {
        null => PythonJson.Dump(null),
        JsonNode node => PythonJson.Dump(node),
        IReadOnlyList<ChatMessage> chat => PythonJson.Dump(new JsonArray([.. chat.Select(m => m.ToJson())])),
        _ => PythonJson.Dump(JsonValues.FromAnyObject(messages)),
    };

    /// <summary>How a rating is spelled in the log.</summary>
    /// <param name="liked">The rating: a boolean, or the value of the option that was chosen.</param>
    /// <returns>The text, or <see langword="null"/> for a rating that says nothing.</returns>
    /// <remarks>
    /// The cell is written through <see cref="CsvText.Row"/>, which is what makes a rating that a
    /// spreadsheet could read as a formula safe; nothing is escaped here. A rating that is missing
    /// is left as <see langword="null"/> rather than turned into an empty cell, because the Python
    /// version passes it straight through and the cell writer spells a missing value its own way.
    /// </remarks>
    private static string? Feedback(object? liked) => liked switch
    {
        null => null,
        true => "Like",
        false => "Dislike",
        _ => liked.ToString(),
    };

    /// <summary>How many rows the log holds.</summary>
    /// <param name="path">The file.</param>
    /// <returns>The number of rows after the header.</returns>
    private static int RowCount(string path)
    {
        var rows = System.IO.File.ReadAllLines(path);

        return Math.Max(0, rows.Length - 1);
    }
}
