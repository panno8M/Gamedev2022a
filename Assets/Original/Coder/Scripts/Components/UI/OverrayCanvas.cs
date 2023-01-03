using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Assembly.GameSystem.Input;

namespace Assembly.Components.UI
{
  public class OverrayCanvas : MonoBehaviour
  {
    InputControl control;
    [Zenject.Inject]
    public void DepsInject(InputControl control)
    {
      this.control = control;
    }

    [SerializeField] Image imgFeather;

    Color featherBaseColor = Color.white;
    Color featherPressColor = new Color(0.4f, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
      control.RespawnInput
          .Subscribe(b => imgFeather.color = b ? featherPressColor : featherBaseColor)
          .AddTo(this);
    }
  }
}
