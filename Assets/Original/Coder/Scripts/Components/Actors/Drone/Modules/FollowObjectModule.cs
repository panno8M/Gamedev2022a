using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Utilities;

namespace Assembly.Components.Actors
{
  public class FollowObjectModule : DiBehavior
  {
    [SerializeField] DroneAct _actor;
    [SerializeField] PositionConstraints _pcons;
    [SerializeField] RotationConstraints _rcons;

    protected override void Blueprint()
    {
      _actor.OnPhaseEnter(DronePhase.Hostile)
        .Subscribe(_ => enabled = true);
      _actor.OnPhaseExit(DronePhase.Hostile)
        .Subscribe(_ => enabled = false);

      _actor.Target
        .Where(_ => _actor.phase == DronePhase.Hostile)
        .Where(target => !target)
        .Subscribe(_ => _actor.phase = DronePhase.Patrol);

      this.FixedUpdateAsObservable()
        .Where(_ => _actor.phase == DronePhase.Hostile)
        .Where(_ => _actor.target)
        .Subscribe(_ =>
        {
          var rot = Quaternion.LookRotation(_actor.target.position - transform.position);
          var rotangles = rot.eulerAngles;
          rot = Quaternion.Euler(
            Mathf.Clamp((rotangles.x > 180f ? rotangles.x - 360f: rotangles.x), -_rcons.maximumHullTilt, _rcons.maximumHullTilt),
            rotangles.y,
            rotangles.z);
          transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rot,
            2f);

          Vector3 dir;

          float sqrDistance = (_actor.target.position - transform.position).sqrMagnitude;
          if (sqrDistance < _pcons.sqrClosestDistance)
          {
            dir = -transform.forward.x_z();
          }
          else if (_pcons.sqrFurthestDistance < sqrDistance)
          {
            dir = transform.forward;
          }
          else
          {
            dir = Vector3.zero;
          }

          if (!_pcons.HasEnoughHight(transform, out RaycastHit hit))
          {
            dir += Vector3.up;
          }

          _actor.Move(dir);
        });
    }

    [System.Serializable]
    class RotationConstraints
    {
      public float maximumHullTilt = 30;
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