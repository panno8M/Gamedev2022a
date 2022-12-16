using UnityEngine;
using UniRx;
using Utilities;

namespace Assembly.Components.Actors
{
  public class PlayerAnimator : ActorBehavior<PlayerAct>
  {
    [SerializeField] Animator _anim;
    protected override void Blueprint()
    {
      _actor.ctl.Horizontal
          .ThrottleFrame(5)
          .Subscribe(Walk)
          .AddTo(this);

      _actor.hand.holder.HoldingItem
          .Subscribe(Hold)
          .AddTo(this);

      _actor.mouse.exhalingProgress.OnModeChanged
          .Subscribe(Breath)
          .AddTo(this);

      _actor.life.OnDead
          .Subscribe(Die)
          .AddTo(this);

      _actor.life.OnRevived
          .Subscribe(Revival)
          .AddTo(this);

      _actor.behavior.OnJump
          .Subscribe(Jump)
          .AddTo(this);

      _actor.behavior.OnFlap1
          .Subscribe(Flap)
          .AddTo(this);

      _actor.physics.OnLand
          .Subscribe(Land)
          .AddTo(this);
    }

    void Walk(bool b)
    {
      _anim.SetBool("Walk", b);
    }
    void Walk(float move)
    {
      Walk(move != 0);
    }

    void Hold(Holdable holdable)
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