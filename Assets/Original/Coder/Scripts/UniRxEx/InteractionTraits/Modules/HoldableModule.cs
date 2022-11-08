using System;
using UnityEngine;
using UniRx.Ex.InteractionTraits.Core;

namespace UniRx.Ex.InteractionTraits
{
  public class HoldableModule : InteractableModuleBase
  {
    HolderModule _owner;
    public HolderModule owner => _owner;
    [SerializeField] Rigidbody _rb;
    public Rigidbody rb => _rb;

    Subject<HolderModule> _OnHold = new Subject<HolderModule>();
    public IObservable<HolderModule> OnHold => _OnHold;
    Subject<HolderModule> _OnRelease = new Subject<HolderModule>();
    public IObservable<HolderModule> OnRelease => _OnRelease;

    void Awake() {
      OnDisactivated.Subscribe(_ => ReleaseMe());
    }

    public bool HoldAccepted(HolderModule holder)
    {
      if (!isActive) { return false; }
      _owner = holder;
      _OnHold.OnNext(owner);
      return true;
    }
    public bool ReleaseAccepted(HolderModule holder)
    {
      _OnRelease.OnNext(owner);
      _owner = null;
      return true;
    }

    public void ReleaseMe()
    {
      owner?.Release(this);
    }
  }
}
