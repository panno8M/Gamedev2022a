using System;
using UnityEngine;
using UniRx;

namespace Utilities
{

  [Serializable]
  public class EzLerp : MixFactor
  {
    public enum Mode { Increase = 1, Decrease = -1 }

    public EzLerp(float secDuration, Mode mode = Mode.Increase)
    {
      this.secDuration = secDuration;
      this.mode = mode;
    }
    public EzLerp()
    {
      this.secDuration = 1;
      this.mode = Mode.Increase;
    }

    float latestCallTime;

    public bool useCurve;
    public AnimationCurve curve;

    float _curvedAplha;
    bool _needsCalc = true;
    public bool needsCalc => _needsCalc;

    public float alpha => useCurve ? _curvedAplha : _factor;
    public override float factor
    {
      get
      {
        if (latestCallTime == 0) { latestCallTime = Time.time; return 0; }
        var delta = Time.time - latestCallTime;
        if (delta < 0.001) { return alpha; }
        latestCallTime = Time.time;
        if (!_needsCalc) { return alpha; }

        factor = _factor + (float)mode * delta / secDuration;

        return alpha;
      }
      set
      {
        base.factor = value;
        _needsCalc = ((isDecreasing && 0 < _factor) || (isIncreasing && _factor < 1));
        if (useCurve)
        {
          _curvedAplha = curve.Evaluate(_factor);
        }
      }
    }
    public override void Set0()
    {
      _factor = 0;
      if (useCurve) { _curvedAplha = curve.Evaluate(0); }
      _needsCalc = isIncreasing;
    }
    public override void Set1()
    {
      _factor = 1;
      if (useCurve) { _curvedAplha = curve.Evaluate(1); }
      _needsCalc = isDecreasing;
    }

    [SerializeField] public float secDuration;

    [SerializeField] ReactiveProperty<Mode> _mode = new ReactiveProperty<Mode>();
    public Mode mode
    {
      get { return _mode.Value; }
      set
      {
        _needsCalc = true;
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