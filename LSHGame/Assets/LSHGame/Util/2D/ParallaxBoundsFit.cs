using System;
using UnityEngine;

namespace LSHGame.Util
{
    public class ParallaxBoundsFit : MonoBehaviour
    {
        [SerializeField]
        private Rect orthographicBounds;

        [SerializeField]
        private float maxCameraOrthographicSize = 5;

        [SerializeField]
        private float maxAspectRatio = 16 / 9;

        [SerializeField] private SpriteRenderer referenceSprite;

        [SerializeField] private Vector2 startPivot = Vector2.one / 2;

        protected void Start()
        {
            if (referenceSprite == null)
                referenceSprite = GetComponent<SpriteRenderer>();
            if (referenceSprite == null)
                Debug.LogError("No sprite renderer has been asigned to sprite parallax");

            if (referenceSprite != null)
                GetSpirteSize();

            
        }

        private void GetSpirteSize()
        {
            Vector2 cameraSize = GetCameraSize();
            Transform cameraTransform = Camera.main.transform;

            float multiplier = ParallaxLayer.GetMultiplier(referenceSprite.transform,cameraTransform);
            Vector2 pivot = orthographicBounds.min + orthographicBounds.size * startPivot;
            Vector2 rectSize = orthographicBounds.size / 2;

            Vector2 spriteSize = rectSize + cameraSize - multiplier * rectSize;

            if(referenceSprite.sprite != null)
            {
                Vector2 realSpriteSize = referenceSprite.size / 2;
                Vector2 factor = spriteSize / realSpriteSize;
                float scale = Mathf.Max(0.1f, factor.x, factor.y, transform.localScale.x, transform.localScale.y);
                transform.localScale = new Vector3(scale, scale, 1);
            }

            Vector2 newStartPos = ((Vector2) cameraTransform.position + cameraSize * (startPivot - Vector2.one /2) - pivot) * multiplier + pivot;
            transform.position = new Vector3(newStartPos.x, newStartPos.y, transform.position.z);
        }

        private Vector2 GetCameraSize()
        {
            return new Vector2(maxAspectRatio * maxCameraOrthographicSize, maxCameraOrthographicSize);
        }

#if UNITY_EDITOR
        //private void OnValidate()
        //{
        //    try
        //    {
        //        if (Camera.main.transform != null)
        //        {
        //            Start();
        //        }
        //    }
        //    catch (Exception) { }
        //}

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(orthographicBounds.center, orthographicBounds.size);

            //Gizmos.DrawWireCube(Camera.main.transform.position, GetCameraSize(Camera.main) * 2 + new Vector2(0.1f,0.1f));
        }
#endif
    }
}
