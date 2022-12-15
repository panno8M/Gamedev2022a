using UnityEngine;

namespace Assembly.GameSystem
{
  public abstract class DiBehavior : MonoBehaviour
  {
    protected virtual void Blueprint() { }

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
}