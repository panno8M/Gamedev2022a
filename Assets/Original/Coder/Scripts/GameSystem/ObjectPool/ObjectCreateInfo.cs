using UnityEngine;

namespace Assembly.GameSystem.ObjectPool
{
  public class ObjectCreateInfo
  {
    static readonly ObjectCreateInfo _none = new ObjectCreateInfo();
    public static ObjectCreateInfo None => _none;

    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;
    public Transform parent;
    public Transform offset;
    public object userData;
  }
}