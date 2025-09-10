using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Linq;

namespace audiosynth
{
    public partial class KeyPlayerForm : Form
    {
        private readonly SynthEngine synthEngine;

        private int currentOctave = 0; // Default to C4, D4, etc.

        private readonly Dictionary<Keys, float> baseNoteFrequencies = new Dictionary<Keys, float>
        {
            { Keys.C, 261.63f },
            { Keys.D, 293.66f },
            { Keys.E, 329.63f },
            { Keys.F, 349.23f },
            { Keys.G, 392.00f },
            { Keys.A, 440.00f },
            { Keys.B, 493.88f }
        };

        private readonly Dictionary<Keys, float> baseAltNoteFrequencies = new Dictionary<Keys, float>
        {
            { Keys.C, 277.18f }, // C#
            { Keys.D, 311.13f }, // D#
            { Keys.F, 369.99f }, // F#
            { Keys.G, 415.30f }, // G#
            { Keys.A, 466.16f }  // A#
        };

        private readonly ConcurrentQueue<string> keyHistory = new ConcurrentQueue<string>();
        private const int maxHistory = 10;
        private bool isWaveTypeMode = false;

        public KeyPlayerForm()
        {
            InitializeComponent();
            this.Text = "Synth Player";
            this.KeyPreview = true;
            synthEngine = new SynthEngine();
            PopulateWaveTypeComboBox();
            UpdateOctaveLabel();
            UpdateWaveTypeModeLabel();
        }

        // This method is called from the designer's KeyDown event handler
        private void KeyPlayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            textBoxFoo.Text = e.KeyCode.ToString();

            if (e.KeyCode == Keys.X)
            {
                isWaveTypeMode = !isWaveTypeMode;
                UpdateWaveTypeModeLabel();
                return;
            }

            if (isWaveTypeMode)
            {
                HandleWaveTypeSelection(e.KeyCode);
                return;
            }

            // Octave switching with Previous Track / Next Track keys
            if (e.KeyCode == Keys.MediaPreviousTrack)
            {
                currentOctave--;
                UpdateOctaveLabel();
                return;
            }

            if (e.KeyCode == Keys.MediaNextTrack)
            {
                currentOctave++;
                UpdateOctaveLabel();
                return;
            }

            // Check if a musical note key is being pressed
            bool isMusicalNote = baseNoteFrequencies.ContainsKey(e.KeyCode) || baseAltNoteFrequencies.ContainsKey(e.KeyCode);

            if (isMusicalNote)
            {

                float frequency = 0;
                WaveType selectedWaveType = (WaveType)comboBoxWaveType.SelectedIndex;

                // The AdjustFrequencyForOctave method uses the currentOctave value
                if (e.Alt)
                {
                    if (baseAltNoteFrequencies.TryGetValue(e.KeyCode, out frequency))
                    {
                        synthEngine.NoteOn(e.KeyCode, AdjustFrequencyForOctave(frequency), selectedWaveType);
                    }
                }
                else
                {
                    if (baseNoteFrequencies.TryGetValue(e.KeyCode, out frequency))
                    {
                        synthEngine.NoteOn(e.KeyCode, AdjustFrequencyForOctave(frequency), selectedWaveType);
                    }
                }

                // Update key history
                string keyName = e.KeyCode.ToString();
                if (e.Alt) keyName = "Alt+" + keyName;

                if (keyHistory.Count >= maxHistory)
                {
                    keyHistory.TryDequeue(out _);
                }
                keyHistory.Enqueue(keyName);
                UpdateKeyHistoryDisplay();
            }
        }

        // This method is called from the designer's KeyUp event handler
        private void KeyPlayerForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (baseNoteFrequencies.ContainsKey(e.KeyCode) || baseAltNoteFrequencies.ContainsKey(e.KeyCode))
            {
                synthEngine.NoteOff(e.KeyCode);
            }
        }

        private float AdjustFrequencyForOctave(float baseFreq)
        {
            return (float)(baseFreq * Math.Pow(2, currentOctave));
        }

        private void PopulateWaveTypeComboBox()
        {
            comboBoxWaveType.DataSource = Enum.GetValues(typeof(WaveType));
            comboBoxWaveType.SelectedIndex = 0;
        }

        private void UpdateKeyHistoryDisplay()
        {
            var historyArray = keyHistory.ToArray();
            Array.Reverse(historyArray);
            textBoxHistory.Text = string.Join(" - ", historyArray);
        }

        private void UpdateOctaveLabel()
        {
            int displayOctave = 4 + currentOctave;
            labelOctave.Text = $"Octave: {displayOctave}";
        }

        private void UpdateWaveTypeModeLabel()
        {
            labelWaveTypeMode.Text = isWaveTypeMode ? "Mode: Wave Select" : "Mode: Play Notes";
        }

        private void HandleWaveTypeSelection(Keys key)
        {
            switch (key)
            {
                case Keys.D1:
                    comboBoxWaveType.SelectedIndex = 0; // Sine
                    isWaveTypeMode = false;
                    break;
                case Keys.D2:
                    comboBoxWaveType.SelectedIndex = 1; // Saw
                    isWaveTypeMode = false;
                    break;
                case Keys.D3:
                    comboBoxWaveType.SelectedIndex = 2; // Square
                    isWaveTypeMode = false;
                    break;
                case Keys.D4:
                    comboBoxWaveType.SelectedIndex = 3; // Triangle
                    isWaveTypeMode = false;
                    break;
            }
            UpdateWaveTypeModeLabel();
        }

        private void textBoxFoo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}