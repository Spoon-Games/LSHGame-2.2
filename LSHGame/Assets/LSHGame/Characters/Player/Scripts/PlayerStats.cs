using System;
using System.Collections.Generic;
using LSHGame.Util;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [System.Serializable]
    public class PlayerStats : IPlayerLocomotionRec, IPlayerJumpRec, IPlayerClimbingRec, IPlayerDashRec, IGravityRec,
        IDataReciever, IDamageRec, IIsLadderRec, IIsLadderFeetRec, IMatVelocityRec, IMatBounceRec, IJumpCallbackRec, ISneekCallbackRec, IEffectsMaterialRec,
        ITriggerEffectRec, IPlayerIsJumpableSubProp, IDashRedirectorRec, IDashCenteredRedirectorRec, ILiliumReciever,IBlackLiliumReciever, ISaveGroundRec,
        IDashActivateRec
    {
        [Header("Locomotion")]
        [SerializeField] private AnimationCurve _runAccelCurve;
        [SerializeField] private AnimationCurve _runDeaccelCurve;
        [SerializeField] private AnimationCurve _runAccelAirborneCurve;
        [SerializeField] private AnimationCurve _runDeaccelAirborneCurve;
        [Header("Jump")]
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private float _jumpSpeedCutter;
        [Header("Climbing")]
        [SerializeField] private float _climbingLadderSpeed;
        [SerializeField] private float _climbingWallSlideSpeed;
        [SerializeField] private float _climbingWallSlowSlideSpeed;
        [SerializeField] private Vector2 _climbingWallJumpVelocity;
        [Header("Dash")]
        [SerializeField] private float _dashDurration;
        [SerializeField] private float _dashSpeed;
        [SerializeField] private float _dashRecoverDurration;
        [Header("Physics")]
        [SerializeField] private float _gravity;
        [SerializeField] private float _fallDamping;

        public AnimationCurve RunAccelCurve { get => _runAccelCurve; set => _runAccelCurve = value; }
        public AnimationCurve RunDeaccelCurve { get => _runDeaccelCurve; set => _runDeaccelCurve = value; }
        public AnimationCurve RunAccelAirborneCurve { get => _runAccelAirborneCurve; set => _runAccelAirborneCurve = value; }
        public AnimationCurve RunDeaccelAirborneCurve { get => _runDeaccelAirborneCurve; set => _runDeaccelAirborneCurve = value; }
        public float JumpSpeed { get => _jumpSpeed; set => _jumpSpeed = value; }
        public float JumpSpeedCutter { get => _jumpSpeedCutter; set => _jumpSpeedCutter = value; }
        public float ClimbingLadderSpeed { get => _climbingLadderSpeed; set => _climbingLadderSpeed = value; }
        public float ClimbingWallSlideSpeed { get => _climbingWallSlideSpeed; set => _climbingWallSlideSpeed = value; }
        public float ClimbingWallSlowSlideSpeed { get => _climbingWallSlowSlideSpeed; set => _climbingWallSlowSlideSpeed = value; }
        public Vector2 ClimbingWallJumpVelocity { get => _climbingWallJumpVelocity; set => _climbingWallJumpVelocity = value; }
        public float DashDurration { get => _dashDurration; set => _dashDurration = value; }
        public float DashSpeed { get => _dashSpeed; set => _dashSpeed = value; }
        public float DashRecoverDurration { get => _dashRecoverDurration; set => _dashRecoverDurration = value; }
        public float Gravity { get => _gravity; set => _gravity = value; }
        public float FallDamping { get => _fallDamping; set => _fallDamping = value; }
        public bool IsDamage { get; set; } = false;
        public bool IsLadder { get; set; } = false;
        public Vector2 MovingVelocity { get; set; } = Vector2.zero;
        public BounceSettings BounceSettings { get; set; } = null;
        public Action OnJump { get; set; }
        public Action OnBounce { get; set; }
        public Action OnSneek { get; set; }
        public Dictionary<string, string> EffectMaterials { get; } = new Dictionary<string, string>();
        public HashSet<string> TriggerEffects { get; } = new HashSet<string>();
        public bool IsFeetLadder { get; set; }
        public bool IsJumpableInAir { get; set; } = false;
        public float TargetDashAngle { get; set; }
        public float DashTurningRadius { get; set; } = -1;
        public Vector2 GlobalDashTurningCenter { get; set; } = Vector2.negativeInfinity;
        public float DashDeltaTurningAngle { get; set; }
        public LiliumSubProp LiliumReference { get; set; }
        public BlackLiliumSubProp BlackLiliumReference { get; set; }
        public bool IsSaveGround { get; set; } = true;
        public bool IsDashActive { get; set; }
        public Vector2 DashDirection { get; set; }
        public float EqualizeDashLengthWeight { get; set; }
        public Vector2 DashActivateCenterPos { get; set; }
        public float EqualizeDashDirectionWeight { get; set; }

        public PlayerStats Clone()
        {
            var stats = (PlayerStats)this.MemberwiseClone();
            stats.Reset();
            return stats;
        }

        private void Reset()
        {
            EffectMaterials.Clear();
            TriggerEffects.Clear();
            LiliumReference = null;
            BlackLiliumReference = null;
        }
    }
}
