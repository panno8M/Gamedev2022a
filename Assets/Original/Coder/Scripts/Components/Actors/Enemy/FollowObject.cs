using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembly.GameSystem;

namespace Assembly.Components.Actors
{
  public class FollowObject : MonoBehaviour
  {
    [SerializeField] HostileDrone _actor;
    public Transform target;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform hose;
    [SerializeField] WaterEmitter _emitter;
    [SerializeField] PositionConstraints _constraints;



    void FixedUpdate()
    {
      var rot = Quaternion.LookRotation(target.position - transform.position);
      transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        rot,
        2f);
      hose.LookAt(target);

      Vector3 dir;

      float sqrDistance = (target.position - transform.position).sqrMagnitude;
      if (sqrDistance < _constraints.sqrClosestDistance)
      {
        dir = -transform.forward;
      }
      else if (_constraints.sqrFurthestDistance < sqrDistance)
      {
        dir = transform.forward;
      }
      else
      {
        dir = Vector3.zero;
      }

      if (!_constraints.HasEnoughHight(transform, out RaycastHit hit))
      {
        dir += Vector3.up;
      }

      _actor.Move(dir);
      _emitter.Launch();
    }
    [System.Serializable]
    class PositionConstraints
    {
      public float closestDistance = 3;
      public float furthestDistance = 4;

      public float relativeHeightFromGround = 1;

      public float sqrClosestDistance => closestDistance * closestDistance;
      public float sqrFurthestDistance => furthestDistance * furthestDistance;

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