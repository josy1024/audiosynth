using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace audiosynth
{
    public class SynthEngine
    {
        private readonly MixingSampleProvider mixer;
        private readonly IWavePlayer waveOut;
        private readonly ConcurrentDictionary<Keys, VoiceProvider> activeVoices;

        public SynthEngine()
        {
            var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
            mixer = new MixingSampleProvider(waveFormat);
            mixer.ReadFully = true;

            waveOut = new WaveOutEvent();
            waveOut.Init(mixer);
            waveOut.Play();

            activeVoices = new ConcurrentDictionary<Keys, VoiceProvider>();
        }

        public void NoteOn(Keys key, float frequency, WaveType waveType)
        {
            if (!activeVoices.ContainsKey(key))
            {
                var newVoice = new VoiceProvider(frequency, waveType);
                mixer.AddMixerInput(newVoice);
                activeVoices.TryAdd(key, newVoice);
            }
        }

        public void NoteOff(Keys key)
        {
            if (activeVoices.TryRemove(key, out var voice))
            {
                voice.Stop();
            }
        }

        public void UpdateNoteFrequency(Keys key, float newFrequency)
        {
            if (activeVoices.TryGetValue(key, out var voice))
            {
                voice.Frequency = newFrequency;
            }
        }

        public void UpdateNoteWaveType(Keys key, WaveType newWaveType)
        {
            if (activeVoices.TryGetValue(key, out var voice))
            {
                voice.Type = newWaveType;
            }
        }
    }
}