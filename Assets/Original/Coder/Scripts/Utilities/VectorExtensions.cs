using UnityEngine;

namespace Utilities
{
  public static class Vector3SwizzleExtension
  {
    public static Vector3 v__(this int v)
    {
      return new Vector3(v, 0, 0);
    }
    public static Vector3 v__(this float v)
    {
      return new Vector3(v, 0, 0);
    }
    public static Vector3 _v_(this float v)
    {
      return new Vector3(0, v, 0);
    }
    public static Vector3 _V_(this float v)
    {
      return new Vector3(0, -v, 0);
    }

    public static Vector3 x_y(this Vector2 v)
    {
      return new Vector3(v.x, 0, v.y);
    }

    public static Vector3 _y_(this Vector3 v)
    {
      return new Vector3(0, v.y, 0);
    }

    public static Vector3 x_z(this Vector3 v)
    {
      return new Vector3(v.x, 0, v.z);
    }

    public static Vector3 _yz(this Vector3 v)
    {
      return new Vector3(0, v.y, v.z);
    }

    public static Vector3 Xyz(this Vector3 v)
    {
      return new Vector3(-v.x, v.y, v.z);
    }

    public static Vector3 SignX(this Vector3 v, int sign)
    {
      return new Vector3(Mathf.Abs(v.x) * sign, v.y, v.z);
    }
  }
}