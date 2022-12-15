using System;
using UnityEngine;

namespace Utilities
{
  [Serializable]
  public class MixFactor
  {
    [SerializeField][Range(0f, 1f)] protected float _factor;
    public virtual float factor
    {
      get { return _factor; }
      set { _factor = Mathf.Clamp01(value); }
    }
    public virtual void Set0() { _factor = 0; }
    public virtual void Set1() { _factor = 1; }

    public float Mix(float from, float to)
    {
      return Mathf.Lerp(from, to, factor);
    }
    public float Add(float from, float to)
    {
      return from + (to * factor);
    }

    public Color Mix(Color from, Color to)
    {
      return Color.Lerp(from, to, factor);
    }
    public Color Add(Color from, Color to)
    {
      return from + (to * factor);
    }

    public Vector3 Mix(Vector3 from, Vector3 to)
    {
      return Vector3.Lerp(from, to, factor);
    }
    public Vector3 Add(Vector3 from, Vector3 to)
    {
      return from + (to * factor);
    }
    public Vector3 AddX(Vector3 from, float to)
    {
      return new Vector3(from.x + (to * factor), from.y, from.z);
    }
  }
}