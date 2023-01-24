using System;
using System.Collections.Generic;
using System.Threading;
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

    public T Spawn(IInfuser<T> info)
    {
      if (q == null) q = new Queue<T>();

      T instance = null;
      while (!instance && q.Count > 0)
      {
        instance = q.Dequeue();
      }
      if (!instance) { instance = CreateInstance(); }

      instance.gameObject.SetActive(true);
      instance.Assemble();
      info.Infuse(instance);
      instance.despawnable = new Despawnable(() => this.Despawn(instance));
      return instance;
    }

    public void Despawn(T instance)
    {
      if (instance == null) throw new ArgumentNullException("instance");
      if (!instance.isActiveAndEnabled) { return; }

      if (q == null) q = new Queue<T>();

      if ((q.Count + 1) == MaxPoolCount)
      {
        throw new InvalidOperationException("Reached Max PoolSize");
      }

      instance.Disassemble();
      instance.gameObject.SetActive(false);
      q.Enqueue(instance);
    }

    protected void Start()
    {
      this.OnDestroyAsObservable()
          .Subscribe(_ => Clear());
      Initialize();
    }
    Queue<T> q;

    /// <summary>
    /// Limit of instace count.
    /// </summary>
    protected int MaxPoolCount => int.MaxValue;


    /// <summary>
    /// Called when clear or disposed, useful for destroy instance or other finalize method.
    /// </summary>
    protected virtual void OnClear(T instance)
    {
      if (instance == null) return;

      var go = instance.gameObject;
      if (go == null) return;
      UnityEngine.Object.Destroy(go);
    }

    /// <summary>
    /// Current pooled object count.
    /// </summary>
    public int Count => (q == null) ? 0 : q.Count;


    /// <summary>
    /// Clear pool.
    /// </summary>
    public void Clear()
    {
      if (q == null) return;
      while (q.Count != 0)
      {
        var instance = q.Dequeue();
        OnClear(instance);
      }
    }

    /// <summary>
    /// Trim pool instances. 
    /// </summary>
    /// <param name="instanceCountRatio">0.0f = clear all ~ 1.0f = live all.</param>
    /// <param name="minSize">Min pool count.</param>
    /// <param name="callOnBeforeRent">If true, call OnBeforeRent before OnClear.</param>
    public void Shrink(float instanceCountRatio, int minSize)
    {
      if (q == null) return;

      if (instanceCountRatio <= 0) instanceCountRatio = 0;
      if (instanceCountRatio >= 1.0f) instanceCountRatio = 1.0f;

      var size = (int)(q.Count * instanceCountRatio);
      size = Math.Max(minSize, size);

      while (q.Count > size)
      {
        var instance = q.Dequeue();
        OnClear(instance);
      }
    }

    /// <summary>
    /// If needs shrink pool frequently, start check timer.
    /// </summary>
    /// <param name="checkInterval">Interval of call Shrink.</param>
    /// <param name="instanceCountRatio">0.0f = clearAll ~ 1.0f = live all.</param>
    /// <param name="minSize">Min pool count.</param>
    /// <param name="callOnBeforeRent">If true, call OnBeforeRent before OnClear.</param>
    public IDisposable StartShrinkTimer(TimeSpan checkInterval, float instanceCountRatio, int minSize)
    {
      return Observable.Interval(checkInterval)
          .Subscribe(_ =>
          {
            Shrink(instanceCountRatio, minSize);
          });
    }

    /// <summary>
    /// Fill pool before rent operation.
    /// </summary>
    /// <param name="preloadCount">Pool instance count.</param>
    /// <param name="threshold">Create count per frame.</param>
    public IObservable<Unit> PreloadAsync(int preloadCount, int threshold)
    {
      if (q == null) q = new Queue<T>(preloadCount);

      return Observable.FromMicroCoroutine<Unit>((observer, cancel) => PreloadCore(preloadCount, threshold, observer, cancel));
    }

    IEnumerator<T> PreloadCore(int preloadCount, int threshold, IObserver<Unit> observer, CancellationToken cancellationToken)
    {
      while (Count < preloadCount && !cancellationToken.IsCancellationRequested)
      {
        var requireCount = preloadCount - Count;
        if (requireCount <= 0) break;

        var createCount = Math.Min(requireCount, threshold);

        for (int i = 0; i < createCount; i++)
        {
          try
          {
            var instance = CreateInstance();
            Despawn(instance);
          }
          catch (Exception ex)
          {
            observer.OnError(ex);
            yield break;
          }
        }
        yield return null; // next frame.
      }

      observer.OnNext(Unit.Default);
      observer.OnCompleted();
    }
  }
}