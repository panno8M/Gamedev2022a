using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components.Actors
{
  public enum Axis2Direction { Left = -1, Right = 1 }
  public class A2Dir
  {
    public Axis2Direction current;
    public static explicit operator Vector3(A2Dir x) => new Vector3((int)x.current, 0, 0);
    public static implicit operator Axis2Direction(A2Dir x) => x.current;

    public Axis2Direction CalcNewDirection(int v)
    {
      return (v == -1 || v == 1) ? (Axis2Direction)v : current;
    }
    public void Clear()
    {
      current = Axis2Direction.Right;
    }
    public A2Dir()
    {
      Clear();
    }

    public Vector3 FollowX(Vector3 v)
    {
      return new Vector3(v.x * (int)current, v.y, v.z);
    }
    public Vector3 SignX(Vector3 v)
    {
      return new Vector3(Mathf.Abs(v.x) * (int)current, v.y, v.z);
    }
  }
}