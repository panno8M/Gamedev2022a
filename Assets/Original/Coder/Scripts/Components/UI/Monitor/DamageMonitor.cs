using UnityEngine;
using TMPro;
using UniRx;
using Assembly.GameSystem.Damage;

namespace Assembly.Components.UI
{
  public class DamageMonitor : MonoBehaviour
  {
    [SerializeField] TMP_Text _text;
    [SerializeField] IDamagable _damagable;
    void Reset()
    {
      if (!_text) { _text = GetComponent<TMP_Text>(); }
      if (_damagable == null) { _damagable = Global.Player.life.damagable; }
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