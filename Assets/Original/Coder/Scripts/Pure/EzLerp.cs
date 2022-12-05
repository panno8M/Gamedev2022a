using UnityEngine;

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
  [SerializeField] public float secDuration;
  public Mode mode;

  public static implicit operator float(EzLerp ezlerp)
  {
    var delta = Time.time - ezlerp.latestCallTime;
    if (delta < 0.001) { return ezlerp._alpha; }
    ezlerp.latestCallTime = Time.time;
    ezlerp._alpha = Mathf.Clamp01(ezlerp._alpha + (float)ezlerp.mode * delta / ezlerp.secDuration);
    return ezlerp._alpha;
  }

  public void SetAsIncrease(bool b)
  {
    mode = b ? Mode.Increase : Mode.Decrease;
  }

  public float Lerp(float from, float to)
  {
    return Mathf.Lerp(from, to, this);
  }
}
