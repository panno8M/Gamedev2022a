using System;
using UnityEngine;
using Utilities;

namespace Assembly.Components.Actors
{
  [System.Serializable]
  public class PlayerParam
  {
    public enum Mobility { Normal, knackered }
    public float jumpHeight = 5;
    public float soarHeight = 3;
    public float moveSpeedNormal = 3.5f;
    public float moveSpeedKnackered = 1.8f;

    public PlayerParam()
    {
      mobility = Mobility.Normal;
      degreeClimbableObstacle = 30;
    }

    Mobility _mobility;
    public Mobility mobility
    {
      get
      {
        return _mobility;
      }
      set
      {
        moveSpeedLerp.SetAsIncrease(value == Mobility.knackered);
        _mobility = value;
      }
    }

    [Range(0, 90)]
    [SerializeField]
    int _degreeClimbableObstacle = 30;
    [Range(0, 90)]
    int _degreeUnclimbableObstacle = 60;
    public int degreeClimbableObstacle
    {
      get
      {
        return _degreeClimbableObstacle;
      }
      set
      {
        _degreeClimbableObstacle = value;
        _degreeUnclimbableObstacle = 90 - value;
      }
    }
    public int degreeUnclimbableObstacle => _degreeUnclimbableObstacle;

    public Vector3 steppableBoxCenter = new Vector3(0.4f, 0.6f, 0f);
    public Vector3 steppableBoxExtents = new Vector3(0.1f, 0.4f, 0.2f);

    EzLerp moveSpeedLerp = new EzLerp(1);

    public float MoveSpeed => moveSpeedLerp.UpdMix(moveSpeedNormal, moveSpeedKnackered);

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