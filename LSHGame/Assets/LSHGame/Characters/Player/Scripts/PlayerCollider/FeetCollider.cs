using LSHGame.Util;
using System.Collections.Generic;
using UnityEngine;

namespace LSHGame.PlayerN
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class FeetCollider : MonoBehaviour
    {
        public BoxCollider2D Col { get; private set; }
        public List<ContactPoint2D> allCPs = new List<ContactPoint2D>();

        private LayerMask groundLayers;
        private PlayerController parent;

        public void Initialize(LayerMask groundLayers, PlayerController parent)
        {
            this.groundLayers = groundLayers;
            this.parent = parent;

            Col = GetComponent<BoxCollider2D>();
        }

        public bool FindGround()
        {
            ValidateCPs();

            bool found = false;
            ContactPoint2D groundCP = default;
            foreach (ContactPoint2D cp in allCPs)
            {
                //Pointing with some up direction
                //Debug.Log("CP: " + cp.collider.name + " normalY "+cp.normal.y+"" +
                //    " GreaterAbs: "+parent.GreaterYAbs(cp.normal.y,0.0001f) + " groundCp.NormalY: "+groundCP.normal.y+" GreaterY: "+ parent.GreaterY(cp.normal.y, groundCP.normal.y));
                if (parent.GreaterYAbs(cp.normal.y, 0.0001f) && (found == false || parent.GreaterY(cp.normal.y, groundCP.normal.y)))
                {
                    found = true;
                    groundCP = cp;
                }
            }
            //if (found)
            //Debug.Log("FindGround: " + allCPs.Count);
            return found;
        }

        public void ExeUpdate()
        {
            allCPs.Clear();
        }

        public void Reset()
        {
            allCPs.Clear();
        }

        #region Collision

        private void ValidateCPs()
        {
            int i = 0;
            while (allCPs.Count > i)
            {
                if (allCPs[i].collider == null || allCPs[i].collider.gameObject == null)
                {
                    allCPs.RemoveAt(i);
                }
                i++;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (groundLayers.IsLayer(collision.gameObject.layer) && collision.gameObject != gameObject)
            {
                allCPs.AddRange(collision.contacts);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (groundLayers.IsLayer(collision.gameObject.layer) && collision.gameObject != gameObject)
            {
                allCPs.AddRange(collision.contacts);
            }
        }

        #endregion

        public Rect GetColliderRect() => Col.GetColliderRect();
    }
}
