using UnityEngine;
using UniRx;
using UniRx.Ex.InteractionTraits;
using Utilities;

namespace Assembly.Components.Actors
{
  public class PlayerAnimator : ActorBehavior<PlayerAct>
  {
    [SerializeField] PlayerBreath _breath;
    [SerializeField] Animator _anim;
    protected override void Blueprint()
    {
      _actor.HoriMove
          .ThrottleFrame(5)
          .Subscribe(Walk)
          .AddTo(this);

      _actor.interactor.holder.HoldingItem
          .Subscribe(Hold)
          .AddTo(this);

      _breath.exhalingProgress.OnModeChanged
          .Subscribe(Breath)
          .AddTo(this);

      _actor.damagable.OnBroken
          .Subscribe(Die)
          .AddTo(this);

      _actor.damagable.OnRepaired
          .Subscribe(Revival)
          .AddTo(this);

      _actor.OnJump
          .Subscribe(Jump)
          .AddTo(this);

      _actor.OnLand
          .Subscribe(Land)
          .AddTo(this);

      _actor.OnFlapWhileFalling
          .Subscribe(Flap)
          .AddTo(this);
    }

    void Walk(bool b)
    {
      _anim.SetBool("Walk", b);
    }
    void Walk(PlayerAct.HoriMoveStat move)
    {
      Walk(move != 0);
    }

    void Hold(HoldableModule holdable)
    {
      _anim.SetBool("Grab", holdable);
    }

    void Breath(bool b) { _anim.SetBool("Fire", b); }
    void Breath(EzLerp.Mode mode) { _anim.SetBool("Fire", mode == EzLerp.Mode.Increase); }
    void Die(bool b) { _anim.SetBool("Die", b); }
    void Die(Unit _) { Die(true); }
    void Revival(Unit _) { Die(false); }
    void Jump(bool b) { _anim.SetBool("Jump", b); }
    void Jump(Unit _) { Jump(true); }
    void Land(Unit _) { Jump(false); }
    void Flap(int i) { _anim.SetTrigger("Wing"); }
  }
}