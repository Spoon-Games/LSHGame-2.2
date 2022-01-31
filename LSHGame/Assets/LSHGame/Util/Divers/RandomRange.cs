using UnityEngine;

namespace LSHGame.Util
{
    [System.Serializable]
    public struct RandomRange
    {
        public float min;
        public float max;

        private float _value;
        public float Value { get {

                if (_value < min || _value >= max)
                    Debug.LogWarning("You have not jet created a random value");

                return _value;
                } }

        public float NextValue()
        {
            _value = Random.Range(min, max);

            return _value;
        }
    }
}
