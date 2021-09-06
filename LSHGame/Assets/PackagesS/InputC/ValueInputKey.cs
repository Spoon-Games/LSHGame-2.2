using UnityEngine.InputSystem;

namespace InputC
{
    [System.Serializable]
    public class ValueInputKey<T> : InputKey where T:struct
    {

        public T Value { get {
                IsRegistredCheck();
                if (!IsActive)
                    return default;
                else
                    return input.ReadValue<T>();
            } }
    }
}
