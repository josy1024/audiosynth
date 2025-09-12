using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

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

        private readonly Dictionary<Keys, Timer> noteOffTimers = new Dictionary<Keys, Timer>();

        private readonly HashSet<Keys> _heldKeys = new HashSet<Keys>();

        public KeyPlayerForm()
        {
            InitializeComponent();
            this.Text = "Synth Player";
            this.KeyPreview = true;
            synthEngine = new SynthEngine();
            PopulateWaveTypeComboBox();
            PopulateFmMultiplierComboBox();
            UpdateOctaveLabel();

            ModulatorFrequencyTrackBar.Minimum = 100;
            ModulatorFrequencyTrackBar.Maximum = 4400;
            ModulatorFrequencyTrackBar.Value = 616;

            modulationIndexTrackBar.Minimum = 0;
            modulationIndexTrackBar.Maximum = 100;
            modulationIndexTrackBar.Value = 17; // Represents 1.7
            labelModulationIndex.Text = $"Modulation Index: 1.7"; // Update the label

            // Set an initial value within the new range

            Task.Run(() => ProcessNoteCommands(cts.Token));
        }
        private void KeyPlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cts.Cancel();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x0100;
            const int WM_KEYUP = 0x0101;

            if (msg.Msg == WM_KEYDOWN)
            {
                // If the key is already in the set, it's a repeat. Return true to stop processing.
                if (heldKeys.Contains(keyData))
                {
                    return true;
                }
                // If it's not a repeat, add it to the set.
                heldKeys.Add(keyData);
            }
            else if (msg.Msg == WM_KEYUP)
            {
                // The key has been released, so remove it from the set.
                heldKeys.Remove(keyData);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void PopulateFmMultiplierComboBox()
        {
            // Add common harmonic ratios to the ComboBox
            comboBoxFmMultiplier.Items.Clear();
            comboBoxFmMultiplier.Items.AddRange(new object[] { 1, 2, 3, 4, 5 });
            comboBoxFmMultiplier.SelectedIndex = 0; // Default value is 1
        }

        private void comboBoxFmMultiplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(comboBoxFmMultiplier.SelectedItem.ToString(), out int multiplier))
            {
                // Enqueue the command for the background thread to handle
                noteCommands.Enqueue(new NoteCommand
                {
                    Action = NoteAction.UpdateFmMultiplier,
                    Value = multiplier
                });
            }
        }
        // Inside KeyPlayerForm.cs

        private void ProcessNoteCommands(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // 1. Process pending commands from the UI thread
                while (noteCommands.TryDequeue(out NoteCommand command))
                {
                    switch (command.Action)
                    {
                        case NoteAction.NoteOn:
                            synthEngine.NoteOn(
                                command.KeyCode,
                                command.Frequency,
                                command.WaveType,
                                command.ModulationIndex,
                                command.FmMultiplier
                            );
                            break;
                        case NoteAction.NoteOff:
                            synthEngine.NoteOff(command.KeyCode);
                            break;
                        case NoteAction.UpdateNote:
                            synthEngine.UpdateNote(
                                command.KeyCode,
                                command.Frequency,
                                command.WaveType
                            );
                            break;
                        case NoteAction.UpdateFmMultiplier:
                            synthEngine.UpdateAllVoicesFmMultiplier(command.Value);
                            break;
                        case NoteAction.UpdateModulationIndex:
                            synthEngine.UpdateAllVoicesModulationIndex(command.Value);
                            break;
                    }
                }

                // 2. Perform periodic cleanup of voices in the release phase
                synthEngine.CleanupReleasedVoices();

                // 3. Briefly yield the thread to avoid 100% CPU usage
                Thread.Sleep(1);
            }
        }

        private void comboBoxWaveType_KeyPress(object sender, KeyPressEventArgs e)
        {
            // This line tells the ComboBox to ignore the key press
            e.Handled = true;
        }
        private void KeyPlayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle special function keys first.
            if (e.KeyCode == Keys.X)
            {
                CycleWaveType();
                return;
            }

            // The ProcessCmdKey override now handles key repeats. We just need to handle the first press.
            // Check if the key is already in the heldKeys set. If it is, this KeyDown event is a repeat
            // and we should do nothing.
            if (!heldKeys.Add(e.KeyCode))
            {
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

            // Process musical notes.
            if (noteFrequencies.TryGetValue(e.KeyCode, out float baseFrequency))
            {
                // If there's an active NoteOff timer for this key, stop and dispose it.
                if (noteOffTimers.TryGetValue(e.KeyCode, out var existingTimer))
                {
                    existingTimer.Stop();
                    existingTimer.Dispose();
                    noteOffTimers.Remove(e.KeyCode);
                }

                float frequency = AdjustFrequencyForOctave(baseFrequency);
                WaveType selectedWaveType = (WaveType)comboBoxWaveType.SelectedItem;

                // Get the current FM settings from the UI.
                double currentModulationIndex = modulationIndexTrackBar.Value / 10.0;
                double currentFmMultiplier = int.Parse(comboBoxFmMultiplier.SelectedItem.ToString());

                // Enqueue the NoteOn command with the current UI settings.
                noteCommands.Enqueue(new NoteCommand
                {
                    Action = NoteAction.NoteOn,
                    KeyCode = e.KeyCode,
                    Frequency = frequency,
                    WaveType = selectedWaveType,
                    ModulationIndex = currentModulationIndex,
                    FmMultiplier = currentFmMultiplier
                });

                // Update UI for the new key press
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
            // The ProcessCmdKey override handles removing the key from the heldKeys set.
            // We just need to check if it was a valid musical key.
            if (noteFrequencies.ContainsKey(e.KeyCode))
            {
                // Start a new timer for the released key
                var timer = new Timer();
                timer.Interval = 50; // Delay in milliseconds (e.g., 50ms for a quick tap)
                timer.Tag = e.KeyCode; // Store the key code for later use
                timer.Tick += NoteOffTimer_Tick; // Assign the event handler

                noteOffTimers[e.KeyCode] = timer; // Store the timer in the dictionary
                timer.Start();
            }
        }

        // Inside KeyPlayerForm.cs

        private void NoteOffTimer_Tick(object sender, EventArgs e)
        {
            var timer = (Timer)sender;
            timer.Stop(); // Stop the timer
            timer.Dispose(); // Clean up the timer object

            Keys key = (Keys)timer.Tag; // Get the key code from the timer's tag

            if (noteOffTimers.ContainsKey(key))
            {
                // Enqueue the NoteOff command
                noteCommands.Enqueue(new NoteCommand
                {
                    Action = NoteAction.NoteOff,
                    KeyCode = key
                });

                noteOffTimers.Remove(key); // Remove the timer from the dictionary
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

        private void modulationIndexTrackBar_Scroll(object sender, EventArgs e)
        {
            // Scale the integer value to a more useful double value (e.g., 0.0 to 10.0)
            double modulationIndex = modulationIndexTrackBar.Value / 10.0;
            labelModulationIndex.Text = $"Modulation Index: {modulationIndex:F1}";

            // Enqueue the command for the background thread
            noteCommands.Enqueue(new NoteCommand
            {
                Action = NoteAction.UpdateModulationIndex,
                Value = modulationIndex
            });
        }
    }
}