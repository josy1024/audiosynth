namespace audiosynth
{
    partial class KeyPlayerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            labelOctave = new Label();
            comboBoxWaveType = new ComboBox();
            textBoxHistory = new TextBox();
            labelWaveTypeMode = new Label();
            textBoxFoo = new TextBox();
            Reset = new Button();
            ModulatorFrequencyTrackBar = new TrackBar();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)ModulatorFrequencyTrackBar).BeginInit();
            SuspendLayout();
            // 
            // labelOctave
            // 
            labelOctave.AutoSize = true;
            labelOctave.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelOctave.Location = new Point(14, 10);
            labelOctave.Name = "labelOctave";
            labelOctave.Size = new Size(89, 25);
            labelOctave.TabIndex = 0;
            labelOctave.Text = "Octave: 4";
            // 
            // comboBoxWaveType
            // 
            comboBoxWaveType.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxWaveType.FormattingEnabled = true;
            comboBoxWaveType.Location = new Point(360, 10);
            comboBoxWaveType.Name = "comboBoxWaveType";
            comboBoxWaveType.Size = new Size(141, 33);
            comboBoxWaveType.TabIndex = 1;
            comboBoxWaveType.TabStop = false;
            comboBoxWaveType.KeyPress += comboBoxWaveType_KeyPress;
            // 
            // textBoxHistory
            // 
            textBoxHistory.Location = new Point(14, 97);
            textBoxHistory.Multiline = true;
            textBoxHistory.Name = "textBoxHistory";
            textBoxHistory.ReadOnly = true;
            textBoxHistory.Size = new Size(303, 41);
            textBoxHistory.TabIndex = 2;
            textBoxHistory.TabStop = false;
            // 
            // labelWaveTypeMode
            // 
            labelWaveTypeMode.AutoSize = true;
            labelWaveTypeMode.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelWaveTypeMode.Location = new Point(14, 45);
            labelWaveTypeMode.Name = "labelWaveTypeMode";
            labelWaveTypeMode.Size = new Size(159, 25);
            labelWaveTypeMode.TabIndex = 3;
            labelWaveTypeMode.Text = "Mode: Play Notes";
            // 
            // textBoxFoo
            // 
            textBoxFoo.Location = new Point(391, 88);
            textBoxFoo.Margin = new Padding(2, 1, 2, 1);
            textBoxFoo.Name = "textBoxFoo";
            textBoxFoo.Size = new Size(110, 23);
            textBoxFoo.TabIndex = 0;
            // 
            // Reset
            // 
            Reset.Location = new Point(22, 224);
            Reset.Margin = new Padding(2, 1, 2, 1);
            Reset.Name = "Reset";
            Reset.Size = new Size(81, 22);
            Reset.TabIndex = 4;
            Reset.Text = "Reset";
            Reset.UseVisualStyleBackColor = true;
            Reset.Click += Reset_Click;
            // 
            // ModulatorFrequencyTrackBar
            // 
            ModulatorFrequencyTrackBar.Location = new Point(622, 10);
            ModulatorFrequencyTrackBar.Margin = new Padding(2, 1, 2, 1);
            ModulatorFrequencyTrackBar.Name = "ModulatorFrequencyTrackBar";
            ModulatorFrequencyTrackBar.Size = new Size(221, 45);
            ModulatorFrequencyTrackBar.TabIndex = 5;
            ModulatorFrequencyTrackBar.Scroll += ModulatorFrequencyTrackBar_Scroll;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(142, 220);
            label1.Name = "label1";
            label1.Size = new Size(135, 25);
            label1.TabIndex = 6;
            label1.Text = "x = Instrument";
            label1.Click += label1_Click;
            // 
            // KeyPlayerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(857, 399);
            Controls.Add(label1);
            Controls.Add(ModulatorFrequencyTrackBar);
            Controls.Add(Reset);
            Controls.Add(textBoxFoo);
            Controls.Add(labelWaveTypeMode);
            Controls.Add(textBoxHistory);
            Controls.Add(comboBoxWaveType);
            Controls.Add(labelOctave);
            Name = "KeyPlayerForm";
            Text = "KeyPlayerForm";
            KeyDown += KeyPlayerForm_KeyDown;
            KeyUp += KeyPlayerForm_KeyUp;
            ((System.ComponentModel.ISupportInitialize)ModulatorFrequencyTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label labelOctave;
        private System.Windows.Forms.ComboBox comboBoxWaveType;
        private System.Windows.Forms.TextBox textBoxHistory;
        private System.Windows.Forms.Label labelWaveTypeMode;
        private TextBox textBoxFoo;
        private Button Reset;
        private TrackBar ModulatorFrequencyTrackBar;
        private Label label1;
    }
}