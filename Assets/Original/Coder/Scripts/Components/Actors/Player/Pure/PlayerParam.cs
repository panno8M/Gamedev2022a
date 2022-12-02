using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembly.Components.Actors
{
  [System.Serializable]
  public class PlayerParam
  {
    public enum Mobility { Normal, knackered }
    public PlayerParam(
        float jumpHeight = 5,
        float soarHeight = 3,
        float moveSpeedNormal = 3.5f,
        float moveSpeedKnackered = 1.8f)
    {
      this.jumpHeight = jumpHeight;
      this.soarHeight = soarHeight;
      this.moveSpeedNormal = moveSpeedNormal;
      this.moveSpeedKnackered = moveSpeedKnackered;
      this.mobility = Mobility.Normal;
    }
    public float jumpHeight;
    public float soarHeight;
    public float moveSpeedNormal;
    public float moveSpeedKnackered;
    public Mobility mobility;

    [Range(0f, 1f)] float moveSpeedBlend;
    [SerializeField] float secTransitionSpeed = 1;

    float latestCallTime;

    void CalcBlend()
    {
      var delta = Time.time - latestCallTime;
      if (delta < 0.001) { return; }
      latestCallTime = Time.time;
      moveSpeedBlend = Mathf.Clamp01(moveSpeedBlend + (mobility == Mobility.Normal ? -1 : 1) * delta / secTransitionSpeed);
    }

    public float MoveSpeed()
    {
      CalcBlend();
      return moveSpeedNormal * (1 - moveSpeedBlend) + moveSpeedKnackered * moveSpeedBlend;
    }
    public void SetAsNormal()
    {
      mobility = Mobility.Normal;
    }
    public void SetAsKnackered()
    {
      mobility = Mobility.knackered;
    }
  }
}