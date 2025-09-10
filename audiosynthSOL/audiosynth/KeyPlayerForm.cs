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

        private readonly Dictionary<Keys, float> noteFrequencies = new Dictionary<Keys, float>
        {
            { Keys.C, 261.63f },
            { Keys.D, 293.66f },
            { Keys.E, 329.63f },
            { Keys.F, 349.23f },
            { Keys.G, 392.00f },
            { Keys.A, 440.00f },
            { Keys.B, 493.88f },
             // Sharp notes
            { Keys.Q, 277.18f }, // C#
            { Keys.W, 311.13f }, // D#
            { Keys.V, 369.99f }, // F#
            { Keys.T, 415.30f }, // G#
            { Keys.U, 466.16f }  // A#
        };


        // private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();
        // private readonly HashSet<(Keys key, bool alt)> heldKeys = new HashSet<(Keys, bool)>();
        private HashSet<Keys> heldKeys = new HashSet<Keys>();


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

            ModulatorFrequencyTrackBar.Minimum = 100;
            ModulatorFrequencyTrackBar.Maximum = 4400;
            ModulatorFrequencyTrackBar.Value = 616;

            // Set an initial value within the new range
        }
        private void comboBoxWaveType_KeyPress(object sender, KeyPressEventArgs e)
        {
            // This line tells the ComboBox to ignore the key press
            e.Handled = true;
        }
        private void KeyPlayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            float frequency = 0;
            bool isAlt = false;

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



            bool isMusicalNote = noteFrequencies.TryGetValue(e.KeyCode, out frequency);


            if (isMusicalNote)
            {
                // If a key is already held, update both its frequency and wave type
                if (heldKeys.Contains(e.KeyCode))
                {
                    // Note is already held, update it
                    synthEngine.UpdateNote(e.KeyCode, AdjustFrequencyForOctave(frequency), selectedWaveType);
                }
                else
                {
                    // New note, add it and start it
                    synthEngine.NoteOn(e.KeyCode, AdjustFrequencyForOctave(frequency), selectedWaveType);
                    heldKeys.Add(e.KeyCode);
                }

                textBoxFoo.Text = e.KeyCode.ToString();

                string keyName = e.KeyCode.ToString();

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
            heldKeys.Remove(e.KeyCode);
            synthEngine.NoteOff(e.KeyCode);
        }

        private void UpdateAllActiveNotes()
        {
            WaveType currentWaveType = (WaveType)comboBoxWaveType.SelectedItem;
            foreach (var keyId in heldKeys)
            {
                float frequency = 0;

                if (noteFrequencies.TryGetValue(keyId, out frequency))
                {
                    float newFrequency = AdjustFrequencyForOctave(frequency);
                    synthEngine.UpdateNote(keyId, newFrequency, currentWaveType);
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

        private void volumeSlider1_Load(object sender, EventArgs e)
        {

        }

        private void ModulatorFrequencyTrackBar_Scroll(object sender, EventArgs e)
        {
            // The track bar's value will be used as the new modulator frequency
            double newModFreq = ModulatorFrequencyTrackBar.Value;

            // Update all currently playing notes
            foreach (var keyId in heldKeys)
            {
                synthEngine.UpdateModulatorFrequency(keyId, newModFreq);
            }
        }
    }
}