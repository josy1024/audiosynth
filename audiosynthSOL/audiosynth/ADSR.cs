using System;

namespace audiosynth
{
    public class ADSR
    {
        public enum EnvelopeState { Attack, Decay, Sustain, Release, Idle }
        public EnvelopeState State { get; private set; }

        private readonly float attackRate;
        private readonly float decayRate;
        private readonly float releaseRate;
        private readonly float sustainLevel;

        private float currentValue;

        public ADSR(int sampleRate, float attackTime, float decayTime, float sustainLevel, float releaseTime)
        {
            attackRate = 1.0f / (attackTime * sampleRate);
            decayRate = (1.0f - sustainLevel) / (decayTime * sampleRate);
            releaseRate = sustainLevel / (releaseTime * sampleRate);
            this.sustainLevel = sustainLevel;
            State = EnvelopeState.Attack;
            currentValue = 0.0f;
        }

        public float GetNextSample()
        {
            switch (State)
            {
                case EnvelopeState.Attack:
                    currentValue += attackRate;
                    if (currentValue >= 1.0f)
                    {
                        currentValue = 1.0f;
                        State = EnvelopeState.Decay;
                    }
                    break;
                case EnvelopeState.Decay:
                    currentValue -= decayRate;
                    if (currentValue <= sustainLevel)
                    {
                        currentValue = sustainLevel;
                        State = EnvelopeState.Sustain;
                    }
                    break;
                case EnvelopeState.Sustain:
                    break;
                case EnvelopeState.Release:
                    currentValue -= releaseRate;
                    if (currentValue <= 0.0f)
                    {
                        currentValue = 0.0f;
                        State = EnvelopeState.Idle;
                    }
                    break;
            }
            return currentValue;
        }

        public void Release()
        {
            if (State != EnvelopeState.Idle)
            {
                State = EnvelopeState.Release;
            }
        }
    }
}