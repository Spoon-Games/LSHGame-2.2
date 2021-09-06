using UnityEngine;

namespace UINavigation
{
    public class MinTimePanelTranstion : MonoBehaviour, IPanelTransition
    {
        public float minLeaveTime;
        public float minEnterTime;

        public void Enter(Panel previousPanel)
        {
            
        }

        public void Leave(Panel nextPanel)
        {
           
        }

        public float StartEntering(Panel previousPanel)
        {
            return minEnterTime;
        }

        public float StartLeaving(Panel nextPanel)
        {
            return minLeaveTime;
        }
    }
}
