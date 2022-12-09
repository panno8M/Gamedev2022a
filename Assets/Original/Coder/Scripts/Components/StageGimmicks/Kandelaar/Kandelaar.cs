using System;
using UniRx;
using UniRx.Ex.InteractionTraits.Core;
using UnityEngine;

namespace Assembly.Components.StageGimmicks
{
  public class Kandelaar : MonoBehaviour
  {
    [SerializeField] Interactable _interactable;
    [SerializeField] Rigidbody _rb;

    void Start()
    {
      _interactable.holdable.OnHold
          .Subscribe(_ =>
          {
            _rb.useGravity = false;
            _rb.isKinematic = true;
          });
      _interactable.holdable.OnRelease
          .Subscribe(_ =>
          {
            _rb.useGravity = true;
            _rb.isKinematic = false;
            Observable.Timer(TimeSpan.FromMilliseconds(100))
              .Subscribe(_ =>
              {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
              });
          });
    }
  }
}
