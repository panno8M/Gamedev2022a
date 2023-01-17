using System;
using UniRx;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class Holder : MonoBehaviour
  {
    SafetyTrigger _trigger;
    [SerializeField] List<Holdable> _accessibles = new List<Holdable>();

    Subject<Holdable> _RequestHold = new Subject<Holdable>();
    Subject<Holdable> _RequestRelease = new Subject<Holdable>();
    ReactiveProperty<Holdable> _HoldingItem = new ReactiveProperty<Holdable>();

    Transform _previousParentOfHoldingItem;

    public IObservable<Holdable> RequestHold => _RequestHold;
    public IObservable<Holdable> RequestRelease => _RequestRelease;
    public IObservable<Holdable> HoldingItem => _HoldingItem;
    public Holdable holdingItem
    {
      get => _HoldingItem.Value;
      set => _HoldingItem.Value = value;
    }
    public bool hasItem => holdingItem != null;

    void Awake()
    {
      GetComponent<Collider>().isTrigger = true;
      _trigger = GetComponent<SafetyTrigger>();

      _trigger.OnEnter.Subscribe(AddToAccessibles);
      _trigger.OnExit.Subscribe(RemoveFromAccessibles);

      void AddToAccessibles(SafetyTrigger trigger)
      {
        Holdable holdable = trigger.GetComponent<Holdable>();
        if (holdable) { _accessibles.Add(holdable); }
      }
      void RemoveFromAccessibles(SafetyTrigger trigger)
      {
        Holdable holdable = trigger.GetComponent<Holdable>();
        if (holdable) { _accessibles.Remove(holdable); }
      }
    }
    public void Forget()
    {
      ReleaseForce();
    }

    public void HoldForce(Holdable item)
    {
      if (holdingItem == item) { return; }
      if (!item.HoldAccepted(this)) { return; }
      ReleaseForce();

      holdingItem = item;
      _accessibles.Remove(item);
      _RequestHold.OnNext(item);

      _previousParentOfHoldingItem = item.rb.transform.parent;
      item.rb.transform.SetParent(transform);
      item.rb.transform.localPosition = Vector3.zero;
    }
    public bool Hold(Holdable item, bool swapItem = true)
    {
      if (item == null) { return false; }
      if (hasItem && !swapItem) { return false; }
      HoldForce(item);
      return true;
    }
    public void ReleaseForce()
    {
      if (!holdingItem) { return; }
      if (!holdingItem.ReleaseAccepted(this)) { return; }

      if (_previousParentOfHoldingItem)
      {
        holdingItem.rb.transform.SetParent(_previousParentOfHoldingItem);
      }
      else { holdingItem.rb.transform.parent = null; }

      _RequestRelease.OnNext(holdingItem);
      _accessibles.Add(holdingItem);
      holdingItem = null;
    }
    public void Release(Holdable item)
    {
      if (item != holdingItem) { return; }
      ReleaseForce();
    }
    public bool FindHoldableInAround(out Holdable result)
    {
      result = _accessibles.Count == 0 ? null : _accessibles[0];
      return _accessibles.Count != 0;
    }
  }

}