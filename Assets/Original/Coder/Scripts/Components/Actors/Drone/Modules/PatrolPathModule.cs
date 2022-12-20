using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Assembly.GameSystem.PathNetwork;

namespace Assembly.Components.Actors
{
  public class PatrolPathModule : DiBehavior
  {
    [SerializeField] DroneAct _actor;
    public PathNode current;
    PathNode next;

    bool needsFace = true;
    bool needsMove = true;

    PathNode Select(PathNode from)
    {
      return from.routes[0].dst;
    }

    protected override void Blueprint()
    {

      _actor.OnPhaseEnter(DronePhase.Patrol)
        .Subscribe(_ => enabled = true)
        .AddTo(this);
      _actor.OnPhaseExit(DronePhase.Patrol)
        .Subscribe(_ => enabled = false)
        .AddTo(this);

      _actor.Target
        .Where(_ => _actor.phase == DronePhase.Patrol)
        .Where(target => target)
        .Subscribe(_ => _actor.phase = DronePhase.Hostile);

      this.FixedUpdateAsObservable()
        .Where(_ => this && isActiveAndEnabled)
        .Subscribe(_ => Patrol());
    }
    void Patrol()
    {
      if (!next) { next = Select(current); }
      if (!next) { return; }

      LookNext(2f);

      if (!needsFace) { MoveNext(); }

      if (!needsMove)
      {
        current = Select(current);
        next = Select(current);
        needsFace = true;
        needsMove = true;
      }
    }
    void MoveNext()
    {
      if (!needsMove) { return; }
      if ((next.transform.position - transform.position).sqrMagnitude < 0.01f) { needsMove = false; }
      _actor.Move(transform.forward);
    }

    void LookNext(float delta)
    {
      var target = Quaternion.LookRotation(next.transform.position - transform.position);

      if (transform.rotation == target)
      {
        needsFace = false;
        return;
      }
      transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        target,
        delta);
    }
  }
}