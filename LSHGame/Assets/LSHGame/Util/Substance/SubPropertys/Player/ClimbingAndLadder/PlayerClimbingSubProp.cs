using UnityEngine;

namespace LSHGame.Util
{
    public class PlayerClimbingSubProp : SubstanceProperty
    {
        [Header("Climbing Ladder")]
        public DefaultableProperty<float> ClimbingLadderSpeed;

        [Header("Climbing Wall")]
        public DefaultableProperty<float> ClimbingWallSlideSpeed;
        public DefaultableProperty<float> ClimbingWallSlowSlideSpeed;
        public DefaultableProperty<Vector2> ClimbingWallJumpVelocity;


        protected internal override void RecieveData(IDataReciever reciever)
        {
            if(reciever is IPlayerClimbingRec r)
            {
                r.ClimbingLadderSpeed += ClimbingLadderSpeed;
                r.ClimbingWallSlideSpeed += ClimbingWallSlideSpeed;
                r.ClimbingWallSlowSlideSpeed += ClimbingWallSlowSlideSpeed;
                r.ClimbingWallJumpVelocity += ClimbingWallJumpVelocity;
            }
        }
    }

    public interface IPlayerClimbingRec : IDataReciever
    {
        float ClimbingLadderSpeed { get; set; }
        float ClimbingWallSlideSpeed { get; set; }
        float ClimbingWallSlowSlideSpeed { get; set; }
        
        Vector2 ClimbingWallJumpVelocity { get; set; }
    }


}
