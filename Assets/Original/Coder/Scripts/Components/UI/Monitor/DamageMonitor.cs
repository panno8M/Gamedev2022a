using UnityEngine;
using TMPro;
using UniRx;
using Senses.Pain;

namespace Assembly.Components.UI
{
  public class DamageMonitor : MonoBehaviour
  {
    [SerializeField] TMP_Text _text;
    [SerializeField] IDamagable _damagable;
    void Reset()
    {
      if (!_text) { _text = GetComponent<TMP_Text>(); }
      if (_damagable == null) { _damagable = Global.Player.damagable; }
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