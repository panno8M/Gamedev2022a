using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components.Actors
{
  public class PlayerFlameReceptor : ActorBehavior<PlayerAct>
  {
    [SerializeField] Renderer _flame;
    Vector3 flameDefaultScale;

    [SerializeField][Range(0, 1)] float _flameQuantity;

    public float flameQuantity
    {
      get { return _flameQuantity; }
      set
      {
        _flameQuantity = value;
        _flame.enabled = value != 0;
        if (flameDefaultScale == Vector3.zero)
        {
          flameDefaultScale = _flame.transform.localScale;
        }
        _flame.transform.localScale = flameDefaultScale * value;
      }
    }

    protected override void Blueprint()
    {
      _flame.enabled = false;
    }
  }
}