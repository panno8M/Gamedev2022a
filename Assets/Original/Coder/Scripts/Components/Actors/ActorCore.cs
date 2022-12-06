using System;
using UniRx;
using Assembly.GameSystem;

namespace Assembly.Components.Actors
{
  public abstract class ActorCore<Actor> : ActorBehavior<Actor>, IPoolCollectable
    where Actor : ActorCore<Actor>
  {
    protected Subject<Unit> _OnAssemble = new Subject<Unit>();
    public IObservable<Unit> OnAssembleObservable => _OnAssemble;
    public void Assemble() { _OnAssemble.OnNext(Unit.Default); }
    public void Disassemble() { }

  }
}