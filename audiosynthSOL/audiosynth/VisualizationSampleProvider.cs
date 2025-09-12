using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Drawing;
using System.Windows.Forms;

namespace audiosynth
{
    // VisualizationSampleProvider.cs (create a new class)

    public class VisualizationSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly WaveformViewer viewer;

        public WaveFormat WaveFormat => source.WaveFormat;

        public VisualizationSampleProvider(ISampleProvider source, WaveformViewer viewer)
        {
            this.source = source;
            this.viewer = viewer;
        }


        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead = source.Read(buffer, offset, sampleCount);

            if (samplesRead > 0)
            {
                Console.WriteLine(buffer[0]);
            }

            // This is the crucial line. Make sure it's correct.
            viewer.AddSamples(buffer, offset, samplesRead);

            return samplesRead;
        }


    }
}
