using System.Text.Json.Nodes;
namespace Gradio.Net;
/// <summary>
/// A grid of audio players with a waveform each, where picking one sets the component's value
/// </summary>
/// <remarks>
/// <para>
/// The Python version builds this component out of its custom-HTML one rather than out of its audio
/// component: the template draws the players, the stylesheet lays the grid out, and the JavaScript
/// keeps the value in step with the player the user picks. The parameters are therefore the HTML
/// component's plus four of its own — the audio files, the labels, the column count and the value —
/// and they travel in the config's extra-props dictionary, which is where a custom component's own
/// parameters go.
/// </para>
/// <para>
/// The templates are the Python version's, character for character. They are not configuration: the
/// frontend runs them, so a difference in whitespace would be a difference in what a user sees.
/// </para>
/// </remarks>
[GradioBlock("html")]
public sealed partial class AudioGallery : Html
{
    /// <summary>Declares the component with its templates.</summary>
    internal AudioGallery()
    {
        HtmlTemplate = HtmlTemplateSource;
        CssTemplate = CssTemplateSource;
        JsOnLoad = JsOnLoadSource;
        ApplyDefaultCss = false;
    }

    /// <summary>The audio files offered, by address.</summary>
    public IReadOnlyList<string> AudioUrls { get; set; } = [];

    /// <summary>The labels shown above the players, one per file.</summary>
    public IReadOnlyList<string>? Labels { get; set; }

    /// <summary>How many players are shown in one row.</summary>
    public int Columns { get; set; } = 3;

    /// <summary>The audio the component starts with, or <see langword="null"/> to start with none.</summary>
    /// <value>
    /// The address of one of <see cref="AudioUrls"/>, or <see langword="null"/> to leave the choice
    /// to the user; the Python version answers the first file when the application names none.
    /// </value>
    public override object? Value
    {
        get => base.Value ?? (AudioUrls.Count > 0 ? AudioUrls[0] : null);
        set => base.Value = value;
    }

    /// <summary>The props this component writes, in the subclass's parameter order.</summary>
    /// <param name="props">The props being built.</param>
    /// <remarks>
    /// The Python version writes <c>audio_urls</c> twice: once as the subclass's own constructor
    /// parameter, which it assigns to the instance, and once in the extra-props dictionary it hands
    /// the HTML component. The first is what the frontend reads as a prop, the second is what the
    /// template iterates, so both are here.
    /// </remarks>
    protected internal override void WriteProps(PropWriter props)
    {
        ArgumentNullException.ThrowIfNull(props);

        props.Add("audio_urls", AudioUrls);
        base.WriteProps(props);
    }

    /// <summary>The files, labels and column count, as a custom component's parameters are carried.</summary>
    public override JsonObject? ExtraProperties => new()
    {
        ["audio_urls"] = Strings(AudioUrls),
        ["labels"] = Labels is null ? null : Strings(Labels),
        ["columns"] = Columns,
    };

    /// <inheritdoc />
    public override JsonObject ApiInfo => new()
    {
        ["type"] = "string",
        ["title"] = "Audio URL",
    };

    /// <summary>The template that draws the players.</summary>
    private const string HtmlTemplateSource =
        "\n"
        + "        <div class=\"audio-gallery-container\">\n"
        + "            ${label ? `<label class=\"container-label\">${label}</label>` : ''}\n"
        + "            <div class=\"audio-gallery-grid\" style=\"grid-template-columns: repeat(${columns}, 1fr);\">\n"
        + "                ${audio_urls.map((url, i) => `\n"
        + "                    <div class=\"audio-item\" data-index=\"${i}\">\n"
        + "                        <div class=\"audio-label\">${labels && labels[i] ? labels[i] : 'Audio ' + (i + 1)}</div>\n"
        + "                        <canvas class=\"waveform-canvas\" data-url=\"${url}\" width=\"300\" height=\"80\"></canvas>\n"
        + "                        <audio src=\"${url}\" preload=\"metadata\" ${value === url ? 'data-selected=\"true\"' : ''}></audio>\n"
        + "                        <div class=\"audio-controls\">\n"
        + "                            <button class=\"play-btn\">▶</button>\n"
        + "                            <div class=\"time-display\">0:00</div>\n"
        + "                        </div>\n"
        + "                    </div>\n"
        + "                `).join('')}\n"
        + "            </div>\n"
        + "        </div>\n"
        + "        ";

    /// <summary>The stylesheet that lays the grid out.</summary>
    private const string CssTemplateSource =
        "\n"
        + "        .audio-gallery-container { padding: var(--spacing-lg); }\n"
        + "        .container-label { display: block; margin-bottom: var(--spacing-md); font-weight: 600; }\n"
        + "        .audio-gallery-grid { display: grid; gap: var(--spacing-lg); }\n"
        + "        .audio-item { border: 2px solid var(--border-color-primary); border-radius: var(--radius-md); padding: var(--spacing-md); cursor: pointer; transition: all 0.2s; }\n"
        + "        .audio-item:hover { border-color: var(--color-accent); box-shadow: 0 2px 8px rgba(0,0,0,0.1); }\n"
        + "        .audio-item[data-selected=\"true\"] { border-color: var(--color-accent); background-color: var(--background-fill-secondary); }\n"
        + "        .audio-label { margin-bottom: 8px; text-align: center; }\n"
        + "        .waveform-canvas { width: 100%; height: 80px; background: var(--background-fill-secondary); margin-bottom: 8px; }\n"
        + "        .audio-controls { display: flex; align-items: center; gap: 8px; }\n"
        + "        .play-btn { width: 32px; height: 32px; border-radius: 50%; border: none; background: var(--color-accent); color: white; cursor: pointer; }\n"
        + "        .play-btn:hover { opacity: 0.8; }\n"
        + "        .time-display { font-size: 12px; }\n"
        + "        ";

    /// <summary>The JavaScript that keeps the value in step with the player the user picks.</summary>
    private const string JsOnLoadSource =
        "\n"
        + "        const audioItems = element.querySelectorAll('.audio-item');\n"
        + "\n"
        + "        audioItems.forEach((item, index) => {\n"
        + "            const canvas = item.querySelector('.waveform-canvas');\n"
        + "            const audio = item.querySelector('audio');\n"
        + "            const playBtn = item.querySelector('.play-btn');\n"
        + "            const timeDisplay = item.querySelector('.time-display');\n"
        + "            const ctx = canvas.getContext('2d');\n"
        + "\n"
        + "            drawWaveform(canvas, ctx);\n"
        + "\n"
        + "            item.addEventListener('click', (e) => {\n"
        + "                if (e.target === playBtn) return;\n"
        + "                audioItems.forEach(i => i.removeAttribute('data-selected'));\n"
        + "                item.setAttribute('data-selected', 'true');\n"
        + "                props.value = audio.src;\n"
        + "            });\n"
        + "\n"
        + "            playBtn.addEventListener('click', (e) => {\n"
        + "                e.stopPropagation();\n"
        + "                if (audio.paused) {\n"
        + "                    document.querySelectorAll('.audio-item audio').forEach(a => a.pause());\n"
        + "                    document.querySelectorAll('.play-btn').forEach(b => b.textContent = '▶');\n"
        + "                    audio.play();\n"
        + "                    playBtn.textContent = '⏸';\n"
        + "                } else {\n"
        + "                    audio.pause();\n"
        + "                    playBtn.textContent = '▶';\n"
        + "                }\n"
        + "            });\n"
        + "\n"
        + "            audio.addEventListener('timeupdate', () => {\n"
        + "                const currentTime = Math.floor(audio.currentTime);\n"
        + "                const minutes = Math.floor(currentTime / 60);\n"
        + "                const seconds = currentTime % 60;\n"
        + "                timeDisplay.textContent = `${minutes}:${seconds.toString().padStart(2, '0')}`;\n"
        + "\n"
        + "                const progress = audio.currentTime / audio.duration;\n"
        + "                drawWaveform(canvas, ctx, progress);\n"
        + "            });\n"
        + "\n"
        + "            audio.addEventListener('ended', () => {\n"
        + "                playBtn.textContent = '▶';\n"
        + "                drawWaveform(canvas, ctx, 0);\n"
        + "            });\n"
        + "        });\n"
        + "\n"
        + "        function drawWaveform(canvas, ctx, progress = 0) {\n"
        + "            const width = canvas.width;\n"
        + "            const height = canvas.height;\n"
        + "            const bars = 50;\n"
        + "            const barWidth = width / bars;\n"
        + "\n"
        + "            ctx.clearRect(0, 0, width, height);\n"
        + "\n"
        + "            for (let i = 0; i < bars; i++) {\n"
        + "                const barHeight = (Math.sin(i * 0.5) * 0.3 + Math.random() * 0.7) * height * 0.8;\n"
        + "                const x = i * barWidth;\n"
        + "                const y = (height - barHeight) / 2;\n"
        + "\n"
        + "                ctx.fillStyle = i / bars < progress ? '#FF7C00' : '#ccc';\n"
        + "                ctx.fillRect(x, y, barWidth - 2, barHeight);\n"
        + "            }\n"
        + "        }\n"
        + "        ";

    /// <summary>A list of strings as a JSON array.</summary>
    /// <param name="items">The strings.</param>
    /// <returns>A new array.</returns>
    private static JsonArray Strings(IReadOnlyList<string> items)
    {
        var array = new JsonArray();

        foreach (var item in items)
        {
            array.Add(item);
        }

        return array;
    }
}
