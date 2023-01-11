#if UNITY_EDITOR
// #define DEBUG_EZ_LERP
#endif

using System;
using UnityEngine;
using UniRx;

namespace Utilities
{

  [Serializable]
  public class EzLerp : MixFactor
  {
    public enum Mode { Increase = 1, Decrease = -1 }

    public EzLerp(float secDuration, Mode mode = Mode.Decrease)
    {
      this.secDuration = secDuration;
      this.mode = mode;
    }
    public EzLerp()
    {
      this.secDuration = 1;
    }


    public float secDuration;
    public float localTimeScale = 1f;
    public bool useCurve;
    public AnimationCurve curve;

#if DEBUG_EZ_LERP
    [SerializeField]
#endif
    ReactiveProperty<Mode> _mode = new ReactiveProperty<Mode>(Mode.Decrease);

    float latestCallTime;
    float _curvedAplha;
    ReactiveProperty<bool> _needsCalc = new ReactiveProperty<bool>(true);

    public Mode mode
    {
      get => _mode.Value;
      set
      {
        if (mode == value) { return; }
        needsCalc = true;
        // latestCallTime == 0、つまりUpdateが呼ばれる前の初期化時ではTime.timeを呼ぶと例外が投げられる
        if (latestCallTime != 0) { latestCallTime = Time.time; }
        _mode.Value = value;
      }
    }
    public bool needsCalc
    {
      get => _needsCalc.Value;
      private set => _needsCalc.Value = value;
    }
    public float alpha => useCurve ? _curvedAplha : _factor;

    public IObservable<bool> NeedsCalc => _needsCalc;

    void decideCalculationNeeds()
    {
      switch (_mode.Value)
      {
        case Mode.Decrease:
          needsCalc = 0 < _factor;
          return;
        case Mode.Increase:
          needsCalc = _factor < 1;
          return;
      }
    }
    public override float UpdFactor()
    {
      if (latestCallTime == 0)
      {
        latestCallTime = Time.time;
        decideCalculationNeeds();
        return alpha;
      }

      var delta = (Time.time - latestCallTime);
      if (delta == 0) { return alpha; }
      latestCallTime = Time.time;

      if (!needsCalc) { return alpha; }

      _SetFactor(_factor + (float)mode * delta * localTimeScale / secDuration);

      return alpha;
    }
    void _SetFactor(float value)
    {
      base.SetFactor(value);
      decideCalculationNeeds();
      if (useCurve)
      {
        _curvedAplha = curve.Evaluate(_factor);
      }
    }
    public override void SetFactor(float value)
    {
      base.SetFactor(value);
      if (useCurve) { _curvedAplha = curve.Evaluate(_factor); }
      needsCalc = true;
    }
    public override void SetFactor0() => SetFactor(0);
    public override void SetFactor1() => SetFactor(1);

    public IObservable<Mode> OnModeChanged => _mode;
    public IObservable<Unit> OnIncrease => _mode
      .Where(x => x == Mode.Increase)
      .AsUnitObservable();
    public IObservable<Unit> OnDecrease => _mode
      .Where(x => x == Mode.Decrease)
      .AsUnitObservable();

    public bool isIncreasing => mode == Mode.Increase;
    public bool isDecreasing => mode == Mode.Decrease;

    public float elapsedSeconds => alpha * secDuration;

    public void SetMode(bool increase)
    {
      mode = increase ? Mode.Increase : Mode.Decrease;
    }
    public void SetAsIncrease() { mode = Mode.Increase; }
    public void SetAsDecrease() { mode = Mode.Decrease; }
    public void FlipMode() { mode = (Mode)(-(int)mode); }
  }

  public static class EzLerpReactiveXOperationExtensions
  {
    public static bool isNeedsCalc<T>(this EzLerp ezLerp, T t)
      => ezLerp.needsCalc;
    public static void SetAsIncrease<T>(this EzLerp ezLerp, T t)
      => ezLerp.SetAsIncrease();
    public static void SetAsDecrease<T>(this EzLerp ezLerp, T t)
      => ezLerp.SetAsDecrease();
    public static void FlipMode<T>(this EzLerp ezLerp, T t)
      => ezLerp.FlipMode();
  }
}