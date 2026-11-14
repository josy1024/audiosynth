namespace audiosynth
{
    public enum WaveType
    {
        Pulse,
        Saw,
        Square,
        Triangle,
        Sine,
        Noise,
        FM
    }

    /// <summary>
    /// Waveform type for the sub-oscillators in PolyBlepOscillator.
    /// CopyPrimary = copy of the primary waveform (Saw by default),
    /// Square      = classic 50% duty-cycle square,
    /// Sine        = pure sine tone.
    /// </summary>
    public enum SubOscWaveType
    {
        CopyPrimary = 0,
        Square      = 1,
        Sine        = 2
    }
}
