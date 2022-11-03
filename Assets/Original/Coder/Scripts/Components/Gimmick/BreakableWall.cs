using System;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class BreakableWall : MonoBehaviour
{
    [SerializeField] Damagable damagable;

    void Start()
    {
        var slowupdate = this
            .UpdateAsObservable()
            .ThrottleFirst(TimeSpan.FromSeconds(.5f))
            .Share();

        damagable.OnBroken
            .Delay(TimeSpan.FromSeconds(0.5))
            .Subscribe(_ => Break())
            .AddTo(this);
    }

    void Break(){
        Destroy(gameObject);
        
    }
}
