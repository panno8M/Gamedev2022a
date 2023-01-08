using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace Assembly.GameSystem.Input
{
  public static class InputSystemExtensions
  {
    public static IDisposable AsButton(this InputAction inputAction,
      ReactiveProperty<bool> button, Subject<Unit> OnPressed)
    {
      return Observable.FromEvent<InputAction.CallbackContext>(
          h => inputAction.performed += h,
          h => inputAction.performed -= h)
          .Subscribe(x =>
          {
            button.Value = x.ReadValueAsButton();
            if (button.Value) { OnPressed.OnNext(Unit.Default); }
          });
    }
    public static IDisposable AsAxis(this InputAction inputAction, ReactiveProperty<float> axis)
    {
      return Observable.FromEvent<InputAction.CallbackContext>(
          h => inputAction.performed += h,
          h => inputAction.performed -= h)
          .Subscribe(x => axis.Value = x.ReadValue<float>());
    }
    public static IDisposable AsAxis2d(this InputAction inputAction, ReactiveProperty<Vector2> axis2d)
    {
      return Observable.FromEvent<InputAction.CallbackContext>(
          h => inputAction.performed += h,
          h => inputAction.performed -= h)
          .Subscribe(x => axis2d.Value = x.ReadValue<Vector2>());
    }
  }
}
