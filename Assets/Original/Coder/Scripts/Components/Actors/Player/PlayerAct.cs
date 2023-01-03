using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors.Player
{
  public class PlayerAct : ActorCore<PlayerAct>
  {
    [UnityEngine.SerializeField]
    Rollback rollback;
    [Zenject.Inject]
    public void DepsInject(Rollback rollback)
    {
      this.rollback = rollback;
    }


    #region modules
    [SerializeField] PlayerController _ctl;
    [SerializeField] PlayerPhysics _physics;
    [SerializeField] PlayerHand _hand;
    [SerializeField] PlayerBreath _mouse;
    [SerializeField] PlayerFlameReceptor _flame;
    [SerializeField] PlayerWings _wings;
    [SerializeField] PlayerLife _life;
    [SerializeField] PlayerRebirth _rebirth;
    [SerializeField] PlayerBehavior _behavior;
    [SerializeField] PlayerAnimator _animator;

    internal PlayerController ctl => _ctl;
    internal PlayerPhysics physics => _physics;
    internal PlayerHand hand => _hand;
    internal PlayerBreath mouse => _mouse;
    internal PlayerFlameReceptor flame => _flame;
    internal PlayerWings wings => _wings;
    internal PlayerLife life => _life;
    internal PlayerRebirth rebirth => _rebirth;
    internal PlayerBehavior behavior => _behavior;
    internal PlayerAnimator animator => _animator;
    #endregion


    void Awake()
    {
      Initialize();
      rebirth.Spawn();
    }

    protected override void OnAssemble()
    {
      ctl.enabled = true;
    }

    protected override void Blueprint()
    {
      ctl.Initialize();
      physics.Initialize();
      life.Initialize();
      rebirth.Initialize();
      flame.Initialize();

      hand.Initialize();
      mouse.Initialize();
      wings.Initialize();
      behavior.Initialize();
      animator.Initialize();

      rollback.OnExecute.Subscribe(_ => rebirth.Respawn());
    }

  }
}