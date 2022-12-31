using UnityEngine;
using TMPro;
using UniRx;
using Assembly.GameSystem.Damage;
using Assembly.Components.Pools;

namespace Assembly.Components.UI
{
  public class DamageMonitor : MonoBehaviour
  {
    PlayerPool playerPool;
    [Zenject.Inject]
    public void DepsInject(PlayerPool playerPool)
    {
      this.playerPool = playerPool;
    }
    [SerializeField] TMP_Text _text;
    [SerializeField] IDamagable _damagable;
    void Reset()
    {
      if (!_text) { _text = GetComponent<TMP_Text>(); }
      if (_damagable == null) { _damagable = playerPool.player.life.damagable; }
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