using UnityEngine;

namespace Assembly.GameSystem.ObjectPool
{
  public enum eopSpawnSpace { Parent, Global }
  public enum eopReferenceUsage { Discard, Local, Global }
  public abstract class ObjectCreateInfo<T>
    where T : DiBehavior, IPoolCollectable
  {
    public eopSpawnSpace spawnSpace = eopSpawnSpace.Parent;
    public eopReferenceUsage referenceUsage = eopReferenceUsage.Discard;
    public Transform reference;
    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;
    Transform _parent;

    public virtual Transform parent
    {
      get => _parent;
      set => _parent = value;
    }
    public virtual void Infuse(T instance)
    {
      Transform parent = this.parent;
      if (instance.transform.parent != parent)
      { instance.transform.SetParent(parent, false); }

      Vector3 position;
      Quaternion rotation;
      switch (referenceUsage)
      {
        case eopReferenceUsage.Local:
          position = reference?.localPosition ?? this.position;
          rotation = reference?.localRotation ?? this.rotation;
          break;
        case eopReferenceUsage.Global:
          position = reference?.position ?? this.position;
          rotation = reference?.rotation ?? this.rotation;
          break;
        case eopReferenceUsage.Discard:
        default:
          position = this.position;
          rotation = this.rotation;
          break;
      }

      switch (spawnSpace)
      {
        case eopSpawnSpace.Parent:
          instance.transform.localRotation = rotation;
          instance.transform.localPosition = position;
          break;
        case eopSpawnSpace.Global:
          if (instance.transform.parent == null)
          {
            instance.transform.localRotation = rotation;
            instance.transform.localPosition = position;
          }
          else
          {
            instance.transform.rotation = rotation;
            instance.transform.position = position;
          }
          break;
      }
    }
  }
}