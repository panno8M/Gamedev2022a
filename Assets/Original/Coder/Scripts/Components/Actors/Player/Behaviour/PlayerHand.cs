using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors.Player
{
  public class PlayerHand : ActorBehavior<PlayerAct>
  {
    [SerializeField] Holder _holder;
    public Holder holder => _holder;
    [SerializeField] Interactor _interactor;
    public Interactor interactor => _interactor;
    protected override void Blueprint()
    {
      _actor.ctl.Interact
        .Subscribe(_ =>
        {
          if (_holder.FindHoldableInAround(out Holdable holdable))
          {
            _holder.Hold(holdable);
          }
          else if (interactor.FindInteractableInAround(out Interactable interactable))
          {
            _interactor.Interact();
          }
          else if (_holder.hasItem)
          {
            _holder.ReleaseForce();
          }
        }).AddTo(this);

      _holder.HoldingItem
        .Subscribe(item =>
          {
            if (item)
            {
              _actor.wings.TightenLimit(0);
            }
            else
            {
              _actor.wings.ResetLimit();
            }
          }).AddTo(this);
    }
  }
}