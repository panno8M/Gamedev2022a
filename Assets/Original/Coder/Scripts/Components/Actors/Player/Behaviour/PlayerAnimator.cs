using UnityEngine;
using UniRx;
using UniRx.Ex.InteractionTraits;

namespace Assembly.Components.Actors.Player
{
  public class PlayerAnimator : MonoBehaviour
  {
    [SerializeField] PlayerAct _player;
    [SerializeField] Animator _anim;
    void Awake()
    {
      Global.Control.HorizontalMoveInput
          .Subscribe(Walk)
          .AddTo(this);

      _player.interactor.holder.HoldingItem
          .Subscribe(Hold)
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
  }
}