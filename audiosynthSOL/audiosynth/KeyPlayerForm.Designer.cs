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
            waveformViewer = new WaveformViewer();
            spectrumViewer = new SpectrumViewer();
            labelAppTitle = new Label();
            labelWaveform = new Label();
            comboBoxWaveType = new ComboBox();
            chkAntiAliasing = new CheckBox();
            labelOctave = new Label();
            btnOctaveDown = new Button();
            btnOctaveUp = new Button();
            Reset = new Button();
            labelWaveTypeMode = new Label();
            
            // Group 1: Synth Sound Shaping (Row 1: Cutoff, Res, EnvDepth, EnvDecay; Row 2: Sub1Mix+Pitch, Sub2Mix+Pitch, Release)
            grpSynth = new GroupBox();
            knobCutoff = new KnobControl();
            knobResonance = new KnobControl();
            knobEnvMod = new KnobControl();
            knobFilterDecay = new KnobControl();
            knobSub1Mix = new KnobControl();
            knobSub1Pitch = new KnobControl();
            knobSub2Mix = new KnobControl();
            knobSub2Pitch = new KnobControl();
            knobAmpRelease = new KnobControl();
            lblSub1Wave = new Label();
            cboSub1Wave = new ComboBox();
            lblSub2Wave = new Label();
            cboSub2Wave = new ComboBox();

            // Group 2: Step Sequencer & Arpeggiator
            grpSequencer = new GroupBox();
            btnSeqPlayStop = new Button();
            lblBpm = new Label();
            trackBarBpm = new TrackBar();
            rbPatternA = new RadioButton();
            rbPatternB = new RadioButton();
            lblTranspose = new Label();
            btnTransposeRoot = new Button();
            btnTransposeFourth = new Button();
            btnTransposeFifth = new Button();
            lblArpTitle = new Label();
            txtArpOffsets = new TextBox();
            btnApplyArp = new Button();
            lblStepsHeader = new Label();
            pnlStepIndicators = new Panel();

            // Group 3: Backing Drums & Master FX
            grpBackingFx = new GroupBox();
            btnDrumsToggle = new Button();
            btnChorusToggle = new Button();
            knobKickVol = new KnobControl();
            knobSnareHatVol = new KnobControl();
            knobChorusDepth = new KnobControl();
            knobMasterVol = new KnobControl();

            // Footer
            textBoxHistory = new TextBox();
            lblFooterInfo = new Label();

            // Legacy hidden bindings for backward compatibility
            btnSoundShapingToggle = new Button();
            chkSoundShaping = new CheckBox();
            chkSubSquare = new CheckBox();
            ModulatorFrequencyTrackBar = new TrackBar();
            modulationIndexTrackBar = new TrackBar();
            comboBoxFmMultiplier = new ComboBox();
            labelModulationIndex = new Label();
            labelFmMultiplier = new Label();
            label1 = new Label();
            FM = new Label();
            Instrument = new Label();
            trackBarCutoff = new TrackBar();
            trackBarResonance = new TrackBar();
            trackBarEnvMod = new TrackBar();
            trackBarFilterDecay = new TrackBar();
            trackBarSubMix = new TrackBar();
            trackBarAmpRelease = new TrackBar();
            trackBarKickVol = new TrackBar();
            trackBarSnareHatVol = new TrackBar();
            trackBarChorusDepth = new TrackBar();
            trackBarMasterVol = new TrackBar();
            chkDrumsEnabled = new CheckBox();
            chkChorusEnabled = new CheckBox();

            ((System.ComponentModel.ISupportInitialize)trackBarBpm).BeginInit();
            grpSynth.SuspendLayout();
            grpSequencer.SuspendLayout();
            grpBackingFx.SuspendLayout();
            SuspendLayout();

            // 
            // waveformViewer (Left Half - Royal Blue Oscilloscope, ~10 waves)
            // 
            waveformViewer.Location = new Point(14, 12);
            waveformViewer.Name = "waveformViewer";
            waveformViewer.Size = new Size(325, 115);
            waveformViewer.TabIndex = 0;

            // 
            // spectrumViewer (Right Half - Continuous Copper/Amber Harmonic Spectrum)
            // 
            spectrumViewer.Location = new Point(348, 12);
            spectrumViewer.Name = "spectrumViewer";
            spectrumViewer.Size = new Size(325, 115);
            spectrumViewer.TabIndex = 1;

            // 
            // labelAppTitle
            // 
            labelAppTitle.AutoSize = true;
            labelAppTitle.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
            labelAppTitle.ForeColor = Color.SteelBlue;
            labelAppTitle.Location = new Point(685, 8);
            labelAppTitle.Name = "labelAppTitle";
            labelAppTitle.Size = new Size(185, 23);
            labelAppTitle.TabIndex = 2;
            labelAppTitle.Text = "Acid Bass Groovebox";

            // 
            // labelWaveform
            // 
            labelWaveform.AutoSize = true;
            labelWaveform.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular);
            labelWaveform.Location = new Point(685, 33);
            labelWaveform.Name = "labelWaveform";
            labelWaveform.Size = new Size(144, 13);
            labelWaveform.TabIndex = 3;
            labelWaveform.Text = "Waveform (Press X to cycle):";

            // 
            // comboBoxWaveType
            // 
            comboBoxWaveType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxWaveType.Font = new Font("Segoe UI", 8.5F);
            comboBoxWaveType.FormattingEnabled = true;
            comboBoxWaveType.Location = new Point(685, 50);
            comboBoxWaveType.Name = "comboBoxWaveType";
            comboBoxWaveType.Size = new Size(160, 21);
            comboBoxWaveType.TabIndex = 4;
            comboBoxWaveType.TabStop = false;
            comboBoxWaveType.SelectedIndexChanged += comboBoxWaveType_SelectedIndexChanged;
            comboBoxWaveType.KeyPress += comboBoxWaveType_KeyPress;

            // 
            // chkAntiAliasing
            // 
            chkAntiAliasing.AutoSize = true;
            chkAntiAliasing.Font = new Font("Segoe UI", 8.25F);
            chkAntiAliasing.Location = new Point(685, 78);
            chkAntiAliasing.Name = "chkAntiAliasing";
            chkAntiAliasing.Size = new Size(157, 17);
            chkAntiAliasing.TabIndex = 5;
            chkAntiAliasing.TabStop = false;
            chkAntiAliasing.Text = "Anti-Aliased (PolyBLEP)";
            chkAntiAliasing.UseVisualStyleBackColor = true;
            chkAntiAliasing.CheckedChanged += chkAntiAliasing_CheckedChanged;

            // 
            // labelOctave
            // 
            labelOctave.AutoSize = true;
            labelOctave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            labelOctave.Location = new Point(860, 31);
            labelOctave.Name = "labelOctave";
            labelOctave.Size = new Size(66, 17);
            labelOctave.TabIndex = 6;
            labelOctave.Text = "Octave: 4";

            // 
            // btnOctaveDown
            // 
            btnOctaveDown.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnOctaveDown.Location = new Point(860, 49);
            btnOctaveDown.Name = "btnOctaveDown";
            btnOctaveDown.Size = new Size(30, 24);
            btnOctaveDown.TabIndex = 7;
            btnOctaveDown.TabStop = false;
            btnOctaveDown.Text = "-";
            btnOctaveDown.UseVisualStyleBackColor = true;
            btnOctaveDown.Click += btnOctaveDown_Click;

            // 
            // btnOctaveUp
            // 
            btnOctaveUp.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnOctaveUp.Location = new Point(894, 49);
            btnOctaveUp.Name = "btnOctaveUp";
            btnOctaveUp.Size = new Size(30, 24);
            btnOctaveUp.TabIndex = 8;
            btnOctaveUp.TabStop = false;
            btnOctaveUp.Text = "+";
            btnOctaveUp.UseVisualStyleBackColor = true;
            btnOctaveUp.Click += btnOctaveUp_Click;

            // 
            // Reset
            // 
            Reset.BackColor = Color.IndianRed;
            Reset.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            Reset.ForeColor = Color.White;
            Reset.Location = new Point(934, 49);
            Reset.Name = "Reset";
            Reset.Size = new Size(86, 24);
            Reset.TabIndex = 9;
            Reset.TabStop = false;
            Reset.Text = "Reset Audio";
            Reset.UseVisualStyleBackColor = false;
            Reset.Click += Reset_Click;

            // 
            // labelWaveTypeMode
            // 
            labelWaveTypeMode.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular);
            labelWaveTypeMode.ForeColor = Color.DimGray;
            labelWaveTypeMode.Location = new Point(860, 78);
            labelWaveTypeMode.Name = "labelWaveTypeMode";
            labelWaveTypeMode.Size = new Size(160, 39);
            labelWaveTypeMode.TabIndex = 10;
            labelWaveTypeMode.Text = "Mode: Pure Clean Saw Wave";

            // -------------------------------------------------------------
            // GROUP 1: SYNTH SOUND SHAPING (FIRST IN ROW: CUTOFF, END 2 ENV)
            // -------------------------------------------------------------
            grpSynth.Controls.Add(knobCutoff);
            grpSynth.Controls.Add(knobResonance);
            grpSynth.Controls.Add(knobEnvMod);
            grpSynth.Controls.Add(knobFilterDecay);
            grpSynth.Controls.Add(knobSub1Mix);
            grpSynth.Controls.Add(knobSub1Pitch);
            grpSynth.Controls.Add(knobSub2Mix);
            grpSynth.Controls.Add(knobSub2Pitch);
            grpSynth.Controls.Add(knobAmpRelease);
            grpSynth.Controls.Add(lblSub1Wave);
            grpSynth.Controls.Add(cboSub1Wave);
            grpSynth.Controls.Add(lblSub2Wave);
            grpSynth.Controls.Add(cboSub2Wave);
            grpSynth.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grpSynth.Location = new Point(14, 138);
            grpSynth.Name = "grpSynth";
            grpSynth.Size = new Size(325, 290);
            grpSynth.TabIndex = 11;
            grpSynth.TabStop = false;
            grpSynth.Text = "1. Sound Shaping (Right-Click Knob to Bypass)";

            // Row 1: Cutoff, Resonance, and the 2 Envelopes (Env Depth, Env Decay)
            knobCutoff.Location = new Point(4, 24);
            knobCutoff.Size = new Size(76, 88);
            knobCutoff.Minimum = 40f;
            knobCutoff.Maximum = 5000f;
            knobCutoff.Value = 1200f;
            knobCutoff.DefaultValue = 1200f;
            knobCutoff.Title = "CUTOFF";
            knobCutoff.ValueFormat = "{0:F0} Hz";
            knobCutoff.ArcColor = Color.FromArgb(245, 145, 45);
            knobCutoff.Description = "Low-Pass Filter Cutoff: Removes high frequencies. Right-click to bypass filter.";
            knobCutoff.ValueChanged += knobCutoff_ValueChanged;
            knobCutoff.BypassChanged += knobCutoff_BypassChanged;
            knobCutoff.DescriptionHovered += OnKnobHovered;

            knobResonance.Location = new Point(84, 24);
            knobResonance.Size = new Size(76, 88);
            knobResonance.Minimum = 0.5f;
            knobResonance.Maximum = 14f;
            knobResonance.Value = 1.0f; // Clean flat resonance on start
            knobResonance.DefaultValue = 1.0f;
            knobResonance.Title = "RESONANCE";
            knobResonance.ValueFormat = "{0:F1} Q";
            knobResonance.ArcColor = Color.FromArgb(245, 145, 45);
            knobResonance.Description = "Resonance (Q): Boosts frequency peak at cutoff. Right-click to flatten.";
            knobResonance.ValueChanged += knobResonance_ValueChanged;
            knobResonance.BypassChanged += knobResonance_BypassChanged;
            knobResonance.DescriptionHovered += OnKnobHovered;

            knobEnvMod.Location = new Point(164, 24);
            knobEnvMod.Size = new Size(76, 88);
            knobEnvMod.Minimum = 0f;
            knobEnvMod.Maximum = 6.5f;
            knobEnvMod.Value = 0.0f; // 0 by default for pure clean saw
            knobEnvMod.DefaultValue = 0.0f;
            knobEnvMod.Title = "ENV DEPTH";
            knobEnvMod.ValueFormat = "{0:F1} oct";
            knobEnvMod.ArcColor = Color.FromArgb(220, 130, 40);
            knobEnvMod.Description = "Filter Envelope Depth: How far filter opens on key press. Right-click to disable.";
            knobEnvMod.ValueChanged += knobEnvMod_ValueChanged;
            knobEnvMod.BypassChanged += knobEnvMod_BypassChanged;
            knobEnvMod.DescriptionHovered += OnKnobHovered;

            knobFilterDecay.Location = new Point(244, 24);
            knobFilterDecay.Size = new Size(76, 88);
            knobFilterDecay.Minimum = 0.02f;
            knobFilterDecay.Maximum = 1.20f;
            knobFilterDecay.Value = 0.22f;
            knobFilterDecay.DefaultValue = 0.22f;
            knobFilterDecay.Title = "ENV DECAY";
            knobFilterDecay.ValueFormat = "{0:F2} s";
            knobFilterDecay.ArcColor = Color.FromArgb(220, 130, 40);
            knobFilterDecay.Description = "Filter Envelope Decay: How fast filter closes down after key strike.";
            knobFilterDecay.ValueChanged += knobFilterDecay_ValueChanged;
            knobFilterDecay.DescriptionHovered += OnKnobHovered;

            // Row 2: 2 Sub-Mix Knobs + Amp Release
            knobSub1Mix.Location = new Point(18, 122);
            knobSub1Mix.Size = new Size(68, 80);
            knobSub1Mix.Minimum = 0f;
            knobSub1Mix.Maximum = 100f;
            knobSub1Mix.Value = 0f; // 0% clean start
            knobSub1Mix.DefaultValue = 0f;
            knobSub1Mix.Title = "SUB1 MIX";
            knobSub1Mix.ValueFormat = "{0:F0}%";
            knobSub1Mix.ArcColor = Color.DodgerBlue;
            knobSub1Mix.Description = "Sub-Oscillator 1 Mix. Right-click to mute.";
            knobSub1Mix.ValueChanged += knobSub1Mix_ValueChanged;
            knobSub1Mix.BypassChanged += knobSub1Mix_BypassChanged;
            knobSub1Mix.DescriptionHovered += OnKnobHovered;

            knobSub1Pitch.Location = new Point(90, 122);
            knobSub1Pitch.Size = new Size(68, 80);
            knobSub1Pitch.Minimum = -24f;
            knobSub1Pitch.Maximum = 24f;
            knobSub1Pitch.Value = -12f; // Default: -1 octave
            knobSub1Pitch.DefaultValue = -12f;
            knobSub1Pitch.Title = "SUB1 PITCH";
            knobSub1Pitch.ValueFormat = "{0:+0;-0;0} st";
            knobSub1Pitch.ArcColor = Color.CornflowerBlue;
            knobSub1Pitch.Description = "Sub-Oscillator 1 Pitch: -24 to +24 semitones (-12 = one octave below).";
            knobSub1Pitch.ValueChanged += knobSub1Pitch_ValueChanged;
            knobSub1Pitch.DescriptionHovered += OnKnobHovered;

            knobSub2Mix.Location = new Point(170, 122);
            knobSub2Mix.Size = new Size(68, 80);
            knobSub2Mix.Minimum = 0f;
            knobSub2Mix.Maximum = 100f;
            knobSub2Mix.Value = 0f; // 0% clean start
            knobSub2Mix.DefaultValue = 0f;
            knobSub2Mix.Title = "SUB2 MIX";
            knobSub2Mix.ValueFormat = "{0:F0}%";
            knobSub2Mix.ArcColor = Color.DodgerBlue;
            knobSub2Mix.Description = "Sub-Oscillator 2 Mix. Right-click to mute.";
            knobSub2Mix.ValueChanged += knobSub2Mix_ValueChanged;
            knobSub2Mix.BypassChanged += knobSub2Mix_BypassChanged;
            knobSub2Mix.DescriptionHovered += OnKnobHovered;

            knobSub2Pitch.Location = new Point(242, 122);
            knobSub2Pitch.Size = new Size(68, 80);
            knobSub2Pitch.Minimum = -24f;
            knobSub2Pitch.Maximum = 24f;
            knobSub2Pitch.Value = -24f; // Default: -2 octaves
            knobSub2Pitch.DefaultValue = -24f;
            knobSub2Pitch.Title = "SUB2 PITCH";
            knobSub2Pitch.ValueFormat = "{0:+0;-0;0} st";
            knobSub2Pitch.ArcColor = Color.CornflowerBlue;
            knobSub2Pitch.Description = "Sub-Oscillator 2 Pitch: -24 to +24 semitones (-24 = two octaves below).";
            knobSub2Pitch.ValueChanged += knobSub2Pitch_ValueChanged;
            knobSub2Pitch.DescriptionHovered += OnKnobHovered;

            knobAmpRelease.Location = new Point(130, 210);
            knobAmpRelease.Size = new Size(68, 80);
            knobAmpRelease.Minimum = 0.02f;
            knobAmpRelease.Maximum = 0.80f;
            knobAmpRelease.Value = 0.12f;
            knobAmpRelease.DefaultValue = 0.12f;
            knobAmpRelease.Title = "AMP REL";
            knobAmpRelease.ValueFormat = "{0:F2} s";
            knobAmpRelease.ArcColor = Color.SpringGreen;
            knobAmpRelease.Description = "Amplitude Release: Fade-out time after releasing key.";
            knobAmpRelease.ValueChanged += knobAmpRelease_ValueChanged;
            knobAmpRelease.DescriptionHovered += OnKnobHovered;

            // Sub Waveform Selectors (row 3, below pitch knobs)
            lblSub1Wave.AutoSize = true;
            lblSub1Wave.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblSub1Wave.ForeColor = Color.DarkSlateBlue;
            lblSub1Wave.Location = new Point(14, 212);
            lblSub1Wave.Name = "lblSub1Wave";
            lblSub1Wave.Size = new Size(95, 12);
            lblSub1Wave.Text = "Sub 1 Wave:";

            cboSub1Wave.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSub1Wave.Font = new Font("Segoe UI", 8F);
            cboSub1Wave.FormattingEnabled = true;
            cboSub1Wave.Location = new Point(14, 226);
            cboSub1Wave.Name = "cboSub1Wave";
            cboSub1Wave.Size = new Size(108, 21);
            cboSub1Wave.TabIndex = 8;
            cboSub1Wave.TabStop = false;
            cboSub1Wave.SelectedIndexChanged += cboSub1Wave_SelectedIndexChanged;

            lblSub2Wave.AutoSize = true;
            lblSub2Wave.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblSub2Wave.ForeColor = Color.DarkSlateBlue;
            lblSub2Wave.Location = new Point(216, 212);
            lblSub2Wave.Name = "lblSub2Wave";
            lblSub2Wave.Size = new Size(95, 12);
            lblSub2Wave.Text = "Sub 2 Wave:";

            cboSub2Wave.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSub2Wave.Font = new Font("Segoe UI", 8F);
            cboSub2Wave.FormattingEnabled = true;
            cboSub2Wave.Location = new Point(216, 226);
            cboSub2Wave.Name = "cboSub2Wave";
            cboSub2Wave.Size = new Size(100, 21);
            cboSub2Wave.TabIndex = 9;
            cboSub2Wave.TabStop = false;
            cboSub2Wave.SelectedIndexChanged += cboSub2Wave_SelectedIndexChanged;

            // -------------------------------------------------------------
            // GROUP 2: STEP SEQUENCER & ARPEGGIATOR
            // -------------------------------------------------------------
            grpSequencer.Controls.Add(btnSeqPlayStop);
            grpSequencer.Controls.Add(lblBpm);
            grpSequencer.Controls.Add(trackBarBpm);
            grpSequencer.Controls.Add(rbPatternA);
            grpSequencer.Controls.Add(rbPatternB);
            grpSequencer.Controls.Add(lblTranspose);
            grpSequencer.Controls.Add(btnTransposeRoot);
            grpSequencer.Controls.Add(btnTransposeFourth);
            grpSequencer.Controls.Add(btnTransposeFifth);
            grpSequencer.Controls.Add(lblArpTitle);
            grpSequencer.Controls.Add(txtArpOffsets);
            grpSequencer.Controls.Add(btnApplyArp);
            grpSequencer.Controls.Add(lblStepsHeader);
            grpSequencer.Controls.Add(pnlStepIndicators);
            grpSequencer.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grpSequencer.Location = new Point(348, 138);
            grpSequencer.Name = "grpSequencer";
            grpSequencer.Size = new Size(335, 400);
            grpSequencer.TabIndex = 12;
            grpSequencer.TabStop = false;
            grpSequencer.Text = "2. 16-Step Sequencer & Arp";

            // Play/Stop Button
            btnSeqPlayStop.BackColor = Color.ForestGreen;
            btnSeqPlayStop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSeqPlayStop.ForeColor = Color.White;
            btnSeqPlayStop.Location = new Point(14, 22);
            btnSeqPlayStop.Name = "btnSeqPlayStop";
            btnSeqPlayStop.Size = new Size(307, 34);
            btnSeqPlayStop.TabIndex = 0;
            btnSeqPlayStop.TabStop = false;
            btnSeqPlayStop.Text = "▶ START SEQUENCER";
            btnSeqPlayStop.UseVisualStyleBackColor = false;
            btnSeqPlayStop.Click += btnSeqPlayStop_Click;

            // BPM Slider
            lblBpm.AutoSize = true;
            lblBpm.Font = new Font("Segoe UI", 8.5F);
            lblBpm.Location = new Point(14, 60);
            lblBpm.Name = "lblBpm";
            lblBpm.Size = new Size(115, 15);
            lblBpm.Text = "Tempo: 125 BPM";

            trackBarBpm.Location = new Point(14, 76);
            trackBarBpm.Maximum = 175;
            trackBarBpm.Minimum = 70;
            trackBarBpm.Name = "trackBarBpm";
            trackBarBpm.Size = new Size(307, 30);
            trackBarBpm.TabIndex = 1;
            trackBarBpm.TabStop = false;
            trackBarBpm.TickFrequency = 10;
            trackBarBpm.Value = 125;
            trackBarBpm.Scroll += trackBarBpm_Scroll;

            // Pattern Selection
            rbPatternA.AutoSize = true;
            rbPatternA.Checked = true;
            rbPatternA.Font = new Font("Segoe UI", 8.5F);
            rbPatternA.Location = new Point(18, 110);
            rbPatternA.Name = "rbPatternA";
            rbPatternA.Size = new Size(155, 19);
            rbPatternA.TabIndex = 2;
            rbPatternA.TabStop = false;
            rbPatternA.Text = "Pattern A (Acid Drive)";
            rbPatternA.UseVisualStyleBackColor = true;
            rbPatternA.CheckedChanged += rbPattern_CheckedChanged;

            rbPatternB.AutoSize = true;
            rbPatternB.Font = new Font("Segoe UI", 8.5F);
            rbPatternB.Location = new Point(180, 110);
            rbPatternB.Name = "rbPatternB";
            rbPatternB.Size = new Size(140, 19);
            rbPatternB.TabIndex = 3;
            rbPatternB.TabStop = false;
            rbPatternB.Text = "Pattern B (Bounce)";
            rbPatternB.UseVisualStyleBackColor = true;
            rbPatternB.CheckedChanged += rbPattern_CheckedChanged;

            // Transposition Section (Root, 4th, 5th)
            lblTranspose.AutoSize = true;
            lblTranspose.Font = new Font("Segoe UI", 8F);
            lblTranspose.Location = new Point(14, 134);
            lblTranspose.Name = "lblTranspose";
            lblTranspose.Size = new Size(205, 13);
            lblTranspose.TabIndex = 4;
            lblTranspose.Text = "Real-Time Transposition (Root, 4th, 5th):";

            btnTransposeRoot.BackColor = Color.LightSkyBlue;
            btnTransposeRoot.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnTransposeRoot.Location = new Point(14, 150);
            btnTransposeRoot.Name = "btnTransposeRoot";
            btnTransposeRoot.Size = new Size(95, 28);
            btnTransposeRoot.TabIndex = 5;
            btnTransposeRoot.TabStop = false;
            btnTransposeRoot.Text = "Root (0st)";
            btnTransposeRoot.UseVisualStyleBackColor = false;
            btnTransposeRoot.Click += btnTransposeRoot_Click;

            btnTransposeFourth.BackColor = Color.WhiteSmoke;
            btnTransposeFourth.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnTransposeFourth.Location = new Point(120, 150);
            btnTransposeFourth.Name = "btnTransposeFourth";
            btnTransposeFourth.Size = new Size(95, 28);
            btnTransposeFourth.TabIndex = 6;
            btnTransposeFourth.TabStop = false;
            btnTransposeFourth.Text = "4th (+5st)";
            btnTransposeFourth.UseVisualStyleBackColor = false;
            btnTransposeFourth.Click += btnTransposeFourth_Click;

            btnTransposeFifth.BackColor = Color.WhiteSmoke;
            btnTransposeFifth.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnTransposeFifth.Location = new Point(226, 150);
            btnTransposeFifth.Name = "btnTransposeFifth";
            btnTransposeFifth.Size = new Size(95, 28);
            btnTransposeFifth.TabIndex = 7;
            btnTransposeFifth.TabStop = false;
            btnTransposeFifth.Text = "5th (+7st)";
            btnTransposeFifth.UseVisualStyleBackColor = false;
            btnTransposeFifth.Click += btnTransposeFifth_Click;

            // Arpeggiator / Custom Pitch Sequence Box
            lblArpTitle.AutoSize = true;
            lblArpTitle.Font = new Font("Segoe UI", 8F);
            lblArpTitle.Location = new Point(14, 184);
            lblArpTitle.Name = "lblArpTitle";
            lblArpTitle.Size = new Size(245, 13);
            lblArpTitle.TabIndex = 8;
            lblArpTitle.Text = "Custom Arp Range / Offsets (e.g. 0, 12, 2 or 12, 2):";

            txtArpOffsets.Font = new Font("Consolas", 8.5F);
            txtArpOffsets.Location = new Point(14, 200);
            txtArpOffsets.Name = "txtArpOffsets";
            txtArpOffsets.Size = new Size(160, 21);
            txtArpOffsets.TabIndex = 9;
            txtArpOffsets.Text = "0, 12, 2";

            btnApplyArp.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnApplyArp.Location = new Point(180, 199);
            btnApplyArp.Name = "btnApplyArp";
            btnApplyArp.Size = new Size(141, 23);
            btnApplyArp.TabIndex = 10;
            btnApplyArp.TabStop = false;
            btnApplyArp.Text = "Arp / Copy to Steps";
            btnApplyArp.UseVisualStyleBackColor = true;
            btnApplyArp.Click += btnApplyArp_Click;

            // 16-Step Clickable Drum Machine Buttons
            lblStepsHeader.AutoSize = true;
            lblStepsHeader.Font = new Font("Segoe UI", 8F);
            lblStepsHeader.Location = new Point(14, 228);
            lblStepsHeader.Name = "lblStepsHeader";
            lblStepsHeader.Size = new Size(295, 13);
            lblStepsHeader.Text = "Click Step to Toggle ON/OFF (Right-click to shift note):";

            pnlStepIndicators.BackColor = Color.FromArgb(20, 24, 28);
            pnlStepIndicators.BorderStyle = BorderStyle.FixedSingle;
            pnlStepIndicators.Location = new Point(14, 244);
            pnlStepIndicators.Name = "pnlStepIndicators";
            pnlStepIndicators.Size = new Size(307, 146);
            pnlStepIndicators.TabIndex = 11;

            // -------------------------------------------------------------
            // GROUP 3: BACKING DRUMS & MASTER FX
            // -------------------------------------------------------------
            grpBackingFx.Controls.Add(btnDrumsToggle);
            grpBackingFx.Controls.Add(btnChorusToggle);
            grpBackingFx.Controls.Add(knobKickVol);
            grpBackingFx.Controls.Add(knobSnareHatVol);
            grpBackingFx.Controls.Add(knobChorusDepth);
            grpBackingFx.Controls.Add(knobMasterVol);
            grpBackingFx.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            grpBackingFx.Location = new Point(692, 138);
            grpBackingFx.Name = "grpBackingFx";
            grpBackingFx.Size = new Size(325, 400);
            grpBackingFx.TabIndex = 13;
            grpBackingFx.TabStop = false;
            grpBackingFx.Text = "3. Backing Drums & Master FX";

            // Front Drum Toggle Button
            btnDrumsToggle.BackColor = Color.FromArgb(70, 75, 85);
            btnDrumsToggle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDrumsToggle.ForeColor = Color.White;
            btnDrumsToggle.Location = new Point(12, 22);
            btnDrumsToggle.Name = "btnDrumsToggle";
            btnDrumsToggle.Size = new Size(301, 32);
            btnDrumsToggle.TabIndex = 0;
            btnDrumsToggle.TabStop = false;
            btnDrumsToggle.Text = "⚪ DRUM MACHINE: OFF (Click to Enable)";
            btnDrumsToggle.UseVisualStyleBackColor = false;
            btnDrumsToggle.Click += btnDrumsToggle_Click;

            // Drum Knobs
            knobKickVol.Location = new Point(40, 62);
            knobKickVol.Minimum = 0f;
            knobKickVol.Maximum = 100f;
            knobKickVol.Value = 100f;
            knobKickVol.DefaultValue = 100f;
            knobKickVol.Title = "KICK VOL";
            knobKickVol.ValueFormat = "{0:F0}%";
            knobKickVol.ArcColor = Color.FromArgb(240, 100, 50);
            knobKickVol.Description = "Kick drum volume.";
            knobKickVol.ValueChanged += knobKickVol_ValueChanged;
            knobKickVol.DescriptionHovered += OnKnobHovered;

            knobSnareHatVol.Location = new Point(180, 62);
            knobSnareHatVol.Minimum = 0f;
            knobSnareHatVol.Maximum = 100f;
            knobSnareHatVol.Value = 80f;
            knobSnareHatVol.DefaultValue = 80f;
            knobSnareHatVol.Title = "SNARE / HAT";
            knobSnareHatVol.ValueFormat = "{0:F0}%";
            knobSnareHatVol.ArcColor = Color.FromArgb(240, 100, 50);
            knobSnareHatVol.Description = "Snare and hi-hat volume.";
            knobSnareHatVol.ValueChanged += knobSnareHatVol_ValueChanged;
            knobSnareHatVol.DescriptionHovered += OnKnobHovered;

            // Front Chorus Toggle Button
            btnChorusToggle.BackColor = Color.FromArgb(70, 75, 85);
            btnChorusToggle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnChorusToggle.ForeColor = Color.White;
            btnChorusToggle.Location = new Point(12, 165);
            btnChorusToggle.Name = "btnChorusToggle";
            btnChorusToggle.Size = new Size(301, 32);
            btnChorusToggle.TabIndex = 3;
            btnChorusToggle.TabStop = false;
            btnChorusToggle.Text = "⚪ STEREO CHORUS: OFF (Click to Enable)";
            btnChorusToggle.UseVisualStyleBackColor = false;
            btnChorusToggle.Click += btnChorusToggle_Click;

            // Chorus & Master Volume Knobs
            knobChorusDepth.Location = new Point(40, 210);
            knobChorusDepth.Minimum = 0f;
            knobChorusDepth.Maximum = 100f;
            knobChorusDepth.Value = 50f;
            knobChorusDepth.DefaultValue = 50f;
            knobChorusDepth.Title = "CHORUS WET";
            knobChorusDepth.ValueFormat = "{0:F0}%";
            knobChorusDepth.ArcColor = Color.FromArgb(160, 100, 240);
            knobChorusDepth.Description = "Chorus wet mix. Right-click to bypass.";
            knobChorusDepth.ValueChanged += knobChorusDepth_ValueChanged;
            knobChorusDepth.DescriptionHovered += OnKnobHovered;

            knobMasterVol.Location = new Point(180, 210);
            knobMasterVol.Minimum = 0f;
            knobMasterVol.Maximum = 100f;
            knobMasterVol.Value = 85f;
            knobMasterVol.DefaultValue = 85f;
            knobMasterVol.Title = "MASTER VOL";
            knobMasterVol.ValueFormat = "{0:F0}%";
            knobMasterVol.ArcColor = Color.LimeGreen;
            knobMasterVol.Description = "Master groovebox output volume.";
            knobMasterVol.ValueChanged += knobMasterVol_ValueChanged;
            knobMasterVol.DescriptionHovered += OnKnobHovered;

            // -------------------------------------------------------------
            // FOOTER: NOTE HISTORY & PERFORMANCE TIPS
            // -------------------------------------------------------------
            textBoxHistory.BackColor = Color.FromArgb(245, 247, 250);
            textBoxHistory.Font = new Font("Consolas", 8.5F);
            textBoxHistory.Location = new Point(14, 550);
            textBoxHistory.Multiline = true;
            textBoxHistory.Name = "textBoxHistory";
            textBoxHistory.ReadOnly = true;
            textBoxHistory.ScrollBars = ScrollBars.Vertical;
            textBoxHistory.Size = new Size(325, 125);
            textBoxHistory.TabIndex = 14;
            textBoxHistory.TabStop = false;

            lblFooterInfo.BorderStyle = BorderStyle.FixedSingle;
            lblFooterInfo.Font = new Font("Segoe UI", 9F);
            lblFooterInfo.Location = new Point(348, 550);
            lblFooterInfo.Name = "lblFooterInfo";
            lblFooterInfo.Padding = new Padding(8);
            lblFooterInfo.Size = new Size(669, 125);
            lblFooterInfo.TabIndex = 15;
            lblFooterInfo.Text = "Keyboard Mapping: C, D, E, F, G, A, B  |  Sharps: Q (C#), W (D#), V (F#), T (G#), U (A#)\r\n  Octaves: Press [-] / [+] buttons or MediaPrev / MediaNext keys\r\n\r\nWorkflow:\r\n  * Clean Start: Plays raw Saw wave! Sub-oscillators, Drums & Chorus start OFF for complete purity.\r\n  * Knobs: Row 1 = Cutoff, Resonance, Env Depth, Env Decay! Right-click any knob to BYPASS it.\r\n  * Arp / Interval Copier: Type custom intervals (e.g. 0, 12, 2) and click Arp/Copy to pattern steps!";

            // Invisible legacy controls for backward compatibility
            btnSoundShapingToggle.Visible = false;
            chkSoundShaping.Visible = false;
            chkSubSquare.Visible = false;
            chkDrumsEnabled.Visible = false;
            chkChorusEnabled.Visible = false;
            trackBarCutoff.Visible = false;
            trackBarResonance.Visible = false;
            trackBarEnvMod.Visible = false;
            trackBarFilterDecay.Visible = false;
            trackBarSubMix.Visible = false;
            trackBarAmpRelease.Visible = false;
            trackBarKickVol.Visible = false;
            trackBarSnareHatVol.Visible = false;
            trackBarChorusDepth.Visible = false;
            trackBarMasterVol.Visible = false;
            ModulatorFrequencyTrackBar.Visible = false;
            modulationIndexTrackBar.Visible = false;
            comboBoxFmMultiplier.Visible = false;
            labelModulationIndex.Visible = false;
            labelFmMultiplier.Visible = false;
            label1.Visible = false;
            FM.Visible = false;
            Instrument.Visible = false;

            // KeyPlayerForm
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1034, 690);
            Controls.Add(lblFooterInfo);
            Controls.Add(textBoxHistory);
            Controls.Add(grpBackingFx);
            Controls.Add(grpSequencer);
            Controls.Add(grpSynth);
            Controls.Add(Reset);
            Controls.Add(labelWaveTypeMode);
            Controls.Add(btnOctaveUp);
            Controls.Add(btnOctaveDown);
            Controls.Add(labelOctave);
            Controls.Add(chkAntiAliasing);
            Controls.Add(comboBoxWaveType);
            Controls.Add(labelWaveform);
            Controls.Add(labelAppTitle);
            Controls.Add(spectrumViewer);
            Controls.Add(waveformViewer);
            Controls.Add(btnSoundShapingToggle);
            Controls.Add(chkSoundShaping);
            Controls.Add(chkSubSquare);
            Controls.Add(chkDrumsEnabled);
            Controls.Add(chkChorusEnabled);
            Controls.Add(trackBarCutoff);
            Controls.Add(trackBarResonance);
            Controls.Add(trackBarEnvMod);
            Controls.Add(trackBarFilterDecay);
            Controls.Add(trackBarSubMix);
            Controls.Add(trackBarAmpRelease);
            Controls.Add(trackBarKickVol);
            Controls.Add(trackBarSnareHatVol);
            Controls.Add(trackBarChorusDepth);
            Controls.Add(trackBarMasterVol);
            Controls.Add(ModulatorFrequencyTrackBar);
            Controls.Add(modulationIndexTrackBar);
            Controls.Add(comboBoxFmMultiplier);
            Controls.Add(labelModulationIndex);
            Controls.Add(labelFmMultiplier);
            Controls.Add(label1);
            Controls.Add(FM);
            Controls.Add(Instrument);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "KeyPlayerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Acid Bass Groovebox";
            FormClosing += KeyPlayerForm_FormClosing;
            KeyDown += KeyPlayerForm_KeyDown;
            KeyUp += KeyPlayerForm_KeyUp;
            ((System.ComponentModel.ISupportInitialize)trackBarBpm).EndInit();
            grpSynth.ResumeLayout(false);
            grpSynth.PerformLayout();
            grpSequencer.ResumeLayout(false);
            grpSequencer.PerformLayout();
            grpBackingFx.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private WaveformViewer waveformViewer;
        private SpectrumViewer spectrumViewer;
        private Label labelAppTitle;
        private Label labelWaveform;
        private ComboBox comboBoxWaveType;
        private CheckBox chkAntiAliasing;
        private Label labelOctave;
        private Button btnOctaveDown;
        private Button btnOctaveUp;
        private Button Reset;
        private Label labelWaveTypeMode;

        private GroupBox grpSynth;
        private KnobControl knobCutoff;
        private KnobControl knobResonance;
        private KnobControl knobEnvMod;
        private KnobControl knobFilterDecay;
        private KnobControl knobSub1Mix;
        private KnobControl knobSub1Pitch;
        private KnobControl knobSub2Mix;
        private KnobControl knobSub2Pitch;
        private KnobControl knobAmpRelease;
        private Label lblSub1Wave;
        private ComboBox cboSub1Wave;
        private Label lblSub2Wave;
        private ComboBox cboSub2Wave;

        private GroupBox grpSequencer;
        private Button btnSeqPlayStop;
        private Label lblBpm;
        private TrackBar trackBarBpm;
        private RadioButton rbPatternA;
        private RadioButton rbPatternB;
        private Label lblTranspose;
        private Button btnTransposeRoot;
        private Button btnTransposeFourth;
        private Button btnTransposeFifth;
        private Label lblArpTitle;
        private TextBox txtArpOffsets;
        private Button btnApplyArp;
        private Label lblStepsHeader;
        private Panel pnlStepIndicators;

        private GroupBox grpBackingFx;
        private Button btnDrumsToggle;
        private Button btnChorusToggle;
        private KnobControl knobKickVol;
        private KnobControl knobSnareHatVol;
        private KnobControl knobChorusDepth;
        private KnobControl knobMasterVol;

        private TextBox textBoxHistory;
        private Label lblFooterInfo;

        // Legacy compatibility
        private Button btnSoundShapingToggle;
        private CheckBox chkSoundShaping;
        private CheckBox chkSubSquare;
        private CheckBox chkDrumsEnabled;
        private CheckBox chkChorusEnabled;
        private TrackBar trackBarCutoff;
        private TrackBar trackBarResonance;
        private TrackBar trackBarEnvMod;
        private TrackBar trackBarFilterDecay;
        private TrackBar trackBarSubMix;
        private TrackBar trackBarAmpRelease;
        private TrackBar trackBarKickVol;
        private TrackBar trackBarSnareHatVol;
        private TrackBar trackBarChorusDepth;
        private TrackBar trackBarMasterVol;
        private TrackBar ModulatorFrequencyTrackBar;
        private TrackBar modulationIndexTrackBar;
        private ComboBox comboBoxFmMultiplier;
        private Label labelModulationIndex;
        private Label labelFmMultiplier;
        private Label label1;
        private Label FM;
        private Label Instrument;
    }
}
