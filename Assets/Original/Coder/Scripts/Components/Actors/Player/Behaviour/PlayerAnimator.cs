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
      Global.Control.HorizontalMoveInput
          .Subscribe(Walk)
          .AddTo(this);

      _player.interactor.holder.HoldingItem
          .Subscribe(Hold)
          .AddTo(this);

      _breath.IsExhaling
          .Subscribe(Breath)
          .AddTo(this);
    
      _player.Damagable.OnBroken
          .Subscribe(Die)
          .AddTo(this);

      _player.Damagable.OnRepaired
          .Subscribe(Revival)
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
  }
}