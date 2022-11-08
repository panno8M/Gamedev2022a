using System;
using UnityEngine;
using UniRx;

namespace Assembly.Components.Senses
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

    public void HoldAccepted(HolderModule holder)
    {
      _owner = holder;
      _OnHold.OnNext(owner);
    }
    public void ReleaseAccepted(HolderModule holder)
    {
      _OnRelease.OnNext(owner);
      _owner = null;
    }

    public void ReleaseMe()
    {
      owner?.Release(this);
    }
  }
}
