using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors.Player
{
  public class PlayerAnimator : MonoBehaviour
  {
    [SerializeField] Animator _anim;
    void Awake()
    {
      Global.Control.HorizontalMoveInput
          .Subscribe(Walk)
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
  }
}