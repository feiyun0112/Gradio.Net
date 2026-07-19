namespace Gradio.Net.Components
{
    public class WaveformOptions
    {
        public string WaveformColor { get; set; }
        public string WaveformProgressColor { get; set; }
        public string TrimRegionColor { get; set; }
        public bool ShowRecordingWaveform { get; set; } = true;
        public double SkipLength { get; set; } = 5;
        public int SampleRate { get; set; } = 44100;
    }
}
