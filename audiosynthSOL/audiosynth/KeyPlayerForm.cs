using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace audiosynth
{
    public partial class KeyPlayerForm : Form
    {
        private readonly SynthEngine synthEngine;

        private int _currentOctave = 0;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int CurrentOctave
        {
            get => _currentOctave;
            set
            {
                if (value < -3) _currentOctave = -3;
                else if (value > 4) _currentOctave = 4;
                else _currentOctave = value;
            }
        }

        private readonly Dictionary<Keys, float> noteFrequencies = new Dictionary<Keys, float>
        {
            { Keys.C, 130.81f },
            { Keys.D, 146.83f },
            { Keys.E, 164.81f },
            { Keys.F, 174.61f },
            { Keys.G, 196.00f },
            { Keys.A, 220.00f },
            { Keys.B, 246.94f },
            { Keys.Q, 138.59f },
            { Keys.W, 155.56f },
            { Keys.V, 185.00f },
            { Keys.T, 207.65f },
            { Keys.U, 233.08f }
        };

        private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();
        private readonly ConcurrentQueue<(string keyName, long timestamp)> keyHistory = new ConcurrentQueue<(string, long)>();
        private const int maxHistory = 10;
        private readonly ConcurrentQueue<NoteCommand> noteCommands = new ConcurrentQueue<NoteCommand>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Dictionary<Keys, Timer> noteOffTimers = new Dictionary<Keys, Timer>();
        private readonly System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        // 16 Clickable Drum-Machine Style Step Buttons
        private readonly Button[] stepButtons = new Button[16];

        public KeyPlayerForm()
        {
            InitializeComponent();
            this.KeyPreview = true;

            synthEngine = new SynthEngine(this.waveformViewer, this.spectrumViewer);

            PopulateWaveTypeComboBox();
            PopulateSubWaveComboBox();
            BuildInteractiveStepButtons();
            WireSequencerStepEvent();
            UpdateOctaveLabel();
            UpdateSoundShapingButtonAppearance();
            UpdateDrumsButtonAppearance();
            UpdateChorusButtonAppearance();

            stopwatch.Start();

            Task.Run(() => ProcessNoteCommands(cts.Token));
        }

        private void PopulateWaveTypeComboBox()
        {
            comboBoxWaveType.Items.Clear();
            comboBoxWaveType.Items.Add(WaveType.Saw);
            comboBoxWaveType.Items.Add(WaveType.Square);
            comboBoxWaveType.Items.Add(WaveType.Sine);
            comboBoxWaveType.Items.Add(WaveType.Triangle);
            comboBoxWaveType.Items.Add(WaveType.FM);
            comboBoxWaveType.Items.Add(WaveType.Noise);
            comboBoxWaveType.Items.Add(WaveType.Pulse);
            comboBoxWaveType.SelectedItem = WaveType.Saw;
        }

        private void PopulateSubWaveComboBox()
        {
            string[] waveNames = { "Copy Saw (Primary)", "Square Wave", "Sine Wave" };

            cboSub1Wave.Items.Clear();
            cboSub1Wave.Items.AddRange(waveNames);
            cboSub1Wave.SelectedIndex = 0;

            cboSub2Wave.Items.Clear();
            cboSub2Wave.Items.AddRange(waveNames);
            cboSub2Wave.SelectedIndex = 0;
        }

        private void BuildInteractiveStepButtons()
        {
            int colWidth = 36;
            int rowHeight = 68;

            for (int i = 0; i < 16; i++)
            {
                int index = i;
                int row = i / 8;
                int col = i % 8;

                var btn = new Button
                {
                    Name = $"btnStep_{i}",
                    Size = new Size(colWidth - 2, rowHeight - 4),
                    Location = new Point(4 + (col * colWidth), 6 + (row * rowHeight)),
                    Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    TabStop = false
                };
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.FromArgb(70, 80, 95);

                btn.MouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        synthEngine.Sequencer.ToggleStepActive(index);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        synthEngine.Sequencer.CycleStepNote(index);
                    }
                    UpdateStepButtonAppearance(index, index == synthEngine.Sequencer.CurrentStepIndex);
                };

                stepButtons[i] = btn;
                pnlStepIndicators.Controls.Add(btn);
                UpdateStepButtonAppearance(i, false);
            }
        }

        private void UpdateAllStepButtons()
        {
            for (int i = 0; i < 16; i++)
            {
                UpdateStepButtonAppearance(i, i == synthEngine.Sequencer.CurrentStepIndex);
            }
        }

        private void UpdateStepButtonAppearance(int i, bool isPlayingCursor)
        {
            if (i < 0 || i >= 16) return;

            bool isActive = synthEngine.Sequencer.IsStepActive(i);
            int noteOffset = synthEngine.Sequencer.GetStepNoteOffset(i);

            Button btn = stepButtons[i];
            btn.Text = $"{i + 1}\r\n+{noteOffset}";

            if (isPlayingCursor)
            {
                btn.BackColor = Color.FromArgb(80, 255, 120); // Bright Neon Lime
                btn.ForeColor = Color.Black;
            }
            else if (isActive)
            {
                btn.BackColor = (i % 4 == 0) ? Color.FromArgb(245, 155, 45) : Color.FromArgb(215, 130, 35);
                btn.ForeColor = Color.White;
            }
            else
            {
                btn.BackColor = (i % 4 == 0) ? Color.FromArgb(50, 56, 65) : Color.FromArgb(36, 40, 48);
                btn.ForeColor = Color.Gray;
            }
        }

        private void WireSequencerStepEvent()
        {
            synthEngine.Sequencer.StepTriggered += stepIndex =>
            {
                if (IsHandleCreated && !IsDisposed)
                {
                    try
                    {
                        BeginInvoke(new Action(() =>
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                UpdateStepButtonAppearance(i, i == stepIndex);
                            }
                        }));
                    }
                    catch
                    {
                    }
                }
            };
        }

        private void ClearStepHighlights()
        {
            for (int i = 0; i < 16; i++)
            {
                UpdateStepButtonAppearance(i, false);
            }
        }

        private void KeyPlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cts.Cancel();
            synthEngine.StopAndDispose();
        }

        private void ProcessNoteCommands(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                while (noteCommands.TryDequeue(out NoteCommand? command))
                {
                    if (command == null) continue;

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
                    }
                }

                synthEngine.CleanupReleasedVoices();
                Thread.Sleep(1);
            }
        }

        private void KeyPlayerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.X)
            {
                CycleWaveType();
                return;
            }

            if (e.KeyCode == Keys.MediaPreviousTrack || e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
            {
                CurrentOctave--;
                UpdateOctaveLabel();
                return;
            }

            if (e.KeyCode == Keys.MediaNextTrack || e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add)
            {
                CurrentOctave++;
                UpdateOctaveLabel();
                return;
            }

            if (!heldKeys.Add(e.KeyCode))
            {
                return;
            }

            if (noteFrequencies.TryGetValue(e.KeyCode, out float baseFrequency))
            {
                if (noteOffTimers.TryGetValue(e.KeyCode, out var existingTimer))
                {
                    existingTimer.Stop();
                    existingTimer.Dispose();
                    noteOffTimers.Remove(e.KeyCode);
                }

                float frequency = AdjustFrequencyForOctave(baseFrequency);
                WaveType currentWaveType = (WaveType)(comboBoxWaveType.SelectedItem ?? WaveType.Saw);

                noteCommands.Enqueue(new NoteCommand
                {
                    Action = NoteAction.NoteOn,
                    KeyCode = e.KeyCode,
                    Frequency = frequency,
                    WaveType = currentWaveType
                });

                string keyName = e.KeyCode.ToString();
                long timestamp = stopwatch.ElapsedMilliseconds;

                if (keyHistory.Count >= maxHistory)
                {
                    keyHistory.TryDequeue(out _);
                }
                keyHistory.Enqueue((keyName, timestamp));
                UpdateKeyHistoryDisplay();
            }
        }

        private void CycleWaveType()
        {
            int nextIndex = (comboBoxWaveType.SelectedIndex + 1) % comboBoxWaveType.Items.Count;
            comboBoxWaveType.SelectedIndex = nextIndex;
        }

        private void KeyPlayerForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (heldKeys.Remove(e.KeyCode))
            {
                if (noteFrequencies.ContainsKey(e.KeyCode))
                {
                    var timer = new Timer();
                    timer.Interval = 50;
                    timer.Tag = e.KeyCode;
                    timer.Tick += NoteOffTimer_Tick;

                    noteOffTimers[e.KeyCode] = timer;
                    timer.Start();
                }
            }

            string keyName = e.KeyCode.ToString() + " (up)";
            long timestamp = stopwatch.ElapsedMilliseconds;

            if (keyHistory.Count >= maxHistory)
            {
                keyHistory.TryDequeue(out _);
            }
            keyHistory.Enqueue((keyName, timestamp));
            UpdateKeyHistoryDisplay();
        }

        private void NoteOffTimer_Tick(object? sender, EventArgs e)
        {
            if (sender is not Timer timer) return;

            timer.Stop();
            timer.Dispose();

            if (timer.Tag is Keys key && noteOffTimers.ContainsKey(key))
            {
                noteCommands.Enqueue(new NoteCommand
                {
                    Action = NoteAction.NoteOff,
                    KeyCode = key
                });

                noteOffTimers.Remove(key);
            }
        }

        private float AdjustFrequencyForOctave(float baseFreq)
        {
            return (float)(baseFreq * Math.Pow(2.0, CurrentOctave));
        }

        private void UpdateKeyHistoryDisplay()
        {
            var historyArray = keyHistory.ToArray().Reverse();
            var historyLines = historyArray.Select(item => $"{item.keyName} at {item.timestamp}ms");
            textBoxHistory.Text = string.Join(Environment.NewLine, historyLines);
        }

        private void UpdateOctaveLabel()
        {
            int displayOctave = 3 + CurrentOctave;
            labelOctave.Text = $"Octave: {displayOctave}";
        }

        private void btnOctaveDown_Click(object sender, EventArgs e)
        {
            CurrentOctave--;
            UpdateOctaveLabel();
        }

        private void btnOctaveUp_Click(object sender, EventArgs e)
        {
            CurrentOctave++;
            UpdateOctaveLabel();
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            synthEngine.Reset();
            heldKeys.Clear();
            textBoxHistory.Text = string.Empty;
            ClearStepHighlights();
            btnSeqPlayStop.Text = "▶ START SEQUENCER";
            btnSeqPlayStop.BackColor = Color.ForestGreen;
            WireSequencerStepEvent();
        }

        // =============================================================
        // WAVEFORM & SOUND SHAPING TOGGLES
        // =============================================================

        private void comboBoxWaveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxWaveType.SelectedItem is WaveType selectedType)
            {
                synthEngine.BassVoice.Waveform = selectedType;
            }
        }

        private void btnSoundShapingToggle_Click(object sender, EventArgs e)
        {
            bool newState = !synthEngine.BassVoice.SoundShapingEnabled;
            synthEngine.BassVoice.SoundShapingEnabled = newState;
            UpdateSoundShapingButtonAppearance();
        }

        private void UpdateSoundShapingButtonAppearance()
        {
            bool isShaping = synthEngine.BassVoice.SoundShapingEnabled;
            if (isShaping)
            {
                btnSoundShapingToggle.Text = "⚡ SOUND SHAPING: ON (Filter & Sub)";
                btnSoundShapingToggle.BackColor = Color.FromArgb(30, 140, 60); // Vibrant Green
                labelWaveTypeMode.Text = "Mode: Shaped Sound (Resonant LPF Active)";
                grpSynth.Text = "1. Sound Shaping (Subtractive Synth - Active)";
            }
            else
            {
                btnSoundShapingToggle.Text = "⚪ SOUND SHAPING: BYPASSED (Pure Wave)";
                btnSoundShapingToggle.BackColor = Color.FromArgb(70, 75, 85); // Dim Slate Gray
                labelWaveTypeMode.Text = "Mode: Pure Raw Wave (Filter & Sub Bypassed)";
                grpSynth.Text = "1. Sound Shaping (BYPASSED - Pure Wave Mode)";
            }
        }

        private void chkAntiAliasing_CheckedChanged(object sender, EventArgs e)
        {
            synthEngine.BassVoice.UseAntiAliasing = chkAntiAliasing.Checked;
        }

        // =============================================================
        // GROUP 1: ROTARY KNOB EVENT HANDLERS
        // =============================================================

        private void knobCutoff_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.BaseCutoff = knobCutoff.Value;
        }

        private void knobResonance_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.Resonance = knobResonance.Value;
        }

        private void knobEnvMod_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.FilterEnvAmount = knobEnvMod.Value;
        }

        private void knobFilterDecay_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.FilterDecay = knobFilterDecay.Value;
        }

        private void knobSub1Mix_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.Sub1Mix = knobSub1Mix.Value / 100.0f;
        }

        private void knobSub1Mix_BypassChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.Sub1Bypassed = knobSub1Mix.IsBypassed;
        }

        private void knobSub2Mix_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.Sub2Mix = knobSub2Mix.Value / 100.0f;
        }

        private void knobSub2Mix_BypassChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.Sub2Bypassed = knobSub2Mix.IsBypassed;
        }

        private void knobAmpRelease_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.AmpRelease = knobAmpRelease.Value;
        }

        // =============================================================
        // GROUP 2: SEQUENCER EVENT HANDLERS
        // =============================================================

        private void btnSeqPlayStop_Click(object sender, EventArgs e)
        {
            if (synthEngine.Sequencer.IsRunning)
            {
                synthEngine.Sequencer.Stop();
                btnSeqPlayStop.Text = "▶ START SEQUENCER";
                btnSeqPlayStop.BackColor = Color.ForestGreen;
                ClearStepHighlights();
            }
            else
            {
                synthEngine.Sequencer.Start();
                btnSeqPlayStop.Text = "⏹ STOP SEQUENCER";
                btnSeqPlayStop.BackColor = Color.Crimson;
            }
        }

        private void trackBarBpm_Scroll(object sender, EventArgs e)
        {
            int bpm = trackBarBpm.Value;
            lblBpm.Text = $"Tempo: {bpm} BPM";
            synthEngine.Sequencer.BPM = bpm;
            synthEngine.Sequencer.RecalculateStepLength();
        }

        private void rbPattern_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPatternA.Checked)
            {
                synthEngine.Sequencer.CurrentPatternChoice = SequencerPatternChoice.PatternA;
            }
            else if (rbPatternB.Checked)
            {
                synthEngine.Sequencer.CurrentPatternChoice = SequencerPatternChoice.PatternB;
            }
            UpdateAllStepButtons();
        }

        private void btnTransposeRoot_Click(object sender, EventArgs e)
        {
            synthEngine.Sequencer.TranspositionSemitones = 0;
            btnTransposeRoot.BackColor = Color.LightSkyBlue;
            btnTransposeFourth.BackColor = Color.WhiteSmoke;
            btnTransposeFifth.BackColor = Color.WhiteSmoke;
        }

        private void btnTransposeFourth_Click(object sender, EventArgs e)
        {
            synthEngine.Sequencer.TranspositionSemitones = 5; // 4th
            btnTransposeRoot.BackColor = Color.WhiteSmoke;
            btnTransposeFourth.BackColor = Color.LightSkyBlue;
            btnTransposeFifth.BackColor = Color.WhiteSmoke;
        }

        private void btnTransposeFifth_Click(object sender, EventArgs e)
        {
            synthEngine.Sequencer.TranspositionSemitones = 7; // 5th
            btnTransposeRoot.BackColor = Color.WhiteSmoke;
            btnTransposeFourth.BackColor = Color.WhiteSmoke;
            btnTransposeFifth.BackColor = Color.LightSkyBlue;
        }

        // =============================================================
        // GROUP 3: BACKING DRUMS & FX EVENT HANDLERS (KNOBS & TOGGLES)
        // =============================================================

        private void btnDrumsToggle_Click(object sender, EventArgs e)
        {
            synthEngine.Drums.IsEnabled = !synthEngine.Drums.IsEnabled;
            UpdateDrumsButtonAppearance();
        }

        private void UpdateDrumsButtonAppearance()
        {
            if (synthEngine.Drums.IsEnabled)
            {
                btnDrumsToggle.Text = "🥁 DRUM MACHINE: ON (Synced)";
                btnDrumsToggle.BackColor = Color.FromArgb(40, 120, 180);
            }
            else
            {
                btnDrumsToggle.Text = "⚪ DRUM MACHINE: MUTED";
                btnDrumsToggle.BackColor = Color.FromArgb(70, 75, 85);
            }
        }

        private void btnChorusToggle_Click(object sender, EventArgs e)
        {
            synthEngine.Chorus.IsEnabled = !synthEngine.Chorus.IsEnabled;
            UpdateChorusButtonAppearance();
        }

        private void UpdateChorusButtonAppearance()
        {
            if (synthEngine.Chorus.IsEnabled)
            {
                btnChorusToggle.Text = "✨ STEREO CHORUS: ON (Width)";
                btnChorusToggle.BackColor = Color.FromArgb(100, 60, 160);
            }
            else
            {
                btnChorusToggle.Text = "⚪ STEREO CHORUS: BYPASSED";
                btnChorusToggle.BackColor = Color.FromArgb(70, 75, 85);
            }
        }

        private void knobKickVol_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.Drums.KickVolume = knobKickVol.Value / 100.0f;
        }

        private void knobSnareHatVol_ValueChanged(object? sender, EventArgs e)
        {
            float vol = knobSnareHatVol.Value / 100.0f;
            synthEngine.Drums.SnareVolume = vol;
            synthEngine.Drums.HatVolume = vol * 0.8f;
        }

        private void knobChorusDepth_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.Chorus.WetMix = knobChorusDepth.Value / 100.0f;
        }

        private void knobMasterVol_ValueChanged(object? sender, EventArgs e)
        {
            synthEngine.Groovebox.MasterVolume = knobMasterVol.Value / 100.0f;
        }

        // =============================================================
        // KNOB BYPASS HANDLERS (Right-Click on Knob)
        // =============================================================

        private void knobCutoff_BypassChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.CutoffBypassed = knobCutoff.IsBypassed;
            UpdateSoundShapingButtonAppearance();
        }

        private void knobResonance_BypassChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.ResonanceBypassed = knobResonance.IsBypassed;
        }

        private void knobEnvMod_BypassChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.EnvModBypassed = knobEnvMod.IsBypassed;
        }

        // =============================================================
        // SUB WAVEFORM DROPDOWNS (new cboSub1Wave / cboSub2Wave)
        // =============================================================

        private void cboSub1Wave_SelectedIndexChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.Sub1Waveform = cboSub1Wave.SelectedIndex switch
            {
                1 => SubOscWaveType.Square,
                2 => SubOscWaveType.Sine,
                _ => SubOscWaveType.CopyPrimary
            };
        }

        private void cboSub2Wave_SelectedIndexChanged(object? sender, EventArgs e)
        {
            synthEngine.BassVoice.Sub2Waveform = cboSub2Wave.SelectedIndex switch
            {
                1 => SubOscWaveType.Square,
                2 => SubOscWaveType.Sine,
                _ => SubOscWaveType.CopyPrimary
            };
        }

        // =============================================================
        // ARPEGGIATOR / CUSTOM PITCH SEQUENCE
        // =============================================================

        private void btnApplyArp_Click(object? sender, EventArgs e)
        {
            string csv = txtArpOffsets.Text.Trim();
            if (string.IsNullOrEmpty(csv)) return;

            synthEngine.Sequencer.ApplyCustomPitchSequence(csv);
            UpdateAllStepButtons();
        }

        // =============================================================
        // KNOB DESCRIPTION HOVER BAR
        // =============================================================

        private void OnKnobHovered(object? sender, string description)
        {
            if (InvokeRequired)
                BeginInvoke(new Action(() => lblKnobDescription.Text = description));
            else
                lblKnobDescription.Text = description;
        }

        // Legacy compatibility stubs
        private void comboBoxWaveType_KeyPress(object sender, KeyPressEventArgs e) => e.Handled = true;
        private void comboBoxFmMultiplier_SelectedIndexChanged(object sender, EventArgs e) { }
        private void ModulatorFrequencyTrackBar_Scroll(object sender, EventArgs e) { }
        private void modulationIndexTrackBar_Scroll(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void chkSoundShaping_CheckedChanged(object sender, EventArgs e) { }
        private void chkSubSquare_CheckedChanged(object sender, EventArgs e) { }
        private void chkDrumsEnabled_CheckedChanged(object sender, EventArgs e) { }
        private void chkChorusEnabled_CheckedChanged(object sender, EventArgs e) { }
        private void trackBarCutoff_Scroll(object sender, EventArgs e) { }
        private void trackBarResonance_Scroll(object sender, EventArgs e) { }
        private void trackBarEnvMod_Scroll(object sender, EventArgs e) { }
        private void trackBarFilterDecay_Scroll(object sender, EventArgs e) { }
        private void trackBarSubMix_Scroll(object sender, EventArgs e) { }
        private void trackBarAmpRelease_Scroll(object sender, EventArgs e) { }
        private void trackBarKickVol_Scroll(object sender, EventArgs e) { }
        private void trackBarSnareHatVol_Scroll(object sender, EventArgs e) { }
        private void trackBarChorusDepth_Scroll(object sender, EventArgs e) { }
        private void trackBarMasterVol_Scroll(object sender, EventArgs e) { }
    }
}
