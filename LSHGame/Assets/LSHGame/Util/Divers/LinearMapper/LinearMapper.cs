using SceneM;
using UnityEngine;

namespace LSHGame.Util
{
    public abstract class LinearMapper : StateMachineBehaviour
    {
        [SerializeField]
        protected string parameterName;

        [SerializeField]
        protected float StepsPerUnit = 1;

        [SerializeField]
        protected float offset = 0;

        [SerializeField]
        protected bool looping = true;

        protected abstract int ArrayLength { get; }

        protected virtual bool UpdateEveryFrame { get => false; } 

        protected Animator animator;
        private int parameterHash = -1;

        private float lastAbsIndex = float.NegativeInfinity;

        private void Awake()
        {
            parameterHash = Animator.StringToHash(parameterName);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            this.animator = animator;
            TimeSystem.OnLateUpdate += LateUpdate;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            TimeSystem.OnLateUpdate -= LateUpdate;
        }

        private void OnDisable()
        {
            TimeSystem.OnLateUpdate -= LateUpdate;
        }


        private void LateUpdate()
        {
            if (animator == null|| ArrayLength == 0)
                return;

            float v = animator.GetFloat(parameterHash);
            int index = (int)((v - offset) * StepsPerUnit);
            int absIndex = index;

            if (looping)
                index = mod(index, ArrayLength);
            else
            {
                index = Mathf.Clamp(index, 0, ArrayLength - 1);
                absIndex = index;
            }

            if (absIndex != lastAbsIndex || UpdateEveryFrame)
            {
                lastAbsIndex = absIndex;
                SetIndex(index);
            }

            //Debug.Log("UpdateSprite: " + index + " Value: " + v);
        }

        protected abstract void SetIndex(int index);

        int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
