using SceneM;
using TMPro;
using UnityEngine;

namespace LSHGame.UI
{
    public class MainMenuScoreField : MonoBehaviour
    {

        [SerializeField]
        private InventoryItem item;

        [SerializeField]
        private TMP_Text textField;


        [SerializeField]
        private bool hideIfNoItems = true;

        private void Start()
        {
            int count = Inventory.GetCount(item);

            if (hideIfNoItems && count == 0)
            {
                gameObject.SetActive(false);
            }

            textField.text = count.ToString();
        }

    }
}
