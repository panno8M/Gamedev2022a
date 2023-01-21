using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Assembly.GameSystem.Message;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(SafetyTrigger))]
  [RequireComponent(typeof(Collider))]
  public class Generator : DiBehavior
  {
    Collider _collider;
    SafetyTrigger _trigger;
    [SerializeField]
    PowerSupplier _OnPutKandelaar = new PowerSupplier();

    [SerializeField]
    EzLerp powerProgress = new EzLerp(3);

    protected override void Blueprint()
    {
      throw new NotImplementedException();
    }
    void Start()
    {
      _collider = GetComponent<Collider>();
      _collider.isTrigger = true;
      _trigger = GetComponent<SafetyTrigger>();

      _trigger.OnEnter
        .Where(trigger => trigger.CompareTag(Tag.Kandelaar.GetName()))
        .Subscribe(trigger =>
        {
          powerProgress.SetAsIncrease();
          trigger.GetComponent<Kandelaar>().supply.isBeingAbsorbed = true;
        }).AddTo(this);
      _trigger.OnExit
        .Where(trigger => trigger.CompareTag(Tag.Kandelaar.GetName()))
        .Subscribe(trigger =>
        {
          powerProgress.SetAsDecrease();
          trigger.GetComponent<Kandelaar>().supply.isBeingAbsorbed = false;
        }).AddTo(this);

      this.FixedUpdateAsObservable()
          .Where(powerProgress.isNeedsCalc)
          .Subscribe(_ =>
          {
            powerProgress.UpdFactor();
            _OnPutKandelaar.Supply(powerProgress);
          });
    }

    void OnDrawGizmos()
    {
      _OnPutKandelaar.DrawArrow(transform, nameof(_OnPutKandelaar));
    }
  }
}