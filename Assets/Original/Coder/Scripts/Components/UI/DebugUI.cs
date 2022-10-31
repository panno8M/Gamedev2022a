using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class DebugUI : MonoBehaviour
{

    public Image RightUI;
    public Image LeftUI;
    public Image JumpUI;

    Color white = new Color(1f, 1f, 1f);
    Color gray = new Color(.5f, .5f, .5f);
    [SerializeField] Text _uiTextPlayerDmg;

    void Start() {
        Player.Instance.Damagable.OnDamage
            .Scan((total, current) => total + current)
            .Select(x => x.ToString())
            .Subscribe(x => _uiTextPlayerDmg.text = x)
            .AddTo(this);

        Player.Control.HorizontalMoveInput.Subscribe(hmi => {
            LeftUI.color = (hmi == -1) ? gray : white;
            RightUI.color = (hmi == 1) ? gray : white;
        }).AddTo(this);

        Player.Control.input.Player.GoUp.started +=
            context => JumpUI.color = context.ReadValue<float>() == 1 ? gray : white;
        Player.Control.input.Player.GoUp.canceled +=
            context => JumpUI.color = context.ReadValue<float>() == 1 ? gray : white;
    }
}
