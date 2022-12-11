using System.Collections.Generic;
using UniRx;
using Assembly.GameSystem.ObjectPool;

namespace Assembly.Components.StageGimmicks
{
  public class BombPool : GameObjectPool<Bomb>
  {
    protected override Bomb CreateInstance()
    {
      throw new System.NotImplementedException();
    }
    protected override void InfuseInfoOnSpawn(Bomb newObj, ObjectCreateInfo info)
    {
      newObj.transform.position = info.position;
    }
    protected override void Blueprint()
    {
      Global.PlayerPool.OnSpawn.Subscribe(_ =>
      {
        try
        {
          while (true) { Spawn(ObjectCreateInfo.None); }
        }
        catch (System.NotImplementedException) { }

      }).AddTo(this);
    }
  }
}