using System.Text.Json;
using System.Text.Json.Nodes;
using Gradio.Net;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gradio.Net.Tests.Kernel;

/// <summary>
/// A <c>BrowserState</c> as the server sees it: a value the browser keeps, sealed by the server so
/// that a client which edited it is refused rather than believed.
/// </summary>
public sealed class BrowserStateTests
{
    [Fact]
    public void A_secret_and_a_key_are_drawn_once_and_kept()
    {
        var state = new BrowserState();

        var secret = state.Secret;
        var key = state.StorageKey;

        Assert.Equal(16, secret.Length);
        Assert.Equal(16, key.Length);
        Assert.All(secret, character => Assert.True(char.IsAsciiLetterOrDigit(character)));
        Assert.All(key, character => Assert.True(char.IsAsciiLetterOrDigit(character)));

        // Drawn once: the config is built per request, and two requests that drew two secrets would
        // hand out values sealed with keys the other request does not know.
        Assert.Equal(secret, state.Secret);
        Assert.Equal(key, state.StorageKey);
    }

    [Fact]
    public void Two_browser_states_do_not_share_a_secret()
    {
        Assert.NotEqual(new BrowserState().Secret, new BrowserState().Secret);
    }

    [Fact]
    public void The_application_can_choose_the_secret_and_the_key()
    {
        var state = new BrowserState { Secret = "mine", StorageKey = "chosen" };

        Assert.Equal("mine", state.Secret);
        Assert.Equal("chosen", state.StorageKey);
    }

    [Fact]
    public void A_browser_with_nothing_stored_reads_the_default()
    {
        var state = new BrowserState { DefaultValue = JsonValue.Create("fallback") };

        Assert.Equal("fallback", Assert.IsType<string>(state.Preprocess(null)));
    }

    [Fact]
    public void What_the_server_sealed_is_what_the_server_opens()
    {
        var state = new BrowserState();
        var sealedValue = state.Postprocess(new JsonObject { ["turns"] = 3 });

        Assert.Equal(JsonValueKind.String, sealedValue!.GetValueKind());

        var opened = Assert.IsAssignableFrom<IReadOnlyDictionary<string, object?>>(state.Preprocess(sealedValue));

        Assert.Equal(3d, opened["turns"]);
    }

    [Fact]
    public void A_sealed_value_does_not_carry_its_plaintext()
    {
        var state = new BrowserState();

        var sealedValue = state.Postprocess("a secret the browser should not be able to read");

        Assert.DoesNotContain("secret", sealedValue!.GetValue<string>(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void An_edited_value_is_refused_and_the_default_is_used()
    {
        var state = new BrowserState { DefaultValue = JsonValue.Create("fallback") };
        var sealedValue = state.Postprocess("genuine")!.GetValue<string>();

        // One character in the middle of the seal changed, which is a byte of the nonce, the tag or
        // the ciphertext: the tag no longer authenticates the payload, which is what tells an
        // edited value from one this server issued. (The last character is the wrong one to change:
        // base64 padding bits can make it encode the same bytes.)
        var at = sealedValue.Length / 2;
        var tampered = JsonValue.Create(
            string.Concat(
                sealedValue.AsSpan(0, at),
                sealedValue[at] == 'A' ? "B" : "A",
                sealedValue.AsSpan(at + 1)));

        Assert.Equal("fallback", Assert.IsType<string>(state.Preprocess(tampered)));
    }

    [Fact]
    public void A_value_sealed_by_another_server_is_refused_even_with_the_same_secret()
    {
        // The secret is in the config, so a client knows it: a seal keyed by it could be forged by
        // anyone who read the page. The seal key is the server's own, which is why two components
        // that share a secret still refuse each other's values.
        var elsewhere = new BrowserState { Secret = "shared-secret" };
        var mine = new BrowserState { Secret = "shared-secret", DefaultValue = JsonValue.Create("fallback") };

        var sealedElsewhere = elsewhere.Postprocess("not mine");

        Assert.Equal("fallback", Assert.IsType<string>(mine.Preprocess(sealedElsewhere)));
    }

    [Theory]
    [InlineData("plain text")]
    [InlineData("v1.not-base64!!")]
    [InlineData("v1.")]
    [InlineData("")]
    public void A_value_the_server_did_not_seal_is_refused(string value)
    {
        var state = new BrowserState { DefaultValue = JsonValue.Create("fallback") };

        Assert.Equal("fallback", Assert.IsType<string>(state.Preprocess(JsonValue.Create(value))));
    }

    [Fact]
    public void A_refused_value_falls_back_to_nothing_when_there_is_no_default()
    {
        var state = new BrowserState();

        Assert.Null(state.Preprocess(JsonValue.Create("forged")));
    }

    [Fact]
    public void The_config_carries_the_default_the_key_and_the_secret()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        BrowserState state;

        using (blocks.BeginRender())
        {
            state = blocks.Add(new BrowserState { DefaultValue = JsonValue.Create("fallback") });
        }

        var entry = blocks.GetConfig()["components"]!.AsArray()
            .Select(component => component!.AsObject())
            .Single(component => component["id"]!.GetValue<int>() == state.Id);

        var props = entry["props"]!.AsObject();

        Assert.Equal("fallback", props["default_value"]!.GetValue<string>());
        Assert.Equal(state.StorageKey, props["storage_key"]!.GetValue<string>());
        Assert.Equal(state.Secret, props["secret"]!.GetValue<string>());
        Assert.False(entry["skip_api"]!.GetValue<bool>());

        // The Python version declares the typing information as "any json-serializable value", which is what
        // the API description and the OpenAPI document are built from.
        Assert.Equal("any json-serializable value", entry["api_info"]!["description"]!.GetValue<string>());
        Assert.Equal("test", entry["example_inputs"]!.GetValue<string>());
    }

    [Fact]
    public void An_unset_default_is_left_out_of_the_config()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        BrowserState state;

        using (blocks.BeginRender())
        {
            state = blocks.Add(new BrowserState());
        }

        var props = blocks.GetConfig()["components"]!.AsArray()
            .Select(component => component!.AsObject())
            .Single(component => component["id"]!.GetValue<int>() == state.Id)["props"]!
            .AsObject();

        // The Python version deletes every None from the props before serving them, so an unset default is
        // absent rather than null.
        Assert.False(props.ContainsKey("default_value"));
    }

    [Fact]
    public void A_browser_state_does_not_split_the_form_around_it()
    {
        using var provider = KernelHost.BuildProvider();
        using var blocks = provider.CreateBlocksWithContainers();
        BrowserState state;

        using (blocks.BeginRender())
        {
            blocks.Add(new Textbox { Label = "A" });
            state = blocks.Add(new BrowserState());
            blocks.Add(new Textbox { Label = "B" });
        }

        var form = Assert.Single(blocks.Children.OfType<Form>());

        Assert.Equal(2, form.Children.Count);
        Assert.Same(blocks, state.Parent);
    }
}
