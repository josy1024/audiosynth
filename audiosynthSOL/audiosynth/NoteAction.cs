namespace audiosynth
{
    public enum NoteAction
    {
        NoteOn,
        NoteOff,
        UpdateNote,
        UpdateModulator
    }

    public class NoteCommand
    {
        public NoteAction Action { get; set; }
        public Keys KeyCode { get; set; }
        public float Frequency { get; set; }
        public WaveType WaveType { get; set; }
        public double ModulatorFrequency { get; set; }
    }
}
