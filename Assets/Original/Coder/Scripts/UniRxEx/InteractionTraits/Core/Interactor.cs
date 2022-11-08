using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;

namespace UniRx.Ex.InteractionTraits.Core
{
  [RequireComponent(typeof(Collider))]
  public class Interactor : MonoBehaviour
  {
    // TODO: AiSight等と統合できないか？
    ReactiveCollection<Interactable> _interactables = new ReactiveCollection<Interactable>();
    public ReactiveCollection<Interactable> interactables => _interactables;

    Subject<Unit> _OnForget = new Subject<Unit>();
    public IObservable<Unit> OnForget => _OnForget;

    public void Forget()
    {
      _OnForget.OnNext(Unit.Default);
      _interactables.Clear();
    }


    public void Process()
    {
      if (isInteractable)
      {
        Interact();
      }
      else
      {
        Discard();
      }
    }
    public bool isInteractable => (interactables.Count != 0) && (_holder?.isInteractable ?? false);
    public void Discard()
    {
      _holder?.Discard();
    }
    public void Interact()
    {
      _holder?.Interact();
    }

    #region buffers
    [SerializeField] HolderModule _holder;
    public HolderModule holder => _holder;
    #endregion

    void Awake()
    {
      GetComponent<Collider>().isTrigger = true;
      this.OnTriggerEnterAsObservable()
          .Subscribe(other =>
          {
            _interactables.Add(other.GetComponent<Interactable>());
          });
      this.OnTriggerExitAsObservable()
          .Subscribe(other =>
          {
            _interactables.Remove(other.GetComponent<Interactable>());
          });

    }

    void Reset()
    {
      SetDefaultComponent();
    }

    void SetDefaultComponent()
    {
      _holder = GetComponent<HolderModule>();
    }

  }
}