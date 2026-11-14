using System;

namespace audiosynth
{
    public class ADSR
    {
        public enum EnvelopeState { Attack, Decay, Sustain, Release, Idle }
        public EnvelopeState State { get; private set; } = EnvelopeState.Idle;

        private readonly int sampleRate;
        private float attackTime = 0.005f;
        private float decayTime = 0.2f;
        private float sustainLevel = 0.5f;
        private float releaseTime = 0.2f;

        private float attackRate;
        private float decayRate;
        private float releaseRate;

        private float currentValue;
        public float CurrentValue => currentValue;

        public float AttackTime
        {
            get => attackTime;
            set { attackTime = Math.Max(0.0005f, value); RecalculateRates(); }
        }

        public float DecayTime
        {
            get => decayTime;
            set { decayTime = Math.Max(0.001f, value); RecalculateRates(); }
        }

        public float SustainLevel
        {
            get => sustainLevel;
            set { sustainLevel = Math.Clamp(value, 0.0f, 1.0f); RecalculateRates(); }
        }

        public float ReleaseTime
        {
            get => releaseTime;
            set { releaseTime = Math.Max(0.001f, value); RecalculateRates(); }
        }

        public ADSR(int sampleRate, float attackTime = 0.005f, float decayTime = 0.2f, float sustainLevel = 0.5f, float releaseTime = 0.2f, bool startActive = true)
        {
            this.sampleRate = Math.Max(1, sampleRate);
            this.attackTime = attackTime;
            this.decayTime = decayTime;
            this.sustainLevel = sustainLevel;
            this.releaseTime = releaseTime;
            RecalculateRates();
            State = startActive ? EnvelopeState.Attack : EnvelopeState.Idle;
            currentValue = 0.0f;
        }

        private void RecalculateRates()
        {
            attackRate = 1.0f / (attackTime * sampleRate);
            decayRate = Math.Max(0.00001f, (1.0f - sustainLevel)) / (decayTime * sampleRate);
            releaseRate = 1.0f / (releaseTime * sampleRate);
        }

        public void Trigger()
        {
            State = EnvelopeState.Attack;
        }

        public void Release()
        {
            if (State != EnvelopeState.Idle)
            {
                State = EnvelopeState.Release;
            }
        }

        public void Reset()
        {
            State = EnvelopeState.Idle;
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
                    currentValue = sustainLevel;
                    break;

                case EnvelopeState.Release:
                    currentValue -= releaseRate * Math.Max(sustainLevel, 0.2f);
                    if (currentValue <= 0.0f)
                    {
                        currentValue = 0.0f;
                        State = EnvelopeState.Idle;
                    }
                    break;

                case EnvelopeState.Idle:
                    currentValue = 0.0f;
                    break;
            }

            return currentValue;
        }
    }
}
