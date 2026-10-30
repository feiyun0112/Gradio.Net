using System.Net;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Server;

/// <summary>
/// The five helpers that hand a handler a path to a sample file, and the one that fetches an address
/// the caller named.
/// </summary>
/// <remarks>
/// <para>
/// What is asserted is what a handler would rely on: the file is there when the call returns, its
/// bytes are the Python version's, and the name the helper was asked for is the name that arrives.
/// The random form is asserted as "one of the files that directory has", which is the whole of its
/// contract.
/// </para>
/// <para>
/// The remote form is the one place this differs from the Python version — it returns a local file
/// rather than the address — so what it does is pinned here: one request, the bytes that came back
/// on disk, and a refusal for anything that is not http(s).
/// </para>
/// </remarks>
public sealed class MediaHelperTests
{
    [Theory]
    [InlineData("images", "cheetah1.jpg")]
    [InlineData("videos", "a.mp4")]
    [InlineData("audio", "cantina.wav")]
    [InlineData("models3d", "Bunny.obj")]
    [InlineData("data", "titanic.csv")]
    public void Each_helper_hands_over_a_file_from_its_own_directory(string directory, string name)
    {
        var path = Resolve(directory, name);

        Assert.True(System.IO.File.Exists(path), $"'{path}' is not a file.");
        Assert.Equal(
            Path.Combine(MediaCache.DefaultFolder, "media_assets", directory, name),
            path);

        // The Python version's sample, byte for byte: the tree is vendored, so its size is the file's size.
        Assert.True(new FileInfo(path).Length > 0, $"'{path}' is empty.");
    }

    [Fact]
    public void A_helper_asked_for_no_name_answers_with_one_of_its_files()
    {
        var path = Media.GetImage();

        Assert.True(System.IO.File.Exists(path), $"'{path}' is not a file.");

        // Which one is not the contract; that it is one of them is. The list comes from the
        // assembly's resources, so a file added to the tree is a file this accepts.
        var expected = Directory.GetFiles(
            Path.Combine(MediaCache.DefaultFolder, "media_assets", "images")).Select(Path.GetFileName);

        Assert.Contains(Path.GetFileName(path), expected);
    }

    [Fact]
    public void A_name_the_tree_does_not_carry_is_refused()
    {
        var error = Assert.Throws<FileNotFoundException>(() => Media.GetImage("no-such-picture.jpg"));

        // The Python version's sentence, with the path this framework would have written it to.
        Assert.Contains("Media file not found:", error.Message, StringComparison.Ordinal);
        Assert.Contains("no-such-picture.jpg", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_remote_address_is_fetched_once_and_answered_with_a_local_file()
    {
        var service = new FakeService().Answering("the bytes of a picture");

        var path = Media.GetImage("https://example.test/pictures/cheetah.jpg", service.Client());

        // One request, and no retry loop: what a handler waits for is one fetch.
        var call = Assert.Single(service.Calls);
        Assert.Equal("GET", call.Method);
        Assert.Equal("https://example.test/pictures/cheetah.jpg", call.Address);

        Assert.True(System.IO.File.Exists(path), $"'{path}' is not a file.");
        Assert.Equal("the bytes of a picture", System.IO.File.ReadAllText(path));

        // The name the address ended with is the name the file keeps, so that a handler which logs
        // the path logs something it can recognise.
        Assert.Equal("cheetah.jpg", Path.GetFileName(path));
    }

    [Fact]
    public void An_address_that_answered_with_a_failure_is_refused()
    {
        var service = new FakeService().Refusing(HttpStatusCode.NotFound, "{\"detail\":\"Not Found\"}");

        Assert.Throws<HttpRequestException>(
            () => Media.GetFile("https://example.test/missing.csv", service.Client()));
    }

    [Theory]
    [InlineData("ftp://example.test/pictures/cheetah.jpg")]
    [InlineData("file:///etc/passwd")]
    [InlineData("//example.test/pictures/cheetah.jpg")]
    public void An_address_that_is_not_http_is_refused(string address)
    {
        // The rule for this helper: the address is the caller's, and what may be
        // fetched from it is http(s) and nothing else — a protocol-relative address included,
        // because a server has no page to resolve it against.
        var error = Assert.Throws<HttpRequestException>(() => Media.GetImage(address));

        Assert.Contains("http", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Asks one of the helpers for a sample, by the directory it lives in.</summary>
    /// <param name="directory">The directory, as the tree spells it.</param>
    /// <param name="name">The file's name.</param>
    /// <returns>The path the helper answered with.</returns>
    private static string Resolve(string directory, string name) => directory switch
    {
        "images" => Media.GetImage(name),
        "videos" => Media.GetVideo(name),
        "audio" => Media.GetAudio(name),
        "models3d" => Media.GetModel3d(name),
        _ => Media.GetFile(name),
    };
}
