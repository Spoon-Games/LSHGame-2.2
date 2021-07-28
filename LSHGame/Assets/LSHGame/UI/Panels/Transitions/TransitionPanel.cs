using SceneM;
using System;
using UINavigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LSHGame.UI
{
    public abstract class TransitionPanel : BasePanel<TransitionInfo, TransitionPanel, TransitionManager>
    {

        public float Progress { get; private set; } = 0;

        public UnityEvent OnStartTransition;
        public UnityEvent OnMiddleTransition;
        public UnityEvent OnEndTransition;

        private float timeMark = float.PositiveInfinity;
        private Func<float> getProgress;

        public enum State { Idle, Start, Middle, End }
        private State currentState;
        public State CurrentState
        {
            get => currentState; private set
            {
                currentState = value;
                OnSwitchState(currentState);
            }
        }

        public void StartTransition(Func<float> getProgress)
        {


            this.getProgress = getProgress;

            CurrentState = State.Start;
            SetProgress(0);

            timeMark = PanelName.StartDurration + Time.time;
        }

        private void Update()
        {
            if (CurrentState != State.Idle)
            {
                SetProgress(GetProgress());

                if (Time.time >= timeMark && CurrentState == State.Start)
                {
                    CurrentState = State.Middle;
                    timeMark = Time.time + PanelName.MinMiddleDurration;
                }

                if (CurrentState == State.Middle && Time.time >= timeMark && GetProgress()>= 1)
                {
                    CurrentState = State.End;
                    timeMark = Time.time + PanelName.EndDurration;
                }

                if (CurrentState == State.End && Time.time >= timeMark)
                {
                    CurrentState = State.Idle;
                    Parent.ShowPanel(null);
                }
            }
        }

        private void SetProgress(float progress)
        {
            Progress = Mathf.Clamp01(progress);

            OnSetProgress(Progress);
        }

        private float GetProgress()
        {
            if (getProgress != null)
                return getProgress.Invoke();
            else return 1;
        }

        protected virtual void OnSwitchState(State state) {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Start:
                    OnStartTransition.Invoke();
                    break;
                case State.Middle:
                    OnMiddleTransition.Invoke();
                    break;
                case State.End:
                    OnEndTransition.Invoke();
                    break;
            }
        }

        protected virtual void OnSetProgress(float progress) { }

        //public void StartTransition(bool automatic = true)
        //{
        //    Animator = GetComponent<Animator>();
        //    Animator.SetTrigger("Start");

        //    if (!automatic)
        //    {
        //        Animator.SetBool("IsEndAfterDurration", false);
        //    }

        //    StartTrans();
        //}

        //public void EndTransition()
        //{
        //    Animator.SetBool("IsEndAfterDurration", true);
        //}

        //public void SetProgress(float progress)
        //{
        //    Progress = Mathf.Clamp01(progress);
        //    OnSetProgress(progress);
        //}

        //private void StartTrans() {
        //    InStart = true;
        //    OnStart();
        //}

        //protected virtual void OnStart() { }

        //internal void EnterMiddle()
        //{
        //    InStart = false;
        //    InMiddle = true;
        //    OnEnterMiddle();
        //}

        //protected virtual void OnEnterMiddle() {}

        //protected virtual void OnSetProgress(float progress)
        //{
        //    if (slider != null)
        //        slider.value = progress;
        //}

        //internal void ExitMiddle()
        //{
        //    InMiddle = false;
        //    OnExitMiddle();
        //}

        //protected virtual void OnExitMiddle() { }

        //internal void End()
        //{
        //    GetComponent<Panel>().Parent.ShowPanel("");
        //    OnEnd();
        //}

        //protected virtual void OnEnd() { }
    }
}
