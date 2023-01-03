using System;
using UniRx;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.Actors
{
  public abstract class ActorCore<Actor> : ActorBehavior<Actor>, IPoolCollectable
    where Actor : ActorCore<Actor>
  {
    public IDespawnable despawnable { get; set; }
    protected Subject<Unit> _OnAssemble = new Subject<Unit>();
    public IObservable<Unit> OnAssembleObservable => _OnAssemble;
    public void Assemble() { _OnAssemble.OnNext(Unit.Default); }
    public void Disassemble() { }

  }
}