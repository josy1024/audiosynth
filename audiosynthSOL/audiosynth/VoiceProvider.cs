using NAudio.Wave;
using System;
using System.Security.AccessControl;
namespace audiosynth
{
    public class VoiceProvider : ISampleProvider
    {
        private readonly double frequency;
        private double phase;
        private readonly ADSR adsr;

        public WaveFormat WaveFormat { get; }
        public WaveType Type { get; set; }
        public VoiceProvider(double freq, WaveType type)
        {
            frequency = freq;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
            Type = type; 
            adsr = new ADSR(WaveFormat.SampleRate);
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
