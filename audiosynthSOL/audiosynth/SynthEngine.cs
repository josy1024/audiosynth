using NAudio.Wave;
using System.Windows.Forms;

namespace audiosynth
{
    /// <summary>
    /// Master Audio Engine orchestrating the Groovebox sound engine:
    /// MonoBassVoice, StepSequencer, DrumMachine, and StereoChorus,
    /// feeding into NAudio WaveOut, WaveformViewer oscilloscope, and SpectrumViewer FFT bars.
    /// </summary>
    public class SynthEngine
    {
        private IWavePlayer waveOut;
        private readonly WaveformViewer viewer;
        private readonly SpectrumViewer? spectrumViewer;

        public MasterGrooveboxProvider Groovebox { get; private set; }
        public MonoBassVoice BassVoice => Groovebox.BassVoice;
        public StepSequencer Sequencer => Groovebox.Sequencer;
        public DrumMachine Drums => Groovebox.Drums;
        public StereoChorus Chorus => Groovebox.Chorus;

        public SynthEngine(WaveformViewer viewer, SpectrumViewer? spectrumViewer = null)
        {
            this.viewer = viewer;
            this.spectrumViewer = spectrumViewer;
            Groovebox = new MasterGrooveboxProvider(44100);

            waveOut = new WaveOutEvent();
            waveOut.Init(new VisualizationSampleProvider(Groovebox, viewer, spectrumViewer));
            waveOut.Play();
        }

        public void StopAndDispose()
        {
            Groovebox.Sequencer.Stop();
            Groovebox.BassVoice.Stop();

            waveOut.Stop();
            waveOut.Dispose();
        }

        public void Reset()
        {
            StopAndDispose();

            Groovebox = new MasterGrooveboxProvider(44100);
            waveOut = new WaveOutEvent();
            waveOut.Init(new VisualizationSampleProvider(Groovebox, viewer, spectrumViewer));
            waveOut.Play();
        }

        public void NoteOn(Keys key, float frequency, WaveType waveType = WaveType.Saw, double modulationIndex = 1.0, double fmMultiplier = 1.0)
        {
            Groovebox.BassVoice.Waveform = waveType;
            Groovebox.BassVoice.NoteOn(frequency);
        }

        public void NoteOff(Keys key)
        {
            Groovebox.BassVoice.NoteOff();
        }

        public void UpdateNote(Keys key, float newFrequency, WaveType newWaveType)
        {
            Groovebox.BassVoice.Frequency = newFrequency;
            Groovebox.BassVoice.Waveform = newWaveType;
        }

        public float GetNoteFrequency(Keys key)
        {
            return (float)Groovebox.BassVoice.Frequency;
        }

        public void UpdateNoteFrequency(Keys key, float newFrequency)
        {
            Groovebox.BassVoice.Frequency = newFrequency;
        }

        public void UpdateNoteWaveType(Keys key, WaveType newWaveType)
        {
            Groovebox.BassVoice.Waveform = newWaveType;
        }

        public void UpdateModulatorFrequency(Keys key, double newModFreq)
        {
            Groovebox.BassVoice.BaseCutoff = newModFreq;
        }

        public void UpdateAllVoicesFmMultiplier(double multiplier)
        {
            Groovebox.BassVoice.Resonance = multiplier * 2.0;
        }

        public void UpdateAllVoicesModulationIndex(double index)
        {
            Groovebox.BassVoice.FilterEnvAmount = index;
        }

        public void CleanupReleasedVoices()
        {
        }
    }
}
