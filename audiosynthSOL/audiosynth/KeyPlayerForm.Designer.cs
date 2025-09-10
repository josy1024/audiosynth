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
            SuspendLayout();
            // 
            // labelOctave
            // 
            labelOctave.AutoSize = true;
            labelOctave.Location = new Point(26, 22);
            labelOctave.Margin = new Padding(6, 0, 6, 0);
            labelOctave.Name = "labelOctave";
            labelOctave.Size = new Size(113, 32);
            labelOctave.TabIndex = 0;
            labelOctave.Text = "Octave: 4";
            // 
            // comboBoxWaveType
            // 
            comboBoxWaveType.FormattingEnabled = true;
            comboBoxWaveType.Location = new Point(668, 22);
            comboBoxWaveType.Margin = new Padding(6, 7, 6, 7);
            comboBoxWaveType.Name = "comboBoxWaveType";
            comboBoxWaveType.Size = new Size(258, 40);
            comboBoxWaveType.TabIndex = 1;
            comboBoxWaveType.TabStop = false;
            // 
            // textBoxHistory
            // 
            textBoxHistory.Location = new Point(26, 174);
            textBoxHistory.Margin = new Padding(6, 7, 6, 7);
            textBoxHistory.Multiline = true;
            textBoxHistory.Name = "textBoxHistory";
            textBoxHistory.ReadOnly = true;
            textBoxHistory.Size = new Size(559, 83);
            textBoxHistory.TabIndex = 2;
            textBoxHistory.TabStop = false;
            // 
            // labelWaveTypeMode
            // 
            labelWaveTypeMode.AutoSize = true;
            labelWaveTypeMode.Location = new Point(26, 66);
            labelWaveTypeMode.Margin = new Padding(6, 0, 6, 0);
            labelWaveTypeMode.Name = "labelWaveTypeMode";
            labelWaveTypeMode.Size = new Size(202, 32);
            labelWaveTypeMode.TabIndex = 3;
            labelWaveTypeMode.Text = "Mode: Play Notes";
            // 
            // textBoxFoo
            // 
            textBoxFoo.Location = new Point(726, 188);
            textBoxFoo.Name = "textBoxFoo";
            textBoxFoo.Size = new Size(200, 39);
            textBoxFoo.TabIndex = 0;
            textBoxFoo.TextChanged += textBoxFoo_TextChanged;
            // 
            // KeyPlayerForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1592, 851);
            Controls.Add(textBoxFoo);
            Controls.Add(labelWaveTypeMode);
            Controls.Add(textBoxHistory);
            Controls.Add(comboBoxWaveType);
            Controls.Add(labelOctave);
            Margin = new Padding(6, 7, 6, 7);
            Name = "KeyPlayerForm";
            Text = "KeyPlayerForm";
            KeyDown += KeyPlayerForm_KeyDown;
            KeyUp += KeyPlayerForm_KeyUp;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label labelOctave;
        private System.Windows.Forms.ComboBox comboBoxWaveType;
        private System.Windows.Forms.TextBox textBoxHistory;
        private System.Windows.Forms.Label labelWaveTypeMode;
        private TextBox textBoxFoo;
    }
}