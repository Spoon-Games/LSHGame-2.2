using LSHGame.Util;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.PlayerN
{
    public class PinchPlayerCollider : BasePlayerCollider
    {
        #region Serialized Fields
        [Header("References")]
        [SerializeField]
        private Rigidbody2D rb;

        [SerializeField]
        private FeetCollider feetCollider;

        [SerializeField]
        private PinchCollider horizontalPinch;

        [SerializeField]
        private PinchCollider verticalUpPinch;

        [SerializeField]
        private PinchCollider defaultPinch;

        [Header("Touchpoints")]
        [SerializeField]
        private Rect mainCollider;

        [SerializeField]
        private Rect headTouchRect;
        private Rect HeadTouchRect => headTouchRect;

        [SerializeField]
        private Rect climbLadderTouchRect;

        [SerializeField]
        private Rect rightSideTouchRect;
        private Rect RightSideTouchRect => rightSideTouchRect;

        [SerializeField]
        private float crushedRectInset = 0.1f;

        [Header("LayerMasks")]
        [SerializeField]
        private LayerMask groundLayers;
        [SerializeField]
        private LayerMask crushLayers;
        [SerializeField]
        private LayerMask hazardsLayers;
        [SerializeField]
        private LayerMask saveGroundLayers;
        #endregion

        #region Attributes
        private PlayerController parent;
        private PlayerStateMachine stateMachine;

        private PlayerStats Stats => parent.Stats;

        private enum PinchStates { Default, Horizontal, Up }
        private PinchStates pinchState = PinchStates.Default;

        private PinchCollider currentPinchCollider;
        #endregion

        #region Lifecycle

        public override Rigidbody2D Initialize(PlayerController parent, PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            this.parent = parent;

            feetCollider.Initialize(groundLayers, parent);

            currentPinchCollider = defaultPinch;
            defaultPinch.Enable();
            horizontalPinch.Disable();
            verticalUpPinch.Disable();

            return rb;
        }

        public override void CheckUpdate()
        {
            CheckTouch();
        }

        public override void ExeUpdate()
        {
            //ValidateCPs();
            ExeBounce();
        }

        public override void LateExeUpdate()
        {
            ExePinching();

            feetCollider.ExeUpdate();
        }

        public override void Reset()
        {
            gameObject.layer = 12;
            feetCollider.Reset();
        }
        #endregion

        #region Update Touch

        private void CheckTouch()
        {
            /* Retrive substances */

            RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Ladders, climbLadderTouchRect);

            IsTouchingClimbWallRight = RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Sides, RightSideTouchRect, true);
            IsTouchingClimbWallLeft = RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Sides, InvertOnX(RightSideTouchRect), true);


            stateMachine.IsGrounded = feetCollider.FindGround();
            RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Feet, feetCollider.Col);
            Stats.IsSaveGround &= IsTouchingLayerRectRelative(feetCollider.GetColliderRect(), saveGroundLayers);

            RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Main, mainCollider);
            bool isCrushed = false;
            if(currentPinchCollider != null)
                isCrushed = IsTouchingLayerRectRelative( currentPinchCollider.GetColliderRect().InsetRect(crushedRectInset) , crushLayers, false);

            stateMachine.IsHeadObstructed = RetrieveSubstanceOnRect(PlayerSubstanceColliderType.Head, HeadTouchRect, true);


            /* Activate queried substances */
            parent.SubstanceSet.ExecuteQuery();

            /* Recieve data */
            parent.SubstanceSet.RecieveDataAndReset(parent.Stats);


            /* Update internal values */
            stateMachine.IsTouchingClimbLadder = parent.Stats.IsLadder;
            stateMachine.IsFeetTouchingClimbLadder = parent.Stats.IsFeetLadder;


            if (parent.Stats.IsDamage || IsTouchingLayerRectRelative(mainCollider, hazardsLayers,true) || isCrushed) // if touching hazard
            {
                parent.Kill();
            }

        }


        #endregion

        #region Exe Pinching
        private void ExePinching()
        {
            PinchStates newState = GetPinchState();
            if (newState != pinchState)
            {
                pinchState = newState;

                currentPinchCollider?.Disable();

                switch (pinchState)
                {
                    case PinchStates.Default:
                        currentPinchCollider = defaultPinch;
                        break;
                    case PinchStates.Horizontal:
                        currentPinchCollider = horizontalPinch;
                        break;
                    case PinchStates.Up:
                        currentPinchCollider = verticalUpPinch;
                        break;
                }

                currentPinchCollider.Enable();
            }

            if (currentPinchCollider != null)
            {
                Vector2 pinchVelocity = currentPinchCollider.GetPinchOfVelocity();

                if (pinchVelocity.x > 0)
                    parent.localVelocity.x = Mathf.Max(pinchVelocity.x, parent.localVelocity.x);
                else if (pinchVelocity.x < 0)
                    parent.localVelocity.x = Mathf.Min(pinchVelocity.x, parent.localVelocity.x);

                if (pinchVelocity.y > 0)
                    parent.localVelocity.y = Mathf.Max(pinchVelocity.y, parent.localVelocity.y);
                else if (pinchVelocity.x < 0)
                    parent.localVelocity.y = Mathf.Min(pinchVelocity.y, parent.localVelocity.y);

               // Debug.Log("LocalVelocity: " + parent.localVelocity + " pinchVelocity: " + pinchVelocity);
            }
        }

        private PinchStates GetPinchState()
        {
            Vector2 localVelocity = parent.localVelocity;
            bool isUp = localVelocity.y > 1f;
            bool isHorizontal = !localVelocity.x.Approximately(0, 0.5f);

            if (stateMachine.State == PlayerStates.Death)
                return PinchStates.Default;

            if (parent.lastJumpTimer + 0.2f > Time.fixedTime)
                return PinchStates.Up;

            if ((stateMachine.State == PlayerStates.ClimbLadder ||
                stateMachine.State == PlayerStates.ClimbLadderTop)
                && !parent.inputMovement.y.Approximately(0, 0.1f))
            {
                return PinchStates.Up;
            }

            if((stateMachine.State == PlayerStates.Locomotion ||
                stateMachine.State == PlayerStates.Aireborne)
                && parent.inputMovement.y < -0.1f 
                && Stats.IsFeetLadder)
            {
                return PinchStates.Up;
            }

            if (stateMachine.State == PlayerStates.Locomotion ||
                stateMachine.State == PlayerStates.Dash || 
                stateMachine.State == PlayerStates.ClimbWall ||
                stateMachine.State == PlayerStates.ClimbWallExhaust)
                return PinchStates.Horizontal;

            if (isUp)
            {
                return PinchStates.Up;
            }

            return PinchStates.Horizontal;
        }
        #endregion

        #region Exe Bounce

        private void ExeBounce()
        {
            if (Stats.BounceSettings == null || feetCollider.allCPs.Count == 0)
                return;

            Vector2 normal = feetCollider.allCPs[0].normal;
            for (int i = 1; i < feetCollider.allCPs.Count; i++)
            {
                normal += feetCollider.allCPs[i].normal;
            }
            normal.Normalize();

            normal = Stats.BounceSettings.GetRotation(normal);
            float bounceSpeed = Stats.BounceSettings.GetBounceSpeed(Mathf.Abs(Vector2.Dot(normal, rb.velocity)));

            Vector2 bounceVelocity = normal * bounceSpeed;

            Vector2 orthogonalVel = Vector2.Perpendicular(normal);
            orthogonalVel = orthogonalVel * Vector2.Dot(orthogonalVel, rb.velocity);

            bounceVelocity += orthogonalVel;

            if ((bounceVelocity - ((Vector2)rb.velocity)).SqrMagnitude() < 0.1f)
                return;

            parent.localVelocity = bounceVelocity - parent.lastFrameMovingVelocity;

            Stats.OnBounce?.Invoke();
        }

        #endregion

        #region Spawnig

        public override void SetToDeadBody()
        {
            gameObject.layer = 19;
        }

        public override void SetPositionCorrected(Vector2 position)
        {
            int i = 0;
            do
            {
                parent.Teleport(position);
                position += Vector2.up * 0.1f;

                if (i > 1000)
                    break;
                i++;
            } while (IsTouchingLayerRectRelative(mainCollider, groundLayers));
        }

        #endregion

        #region Helper Methods



#if UNITY_EDITOR
        #region Draw Gizmos
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //if (ladderTouchPoint != null)
            //Gizmos.DrawWireSphere(ladderTouchPoint.position, ladderTouchRadius);
            DrawRectRelative(rightSideTouchRect);
            DrawRectRelative(InvertOnX(rightSideTouchRect));

            DrawRectRelative(climbLadderTouchRect);

            DrawRectRelative(headTouchRect);

            Gizmos.color = Color.blue;
            DrawRectRelative(mainCollider);

            Gizmos.color = Color.yellow;

            DrawRectRelative(mainCollider.InsetRect(crushedRectInset));
            //foreach(ContactPoint2D contactPoint2D in allCPs)
            //{
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawSphere(contactPoint2D.point, 0.05f);
            //}
        }

        private void DrawRectRelative(Rect rect)
        {
            Rect r = rect.LocalToWorldRect(transform);//TransformRectPS(rect);
            Gizmos.DrawWireCube(r.center, r.size);
        }

        private void OnDrawGizmos()
        {
            SubstanceManager.Instance.OnDrawGizmos();
        }
        #endregion

        #region Loco Preview
        public PlayerStats GetStatePreview(Vector3 position, SubstanceSet prevSubSet, PlayerStateMachine stateMachine, PlayerController parent, out Rect mainColliderRect, out bool isTouchingClimbWallLeft)
        {
            PlayerStats prevStats = parent.defaultStats.Clone();
            Matrix4x4 trs = Matrix4x4.Translate(position);


            RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Ladders, climbLadderTouchRect);

            bool IsTouchingClimbWallRight = RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Sides, rightSideTouchRect, true);
            isTouchingClimbWallLeft = RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Sides, InvertOnX(rightSideTouchRect), true);
            stateMachine.IsTouchingClimbWall = isTouchingClimbWallLeft || IsTouchingClimbWallRight;

            RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Feet, feetCollider.GetColliderRect());
            stateMachine.IsGrounded = IsTouchingLayer(trs, feetCollider.GetColliderRect(), groundLayers);

            RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Main, mainCollider);

            stateMachine.IsHeadObstructed = RetrieveSubstancePreview(trs, prevSubSet, PlayerSubstanceColliderType.Head, headTouchRect, true);


            /* Activate queried substances */
            prevSubSet.ExecuteQuery();

            /* Recieve data */
            prevSubSet.RecieveDataAndReset(prevStats);


            /* Update internal values */
            stateMachine.IsTouchingClimbLadder = prevStats.IsLadder;
            stateMachine.IsFeetTouchingClimbLadder = prevStats.IsFeetLadder;

            stateMachine.IsDead = prevStats.IsDamage || IsTouchingLayer(trs, mainCollider, hazardsLayers); // if touching hazard

            mainColliderRect = trs.Multiply(mainCollider);
            return prevStats;
        }

        private bool RetrieveSubstancePreview(Matrix4x4 trs, SubstanceSet subSet, PlayerSubstanceColliderType colliderType, Rect localRect, bool noTouchOnTriggers = false)
        {
            SubstanceManager.RetrieveSubstances(
                trs.Multiply(localRect),
                subSet,
                new PlayerSubstanceFilter { ColliderType = colliderType },
                groundLayers,
                out bool touch,
                noTouchOnTriggers);
            return touch;
        }

        private bool IsTouchingLayer(Matrix4x4 trs, Rect localRect, LayerMask layerMask, bool useTriggers = false)
        {
            Rect rect = trs.Multiply(localRect);
            return Physics2D.OverlapBox(rect.center, rect.size, 0, layerMask);
        }
        #endregion
#endif

        #region IsTouching [obsolete]
        private bool IsTouchingLayers(Collider2D collider, LayerMask layers)
        {
            Collider2D[] colliders = new Collider2D[0];
            Physics2D.OverlapCollider(collider, new ContactFilter2D() { layerMask = layers }, colliders);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsTochingLayers<T>(Collider2D collider, LayerMask layers, out T component)
        {
            Collider2D[] colliders = new Collider2D[0];
            Physics2D.OverlapCollider(collider, new ContactFilter2D() { layerMask = layers }, colliders);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject && colliders[i].TryGetComponent(out component))
                {
                    return true;
                }
            }
            component = default;
            return false;
        }

        private bool IsTochingLayersCircle(Vector2 position, float radius, LayerMask layers)
        {
            return Physics2D.OverlapCircle(position, radius, layers) != null;
        }

        private bool IsTouchingLayerRectRelative(Rect rect, LayerMask layers, out Transform touch, bool useTriggers = false)
        {
            rect = TransformRectPS(rect);
            List<Collider2D> collider2Ds = new List<Collider2D>();
            Physics2D.OverlapBox(rect.center, rect.size, 0, new ContactFilter2D() { useTriggers = useTriggers, layerMask = layers, useLayerMask = true }, collider2Ds);
            //Debug.Log("IsTouchingLayerRect: Center: " + ((Vector3)rect.center + transform.position) + "\n Size: " + rect.size + "\nLayers: " + layers.value + " isTouching: "+isTouching);

            if (collider2Ds.Count > 0)
            {
                touch = collider2Ds[0].transform;
                return true;
            }
            else
            {
                touch = null;
                return false;
            }

        }

        private bool IsTouchingLayerRectRelative(Rect rect, LayerMask layers, bool useTriggers = false)
        {
            rect = TransformRectPS(rect);
            return IsTouchingLayerRectAbsolute(rect, layers, useTriggers);
        }

        private bool IsTouchingLayerRectAbsolute(Rect rect, LayerMask layers, bool useTriggers = false)
        {
            return Physics2D.OverlapBox(rect.center, rect.size, 0, new ContactFilter2D() { useTriggers = useTriggers, layerMask = layers, useLayerMask = true }, new Collider2D[1]) > 0;
        }
        #endregion

        #region Retrieve Substances
        private bool RetrieveSubstanceOnRect(PlayerSubstanceColliderType colliderType, Rect localRect, bool noTouchOnTriggers = false)
        {
            SubstanceManager.RetrieveSubstances(
                localRect.LocalToWorldRect(transform),
                parent.SubstanceSet,
                new PlayerSubstanceFilter { ColliderType = colliderType },
                groundLayers,
                out bool touch,
                noTouchOnTriggers);
            return touch;
        }

        private bool RetrieveSubstanceOnRect(PlayerSubstanceColliderType colliderType, BoxCollider2D collider)
        {
            SubstanceManager.RetrieveSubstances(
                collider,
                parent.SubstanceSet,
                new PlayerSubstanceFilter { ColliderType = colliderType },
                groundLayers,
                out bool touch);
            return touch;
        }
        #endregion

        #region Util

        private Vector2 AbsVector2(Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        private Rect InvertOnX(Rect rect)
        {
            return new Rect(-rect.x - rect.width, rect.y, rect.width, rect.height);
        }

        private Rect TransformRectPS(Rect rect)
        {
            Rect r = new Rect() { size = rect.size * AbsVector2(transform.lossyScale) };
            r.center = rect.center * transform.lossyScale + (Vector2)transform.position;
            return r;
        }
        #endregion
        #endregion

    }
}