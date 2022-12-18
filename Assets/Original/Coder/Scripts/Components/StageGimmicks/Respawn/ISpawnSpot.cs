using UnityEngine;

namespace Assembly.Components.StageGimmicks
{
  public interface ISpawnSpot
  {
    Vector3 spawnPosition { get; }
    void OnActivate();
    void OnDeactivate();
  }
  public static class ISpawnSpotExtensions
  {
    public static void Activate(this ISpawnSpot spot)
    {
        if (spot != null) spot.OnActivate();
    }
    public static void Deactivate(this ISpawnSpot spot)
    {
        if (spot != null) spot.OnDeactivate();
    }
  }
}