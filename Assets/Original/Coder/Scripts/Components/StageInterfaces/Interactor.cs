using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Collider))]
public class Interactor : MonoBehaviour {
    // TODO: AiSight等と統合できないか？
    public ReactiveProperty<Interactable> Interactable;
    public IObservable<Interactable> OnFind;
    public IObservable<Interactable> OnLost;

    Subject<Interactable> _react = new Subject<Interactable>();
    public IObservable<Interactable> OnReacted => _react;

    void Awake() {
        GetComponent<Collider>().isTrigger = true;
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
    public void React(Interactable interactable) {
        _react.OnNext(interactable);
    }

    #region reactions

    #region hold/release
    public ReactiveProperty<Rigidbody> HoldingItem = new ReactiveProperty<Rigidbody>();
    public IObservable<Rigidbody> OnHoldRequested => HoldingItem.Where(x=>x);
    public IObservable<Rigidbody> OnReleaseRequested => HoldingItem
        .Pairwise()
        .Where(x=>!x.Current)
        .Select(x=>x.Previous);
    public void Hold(Rigidbody item) {
        HoldingItem.Value = item;
    }
    public void Release() {
        HoldingItem.Value = null;
    }
    #endregion

    #endregion
}
