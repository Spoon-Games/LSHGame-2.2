namespace LSHGame.Util
{
    public class SaveGroundSubProp : SubstanceProperty
    {
        public bool IsSaveGround = false;

        protected internal override void RecieveData(IDataReciever reciever)
        {
            if (reciever is ISaveGroundRec r)
            {
                r.IsSaveGround = IsSaveGround;
            }
        }
    }

    public interface ISaveGroundRec
    {
        bool IsSaveGround { get; set; }
    }
}
