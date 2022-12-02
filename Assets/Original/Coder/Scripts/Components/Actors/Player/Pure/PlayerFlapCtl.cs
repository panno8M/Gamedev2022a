using System;
using UnityEngine;
using UniRx;

namespace Assembly.Components.Actors
{

  [Serializable]
  public class PlayerFlapCtl
  {
    [SerializeField] int _upperLimitInitial;
    [SerializeField] int _upperLimitOverride;
    [SerializeField] bool _upperLimitOverriding;
    [SerializeField] ReactiveProperty<int> _currentCnt;

    public readonly IObservable<int> OnFlap;

    public PlayerFlapCtl(int upperLimit)
    {
      _upperLimitInitial = upperLimit;
      _currentCnt = new ReactiveProperty<int>();

      OnFlap = _currentCnt;
    }

    public int UpperLimit => _upperLimitOverriding ? _upperLimitOverride : _upperLimitInitial;
    public int CurrentCnt => _currentCnt.Value;

    public void Inc()
    {
      if (_currentCnt.Value >= UpperLimit) { return; }
      _currentCnt.Value++;
    }

    public void ResetCount()
    {
      _currentCnt.Value = 0;
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