using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Assembly.Components.Actors;

namespace Assembly.Components.UI
{
  public class OverrayCanvas : MonoBehaviour
  {
    [SerializeField] Image imgFeather;

    Color featherBaseColor = Color.white;
    Color featherPressColor = new Color(0.4f, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
      Global.Control.RespawnInput
          .Subscribe(b => imgFeather.color = b ? featherPressColor : featherBaseColor)
          .AddTo(this);
    }
  }
}
