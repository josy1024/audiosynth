using NAudio.Wave;
using System;
using System.Security.AccessControl;
namespace audiosynth
{
    public class VoiceProvider : ISampleProvider
    {
        private double frequency;
        private double phase;
        private readonly ADSR adsr;

        public WaveFormat WaveFormat { get; }
        public WaveType Type { get; set; }
        public VoiceProvider(double freq, WaveType type)
        {
            this.frequency = freq; // assign to instance variable
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
            adsr = new ADSR(
                WaveFormat.SampleRate,
                attackTime: 0.01f,
                decayTime: 0.1f,
                sustainLevel: 0.7f,
                releaseTime: 0.5f
            );
        }

        public double Frequency
        {
            get { return frequency; }
            set { frequency = value; }
        }
        public void Stop()
        {
            adsr.Release();
        }

        // In VoiceProvider.cs

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            if (adsr.State == ADSR.EnvelopeState.Idle) return 0;

            double phaseIncrement = frequency / WaveFormat.SampleRate;

            for (int n = 0; n < sampleCount; n++)
            {
                float waveSample = 0;
                switch (Type)
                {
                    case WaveType.Sine:
                        waveSample = (float)Math.Sin(2 * Math.PI * phase);
                        break;
                    case WaveType.Saw:
                        waveSample = (float)(2 * (phase - Math.Floor(0.5 + phase)));
                        break;
                    case WaveType.Square:
                        waveSample = (phase % 1.0 < 0.5) ? 1.0f : -1.0f;
                        break;
                    case WaveType.Triangle:
                        double sawtooth = (phase % 1.0) * 2 - 1;
                        waveSample = (float)(2 * (Math.Abs(sawtooth) - 0.5));
                        break;
                }

                var sample = waveSample * adsr.GetNextSample();
                buffer[n + offset] = sample;

                phase += phaseIncrement;
            }
            return sampleCount;
        }
    }
}
