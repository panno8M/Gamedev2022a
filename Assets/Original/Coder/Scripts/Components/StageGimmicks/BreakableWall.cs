using System;
using UnityEngine;

using UniRx;
using Assembly.Components.Senses;

namespace Assembly.Components.StageGimmicks
{

  public class BreakableWall : MonoBehaviour
  {
    [SerializeField] DamagableWrapper damagable;

    void Start()
    {
      damagable.OnBroken
          .Delay(TimeSpan.FromSeconds(0.5))
          .Subscribe(_ => Break())
          .AddTo(this);
    }

    void Break()
    {
      Destroy(gameObject);

    }
  }
}