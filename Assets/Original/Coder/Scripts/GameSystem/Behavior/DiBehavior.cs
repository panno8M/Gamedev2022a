using UnityEngine;

namespace Assembly.GameSystem
{
  public abstract class DiBehavior : MonoBehaviour
  {
    bool initialized;
    public void Initialize()
    {
      if (initialized) { return; }
      Blueprint();
      initialized = true;
    }
    protected abstract void Blueprint();

    Rigidbody _rigidbody;
    Transform _transform;

    public new Rigidbody rigidbody
    {
      get
      {
        if (_rigidbody == null)
        {
          _rigidbody = GetComponent<Rigidbody>();
        }
        return _rigidbody;
      }
    }
    public new Transform transform
    {
      get
      {
        if (_transform == null)
        {
          _transform = GetComponent<Transform>();
        }
        return _transform;
      }
    }
  }
  public static class GameObjectExtensions
  {
    public static T Instantiate<T>(this GameObject prefab)
      where T : DiBehavior
    {
      T result = GameObject.Instantiate(prefab).GetComponent<T>();
      result.Initialize();
      return result;
    }

    public static T Instantiate<T>(this GameObject prefab, Transform parent)
      where T : DiBehavior
    {
      T result = prefab.Instantiate<T>();
      result.transform.SetParent(parent);
      return result;
    }
  }
}