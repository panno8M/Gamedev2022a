using UnityEngine;
using UnityEngine.InputSystem;

namespace UniRx.Ex.Inputs {
    public static class InputSystemExtensions {
        public static ReadOnlyReactiveProperty<bool> AsButton(this InputAction inputAction) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h)
                .Select(x => x.ReadValueAsButton())
                .ToReadOnlyReactiveProperty(false);
        }
        public static ReadOnlyReactiveProperty<float> AsAxis(this InputAction inputAction) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h)
                .Select(x => x.ReadValue<float>())
                .ToReadOnlyReactiveProperty(0f);
        }
        public static ReadOnlyReactiveProperty<Vector2> As2dAxis(this InputAction inputAction) {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h)
                .Select(x => x.ReadValue<Vector2>())
                .ToReadOnlyReactiveProperty(Vector2.zero);
        }
    }
}
