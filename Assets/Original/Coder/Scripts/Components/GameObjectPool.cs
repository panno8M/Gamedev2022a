using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace Assembly.Components
{
  public abstract class GameObjectPool<T> : UniqueBehaviour<GameObjectPool<T>>
    where T : DiBehavior, IPoolCollectable
  {
    [SerializeField] protected GameObject prefab;

    protected abstract T CreateInstance();
    protected virtual void OnBeforeSpawn(T instance) { }
    protected virtual void OnBeforeDespawn(T instance) { }

    Subject<T> _OnSpawn = new Subject<T>();
    public IObservable<T> OnSpawn => _OnSpawn;

    Subject<T> _OnDespawn = new Subject<T>();
    public IObservable<T> OnDespawn => _OnDespawn;

    public T Spawn()
    {
      var result = pool.Rent();
      if (result) _OnSpawn.OnNext(result);
      return result;
    }
    public void Despawn(T obj)
    {
      if (obj)
      {
        _OnDespawn.OnNext(obj);
        pool.Return(obj);
      }
    }


    InternalPool _pool;
    InternalPool pool => _pool ?? (_pool = new InternalPool(this));
    protected void Start()
    {
      if (_pool == null) _pool = new InternalPool(this);
      this.OnDestroyAsObservable()
          .Subscribe(_ => _pool.Dispose());
      Blueprint();
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