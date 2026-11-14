using System;

namespace audiosynth
{
    /// <summary>
    /// Resonant Low-Pass Filter (Zero-Delay Feedback State Variable Filter).
    /// Supports both 12 dB/oct (2-pole) and 24 dB/oct (4-pole cascade) slopes,
    /// with soft-saturation on high resonance to mimic analog overdrive.
    /// </summary>
    public class StateVariableFilter
    {
        private readonly int sampleRate;

        // Stage 1 state
        private double s1_1;
        private double s2_1;

        // Stage 2 state (for 24 dB/oct cascade)
        private double s1_2;
        private double s2_2;

        public bool Is24dB { get; set; } = true; // 24dB/oct default for fat bass

        public StateVariableFilter(int sampleRate = 44100)
        {
            this.sampleRate = sampleRate;
            Reset();
        }

        public void Reset()
        {
            s1_1 = 0;
            s2_1 = 0;
            s1_2 = 0;
            s2_2 = 0;
        }

        /// <summary>
        /// Processes a single audio sample through the resonant filter.
        /// </summary>
        /// <param name="input">Incoming audio sample</param>
        /// <param name="cutoffHz">Cutoff frequency in Hz (20 Hz to 20,000 Hz)</param>
        /// <param name="resonance">Resonance Q (0.5 = no resonance, 10+ = self-oscillation)</param>
        public float Process(float input, double cutoffHz, double resonance)
        {
            // Clamp parameters for stability
            cutoffHz = Math.Clamp(cutoffHz, 20.0, sampleRate * 0.48);
            resonance = Math.Clamp(resonance, 0.5, 20.0);

            // Prewarp cutoff
            double g = Math.Tan(Math.PI * cutoffHz / sampleRate);
            double k = 1.0 / resonance;

            // Stage 1
            double a1 = 1.0 / (1.0 + g * (g + k));
            double a2 = g * a1;
            double a3 = g * a2;

            double v3_1 = input - s2_1;
            double v1_1 = a1 * s1_1 + a2 * v3_1;
            double v2_1 = s2_1 + a2 * s1_1 + a3 * v3_1;

            // Soft saturation in state updates to prevent blowout at high resonance
            s1_1 = SoftClip(2.0 * v1_1 - s1_1);
            s2_1 = SoftClip(2.0 * v2_1 - s2_1);

            double out1 = v2_1;

            if (!Is24dB)
            {
                return (float)out1;
            }

            // Stage 2 for 24 dB/oct slope
            double v3_2 = out1 - s2_2;
            double v1_2 = a1 * s1_2 + a2 * v3_2;
            double v2_2 = s2_2 + a2 * s1_2 + a3 * v3_2;

            s1_2 = SoftClip(2.0 * v1_2 - s1_2);
            s2_2 = SoftClip(2.0 * v2_2 - s2_2);

            return (float)v2_2;
        }

        private static double SoftClip(double x)
        {
            // Polynomial soft saturator between -1.5 and 1.5
            if (x > 1.5) return 1.0;
            if (x < -1.5) return -1.0;
            return x - (x * x * x) * (1.0 / 6.75);
        }
    }
}
