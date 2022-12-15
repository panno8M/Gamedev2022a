using System;
using UniRx;
using UnityEngine;
using Assembly.GameSystem;
using Assembly.GameSystem.Damage;
using Assembly.Components.Actors;

namespace Assembly.Components.StageGimmicks
{
  public class KandelaarSupply : DiBehavior
  {
    [SerializeField] Kandelaar _actor;
    [SerializeField] ParticleSystem _psSupplyField;
    [SerializeField] SafetyTrigger _supplyFieldTrigger;
    PlayerFlameReceptor _playerFlameReceptor;
    protected override void Blueprint()
    {

      _supplyFieldTrigger.OnEnter
        .Subscribe(trigger =>
        {
          if (!isActiveAndEnabled) { return; }
          if (!_playerFlameReceptor)
          {
            _playerFlameReceptor = trigger.GetComponent<PlayerFlameReceptor>();
          }
          if (_playerFlameReceptor)
          {
            _playerFlameReceptor.flameQuantity = 1;
          }
        });
    }
    void OnEnable()
    {
      _psSupplyField.Play();
      _supplyFieldTrigger.raw.enabled = true;
    }
    void OnDisable()
    {
      _psSupplyField.Stop();
      _supplyFieldTrigger.raw.enabled = false;
    }
  }
}