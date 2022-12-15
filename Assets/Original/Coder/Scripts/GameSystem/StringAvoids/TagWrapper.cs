using System.Collections.Generic;

namespace Assembly.GameSystem
{
  public enum Tag
  {
    Untagged,
    Player,
    Kandelaar,
    CtlvolDroneMovable,
    CtlvolCamera,
  }

  public static class TagExtensions
  {
    static string[] names = {
    "Untagged",
    "Player",
    "Kandelaar",
    "Ctlvol/DroneMovable",
    "Ctlvol/Camera",
  };

    public static string GetName(this Tag x)
    {
      return names[(int)x];
    }

    public static bool Contains(this List<Tag> tags, string tagstr)
    {
      foreach (var tag in tags)
      {
        if (tag.GetName() == tagstr)
        {
          return true;
        }
      }
      return false;
    }
  }
}