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

        // Tracks the current octave offset from the base (C4)
        private int currentOctave = 0;
        private int currentWaveTypeIndex = 0;

        // Base frequencies for notes without alt modifier
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

        // Base frequencies for notes with alt modifier (sharps)
        private readonly Dictionary<Keys, float> baseAltNoteFrequencies = new Dictionary<Keys, float>
        {
            { Keys.C, 277.18f }, // C#
            { Keys.D, 311.13f }, // D#
            { Keys.F, 369.99f }, // F#
            { Keys.G, 415.30f }, // G#
            { Keys.A, 466.16f }  // A#
        };

        // Tracks keys currently being held down to allow for octave changes
        private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();

        // For displaying key history
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
            // The 'X' key directly cycles the waveform
            if (e.KeyCode == Keys.X)
            {
                CycleWaveType();
                return;
            }

            // Octave switching with Previous Track / Next Track keys
            if (e.KeyCode == Keys.MediaPreviousTrack)
            {
                currentOctave--;
                UpdateOctaveLabel();
                UpdateAllActiveNoteFrequencies(); // Update held notes' frequencies
                return;
            }

            if (e.KeyCode == Keys.MediaNextTrack)
            {
                currentOctave++;
                UpdateOctaveLabel();
                UpdateAllActiveNoteFrequencies(); // Update held notes' frequencies
                return;
            }

            // Get the frequency and wave type for the pressed key
            float frequency = 0;
            WaveType selectedWaveType = (WaveType)Enum.GetValues(typeof(WaveType)).GetValue(currentWaveTypeIndex);

            bool isMusicalNote = false;
            if (e.Alt)
            {
                isMusicalNote = baseAltNoteFrequencies.TryGetValue(e.KeyCode, out frequency);
            }
            else
            {
                isMusicalNote = baseNoteFrequencies.TryGetValue(e.KeyCode, out frequency);
            }

            // If a musical note key is pressed
            if (isMusicalNote)
            {
                // Check if the key is already held
                if (heldKeys.Contains(e.KeyCode))
                {
                    // If the key is already held, update its frequency instead of starting a new note
                    synthEngine.UpdateNoteFrequency(e.KeyCode, AdjustFrequencyForOctave(frequency));
                }
                else
                {
                    // If the key is not held, start a new note
                    heldKeys.Add(e.KeyCode); // Track the held key
                    synthEngine.NoteOn(e.KeyCode, AdjustFrequencyForOctave(frequency), selectedWaveType);
                }

                // Update the last pressed key display
                textBoxFoo.Text = e.KeyCode.ToString();

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

        private void KeyPlayerForm_KeyUp(object sender, KeyEventArgs e)
        {
            // Stop the note and remove the key from the held keys set
            if (heldKeys.Contains(e.KeyCode))
            {
                synthEngine.NoteOff(e.KeyCode);
                heldKeys.Remove(e.KeyCode);
            }
        }

        // Updates the frequencies of all currently playing notes
        private void UpdateAllActiveNoteFrequencies()
        {
            foreach (var key in heldKeys)
            {
                float frequency = 0;
                if (baseNoteFrequencies.TryGetValue(key, out frequency))
                {
                    float newFrequency = AdjustFrequencyForOctave(frequency);
                    synthEngine.UpdateNoteFrequency(key, newFrequency);
                }
                else if (baseAltNoteFrequencies.TryGetValue(key, out frequency))
                {
                    float newFrequency = AdjustFrequencyForOctave(frequency);
                    synthEngine.UpdateNoteFrequency(key, newFrequency);
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
            currentWaveTypeIndex = (currentWaveTypeIndex + 1) % comboBoxWaveType.Items.Count;
            comboBoxWaveType.SelectedIndex = currentWaveTypeIndex;

            UpdateAllActiveNoteWaveTypes();
        }

        private void UpdateAllActiveNoteWaveTypes()
        {
            WaveType newWaveType = (WaveType)Enum.GetValues(typeof(WaveType)).GetValue(currentWaveTypeIndex);
            foreach (var key in heldKeys)
            {
                synthEngine.UpdateNoteWaveType(key, newWaveType);
            }
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
        private void textBoxFoo_TextChanged(object sender, EventArgs e)
        {

        }

    }
}