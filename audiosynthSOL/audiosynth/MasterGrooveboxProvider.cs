using System;
using NAudio.Wave;

namespace audiosynth
{
    /// <summary>
    /// Master stereo audio provider for the Groovebox:
    /// Coordinates the StepSequencer, MonoBassVoice, DrumMachine, and StereoChorus
    /// into a synchronized stereo output stream.
    /// </summary>
    public class MasterGrooveboxProvider : ISampleProvider
    {
        public WaveFormat WaveFormat { get; }

        public MonoBassVoice BassVoice { get; }
        public StepSequencer Sequencer { get; }
        public DrumMachine Drums { get; }
        public StereoChorus Chorus { get; }

        public float MasterVolume { get; set; } = 0.85f;

        private readonly float[] monoVoiceBuffer = new float[1];

        public MasterGrooveboxProvider(int sampleRate = 44100)
        {
            // Stereo output (2 channels)
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2);

            BassVoice = new MonoBassVoice(sampleRate);
            Sequencer = new StepSequencer(BassVoice, sampleRate);
            Drums = new DrumMachine(sampleRate) { IsEnabled = false };
            Chorus = new StereoChorus(sampleRate) { IsEnabled = false };

            // Connect sequencer step trigger to drum machine
            Sequencer.StepTriggered += stepIndex =>
            {
                Drums.TriggerStep(stepIndex);
            };
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int frames = count / 2; // 2 channels (interleaved Left/Right)

            for (int i = 0; i < frames; i++)
            {
                // 1. Advance sequencer clock by 1 sample frame
                Sequencer.AdvanceSample();

                // 2. Read 1 sample from MonoBassVoice
                BassVoice.Read(monoVoiceBuffer, 0, 1);
                float bassSample = monoVoiceBuffer[0];

                // 3. Process bass through Stereo Chorus
                Chorus.Process(bassSample, out float leftBass, out float rightBass);

                // 4. Read 1 sample from Drum Machine
                float drumSample = Drums.GetNextSample();

                // 5. Interleave stereo output
                float leftOut = (leftBass + drumSample) * MasterVolume;
                float rightOut = (rightBass + drumSample) * MasterVolume;

                // Soft limiter clamp for master safety
                buffer[offset + (i * 2)] = Math.Clamp(leftOut, -1.0f, 1.0f);
                buffer[offset + (i * 2) + 1] = Math.Clamp(rightOut, -1.0f, 1.0f);
            }

            return count;
        }
    }
}
