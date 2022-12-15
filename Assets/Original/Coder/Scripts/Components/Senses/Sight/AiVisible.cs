using System;
using UnityEngine;
using UniRx;

namespace Senses
{
  public class AiVisible : MonoBehaviour
  {
    [SerializeField] Transform _center;
    public Transform center => _center;
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