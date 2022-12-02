using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Assembly.Components.Actors.Player
{
  [RequireComponent(typeof(Rigidbody))]
  public class PlayerBehaviour : MonoBehaviour
  {
    public static float G = -9.8f;

    [System.Serializable]
    public struct GravityScale
    {
      public GravityScale(float normal, float flying)
      {
        this.normal = normal;
        this.flying = flying;
      }
      public float normal;
      public float flying;
    }

    #region editable params
    [SerializeField] GravityScale _scaleGravity;

    void Reset()
    {
      _scaleGravity = new GravityScale(1.3f, .1f);
    }
    #endregion

    #region assets
    Rigidbody rb;
    PlayerAct _player;
    #endregion

    void Awake()
    {
      rb = GetComponent<Rigidbody>();
      _player = Global.Player;

      sbsc_AddGravity();

      _player.OnJump.Subscribe(_ => Jump()).AddTo(this);
      _player.OnFlapWhileFalling.Subscribe(_ => Jump()).AddTo(this);

      _player.WhileWalking
          .Subscribe(MoveHorizontal);
      _player.WhileSkywalking
          .Subscribe(MoveHorizontal);

      Global.Control.HorizontalMoveInput
          .Where(hmi => hmi == 0)
          .Subscribe(hmi => StopHorizontal())
          .AddTo(this);

      _player.interactor.holder.RequestHold
          .Subscribe(_player.interactor.holder.Grab);
    }

    void sbsc_AddGravity()
    {
      this.FixedUpdateAsObservable()
          .Subscribe(_ =>
          {
            rb.AddForce((G * _scaleGravity.normal)._y_(), ForceMode.Acceleration);
          });
    }

    void Jump()
    {
      rb.velocity = rb.velocity.x_z();
      rb.AddForce(_player.param.jumpHeight._y_(), ForceMode.Impulse);
    }


    void MoveHorizontal(PlayerAct.Direction dir)
    {
      rb.velocity = new Vector3(
          (float)dir * _player.param.MoveSpeed(),
          rb.velocity.y,
          rb.velocity.z);
    }
    void StopHorizontal()
    {
      rb.velocity = rb.velocity._yz();
    }
  }
}