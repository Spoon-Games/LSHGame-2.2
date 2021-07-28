using UnityEngine;

namespace LSHGame.Util
{
    [System.Serializable]
    public class Timer : BaseTimer
    {
        public float durration = 1;

        public override bool Finished => ClockedTime + durration <= Time.fixedTime && !IsReset;

        public Timer(){}

        public Timer(float durration)
        {
            this.durration = durration;
        }
        
    }

    [System.Serializable]
    public class RandomTimer : BaseTimer
    {
        public float minDurration = 1;
        public float maxDurration = 2;
        private float randomDurration = 0;

        public override void Clock()
        {
            randomDurration = Random.Range(minDurration, maxDurration);
            base.Clock();
        }

        public override bool Finished => ClockedTime + randomDurration <= Time.fixedTime && !IsReset;
    }

    public abstract class BaseTimer
    {
        protected float _clockedTime = float.NegativeInfinity;
        public float ClockedTime => _clockedTime;

        public abstract bool Finished { get; }

        public virtual bool Active => !Finished && !IsReset;

        public virtual bool IsReset => _clockedTime == float.NegativeInfinity;

        public virtual void Clock()
        {
            _clockedTime = Time.fixedTime;
        }

        public virtual void Reset() => _clockedTime = float.NegativeInfinity;
    }
}
