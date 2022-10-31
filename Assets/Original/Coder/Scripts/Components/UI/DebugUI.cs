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
        Global.Player.Damagable.OnDamage
            .Scan((total, current) => total + current)
            .Select(x => x.ToString())
            .Subscribe(x => _uiTextPlayerDmg.text = x)
            .AddTo(this);

        Global.Control.HorizontalMoveInput.Subscribe(hmi => {
            LeftUI.color = (hmi == -1) ? gray : white;
            RightUI.color = (hmi == 1) ? gray : white;
        }).AddTo(this);

        Global.Control.GoUpInput.Subscribe(b => JumpUI.color = b ? gray : white).AddTo(this);

    }
}
