using UnityEngine;

namespace LSHGame.Util
{
    public class DamageSubProp : SubstanceProperty
    {
        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IDamageRec r)
            {
                r.IsDamage = true;
            }
        }
    }

    public interface IDamageRec : IDataReciever
    {
        bool IsDamage { get; set; }
    }
}
