using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Interactor : MonoBehaviour {
    // TODO: AiSight等と統合できないか？
    public ReactiveProperty<Interactable> Interactable;
    public IObservable<Interactable> OnFind;
    public IObservable<Interactable> OnLost;

    void Awake() {
        this.OnTriggerEnterAsObservable()
            .Subscribe(other => Interactable.Value = other.GetComponent<Interactable>());
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

    public void Interact() {
        if (Interactable.Value) {
            Interactable.Value.Interact(this);
        }
    }

    #region reactions

    #region hold
    public ReactiveProperty<Rigidbody> HoldingItem = new ReactiveProperty<Rigidbody>();
    public IObservable<Rigidbody> OnHoldRequested => HoldingItem.Where(x=>x);
    public void Hold(Rigidbody item) {
        HoldingItem.Value = item;
    }
    #endregion

    #endregion
}
