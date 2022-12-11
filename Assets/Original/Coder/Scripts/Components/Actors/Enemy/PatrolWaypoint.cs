using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Assembly.Components.StageGimmicks;

namespace Assembly.Components.Actors
{
  [RequireComponent(typeof(InstantPortal))]
  public class PatrolWaypoint : TransferableBase
  {
    bool _actable = true;
    Portal defaultPortal;

    [SerializeField] float moveSpeed = 0.03f;

    async UniTask Yield(CancellationToken token)
    {
      do
      {
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
      }
      while (!_actable);
    }

    async UniTask LookAt(Portal portal, float delta, CancellationToken token)
    {
      while (true)
      {
        if (!this || !isActiveAndEnabled) { return; }
        if (!portal) { return; }
        var target = Quaternion.LookRotation(portal.transform.position - transform.position);
        if (transform.rotation == target) { return; }
        transform.rotation = Quaternion.RotateTowards(
          transform.rotation,
          target,
          delta);

        await Yield(token);
      }
    }
    void MoveForward()
    {
      transform.position += transform.forward * moveSpeed;
    }

    void Awake()
    {
      defaultPortal = GetComponent<InstantPortal>();
    }
    void OnEnable()
    {
      closestPortal = defaultPortal;
      Transfer().Forget();
    }

    protected override async UniTask OnProcessTransfer(Portal portal, CancellationToken token)
    {
      await LookAt(portal.next, 2f, token);

      while (portal && (transform.position - portal.next.transform.position).sqrMagnitude >= 0.01f)
      {
        if (!this || !isActiveAndEnabled) { return; }
        MoveForward();
        await Yield(token);
      }
    }

    protected override void AfterCompleteTransfer(Portal portal)
    {
      if (!this || !isActiveAndEnabled) { return; }
      portal.next.Transfer(this).Forget();
    }
  }
}