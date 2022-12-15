using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;

namespace Assembly.Components.Actors
{
  public class FollowObjectModule : ActorBehavior<HostileDrone>
  {
    [SerializeField] PositionConstraints _constraints;

    protected override void Blueprint()
    {
      this.FixedUpdateAsObservable()
      .Subscribe(_ =>
      {

        if (!_actor.target) { return; }
        var rot = Quaternion.LookRotation(_actor.target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(
          transform.rotation,
          rot,
          2f);

        Vector3 dir;

        float sqrDistance = (_actor.target.position - transform.position).sqrMagnitude;
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
      });
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