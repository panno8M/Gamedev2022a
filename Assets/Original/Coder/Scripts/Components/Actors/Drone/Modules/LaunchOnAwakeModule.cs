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
      _actor.OnPhaseEnter(DronePhase.Standby)
        .Subscribe(phase =>
        {
          _actor.patrol.current = baseNode;
          _actor.phase = DronePhase.Patrol;
        });
    }
    public override UniTask Collect()
    {
      throw new System.NotImplementedException();
    }
    public override UniTask Launch()
    {
      throw new System.NotImplementedException();
    }
  }
}