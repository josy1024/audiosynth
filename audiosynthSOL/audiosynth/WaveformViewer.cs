using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace audiosynth
{
    // WaveformViewer.cs (create a new UserControl)

    public partial class WaveformViewer : UserControl
    {
        // Change the field to not be readonly so it can be resized
        private float[] waveformData = null;
        private int dataIndex = 0;

        public WaveformViewer()
        {
            InitializeComponent();
            DoubleBuffered = true; // Prevents flickering
            waveformData = new float[512];
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Check for null or if the width has changed.
            if (waveformData == null || waveformData.Length != Width)
            {
                // Only create the array if the width is valid.
                if (Width > 0)
                {
                    waveformData = new float[Width];
                    dataIndex = 0; // Reset the index after resizing
                }
            }
        }


        public void AddSamples(float[] buffer, int offset, int count)
        {
            // Check if the waveformData array is null or has no size.
            if (waveformData == null || waveformData.Length == 0)
            {
                return;
            }

            // Find a zero-crossing point to stabilize the waveform display.
            int zeroCrossingIndex = -1;
            for (int i = 1; i < count; i++)
            {
                // A zero-crossing occurs when the sign of the sample changes.
                if (buffer[offset + i - 1] < 0 && buffer[offset + i] >= 0)
                {
                    zeroCrossingIndex = i;
                    break;
                }
            }

            // If a zero-crossing is found, copy the data from that point.
            if (zeroCrossingIndex != -1)
            {
                // Determine how many samples to copy after the zero-crossing.
                int samplesToCopy = Math.Min(count - zeroCrossingIndex, waveformData.Length);

                // This is the recommended gain value. It provides good visibility without excessive clipping.
                float gain = 1.0f; // A gain of 1.0f is a good starting point. Adjust as needed.

                for (int i = 0; i < samplesToCopy; i++)
                {
                    float sampleValue = buffer[offset + zeroCrossingIndex + i] * gain;

                    // Clamp the value to the visible range [-1.0, 1.0].
                    waveformData[i] = Math.Clamp(sampleValue, -1.0f, 1.0f);
                }

                // This tells the drawing loop how many samples it has to work with.
                dataIndex = samplesToCopy;

                // Force a redraw of the control.
                Invalidate();
            }
        }

        // In WaveformViewer.cs

        // In WaveformViewer.cs

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (waveformData == null || dataIndex == 0) return;

            e.Graphics.Clear(Color.Black);

            Pen pen = new Pen(Color.LimeGreen);
            float midY = Height / 2.0f;
            float scaleX = (float)Width / waveformData.Length;
            float scaleY = midY * 0.7f;

            for (int i = 0; i < dataIndex - 1; i++)
            {
                float x1 = i * scaleX;
                float y1 = midY - (waveformData[i] * scaleY);
                float x2 = (i + 1) * scaleX;
                float y2 = midY - (waveformData[i + 1] * scaleY);

                e.Graphics.DrawLine(pen, x1, y1, x2, y2);
            }
        }
    }
}
