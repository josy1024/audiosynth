using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace audiosynth
{
    public partial class KeyPlayerForm : Form
    {
        private readonly SynthEngine synthEngine;

        // private int currentOctave = 0;
        private int _currentOctave;

        public int CurrentOctave
        {
            get => _currentOctave;
            set
            {
                if (value < -4)
                    _currentOctave = -4;
                else if (value > 20)
                    _currentOctave = 20;
                else
                    _currentOctave = value;
            }
        }

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
        private readonly ConcurrentQueue<NoteCommand> noteCommands = new ConcurrentQueue<NoteCommand>();
        private CancellationTokenSource cts = new CancellationTokenSource();

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

            Task.Run(() => ProcessNoteCommands(cts.Token));
        }
        private void KeyPlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cts.Cancel();
        }

        private void ProcessNoteCommands(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (noteCommands.TryDequeue(out NoteCommand command))
                {
                    switch (command.Action)
                    {
                        case NoteAction.NoteOn:
                            synthEngine.NoteOn(command.KeyCode, command.Frequency, command.WaveType);
                            break;
                        case NoteAction.NoteOff:
                            synthEngine.NoteOff(command.KeyCode);
                            break;
                        case NoteAction.UpdateNote:
                            synthEngine.UpdateNote(command.KeyCode, command.Frequency, command.WaveType);
                            break;
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        private void comboBoxWaveType_KeyPress(object sender, KeyPressEventArgs e)
        {
            // This line tells the ComboBox to ignore the key press
            e.Handled = true;
        }
        private void KeyPlayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Ignore key repeat events
            // Windows Forms KeyEventArgs does not have IsRepeat.
            // To prevent repeats, check if the key is already in heldKeys.
            if (heldKeys.Contains(e.KeyCode))
            {
                return;
            }

            // Handle non-musical keys first
            if (e.KeyCode == Keys.X)
            {
                CycleWaveType();
                return;
            }

            if (e.KeyCode == Keys.MediaPreviousTrack)
            {
                CurrentOctave--;
                UpdateOctaveLabel();
                UpdateAllActiveNotes();
                return;
            }

            if (e.KeyCode == Keys.MediaNextTrack)
            {
                CurrentOctave++;
                UpdateOctaveLabel();
                UpdateAllActiveNotes();
                return;
            }

            // Process musical notes
            if (noteFrequencies.TryGetValue(e.KeyCode, out float baseFrequency))
            {
                // Only enqueue a new command if the key isn't already held
                if (heldKeys.Add(e.KeyCode))
                {
                    float frequency = AdjustFrequencyForOctave(baseFrequency);
                    WaveType selectedWaveType = (WaveType)comboBoxWaveType.SelectedItem;
                    noteCommands.Enqueue(new NoteCommand
                    {
                        Action = NoteAction.NoteOn,
                        KeyCode = e.KeyCode,
                        Frequency = frequency,
                        WaveType = selectedWaveType
                    });
                }

                // UI updates can happen here without blocking
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
            // Only enqueue a NoteOff if the key was actually being tracked as held
            if (heldKeys.Remove(e.KeyCode))
            {
                noteCommands.Enqueue(new NoteCommand
                {
                    Action = NoteAction.NoteOff,
                    KeyCode = e.KeyCode
                });
            }
        }


        private void UpdateAllActiveNotes()
        {
            WaveType currentWaveType = (WaveType)comboBoxWaveType.SelectedItem;
            foreach (var key in heldKeys)
            {
                if (noteFrequencies.TryGetValue(key, out float baseFrequency))
                {
                    float newFrequency = AdjustFrequencyForOctave(baseFrequency);
                    noteCommands.Enqueue(new NoteCommand
                    {
                        Action = NoteAction.UpdateNote,
                        KeyCode = key,
                        Frequency = newFrequency,
                        WaveType = currentWaveType
                    });
                }
            }
        }

        private float AdjustFrequencyForOctave(float baseFreq)
        {
            return (float)(baseFreq * Math.Pow(2, CurrentOctave));
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
            int displayOctave = 4 + CurrentOctave;
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

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}