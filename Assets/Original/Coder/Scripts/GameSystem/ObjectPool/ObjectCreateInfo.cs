using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.GameSystem.ObjectPool
{
  public class ObjectCreateInfo
  {
    static readonly ObjectCreateInfo _none = new ObjectCreateInfo();
    public static ObjectCreateInfo None => _none;

    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;
    public object userData;
  }
}