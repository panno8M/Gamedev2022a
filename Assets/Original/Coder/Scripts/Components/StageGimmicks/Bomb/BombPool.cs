using UniRx;
using Assembly.GameSystem;

namespace Assembly.Components.StageGimmicks
{
  public class BombPool : GameObjectPool<Bomb>
  {
    protected override Bomb CreateInstance()
    {
      throw new System.NotImplementedException();
    }
    protected override void Blueprint()
    {
      Global.PlayerPool.OnSpawn.Subscribe(_ =>
      {
        try
        {
          while (true) Spawn();
        }
        catch (System.NotImplementedException) {}

      }).AddTo(this);
    }
  }
}