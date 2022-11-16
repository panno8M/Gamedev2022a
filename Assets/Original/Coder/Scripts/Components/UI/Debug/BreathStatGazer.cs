using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assembly.Components;
using Assembly.Components.Actors.Player;

public class BreathStatGazer : MonoBehaviour
{
    TMP_Text _text;
    PlayerBreath _breath;
    void Start()
    {
        _text = GetComponent<TMP_Text>();
        _breath = Global.Player.GetComponent<PlayerBreath>();
    }

    void Update() {
        if (_breath.isCoolingDown) {
            _text.SetText("Breath: Cooling Down [" + _breath.msecCooldown.ToString().PadLeft(4, '0') + " ms]");
            _text.color = Color.cyan;
        }
        else if (_breath.IsExhaling.Value) {
            _text.SetText("Breath: Exhaling     [" + _breath.msecExhaling.ToString().PadLeft(4, '0') + " ms]");
            _text.color = Color.red;
        }
        else {
            _text.SetText("Breath: Available");
            _text.color = Color.white;
        }
    }


}
