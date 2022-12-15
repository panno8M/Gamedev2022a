using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.StageGimmicks
{
  public class BombPool : GameObjectPool<Bomb>
  {
    [SerializeField] Subject<Unit> _OnClear = new Subject<Unit>();
    public void Clear() { _OnClear.OnNext(Unit.Default); }
    public IObservable<Unit> OnClear => _OnClear;
    protected override Bomb CreateInstance()
    {
      return prefab.Instantiate<Bomb>();
    }
    protected override void InfuseInfoOnSpawn(Bomb newObj, ObjectCreateInfo info)
    {
      newObj.transform.position = (info.userData as Transform).position;
      newObj.transform.rotation = (info.userData as Transform).rotation;
    }
    protected override void Blueprint()
    {
      Global.PlayerPool.OnSpawn.Subscribe(_ => Clear());
    }
  }
}