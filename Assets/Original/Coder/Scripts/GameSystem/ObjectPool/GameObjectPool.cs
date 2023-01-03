using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.GameSystem.ObjectPool
{
  public abstract class GameObjectPool<T> : DiBehavior, IObjectPool<T>
    where T : DiBehavior, IPoolCollectable
  {
    [SerializeField] protected T prefab;

    protected abstract T CreateInstance();
    protected virtual void OnBeforeSpawn(T instance) { }
    protected virtual void OnBeforeDespawn(T instance) { }

    public T Spawn(IInfuser<T> info)
    {
      T result = pool.Rent();
      info.Infuse(result);
      result.despawnable = new Despawnable(() => this.Despawn(result));
      return result;
    }
    public void Despawn(T instance)
    {
      if (instance && instance.isActiveAndEnabled)
      {
        pool.Return(instance);
      }
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