using UnityEngine;

namespace LSHGame.PlayerN
{
    public abstract class BasePlayerCollider : MonoBehaviour
    {
        public bool IsTouchingClimbWallLeft { get; protected set; }
        public bool IsTouchingClimbWallRight { get; protected set; }

        public abstract Rigidbody2D Initialize(PlayerController controller, PlayerStateMachine stateMachine);

        public abstract void CheckUpdate();

        public abstract void ExeUpdate();

        public abstract void LateExeUpdate();

        public abstract void Reset();

        public abstract void SetToDeadBody();

        public abstract void SetPositionCorrected(Vector2 position);
    }
}
