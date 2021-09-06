using UnityEngine;

namespace InputC
{
    public class InputListenComponent : MonoBehaviour
    {
        public InputAgent agent;
        public bool listenOnStart = true;
        public bool stopListeningOnDestroy = true;

        private void Start()
        {
            if (listenOnStart)
                StartListening();
        }

        private void OnDestroy()
        {
            if (stopListeningOnDestroy)
                StopListening();
        }

        public void StartListening()
        {
            agent.Listen();
        }

        public void StopListening()
        {
            agent.StopListening();
        }
    }
}
