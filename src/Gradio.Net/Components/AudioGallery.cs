
namespace Gradio.Net.Components;

public class AudioGallery : Html
{
    public List<string> AudioUrls { get; set; }
    public List<string>? Labels { get; set; }
    public int Columns { get; set; }

    public AudioGallery(
        List<string> audioUrls,
        string? value = null,
        List<string>? labels = null,
        int columns = 3,
        string? label = null,
        object? every = null,
        object? inputs = null,
        bool showLabel = false,
        object? visible = null,
        string? elemId = null,
        object? elemClasses = null,
        bool render = true,
        object? key = null,
        object? preservedByKey = null)
        : base(
            value: value ?? (audioUrls.Count > 0 ? audioUrls[0] : null),
            label: label,
            htmlTemplate: HtmlTemplateValue,
            cssTemplate: CssTemplateValue,
            jsOnLoad: JsOnLoadValue,
            applyDefaultCss: false,
            every: every,
            inputs: inputs,
            showLabel: showLabel,
            visible: visible,
            elemId: elemId,
            elemClasses: elemClasses,
            render: render,
            key: key,
            preservedByKey: preservedByKey,
            props: new Dictionary<string, object>
            {
                ["audio_urls"] = audioUrls,
                ["labels"] = labels ?? new List<string>(),
                ["columns"] = columns,
                ["label"] = label ?? string.Empty
            })
    {
        AudioUrls = audioUrls;
        Labels = labels;
        Columns = columns;
    }

    public override string GetBlockName() => "audio_gallery";

    public override Dictionary<string, object> ApiInfo()
    {
        return new Dictionary<string, object>
        {
            ["type"] = "string",
            ["title"] = "Audio URL"
        };
    }

    public override object ExamplePayload()
    {
        return "https://github.com/gradio-app/gradio/raw/main/test/test_files/audio_sample.wav";
    }

    public override object ExampleValue()
    {
        return "https://github.com/gradio-app/gradio/raw/main/test/test_files/audio_sample.wav";
    }

    private const string HtmlTemplateValue = @"
        <div class=""audio-gallery-container"">
            ${label ? `<label class=""container-label"">${label}</label>` : ''}
            <div class=""audio-gallery-grid"" style=""grid-template-columns: repeat(${columns}, 1fr);"">
                ${audio_urls.map((url, i) => `
                    <div class=""audio-item"" data-index=""${i}"">
                        <div class=""audio-label"">${labels && labels[i] ? labels[i] : 'Audio ' + (i + 1)}</div>
                        <canvas class=""waveform-canvas"" data-url=""${url}"" width=""300"" height=""80""></canvas>
                        <audio src=""${url}"" preload=""metadata"" ${value === url ? 'data-selected=""true""' : ''}></audio>
                        <div class=""audio-controls"">
                            <button class=""play-btn"">▶</button>
                            <div class=""time-display"">0:00</div>
                        </div>
                    </div>
                `).join('')}
            </div>
        </div>
        ";

    private const string CssTemplateValue = @"
        .audio-gallery-container { padding: var(--spacing-lg); }
        .container-label { display: block; margin-bottom: var(--spacing-md); font-weight: 600; }
        .audio-gallery-grid { display: grid; gap: var(--spacing-lg); }
        .audio-item { border: 2px solid var(--border-color-primary); border-radius: var(--radius-md); padding: var(--spacing-md); cursor: pointer; transition: all 0.2s; }
        .audio-item:hover { border-color: var(--color-accent); box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
        .audio-item[data-selected=""true""] { border-color: var(--color-accent); background-color: var(--background-fill-secondary); }
        .audio-label { margin-bottom: 8px; text-align: center; }
        .waveform-canvas { width: 100%; height: 80px; background: var(--background-fill-secondary); margin-bottom: 8px; }
        .audio-controls { display: flex; align-items: center; gap: 8px; }
        .play-btn { width: 32px; height: 32px; border-radius: 50%; border: none; background: var(--color-accent); color: white; cursor: pointer; }
        .play-btn:hover { opacity: 0.8; }
        .time-display { font-size: 12px; }
        ";

    private const string JsOnLoadValue = @"
        const audioItems = element.querySelectorAll('.audio-item');

        audioItems.forEach((item, index) => {
            const canvas = item.querySelector('.waveform-canvas');
            const audio = item.querySelector('audio');
            const playBtn = item.querySelector('.play-btn');
            const timeDisplay = item.querySelector('.time-display');
            const ctx = canvas.getContext('2d');

            drawWaveform(canvas, ctx);

            item.addEventListener('click', (e) => {
                if (e.target === playBtn) return;
                audioItems.forEach(i => i.removeAttribute('data-selected'));
                item.setAttribute('data-selected', 'true');
                props.value = audio.src;
            });

            playBtn.addEventListener('click', (e) => {
                e.stopPropagation();
                if (audio.paused) {
                    document.querySelectorAll('.audio-item audio').forEach(a => a.pause());
                    document.querySelectorAll('.play-btn').forEach(b => b.textContent = '▶');
                    audio.play();
                    playBtn.textContent = '⏸';
                } else {
                    audio.pause();
                    playBtn.textContent = '▶';
                }
            });

            audio.addEventListener('timeupdate', () => {
                const currentTime = Math.floor(audio.currentTime);
                const minutes = Math.floor(currentTime / 60);
                const seconds = currentTime % 60;
                timeDisplay.textContent = `${minutes}:${seconds.toString().padStart(2, '0')}`;

                const progress = audio.currentTime / audio.duration;
                drawWaveform(canvas, ctx, progress);
            });

            audio.addEventListener('ended', () => {
                playBtn.textContent = '▶';
                drawWaveform(canvas, ctx, 0);
            });
        });

        function drawWaveform(canvas, ctx, progress = 0) {
            const width = canvas.width;
            const height = canvas.height;
            const bars = 50;
            const barWidth = width / bars;

            ctx.clearRect(0, 0, width, height);

            for (let i = 0; i < bars; i++) {
                const barHeight = (Math.sin(i * 0.5) * 0.3 + Math.random() * 0.7) * height * 0.8;
                const x = i * barWidth;
                const y = (height - barHeight) / 2;

                ctx.fillStyle = i / bars < progress ? '#FF7C00' : '#ccc';
                ctx.fillRect(x, y, barWidth - 2, barHeight);
            }
        }
        ";
}
