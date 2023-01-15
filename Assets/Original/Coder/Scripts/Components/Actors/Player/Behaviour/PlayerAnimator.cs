using UnityEngine;
using UniRx;
using Utilities;

namespace Assembly.Components.Actors.Player
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

    void Walk(bool b) => _anim.SetBool(walk, b);
    void Breath(bool b) => _anim.SetBool(fire, b);
    void Hold(Holdable holdable) => _anim.SetBool(grab, holdable);
    void Die(bool b) => _anim.SetBool(die, b);
    void Jump(bool b) => _anim.SetBool(jump, b);
    void Flap(int i) => _anim.SetTrigger(wing);


    void Walk(float move) => Walk(move != 0);
    void Breath(EzLerp.Mode mode) => Breath(mode == EzLerp.Mode.Increase);
    void Die(Unit _) => Die(true);
    void Revival(Unit _) => Die(false);
    void Jump(Unit _) => Jump(true);
    void Land(Unit _) => Jump(false);

    static readonly int walk = Animator.StringToHash("Walk");
    static readonly int grab = Animator.StringToHash("Grab");
    static readonly int fire = Animator.StringToHash("Fire");
    static readonly int die = Animator.StringToHash("Die");
    static readonly int jump = Animator.StringToHash("Jump");
    static readonly int wing = Animator.StringToHash("Wing");
  }
}