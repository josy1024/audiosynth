using System;

namespace audiosynth
{
    /// <summary>
    /// Stereo modulated delay chorus effect.
    /// Modulates dual delay lines with a quadrature (90-degree offset) LFO
    /// to transform a mono bassline into a lush, wide stereo sound field.
    /// </summary>
    public class StereoChorus
    {
        private readonly int sampleRate;
        private readonly float[] delayBuffer;
        private int writeIndex;
        private double lfoPhase;

        public bool IsEnabled { get; set; } = true;
        public float WetMix { get; set; } = 0.5f;       // 50/50 dry/wet mix
        public double BaseDelayMs { get; set; } = 20.0; // 10 to 30 ms
        public double DepthMs { get; set; } = 6.0;      // Modulation depth in ms
        public double RateHz { get; set; } = 0.85;      // LFO modulation rate in Hz

        public StereoChorus(int sampleRate = 44100)
        {
            this.sampleRate = sampleRate;
            // 100 ms buffer size is more than enough for chorus
            int bufferSize = (int)(sampleRate * 0.1);
            delayBuffer = new float[bufferSize];
            writeIndex = 0;
            lfoPhase = 0.0;
        }

        public void Process(float monoInput, out float leftOutput, out float rightOutput)
        {
            if (!IsEnabled)
            {
                leftOutput = monoInput;
                rightOutput = monoInput;
                return;
            }

            // Write dry input into circular buffer
            delayBuffer[writeIndex] = monoInput;

            // Compute modulated delay times
            double lfoSin = Math.Sin(2.0 * Math.PI * lfoPhase);
            double lfoCos = Math.Cos(2.0 * Math.PI * lfoPhase); // 90-degree phase shift for stereo width

            double leftDelayMs = BaseDelayMs + DepthMs * lfoSin;
            double rightDelayMs = BaseDelayMs + DepthMs * lfoCos;

            double leftDelaySamples = leftDelayMs * 0.001 * sampleRate;
            double rightDelaySamples = rightDelayMs * 0.001 * sampleRate;

            float leftDelayed = ReadInterpolated(leftDelaySamples);
            float rightDelayed = ReadInterpolated(rightDelaySamples);

            // 50/50 Dry/Wet mix
            float dryMix = 1.0f - (WetMix * 0.5f);
            leftOutput = (monoInput * dryMix) + (leftDelayed * WetMix);
            rightOutput = (monoInput * dryMix) + (rightDelayed * WetMix);

            // Advance buffer write pointer
            writeIndex++;
            if (writeIndex >= delayBuffer.Length)
            {
                writeIndex = 0;
            }

            // Advance LFO
            lfoPhase += RateHz / sampleRate;
            if (lfoPhase >= 1.0)
            {
                lfoPhase -= 1.0;
            }
        }

        private float ReadInterpolated(double delayInSamples)
        {
            double readPosition = writeIndex - delayInSamples;
            while (readPosition < 0)
            {
                readPosition += delayBuffer.Length;
            }

            int indexA = (int)readPosition;
            int indexB = (indexA + 1) % delayBuffer.Length;
            double fraction = readPosition - indexA;

            return (float)((1.0 - fraction) * delayBuffer[indexA] + fraction * delayBuffer[indexB]);
        }
    }
}
