using UniRx;
using UnityEngine;
using Assembly.GameSystem;
using Assembly.Components.Actors.Player;

namespace Assembly.Components.StageGimmicks
{
  public class KandelaarSupply : DiBehavior
  {
    [SerializeField] Kandelaar _actor;
    [SerializeField] ParticleSystem _psSupplyField;
    [SerializeField] SafetyTrigger _supplyFieldTrigger;
    PlayerFlameReceptor _playerFlameReceptor;
    ReactiveProperty<bool> _IsBeingAbsorbed = new ReactiveProperty<bool>();
    public bool isBeingAbsorbed
    {
      get { return _IsBeingAbsorbed.Value; }
      set { _IsBeingAbsorbed.Value = value; }
    }
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
      _IsBeingAbsorbed.Subscribe(b =>
      {
        if (b) { OnDisable(); }
        else
        {
          if (!isActiveAndEnabled) { return; }
          OnEnable();
        }
      });
    }
    void OnEnable()
    {
      if (isBeingAbsorbed) { return; }
      _psSupplyField.Play();
      _supplyFieldTrigger.enabled = true;
    }
    void OnDisable()
    {
      _psSupplyField.Stop();
      _supplyFieldTrigger.enabled = false;
    }
  }
}