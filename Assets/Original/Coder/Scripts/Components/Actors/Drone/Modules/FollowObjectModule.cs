using UnityEngine;
using UniRx;
using Assembly.GameSystem;

namespace Assembly.Components.Actors
{
  public class FollowObjectModule : DiBehavior
  {
    [SerializeField] DroneAct _actor;

    protected override void Blueprint()
    {
      _actor.phase.ActivateSwitch(targets: this,
        cond: DronePhase.Hostile);

      _actor.aim.Target
        .Where(_ => isActiveAndEnabled)
        .Where(target => !target)
        .Subscribe(_actor.phase.ShiftStandby);

      _actor.BehaviorUpdate(this)
        .Where(_ => _actor.aim.target)
        .Subscribe(_ =>
        {
          _actor.LookTowards(_actor.aim.target.center);

          float sqrDistance = _actor.sqrDistance(_actor.aim.target.center);
          if (sqrDistance < _actor.param.constraints.sqrClosestDistance)
          {
            _actor.MoveObjective(Vector3.back);
          }
          else if (_actor.param.constraints.sqrFurthestDistance < sqrDistance)
          {
            _actor.MoveSubjective(Vector3.forward);
          }

          if (!_actor.param.constraints.HasEnoughHight(transform, out RaycastHit hit))
          {
            _actor.MoveObjective(Vector3.up);
          }
        });
    }
  }
}