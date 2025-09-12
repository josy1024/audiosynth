namespace audiosynth
{
    public enum NoteAction
    {
        NoteOn,
        NoteOff,
        UpdateNote,
        UpdateModulator,
        UpdateFmMultiplier, // New
        UpdateModulationIndex // New
    }

    public class NoteCommand
    {
        public NoteAction Action { get; set; }
        public Keys KeyCode { get; set; }
        public float Frequency { get; set; }
        public WaveType WaveType { get; set; }

        public double Value { get; set; } // 
        public double ModulatorFrequency { get; set; }
        public double ModulationIndex { get; set; }
        public double FmMultiplier { get; set; }

    }
}
