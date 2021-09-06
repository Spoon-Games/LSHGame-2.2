using LSHGame.UI;
using LSHGame.Util;
using SceneM;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(PlayerLSM))]
    public class PlayerController : MonoBehaviour
    {
        #region Attributes

        [Header("Default Stats")]
        [SerializeField]
        internal PlayerStats defaultStats = new PlayerStats();

        internal PlayerStats Stats { get; private set; }
        internal SubstanceSet SubstanceSet { get; private set; }

        [SerializeField]
        private float liliumDeathDurration = 30;

        [SerializeField]
        private TransitionInfo deathTransition;

        [Header("Experimental")]
        [SerializeField]
        private bool dashWithoutLilium = false;

        [SerializeField]
        private bool isInvincible = false;

        [SerializeField]
        private bool isCroushing = false;

        [SerializeField]
        private bool isClimbExhaust = false;

        [SerializeField]
        private bool isRespawnOnStart = true;

        [SerializeField]
        private bool isOverrideMovingVelocity = false;

        [SerializeField]
        private Vector2 overrideMovingVelocity = Vector2.zero;

        [SerializeField]
        private bool overrideGoRight;


        [Header("Input")]
        [SerializeField]
        private GlobalInputAgent inputAgent;
        [SerializeField]
        private InputButton jumpInput;

        [SerializeField]
        private float wallClimbInputExtendDurration = 0.2f;

        private Timer wallClimbInputExtendLeftTimer;
        private Timer wallClimbInputExtendRightTimer;

        [Header("References")]
        [SerializeField]
        private BasePlayerCollider playerCollider;
        [SerializeField]
        private LiliumEffect liliumEffect;
        [SerializeField]
        private CameraController cameraController;

        private Rigidbody2D rb;
        private EffectsController effectsController;
        private PlayerStateMachine stateMachine;

        private Player parent;

        //Dash
        private float dashStartDisableTimer = float.NegativeInfinity;
        private Vector2 dashVelocity;
        private Vector2 estimatedDashPosition;
        private float dashEndTimer = 0;
        private Vector2 lastDashTurningCenter = Vector2.negativeInfinity;
        private float dashTurningTargetAngle;
        //Lilium
        public int liliumState = 0;
        private float liliumStartTimer = 0;


        //Climbing
        internal float lastJumpTimer = float.NegativeInfinity;

        private Vector2 localScale;

        internal Vector2 inputMovement;
        private Vector2 lastInputMovement;

        internal Vector2 lastFrameMovingVelocity = default;
        internal Vector2 lastFramePosition = default;

        internal Vector2 localVelocity = Vector2.zero;
        internal float localGravity = 0;

        internal Vector2 flipedDirection = Vector2.zero;
        private bool isYFliped => flipedDirection.y == -1;
        private bool isXFliped => flipedDirection.x == -1;

        private bool IsTouchingClimbWallLeftAbs => (!isXFliped && playerCollider.IsTouchingClimbWallLeft ||
            isXFliped && playerCollider.IsTouchingClimbWallRight);
        private bool IsTouchingClimbWallRightAbs => (!isXFliped && playerCollider.IsTouchingClimbWallRight ||
            isXFliped && playerCollider.IsTouchingClimbWallLeft);

        [SerializeField]
        private Timer releaseFromClimbWallTimer = new Timer(0.2f);

        private bool isJumpSpeedCutterActivated = false;

        public bool IsSaveGround => Stats.IsSaveGround;

        #endregion

        #region Initialization
        private void Awake()
        {
            Stats = defaultStats.Clone();
            SubstanceSet = new SubstanceSet();

            effectsController = GetComponent<EffectsController>();

            stateMachine = new PlayerStateMachine(GetComponent<PlayerLSM>());
            stateMachine.OnStateChanged += OnPlayerStateChanged;

            rb = playerCollider.Initialize(this, stateMachine);

            localScale = transform.localScale;

            wallClimbInputExtendRightTimer = new Timer(wallClimbInputExtendDurration);
            wallClimbInputExtendLeftTimer = new Timer(wallClimbInputExtendDurration);

            inputAgent.Listen();
            //Debug.Log("PlayerController IsGroundedHash: " + PlayerLSM.IsGroundedHash + " AnimHash " + Animator.StringToHash("IsGrounded"));
            //GetComponent<Animator>().SetBool(Animator.StringToHash("IsGrounded"), true);
            //inputMaster.Enable();
        }

        private void Start()
        {
            Spawn();
        }

        internal void Initialize(Player parent)
        {
            this.parent = parent;
        }
        #endregion

        #region Update Loop
        private void FixedUpdate()
        {
            //Get Stats
            flipedDirection = transform.localScale;

            Vector2 rbVelocity = ((Vector2)transform.position - lastFramePosition) / Time.fixedDeltaTime;
            lastFramePosition = transform.position;

            lastFrameMovingVelocity = Stats.MovingVelocity;
            localVelocity = rbVelocity - lastFrameMovingVelocity;

            Stats = defaultStats.Clone();

            lastInputMovement = inputMovement;
            inputMovement = inputAgent.Move.Value;
            if (overrideGoRight)
                inputMovement.x = 1;


            //Get Substances
            playerCollider.CheckUpdate();

            //Check
            CheckLilium();
            CheckClimbWall();
            CheckDash();
            CheckPlayerEnabled();
            CheckGravity();
            CheckSaveGround();
            AsignEffectMaterials();

            stateMachine.UpdateState();
            //Debug.Log(stateMachine.ToString());
            //Exe Update
            ExeUpdate();

            playerCollider.ExeUpdate();

            Jump();
            FlipSprite();
            ExeLilium();
            //Update Animator
            stateMachine.Velocity = localVelocity;
            stateMachine.Position = transform.position;
            stateMachine.UpdateAnimator();

            //Late ExeUpdate
            playerCollider.LateExeUpdate();
            if (isOverrideMovingVelocity)
                Stats.MovingVelocity = overrideMovingVelocity;

            //Set Ridgidbody
            rb.velocity = localVelocity + Stats.MovingVelocity;
            rb.gravityScale = localGravity;
            //Debug.Log("State: " + stateMachine.State + " MovingVelocity: " + Stats.MovingVelocity.x + " Local: " + localVelocity.x +
            //    "\nRB: " + rb.velocity.x + " previous RB: " + rbVelocity.x + " previous MV: " + lastFrameMovingVelocity.x);

            //Debug.Log("State: " + stateMachine.State + " MovingVelocity: " + Stats.MovingVelocity.y + " Local: " + localVelocity.y +
            //    "\nRB: " + rb.velocity.y + " previous RB: " + rbVelocity.y + " previous MV: " + lastFrameMovingVelocity);

            //rb.MovePosition(rb.position + (localVelocity + Stats.MovingVelocity) * Time.fixedDeltaTime);
            //rb.AddForce(Stats.MovingVelocity, ForceMode2D.Impulse);

        }
        #endregion

        #region Check Methods
        private void CheckLilium()
        {
            //Debug.Log("liliumState: " + liliumState);

            if (Stats.LiliumReference != null && liliumState < 1)
            {
                if (Stats.LiliumReference.GetLilium())
                {
                    if (liliumState <= 0)
                        liliumStartTimer = Time.fixedTime;

                    liliumState++;
                    liliumEffect.Collect(Stats.LiliumReference);
                }
            }

            if (Stats.BlackLiliumReference != null && liliumState > 0)
            {
                if (Stats.BlackLiliumReference.DeliverLilium())
                {
                    liliumState--;
                    liliumEffect.Deliver(Stats.BlackLiliumReference);
                }
            }

            if (liliumState > 0 && Time.fixedTime >= liliumStartTimer + liliumDeathDurration)
            {
                Kill();
            }

            GameInput.Hint_IsLilium = liliumState > 0;
        }

        private void CheckClimbWall()
        {
            if (inputMovement.y > 0 || lastInputMovement.y > 0)
                stateMachine.IsTouchingClimbLadder &= localVelocity.y <= Stats.ClimbingLadderSpeed + 0.1f;
            else
                stateMachine.IsTouchingClimbLadder &= localVelocity.y <= 0.2f;

            stateMachine.IsTouchingClimbLadder &= !(inputMovement.y <= 0 && stateMachine.IsGrounded);

            stateMachine.IsFeetTouchingClimbLadder &= localVelocity.y <= 0 + 0.1f;


            //Climb wall
            bool isTouchingClimbWall = playerCollider.IsTouchingClimbWallLeft || playerCollider.IsTouchingClimbWallRight;
            //stateMachine.IsTouchingClimbWall = wallClimbInput.Check(GameInput.IsWallClimbHold, isTouchingClimbWall);

            if (inputMovement.x > 0)
                wallClimbInputExtendRightTimer.Clock();
            if (inputMovement.x < 0)
                wallClimbInputExtendLeftTimer.Clock();

            if (!stateMachine.IsTouchingClimbWall && isTouchingClimbWall)
            {
                stateMachine.IsTouchingClimbWall = IsTouchingClimbWallLeftAbs && ((wallClimbInputExtendLeftTimer.Active || localVelocity.x < -0.05f));
                stateMachine.IsTouchingClimbWall |= IsTouchingClimbWallRightAbs && (wallClimbInputExtendRightTimer.Active || localVelocity.x > 0.05f);
            }

            stateMachine.IsTouchingClimbWall &= isTouchingClimbWall;
            stateMachine.IsTouchingClimbWall &= Time.fixedTime > lastJumpTimer + 0.2f;




            //Debug.Log("CheckClimbWall: " + stateMachine.IsTouchingClimbWall + " isPress: " + inputController.Player.WallClimbHold.GetBC().isPressed);

        }


        private void CheckDash()
        {
            Stats.IsDashActive &= liliumState > 0 || dashWithoutLilium;

            if (jumpInput.Check(inputAgent.Jump.IsFired,
                stateMachine.State != PlayerStates.Dash
                && dashStartDisableTimer + Stats.DashRecoverDurration <= Time.fixedTime 
                && Stats.IsDashActive, conIndex: 2)
                && Stats.IsDashActive) // make sure that dash is only activated while active
            {
                stateMachine.IsDash = true;
            }

            if (stateMachine.State == PlayerStates.Dash)
            {

                //stateMachine.IsDash &= !GameInput.WasDashRealeased;
                stateMachine.IsDash &= localVelocity.Approximately(dashVelocity, 0.5f) && estimatedDashPosition.Approximately(rb.transform.position, 0.5f);
                stateMachine.IsDash &= Time.fixedTime < dashEndTimer;

                //stateMachine.IsDash &= liliumState > 0; //Check for lilium
            }
        }

        private void CheckPlayerEnabled()
        {
            stateMachine.IsTouchingClimbWall &= parent.IsWallClimbEnabled;
            stateMachine.IsDash &= parent.IsDashEnabled;
        }

        private void CheckGravity()
        {
            if (GreaterYAbs(localVelocity.y, 0) && isJumpSpeedCutterActivated)
            {
                Stats.Gravity /= Stats.JumpSpeedCutter;
            }
            else
            {
                isJumpSpeedCutterActivated = false;
            }
        }

        private void CheckSaveGround()
        {
            Stats.IsSaveGround &= !Stats.IsDamage;
            Stats.IsSaveGround &= stateMachine.State != PlayerStates.Death;
            Stats.IsSaveGround &= stateMachine.State != PlayerStates.Dash;
            Stats.IsSaveGround &= stateMachine.State != PlayerStates.ClimbWall;
            Stats.IsSaveGround &= stateMachine.State != PlayerStates.ClimbLadder;
            Stats.IsSaveGround &= stateMachine.State != PlayerStates.ClimbLadderTop;
            Stats.IsSaveGround &= stateMachine.State != PlayerStates.Aireborne;

        }

        private void AsignEffectMaterials()
        {
            effectsController.SetAllMaterialsToDefault();
            foreach (var em in Stats.EffectMaterials)
            {
                effectsController.SetMaterial(em.Key, em.Value);
            }
        }
        #endregion

        #region State Changed
        private void OnPlayerStateChanged(PlayerStates from, PlayerStates to)
        {
            //Debug.Log("StateChanged To: " + to);
            if (from != PlayerStates.Dash && to == PlayerStates.Dash)
            {

                dashStartDisableTimer = Time.fixedTime;

                dashEndTimer = Time.fixedTime + Stats.DashDurration;
                if (!GetSign(inputMovement.x, out float sign))
                    sign = flipedDirection.x;
                if (from == PlayerStates.ClimbWall)
                    sign = playerCollider.IsTouchingClimbWallLeft ^ isXFliped ? 1 : -1;

                Vector2 direction = Stats.DashDirection.normalized;//verticalDashSpeed > 0 ? inputMovement.normalized : new Vector2(sign, 0);
                float speed = Stats.DashSpeed;
                Vector2 dashCenterOffset = (Vector2)transform.position - Stats.DashActivateCenterPos;

                Vector2 directionToTargetPos = (direction * Stats.DashSpeed * Stats.DashDurration - dashCenterOffset).normalized;

                Quaternion dirq = Quaternion.Lerp(Quaternion.LookRotation(Vector3.forward, direction), Quaternion.LookRotation(Vector3.forward, directionToTargetPos)
                    , Stats.EqualizeDashDirectionWeight);
                direction = dirq * Vector2.up;

                float inDirectionLengthDiff = Vector2.Dot(dashCenterOffset, direction);
                speed -= inDirectionLengthDiff / Stats.DashDurration * Mathf.Clamp01(Stats.EqualizeDashLengthWeight);



                dashVelocity = direction * speed;//new Vector2(direction.x * Stats.DashSpeed, direction.y * verticalDashSpeed);
                estimatedDashPosition = rb.transform.position;
                localVelocity = dashVelocity;

                liliumEffect.Dash();
            }

            if (to == PlayerStates.ClimbWall)
            {
                GameInput.Hint_WallClimb?.Invoke();

                effectsController.TriggerEffect("WallSlide");
                effectsController.SetMaterial("Jump", "WallJump");
            }
            else if (from == PlayerStates.ClimbWall)
            {
                effectsController.StopEffect("WallSlide");
                effectsController.SetMaterial("Jump", "Default");
            }

            if (from == PlayerStates.Dash)
            {
                lastDashTurningCenter = Vector2.negativeInfinity;
            }

            if (to == PlayerStates.Death)
            {
                Respawn();
                playerCollider.SetToDeadBody();
                cameraController.SetFallingDead(true);
            }

        }
        #endregion

        #region Exe Methods
        private void ExeUpdate()
        {
            switch (stateMachine.State)
            {
                case PlayerStates.Locomotion:

                    Run(isAirborne: false);
                    ExeSneek();
                    localGravity = Stats.Gravity;
                    break;
                case PlayerStates.Aireborne:

                    Run(true);
                    localGravity = Stats.Gravity;

                    if (SmalerY(localVelocity.y, 0))
                        localVelocity.y *= Stats.FallDamping;
                    break;
                case PlayerStates.ClimbWall:

                    localGravity = 0;
                    SetClimbWallSpeedX();

                    if (inputMovement.y > 0)
                        localVelocity.y = -Stats.ClimbingWallSlowSlideSpeed;
                    else
                        localVelocity.y = -Stats.ClimbingWallSlideSpeed;

                    break;
                case PlayerStates.ClimbLadder:

                    Run(false);
                    localGravity = 0;
                    localVelocity.y = inputMovement.y * Stats.ClimbingLadderSpeed;

                    break;
                case PlayerStates.ClimbLadderTop:
                    Run(false);
                    localGravity = 0;
                    //Only move downwards vertically
                    localVelocity.y = Mathf.Min(0, inputMovement.y * Stats.ClimbingLadderSpeed);
                    break;
                case PlayerStates.Dash:
                    Dash();
                    localGravity = 0;
                    break;
                case PlayerStates.Death:
                    localVelocity.x = 0;
                    //localGravity = 0;
                    break;
            }
        }

        private void Run(bool isAirborne = false, bool isCrouching = false)
        {
            if (Mathf.Abs(inputMovement.x) > 0.01f)
                GameInput.Hint_Movement?.Invoke();

            if (Mathf.Abs(inputMovement.y) > 0.01f && stateMachine.State == PlayerStates.ClimbLadder)
                GameInput.Hint_LadderClimb?.Invoke();

            float horVelocityRel = localVelocity.x;
            AnimationCurve accelCurve = Stats.RunAccelCurve;
            AnimationCurve deaccelCurve = Stats.RunDeaccelCurve;

            if (isAirborne)
            {
                accelCurve = Stats.RunAccelAirborneCurve;
                deaccelCurve = Stats.RunDeaccelAirborneCurve;
            }

            if (Mathf.Abs(inputMovement.x) < 0.01f)
            {
                horVelocityRel = deaccelCurve.EvaluateValueByStep(Mathf.Abs(horVelocityRel), Time.fixedDeltaTime, true) * Mathf.Sign(horVelocityRel);
            }
            else
            {
                horVelocityRel = accelCurve.EvaluateValueByStep(horVelocityRel * Mathf.Sign(inputMovement.x), Time.fixedDeltaTime) * Mathf.Sign(inputMovement.x);
            }

            //Debug.Log("MovingPlatformVel: " + playerColliders.movingPlatformVelocity);

            //Debug
            localVelocity.x = horVelocityRel;
            //Debug.Log("MovingPlatformVel: " + playerColliders.movingPlatformVelocity);
        }

        private void Jump()
        {
            if (Stats.IsDashActive)
                return;

            bool buttonReleased = false;

            bool isJumpCondition = stateMachine.State == PlayerStates.Locomotion || stateMachine.State == PlayerStates.ClimbLadder || stateMachine.State == PlayerStates.ClimbLadderTop || (stateMachine.State == PlayerStates.Aireborne && Stats.IsJumpableInAir);
            //Debug.Log($"IsJumpCondition: {isJumpCondition}\tState: {stateMachine.State}\tJumpableInAir: {Stats.IsJumpableInAir}");
            if (GreaterYAbs(Stats.JumpSpeed, localVelocity.y) &&
                jumpInput.Check(inputAgent.Jump.IsFired, isJumpCondition, ref buttonReleased))
            {
                localVelocity.y = Mathf.Max(Stats.JumpSpeed * flipedDirection.y, localVelocity.y);
                lastJumpTimer = Time.fixedTime + 0.2f;

                Stats.OnJump?.Invoke();
                effectsController.TriggerEffect("Jump");
                //rb.AddForce(new Vector2(0, jumpSpeed),ForceMode2D.Impulse);
            }
            else if (jumpInput.Check(inputAgent.Jump.IsFired, stateMachine.State == PlayerStates.ClimbWall, ref buttonReleased, 1))
            {
                float xfactor = 1;
                if (IsTouchingClimbWallLeftAbs)
                    xfactor = inputMovement.x > 0 ? 1 : 0.8f;
                else
                    xfactor = inputMovement.x < 0 ? -1 : -0.8f;
                localVelocity = Stats.ClimbingWallJumpVelocity * new Vector2(xfactor, flipedDirection.y);
                lastJumpTimer = Time.fixedTime;
                effectsController.TriggerEffect("Jump", "WallJump");
            }

            if (buttonReleased && GreaterYAbs(localVelocity.y, 0.05f) && GreaterYAbs(Stats.JumpSpeed, localVelocity.y))
            {
                isJumpSpeedCutterActivated = true;
                lastJumpTimer = float.NegativeInfinity;
            }
        }

        private float JumpXVelClimbWallDir()
        {
            return isXFliped ^ playerCollider.IsTouchingClimbWallLeft ? 1 : -1;
        }

        private void ExeSneek()
        {
            if (inputMovement.y < 0)
            {
                Stats.OnSneek?.Invoke();
            }
        }

        private void Dash()
        {

            //var currentRotation = Quaternion.FromToRotation(Vector3.right, localVelocity);
            //if (Stats.GlobalDashTurningCenter != Vector2.negativeInfinity)
            //{
            //    Vector2 radius = (Vector2)transform.position - Stats.GlobalDashTurningCenter;
            //    if (Stats.GlobalDashTurningCenter != lastDashTurningCenter)
            //    {
            //        lastDashTurningCenter = Stats.GlobalDashTurningCenter;
            //        dashTurningTargetAngle = Stats.DashDeltaTurningAngle * Mathf.Sign(Vector2.SignedAngle(radius,localVelocity))  + currentRotation.eulerAngles.z;
            //        Debug.Log("Radius: " + radius + " Velocity: " + localVelocity + " AngleSign: " + Mathf.Sign(Vector2.SignedAngle(radius, localVelocity)) +
            //            nTurningCenter: "+Stats.GlobalDashTurningCenter);
            //    }

            //    RotateDash(currentRotation, Quaternion.Euler(0, 0, dashTurningTargetAngle), radius.magnitude);
            //}
            //else if(Stats.DashTurningRadius >= 0)
            //{
            //    var targetRotation = Quaternion.Euler(0, 0, Stats.TargetDashAngle);
            //    RotateDash(currentRotation, targetRotation, Stats.DashTurningRadius);
            //}
            estimatedDashPosition = ((Vector2)rb.transform.position) + (dashVelocity * Time.fixedDeltaTime);
        }

        private void RotateDash(Quaternion currentRotation, Quaternion targetRotation, float radius)
        {
            if (radius > 0)
            {
                float deltaAngle = localVelocity.magnitude / radius * Time.fixedDeltaTime * Mathf.Rad2Deg;
                dashVelocity = Quaternion.RotateTowards(currentRotation, targetRotation, deltaAngle) * Vector2.right * Stats.DashSpeed;
            }
            else
                dashVelocity = targetRotation * Vector2.right * Stats.DashSpeed;
            localVelocity = dashVelocity;
        }

        private void ExeLilium()
        {
            liliumEffect.UpdateState(liliumState, (Time.fixedTime - liliumStartTimer) / liliumDeathDurration);
        }
        #endregion

        #region Helper Methods

        private void FlipSprite()
        {

            if (GetSign(localGravity, out float ysign))
                flipedDirection.y = ysign;

            if ((stateMachine.State == PlayerStates.ClimbWall))
            {
                if (GetSign(inputMovement.x, out float sign2))
                    flipedDirection.x = sign2;
            }
            else if (GetSign(localVelocity.x, out float sign) || GetSign(inputMovement.x, out sign))
            {
                flipedDirection.x = sign;
            }

            transform.localScale = flipedDirection;
        }

        private bool GetSign(float v, out float sign)
        {
            return GetSign(v, out sign, Mathf.Epsilon);
        }

        private bool GetSign(float v, out float sign, float accuracy)
        {
            sign = Mathf.Sign(v);
            return Mathf.Abs(v) > accuracy;
        }

        private void SetClimbWallSpeedX()
        {
            if (!(inputMovement.x > 0 && IsTouchingClimbWallLeftAbs || inputMovement.x < 0 && IsTouchingClimbWallRightAbs))
            {
                releaseFromClimbWallTimer.Clock();
            }

            if (releaseFromClimbWallTimer.Finished)
            {
                Run(true);
            }
            else
            {
                //Glue to the wall
                localVelocity.x = IsTouchingClimbWallRightAbs ? 1 : -1;
            }

        }

        internal bool SmalerY(float y, float y2)
        {
            if (!isYFliped)
                return y < y2;
            else
                return y > y2;
        }

        internal bool SmalerYAbs(float y, float abs)
        {
            if (!isYFliped)
                return y < abs;
            else
                return y > -abs;
        }

        internal bool SmalerEqualY(float y, float y2) => !GreaterY(y, y2);

        internal bool GreaterYAbs(float y, float abs)
        {
            if (!isYFliped)
                return y > abs;
            else
                return y < -abs;
        }

        internal bool GreaterY(float y, float y2)
        {
            if (!isYFliped)
                return y > y2;
            else
                return y < y2;
        }

        internal bool GreaterEqualY(float y, float y2) => !SmalerY(y, y2);

        #endregion

        #region Spawning
        public void Kill()
        {
            if (stateMachine.State != PlayerStates.Death && !isInvincible)
                stateMachine.IsDead = true;
        }

        private void Spawn()
        {
            SetRespawn(CheckpointManager.GetCheckpointPos());
        }

        private void Respawn()
        {
            Vector2 position = CheckpointManager.GetCheckpointPos();
            TransitionManager.Instance.ShowTransition(deathTransition, null, () => SetRespawn(position));
            effectsController.TriggerEffect("Respawn");
            //LevelManager.LoadScene(LevelManager.CurrantLevel.StartScene, deathTransition);
        }

        private void SetRespawn(Vector2 position)
        {
            LevelManager.ResetLevel();
            Reset();

            if (isRespawnOnStart)
                playerCollider.SetPositionCorrected(position);
            else
                Teleport(transform.position);

            stateMachine.IsDead = false;
        }

        public void Teleport(Vector2 position)
        {
            transform.position = position;
            lastFramePosition = position;
            rb.velocity = Vector2.zero;
        }

        private void Reset()
        {
            stateMachine.Reset();
            playerCollider.Reset();
            cameraController.SetFallingDead(false);

            jumpInput.Reset();


            dashStartDisableTimer = float.NegativeInfinity;
            dashVelocity = Vector2.zero;
            estimatedDashPosition = Vector2.zero;
            dashEndTimer = 0;
            lastDashTurningCenter = Vector2.negativeInfinity;
            dashTurningTargetAngle = 0;

            liliumState = 0;
            liliumStartTimer = 0;

            lastJumpTimer = float.NegativeInfinity;
            lastFrameMovingVelocity = default;
            localGravity = 0;
            Vector2 flipedDirection = Vector2.zero;
        }
        #endregion
    }
}
