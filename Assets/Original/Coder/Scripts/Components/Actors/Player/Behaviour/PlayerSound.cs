using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Assembly.GameSystem;

namespace Assembly.Components.Actors.Player
{
  public class PlayerSound : DiBehavior
  {
    [SerializeField] PlayerAct _actor;
    [SerializeField] AudioSource seWalk;

    void Start() => InitializeAfter(_actor);

    protected override void Blueprint()
    {
      _actor.ctl.Horizontal.Subscribe(f =>
      {
        if (f == 0)
        {
          if (seWalk.isPlaying) { seWalk.Stop(); }
        }
        else
        {
          if (!seWalk.isPlaying) { seWalk.Play(); }
        }
      });
    }
  }
}