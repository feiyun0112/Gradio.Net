using Gradio.Net.Components;


namespace Gradio.Net;

public static class Templates
{
    // Python alias: Mic = Microphone
    public static Microphone Mic(object value = null) => new Microphone(value: value);
}
