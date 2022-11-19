using System;
using UniRx.Ex.InteractionTraits.Core;
using System.Collections.Generic;
#if DEBUG
using UnityEngine;
#endif

namespace UniRx.Ex.InteractionTraits
{
  public class HolderModule : InteractorModuleBase
  {

    #region hold/release
    public Subject<HoldableModule> _RequestHold = new Subject<HoldableModule>();
    public IObservable<HoldableModule> RequestHold => _RequestHold;

    public Subject<HoldableModule> _RequestRelease = new Subject<HoldableModule>();
    public IObservable<HoldableModule> RequestRelease => _RequestRelease;

    public Transform _prevParent;
    ReadOnlyReactiveProperty<HoldableModule> _HoldingItem;
    public ReadOnlyReactiveProperty<HoldableModule> HoldingItem => _HoldingItem ?? (
        _HoldingItem = Observable.Merge(
            RequestHold,
            RequestRelease.Select(_ => (HoldableModule)null)
        ).ToReadOnlyReactiveProperty());

    void Awake()
    {
      interactor.OnForget
        .Subscribe(_ =>
        {
          ReleaseForce();
        });
#if DEBUG
      Inspect();
#endif
    }

    public void HoldForce(HoldableModule item)
    {
      if (!item.HoldAccepted(this)) { return; }
      _RequestHold.OnNext(item);
    }
    public bool Hold(HoldableModule item)
    {
      if (item == null) { return false; }
      if (HoldingItem.Value != null) { return false; }
      HoldForce(item);
      return true;
    }
    public void ReleaseForce()
    {
      var item = HoldingItem.Value;
      if (!item) { return; }
      if (!item.ReleaseAccepted(this)) { return; }
      _RequestRelease.OnNext(item);
    }
    public void Release(HoldableModule item)
    {
      if (HoldingItem.Value != item) { return; }
      ReleaseForce();
    }

    public void AttemptToHold()
    {
      foreach (var interactable in _interactables)
      {
        if (Hold(interactable.holdable))
        {
          return;
        }
      }
    }

    public void Grab(HoldableModule item)
    {
      _prevParent = item.rb.transform.parent;
      item.rb.transform.SetParent(transform);
      item.rb.transform.localPosition = Vector3.zero;
    }
    public void Ungrab(HoldableModule item)
    {
      item.rb.transform.SetParent(_prevParent);
      _prevParent = null;
    }

    public bool hasItem => HoldingItem.Value != null;


    public override bool isInteractable => !hasItem;

    public override void Discard()
    {
      if (hasItem)
      {
        ReleaseForce();
      }
    }

    public override void Interact()
    {
      if (!hasItem)
      {
        AttemptToHold();
      }
    }
    #endregion

#if DEBUG
    [Serializable]
    struct Inspector
    {
      public List<HoldableModule> Holdables;
      public HoldableModule Holding;
    }
    [SerializeField] Inspector inspector;

    void Inspect()
    {
      HoldingItem
          .Subscribe(x => inspector.Holding = x);
      _interactables
          .ObserveAdd()
          .Select(x => x.Value.holdable)
          .Where(x => x)
          .Subscribe(x => inspector.Holdables.Add(x));
      _interactables
          .ObserveRemove()
          .Select(x => x.Value.holdable)
          .Where(x => x)
          .Subscribe(x => inspector.Holdables.Remove(x));
    }
#endif

  }

}