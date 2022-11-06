using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Interactor : MonoBehaviour {
    // TODO: AiSight等と統合できないか？
    public ReactiveProperty<GameObject> Interactable;
    public IObservable<GameObject> OnFind;
    public IObservable<GameObject> OnLost;
    void Awake() {
        this.OnTriggerEnterAsObservable()
            .Subscribe(other => Interactable.Value = other.gameObject);
        this.OnTriggerExitAsObservable()
            .Subscribe(other => Interactable.Value = null);

        OnFind = Interactable
            .Where(x =>  x)
            .Share();
        OnLost = Interactable
            .Pairwise()
            .Where(x => !x.Current)
            .Select(x => x.Previous)
            .Share();

    }
}
