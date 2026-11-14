using System;
using NAudio.Wave;

namespace audiosynth
{
    /// <summary>
    /// Synthesized 808-style drum machine engine.
    /// Provides punchy Kick, snappy Snare, and crisp Hi-Hats,
    /// locked to the 16-step sequencer clock.
    /// </summary>
    public class DrumMachine : ISampleProvider
    {
        private readonly int sampleRate;
        private readonly Random random = new Random();

        public WaveFormat WaveFormat { get; }

        public bool IsEnabled { get; set; } = true;
        public float MasterVolume { get; set; } = 0.7f;
        public float KickVolume { get; set; } = 1.0f;
        public float SnareVolume { get; set; } = 0.8f;
        public float HatVolume { get; set; } = 0.6f;

        // --- Kick Voice State ---
        private double kickPhase;
        private double kickCurrentFreq;
        private double kickAmp;

        // --- Snare Voice State ---
        private double snareTonePhase;
        private double snareAmp;
        private double snareNoiseFilter;

        // --- Hi-Hat Voice State ---
        private double hatAmp;
        private double hatNoiseFilter;

        public DrumMachine(int sampleRate = 44100)
        {
            this.sampleRate = sampleRate;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
        }

        public void TriggerStep(int stepIndex)
        {
            if (!IsEnabled) return;

            // Step 0..15
            // Standard House / Techno Groove:
            // Kick: 0, 4, 8, 12 (Four on the floor)
            if (stepIndex % 4 == 0)
            {
                TriggerKick();
            }

            // Snare: 4, 12 (Backbeat)
            if (stepIndex == 4 || stepIndex == 12)
            {
                TriggerSnare();
            }

            // Hi-Hat: 2, 6, 10, 14 (Offbeats) + light ghost on 16ths
            if (stepIndex % 2 == 0)
            {
                TriggerHat();
            }
        }

        public void TriggerKick()
        {
            kickPhase = 0.0;
            kickCurrentFreq = 160.0; // Initial pitch drop start
            kickAmp = 1.0;
        }

        public void TriggerSnare()
        {
            snareTonePhase = 0.0;
            snareAmp = 1.0;
            snareNoiseFilter = 0.0;
        }

        public void TriggerHat()
        {
            hatAmp = 1.0;
            hatNoiseFilter = 0.0;
        }

        public float GetNextSample()
        {
            if (!IsEnabled) return 0f;

            double mixed = 0.0;

            // 1. Kick synthesis (pitch-dropping sine wave with exponential volume decay)
            if (kickAmp > 0.001)
            {
                mixed += Math.Sin(2.0 * Math.PI * kickPhase) * kickAmp * KickVolume;

                kickPhase += kickCurrentFreq / sampleRate;
                if (kickPhase >= 1.0) kickPhase -= 1.0;

                // Exponential drop from 160 Hz down to 45 Hz
                kickCurrentFreq = Math.Max(45.0, kickCurrentFreq * 0.9985);
                kickAmp *= 0.99965; // Volume decay
            }

            // 2. Snare synthesis (180 Hz tone + high-pass white noise burst)
            if (snareAmp > 0.001)
            {
                double whiteNoise = random.NextDouble() * 2.0 - 1.0;
                // Simple high-pass filter for snap
                snareNoiseFilter = whiteNoise - snareNoiseFilter * 0.4;
                double tone = Math.Sin(2.0 * Math.PI * snareTonePhase);

                mixed += (tone * 0.35 + snareNoiseFilter * 0.65) * snareAmp * SnareVolume;

                snareTonePhase += 185.0 / sampleRate;
                if (snareTonePhase >= 1.0) snareTonePhase -= 1.0;

                snareAmp *= 0.9992;
            }

            // 3. Hi-Hat synthesis (high-frequency filtered noise with fast decay)
            if (hatAmp > 0.001)
            {
                double whiteNoise = random.NextDouble() * 2.0 - 1.0;
                // High pass filter for metallic sizzle
                hatNoiseFilter = whiteNoise - hatNoiseFilter * 0.7;

                mixed += hatNoiseFilter * hatAmp * HatVolume;
                hatAmp *= 0.9975; // Fast decay
            }

            return (float)(mixed * MasterVolume);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = GetNextSample();
            }
            return count;
        }
    }
}
