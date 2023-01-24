using System.ComponentModel;
using UnityEngine;

namespace Assembly.GameSystem
{
  public enum Layer
  {
    [Description("Default")] Default,
    [Description("TransparentFX")] TransparentFX,
    [Description("Ignore Raycast")] IgnoreRaycast,
    [Description("Physics/Dynamics")] PhsDynamics,
    [Description("Water")] Water,
    [Description("UI")] UI,
    [Description("Physics/Stage")] PhsStage,
    [Description("Physics/Trigger")] PhsTrigger,
    [Description("Control/Target")] CtlTarget,
    [Description("Senses/AiVisible")] SnsAiVisible,
    [Description("Senses/Damagable")] SnsDamagable,
    [Description("ScreenToStageConverter")] ScreenToStageConverter,
    [Description("Senses/PlayerDamagable")] SnsPlayerDamagable,
    [Description("Senses/Interactable")] SnsInteractable,
    [Description("Senses/Interactor")] SnsInteractor,
    [Description("Physics/Item")] PhsItem,
    [Description("Senses/Damager")] SnsDamager,
    [Description("Physics/Player")] PhsPlayer,
  }

  public struct Layers
  {
    int value;
    public Layers(params Layer[] layers)
    {
      value = 0;
      foreach (var layer in layers)
      {
        value |= 1 << (int)layer;
      }
    }
    public static implicit operator int(Layers layers)
    {
      return layers.value;
    }
    public static implicit operator LayerMask(Layers layers)
    {
      return layers.value;
    }
  }

  public static class LayerExtensions
  {
    // static string[] nameCache = new string[]
    public static string GetName(this Layer x)
    {
      var gm = x.GetType().GetMember(x.ToString());
      var attributes = gm[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
      return ((DescriptionAttribute)attributes[0]).Description;
    }
  }
}