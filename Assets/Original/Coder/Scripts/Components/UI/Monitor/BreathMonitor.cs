using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assembly.Components;
using Assembly.Components.Actors;

public class BreathMonitor : MonoBehaviour
{
  TMP_Text _text;
  PlayerBreath _breath;
  void Start()
  {
    _text = GetComponent<TMP_Text>();
    _breath = Global.Player.GetComponent<PlayerBreath>();
  }

  void Update()
  {
    string timeString = "[" + ((int)(_breath.exhalingProgress.elapsedSeconds * 1000)).ToString().PadLeft(4, '0') + " ms]";
    if (_breath.exhalingProgress.isIncreasing)
    {
      _text.SetText("Breath: Exhaling     " + timeString);
      _text.color = Color.red;
    }
    else if (_breath.exhalingProgress.factor != 0)
    {
      _text.SetText("Breath: Cooling Down " + timeString);
      _text.color = Color.cyan;
    }
    else
    {
      _text.SetText("Breath: Available");
      _text.color = Color.white;
    }
  }


}
