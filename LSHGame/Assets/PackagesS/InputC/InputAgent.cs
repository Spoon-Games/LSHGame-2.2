using System;
using System.Collections.Generic;
using UnityEngine;

namespace InputC
{
    public abstract class InputAgent : ScriptableObject, IComparable<InputAgent>
    {
        [SerializeField]
        [InspectorName("Priority")]
        private int _priority;
        public int Priority => _priority;

        [NonSerialized]
        private bool _isListening;
        public bool IsListening
        {
            get => _isListening; internal set
            {
                foreach (var key in inputKeys)
                    key.Deactivate();

                _isListening = value;
            }
        }

        [NonSerialized]
        internal List<InputKey> inputKeys = new List<InputKey>();

        protected abstract void InitializeKeys();


        internal void RegisterInputKey(InputKey key)
        {
            if (inputKeys.Contains(key))
                throw new InvalidProgramException($"InputKey: {key.Name} was already added to InputAgent: {this}");
            inputKeys.Add(key);
        }

        public int CompareTo(InputAgent other)
        {
            return other.Priority - Priority;
        }

        public void Listen()
        {
            InputControllSystem.RegisterListeningAgent(this);
        }

        public void StopListening()
        {
            InputControllSystem.DeregisterListeningAgent(this);
        }

        public IEnumerable<InputKey> GetKeys() => inputKeys;


        public virtual void OnEnable()
        {
            InitializeKeys();
        }
    }
}
