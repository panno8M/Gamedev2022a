using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors
{
  public class PlayerHand : ActorBehavior<PlayerAct>
  {
    [SerializeField] Holder _holder;
    public Holder holder => _holder;
    protected override void Blueprint()
    {
      _actor.ctl.Interact
        .Subscribe(_ =>
        {
          if (_holder.hasItem)
          { _holder.ReleaseForce(); }
          else
          { _holder.AttemptToHold(); }
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