using Assembly.Components.Actors.Player;
using Assembly.Components.Pools;
using Assembly.GameSystem.Input;

namespace Assembly.Components
{
  public static class Global
  {
    public static InputControl Control => InputControl.Instance;
    public static PlayerAct Player => Pool.Player.player;
    public static Camctl Cameraman => Camctl.Instance;
  }

  public static class Pool
  {
    public static PlayerPool Player => PlayerPool.Instance as PlayerPool;
    public static BombPool Bomb => BombPool.Instance as BombPool;
    public static WaterBallPool WaterBall => WaterBallPool.Instance as WaterBallPool;

  }
}