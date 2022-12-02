using System;
using UnityEngine;
using UniRx;
using Senses.Pain;

namespace Assembly.Components.Actors
{
  public abstract class ActorCore<Actor> : ActorBehavior<Actor>, IPoolCollectable
    where Actor : ActorCore<Actor>
  {
    protected Subject<Unit> _OnRebuild = new Subject<Unit>();
    public IObservable<Unit> OnRebuildObservable => _OnRebuild;
    public void Rebuild() { _OnRebuild.OnNext(Unit.Default); }

  }
}