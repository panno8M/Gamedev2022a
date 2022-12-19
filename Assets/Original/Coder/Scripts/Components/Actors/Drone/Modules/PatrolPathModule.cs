using UnityEngine;
using Cysharp.Threading.Tasks;
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
    PathNode Select(PathNode from)
    {
      return from.routes[0].dst;
    }

    protected override void Blueprint()
    {

      _actor.OnPhaseEnter(DronePhase.Patrol)
        .Subscribe(_ =>
        {
          enabled = true;
          Patrol().Forget();
        });
      _actor.OnPhaseExit(DronePhase.Patrol)
        .Subscribe(_ => enabled = false);
      _actor.Target
        .Where(_ => _actor.phase == DronePhase.Patrol)
        .Where(target => target)
        .Subscribe(_ => _actor.phase = DronePhase.Hostile);

      SwipeCamera().Forget();
    }
    async UniTask Patrol()
    {
      while (true)
      {
        if (!this || !isActiveAndEnabled) { return; }
        await LookNext(2f);
        await MoveNext();
        current = Select(current);
      }
    }
    async UniTask MoveNext()
    {
      PathNode next = Select(current);
      while (next && (next.transform.position - transform.position).sqrMagnitude >= 0.01f)
      {
        if (!this || !isActiveAndEnabled) { return; }
        _actor.Move(transform.forward);
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      }
    }

    async UniTask LookNext(float delta)
    {
      while (true)
      {
        if (!this || !isActiveAndEnabled) { return; }
        PathNode next = Select(current);
        var target = Quaternion.LookRotation(next.transform.position - transform.position);

        if (transform.rotation == target) { return; }
        transform.rotation = Quaternion.RotateTowards(
          transform.rotation,
          target,
          delta);
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      }
    }
    public float CamSwipeMin;
    public float CamSwipeMax;
    async UniTask SwipeCamera()
    {
      bool isMin = true;
      while (true)
      {
        var deg = isMin ? CamSwipeMin : CamSwipeMax;
        var target = Quaternion.AngleAxis(deg, _actor.transform.right) * _actor.transform.rotation;
        var rot = Quaternion.RotateTowards(_actor.sightTransform.rotation, target, .1f);
        _actor.sightTransform.rotation = rot;
        if (rot == target) { isMin = !isMin; }
        do
        {
          await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }
        while (!this || !isActiveAndEnabled);
      }
    }
  }
}