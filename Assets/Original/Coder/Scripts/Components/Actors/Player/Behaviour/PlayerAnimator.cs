using UnityEngine;
using UniRx;
using UniRx.Ex.InteractionTraits;

namespace Assembly.Components.Actors.Player
{
  public class PlayerAnimator : MonoBehaviour
  {
    [SerializeField] PlayerAct _player;
    [SerializeField] PlayerBreath _breath;
    [SerializeField] Animator _anim;
    void Awake()
    {
      _player.HorizontalMove
          .ThrottleFrame(5)
          .Subscribe(Walk)
          .AddTo(this);

      _player.interactor.holder.HoldingItem
          .Subscribe(Hold)
          .AddTo(this);

      _breath.IsExhaling
          .Subscribe(Breath)
          .AddTo(this);
    
      _player.damagable.OnBroken
          .Subscribe(Die)
          .AddTo(this);

      _player.damagable.OnRepaired
          .Subscribe(Revival)
          .AddTo(this);

      _player.OnJump
          .Subscribe(Jump)
          .AddTo(this);

      _player.OnLand
          .Subscribe(Land)
          .AddTo(this);

      _player.OnFlapWhileFalling
          .Subscribe(Flap)
          .AddTo(this);
    }

    void Walk(bool b)
    {
      _anim.SetBool("Walk", b);
    }
    void Walk(float hmi)
    {
      Walk(hmi != 0);
    }

    void Hold(HoldableModule holdable)
    {
      _anim.SetBool("Grab", holdable);
    }

    void Breath(bool b) { _anim.SetBool("Fire", b); }
    void Die(bool b) { _anim.SetBool("Die", b); }
    void Die(Unit _) { Die(true); }
    void Revival(Unit _) { Die(false); }
    void Jump(bool b) { _anim.SetBool("Jump", b); }
    void Jump(Unit _) { Jump(true); }
    void Land(Unit _) { Jump(false); }
    void Flap(int i) { _anim.SetTrigger("Wing"); }
  }
}