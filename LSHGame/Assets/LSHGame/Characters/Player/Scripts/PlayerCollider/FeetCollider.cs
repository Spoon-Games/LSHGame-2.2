using LSHGame.Util;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class FeetCollider : MonoBehaviour
    {
        public BoxCollider2D Col { get; private set; }
        public ContactPoint2D[] allCPs = new ContactPoint2D[1];

        private LayerMask groundLayers;
        private PlayerController parent;
        private ContactFilter2D contactFilter;

        public void Initialize(LayerMask groundLayers, PlayerController parent)
        {
            this.groundLayers = groundLayers;
            this.parent = parent;

            Col = GetComponent<BoxCollider2D>();
            contactFilter = new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = groundLayers,
                useTriggers = false,
                useNormalAngle = true,
                minNormalAngle = 0.001f,
                maxNormalAngle = 180 - 0.001f
            };
        }

        public bool FindGround()
        {
            int count = Physics2D.GetContacts(Col, contactFilter, allCPs);
            //bool isTouching = Physics2D.IsTouchingLayers(Col, groundLayers);
            //Debug.Log($"Contacts Count: {count} First CP {(count > 0 ? allCPs[0].collider.name : null)} IsTouching: {isTouching}");
            return count > 0;
        }


        public Rect GetColliderRect() {

            if (Col == null)
                Col = GetComponent<BoxCollider2D>();

            return Col.GetColliderRect();
        }
    }
}
