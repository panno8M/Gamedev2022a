#if UNITY_EDITOR
// #define DEBUG_MIX_FACTOR
#endif

using System;
using UnityEngine;

namespace Utilities
{
  [Serializable]
  public class MixFactor
  {
    public MixFactor() { }
    public MixFactor(float f)
    {
      _factor = f;
    }
#if DEBUG_MIX_FACTOR
    [SerializeField][Range(0f, 1f)]
#endif
    protected float _factor;

    public float Invpeek() => 1 - PeekFactor();
    public float Invpeek(bool b) => b ? Invpeek() : PeekFactor();

    public virtual float UpdFactor() { return _factor; }
    public virtual float PeekFactor() { return _factor; }
    public float UpdFactor<T>(T t) => UpdFactor();
    public float PeekFactor<T>(T t) => PeekFactor();
    public virtual void SetFactor(float value) { _factor = Mathf.Clamp01(value); }
    public virtual void SetFactor0() { _factor = 0; }
    public virtual void SetFactor1() { _factor = 1; }
    public virtual void AddFactor(float value) => SetFactor(PeekFactor() + value);

    public bool isFactor0 => PeekFactor() == 0;
    public bool isFactor1 => PeekFactor() == 1;

    public float UpdMix(float from, float to)
    {
      return Mathf.Lerp(from, to, UpdFactor());
    }
    public float UpdAdd(float from, float to)
    {
      return from + (to * UpdFactor());
    }
    public float Mix(float from, float to)
    {
      return Mathf.Lerp(from, to, PeekFactor());
    }
    public float Add(float from, float to)
    {
      return from + (to * PeekFactor());
    }

    public Color UpdMix(Color from, Color to)
    {
      return Color.Lerp(from, to, UpdFactor());
    }
    public Color UpdAdd(Color from, Color to)
    {
      return from + (to * UpdFactor());
    }
    public Color Mix(Color from, Color to)
    {
      return Color.Lerp(from, to, PeekFactor());
    }
    public Color Add(Color from, Color to)
    {
      return from + (to * PeekFactor());
    }

    public Vector3 UpdMix(Vector3 from, Vector3 to)
    {
      return Vector3.Lerp(from, to, UpdFactor());
    }
    public Vector3 UpdAdd(Vector3 from, Vector3 to)
    {
      return from + (to * UpdFactor());
    }
    public Vector3 Mix(Vector3 from, Vector3 to)
    {
      return Vector3.Lerp(from, to, PeekFactor());
    }
    public Vector3 Add(Vector3 from, Vector3 to)
    {
      return from + (to * PeekFactor());
    }

    public Quaternion UpdMix(Quaternion from, Quaternion to)
    {
      return Quaternion.Slerp(from, to, UpdFactor());
    }
    public Quaternion Mix(Quaternion from, Quaternion to)
    {
      return Quaternion.Slerp(from, to, PeekFactor());
    }

    public Vector3 UpdAddX(Vector3 from, float to)
    {
      return new Vector3(from.x + (to * UpdFactor()), from.y, from.z);
    }
    public Vector3 AddX(Vector3 from, float to)
    {
      return new Vector3(from.x + (to * PeekFactor()), from.y, from.z);
    }
  }
}