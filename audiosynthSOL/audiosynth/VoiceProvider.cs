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
        
        private WaveType type;
        public WaveType Type
        {
            get { return type; }
            set { type = value; }
        }

        public float PulseWidth { get; set; } // Add this property

        public double ModulatorFrequency { get; set; }
        public double ModulatorFrequencyMultiplier { get; set; } = 1.0;
        public double modulationIndex = 1.7;
        private double modulatorPhase;


        private readonly Random random = new Random();
        public VoiceProvider(double freq, WaveType type, float pulseWidth = 0.5f)
        {
            this.frequency = freq; // assign to instance variable
            this.type = type;
            this.PulseWidth = pulseWidth;
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
                switch (this.type)
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
                    case WaveType.Pulse:
                        waveSample = (phase % 1.0 < PulseWidth) ? 1.0f : -1.0f;
                        break;
                    case WaveType.FM:
                        double modulatorValue = Math.Sin(2 * Math.PI * modulatorPhase);
                        double modulatedFrequency = frequency + (modulatorValue * modulationIndex * frequency);
                        
                        double modulatedPhaseIncrement = modulatedFrequency / WaveFormat.SampleRate;

                        // FM synthesis uses a sine wave as the carrier
                        waveSample = (float)Math.Sin(2 * Math.PI * phase);
                        phase += modulatedPhaseIncrement;
                        modulatorPhase += this.ModulatorFrequency / WaveFormat.SampleRate;
                        //modulatorPhase += (frequency * ModulatorFrequencyMultiplier) / WaveFormat.SampleRate;

                        break;
                    case WaveType.Noise:
                        waveSample = (float)(random.NextDouble() * 2.0 - 1.0); // Generate a random value between -1 and 1
                        break;
                }

                var sample = waveSample * adsr.GetNextSample();
                buffer[n + offset] = sample;

                phase += phaseIncrement;

                if (phase >= 1.0)
                {
                    phase -= 1.0;
                }
            }
            return sampleCount;
        }
    }
}
