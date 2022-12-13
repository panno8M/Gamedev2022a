using System;
using UnityEngine;
using UniRx;

namespace Utilities
{

  [System.Serializable]
  public class EzLerp
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

    [SerializeField][Range(0f, 1f)] float _basisAlpha;
    float _curvedAplha;
    bool _needsCalc = true;
    public bool needsCalc => _needsCalc;

    public float alpha
    {
      get { return useCurve ? _curvedAplha : _basisAlpha; }
    }
    public float BasisAlpha
    {
      get { return _basisAlpha; }
      set
      {
        _basisAlpha = value;
        _needsCalc = true;
        if (useCurve)
        {
          _curvedAplha = curve.Evaluate(_basisAlpha);
        }
      }
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

    public float CalcAlpha()
    {
      if (!_needsCalc) { return alpha; }

      var delta = Time.time - latestCallTime;
      if (delta < 0.001) { return alpha; }

      latestCallTime = Time.time;
      BasisAlpha = Mathf.Clamp01(_basisAlpha + (float)mode * delta / secDuration);

      if ((isIncreasing && _basisAlpha == 1)
       || (isDecreasing && _basisAlpha == 0))
      { _needsCalc = false; }

      return alpha;
    }

    public static implicit operator float(EzLerp ezlerp)
    {
      return ezlerp.CalcAlpha();
    }

    public void SetAsIncrease(bool b)
    {
      mode = b ? Mode.Increase : Mode.Decrease;
    }
    public void SetAsIncrease() { mode = Mode.Increase; }
    public void SetAsDecrease() { mode = Mode.Decrease; }

    #region Mixers
    public float Mix(float from, float to)
    {
      return Mathf.Lerp(from, to, this);
    }
    public float Add(float from, float to)
    {
      return from + (to * this);
    }

    public Color Mix(Color from, Color to)
    {
      return Color.Lerp(from, to, this);
    }
    public Color Add(Color from, Color to)
    {
      return from + (to * this);
    }

    public Vector3 Mix(Vector3 from, Vector3 to)
    {
      return Vector3.Lerp(from, to, this);
    }
    public Vector3 Add(Vector3 from, Vector3 to)
    {
      return from + (to * this);
    }
    public Vector3 AddX(Vector3 from, float to)
    {
      return new Vector3(from.x + (to * this), from.y, from.z);
    }
    #endregion // Mixers
  }
}