using System;
using System.Collections.Generic;

using UniRx.InternalUtil;
namespace UniRx.Ex.ReactiveNeuro
{
  public sealed class NeuroSubject<T> : ISubject<Context<T>>, IDisposable, IOptimizedObservable<Context<T>>
  {
    object observerLock = new object();

    bool isStopped;
    bool isDisposed;
    Exception lastError;
    IObserver<Context<T>> outObserver = EmptyObserver<Context<T>>.Instance;

    Context<T> _contextCopy;

    public bool HasObservers
    {
      get
      {
        return !(outObserver is EmptyObserver<Context<T>>) && !isStopped && !isDisposed;
      }
    }

    public void OnCompleted()
    {
      IObserver<Context<T>> old;
      lock (observerLock)
      {
        ThrowIfDisposed();
        if (isStopped) return;

        old = outObserver;
        outObserver = EmptyObserver<Context<T>>.Instance;
        isStopped = true;
      }

      old.OnCompleted();
    }

    public void OnError(Exception error)
    {
      if (error == null) throw new ArgumentNullException("error");

      IObserver<Context<T>> old;
      lock (observerLock)
      {
        ThrowIfDisposed();
        if (isStopped) return;

        old = outObserver;
        outObserver = EmptyObserver<Context<T>>.Instance;
        isStopped = true;
        lastError = error;
      }

      old.OnError(error);
    }

    public void OnNext(Context<T> value)
    {
      _contextCopy = value;
      outObserver.OnNext(value);
    }

    public IDisposable Subscribe(IObserver<Context<T>> observer)
    {
      if (observer == null) throw new ArgumentNullException("observer");

      var ex = default(Exception);

      lock (observerLock)
      {
        ThrowIfDisposed();
        if (!isStopped)
        {
          var listObserver = outObserver as NeuroObserver<T>;
          if (listObserver != null)
          {
            listObserver.Add(observer);
          }
          else
          {
            var current = outObserver;
            outObserver = new PriorityObserver<T>(new List<IObserver<Context<T>>>(new[] { current, observer }));
          }

          return new Subscription(this, observer);
        }

        ex = lastError;
      }

      if (ex != null)
      {
        observer.OnError(ex);
      }
      else
      {
        observer.OnCompleted();
      }

      return Disposable.Empty;
    }

    public void Dispose()
    {
      lock (observerLock)
      {
        isDisposed = true;
        outObserver = DisposedObserver<Context<T>>.Instance;
      }
    }

    void ThrowIfDisposed()
    {
      if (isDisposed) throw new ObjectDisposedException("");
    }

    public bool IsRequiredSubscribeOnCurrentThread()
    {
      return false;
    }

    class Subscription : IDisposable
    {
      readonly object gate = new object();
      NeuroSubject<T> parent;
      IObserver<Context<T>> unsubscribeTarget;

      public Subscription(NeuroSubject<T> parent, IObserver<Context<T>> unsubscribeTarget)
      {
        this.parent = parent;
        this.unsubscribeTarget = unsubscribeTarget;
      }

      public void Dispose()
      {
        lock (gate)
        {
          if (parent == null) { return; }
          lock (parent.observerLock)
          {
            var listObserver = parent.outObserver as NeuroObserver<T>;
            if (listObserver != null)
            {
              listObserver.Remove(unsubscribeTarget);
            }
            else
            {
              parent.outObserver = EmptyObserver<Context<T>>.Instance;
            }

            unsubscribeTarget = null;
            parent = null;
          }
        }
      }
    }
  }


  public class NeuroObserver<T> : IObserver<Context<T>>
  {
    protected readonly List<IObserver<Context<T>>> _observers;
    protected readonly List<Context<T>> _contexts;

    public virtual NeuroObserver<T> Create(List<IObserver<Context<T>>> observers)
    {
      return new NeuroObserver<T>(observers);
    }

    public NeuroObserver(List<IObserver<Context<T>>> observers)
    {
      _observers = observers;
      _contexts = new List<Context<T>>(observers.Count);
      for (int i = 0; i != observers.Count; i++)
      {
        _contexts.Add(new Context<T>());
      }
    }

    public void OnCompleted()
    {
      foreach (var observer in _observers)
      {
        observer.OnCompleted();
      }
    }

    public void OnError(Exception error)
    {
      foreach (var observer in _observers)
      {
        observer.OnError(error);
      }
    }

    public virtual void OnNext(Context<T> value) { }
    internal void Add(IObserver<Context<T>> observer)
    {
      _observers.Add(observer);
      _contexts.Add(new Context<T>());
    }

    internal void Remove(IObserver<Context<T>> observer)
    {
      var idx = _observers.IndexOf(observer);
      _observers.RemoveAt(idx);
      _contexts.RemoveAt(idx);
    }
  }
  public class PriorityObserver<T> : NeuroObserver<T>
  {
    public PriorityObserver(List<IObserver<Context<T>>> observers)
        : base(observers) { }

    public override NeuroObserver<T> Create(List<IObserver<Context<T>>> observers)
    {
      return new PriorityObserver<T>(observers) as NeuroObserver<T>;
    }

    public override void OnNext(Context<T> value)
    {
      if (value.TryContinue()) return;
      for (int i = 0; i != _observers.Count; i++)
      {
        switch (value.Try(_observers[i], _contexts[i]))
        {
          case TaskStatus.Ready:
          case TaskStatus.Failure:
            break;
          case TaskStatus.Success:
            return;
          case TaskStatus.Running:
            return;
        }
      }
      value.Status = TaskStatus.Failure;
    }
  }


  public static class NeuroObservableExtensions
  {
    public static IObservable<Context<T>> ActivateIf<T>(this IObservable<Context<T>> source, Func<Context<T>, bool> predicate)
    {
      return source.Where(value => predicate(value) || value.Status == TaskStatus.Running);
    }
  }
}
