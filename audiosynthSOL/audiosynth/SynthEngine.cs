using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace audiosynth
{
    public class SynthEngine
    {
        private MixingSampleProvider mixer;
        private IWavePlayer waveOut;
        private readonly ConcurrentDictionary<Keys, VoiceProvider> activeVoices;

        private readonly ConcurrentBag<VoiceProvider> voicesInRelease = new ConcurrentBag<VoiceProvider>();

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

        public void StopAndDispose()
        {
            // Stop all currently playing notes
            foreach (var voice in activeVoices.Values)
            {
                voice.Stop();
            }
            activeVoices.Clear();

            // Dispose of the mixer input and the WaveOut device
            mixer.RemoveAllMixerInputs();
            waveOut.Stop();
            waveOut.Dispose();
        }

        public void Reset()
        {
            StopAndDispose();

            // Re-initialize the audio engine
            var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
            mixer = new MixingSampleProvider(waveFormat);
            mixer.ReadFully = true;

            waveOut = new WaveOutEvent();
            waveOut.Init(mixer);
            waveOut.Play();
        }
        public void NoteOn(Keys key, float frequency, WaveType waveType, double modulationIndex, double fmMultiplier)
        {  // Ensure any existing note for this key is completely stopped
            NoteOff(key);

            // Create and add the new voice
            var newVoice = new VoiceProvider(frequency, waveType)
            {
                ModulationIndex = modulationIndex,
                ModulatorFrequencyMultiplier = fmMultiplier
            };
            mixer.AddMixerInput(newVoice);
            activeVoices.TryAdd(key, newVoice);
        }

        public void NoteOff(Keys key)
        {
            if (activeVoices.TryRemove(key, out var voice))
            {
                voice.Stop(); // Sets ADSR to Release state
                voicesInRelease.Add(voice);
            }
        }

        public void UpdateNote(Keys key, float newFrequency, WaveType newWaveType)
        {
            if (activeVoices.TryGetValue(key, out var voice))
            {
                voice.Frequency = newFrequency;
                voice.Type = newWaveType;
            }
        }

        public float GetNoteFrequency(Keys key)
        {
            if (activeVoices.TryGetValue(key, out var voice))
            {
                return (float)voice.Frequency;
            }
            return 0f;
        }

        public void UpdateModulatorFrequency(Keys key, double newModFreq)
        {
            if (activeVoices.TryGetValue(key, out var voice))
            {
                voice.ModulatorFrequency = newModFreq;
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

        public void UpdateAllVoicesFmMultiplier(double multiplier)
        {
            foreach (var voice in activeVoices.Values)
            {
                voice.ModulatorFrequencyMultiplier = multiplier;
            }
        }

        public void UpdateAllVoicesModulationIndex(double index)
        {
            foreach (var voice in activeVoices.Values)
            {
                voice.ModulationIndex = index;
            }
        }

        public void RemoveVoice(VoiceProvider voice)
        {
            mixer.RemoveMixerInput(voice);
            voice.Dispose(); // Best practice
        }

        // Inside SynthEngine.cs
        public void CleanupReleasedVoices()
        {
            var finishedVoices = new List<VoiceProvider>();
            foreach (var voice in voicesInRelease)
            {
                if (voice.IsFinished()) // You will need to add this IsFinished method to VoiceProvider
                {
                    finishedVoices.Add(voice);
                }
            }
            foreach (var voice in finishedVoices)
            {
                if (voicesInRelease.TryTake(out _)) // Attempt to remove from the bag
                {
                    // This voice is finished and should be removed from the mixer
                    mixer.RemoveMixerInput(voice);
                }
            }
        }
    }
}