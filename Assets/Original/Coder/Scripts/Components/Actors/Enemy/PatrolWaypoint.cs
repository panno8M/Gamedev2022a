using UnityEngine;
using Cysharp.Threading.Tasks;
using Assembly.Components.StageGimmicks;

namespace Assembly.Components.Actors
{
  [RequireComponent(typeof(InstantPortal))]
  public class PatrolWaypoint : TransferableBase
  {
    bool _actable = true;

    [SerializeField] float moveSpeed = 0.03f;

    async UniTask Yield()
    {
      do
      {
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
      }
      while (!_actable);
    }

    async UniTask LookAt(Portal portal, float delta)
    {
      while (true)
      {
        if (!portal) { return; }
        var target = Quaternion.LookRotation(portal.transform.position - transform.position);
        if (transform.rotation == target) { return; }
        transform.rotation = Quaternion.RotateTowards(
          transform.rotation,
          target,
          delta);

        await Yield();
      }
    }
    void MoveForward()
    {
      transform.position += transform.forward * moveSpeed;
    }

    void Start()
    {
      closestPortal = GetComponent<InstantPortal>();
      Transfer().Forget();
    }

    protected override async UniTask OnProcessTransfer(Portal portal)
    {
      await LookAt(portal.next, 2f);

      while (portal && (transform.position - portal.next.transform.position).sqrMagnitude >= 0.01f)
      {
        MoveForward();
        await Yield();
      }
    }

    protected override void AfterCompleteTransfer(Portal portal)
    {
      portal.next.Transfer(this).Forget();
    }

    public void Play()
    {
      _actable = true;
    }
    public void Stop()
    {
      _actable = false;
    }
  }
}