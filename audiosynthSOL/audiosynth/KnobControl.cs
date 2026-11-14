using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace audiosynth
{
    /// <summary>
    /// Professional rotary knob control for synthesizer parameters:
    /// - Drag-up/down or scroll wheel to adjust
    /// - Right-click to toggle parameter BYPASS (no impact on wave)
    /// - Displays title, value, and tooltip description
    /// </summary>
    public partial class KnobControl : UserControl
    {
        private float minimum = 0f;
        private float maximum = 100f;
        private float currentValue = 50f;
        private float defaultValue = 50f;

        private string title = "KNOB";
        private string valueFormat = "{0:F0}";
        private string description = "";

        private bool isBypassed = false;
        private bool isDragging = false;
        private int lastMouseY = 0;

        private readonly ToolTip toolTip = new ToolTip();

        public event EventHandler? ValueChanged;
        public event EventHandler? BypassChanged;
        public event EventHandler<string>? DescriptionHovered;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Minimum
        {
            get => minimum;
            set { minimum = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Maximum
        {
            get => maximum;
            set { maximum = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Value
        {
            get => currentValue;
            set
            {
                float clamped = Math.Clamp(value, minimum, maximum);
                if (Math.Abs(clamped - currentValue) > 0.0001f)
                {
                    currentValue = clamped;
                    Invalidate();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float DefaultValue
        {
            get => defaultValue;
            set => defaultValue = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get => title;
            set { title = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ValueFormat
        {
            get => valueFormat;
            set { valueFormat = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Description
        {
            get => description;
            set
            {
                description = value;
                toolTip.SetToolTip(this, $"{title}: {description}\r\n(Right-click to Toggle Bypass)");
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBypassed
        {
            get => isBypassed;
            set
            {
                if (isBypassed != value)
                {
                    isBypassed = value;
                    Invalidate();
                    BypassChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ArcColor { get; set; } = Color.FromArgb(245, 145, 45);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color KnobColor { get; set; } = Color.FromArgb(32, 36, 44);

        public KnobControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Size = new Size(76, 88);

            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 400;
            toolTip.ReshowDelay = 200;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            DescriptionHovered?.Invoke(this, $"{title}: {description} (Right-click to Bypass)");
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Right)
            {
                // Right click toggles parameter bypass!
                IsBypassed = !IsBypassed;
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMouseY = e.Y;
                Capture = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Capture = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging && !isBypassed)
            {
                int deltaY = lastMouseY - e.Y;
                lastMouseY = e.Y;

                if (deltaY != 0)
                {
                    float range = maximum - minimum;
                    float step = range / 120.0f;
                    Value += deltaY * step;
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!isBypassed)
            {
                float range = maximum - minimum;
                float step = (range / 40.0f) * (e.Delta > 0 ? 1 : -1);
                Value += step;
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (!isBypassed)
            {
                Value = defaultValue;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = Width;
            int h = Height;
            if (w <= 0 || h <= 0) return;

            // 1. Draw Title Text
            Color titleColor = isBypassed ? Color.FromArgb(140, 60, 60) : Color.FromArgb(170, 185, 200);
            using (var titleBrush = new SolidBrush(titleColor))
            using (var titleFont = new Font("Segoe UI", 7.0f, FontStyle.Bold))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(title, titleFont, titleBrush, w * 0.5f, 2, sf);
            }

            // Knob Geometry
            float knobDiameter = Math.Min(w - 18, 42);
            float knobX = (w - knobDiameter) * 0.5f;
            float knobY = 16f;

            const float startAngle = 135f;
            const float totalSweep = 270f;
            float fraction = (maximum > minimum) ? (currentValue - minimum) / (maximum - minimum) : 0f;
            fraction = Math.Clamp(fraction, 0f, 1f);
            float currentSweep = totalSweep * fraction;

            var arcRect = new RectangleF(knobX - 4, knobY - 4, knobDiameter + 8, knobDiameter + 8);

            // 2. Background Track Arc
            using (var trackPen = new Pen(Color.FromArgb(42, 48, 56), 3.0f))
            {
                trackPen.StartCap = LineCap.Round;
                trackPen.EndCap = LineCap.Round;
                g.DrawArc(trackPen, arcRect, startAngle, totalSweep);
            }

            // 3. Active Arc or Bypassed Indicator
            if (isBypassed)
            {
                // Muted red dashed bypass ring
                using (var bypassPen = new Pen(Color.FromArgb(160, 50, 50), 2.5f))
                {
                    bypassPen.DashStyle = DashStyle.Dot;
                    g.DrawArc(bypassPen, arcRect, startAngle, totalSweep);
                }
            }
            else if (currentSweep > 1f)
            {
                using (var activePen = new Pen(ArcColor, 3.2f))
                {
                    activePen.StartCap = LineCap.Round;
                    activePen.EndCap = LineCap.Round;
                    g.DrawArc(activePen, arcRect, startAngle, currentSweep);
                }
            }

            // 4. Center Knob Cap
            var knobRect = new RectangleF(knobX, knobY, knobDiameter, knobDiameter);
            Color topGrad = isBypassed ? Color.FromArgb(40, 30, 30) : Color.FromArgb(50, 56, 68);
            Color btmGrad = isBypassed ? Color.FromArgb(25, 18, 18) : Color.FromArgb(22, 25, 32);

            using (var knobBrush = new LinearGradientBrush(knobRect, topGrad, btmGrad, LinearGradientMode.ForwardDiagonal))
            {
                g.FillEllipse(knobBrush, knobRect);
            }

            Color borderCol = isBypassed ? Color.FromArgb(120, 45, 45) : Color.FromArgb(75, 85, 100);
            using (var borderPen = new Pen(borderCol, 1f))
            {
                g.DrawEllipse(borderPen, knobRect);
            }

            // 5. Indicator Line pointing at current angle (or slash when bypassed)
            float angleDeg = startAngle + currentSweep;
            float angleRad = angleDeg * (float)Math.PI / 180f;

            float centerX = knobX + knobDiameter * 0.5f;
            float centerY = knobY + knobDiameter * 0.5f;

            if (isBypassed)
            {
                // Draw small red "BYPASS" text across knob
                using (var bBrush = new SolidBrush(Color.FromArgb(230, 80, 80)))
                using (var bFont = new Font("Segoe UI", 6.5f, FontStyle.Bold))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("BYPASS", bFont, bBrush, centerX, centerY, sf);
                }
            }
            else
            {
                float innerRadius = knobDiameter * 0.15f;
                float outerRadius = knobDiameter * 0.44f;

                float p1X = centerX + innerRadius * (float)Math.Cos(angleRad);
                float p1Y = centerY + innerRadius * (float)Math.Sin(angleRad);
                float p2X = centerX + outerRadius * (float)Math.Cos(angleRad);
                float p2Y = centerY + outerRadius * (float)Math.Sin(angleRad);

                using (var ptrPen = new Pen(Color.White, 2.0f))
                {
                    ptrPen.StartCap = LineCap.Round;
                    ptrPen.EndCap = LineCap.Round;
                    g.DrawLine(ptrPen, p1X, p1Y, p2X, p2Y);
                }
            }

            // 6. Value text at bottom
            string valText;
            if (isBypassed)
            {
                valText = "[OFF]";
            }
            else
            {
                try { valText = string.Format(valueFormat, currentValue); }
                catch { valText = currentValue.ToString("F1"); }
            }

            Color valColor = isBypassed ? Color.FromArgb(180, 80, 80) : Color.White;
            using (var valBrush = new SolidBrush(valColor))
            using (var valFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(valText, valFont, valBrush, w * 0.5f, h - 16, sf);
            }
        }
    }
}
