using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Assembly.GameSystem;
using Assembly.GameSystem.PathNetwork;


namespace Assembly.Components.Actors
{
  public class LaunchModule : DiBehavior
  {
    [SerializeField] public PathNode baseNode;
    [SerializeField] protected DroneAct _actor;
    [SerializeField] bool launchOnAssemble;

    protected override void Blueprint()
    {
      _actor.OnAssemble.Subscribe(_ =>
        {
          if (launchOnAssemble)
          { Launch(); }
        }).AddTo(this);
    }


    public void Launch()
    {
      if (!_actor.phaseDisactive) { return; }
      _actor.patrol.next = baseNode.routes[0].dst;
      _actor.ShiftStandby();
      return;
    }
    public void Collect()
    {
      if (_actor.phaseSilence) { return; }
      _actor.transform.position = baseNode.transform.position;
      _actor.rigidbody.velocity = Vector3.zero;
      _actor.patrol.next = null;
      _actor.ShiftDisactive();
      return;
    }
  }
}