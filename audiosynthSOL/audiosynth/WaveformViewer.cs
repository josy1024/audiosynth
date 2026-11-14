using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace audiosynth
{
    /// <summary>
    /// Oscilloscope visualizer styled after modern hardware/software synths (as seen in Image 2):
    /// - Rich royal blue background
    /// - Bright electric cyan/white waveform trace with anti-aliasing
    /// - Triggered timebase showing ~8-12 full wave cycles across the screen
    /// - Full vertical amplitude scaling
    /// </summary>
    public partial class WaveformViewer : UserControl
    {
        private float[]? displayPoints = null;
        private readonly float[] sampleHistory = new float[8192];
        private int historyWriteIndex = 0;

        public WaveformViewer()
        {
            InitializeComponent();
            DoubleBuffered = true;
            displayPoints = new float[325];
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0)
            {
                displayPoints = new float[Width];
            }
        }

        public void AddSamples(float[] buffer, int offset, int count, int channels = 1)
        {
            if (buffer == null || count <= 0) return;

            int stride = Math.Max(1, channels);

            // Copy incoming samples to ring history buffer
            for (int i = 0; i < count; i += stride)
            {
                sampleHistory[historyWriteIndex] = buffer[offset + i];
                historyWriteIndex = (historyWriteIndex + 1) % sampleHistory.Length;
            }

            if (displayPoints == null || displayPoints.Length == 0) return;

            // Search backwards from historyWriteIndex for a positive zero-crossing
            int searchStart = historyWriteIndex - 1;
            if (searchStart < 0) searchStart += sampleHistory.Length;

            int zeroCrossIndex = -1;
            int maxSearch = Math.Min(4096, sampleHistory.Length - 100);

            for (int i = 0; i < maxSearch; i++)
            {
                int idx = (searchStart - i + sampleHistory.Length) % sampleHistory.Length;
                int prevIdx = (idx - 1 + sampleHistory.Length) % sampleHistory.Length;

                if (sampleHistory[prevIdx] < 0f && sampleHistory[idx] >= 0f)
                {
                    zeroCrossIndex = idx;
                    break;
                }
            }

            if (zeroCrossIndex != -1)
            {
                // To display ~8-10 full waves of a bass note (~100 Hz = 441 samples/wave -> 10 waves = ~4410 samples)
                // across displayPoints.Length pixels (e.g. 325 px), step is ~10-12 samples per pixel.
                float step = 10.0f;
                float gain = 2.0f; // High amplitude filling ~80-85% height

                for (int x = 0; x < displayPoints.Length; x++)
                {
                    int sampleIdx = (zeroCrossIndex + (int)(x * step)) % sampleHistory.Length;
                    float val = sampleHistory[sampleIdx] * gain;
                    displayPoints[x] = Math.Clamp(val, -1.0f, 1.0f);
                }

                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Vibrant Royal Blue background (matching video screenshot)
            g.Clear(Color.FromArgb(6, 68, 160));

            int w = Width;
            int h = Height;
            if (w <= 0 || h <= 0 || displayPoints == null || displayPoints.Length < 2) return;

            float midY = h * 0.5f;
            float maxAmplitude = h * 0.42f;

            // Subtle center zero-axis grid line
            using (var gridPen = new Pen(Color.FromArgb(60, 255, 255, 255), 1.0f))
            {
                gridPen.DashStyle = DashStyle.Dash;
                g.DrawLine(gridPen, 0, midY, w, midY);
            }

            // Build trace points
            var points = new PointF[displayPoints.Length];
            for (int x = 0; x < displayPoints.Length; x++)
            {
                float px = x * ((float)w / displayPoints.Length);
                float py = midY - (displayPoints[x] * maxAmplitude);
                points[x] = new PointF(px, py);
            }

            // Draw bright cyan/white glowing waveform line
            using (var glowPen = new Pen(Color.FromArgb(70, 0, 220, 255), 4.5f))
            {
                g.DrawLines(glowPen, points);
            }

            using (var tracePen = new Pen(Color.FromArgb(220, 255, 255), 2.0f))
            {
                g.DrawLines(tracePen, points);
            }

            // Small header tag
            using (var textBrush = new SolidBrush(Color.FromArgb(180, 220, 255)))
            using (var font = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            {
                g.DrawString("OSCILLOSCOPE (~10 WAVES)", font, textBrush, 6, 4);
            }
        }
    }
}
