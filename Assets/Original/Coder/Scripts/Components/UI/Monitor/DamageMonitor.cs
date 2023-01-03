using UnityEngine;
using TMPro;
using UniRx;
using Assembly.GameSystem.Damage;
using Assembly.Components.Actors.Player;

namespace Assembly.Components.UI
{
  public class DamageMonitor : MonoBehaviour
  {
    PlayerAct player;
    [Zenject.Inject]
    public void DepsInject(PlayerAct player)
    {
      this.player = player;
    }
    [SerializeField] TMP_Text _text;
    [SerializeField] IDamagable _damagable;
    void Reset()
    {
      if (!_text) { _text = GetComponent<TMP_Text>(); }
      if (_damagable == null) { _damagable = player.life.damagable; }
    }
    void Start()
    {
      Reset();

      _damagable?.TotalDamage
        .Subscribe(x => _text.text = (_damagable.stamina - x) + " / " + _damagable.stamina)
        .AddTo(this);
    }
  }
}