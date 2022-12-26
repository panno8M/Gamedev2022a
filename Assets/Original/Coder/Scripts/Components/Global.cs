using Assembly.Components.Actors.Player;
using Assembly.Components.Pools;
using Assembly.GameSystem.Input;

namespace Assembly.Components
{
  public static class Global
  {
    public static InputControl Control => InputControl.Instance;
    public static PlayerAct Player => Pool.player.player;
    public static Camctl Cameraman => Camctl.Instance;
  }
}