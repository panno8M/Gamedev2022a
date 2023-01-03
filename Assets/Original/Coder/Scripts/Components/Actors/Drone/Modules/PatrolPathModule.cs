using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.PathNetwork;
using Utilities;

namespace Assembly.Components.Actors
{
  public class PatrolPathModule : DiBehavior
  {
    [SerializeField] DroneAct _actor;
    PathNode prev;
    PathNode _next;
    public PathNode next
    {
      get => _next;
      set
      {
        _next = value;
      }
    }
    bool flagTurn;

    PathNode Select(PathNode from)
    {
      return from.routes[0].dst;
    }

    protected override void Blueprint()
    {
      _actor.phase.ActivateSwitch(targets: this,
        cond: DronePhase.Patrol);

      _actor.aim.sight.InSight
        .Where(_ => isActiveAndEnabled)
        .Where(target => target)
        .Subscribe(_actor.phase.ShiftStandby);

      _actor.BehaviorUpdate(this)
        .Where(_ => next)
        .Subscribe(_ => Patrol());
    }
    void Patrol()
    {
      if (_actor.LookTowards(next.transform) && flagTurn)
      { flagTurn = false; }


      if (!flagTurn)
      {
        float sqrDistance = _actor.sqrDistance(next.transform);
        if (sqrDistance < 0.01f)
        {
          prev = next;
          next = Select(prev);
          flagTurn = true;
        }

        _actor.MoveSubjective(Vector3.forward);
        if (transform.position.y != next.transform.position.y)
        {
          _actor.MoveObjective((next.transform.position.y - transform.position.y).AsUp());
        }
      }
      else
      { rigidbody.velocity = Vector3.zero; }

    }
  }
}