using System.ComponentModel;
public enum Layer {
    [Description("Default")]         Default,
    [Description("TransparentFX")]   TransparentFX,
    [Description("Ignore Raycast")]  IgnoreRaycast,
    [Description("Player")]          Player,
    [Description("Water")]           Water,
    [Description("UI")]              UI,
    [Description("Stage")]           Stage,
    [Description("Enemy")]           Enemy,
    [Description("AiControlVolume")] AiControlVolume,
    [Description("AiVisible")]       AiVisible,
    [Description("Damagable")]       Damagable,
    }

public struct Layers {
    int value;
    public Layers(params Layer[] layers) {
        value = 0;
        foreach (var layer in layers) {
            value |= 1 << (int)layer;
        }
    }
    public static implicit operator int(Layers layers) {
        return layers.value;
    }
}

public static class LayerExtensions
{
    // static string[] nameCache = new string[]
    public static string GetName(this Layer x) {
      var gm = x.GetType().GetMember(x.ToString());
      var attributes = gm[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
      return ((DescriptionAttribute)attributes[0]).Description;
    }
}
