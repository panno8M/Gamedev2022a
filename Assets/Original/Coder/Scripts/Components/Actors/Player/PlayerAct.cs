using System;
using UnityEngine;
using Utilities;

namespace Assembly.Components.Actors.Player
{
  public class PlayerAct : ActorCore<PlayerAct>
  {

    #region modules
    [SerializeField] PlayerController _ctl;
    [SerializeField] PlayerPhysics _physics;
    [SerializeField] PlayerHand _hand;
    [SerializeField] PlayerBreath _mouse;
    [SerializeField] PlayerFlameReceptor _flame;
    [SerializeField] PlayerWings _wings;
    [SerializeField] PlayerLife _life;
    [SerializeField] PlayerBehavior _behavior;
    [SerializeField] PlayerAnimator _animator;

    internal PlayerController ctl => _ctl;
    internal PlayerPhysics physics => _physics;
    internal PlayerHand hand => _hand;
    internal PlayerBreath mouse => _mouse;
    internal PlayerFlameReceptor flame => _flame;
    internal PlayerWings wings => _wings;
    internal PlayerLife life => _life;
    internal PlayerBehavior behavior => _behavior;
    internal PlayerAnimator animator => _animator;
    #endregion

    protected override void OnAssemble()
    {
      transform.position = Global.PlayerPool.activeSpawnPoint.position;
      ctl.enabled = true;
    }

    protected override void Blueprint()
    {
      ctl.Initialize();
      physics.Initialize();
      life.Initialize();
      flame.Initialize();

      hand.Initialize();
      mouse.Initialize();
      wings.Initialize();
      behavior.Initialize();
      animator.Initialize();
    }

  }
}