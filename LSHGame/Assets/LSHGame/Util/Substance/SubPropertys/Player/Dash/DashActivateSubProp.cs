using UnityEngine;

namespace LSHGame.Util
{
    public class DashActivateSubProp : SubstanceProperty
    {
        public float EqualizeLengthWeight = 1;
        public float EqualizeDirectionWeight = 1;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IDashActivateRec r)
            {
                r.IsDashActive = true;

                r.DashDirection = transform.right;

                r.EqualizeDashLengthWeight = EqualizeLengthWeight;
                r.EqualizeDashDirectionWeight = EqualizeDirectionWeight;
                r.DashActivateCenterPos = transform.position;
            }
        }
    }

    public interface IDashActivateRec : IDataReciever
    {
        bool IsDashActive { get; set; }

        Vector2 DashDirection { get; set; }

        float EqualizeDashLengthWeight { get; set; }
        float EqualizeDashDirectionWeight { get; set; }
        Vector2 DashActivateCenterPos { get; set; }

    }
}
