using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem.Message;
using Utilities;

namespace Assembly.Components.StageGimmicks
{
  [RequireComponent(typeof(SafetyTrigger))]
  public class Button : InteractableSignal
  {
    [SerializeField] Renderer rendererA;
    [SerializeField] Renderer rendererB;
    [SerializeField][ColorUsage(false, true)] Color colorA;
    [SerializeField][ColorUsage(false, true)] Color colorB;
    Material materialA;
    Material materialB;
    Color defaultEmissionA;
    Color defaultEmissionB;

    protected override void Blueprint()
    {
      base.Blueprint();
      materialA = rendererA.material;
      materialA.EnableKeyword("_EMISSION");
      defaultEmissionA = materialA.GetColor(idEmissionColor);

      materialB = rendererB.material;
      materialB.EnableKeyword("_EMISSION");
      defaultEmissionB = materialB.GetColor(idEmissionColor);
    }

    void OnDestroy()
    {
      if (materialA) { Destroy(materialA); }
      if (materialB) { Destroy(materialB); }
    }

    protected override void OnProgressUpdate(MixFactor factor)
    {

      materialA.SetColor(idEmissionColor, factor.Mix(defaultEmissionA, colorA));
      materialB.SetColor(idEmissionColor, factor.Mix(colorB, defaultEmissionB));
    }

    static int idEmissionColor = Shader.PropertyToID("_EmissionColor");
  }
}