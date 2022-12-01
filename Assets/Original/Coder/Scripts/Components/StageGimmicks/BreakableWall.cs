using System;
using UnityEngine;

using UniRx;
using Senses;

namespace Assembly.Components.StageGimmicks
{

  public class BreakableWall : MonoBehaviour
  {
    [SerializeField] DamagableComponent damagable;

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