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
            Reset = new Button();
            ModulatorFrequencyTrackBar = new TrackBar();
            label1 = new Label();
            modulationIndexTrackBar = new TrackBar();
            labelModulationIndex = new Label();
            comboBoxFmMultiplier = new ComboBox();
            labelFmMultiplier = new Label();
            FM = new Label();
            Instrument = new Label();
            waveformViewer = new WaveformViewer();
            ((System.ComponentModel.ISupportInitialize)ModulatorFrequencyTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)modulationIndexTrackBar).BeginInit();
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
            comboBoxWaveType.Location = new Point(374, 7);
            comboBoxWaveType.Name = "comboBoxWaveType";
            comboBoxWaveType.Size = new Size(141, 33);
            comboBoxWaveType.TabIndex = 1;
            comboBoxWaveType.TabStop = false;
            comboBoxWaveType.KeyPress += comboBoxWaveType_KeyPress;
            // 
            // textBoxHistory
            // 
            textBoxHistory.Location = new Point(12, 220);
            textBoxHistory.Multiline = true;
            textBoxHistory.Name = "textBoxHistory";
            textBoxHistory.ReadOnly = true;
            textBoxHistory.Size = new Size(193, 256);
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
            // Reset
            // 
            Reset.Location = new Point(590, 369);
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
            ModulatorFrequencyTrackBar.Location = new Point(498, 280);
            ModulatorFrequencyTrackBar.Margin = new Padding(2, 1, 2, 1);
            ModulatorFrequencyTrackBar.Name = "ModulatorFrequencyTrackBar";
            ModulatorFrequencyTrackBar.Orientation = Orientation.Vertical;
            ModulatorFrequencyTrackBar.Size = new Size(45, 169);
            ModulatorFrequencyTrackBar.TabIndex = 5;
            ModulatorFrequencyTrackBar.Scroll += ModulatorFrequencyTrackBar_Scroll;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(590, 343);
            label1.Name = "label1";
            label1.Size = new Size(135, 25);
            label1.TabIndex = 6;
            label1.Text = "x = Instrument";
            label1.Click += label1_Click;
            // 
            // modulationIndexTrackBar
            // 
            modulationIndexTrackBar.Location = new Point(788, 45);
            modulationIndexTrackBar.Maximum = 100;
            modulationIndexTrackBar.Name = "modulationIndexTrackBar";
            modulationIndexTrackBar.Orientation = Orientation.Vertical;
            modulationIndexTrackBar.Size = new Size(45, 169);
            modulationIndexTrackBar.TabIndex = 7;
            modulationIndexTrackBar.Scroll += modulationIndexTrackBar_Scroll;
            // 
            // labelModulationIndex
            // 
            labelModulationIndex.AutoSize = true;
            labelModulationIndex.Location = new Point(699, 217);
            labelModulationIndex.Name = "labelModulationIndex";
            labelModulationIndex.Size = new Size(122, 15);
            labelModulationIndex.TabIndex = 8;
            labelModulationIndex.Text = "labelModulationIndex";
            // 
            // comboBoxFmMultiplier
            // 
            comboBoxFmMultiplier.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxFmMultiplier.FormattingEnabled = true;
            comboBoxFmMultiplier.Location = new Point(712, 245);
            comboBoxFmMultiplier.Name = "comboBoxFmMultiplier";
            comboBoxFmMultiplier.Size = new Size(121, 23);
            comboBoxFmMultiplier.TabIndex = 9;
            comboBoxFmMultiplier.SelectedIndexChanged += comboBoxFmMultiplier_SelectedIndexChanged;
            // 
            // labelFmMultiplier
            // 
            labelFmMultiplier.AutoSize = true;
            labelFmMultiplier.Location = new Point(712, 280);
            labelFmMultiplier.Name = "labelFmMultiplier";
            labelFmMultiplier.Size = new Size(100, 15);
            labelFmMultiplier.TabIndex = 10;
            labelFmMultiplier.Text = "labelFmMultiplier";
            // 
            // FM
            // 
            FM.AutoSize = true;
            FM.Location = new Point(788, 18);
            FM.Name = "FM";
            FM.Size = new Size(24, 15);
            FM.TabIndex = 11;
            FM.Text = "FM";
            // 
            // Instrument
            // 
            Instrument.AutoSize = true;
            Instrument.Location = new Point(303, 18);
            Instrument.Name = "Instrument";
            Instrument.Size = new Size(65, 15);
            Instrument.TabIndex = 12;
            Instrument.Text = "Instrument";
            // 
            // waveformViewer
            // 
            waveformViewer.BackColor = Color.Black;
            waveformViewer.ForeColor = Color.LimeGreen;
            waveformViewer.Location = new Point(14, 82);
            waveformViewer.Name = "waveformViewer";
            waveformViewer.Size = new Size(512, 120);
            waveformViewer.TabIndex = 13;
            // 
            // KeyPlayerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(857, 487);
            Controls.Add(waveformViewer);
            Controls.Add(Instrument);
            Controls.Add(FM);
            Controls.Add(labelFmMultiplier);
            Controls.Add(comboBoxFmMultiplier);
            Controls.Add(labelModulationIndex);
            Controls.Add(modulationIndexTrackBar);
            Controls.Add(label1);
            Controls.Add(ModulatorFrequencyTrackBar);
            Controls.Add(Reset);
            Controls.Add(labelWaveTypeMode);
            Controls.Add(textBoxHistory);
            Controls.Add(comboBoxWaveType);
            Controls.Add(labelOctave);
            Name = "KeyPlayerForm";
            Text = "KeyPlayerForm";
            KeyDown += KeyPlayerForm_KeyDown;
            KeyUp += KeyPlayerForm_KeyUp;
            ((System.ComponentModel.ISupportInitialize)ModulatorFrequencyTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)modulationIndexTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label labelOctave;
        private System.Windows.Forms.ComboBox comboBoxWaveType;
        private System.Windows.Forms.TextBox textBoxHistory;
        private System.Windows.Forms.Label labelWaveTypeMode;
        private Button Reset;
        private TrackBar ModulatorFrequencyTrackBar;
        private Label label1;
        private TrackBar modulationIndexTrackBar;
        private Label labelModulationIndex;
        private ComboBox comboBoxFmMultiplier;
        private Label labelFmMultiplier;
        private Label FM;
        private Label Instrument;
        private WaveformViewer waveformViewer;
    }
}