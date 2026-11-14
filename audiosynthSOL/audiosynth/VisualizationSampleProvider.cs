using NAudio.Wave;

namespace audiosynth
{
    public class VisualizationSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly WaveformViewer? waveViewer;
        private readonly SpectrumViewer? spectrumViewer;

        public WaveFormat WaveFormat => source.WaveFormat;

        public VisualizationSampleProvider(ISampleProvider source, WaveformViewer? waveViewer, SpectrumViewer? spectrumViewer = null)
        {
            this.source = source;
            this.waveViewer = waveViewer;
            this.spectrumViewer = spectrumViewer;
        }

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);

            if (samplesRead > 0)
            {
                waveViewer?.AddSamples(buffer, offset, samplesRead, WaveFormat.Channels);
                spectrumViewer?.AddSamples(buffer, offset, samplesRead, WaveFormat.Channels);
            }

            return samplesRead;
        }
    }
}
