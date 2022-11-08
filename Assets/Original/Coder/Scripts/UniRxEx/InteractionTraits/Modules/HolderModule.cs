using System;
using UniRx.Ex.InteractionTraits.Core;

namespace UniRx.Ex.InteractionTraits
{
  public class HolderModule : InteractorModuleBase
  {

    #region hold/release
    public Subject<HoldableModule> _RequestHold = new Subject<HoldableModule>();
    public IObservable<HoldableModule> RequestHold => _RequestHold;

    public Subject<HoldableModule> _RequestRelease = new Subject<HoldableModule>();
    public IObservable<HoldableModule> RequestRelease => _RequestRelease;

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
    }

    public void HoldForce(HoldableModule item)
    {
      _RequestHold.OnNext(item);
      HoldingItem.Value.HoldAccepted(this);
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
      if (item)
      {
        item.ReleaseAccepted(this);
        _RequestRelease.OnNext(item);
      }
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

    public bool hasItem => HoldingItem.Value != null;

    public override void Interact()
    {
      if (hasItem)
      {
        ReleaseForce();
      }
      else
      {
        AttemptToHold();
      }
    }
    #endregion


  }

}