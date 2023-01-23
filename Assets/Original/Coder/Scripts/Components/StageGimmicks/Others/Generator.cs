#if UNITY_EDITOR
// #define DEBUG_GENERATOR
#endif

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
    [SerializeField] Transform snapPoint;
    [SerializeField] ParticleSystem drainParticle;

    [SerializeField]
    EzLerp powerProgress = new EzLerp(3);

    Kandelaar kandelaar;

#if DEBUG_GENERATOR
    [Header("Debug")]
#endif

#if DEBUG_GENERATOR
    [SerializeField]
#endif
    bool generating;

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
          kandelaar = trigger.GetComponent<Kandelaar>();
        }).AddTo(this);

      _trigger.OnStay.Where(_ => kandelaar).Subscribe(_ =>
      {
        for (int i = 0; i < _trigger.others.Count; i++)
        {
          if (_trigger.others[i].gameObject != kandelaar.gameObject) { continue; }
          if (!kandelaar.holdable.owner)
          { BeginGen(); }
          else
          { EndGen(); }
        }
      });
      _trigger.OnExit
        .Where(_trigger => kandelaar && _trigger.gameObject == kandelaar.gameObject)
        .Subscribe(_ =>
        {
          EndGen();
          kandelaar = null;
        }).AddTo(this);

      this.FixedUpdateAsObservable()
          .Where(powerProgress.isNeedsCalc)
          .Subscribe(_ =>
          {
            powerProgress.UpdFactor();
            _OnPutKandelaar.Supply(powerProgress);
          });
    }

    void BeginGen()
    {
      if (generating) { return; }
      powerProgress.SetAsIncrease();
      kandelaar.supply.isBeingAbsorbed = true;
      drainParticle.Play();
      kandelaar.transform.position = snapPoint.position;
      generating = true;
    }
    void EndGen()
    {
      if (!generating) { return; }
      powerProgress.SetAsDecrease();
      drainParticle.Stop();
      kandelaar.supply.isBeingAbsorbed = false;
      generating = false;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
      _OnPutKandelaar.DrawArrow(transform, nameof(_OnPutKandelaar));
    }
#endif
  }
}