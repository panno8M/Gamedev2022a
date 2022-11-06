using System;
using UnityEngine;
using UniRx;

namespace Assembly.Components.Senses
{
  public class AiVisible : MonoBehaviour
  {
    public ReactiveProperty<bool> IsSeen;
    public IObservable<Unit> OnSeen;
    public IObservable<Unit> OnLost;

    void Awake()
    {
      OnSeen = IsSeen.Where(x => x).AsUnitObservable().Share();
      OnLost = IsSeen.Where(x => !x).AsUnitObservable().Share();
    }

    public void Find(bool b = true) { IsSeen.Value = b; }
  }
}