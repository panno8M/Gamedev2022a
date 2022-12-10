using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components.Actors
{
  public class PlayerFlameReceptor : ActorBehavior<PlayerAct>
  {
    bool _flameAvailable;
    public bool flameAvailable
    {
      get
      {
        return _flameAvailable;
      }
      set
      {
        _flame.enabled = value;
        _flameAvailable = value;
      }
    }

    [SerializeField] Renderer _flame;
    protected override void Blueprint()
    {
      _flame.enabled = false;
    }
  }
}