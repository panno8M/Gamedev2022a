using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem.PathNetwork;

namespace Assembly.Components.Actors
{
  public class LaunchOnAwakeModule : LaunchModule
  {
    [SerializeField] PathNode baseNode;
    protected override void Blueprint()
    {
      base.Blueprint();
      Launch();
    }
    public override UniTask Collect()
    {
      if (_actor.phaseSilence) { return UniTask.CompletedTask; }
      _actor.transform.position = base.transform.position;
      _actor.rigidbody.velocity = Vector3.zero;
      _actor.patrol.next = null;
      _actor.ShiftDisactive();
      return UniTask.CompletedTask;
    }
    public override UniTask Launch()
    {
      if (!_actor.phaseDisactive) { return UniTask.CompletedTask; }
      _actor.patrol.next = baseNode.routes[0].dst;
      _actor.ShiftStandby();
      return UniTask.CompletedTask;
    }
  }
}