using System;

namespace audiosynth
{
    public struct SequencerStep
    {
        public bool Active;
        public int NoteOffset; // In semitones from root
        public float GateLength; // 0.1f to 0.95f of step duration

        public SequencerStep(bool active, int noteOffset, float gateLength = 0.65f)
        {
            Active = active;
            NoteOffset = noteOffset;
            GateLength = gateLength;
        }
    }

    public enum SequencerPatternChoice
    {
        PatternA,
        PatternB
    }

    /// <summary>
    /// Sample-accurate 16-step sequencer with dual patterns (A/B)
    /// and real-time global transposition (+5 semitones to shift to fourth).
    /// </summary>
    public class StepSequencer
    {
        private readonly MonoBassVoice bassVoice;
        private readonly int sampleRate;

        private readonly SequencerStep[] patternA = new SequencerStep[16];
        private readonly SequencerStep[] patternB = new SequencerStep[16];

        private int currentStep = -1;
        private long sampleCounter = 0;
        private long stepSampleLength = 0;
        private long gateSamplesRemaining = 0;
        private bool isGateOpen = false;

        public bool IsRunning { get; set; } = false;
        public double BPM { get; set; } = 125.0;
        public double BaseRootFrequency { get; set; } = 55.0; // A1 (55 Hz)
        public int TranspositionSemitones { get; set; } = 0; // Global pitch offset (e.g. +5 semitones)
        public SequencerPatternChoice CurrentPatternChoice { get; set; } = SequencerPatternChoice.PatternA;

        public int CurrentStepIndex => currentStep;

        public event Action<int>? StepTriggered;

        public StepSequencer(MonoBassVoice bassVoice, int sampleRate = 44100)
        {
            this.bassVoice = bassVoice;
            this.sampleRate = sampleRate;
            InitializeDefaultPatterns();
            RecalculateStepLength();
        }

        private void InitializeDefaultPatterns()
        {
            // Pattern A: Classic Acid / Bass groove (Root A1)
            // Steps: 0, 0, 12, 0, 3, 0, 7, 10, 0, 0, 12, 0, 3, 5, 7, 0
            int[] offsetsA = { 0, 0, 12, 0, 3, 0, 7, 10, 0, 0, 12, 0, 3, 5, 7, 0 };
            bool[] activeA = { true, true, true, false, true, false, true, true, true, true, true, false, true, true, true, false };
            for (int i = 0; i < 16; i++)
            {
                patternA[i] = new SequencerStep(activeA[i], offsetsA[i], 0.65f);
            }

            // Pattern B: Bouncy syncopated Funk/Techno groove
            // Steps: 0, 12, 0, 10, 12, 0, 7, 0, 0, 12, 10, 0, 5, 7, 8, 7
            int[] offsetsB = { 0, 12, 0, 10, 12, 0, 7, 0, 0, 12, 10, 0, 5, 7, 8, 7 };
            bool[] activeB = { true, true, false, true, true, false, true, false, true, true, true, false, true, true, true, true };
            for (int i = 0; i < 16; i++)
            {
                patternB[i] = new SequencerStep(activeB[i], offsetsB[i], 0.55f);
            }
        }

        public void RecalculateStepLength()
        {
            // 1 sixteenth note = (60 / (BPM * 4)) seconds
            double secondsPerStep = 60.0 / (Math.Max(30.0, BPM) * 4.0);
            stepSampleLength = (long)(secondsPerStep * sampleRate);
        }

        public bool IsStepActive(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= 16) return false;
            var pattern = (CurrentPatternChoice == SequencerPatternChoice.PatternA) ? patternA : patternB;
            return pattern[stepIndex].Active;
        }

        public void ToggleStepActive(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= 16) return;
            var pattern = (CurrentPatternChoice == SequencerPatternChoice.PatternA) ? patternA : patternB;
            pattern[stepIndex].Active = !pattern[stepIndex].Active;
        }

        public int GetStepNoteOffset(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= 16) return 0;
            var pattern = (CurrentPatternChoice == SequencerPatternChoice.PatternA) ? patternA : patternB;
            return pattern[stepIndex].NoteOffset;
        }

        public void CycleStepNote(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= 16) return;
            var pattern = (CurrentPatternChoice == SequencerPatternChoice.PatternA) ? patternA : patternB;
            // Cycle common bass intervals: 0 (root), 3 (min 3rd), 5 (4th), 7 (5th), 10 (min 7th), 12 (octave)
            int[] scales = { 0, 3, 5, 7, 10, 12 };
            int current = pattern[stepIndex].NoteOffset;
            int nextIdx = 0;
            for (int i = 0; i < scales.Length; i++)
            {
                if (scales[i] == current)
                {
                    nextIdx = (i + 1) % scales.Length;
                    break;
                }
            }
            pattern[stepIndex].NoteOffset = scales[nextIdx];
        }

        public void ApplyCustomPitchSequence(string csvOffsets)
        {
            if (string.IsNullOrWhiteSpace(csvOffsets)) return;

            var parts = csvOffsets.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var offsets = new System.Collections.Generic.List<int>();
            foreach (var p in parts)
            {
                if (int.TryParse(p.Trim(), out int val))
                {
                    offsets.Add(val);
                }
            }

            if (offsets.Count == 0) return;

            var pattern = (CurrentPatternChoice == SequencerPatternChoice.PatternA) ? patternA : patternB;
            for (int i = 0; i < 16; i++)
            {
                pattern[i].NoteOffset = offsets[i % offsets.Count];
            }
        }

        public void Start()
        {
            RecalculateStepLength();
            sampleCounter = stepSampleLength; // Force immediate trigger on first sample
            currentStep = -1;
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
            if (isGateOpen)
            {
                bassVoice.NoteOff();
                isGateOpen = false;
            }
            currentStep = -1;
        }

        /// <summary>
        /// Advances sequencer timing by 1 sample. Should be called per sample from the audio thread.
        /// </summary>
        public void AdvanceSample()
        {
            if (!IsRunning) return;

            // Handle gate-off for active note
            if (isGateOpen)
            {
                gateSamplesRemaining--;
                if (gateSamplesRemaining <= 0)
                {
                    bassVoice.NoteOff();
                    isGateOpen = false;
                }
            }

            sampleCounter++;
            if (sampleCounter >= stepSampleLength)
            {
                sampleCounter = 0;
                currentStep = (currentStep + 1) % 16;

                // Fire event for UI visualization if subscribed
                StepTriggered?.Invoke(currentStep);

                var activePattern = (CurrentPatternChoice == SequencerPatternChoice.PatternA) ? patternA : patternB;
                var step = activePattern[currentStep];

                if (step.Active)
                {
                    int totalSemitones = step.NoteOffset + TranspositionSemitones;
                    double stepFreq = BaseRootFrequency * Math.Pow(2.0, totalSemitones / 12.0);

                    bassVoice.NoteOn(stepFreq);
                    isGateOpen = true;
                    gateSamplesRemaining = (long)(stepSampleLength * step.GateLength);
                }
            }
        }
    }
}
