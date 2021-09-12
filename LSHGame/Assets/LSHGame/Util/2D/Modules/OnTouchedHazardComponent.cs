using UnityEngine;
using UnityEngine.Events;

namespace LSHGame.Util
{
    public class OnTouchedHazardComponent : MonoBehaviour
    {
        [SerializeField] private Rect touchRect;
        [SerializeField] private LayerMask hazardLayers;
        [SerializeField] private LayerMask groundLayers;

        [SerializeField] private UnityEvent OnTouchedHazard;

        private SubstanceSet substanceSet = new SubstanceSet();

        private void FixedUpdate()
        {
            var Stats = new TouchedHazardStats();

            RetrieveSubstanceOnRect(touchRect);

            substanceSet.ExecuteQuery();
            substanceSet.RecieveDataAndReset(Stats);

            if (Stats.IsDamage || IsTouchingLayerRectRelative(touchRect, hazardLayers, true) ) // if touching hazard
            {
                OnTouchedHazard.Invoke();
            }
        }

        private bool RetrieveSubstanceOnRect(Rect localRect, bool noTouchOnTriggers = false)
        {
            SubstanceManager.RetrieveSubstances(
                localRect.LocalToWorldRect(transform),
                substanceSet,
                new PlayerSubstanceFilter { ColliderType = PlayerSubstanceColliderType.Main },
                groundLayers,
                out bool touch,
                noTouchOnTriggers);
            return touch;
        }

        private bool IsTouchingLayerRectRelative(Rect rect, LayerMask layers, bool useTriggers = false)
        {
            rect = TransformRectPS(rect);
            return IsTouchingLayerRectAbsolute(rect, layers, useTriggers);
        }

        private class TouchedHazardStats : IDamageRec
        {
            public bool IsDamage { get; set; }
        }

        private Rect TransformRectPS(Rect rect)
        {
            Rect r = new Rect() { size = rect.size * AbsVector2(transform.lossyScale) };
            r.center = rect.center * transform.lossyScale + (Vector2)transform.position;
            return r;
        }

        private Vector2 AbsVector2(Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        private bool IsTouchingLayerRectAbsolute(Rect rect, LayerMask layers, bool useTriggers = false)
        {
            return Physics2D.OverlapBox(rect.center, rect.size, 0, new ContactFilter2D() { useTriggers = useTriggers, layerMask = layers, useLayerMask = true }, new Collider2D[1]) > 0;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            DrawRectRelative(touchRect);
        }

        private void DrawRectRelative(Rect rect)
        {
            Rect r = rect.LocalToWorldRect(transform);//TransformRectPS(rect);
            Gizmos.DrawWireCube(r.center, r.size);
        }

#endif
    }
}
