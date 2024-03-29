#if UNITY_EDITOR
// #define DEBUG_SAFETY_TRIGGER
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Collider))]
public class SafetyTrigger : MonoBehaviour
{
#if DEBUG_SAFETY_TRIGGER
  [SerializeField]
#endif
  List<SafetyTrigger> _ohters = new List<SafetyTrigger>();
  Collider[] _colliders;

  Subject<SafetyTrigger> _OnEnter = new Subject<SafetyTrigger>();
  Subject<Unit> _OnStay = new Subject<Unit>();
  Subject<SafetyTrigger> _OnExit = new Subject<SafetyTrigger>();

  List<SafetyTrigger> exitNotifyQueue = new List<SafetyTrigger>();
  Queue<SafetyTrigger> enterNotifyQueue = new Queue<SafetyTrigger>();

  bool needsCleanUp;

  public Collider[] colliders => _colliders ?? (_colliders = GetComponents<Collider>());
  public List<SafetyTrigger> others => _ohters;

  public IObservable<SafetyTrigger> OnEnter => _OnEnter;
  public IObservable<Unit> OnStay => _OnStay;
  public IObservable<SafetyTrigger> OnExit => _OnExit;

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
      others.Remove(trigger);
      _OnExit.OnNext(trigger);
    }
    exitNotifyQueue.Clear();
  }
  void NoticeEnter()
  {
    while (enterNotifyQueue.Count != 0)
    {
      SafetyTrigger trigger = enterNotifyQueue.Dequeue();
      others.Add(trigger);
      _OnEnter.OnNext(trigger);
    }
  }
  void NoticeStay() => _OnStay.OnNext(Unit.Default);

  void Start()
  {
    this.OnTriggerEnterAsObservable()
      .Subscribe(other =>
      {
        SafetyTrigger x = other.GetComponent<SafetyTrigger>();
        if (x) { Add(x); x.Add(this); }
      });

    this.OnTriggerExitAsObservable()
      .Subscribe(other =>
      {
        SafetyTrigger x = other.GetComponent<SafetyTrigger>();
        if (x) { Remove(x); x.Remove(this); }
      });

    Observable.EveryFixedUpdate()
      .Where(_ => isActiveAndEnabled || needsCleanUp)
      .Subscribe(_ =>
      {
#if DEBUG_SAFETY_TRIGGER
        var text =
          $"[{name}" + (
          isActiveAndEnabled ?
            "<color=#0F0>●</color>" :
          needsCleanUp ?
            "<color=#F00>●</color>" :
          "") + "]";

        var enterCount = enterNotifyQueue.Count;
        NoticeEnter();
        var stayCount = others.Count;
        NoticeStay();
        var exitCount = exitNotifyQueue.Count;
        NoticeExit();

        text += $"<{enterCount},{stayCount},{exitCount}>";
        Debug.Log(text);
#else
        NoticeEnter();
        NoticeStay();
        NoticeExit();
#endif
        needsCleanUp = false;
      }).AddTo(this);
  }

  void OnEnable()
  {
    foreach (Collider collider in colliders)
    { collider.enabled = true; }
  }

  void OnDisable()
  {
    foreach (SafetyTrigger other in others)
    {
      other?.Remove(this);
      Remove(other);
    }
    foreach (Collider collider in colliders)
    { collider.enabled = false; }
    needsCleanUp = true;
  }

#if UNITY_EDITOR
  public bool showColliderOutlines;
  public Color outlineColor;

  void DrawOutline()
  {
    if (!showColliderOutlines) { return; }
    Gizmos.color = outlineColor;
    _colliders = GetComponents<Collider>();
    foreach (Collider collider in colliders)
    {
      BoxCollider bc = collider as BoxCollider;
      if (bc)
      {
        Gizmos.DrawWireCube(transform.position + bc.center, bc.size);
      }
    }
  }

  void OnDrawGizmos()
  {
    DrawOutline();
  }
#endif
}
