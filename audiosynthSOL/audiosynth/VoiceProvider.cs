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
        public WaveType Type { get; }
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

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            if (adsr.State == ADSR.EnvelopeState.Idle) return 0;

            for (int n = 0; n < sampleCount; n++)
            {
                float waveSample = 0;
                switch (Type)
                {
                    case WaveType.Sine:
                        waveSample = (float)Math.Sin(phase * 2 * Math.PI);
                        break;
                    case WaveType.Saw:
                        waveSample = (float)(2 * (phase - Math.Floor(0.5 + phase)));
                        break;
                    case WaveType.Square:
                        //waveSample = phase < 0.5 ? 1.0f : -1.0f;
                        waveSample = Math.Sin(2 * Math.PI * phase) >= 0 ? 1.0f : -1.0f;
                        break;
                    case WaveType.Triangle:
                        waveSample = (float)(2 * Math.Abs(2 * (phase - Math.Floor(phase + 0.5))) - 1);
                        break;
                }

                var sample = waveSample * adsr.GetNextSample();
                buffer[n + offset] = sample;

                phase += frequency / WaveFormat.SampleRate;
            }
            return sampleCount;
        }
    }
}
