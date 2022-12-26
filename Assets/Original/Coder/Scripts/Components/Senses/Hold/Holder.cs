using System;
using UniRx;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class Holder : MonoBehaviour
  {
    SafetyTrigger _safetyTrigger;
    [SerializeField] List<Holdable> _accessibles = new List<Holdable>();

    Subject<Holdable> _RequestHold = new Subject<Holdable>();
    Subject<Holdable> _RequestRelease = new Subject<Holdable>();
    ReactiveProperty<Holdable> _HoldingItem = new ReactiveProperty<Holdable>();

    Transform _previousParentOfHoldingItem;

    public IObservable<Holdable> RequestHold => _RequestHold;
    public IObservable<Holdable> RequestRelease => _RequestRelease;
    public IObservable<Holdable> HoldingItem => _HoldingItem;
    public Holdable holdingItem => _HoldingItem.Value;
    public bool hasItem => holdingItem != null;
    public List<Holdable> accessibles => _accessibles;

    void Awake()
    {
      GetComponent<Collider>().isTrigger = true;
      _safetyTrigger = GetComponent<SafetyTrigger>();

      _safetyTrigger.OnEnter.Subscribe(AddToAccessibles);
      _safetyTrigger.OnExit.Subscribe(RemoveFromAccessibles);

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

      _HoldingItem.Value = item;
      _RequestHold.OnNext(item);

      _previousParentOfHoldingItem = item.rb.transform.parent;
      item.rb.transform.SetParent(transform);
      item.rb.transform.localPosition = Vector3.zero;
    }
    public bool Hold(Holdable item)
    {
      if (item == null) { return false; }
      if (hasItem) { return false; }
      HoldForce(item);
      return true;
    }
    public void ReleaseForce()
    {
      if (!holdingItem) { return; }
      if (!holdingItem.ReleaseAccepted(this)) { return; }

      holdingItem.rb.transform.SetParent(_previousParentOfHoldingItem);

      _RequestRelease.OnNext(holdingItem);
      _HoldingItem.Value = null;
    }
    public void Release(Holdable item)
    {
      if (item != holdingItem) { return; }
      ReleaseForce();
    }

    public bool AttemptToHold()
    {
      if (_accessibles.Count == 0) { return false; }
      if (_accessibles.Count == 1) { Hold(_accessibles[0]); return true; }
      float closestDistance = Mathf.Infinity;
      int closestIndex = 2;
      for (int i = 1; i != _accessibles.Count; i++)
      {
        float distance = (_accessibles[i].transform.position - transform.position).sqrMagnitude;
        if (closestDistance > distance)
        {
          closestDistance = distance;
          closestIndex = i;
        }
      }
      Hold(_accessibles[closestIndex]);
      return true;
    }
  }

}