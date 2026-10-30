namespace Gradio.Net.Tests.Server;

/// <summary>
/// A directory of a test's own, removed when the test ends.
/// </summary>
/// <remarks>
/// The media tests need one twice over — as the application's upload folder, and as somewhere to
/// keep the file a component is given — so it lives here rather than in the first test that needed
/// it. A test that deletes the directory itself still disposes cleanly: the sweep it may be
/// exercising is free to have removed what was in it.
/// </remarks>
internal sealed class TempFolder : IDisposable
{
    private readonly string _path = Path.Combine(
        Path.GetTempPath(),
        "gradio-net-tests",
        Guid.NewGuid().ToString("N"));

    internal TempFolder() => Directory.CreateDirectory(_path);

    /// <summary>The directory's absolute path.</summary>
    internal string Folder => _path;

    /// <summary>Writes a file into the directory.</summary>
    /// <param name="name">The file's name.</param>
    /// <param name="content">The bytes to write.</param>
    /// <returns>The file's absolute path.</returns>
    internal string Write(string name, ReadOnlySpan<byte> content)
    {
        var path = System.IO.Path.Combine(_path, name);
        using var stream = System.IO.File.Create(path);
        stream.Write(content);

        return path;
    }

    /// <summary>Writes a text file into the directory.</summary>
    /// <param name="name">The file's name.</param>
    /// <param name="content">The text to write.</param>
    /// <returns>The file's absolute path.</returns>
    internal string Write(string name, string content) =>
        Write(name, System.Text.Encoding.UTF8.GetBytes(content));

    /// <summary>Writes a file, dated as if it had been written <paramref name="age"/> ago.</summary>
    /// <param name="name">The file's name.</param>
    /// <param name="age">How long ago the file was last written.</param>
    /// <returns>The file's absolute path.</returns>
    /// <remarks>
    /// The age is what a sweep measures, so the test sets it rather than waiting for it: a file
    /// that had to age for an hour would make the test an hour long.
    /// </remarks>
    internal string Write(string name, TimeSpan age)
    {
        var path = Write(name, "temporary");
        System.IO.File.SetLastWriteTimeUtc(path, System.DateTime.UtcNow - age);

        return path;
    }

    /// <summary>Every file under the directory, by path.</summary>
    /// <returns>The files, or an empty list when the directory holds none.</returns>
    internal IReadOnlyList<string> Files() =>
        Directory.Exists(_path) ? [.. Directory.GetFiles(_path, "*", SearchOption.AllDirectories)] : [];

    public void Dispose()
    {
        try
        {
            Directory.Delete(_path, recursive: true);
        }
        catch (DirectoryNotFoundException)
        {
            // Something the test was exercising deleted it, which is a result rather than a fault.
        }
    }
}
