using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UINavigation
{
    public class Panel : TransitionablePanel<string,Panel,PanelManager> 
    {
        public UnityEvent OnStartLeaving;
        public UnityEvent OnLeave;

        public UnityEvent OnStartEntering;
        public UnityEvent OnEnter;

        public List<IPanelTransition> panelTransitions = new List<IPanelTransition>();

        protected virtual void Awake()
        {
            foreach(var t in GetComponents<IPanelTransition>()){
                if (!panelTransitions.Contains(t))
                    panelTransitions.Add(t);
            }
        }

        protected override void Enter(Panel previousPanel)
        {
            foreach (var t in GetComponents<IPanelTransition>())
            {
                t.Enter(previousPanel);
            }
        }

        protected override void Leave(Panel nextPanel)
        {
            foreach (var t in GetComponents<IPanelTransition>())
            {
                t.Leave(nextPanel);
            }
        }

        protected override float StartEntering(Panel previousPanel)
        {
            float enterTime = 0;
            foreach (var t in GetComponents<IPanelTransition>())
            {
                enterTime = Mathf.Max(enterTime, t.StartEntering(previousPanel));
            }

            return enterTime;
        }

        protected override float StartLeaving(Panel nextPanel)
        {
            float exitTime = 0;
            foreach (var t in GetComponents<IPanelTransition>())
            {
                exitTime = Mathf.Max(exitTime, t.StartLeaving(nextPanel));
            }

            return exitTime;
        }
    }

    public interface IPanelTransition
    {
        /// <summary>
        /// Will be called when start leaving the panel
        /// </summary>
        /// <param name="nextPanel">The next panel which will be come visible. Can be null.</param>
        /// <returns>The time it will take to leave. The max of all IPanelTransition will be the transition time</returns>
        float StartLeaving(Panel nextPanel);

        /// <summary>
        /// Will be called when hiding the panel
        /// </summary>
        /// <param name="nextPanel">The next panel which will be come visible. Can be null.</param>
        void Leave(Panel nextPanel);

        /// <summary>
        /// Will be called when showing the panel
        /// </summary>
        /// <param name="previousPanel">The previous panel which was hidden. Can be null.</param>
        /// <returns>The time it will take to become completly visible. The max of all IPanelTransition will be the transition time</returns>
        float StartEntering(Panel previousPanel);

        /// <summary>
        /// Will be called when finished entering the panel.
        /// </summary>
        /// <param name="previousPanel">The previous panel which was hidden. Can be null.</param>
        void Enter(Panel previousPanel);
    }
} 
