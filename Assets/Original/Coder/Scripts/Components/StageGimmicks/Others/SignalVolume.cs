using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Assembly.GameSystem.Message;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class SignalVolume : MonoBehaviour
  {
    SafetyTrigger _trigger;

    [SerializeField] MessageDispatcher _OnEnter = new MessageDispatcher();
    [SerializeField] EzLerp dispatchProgress = new EzLerp(1);

    [SerializeField] LayerMask layerFilter;

    void Start()
    {
      _trigger = GetComponent<SafetyTrigger>();
      _OnEnter.message.intensity = dispatchProgress;

      _trigger.OnEnter.Subscribe(other =>
      {
        if ((layerFilter & 1 << other.gameObject.layer) != 0)
        {
          dispatchProgress.SetAsIncrease();
        }
      }).AddTo(this);

      _trigger.OnExit
        .Where(_ => _trigger.others.Count == 0)
        .Subscribe(_ =>
        {
          for (int i = 0; i != _trigger.others.Count; i++)
          {
            if ((layerFilter & 1 << _trigger.others[i].gameObject.layer) != 0)
            { return; }
          }
          dispatchProgress.SetAsDecrease();
        }).AddTo(this);

      this.FixedUpdateAsObservable()
        .Where(dispatchProgress.isNeedsCalc)
        .Select(dispatchProgress.UpdFactor)
        .Subscribe(fac =>
        {
          _OnEnter.Dispatch();
        });
    }

    void Update()
    {

    }
  }
}