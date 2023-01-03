using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem;

namespace Assembly.Components.Items
{
  public class CardBoard : DiBehavior
  {
    [SerializeField] Holdable _holdable;
    void Start() => Initialize();

    protected override void Blueprint()
    {
      OnHold(holding: false);

      _holdable.OnHold.Subscribe(_ => OnHold(holding: true));
      _holdable.OnRelease.Subscribe(_ => OnHold(holding: false));
      _holdable.OnRelease
          .Delay(TimeSpan.FromMilliseconds(100))
          .Subscribe(_ => ResetRigidbody());
    }

    void ResetRigidbody()
    {
      rigidbody.velocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;
    }

    void OnHold(bool holding)
    {
      rigidbody.useGravity = !holding;
      rigidbody.isKinematic = holding;
    }
  }
}