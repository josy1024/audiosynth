namespace audiosynth
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Security.AccessControl;
    using System.Windows.Forms;

    public partial class KeyPlayerForm : Form
    {
        private System.ComponentModel.IContainer components = null;

        private readonly SynthEngine synthEngine;

        private readonly Dictionary<Keys, float> noteFrequencies = new Dictionary<Keys, float>
        {
            { Keys.C, 261.63f },
            { Keys.D, 293.66f },
            { Keys.E, 329.63f },
            { Keys.F, 349.23f },
            { Keys.G, 392.00f },
            { Keys.A, 440.00f },
            { Keys.B, 493.88f },
            { Keys.H, 493.88f }
        };

        private readonly Dictionary<Keys, float> altNoteFrequencies = new Dictionary<Keys, float>
        {
            { Keys.C, 277.18f }, // C#
            { Keys.D, 311.13f }, // D#
            { Keys.F, 369.99f }, // F#
            { Keys.G, 415.30f }, // G#
            { Keys.A, 466.16f }  // A#
        };

        private readonly ConcurrentQueue<string> keyHistory = new ConcurrentQueue<string>();
        private const int maxHistory = 5;
        public KeyPlayerForm()
        {
            InitializeComponent();
            this.Text = "Synth Player";
            this.KeyPreview = true;
            synthEngine = new SynthEngine();
            PopulateWaveTypeComboBox();
        }

        private void KeyPlayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            float frequency = 0;
            WaveType selectedWaveType = (WaveType)comboBoxWaveType.SelectedIndex;

            if (noteFrequencies.ContainsKey(e.KeyCode) || altNoteFrequencies.ContainsKey(e.KeyCode))
            {
                // Disable the combo box to prevent changes while playing
                comboBoxWaveType.Enabled = false;
            }

            if ((e.Control) || (e.Alt))
            {
                if (altNoteFrequencies.TryGetValue(e.KeyCode, out frequency))
                {
                    synthEngine.NoteOn(e.KeyCode, frequency, selectedWaveType);
                }
            }
            else
            {
                if (noteFrequencies.TryGetValue(e.KeyCode, out frequency))
                {
                    synthEngine.NoteOn(e.KeyCode, frequency, selectedWaveType);
                }
            }

            // Update key history
            string keyName = e.KeyCode.ToString();
            if (!keyHistory.Contains(keyName))
            {
                if (keyHistory.Count >= maxHistory)
                {
                    keyHistory.TryDequeue(out _);
                }
                keyHistory.Enqueue(keyName);
                UpdateKeyHistoryDisplay();
            }
        }

        // This method will be linked to the KeyUp event in the designer
        private void KeyPlayerForm_KeyUp(object sender, KeyEventArgs e)
        {
            comboBoxWaveType.Enabled = true;
            if (noteFrequencies.ContainsKey(e.KeyCode) || altNoteFrequencies.ContainsKey(e.KeyCode))
            {
                synthEngine.NoteOff(e.KeyCode);
            }
        }

        private void PopulateWaveTypeComboBox()
        {
            comboBoxWaveType.DataSource = Enum.GetValues(typeof(WaveType));
            comboBoxWaveType.SelectedIndex = 0; // Default to Sine wave
        }

        private void UpdateKeyHistoryDisplay()
        {
            // Assuming you have a TextBox named textBoxHistory on your form
            textBoxHistory.Text = string.Join(" - ", keyHistory.ToArray());
        }

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBoxHistory = new TextBox();
            comboBoxWaveType = new ComboBox();
            SuspendLayout();
            // 
            // textBoxHistory
            // 
            textBoxHistory.Location = new Point(540, 12);
            textBoxHistory.Multiline = true;
            textBoxHistory.Name = "textBoxHistory";
            textBoxHistory.ScrollBars = ScrollBars.Vertical;
            textBoxHistory.Size = new Size(248, 117);
            textBoxHistory.TabIndex = 0;
            textBoxHistory.ReadOnly = true;
            // 
            // comboBoxWaveType
            // 
            comboBoxWaveType.FormattingEnabled = true;
            comboBoxWaveType.Location = new Point(12, 12);
            comboBoxWaveType.Name = "comboBoxWaveType";
            comboBoxWaveType.Size = new Size(204, 23);
            //comboBoxWaveType.TabIndex = 1;
            // 
            // KeyPlayerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(comboBoxWaveType);
            Controls.Add(textBoxHistory);
            Name = "KeyPlayerForm";
            Text = "KeyPlayerForm";
            KeyDown += KeyPlayerForm_KeyDown;
            KeyUp += KeyPlayerForm_KeyUp;
            ResumeLayout(false);
            PerformLayout();
        }
        private TextBox textBoxHistory;
        private ComboBox comboBoxWaveType;

    }
}
