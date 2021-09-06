using InputC;
using UnityEngine;

namespace LSHGame.Util
{
    [CreateAssetMenu(fileName = "InputAgent" , menuName = "LSHGame/InputAgent")]
    public class GlobalInputAgent : InputAgent
    {
        public InputKey Jump;
        public ValueInputKey<Vector2> Move;
        public InputKey Back;

        protected override void InitializeKeys()
        {
            Jump.Init(this, GameInput.Controller.Player.Jump,"Jump");
            Move.Init(this, GameInput.Controller.Player.Movement,"Move");
            Back.Init(this, GameInput.Controller.Player.BackUI,"Back");
        }
    }
}
