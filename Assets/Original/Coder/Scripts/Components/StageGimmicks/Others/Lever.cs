using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem.Message;
using Assembly.GameSystem.ObjectPool;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class Lever : MonoBehaviour, IInteractable
  {
    [SerializeField]
    ReactiveProperty<bool> _LeverSwitch = new ReactiveProperty<bool>();
    Interactable interactable;

    [SerializeField] Transform _leverRoot;
    [SerializeField] Quaternion _leverRotateFrom;
    [SerializeField] Quaternion _leverRotateTo;
    [SerializeField] MessageDispatcher _OnSwitch = new MessageDispatcher();
    [SerializeField] EzLerp leverProgress = new EzLerp(.5f);

    public IObservable<bool> OnLeverSwitch => _LeverSwitch;
    public bool leverSwitch
    {
      get { return _LeverSwitch.Value; }
      private set { _LeverSwitch.Value = value; }
    }
    void ToggleLever() => leverSwitch = !leverSwitch;

    void Start()
    {
      interactable = GetComponent<Interactable>();
      _OnSwitch.message.intensity = leverProgress;
      if (leverSwitch) { leverProgress.SetAsIncrease(); leverProgress.SetFactor1(); }
      OnLeverSwitch.Subscribe(leverProgress.SetMode);

      this.FixedUpdateAsObservable()
        .Where(leverProgress.isNeedsCalc)
        .Subscribe(_ =>
        {
          _leverRoot.localRotation = leverProgress.UpdMix(_leverRotateFrom, _leverRotateTo);
          _OnSwitch.Dispatch();
        });
    }

    public void OnInteract()
    {
      ToggleLever();
    }

    void OnDrawGizmos()
    {
      _OnSwitch.DrawArrow(transform, nameof(_OnSwitch));
    }
  }
}