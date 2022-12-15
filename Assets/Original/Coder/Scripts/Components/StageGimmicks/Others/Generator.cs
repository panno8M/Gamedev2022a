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
    SafetyTrigger _safetyTrigger;
    [SerializeField]
    MessageDispatcher _OnPutKandelaar = new MessageDispatcher(MessageKind.Power);

    EzLerp powerProgress = new EzLerp(3);

    protected override void Blueprint()
    {
      throw new NotImplementedException();
    }
    void Start()
    {
      _collider = GetComponent<Collider>();
      _collider.isTrigger = true;
      _safetyTrigger = GetComponent<SafetyTrigger>();

      _OnPutKandelaar.message.intensity = powerProgress;

      _safetyTrigger.OnEnter
        .Subscribe(trigger =>
        {
          if (trigger.CompareTag(Tag.Kandelaar.GetName()))
          {
            powerProgress.SetAsIncrease();
          }
        }).AddTo(this);
      _safetyTrigger.OnExit
        .Subscribe(trigger =>
        {
          if (trigger.CompareTag(Tag.Kandelaar.GetName()))
          {
            powerProgress.SetAsDecrease();
          }
        }).AddTo(this);

      this.FixedUpdateAsObservable()
          .Subscribe(_ =>
          {
            if (powerProgress.needsCalc)
            {
              _OnPutKandelaar.Dispatch();
            }
          });
    }

    void OnDrawGizmos()
    {
      _OnPutKandelaar.DrawArrow(transform);
    }
  }
}