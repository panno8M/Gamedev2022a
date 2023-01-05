using System;
using UniRx;

namespace Assembly.GameSystem.ObjectPool
{
  public static class IObjectPoolExtensions
  {
    public static void Respawn<T>(this IObjectPool<T> pool, T instance, IInfuser<T> info)
    {
      pool.Despawn(instance);
      pool.Spawn(info);
    }

    public static void Despawn<T>(this IObjectPool<T> pool, T instance, TimeSpan timeToDespawn)
      where T : UnityEngine.Component
    {
      Observable
        .Timer(timeToDespawn)
        .Subscribe(_ => pool.Despawn(instance))
        .AddTo(instance);
    }

    public static T Spawn<T>(this IObjectPool<T> pool, IInfuser<T> info, TimeSpan timeToDespawn)
      where T : UnityEngine.Component
    {
      T result = pool.Spawn(info);
      pool.Despawn(result, timeToDespawn);
      return result;
    }

    public static void Respawn<T>(this IObjectPool<T> pool, T instance, IInfuser<T> info, TimeSpan timeToSpawn)
      where T : UnityEngine.Component
    {
      pool.Despawn(instance);
      Observable
        .Timer(timeToSpawn)
        .Subscribe(_ => pool.Spawn(info))
        .AddTo(instance);
    }
  }
}