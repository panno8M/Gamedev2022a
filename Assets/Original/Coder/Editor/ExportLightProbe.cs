using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class ExportLightprobe
{
  [MenuItem("Export/Export LightProbe")]
  static void Export()
  {
    Scene scene = SceneManager.GetActiveScene();
    string path = Path.ChangeExtension(scene.path, null);
    Directory.CreateDirectory(path);
    AssetDatabase.CreateAsset(
      GameObject.Instantiate(LightmapSettings.lightProbes),
      path + "/Lightprobe.asset");
  }
}