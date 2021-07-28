using LSHGame.PlayerN;
using LSHGame.UI;
using UnityEngine;

namespace LSHGame
{
    public class DashFlowerHint : MonoBehaviour
    {
        [SerializeField]private InSceneTextComponent text;

        private bool active = false;

        private void Update()
        {
            if(!active && Player.Instance.IsLiliumActive)
            {
                active = true;
                text.Show();
            }else if(active && !Player.Instance.IsLiliumActive)
            {
                active = false;
                text.Hide();
            }
        }
    }
}
