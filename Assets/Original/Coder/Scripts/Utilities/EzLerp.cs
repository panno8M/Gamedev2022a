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

    float latestCallTime;

    public float localTimeScale = 1f;
    public bool useCurve;
    public AnimationCurve curve;

    float _curvedAplha;
    ReactiveProperty<bool> _needsCalc = new ReactiveProperty<bool>(true);
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
    public bool needsCalc
    {
      get { return _needsCalc.Value; }
      private set { _needsCalc.Value = value; }
    }
    public IObservable<bool> NeedsCalc => _needsCalc;
    public bool isNeedsCalc<T>(T t) => needsCalc;

    public float alpha => useCurve ? _curvedAplha : _factor;
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

      SetFactor(_factor + (float)mode * delta * localTimeScale / secDuration);

      return alpha;
    }
    public override void SetFactor(float value)
    {
      base.SetFactor(value);
      decideCalculationNeeds();
      if (useCurve)
      {
        _curvedAplha = curve.Evaluate(_factor);
      }
    }
    public override void SetFactor0()
    {
      _factor = 0;
      if (useCurve) { _curvedAplha = curve.Evaluate(0); }
      needsCalc = isIncreasing;
    }
    public override void SetFactor1()
    {
      _factor = 1;
      if (useCurve) { _curvedAplha = curve.Evaluate(1); }
      needsCalc = isDecreasing;
    }

    [SerializeField] public float secDuration;

    [SerializeField] ReactiveProperty<Mode> _mode = new ReactiveProperty<Mode>(Mode.Decrease);
    public Mode mode
    {
      get { return _mode.Value; }
      set
      {
        if (mode == value) { return; }
        needsCalc = true;
        // latestCallTime == 0、つまりUpdateが呼ばれる前の初期化時ではTime.timeを呼ぶと例外が投げられる
        if (latestCallTime != 0) { latestCallTime = Time.time; }
        _mode.Value = value;
      }
    }

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

    public void SetAsIncrease<T>(T t) { SetAsIncrease(); }
    public void SetAsDecrease<T>(T t) { SetAsDecrease(); }
    public void FlipMode<T>(T t) { FlipMode(); }

  }
}