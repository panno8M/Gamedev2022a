using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Collider))]
public class SafetyTrigger : MonoBehaviour
{
  Collider _raw;
  public Collider raw => _raw ?? (_raw = GetComponent<Collider>());
  public List<SafetyTrigger> triggers = new List<SafetyTrigger>();

  Subject<SafetyTrigger> _OnEnter = new Subject<SafetyTrigger>();
  Subject<SafetyTrigger> _OnExit = new Subject<SafetyTrigger>();

  public IObservable<SafetyTrigger> OnEnter => _OnEnter;
  public IObservable<SafetyTrigger> OnExit => _OnExit;

  List<SafetyTrigger> exitNotifyQueue = new List<SafetyTrigger>();
  Queue<SafetyTrigger> enterNotifyQueue = new Queue<SafetyTrigger>();
  void Remove(SafetyTrigger trigger)
  {
    exitNotifyQueue.Add(trigger);
  }
  void Add(SafetyTrigger trigger)
  {
    if (!exitNotifyQueue.Remove(trigger))
    {
      enterNotifyQueue.Enqueue(trigger);
    }
  }
  void NoticeExit()
  {
    if (exitNotifyQueue.Count == 0) { return; }
    foreach (SafetyTrigger trigger in exitNotifyQueue)
    {
      triggers.Remove(trigger);
      _OnExit.OnNext(trigger);
    }
    exitNotifyQueue.Clear();
  }
  void NoticeEnter()
  {
    while (enterNotifyQueue.Count != 0)
    {
      SafetyTrigger trigger = enterNotifyQueue.Dequeue();
      triggers.Add(trigger);
      _OnEnter.OnNext(trigger);
    }
  }

  void Start()
  {
    this.OnTriggerEnterAsObservable()
      .Subscribe(other =>
      {
        SafetyTrigger x = other.GetComponent<SafetyTrigger>();
        if (x) Add(x);
      });

    this.OnTriggerExitAsObservable()
      .Subscribe(other =>
      {
        SafetyTrigger x = other.GetComponent<SafetyTrigger>();
        if (x) Remove(x);
      });

    this.OnDisableAsObservable().Subscribe(WhenDisabled);

    void WhenDisabled(Unit _)
    {
      foreach (SafetyTrigger col in triggers)
      {
        col?.Remove(this);
        Remove(col);
      }
    }

    Observable.EveryFixedUpdate()
        .Subscribe(_ =>
        {
          NoticeEnter();
          NoticeExit();
        }).AddTo(this);
  }
}
