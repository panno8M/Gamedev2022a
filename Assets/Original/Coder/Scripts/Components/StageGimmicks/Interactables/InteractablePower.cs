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
  public abstract class InteractablePower : DiBehavior, IInteractable
  {
    [SerializeField]
    protected ReactiveProperty<bool> _Switch = new ReactiveProperty<bool>();
    protected Interactable interactable;

    [SerializeField]
    protected PowerSupplier _OnLever = new PowerSupplier();
    [SerializeField]
    protected EzLerp DispatchProgress = new EzLerp(.5f);

    public IObservable<bool> OnSwitch => _Switch;
    public bool isOn
    {
      get { return _Switch.Value; }
      private set { _Switch.Value = value; }
    }
    protected void Toggle() => isOn = !isOn;

    protected void Start() => Initialize();
    protected override void Blueprint()
    {
      interactable = GetComponent<Interactable>();
      if (isOn) { DispatchProgress.SetAsIncrease(); DispatchProgress.SetFactor1(); }
      OnSwitch.Subscribe(DispatchProgress.SetMode);

      this.FixedUpdateAsObservable()
        .Where(DispatchProgress.isNeedsCalc)
        .Subscribe(_ =>
        {
          DispatchProgress.UpdFactor();
          OnProgressUpdate(DispatchProgress);
          _OnLever.Supply(DispatchProgress);
        });
    }

    protected abstract void OnProgressUpdate(MixFactor factor);

    public void OnInteract() => Toggle();

#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
      _OnLever.DrawArrow(transform, nameof(_OnLever));
    }
#endif
  }
}