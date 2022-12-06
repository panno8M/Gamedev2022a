using System;
using UniRx.Ex.InteractionTraits.Core;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Ex.InteractionTraits
{
  public class HolderModule : InteractorModuleBase
  {

    #region hold/release
    public Subject<HoldableModule> _RequestHold = new Subject<HoldableModule>();
    public IObservable<HoldableModule> RequestHold => _RequestHold;

    public Subject<HoldableModule> _RequestRelease = new Subject<HoldableModule>();
    public IObservable<HoldableModule> RequestRelease => _RequestRelease;

    Transform _previousParentOfGrabingItem;
    bool _isGrabing;

    ReactiveProperty<HoldableModule> _HoldingItem = new ReactiveProperty<HoldableModule>();
    public IObservable<HoldableModule> HoldingItem => _HoldingItem;
    public HoldableModule holdingItem => _HoldingItem.Value;
    public bool hasItem => holdingItem != null;


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
      _HoldingItem.Value = item;
      _RequestHold.OnNext(item);
    }
    public bool Hold(HoldableModule item)
    {
      if (item == null) { return false; }
      if (hasItem) { return false; }
      HoldForce(item);
      return true;
    }
    public void ReleaseForce()
    {
      var item = holdingItem;
      if (!item) { return; }
      if (!item.ReleaseAccepted(this)) { return; }
      Ungrab(item);
      _RequestRelease.OnNext(item);
      _HoldingItem.Value = null;
    }
    public void Release(HoldableModule item)
    {
      if (holdingItem != item) { return; }
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

    /// <summary>
    /// <paramref name="item"/>と<paramref name="holdBy"/>との間に一時的に親子関係を結ばせることで掴む動作を表現する。
    /// <see cref="HolderModule.RequestHold"/>ストリーム内で使われる想定。
    /// <see cref="Ungrab(HoldableModule)"/>で解除。
    /// </summary>
    /// <param name="item">対象のHoldableModule。親の指定にはrbメンバのトランスフォームが使われる</param>
    /// <param name="holdBy">一時的に親にするトランスフォーム。</param>
    /// <param name="offset"></param>
    public void Grab(HoldableModule item, Transform holdBy, Vector3 offset, bool replaceItem = true)
    {
      if (_isGrabing)
      {
        if (replaceItem) { Ungrab(item); }
        else { return; }
      }
      _previousParentOfGrabingItem = item.rb.transform.parent;
      _isGrabing = true;
      item.rb.transform.SetParent(holdBy);
      item.rb.transform.localPosition = offset;
    }
    /// <summary>
    /// <paramref name="item"/>と<see cref="transform"/>との間に一時的に親子関係を結ばせることで掴む動作を表現する。
    /// <see cref="RequestHold"/>ストリーム内で使われる想定。
    /// <see cref="Ungrab(HoldableModule)"/>で解除。
    /// </summary>
    /// <param name="item">対象のHoldableModule。親の指定にはrbメンバのトランスフォームが使われる</param>
    public void Grab(HoldableModule item, bool replaceItem = true) { Grab(item, transform, Vector3.zero, replaceItem); }
    /// <summary>
    /// <see cref="Grab(HoldableModule)"/>で結んだ親子関係を解除する。
    /// <see cref="ReleaseForce()"/>,<see cref="Release(HoldableModule)"/> 実行時に暗黙的に呼ばれる。
    /// <see cref="RequestRelease"/>ストリーム内で使われる想定。
    /// </summary>
    /// <param name="item"></param>
    public void Grab(HoldableModule item) { Grab(item, transform, Vector3.zero, true); }

    public void Ungrab(HoldableModule item)
    {
      if (!_isGrabing) { return; }
      item.rb.transform.SetParent(_previousParentOfGrabingItem);
      _isGrabing = false;
    }

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
      public HoldableModule Holding;
    }
    [SerializeField] Inspector inspector;

    void Inspect()
    {
      HoldingItem
          .Subscribe(x => inspector.Holding = x);
    }
#endif

  }

}