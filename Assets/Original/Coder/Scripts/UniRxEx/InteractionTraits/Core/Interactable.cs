using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRx.Ex.InteractionTraits.Core
{
  [RequireComponent(typeof(Collider))]
  public class Interactable : MonoBehaviour
  {
    public ReactiveProperty<Interactor> Interactor = new ReactiveProperty<Interactor>();

    #region buffers
    [SerializeField] HoldableModule _holdable;
    public HoldableModule holdable => _holdable;
    #endregion

    void Reset()
    {
      SetDefaultComponent();
    }

    void SetDefaultComponent()
    {
      _holdable = GetComponent<HoldableModule>();
    }
  }
}