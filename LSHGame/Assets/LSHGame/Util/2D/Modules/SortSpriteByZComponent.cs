using UnityEngine;

namespace LSHGame.Util
{
    public class SortSpriteByZComponent : MonoBehaviour
    {
        [SerializeField] private float minZ;
        [SerializeField] private float maxZ;
        [SerializeField] private int minLayer;
        [SerializeField] private int maxLayer;

        private void Awake()
        {
            Sort();
        }

        private void Sort()
        {
            var sprites = GetComponentsInChildren<SpriteRenderer>();

            foreach (var sprite in sprites)
            {
                float f = Mathf.InverseLerp(minZ, maxZ, sprite.transform.position.z);

                int layer = (int)Mathf.LerpUnclamped(minLayer, maxLayer, f);
                sprite.sortingOrder = layer;
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            Sort();
        }

#endif
    }
}
