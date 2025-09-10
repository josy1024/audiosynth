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

        private int currentOctave = 0;

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

        // private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();
        private readonly HashSet<(Keys key, bool alt)> heldKeys = new HashSet<(Keys, bool)>();

        private readonly ConcurrentQueue<string> keyHistory = new ConcurrentQueue<string>();
        private const int maxHistory = 10;

        public KeyPlayerForm()
        {
            InitializeComponent();
            this.Text = "Synth Player";
            this.KeyPreview = true;
            synthEngine = new SynthEngine();
            PopulateWaveTypeComboBox();
            UpdateOctaveLabel();
        }

        private void KeyPlayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            float frequency = 0;
            bool isAlt = e.Alt;

            // Corrected: Always get the selected wave type from the ComboBox
            WaveType selectedWaveType = (WaveType)comboBoxWaveType.SelectedItem;


            // Create a unique key identifier
            var keyId = (e.KeyCode, isAlt);

            if (e.KeyCode == Keys.X)
            {
                CycleWaveType();
                return;
            }

            if (e.KeyCode == Keys.MediaPreviousTrack)
            {
                currentOctave--;
                UpdateOctaveLabel();
                UpdateAllActiveNotes();
                return;
            }

            if (e.KeyCode == Keys.MediaNextTrack)
            {
                currentOctave++;
                UpdateOctaveLabel();
                UpdateAllActiveNotes();
                return;
            }



            bool isMusicalNote = isAlt ? baseAltNoteFrequencies.TryGetValue(e.KeyCode, out frequency) : baseNoteFrequencies.TryGetValue(e.KeyCode, out frequency);


            if (isMusicalNote)
            {
                // If a key is already held, update both its frequency and wave type
                if (heldKeys.Contains(keyId))
                {
                    // Note is already held, update it
                    synthEngine.UpdateNote(e.KeyCode, AdjustFrequencyForOctave(frequency), selectedWaveType);
                }
                else
                {
                    // New note, add it and start it
                    synthEngine.NoteOn(e.KeyCode, AdjustFrequencyForOctave(frequency), selectedWaveType);
                    heldKeys.Add(keyId);
                }

                textBoxFoo.Text = e.KeyCode.ToString();

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

        private void KeyPlayerForm_KeyUp(object sender, KeyEventArgs e)
        {
            var keyId = (e.KeyCode, e.Alt);
            if (heldKeys.Remove(keyId)) // Remove the key if it was in the set
            {
                synthEngine.NoteOff(e.KeyCode);
            }
        }

        private void UpdateAllActiveNotes()
        {
            WaveType currentWaveType = (WaveType)comboBoxWaveType.SelectedItem;
            foreach (var keyId in heldKeys)
            {
                float frequency = 0;
                // Use keyId.key and keyId.alt to get the correct frequency
                if (keyId.alt)
                {
                    if (baseAltNoteFrequencies.TryGetValue(keyId.key, out frequency))
                    {
                        float newFrequency = AdjustFrequencyForOctave(frequency);
                        synthEngine.UpdateNote(keyId.key, newFrequency, currentWaveType);
                    }
                }
                else
                {
                    if (baseNoteFrequencies.TryGetValue(keyId.key, out frequency))
                    {
                        float newFrequency = AdjustFrequencyForOctave(frequency);
                        synthEngine.UpdateNote(keyId.key, newFrequency, currentWaveType);
                    }
                }
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

        private void CycleWaveType()
        {
            int nextIndex = (comboBoxWaveType.SelectedIndex + 1) % comboBoxWaveType.Items.Count;
            comboBoxWaveType.SelectedIndex = nextIndex;

            UpdateAllActiveNotes();
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

        private void Reset_Click(object sender, EventArgs e)
        {
            synthEngine.Reset();
            heldKeys.Clear();
            textBoxFoo.Text = string.Empty;
            textBoxHistory.Text = string.Empty;
        }
    }
}