using System;
using UnityEngine;

namespace Assembly.Components
{
  public class LightShift : MonoBehaviour
  {
    [Serializable]
    public struct LightUnit
    {
      [Serializable]
      public struct LightMap
      {
        public Texture2D light;
        public Texture2D dir;
        public LightmapData MakeData()
        {
          LightmapData data = new LightmapData();
          data.lightmapColor = light;
          data.lightmapDir = dir;
          return data;
        }
      }
      public string name;
      public LightMap[] lightMaps;
      public LightProbes lightProbes;
      public Cubemap[] reflectionProbeCubemaps;
      public LightmapData[] data;

      public void MakeData()
      {
        data = new LightmapData[lightMaps.Length];
        for (int i = 0; i < lightMaps.Length; i++)
        { data[i] = lightMaps[i].MakeData(); }
      }
    }
    [SerializeField] LightUnit[] lightUnits;
    [SerializeField] ReflectionProbe[] reflectionProbes;

    void Start()
    {
      for (int i = 0; i < lightUnits.Length; i++)
      { lightUnits[i].MakeData(); }
      Set(0);
    }

    void Set(ref LightUnit lightUnit, in ReflectionProbe[] reflectionProbes)
    {
      LightmapSettings.lightProbes = lightUnit.lightProbes;
      LightmapSettings.lightmaps = lightUnit.data;
      int l = reflectionProbes.Length;
      for (int i = 0; i < l; i++)
      {
        reflectionProbes[i].customBakedTexture = lightUnit.reflectionProbeCubemaps[i];
      }
    }
    public void Set(int index)
    {
      if (lightUnits.Length <= index) { return; }
      Set(ref lightUnits[index], reflectionProbes);
    }
  }
}