using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Utilities;

namespace Assembly.Components.Actors
{
  public class LaunchFromHatchModule : LaunchModule
  {
    [SerializeField] Renderer droneRenderer;
    [SerializeField] float launchSpeed;
    [SerializeField] DroneHatch hatch;

    protected override void Blueprint()
    {
      AlarmMgr.Instance.IsOnAlert.Where(x => !x).Subscribe(_ => Collect());

      hatch.CmdLaunch.Subscribe(_ => Launch().Forget());

      _actor.OnPhaseEnter(DronePhase.Standby)
        .Subscribe(phase => enabled = true)
        .AddTo(this);
      _actor.OnPhaseExit(DronePhase.Standby)
        .Subscribe(phase => enabled = false)
        .AddTo(this);
    }

    public override UniTask Collect()
    {
      switch (_actor.phase)
      {
        case DronePhase.Disactive:
        case DronePhase.Dead:
          return UniTask.CompletedTask;
      }

      _actor.transform.position = hatch.transform.position;
      _actor.phase = DronePhase.Standby;
      return UniTask.CompletedTask;
    }

    public override async UniTask Launch()
    {
      if (_actor.phase != DronePhase.Standby) { return; }
      while (hatch && transform.position != hatch.nextNode.transform.position)
      {
        _actor.transform.position = Vector3.MoveTowards(transform.position, hatch.nextNode.transform.position, launchSpeed * Time.deltaTime);
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      }
      _actor.patrol.current = hatch.nextNode;
      _actor.phase = DronePhase.Patrol;
    }
  }
}