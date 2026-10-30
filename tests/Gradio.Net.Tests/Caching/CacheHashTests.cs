using System.Text.Json.Nodes;
using Gradio.Net;
using Xunit;

namespace Gradio.Net.Tests.Caching;

/// <summary>
/// The digest a value is cached under, held to the Python version's own: every expected digest in
/// this file was produced by running the Python version's <c>cache_hash</c> on the same value, so a
/// difference here is a difference between the two sides rather than a stale expectation.
/// </summary>
/// <remarks>
/// The values are the ones a handler actually works with — what a component's preprocessing makes of
/// a request — which is why they are CLR values of the corresponding type rather than JSON. A JSON
/// number is read as a double on this side, so an integral number's digest is the double's; that
/// difference is spelled out in <see cref="CacheHash"/>'s remarks.
/// </remarks>
public sealed class CacheHashTests
{
    [Theory]
    [InlineData(null, "dc937b59892604f5a86ac96936cd7ff09e25f18ae6b758e8014a24c7fa039e91")]
    [InlineData(true, "3cbc87c7681f34db4617feaa2c8801931bc5e42d8d0f560e756dd4cd92885f18")]
    [InlineData(false, "60a33e6cf5151f2d52eddae9685cfa270426aa89d8dbc7dfb854606f1d1a40fe")]
    [InlineData(0, "5feceb66ffc86f38d952786c6d696c79c2dbc239dd4e91b46729d73a27fb57e9")]
    [InlineData(42, "73475cb40a568e8da8a045ced110137e159f890ac4da883b6b17dc651b3a8049")]
    [InlineData(-7, "a770d3270c9dcdedf12ed9fd70444f7c8a95c26cae3cae9bd867499090a2f14b")]
    [InlineData(1234567890123L, "bca2b41a2b25e137c83fee346af7bd1e0f52bd560583ca07a1b42f9944c5c50b")]
    [InlineData(1.0, "d0ff5974b6aa52cf562bea5921840c032a860a91a3512f7fe8f768f6bbe005f6")]
    [InlineData(1.5, "9f29a130438b81170b92a42650f9a94291ecad60bd47af2a3886e75f7f728725")]
    [InlineData(1e-7, "e485fac25775a7da830698e7daac582737fc8b43ba4b351467a16cf8ed83122a")]
    [InlineData(1e20, "7c18c9fbdcc8281573e9db9e04f04c3790b10696f3706f0f03fa87427d33e28b")]
    [InlineData("", "6f49cdbd80e1b95d5e6427e1501fc217790daee87055fa5b4e71064288bddede")]
    [InlineData("hello", "d543699194a3343443ab84395c0464b018f12e31df1b5e829d65c4440e90b9a5")]
    [InlineData("it's", "e8a14e7df117da8182c2c867aef6f99b3d4a436044f60d375fe7db38695a3777")]
    [InlineData("héllo 世界", "d7841964b695056f2619934ff246e0956f38a099e8150a73b701c89df0dae9cb")]
    public void A_scalar_hashes_the_way_upstream_hashes_it(object? value, string digest) =>
        Assert.Equal(digest, CacheHash.Of(value));

    [Fact]
    public void An_empty_sequence_and_an_empty_mapping_are_different_keys()
    {
        Assert.Equal("d27cbf07d603f889c14a4f228ccd5996f02e0244ce4c890558b9ff483be3bb41", CacheHash.Of(Array.Empty<object?>()));
        Assert.Equal("ad1dc21dfec58caffbb688952139a162b5f2c9b8b3027a521f30e9e752674dd4", CacheHash.Of(new Dictionary<string, object?>()));
    }

    [Fact]
    public void A_sequence_hashes_its_values_in_order()
    {
        object?[] values = [1, 2];

        Assert.Equal("69de305a74ffb74c4ca80f7a839d0485a4161a8cdd4bf828a37b87237e6ce983", CacheHash.Of(values));
        Assert.NotEqual(CacheHash.Of(values), CacheHash.Of(new object?[] { 2, 1 }));
    }

    [Fact]
    public void A_nested_value_hashes_each_level_the_same_way()
    {
        object?[] values = [1, new object?[] { "a" }, null];

        Assert.Equal("6f74a1af3fc72f726dbbc1e91202534bbad5ac654e5c9a29cd96bf11d47af234", CacheHash.Of(values));
    }

    [Fact]
    public void A_mapping_hashes_by_content_rather_than_by_the_order_it_was_built_in()
    {
        var ascending = new Dictionary<string, object?> { ["a"] = 2, ["b"] = 1 };
        var descending = new Dictionary<string, object?> { ["b"] = 1, ["a"] = 2 };

        Assert.Equal("dc4cdfd19c878ec74ca5f85baf78adab7d5ed831839d8232e5a09cc1eb3a3462", CacheHash.Of(ascending));
        Assert.Equal(CacheHash.Of(ascending), CacheHash.Of(descending));
    }

    [Fact]
    public void A_mapping_inside_a_sequence_is_spelled_as_a_mapping()
    {
        var value = new Dictionary<string, object?> { ["k"] = new object?[] { 1, new Dictionary<string, object?> { ["z"] = true } } };

        Assert.Equal("b16d5b83fa1dc1c6291b7a8ad57620f077c492ca8410fec6844be9bfde4be1d6", CacheHash.Of(value));
    }

    [Fact]
    public void A_set_hashes_by_its_members_rather_than_by_their_order()
    {
        var first = new HashSet<object?> { "b", "a" };
        var second = new HashSet<object?> { "a", "b" };

        // The Python version spells a set with its members sorted, so two sets built in a different order are
        // one key rather than two.
        Assert.Equal(CacheHash.Of(first), CacheHash.Of(second));
        Assert.NotEqual(CacheHash.Of(first), CacheHash.Of(new object?[] { "a", "b" }));
    }

    [Fact]
    public void Bytes_hash_by_their_content()
    {
        Assert.Equal("1069a9932784e43022aed75716d97f627edcc1fc227cddea1145673ece433fa3", CacheHash.Of(new byte[] { 0, 1, 2 }));
        Assert.NotEqual(CacheHash.Of(new byte[] { 0, 1, 2 }), CacheHash.Of(new byte[] { 0, 1, 3 }));
    }

    [Fact]
    public void A_json_document_hashes_the_way_the_value_it_describes_does()
    {
        var document = JsonNode.Parse("""{"b": 1, "a": [true, null, "x"]}""");

        // A document and the CLR values it describes are one key: a handler that reads its input
        // from JSON and one that was handed the values must not fill two caches.
        Assert.Equal(
            CacheHash.Of(new Dictionary<string, object?>
            {
                ["a"] = new object?[] { true, null, "x" },
                ["b"] = 1,
            }),
            CacheHash.Of(document));
    }

    [Fact]
    public void An_object_with_no_state_is_refused_rather_than_hashed_by_identity()
    {
        // Hashing an object by identity would produce a key that never hits twice, which is worse
        // than saying so: the application is told to preprocess its inputs.
        var failure = Assert.Throws<NotSupportedException>(() => CacheHash.Of(new Stateless()));

        Assert.Contains("Preprocess your inputs", failure.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void An_object_hashes_by_its_readable_state()
    {
        Assert.Equal(CacheHash.Of(new Dictionary<string, object?> { ["Name"] = "a", ["Size"] = 2 }), CacheHash.Of(new Stateful("a", 2)));
        Assert.NotEqual(CacheHash.Of(new Stateful("a", 2)), CacheHash.Of(new Stateful("a", 3)));
    }

    /// <summary>A value with nothing to spell.</summary>
    private sealed class Stateless;

    /// <summary>A value whose properties are its state.</summary>
    private sealed class Stateful(string name, int size)
    {
        public string Name { get; } = name;

        public int Size { get; } = size;
    }
}
