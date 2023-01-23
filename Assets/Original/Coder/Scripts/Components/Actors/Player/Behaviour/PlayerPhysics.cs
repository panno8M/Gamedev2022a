using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Utilities;

namespace Assembly.Components.Actors.Player
{
  public class PlayerPhysics : ActorBehavior<PlayerAct>
  {
    public static float G = -9.8f;
    [System.Serializable]
    public class GravityScale
    {
      public float normal = 1.3f;
      public float flying = 0.5f;
    }

    #region params
    GravityScale gravityScale = new GravityScale();
    [Range(0, 90)]
    [SerializeField]
    int _degreeClimbableObstacle = 30;
    [Range(0, 90)]
    int _degreeUnclimbableObstacle = 60;
    int degreeClimbableObstacle
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
    int degreeUnclimbableObstacle => _degreeUnclimbableObstacle;

    Vector3 steppableBoxCenter = new Vector3(0.4f, 0.6f, 0f);
    Vector3 steppableBoxExtents = new Vector3(0.1f, 0.4f, 0.2f);
    #endregion //params

    Subject<Unit> _BehaviorUpdate = new Subject<Unit>();
    public IObservable<Unit> BehaviorUpdate => _BehaviorUpdate;

    [SerializeField] Subject<Unit> _OnLand = new Subject<Unit>();
    bool _isOnGround;
    bool _isOnGroundAtLast;

    public IObservable<Unit> OnLand => _OnLand;
    public bool isOnGround => _isOnGround;

    public bool obstacleColliding;
    public bool obstacleClimbable;
    public Vector3 obstacleTangent;
    public Vector3 groundedForward;

    LayerMask __collisionLayer = new Layers(Layer.PhsStage, Layer.PhsDynamics);

    protected override void OnAssemble()
    {
      obstacleColliding = false;
      obstacleClimbable = false;
      obstacleTangent = Vector3.zero;
      groundedForward = Vector3.zero;
    }
    protected override void Blueprint()
    {
      _actor.OnCollisionStayAsObservable()
          .Subscribe(collision =>
          {
            foreach (var contact in collision.contacts)
            {
              if (Vector3.Angle(contact.normal, Vector3.up) < degreeClimbableObstacle)
              {
                _isOnGround = true;
                groundedForward = CrossZ(contact.normal, _actor.ctl.lookDirection);
              }
              else if (Vector3.Angle(contact.normal, -_actor.ctl.lookDirection.v__()) < degreeUnclimbableObstacle)
              {
                obstacleColliding = true;
                obstacleTangent = CrossZ(contact.normal, _actor.ctl.lookDirection);
              }
            }

            Vector3 CrossZ(Vector3 vec, float v) => new Vector3( vec.y * v, -vec.x * v, 0);
          });

      _actor.FixedUpdateAsObservable()
        .Subscribe(_ =>
        {
          rigidbody.AddForce((G * gravityScale.normal)._v_(), ForceMode.Acceleration);

          if (_actor.ctl.isIgnoreAll) { return; }

          if (!_isOnGroundAtLast && _isOnGround) { _OnLand.OnNext(Unit.Default); }

          _BehaviorUpdate.OnNext(Unit.Default);

          obstacleClimbable = obstacleColliding && !Physics.CheckBox(
            transform.position + steppableBoxCenter.SignX(_actor.ctl.lookDirection),
            steppableBoxExtents,
            Quaternion.identity, __collisionLayer);

          _isOnGroundAtLast = _isOnGround;
          _isOnGround = false;
          obstacleColliding = false;
        });
    }

    void OnDrawGizmos()
    {
      Gizmos.color = Color.cyan;
      Gizmos.DrawWireCube(
        transform.position + steppableBoxCenter.SignX(_actor.ctl.lookDirection),
        steppableBoxExtents * 2);
    }
  }
}