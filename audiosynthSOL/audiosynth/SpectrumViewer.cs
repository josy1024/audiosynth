using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace audiosynth
{
    /// <summary>
    /// Real-time continuous FFT Frequency Spectrum Analyzer:
    /// - Continuous glowing copper/amber mountain curve
    /// - Logarithmic frequency distribution (Left = Sub/Bass, Right = Highs)
    /// - Semi-transparent knob description overlay (shown on hover, fades after 3 s)
    /// </summary>
    public partial class SpectrumViewer : UserControl
    {
        private const int FftSize = 1024;
        private readonly float[] sampleBuffer = new float[FftSize];
        private int sampleBufferPos = 0;

        private readonly double[] fftReal = new double[FftSize];
        private readonly double[] fftImag = new double[FftSize];
        private readonly double[] window  = new double[FftSize];

        private const int PointCount = 128;
        private readonly float[] spectrumLevels = new float[PointCount];
        private readonly int[]   binIndices      = new int[PointCount];

        // --- Tooltip overlay ---
        private string _tooltipText  = string.Empty;
        private int    _tooltipAlpha = 0;   // 0 = invisible, 230 = fully opaque
        private readonly System.Windows.Forms.Timer _tooltipFadeTimer;

        public SpectrumViewer()
        {
            InitializeComponent();
            DoubleBuffered = true;

            // Hann window
            for (int i = 0; i < FftSize; i++)
                window[i] = 0.5 * (1.0 - Math.Cos(2.0 * Math.PI * i / (FftSize - 1)));

            // Map 128 visual points logarithmically from 35 Hz to 14 000 Hz
            double minFreq   = 35.0;
            double maxFreq   = 14000.0;
            double sampleRate = 44100.0;
            double binHz     = sampleRate / FftSize; // ~43.06 Hz per bin

            for (int i = 0; i < PointCount; i++)
            {
                double freq = minFreq * Math.Pow(maxFreq / minFreq, (double)i / (PointCount - 1));
                int    bin  = (int)Math.Round(freq / binHz);
                binIndices[i] = Math.Clamp(bin, 0, (FftSize / 2) - 1);
            }

            // Tooltip fade-out timer (fires every 80 ms, decrements alpha)
            _tooltipFadeTimer = new System.Windows.Forms.Timer { Interval = 80 };
            _tooltipFadeTimer.Tick += (s, e) =>
            {
                _tooltipAlpha -= 18;
                if (_tooltipAlpha <= 0)
                {
                    _tooltipAlpha = 0;
                    _tooltipFadeTimer.Stop();
                }
                Invalidate();
            };
        }

        /// <summary>
        /// Shows a semi-transparent description overlay on the spectrum for ~3 s then fades out.
        /// Call this from OnKnobHovered.
        /// </summary>
        public void ShowTooltip(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowTooltip(text)));
                return;
            }

            _tooltipText  = text;
            _tooltipAlpha = 230;
            _tooltipFadeTimer.Stop();

            // Hold for 2.8 s, then start fading (~1 s fade = 230/18*80 ms)
            var holdTimer = new System.Windows.Forms.Timer { Interval = 2800 };
            holdTimer.Tick += (s, e) =>
            {
                holdTimer.Stop();
                holdTimer.Dispose();
                _tooltipFadeTimer.Start();
            };
            holdTimer.Start();

            Invalidate();
        }

        // ----------------------------------------------------------------
        // Audio feed from VisualizationSampleProvider
        // ----------------------------------------------------------------
        public void AddSamples(float[] buffer, int offset, int count, int channels = 1)
        {
            if (buffer == null || count <= 0) return;

            int stride = Math.Max(1, channels);
            for (int i = 0; i < count; i += stride)
            {
                sampleBuffer[sampleBufferPos++] = buffer[offset + i];
                if (sampleBufferPos >= FftSize)
                {
                    sampleBufferPos = 0;
                    ComputeSpectrum();
                    Invalidate();
                }
            }
        }

        private void ComputeSpectrum()
        {
            for (int i = 0; i < FftSize; i++)
            {
                fftReal[i] = sampleBuffer[i] * window[i];
                fftImag[i] = 0.0;
            }

            RunFft(fftReal, fftImag);

            for (int i = 0; i < PointCount; i++)
            {
                int bin     = binIndices[i];
                int prevBin = Math.Max(0, bin - 1);
                int nextBin = Math.Min((FftSize / 2) - 1, bin + 1);

                double mag1 = Math.Sqrt(fftReal[prevBin] * fftReal[prevBin] + fftImag[prevBin] * fftImag[prevBin]);
                double mag2 = Math.Sqrt(fftReal[bin]     * fftReal[bin]     + fftImag[bin]     * fftImag[bin]);
                double mag3 = Math.Sqrt(fftReal[nextBin] * fftReal[nextBin] + fftImag[nextBin] * fftImag[nextBin]);

                double avgMag = (mag1 + mag2 * 2.0 + mag3) / 4.0;
                float  target = Math.Clamp((float)(avgMag / (FftSize * 0.08)), 0f, 1f);

                if (target > spectrumLevels[i])
                    spectrumLevels[i] = target;
                else
                    spectrumLevels[i] *= 0.84f;
            }
        }

        private static void RunFft(double[] real, double[] imag)
        {
            int n = real.Length;
            int j = 0;

            for (int i = 0; i < n - 1; i++)
            {
                if (i < j)
                {
                    double tr = real[i]; real[i] = real[j]; real[j] = tr;
                    double ti = imag[i]; imag[i] = imag[j]; imag[j] = ti;
                }
                int k = n / 2;
                while (k <= j) { j -= k; k /= 2; }
                j += k;
            }

            for (int len = 2; len <= n; len <<= 1)
            {
                double angle  = -2.0 * Math.PI / len;
                double wlen_r = Math.Cos(angle);
                double wlen_i = Math.Sin(angle);

                for (int i = 0; i < n; i += len)
                {
                    double wr = 1.0, wi = 0.0;
                    int    half = len >> 1;

                    for (int m = 0; m < half; m++)
                    {
                        double u_r = real[i + m], u_i = imag[i + m];
                        double v_r = real[i + m + half] * wr - imag[i + m + half] * wi;
                        double v_i = real[i + m + half] * wi + imag[i + m + half] * wr;

                        real[i + m]        = u_r + v_r;  imag[i + m]        = u_i + v_i;
                        real[i + m + half] = u_r - v_r;  imag[i + m + half] = u_i - v_i;

                        double next_wr = wr * wlen_r - wi * wlen_i;
                        wi = wr * wlen_i + wi * wlen_r;
                        wr = next_wr;
                    }
                }
            }
        }

        // ----------------------------------------------------------------
        // Paint
        // ----------------------------------------------------------------
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(12, 16, 22));

            int w = Width, h = Height;
            if (w <= 0 || h <= 0) return;

            // Grid
            using (var gridPen = new Pen(Color.FromArgb(30, 40, 50), 1f))
            {
                g.DrawLine(gridPen, 0,         h * 0.33f, w, h * 0.33f);
                g.DrawLine(gridPen, 0,         h * 0.66f, w, h * 0.66f);
                g.DrawLine(gridPen, w * 0.33f, 0,         w * 0.33f, h);
                g.DrawLine(gridPen, w * 0.66f, 0,         w * 0.66f, h);
            }

            // Curve points
            var curvePoints = new PointF[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                float px = (float)i / (PointCount - 1) * w;
                float py = (h - 4) - (spectrumLevels[i] * (h - 16));
                curvePoints[i] = new PointF(px, Math.Clamp(py, 4, h - 2));
            }

            // Filled copper mountain
            var polyPoints = new PointF[PointCount + 2];
            Array.Copy(curvePoints, polyPoints, PointCount);
            polyPoints[PointCount]     = new PointF(w, h);
            polyPoints[PointCount + 1] = new PointF(0, h);

            using (var brush = new LinearGradientBrush(
                new PointF(0, 0), new PointF(0, h),
                Color.FromArgb(220, 240, 150, 50),
                Color.FromArgb(180, 50,  20,  5)))
            {
                g.FillPolygon(brush, polyPoints);
            }

            using (var contourPen = new Pen(Color.FromArgb(255, 190, 70), 2.2f))
                g.DrawCurve(contourPen, curvePoints, 0.4f);

            // Axis labels
            using (var textBrush = new SolidBrush(Color.FromArgb(160, 180, 200)))
            using (var font = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            {
                g.DrawString("SPECTRUM (LOW BASS)", font, textBrush, 6, 4);
                g.DrawString("HIGHS",               font, textBrush, w - 42, 4);
            }

            // Semi-transparent tooltip overlay
            if (_tooltipAlpha > 0 && !string.IsNullOrEmpty(_tooltipText))
                DrawTooltipOverlay(g, w, h);
        }

        private void DrawTooltipOverlay(Graphics g, int w, int h)
        {
            const int padding = 10;

            using var font   = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            SizeF textSize   = g.MeasureString(_tooltipText, font,
                                   w - padding * 4, StringFormat.GenericDefault);

            int boxW = Math.Min((int)textSize.Width + padding * 2, w - padding * 2);
            int boxH = (int)textSize.Height + padding * 2;
            int boxX = padding;
            int boxY = h - boxH - padding;

            // Frosted-glass dark backdrop
            using (var bgBrush = new SolidBrush(
                Color.FromArgb(Math.Clamp(_tooltipAlpha, 0, 200), 8, 12, 20)))
            {
                g.FillRectangle(bgBrush, boxX, boxY, boxW, boxH);
            }

            // Amber border
            using (var borderPen = new Pen(
                Color.FromArgb(Math.Clamp(_tooltipAlpha, 0, 180), 220, 150, 40), 1.2f))
            {
                g.DrawRectangle(borderPen, boxX, boxY, boxW, boxH);
            }

            // White text
            using (var textBrush = new SolidBrush(
                Color.FromArgb(Math.Clamp(_tooltipAlpha, 0, 230), 240, 230, 200)))
            {
                var fmt = new StringFormat { Trimming = StringTrimming.Word };
                g.DrawString(_tooltipText, font, textBrush,
                    new RectangleF(boxX + padding, boxY + padding / 2f,
                                   boxW - padding * 2, boxH),
                    fmt);
            }
        }
    }
}
