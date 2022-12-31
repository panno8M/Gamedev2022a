using Assembly.Components.Actors.Player;
using Assembly.GameSystem.Input;

namespace Assembly.Components
{
  public static class Global
  {
    public static InputControl Control => InputControl.Instance;
    public static PlayerAct Player => PlayerAct.pool.player;
  }
}