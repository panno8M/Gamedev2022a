using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

namespace Assembly.Components.Actors
{
  public class LaunchFromHatchModule : LaunchModule
  {
    [SerializeField] Renderer droneRenderer;
    [SerializeField] float launchSpeed;
    [SerializeField] DroneHatch hatch;

    protected override void Blueprint()
    {
      base.Blueprint();

      AlarmMgr.Instance.IsOnAlert.Where(x => !x).Subscribe(_ => Collect());

      hatch.CmdLaunch.Subscribe(_ => Launch().Forget());
    }

    public override UniTask Collect()
    {
      if (_actor.phaseSilence) { return UniTask.CompletedTask; }
      _actor.transform.position = hatch.transform.position;
      _actor.rigidbody.velocity = Vector3.zero;
      _actor.patrol.next = null;
      _actor.ShiftDisactive();
      return UniTask.CompletedTask;
    }

    public override async UniTask Launch()
    {
      if (!_actor.phaseDisactive) { return; }
      _actor.ShiftLaunch();

      while (hatch && transform.position != hatch.nextNode.transform.position)
      {
        _actor.transform.position = Vector3.MoveTowards(transform.position, hatch.nextNode.transform.position, launchSpeed * Time.deltaTime);
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      }
      _actor.patrol.next = hatch.nextNode.routes[0].dst;
      _actor.ShiftStandby();
    }
  }
}