using UnityEngine;

namespace Assembly.GameSystem.ObjectPool
{
  public class TransformInfo
  {
    public Transform reference;
    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;
    Transform _parent;

    public virtual Transform parent
    {
      get => _parent;
      set => _parent = value;
    }
    public virtual void Infuse(Transform instance, TransformUsage usage)
    {
      Transform parent = this.parent;
      if (instance.parent != parent)
      { instance.SetParent(parent, false); }

      Vector3 position;
      Quaternion rotation;
      switch (usage.referenceUsage)
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

      switch (usage.spawnSpace)
      {
        case eopSpawnSpace.Parent:
          instance.localRotation = rotation;
          instance.localPosition = position;
          break;
        case eopSpawnSpace.Global:
          if (instance.parent == null)
          {
            instance.localRotation = rotation;
            instance.localPosition = position;
          }
          else
          {
            instance.rotation = rotation;
            instance.position = position;
          }
          break;
      }
    }
  }
}