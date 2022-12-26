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

    public bool useCurve;
    public AnimationCurve curve;

    float _curvedAplha;
    bool _needsCalc = true;
    public bool needsCalc => _needsCalc;
    public bool NeedsCalc<T>(T t) => _needsCalc;

    public float alpha => useCurve ? _curvedAplha : _factor;
    public override float UpdFactor()
    {
      if (latestCallTime == 0) { latestCallTime = Time.time; return 0; }

      var delta = Time.time - latestCallTime;
      if (delta < 0.001) { return alpha; }
      latestCallTime = Time.time;

      if (!_needsCalc) { return alpha; }

      SetFactor(_factor + (float)mode * delta / secDuration);

      return alpha;
    }
    public override void SetFactor(float value)
    {
      base.SetFactor(value);
      _needsCalc = ((isDecreasing && 0 < _factor) || (isIncreasing && _factor < 1));
      if (useCurve)
      {
        _curvedAplha = curve.Evaluate(_factor);
      }
    }
    public override void SetFactor0()
    {
      _factor = 0;
      if (useCurve) { _curvedAplha = curve.Evaluate(0); }
      _needsCalc = isIncreasing;
    }
    public override void SetFactor1()
    {
      _factor = 1;
      if (useCurve) { _curvedAplha = curve.Evaluate(1); }
      _needsCalc = isDecreasing;
    }

    [SerializeField] public float secDuration;

    [SerializeField] ReactiveProperty<Mode> _mode = new ReactiveProperty<Mode>(Mode.Decrease);
    public Mode mode
    {
      get { return _mode.Value; }
      set
      {
        _needsCalc = true;
        if (mode != value && latestCallTime != 0)
        {
          latestCallTime = Time.time;
        }
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

    public void SetAsIncrease(bool b)
    {
      mode = b ? Mode.Increase : Mode.Decrease;
    }
    public void SetAsIncrease() { mode = Mode.Increase; }
    public void SetAsDecrease() { mode = Mode.Decrease; }
  }
}