using System.Collections;
using UnityEngine;

namespace UINavigation
{
    public enum PanelTransitionState { Hidden, Entering, Visible, Leaving }

    public abstract class TransitionablePanelManager<T, P, M> : BasePanelManager<T, P, M>
        where P : TransitionablePanel<T, P, M> where M : TransitionablePanelManager<T, P, M>
    {
        public PanelTransitionState ManagerState { get; protected set; } = PanelTransitionState.Hidden;

        public P RequestedPanel { get; protected set; }
        private P previousPanel;

        private float ExitTimer = 0;
        private float EnterTimer = 0;

        public override P ShowPanel(T panelName)
        {
            P nextPanel;

            if (panelName == null)
                nextPanel = null;
            else
            {
                panels.TryGetValue(panelName, out nextPanel);
            }

            RequestedPanel = nextPanel;

            return nextPanel;
        }

        protected virtual void Update()
        {
            if(ManagerState == PanelTransitionState.Hidden && CurrentPanel != RequestedPanel)
            {
                currentPanel = RequestedPanel;

                CurrentPanel.SetVisible(true);
                EnterTimer = CurrentPanel.InvokeStartEntering(previousPanel) + Time.time;
                ManagerState = PanelTransitionState.Entering;
            }

            if(ManagerState == PanelTransitionState.Entering)
            {
                if(Time.time >= EnterTimer)
                {
                    CurrentPanel.InvokeEnter(previousPanel);
                    ManagerState = PanelTransitionState.Visible;
                }
                else
                {
                    return;
                }
            }

            if (RequestedPanel != CurrentPanel)
            {
                if (ManagerState == PanelTransitionState.Visible)
                {
                    previousPanel = CurrentPanel;

                    ExitTimer = previousPanel.InvokeStartLeaving(RequestedPanel) + Time.time;
                    ManagerState = PanelTransitionState.Leaving;
                }

                if(ManagerState == PanelTransitionState.Leaving )
                {
                    if (Time.time >= ExitTimer)
                    {
                        CurrentPanel.InvokeLeave(RequestedPanel);
                        CurrentPanel.SetVisible(false);

                        currentPanel = null;
                        ManagerState = PanelTransitionState.Hidden;
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }

    public abstract class TransitionablePanel<T, P, M> : BasePanel<T, P, M>
        where P : TransitionablePanel<T, P, M> where M : TransitionablePanelManager<T, P, M>
    {
        public PanelTransitionState State { get; internal set; }

        internal float InvokeStartLeaving(P nextPanel)
        {
            State = PanelTransitionState.Leaving;
            return StartLeaving(nextPanel);
        }
        /// <summary>
        /// Will be called when start leaving the panel
        /// </summary>
        /// <param name="nextPanel">The next panel which will be come visible. Can be null.</param>
        /// <returns>The time it will take to leave.</returns>
        protected abstract float StartLeaving(P nextPanel);

        internal void InvokeLeave(P nextPanel)
        {
            State = PanelTransitionState.Hidden;
            Leave(nextPanel);
        }
        /// <summary>
        /// Will be called when hiding the panel
        /// </summary>
        /// <param name="nextPanel">The next panel which will be come visible. Can be null.</param>
        protected abstract void Leave(P nextPanel);

        internal float InvokeStartEntering(P previousPanel)
        {
            State = PanelTransitionState.Entering;
            return StartEntering(previousPanel);
        }
        /// <summary>
        /// Will be called when showing the panel
        /// </summary>
        /// <param name="previousPanel">The previous panel which was hidden. Can be null.</param>
        /// <returns>The time it will take to become completly visible.</returns>
        protected abstract float StartEntering(P previousPanel);

        internal void InvokeEnter(P previousPanel)
        {
            State = PanelTransitionState.Visible;
            Enter(previousPanel);
        }
        /// <summary>
        /// Will be called when finished entering the panel.
        /// </summary>
        /// <param name="previousPanel">The previous panel which was hidden. Can be null.</param>
        protected  abstract void Enter(P previousPanel);
    }
}
