using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace audiosynth
{
    /// <summary>
    /// Real-time continuous FFT Frequency Spectrum Analyzer styled after the tutorial screen (Image 2):
    /// - Continuous glowing copper/amber mountain curve
    /// - Logarithmic frequency distribution (Left = Sub/Bass, Right = Highs)
    /// - Smooth gradient fill with harmonic peaks visible across the filter sweep
    /// </summary>
    public partial class SpectrumViewer : UserControl
    {
        private const int FftSize = 1024;
        private readonly float[] sampleBuffer = new float[FftSize];
        private int sampleBufferPos = 0;

        private readonly double[] fftReal = new double[FftSize];
        private readonly double[] fftImag = new double[FftSize];
        private readonly double[] window = new double[FftSize];

        private const int PointCount = 128;
        private readonly float[] spectrumLevels = new float[PointCount];
        private readonly int[] binIndices = new int[PointCount];

        public SpectrumViewer()
        {
            InitializeComponent();
            DoubleBuffered = true;

            // Hann window
            for (int i = 0; i < FftSize; i++)
            {
                window[i] = 0.5 * (1.0 - Math.Cos(2.0 * Math.PI * i / (FftSize - 1)));
            }

            // Map 128 visual points logarithmically from 35 Hz to 14,000 Hz
            double minFreq = 35.0;
            double maxFreq = 14000.0;
            double sampleRate = 44100.0;
            double binHz = sampleRate / FftSize; // ~43.06 Hz per bin

            for (int i = 0; i < PointCount; i++)
            {
                double freq = minFreq * Math.Pow(maxFreq / minFreq, (double)i / (PointCount - 1));
                int bin = (int)Math.Round(freq / binHz);
                binIndices[i] = Math.Clamp(bin, 0, (FftSize / 2) - 1);
            }
        }

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
                int bin = binIndices[i];
                // Average adjacent bin for smooth curves
                int prevBin = Math.Max(0, bin - 1);
                int nextBin = Math.Min((FftSize / 2) - 1, bin + 1);

                double mag1 = Math.Sqrt(fftReal[prevBin] * fftReal[prevBin] + fftImag[prevBin] * fftImag[prevBin]);
                double mag2 = Math.Sqrt(fftReal[bin] * fftReal[bin] + fftImag[bin] * fftImag[bin]);
                double mag3 = Math.Sqrt(fftReal[nextBin] * fftReal[nextBin] + fftImag[nextBin] * fftImag[nextBin]);

                double avgMag = (mag1 + mag2 * 2.0 + mag3) / 4.0;

                // Scale to normalized dB height
                float target = (float)(avgMag / (FftSize * 0.08));
                target = Math.Clamp(target, 0f, 1f);

                // Attack fast, decay smoothly
                if (target > spectrumLevels[i])
                {
                    spectrumLevels[i] = target;
                }
                else
                {
                    spectrumLevels[i] = spectrumLevels[i] * 0.84f;
                }
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
                while (k <= j)
                {
                    j -= k;
                    k /= 2;
                }
                j += k;
            }

            for (int len = 2; len <= n; len <<= 1)
            {
                double angle = -2.0 * Math.PI / len;
                double wlen_r = Math.Cos(angle);
                double wlen_i = Math.Sin(angle);

                for (int i = 0; i < n; i += len)
                {
                    double wr = 1.0;
                    double wi = 0.0;
                    int half = len >> 1;

                    for (int m = 0; m < half; m++)
                    {
                        double u_r = real[i + m];
                        double u_i = imag[i + m];
                        double v_r = real[i + m + half] * wr - imag[i + m + half] * wi;
                        double v_i = real[i + m + half] * wi + imag[i + m + half] * wr;

                        real[i + m] = u_r + v_r;
                        imag[i + m] = u_i + v_i;
                        real[i + m + half] = u_r - v_r;
                        imag[i + m + half] = u_i - v_i;

                        double next_wr = wr * wlen_r - wi * wlen_i;
                        wi = wr * wlen_i + wi * wlen_r;
                        wr = next_wr;
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Deep dark studio background
            g.Clear(Color.FromArgb(12, 16, 22));

            int w = Width;
            int h = Height;
            if (w <= 0 || h <= 0) return;

            // Draw frequency grid lines
            using (var gridPen = new Pen(Color.FromArgb(30, 40, 50), 1f))
            {
                g.DrawLine(gridPen, 0, h * 0.33f, w, h * 0.33f);
                g.DrawLine(gridPen, 0, h * 0.66f, w, h * 0.66f);
                g.DrawLine(gridPen, w * 0.33f, 0, w * 0.33f, h);
                g.DrawLine(gridPen, w * 0.66f, 0, w * 0.66f, h);
            }

            // Build continuous curve points
            var curvePoints = new PointF[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                float px = (float)i / (PointCount - 1) * w;
                float py = (h - 4) - (spectrumLevels[i] * (h - 16));
                curvePoints[i] = new PointF(px, Math.Clamp(py, 4, h - 2));
            }

            // Create closed polygon for glowing copper mountain fill
            var polyPoints = new PointF[PointCount + 2];
            Array.Copy(curvePoints, polyPoints, PointCount);
            polyPoints[PointCount] = new PointF(w, h);
            polyPoints[PointCount + 1] = new PointF(0, h);

            // Copper / Amber gradient fill (matching tutorial video spectrum)
            using (var brush = new LinearGradientBrush(
                new PointF(0, 0),
                new PointF(0, h),
                Color.FromArgb(220, 240, 150, 50),   // Bright amber at top
                Color.FromArgb(180, 50, 20, 5)))     // Deep rich copper at bottom
            {
                g.FillPolygon(brush, polyPoints);
            }

            // Golden / Orange contour edge
            using (var contourPen = new Pen(Color.FromArgb(255, 190, 70), 2.2f))
            {
                g.DrawCurve(contourPen, curvePoints, 0.4f);
            }

            // Axis labels
            using (var textBrush = new SolidBrush(Color.FromArgb(160, 180, 200)))
            using (var font = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            {
                g.DrawString("SPECTRUM (LOW BASS)", font, textBrush, 6, 4);
                g.DrawString("HIGHS", font, textBrush, w - 42, 4);
            }
        }
    }
}
