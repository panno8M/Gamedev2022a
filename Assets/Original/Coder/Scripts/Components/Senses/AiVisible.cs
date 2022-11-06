using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AiVisible: MonoBehaviour
{
    public ReactiveProperty<bool> IsSeen;
    public IObservable<Unit> OnSeen;
    public IObservable<Unit> OnLost;

    void Awake() {
        OnSeen = IsSeen.Where(x =>  x).AsUnitObservable().Share();
        OnLost = IsSeen.Where(x => !x).AsUnitObservable().Share();
    }

    public void Find(bool b = true) { IsSeen.Value = b; }
}
