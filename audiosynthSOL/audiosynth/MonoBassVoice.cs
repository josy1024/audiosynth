using System;
using NAudio.Wave;

namespace audiosynth
{
    public class MonoBassVoice : ISampleProvider
    {
        private readonly PolyBlepOscillator oscillator;
        private readonly StateVariableFilter filter;
        private readonly ADSR filterAdsr;
        private readonly ADSR ampAdsr;

        public WaveFormat WaveFormat { get; }

        public WaveType Waveform
        {
            get => oscillator.Waveform;
            set => oscillator.Waveform = value;
        }

        public bool UseAntiAliasing
        {
            get => oscillator.UsePolyBlep;
            set => oscillator.UsePolyBlep = value;
        }

        public bool SoundShapingEnabled { get; set; } = true;

        // Individual knob bypass flags (toggled on right-click)
        public bool CutoffBypassed { get; set; } = false;
        public bool ResonanceBypassed { get; set; } = false;
        public bool EnvModBypassed { get; set; } = false;
        public bool Sub1Bypassed { get; set; } = false;
        public bool Sub2Bypassed { get; set; } = false;

        // Filter parameters
        public double BaseCutoff { get; set; } = 850.0;
        public double Resonance { get; set; } = 1.0;     // Flat initial resonance (Q=1.0)
        public double FilterEnvAmount { get; set; } = 3.5;
        public float MasterGain { get; set; } = 0.85f;

        // 2 Independent Sub-Oscillators
        public float Sub1Mix
        {
            get => oscillator.Sub1Mix;
            set => oscillator.Sub1Mix = Math.Clamp(value, 0f, 1f);
        }

        public float Sub2Mix
        {
            get => oscillator.Sub2Mix;
            set => oscillator.Sub2Mix = Math.Clamp(value, 0f, 1f);
        }

        public SubOscWaveType Sub1Waveform
        {
            get => oscillator.Sub1Waveform;
            set => oscillator.Sub1Waveform = value;
        }

        public SubOscWaveType Sub2Waveform
        {
            get => oscillator.Sub2Waveform;
            set => oscillator.Sub2Waveform = value;
        }

        // Sub pitch offsets in semitones (-24..+24)
        public float Sub1PitchSemitones
        {
            get => oscillator.Sub1PitchSemitones;
            set => oscillator.Sub1PitchSemitones = Math.Clamp(value, -24f, 24f);
        }

        public float Sub2PitchSemitones
        {
            get => oscillator.Sub2PitchSemitones;
            set => oscillator.Sub2PitchSemitones = Math.Clamp(value, -24f, 24f);
        }

        // Envelope timings
        public float FilterAttack
        {
            get => filterAdsr.AttackTime;
            set => filterAdsr.AttackTime = value;
        }

        public float FilterDecay
        {
            get => filterAdsr.DecayTime;
            set => filterAdsr.DecayTime = value;
        }

        public float FilterSustain
        {
            get => filterAdsr.SustainLevel;
            set => filterAdsr.SustainLevel = value;
        }

        public float FilterRelease
        {
            get => filterAdsr.ReleaseTime;
            set => filterAdsr.ReleaseTime = value;
        }

        public float AmpAttack
        {
            get => ampAdsr.AttackTime;
            set => ampAdsr.AttackTime = value;
        }

        public float AmpDecay
        {
            get => ampAdsr.DecayTime;
            set => ampAdsr.DecayTime = value;
        }

        public float AmpSustain
        {
            get => ampAdsr.SustainLevel;
            set => ampAdsr.SustainLevel = value;
        }

        public float AmpRelease
        {
            get => ampAdsr.ReleaseTime;
            set => ampAdsr.ReleaseTime = value;
        }

        public double Frequency
        {
            get => oscillator.Frequency;
            set => oscillator.Frequency = value;
        }

        public bool IsActive => ampAdsr.State != ADSR.EnvelopeState.Idle;

        public MonoBassVoice(int sampleRate = 44100)
        {
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);

            oscillator = new PolyBlepOscillator(sampleRate)
            {
                Waveform = WaveType.Saw,
                UsePolyBlep = false, // Clean classic raw saw by default
                Sub1Mix = 0.0f,     // 0% on startup
                Sub2Mix = 0.0f      // 0% on startup
            };

            filter = new StateVariableFilter(sampleRate)
            {
                Is24dB = true
            };

            filterAdsr = new ADSR(sampleRate, attackTime: 0.002f, decayTime: 0.22f, sustainLevel: 0.15f, releaseTime: 0.15f, startActive: false);
            ampAdsr = new ADSR(sampleRate, attackTime: 0.002f, decayTime: 0.35f, sustainLevel: 0.45f, releaseTime: 0.12f, startActive: false);
        }

        public void NoteOn(double freq, float velocity = 1.0f)
        {
            oscillator.Frequency = freq;

            filterAdsr.Trigger();
            ampAdsr.Trigger();
        }

        public void NoteOff()
        {
            filterAdsr.Release();
            ampAdsr.Release();
        }

        public void Stop()
        {
            filterAdsr.Reset();
            ampAdsr.Reset();
            filter.Reset();
        }

        public int Read(float[] buffer, int offset, int count)
        {
            // Sync sub-bypasses
            float effectiveSub1 = (Sub1Bypassed || !SoundShapingEnabled) ? 0f : Sub1Mix;
            float effectiveSub2 = (Sub2Bypassed || !SoundShapingEnabled) ? 0f : Sub2Mix;
            oscillator.Sub1Mix = effectiveSub1;
            oscillator.Sub2Mix = effectiveSub2;

            for (int i = 0; i < count; i++)
            {
                if (ampAdsr.State == ADSR.EnvelopeState.Idle)
                {
                    buffer[offset + i] = 0f;
                    continue;
                }

                float rawSample = oscillator.GetNextSample();
                float processedSample;

                // If Cutoff is bypassed or SoundShaping is disabled -> 100% pure raw wave bypass!
                if (CutoffBypassed || !SoundShapingEnabled)
                {
                    processedSample = rawSample;
                }
                else
                {
                    float envVal = filterAdsr.GetNextSample();
                    double depth = EnvModBypassed ? 0.0 : FilterEnvAmount;
                    double dynamicCutoff = BaseCutoff * Math.Pow(2.0, envVal * depth);

                    double effectiveResonance = ResonanceBypassed ? 0.5 : Resonance;
                    processedSample = filter.Process(rawSample, dynamicCutoff, effectiveResonance);
                }

                float ampVal = ampAdsr.GetNextSample();
                float finalSample = processedSample * ampVal * MasterGain;

                buffer[offset + i] = finalSample;
            }

            return count;
        }
    }
}
