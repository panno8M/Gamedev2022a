using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Assembly.GameSystem
{
  public abstract class DiBehavior : MonoBehaviour
  {
    List<IObserver<Unit>> _AfterInitializeObservers = new List<IObserver<Unit>>();
    public IObservable<Unit> OnInitialized => Observable.Create<Unit>(observer =>
    {
      if (initialized)
      {
        observer.OnNext(Unit.Default);
        observer.OnCompleted();
      }
      else
      { _AfterInitializeObservers.Add(observer); }

      return Disposable.Create(()
        => _AfterInitializeObservers.Remove(observer));
    });
    public IDisposable InitializeAfter(DiBehavior other)
      => other.OnInitialized.Subscribe(_ => Initialize());
    public bool initialized { get; private set; }
    public void Initialize()
    {
      if (initialized) { return; }
      Blueprint();
      initialized = true;
      for (int i = 0; i < _AfterInitializeObservers.Count; i++)
      {
        _AfterInitializeObservers[i].OnNext(Unit.Default);
        _AfterInitializeObservers[i].OnCompleted();
      }
      _AfterInitializeObservers.Clear();
    }
    protected virtual void Blueprint() { }

    Rigidbody _rigidbody;
    Transform _transform;

    public new Rigidbody rigidbody
    {
      get
      {
        if (_rigidbody == null)
        {
          _rigidbody = GetComponent<Rigidbody>();
        }
        return _rigidbody;
      }
    }
    public new Transform transform
    {
      get
      {
        if (_transform == null)
        {
          _transform = GetComponent<Transform>();
        }
        return _transform;
      }
    }
  }
  public static class GameObjectExtensions
  {
    public static void SetActive(this Behaviour behaviour, bool value)
      => behaviour.gameObject.SetActive(value);
    public static bool SetActiveOnce(this Behaviour behaviour, bool value)
    {
      if (behaviour.gameObject.activeSelf == value) { return false; }
      behaviour.SetActive(value);
      return true;
    }

    public static void Activate(this Behaviour behaviour) => behaviour.SetActive(true);
    public static void Disactivate(this Behaviour behaviour) => behaviour.SetActive(false);
    public static bool ActivateOnce(this Behaviour behaviour) => behaviour.SetActiveOnce(true);
    public static bool DisactivateOnce(this Behaviour behaviour) => behaviour.SetActiveOnce(false);

    public static T Instantiate<T>(this GameObject prefab)
      where T : DiBehavior
    {
      T result = GameObject.Instantiate(prefab).GetComponent<T>();
      result.Initialize();
      return result;
    }

    public static T Instantiate<T>(this GameObject prefab, Transform parent)
      where T : DiBehavior
    {
      T result = prefab.Instantiate<T>();
      result.transform.SetParent(parent);
      return result;
    }

    public static T Instantiate<T>(this T prefab)
      where T : DiBehavior
    {
      T result = GameObject.Instantiate(prefab).GetComponent<T>();
      result.Initialize();
      return result;
    }
  }
}