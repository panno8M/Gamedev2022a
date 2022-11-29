using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Assembly.Components.UI
{
  public class DebugUI : MonoBehaviour
  {

    public Image JumpUI;

    Color white = new Color(1f, 1f, 1f);
    Color gray = new Color(.5f, .5f, .5f);
    [SerializeField] Text _uiTextPlayerDmg;

    void Start()
    {
      Global.Player.damagable.TotalDamage
          .Select(x => x.ToString())
          .Subscribe(x => _uiTextPlayerDmg.text = x)
          .AddTo(this);

      Global.Control.GoUpInput.Subscribe(b => JumpUI.color = b ? gray : white).AddTo(this);

    }
  }
}