using UnityEngine;
using Assembly.GameSystem;


namespace Assembly.Params
{
  [CreateAssetMenu]
  public class DroneParam : ScriptableObject
  {
    public Settings settings = new Settings
    {
      moveSpeedFactor = 1,
      rotateSpeedFactor = 50,
      gravityScale = -3,
    };
    public Constraints constraints = new Constraints
    {
      closestDistance = 3,
      furthestDistance = 4,
      relativeHeightFromGround = 1,
      maximumHullTiltAngle = 30,
    };

    [System.Serializable]
    public struct Settings
    {
      public float rotateSpeedFactor;
      public float moveSpeedFactor;
      public float gravityScale;

      public float rotateSpeed => rotateSpeedFactor * Time.deltaTime;
      public float moveSpeed => moveSpeedFactor;

      public Vector3 gravity => new Vector3(0, gravityScale, 0);
    }

    [System.Serializable]
    public struct Constraints
    {
      public float closestDistance;
      public float furthestDistance;

      public float relativeHeightFromGround;

      public float maximumHullTiltAngle;


      public float sqrClosestDistance => closestDistance * closestDistance;
      public float sqrFurthestDistance => furthestDistance * furthestDistance;

      public float ClampAngle(float angle) => Mathf.Clamp(
        (angle > 180f ? angle - 360f : angle),
        -maximumHullTiltAngle, maximumHullTiltAngle);

      public bool HasEnoughHight(Transform transform, out RaycastHit hit)
      {
        return !Physics.Raycast(
          transform.position,
          Vector3.down,
          out hit,
          relativeHeightFromGround,
          new Layers(Layer.Stage));
      }
    }
  }
}
