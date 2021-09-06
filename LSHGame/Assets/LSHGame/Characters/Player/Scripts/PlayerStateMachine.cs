using UnityEngine;

namespace LSHGame.PlayerN
{
    public enum PlayerStates { Locomotion,Aireborne,ClimbWall,ClimbLadder,ClimbLadderTop,Dash,Death}

    public class PlayerStateMachine
    {
        public PlayerStates State { get; private set; }

        public delegate void OnPlayerStatesChangedEvent(PlayerStates from, PlayerStates to);
        public event OnPlayerStatesChangedEvent OnStateChanged;

        public Vector2 Velocity { get; set; }

        public Vector2 Position { get; set; }

        public bool IsGrounded { get; set; }

        public bool IsTouchingClimbWall { get; set; }

        public bool IsTouchingClimbLadder { get; set; }

        public bool IsFeetTouchingClimbLadder { get; set; }

        public bool IsDash { get; set; }

        public bool IsDead { get; set; }

        private PlayerLSM animatorMachine;
        private bool animatorStateChanged = false;

        public PlayerStateMachine(PlayerLSM animatorMachine)
        {
            this.animatorMachine = animatorMachine;

            UpdateState();
        }

        public void UpdateState()
        {
            PlayerStates newState = GetStateFromAny(State);

            if(newState != State)
            {
                PlayerStates oldState = State;

                State = newState;

                OnStateChanged?.Invoke(oldState, newState);
                animatorStateChanged = true;
            }
        }

        public void UpdateAnimator()
        {
            animatorMachine.VerticalSpeed = Velocity.y;
            animatorMachine.HorizontalSpeed = Velocity.x;

            animatorMachine.VerticalPosition = Position.y;

            animatorMachine.SAireborne = State == PlayerStates.Aireborne;
            animatorMachine.SClimbinLadder = State == PlayerStates.ClimbLadder;
            animatorMachine.SDash = State == PlayerStates.Dash;
            animatorMachine.SDeath = State == PlayerStates.Death;
            animatorMachine.SLocomotion = State == PlayerStates.Locomotion || State == PlayerStates.ClimbLadderTop;
            animatorMachine.SClimbingWall = State == PlayerStates.ClimbWall;

            if (animatorStateChanged)
            {
                animatorStateChanged = false;
                animatorMachine.StateChanged();
            }

        }

        internal void Reset()
        {
            Velocity = Vector2.zero;
            IsGrounded = false;
            IsTouchingClimbLadder = false;
            IsTouchingClimbWall = false;
            IsDash = false;
            IsDead = false;
        }

        private PlayerStates GetStateFromAny(PlayerStates oldState)
        {
            if (IsDead)
                return PlayerStates.Death;

            if (IsDash)
                return PlayerStates.Dash;

            if (IsTouchingClimbLadder)
                return PlayerStates.ClimbLadder;

            if (!IsGrounded && IsFeetTouchingClimbLadder)
                return PlayerStates.ClimbLadderTop;

            if (IsGrounded)
                return PlayerStates.Locomotion;

            if (IsTouchingClimbWall)
                return PlayerStates.ClimbWall;

            return PlayerStates.Aireborne;
        }

        public override string ToString()
        {
            return $"Player State Machine:\n" +
                $"State: {State}\n" +
                $"IsGrounded: {IsGrounded}\tIsDash: {IsDash}\tIsDead: {IsDead}\n" +
                $"IsTouchingClimbWall: {IsTouchingClimbWall}\tIsTouchingClimbLadder: {IsTouchingClimbLadder}\tIsFeetTouchingClimbLadder: {IsFeetTouchingClimbLadder}";
        }
    }
}