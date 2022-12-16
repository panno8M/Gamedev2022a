using System;
using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors.Player
{
  public class PlayerWings : ActorBehavior<PlayerAct>
  {
    protected override void Blueprint()
    {
      _actor.ctl.Up
        .Where(_ => !_actor.physics.isOnGround)
        .Subscribe(_ =>
        {
          if (flapCount >= upperLimit) { return; }
          flapCount++;
        }).AddTo(this);

      _actor.physics.OnLand
        .Subscribe(_ =>
        {
          flapCount = 0;
        }).AddTo(this);
    }
    [SerializeField] int _upperLimitInitial = 1;
    [SerializeField] int _upperLimitOverride;
    [SerializeField] bool _upperLimitOverriding;

    public IObservable<int> FlapCount => _FlapCount;
    public int upperLimit => _upperLimitOverriding ? _upperLimitOverride : _upperLimitInitial;

    [SerializeField] ReactiveProperty<int> _FlapCount = new ReactiveProperty<int>();
    public int flapCount
    {
      get { return _FlapCount.Value; }
      set { _FlapCount.Value = value; }
    }

    public void OverrideLimit(int newLimit)
    {
      _upperLimitOverride = newLimit;
      _upperLimitOverriding = true;
    }
    public void TightenLimit(int newLimit)
    {
      OverrideLimit(Mathf.Min(newLimit, _upperLimitOverride));
    }
    public void ResetLimit()
    {
      _upperLimitOverriding = false;
    }
  }
}