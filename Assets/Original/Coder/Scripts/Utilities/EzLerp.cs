using UnityEngine;
namespace Utilities
{

  [System.Serializable]
  public class EzLerp
  {
    public enum Mode { Increase = 1, Decrease = -1 }

    public EzLerp(float secDuration)
    {
      this.secDuration = secDuration;
      this.mode = Mode.Increase;
    }
    public EzLerp()
    {
      this.secDuration = 1;
      this.mode = Mode.Increase;
    }

    float latestCallTime;
    [Range(0f, 1f)] float _alpha;
    float _curvedAplha;
    [SerializeField] public float secDuration;
    float alpha => useCurve ? _curvedAplha : _alpha;
    public Mode mode { get; set; }
    public bool useCurve;
    public AnimationCurve curve;

    public static implicit operator float(EzLerp ezlerp)
    {
      var delta = Time.time - ezlerp.latestCallTime;
      if (delta < 0.001) { return ezlerp.alpha; }
      ezlerp.latestCallTime = Time.time;
      ezlerp._alpha = Mathf.Clamp01(ezlerp._alpha + (float)ezlerp.mode * delta / ezlerp.secDuration);
      if (ezlerp.useCurve)
      {
        ezlerp._curvedAplha = ezlerp.curve.Evaluate(ezlerp._alpha);
        return ezlerp._curvedAplha;
      }
      return ezlerp._alpha;
    }

    public void SetAsIncrease(bool b)
    {
      mode = b ? Mode.Increase : Mode.Decrease;
    }

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