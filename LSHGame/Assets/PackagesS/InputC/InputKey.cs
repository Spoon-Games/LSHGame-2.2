using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputC
{
    [System.Serializable]
    public class InputKey
    {
        protected bool isFired;
        public bool IsFired
        {
            get
            {
                IsRegistredCheck();
                return isFired && IsActive;
            }
        }

        public bool IsRealeasedThisFrame => releaseFrame == Time.frameCount;
        protected int releaseFrame = -1;

        public event Action OnPress;
        public event Action OnCancel;

        protected InputAgent parent;
        protected InputAction input;

        private bool _isActive;
        [SerializeField]
        private bool _isBlocking;
        [SerializeField]
        private bool _isRegistered;

        public bool IsActive => _isActive;
        public bool IsBlocking => _isBlocking;
        public bool IsRegistered => _isRegistered;
        public bool IsListening => parent.IsListening && IsRegistered;

        public string Name { get; private set; }

        public void Init(InputAgent parent, InputAction input, string name)
        {
            this.parent = parent;
            this.input = input;
            Name = name;

            parent.RegisterInputKey(this);
        }

        protected internal virtual void Activate()
        {
            if (IsActive || !IsRegistered)
                return;

            _isActive = true;

            input.performed += OnActionPerformed;
            input.canceled += OnActionCanceled;

        }

        protected internal virtual void Deactivate()
        {
            if (!IsActive)
                return;

            input.performed -= OnActionPerformed;
            input.canceled -= OnActionCanceled;

            if (isFired)
            {
                isFired = false;
                InvokeCancel();
            }

            _isActive = false;
        }

        protected virtual void OnActionPerformed(InputAction.CallbackContext ctx)
        {
            isFired = true;
            InvokePerformed();
        }

        protected virtual void OnActionCanceled(InputAction.CallbackContext ctx)
        {
            isFired = false;
            InvokeCancel();
        }

        private void InvokePerformed()
        {
            if (IsFired)
            {
                OnPress?.Invoke();
            }
        }

        private void InvokeCancel()
        {
            if (IsActive && !IsFired)
            {
                releaseFrame = Time.frameCount;
                OnCancel?.Invoke();
            }
        }

        protected void IsRegistredCheck()
        {
            if (!IsRegistered)
                Debug.LogWarning($"You are trying to listen to Inputkey: {Name} of InputAgent: {parent.name} which is not registered");
        }

        public void Editor_SetIsBlocking(bool isBlocking) => _isBlocking = isBlocking;
    }
}
