using UnityEngine;

namespace LSHGame.Util
{
    public class ReActivator : MonoBehaviour, IRecreatable
    {
        [SerializeField]
        [Tooltip("Asign GameObjects so they will be set active to thier orignal state on Recreate")]
        private GameObject[] reActiveObjects;

        private bool[] activeStates;

        private void Awake()
        {
            activeStates = new bool[reActiveObjects.Length];

            for (int i = 0; i < reActiveObjects.Length; i++)
            {
                activeStates[i] = reActiveObjects[i].activeSelf;
            }
        }

        public void Recreate()
        {
            for (int i = 0; i < reActiveObjects.Length; i++)
            {
                reActiveObjects[i].SetActive(activeStates[i]);
            }
        }
    }
}
