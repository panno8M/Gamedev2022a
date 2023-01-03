using UnityEngine;
using Assembly.GameSystem;
using Assembly.GameSystem.PathNetwork;

namespace Assembly.Components.Actors
{
  public class LaunchModule : DiBehavior
  {
    [SerializeField] public PathNode baseNode;
    [SerializeField] protected DroneAct _actor;

    protected override void Blueprint() { }

    public void Launch()
    {
      if (!_actor.isActiveAndEnabled) { return; }
      _actor.transform.position = baseNode.transform.position;
      _actor.patrol.next = baseNode.routes[0].dst;
      _actor.LookTowards(_actor.patrol.next.transform, 360);
      _actor.phase.ShiftStandby();
      return;
    }
    public void Collect()
    {
      if (!_actor.isActiveAndEnabled) { return; }
      _actor.transform.position = baseNode.transform.position;
      _actor.patrol.next = null;
      _actor.phase.ShiftSleep();
      return;
    }
  }
}