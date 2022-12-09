using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Collider))]
public class SafetyTrigger : MonoBehaviour
{
  public ReactiveCollection<SafetyTrigger> Triggers;

  void Start()
  {
    this.OnTriggerEnterAsObservable()
      .Subscribe(other =>
      {
        SafetyTrigger x = other.GetComponent<SafetyTrigger>();
        if (x) Triggers.Add(x);
      });

    this.OnTriggerExitAsObservable()
      .Subscribe(other =>
      {
        SafetyTrigger x = other.GetComponent<SafetyTrigger>();
        if (x) Triggers.Remove(x);
      });

    this.OnDisableAsObservable().Subscribe(WhenDisabled);

    void WhenDisabled(Unit _)
    {
      foreach (SafetyTrigger col in Triggers)
      {
        col?.Triggers?.Remove(this);
      }
      Triggers.Clear();
    }
  }
}
