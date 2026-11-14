using System;

namespace audiosynth
{
    public class PolyBlepOscillator
    {
        private double phase;
        private double subPhase1;
        private double subPhase2;
        private double modulatorPhase;
        private readonly Random random = new Random();

        public double Frequency { get; set; } = 110.0;
        public int SampleRate { get; set; } = 44100;

        // Default clean classic saw wave
        public WaveType Waveform { get; set; } = WaveType.Saw;
        public bool UsePolyBlep { get; set; } = false;

        public float PulseWidth { get; set; } = 0.5f;

        // Sub 1: pitch offset in semitones (-24..+24), default -12 (one octave below)
        public float Sub1Mix { get; set; } = 0.0f; // 0% by default for pure clean saw!
        public SubOscWaveType Sub1Waveform { get; set; } = SubOscWaveType.CopyPrimary;
        public float Sub1PitchSemitones { get; set; } = -12f; // -12 = one octave below

        // Sub 2: pitch offset in semitones (-24..+24), default -24 (two octaves below)
        public float Sub2Mix { get; set; } = 0.0f; // 0% by default for pure clean saw!
        public SubOscWaveType Sub2Waveform { get; set; } = SubOscWaveType.CopyPrimary;
        public float Sub2PitchSemitones { get; set; } = -24f; // -24 = two octaves below

        public double ModulatorFrequencyMultiplier { get; set; } = 1.0;
        public double ModulationIndex { get; set; } = 1.5;

        public PolyBlepOscillator(int sampleRate = 44100)
        {
            SampleRate = sampleRate;
        }

        public void ResetPhase()
        {
            phase = 0.0;
            subPhase1 = 0.0;
            subPhase2 = 0.0;
            modulatorPhase = 0.0;
        }

        public float GetNextSample()
        {
            if (Frequency <= 0 || SampleRate <= 0) return 0f;

            double dt = Frequency / SampleRate;
            if (dt >= 0.5) dt = 0.499;

            // 1. Primary Waveform
            double sample = GenerateWave(Waveform, phase, dt);

            // 2. Sub-Oscillator 1 — user-defined semitone offset (-24..+24)
            if (Sub1Mix > 0.001f)
            {
                double sub1Ratio = Math.Pow(2.0, Sub1PitchSemitones / 12.0);
                double subDt1 = Math.Clamp(dt * sub1Ratio, 0.0001, 0.499);
                double sub1 = GenerateSubSample(Sub1Waveform, subPhase1, subDt1);
                sample = (sample * (1.0f - Sub1Mix * 0.4f)) + (sub1 * Sub1Mix);

                subPhase1 += subDt1;
                if (subPhase1 >= 1.0) subPhase1 -= 1.0;
            }

            // 3. Sub-Oscillator 2 — user-defined semitone offset (-24..+24)
            if (Sub2Mix > 0.001f)
            {
                double sub2Ratio = Math.Pow(2.0, Sub2PitchSemitones / 12.0);
                double subDt2 = Math.Clamp(dt * sub2Ratio, 0.0001, 0.499);
                double sub2 = GenerateSubSample(Sub2Waveform, subPhase2, subDt2);
                sample = (sample * (1.0f - Sub2Mix * 0.4f)) + (sub2 * Sub2Mix);

                subPhase2 += subDt2;
                if (subPhase2 >= 1.0) subPhase2 -= 1.0;
            }

            phase += dt;
            if (phase >= 1.0) phase -= 1.0;

            return (float)sample;
        }

        private double GenerateSubSample(SubOscWaveType subType, double p, double dt)
        {
            switch (subType)
            {
                case SubOscWaveType.CopyPrimary:
                    return GenerateWave(Waveform, p, dt);
                case SubOscWaveType.Square:
                    return (p % 1.0 < 0.5) ? 1.0 : -1.0;
                case SubOscWaveType.Sine:
                default:
                    return Math.Sin(2.0 * Math.PI * p);
            }
        }

        private double GenerateWave(WaveType type, double p, double dt)
        {
            switch (type)
            {
                case WaveType.Sine:
                    return Math.Sin(2.0 * Math.PI * p);

                case WaveType.Saw:
                    if (UsePolyBlep)
                    {
                        double saw = (2.0 * p) - 1.0;
                        return saw - PolyBlep(p, dt);
                    }
                    // Pure raw classic saw wave
                    return 2.0 * (p - Math.Floor(0.5 + p));

                case WaveType.Square:
                    if (UsePolyBlep)
                    {
                        double sq = (p < 0.5) ? 1.0 : -1.0;
                        sq += PolyBlep(p, dt);
                        sq -= PolyBlep((p + 0.5) % 1.0, dt);
                        return sq;
                    }
                    return (p % 1.0 < 0.5) ? 1.0 : -1.0;

                case WaveType.Triangle:
                    double triSaw = (p % 1.0) * 2.0 - 1.0;
                    return 2.0 * (Math.Abs(triSaw) - 0.5);

                case WaveType.Pulse:
                    return (p % 1.0 < PulseWidth) ? 1.0 : -1.0;

                case WaveType.FM:
                    double modFreq = Frequency * ModulatorFrequencyMultiplier;
                    double modPhase = (p + ModulationIndex * Math.Sin(2.0 * Math.PI * modulatorPhase));
                    modulatorPhase += modFreq / SampleRate;
                    if (modulatorPhase >= 1.0) modulatorPhase -= 1.0;
                    return Math.Sin(2.0 * Math.PI * modPhase);

                case WaveType.Noise:
                    return random.NextDouble() * 2.0 - 1.0;

                default:
                    return 0.0;
            }
        }

        private static double PolyBlep(double t, double dt)
        {
            if (t < dt)
            {
                double n = t / dt;
                return n + n - n * n - 1.0;
            }
            if (t > 1.0 - dt)
            {
                double n = (t - 1.0) / dt;
                return n * n + n + n + 1.0;
            }
            return 0.0;
        }
    }
}
