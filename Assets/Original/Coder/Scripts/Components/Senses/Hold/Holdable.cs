#if UNITY_EDITOR
// #define DEBUG_HOLDABLE
#endif

using System;
using UnityEngine;
using UniRx;

namespace Assembly.Components
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class Holdable : MonoBehaviour
  {
    [SerializeField] Rigidbody _rigidbodyObjectPhysics;
#if DEBUG_HOLDABLE
    [Header("Debug")]
#endif
#if DEBUG_HOLDABLE
    [SerializeField]
#endif
    Holder _owner;
    Subject<Holder> _OnHold = new Subject<Holder>();
    Subject<Holder> _OnRelease = new Subject<Holder>();

    public Rigidbody rb => _rigidbodyObjectPhysics;
    public Holder owner => _owner;
    public IObservable<Holder> OnHold => _OnHold;
    public IObservable<Holder> OnRelease => _OnRelease;

    public bool HoldAccepted(Holder holder)
    {
      if (!enabled) { return false; }
      _owner = holder;
      _OnHold.OnNext(owner);
      return true;
    }
    public bool ReleaseAccepted(Holder holder)
    {
      _OnRelease.OnNext(owner);
      _owner = null;
      return true;
    }

    public void ReleaseMe()
    {
      if (owner && owner.isActiveAndEnabled)
      {
        owner.Release(this);
      }
    }

    public new bool enabled
    {
      get => base.enabled;
      set
      {
        if (!value) { ReleaseMe(); }
        base.enabled = value;
      }
    }
  }
}
