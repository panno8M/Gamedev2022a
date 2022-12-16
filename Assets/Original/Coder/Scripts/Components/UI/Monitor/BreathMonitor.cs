using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assembly.Components;
using Assembly.Components.Actors;

public class BreathMonitor : MonoBehaviour
{
  TMP_Text _text;
  void Start()
  {
    _text = GetComponent<TMP_Text>();
  }

  void Update()
  {
    PlayerBreath mouse = Global.Player.mouse;
    string timeString = "[" + ((int)(mouse.exhalingProgress.elapsedSeconds * 1000)).ToString().PadLeft(4, '0') + " ms]";
    if (mouse.exhalingProgress.isIncreasing)
    {
      _text.SetText("Breath: Exhaling     " + timeString);
      _text.color = Color.red;
    }
    else if (mouse.exhalingProgress.UpdFactor() != 0)
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
