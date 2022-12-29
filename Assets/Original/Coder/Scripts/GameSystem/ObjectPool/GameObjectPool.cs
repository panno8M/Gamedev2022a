using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.GameSystem.ObjectPool
{
  public abstract class GameObjectPool<T> : DiBehavior
    where T : DiBehavior, IPoolCollectable
  {
    [SerializeField] protected T prefab;

    protected abstract T CreateInstance();
    protected virtual void OnBeforeSpawn(T instance) { }
    protected virtual void OnBeforeDespawn(T instance) { }

    Subject<T> _OnSpawn = new Subject<T>();
    public IObservable<T> OnSpawn() => _OnSpawn;
    public IObservable<T> OnSpawn(T obj) => _OnSpawn.Where(x => x == obj);

    Subject<T> _OnDespawn = new Subject<T>();
    public IObservable<T> OnDespawn() => _OnDespawn;
    public IObservable<T> OnDespawn(T obj) => _OnDespawn.Where(x => x == obj);

    public T Spawn(ObjectCreateInfo<T> info)
    {
      var result = pool.Rent();
      info.Infuse(result);
      _OnSpawn.OnNext(result);
      return result;
    }
    public T Spawn(ObjectCreateInfo<T> info, TimeSpan timeToDespawn)
    {
      T result = Spawn(info);
      Despawn(result, timeToDespawn);
      return result;
    }
    public void Despawn(T obj)
    {
      if (obj && obj.isActiveAndEnabled)
      {
        _OnDespawn.OnNext(obj);
        pool.Return(obj);
      }
    }
    public void Despawn(T obj, TimeSpan timeToDespawn)
    {
      Observable.Timer(timeToDespawn).Subscribe(_ => Despawn(obj)).AddTo(obj);
    }

    public void Respawn(T obj, ObjectCreateInfo<T> info, TimeSpan wait)
    {
      Despawn(obj);
      Observable.Timer(wait).Subscribe(_ => Spawn(info)).AddTo(obj);
    }
    public void Respawn(T obj, ObjectCreateInfo<T> info)
    {
      Despawn(obj);
      Spawn(info);
    }
    InternalPool _pool;
    InternalPool pool => _pool ?? (_pool = new InternalPool(this));
    protected void Start()
    {
      if (_pool == null) _pool = new InternalPool(this);
      this.OnDestroyAsObservable()
          .Subscribe(_ => _pool.Dispose());
      Initialize();
    }
    class InternalPool : UniRx.Toolkit.ObjectPool<T>
    {
      GameObjectPool<T> _super;
      public InternalPool(GameObjectPool<T> pool)
      {
        _super = pool;
      }
      protected override T CreateInstance()
      {
        return _super.CreateInstance();
      }
      protected override void OnBeforeRent(T instance)
      {
        base.OnBeforeRent(instance);
        _super.OnBeforeSpawn(instance);
        instance.Assemble();
      }
      protected override void OnBeforeReturn(T instance)
      {
        instance.Disassemble();
        _super.OnBeforeDespawn(instance);
        base.OnBeforeReturn(instance);
      }
    }

  }
}