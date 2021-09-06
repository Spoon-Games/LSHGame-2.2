using UnityEngine;

namespace UINavigation
{
    [RequireComponent(typeof(Animator))]
    public class AnimationPanelTransition : MonoBehaviour, IPanelTransition
    {
        public string startEnteringTrigger = "StartEntering";
        public string enterTrigger = "Enter";
        public string startLeavingTrigger = "StartLeaving";
        public string leaveTrigger = "Leave";

        public float enterTime = 0;
        public float leaveTime = 0;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Enter(Panel previousPanel)
        {
            animator.SetTrigger(enterTrigger);
        }

        public void Leave(Panel nextPanel)
        {
            animator.SetTrigger(leaveTrigger);
        }

        public float StartEntering(Panel previousPanel)
        {
            animator.SetTrigger(startEnteringTrigger);
            return enterTime;
        }

        public float StartLeaving(Panel nextPanel)
        {
            animator.SetTrigger(startLeavingTrigger);
            return leaveTime;
        }
    }
}
