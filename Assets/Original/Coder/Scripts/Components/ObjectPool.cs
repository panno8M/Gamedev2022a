using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace Assembly.Components
{
  public abstract class ObjectPool<T> : UniqueBehaviour<ObjectPool<T>>
    where T : MonoBehaviour
  {
    protected abstract T CreateInstance();

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
    void Start()
    {
      if (_pool == null) _pool = new InternalPool(this);
      this.OnDestroyAsObservable()
          .Subscribe(_ => _pool.Dispose());
      OnInit();
    }
    protected abstract void OnInit();

    class InternalPool : UniRx.Toolkit.ObjectPool<T>
    {
      ObjectPool<T> _super;
      public InternalPool(ObjectPool<T> pool)
      {
        _super = pool;
      }
      protected override T CreateInstance()
      {
        return _super.CreateInstance();
      }
    }

  }
}