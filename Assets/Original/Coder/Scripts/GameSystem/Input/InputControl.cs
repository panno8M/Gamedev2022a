using System;
using UnityEngine;
using UniRx;
using Assembly.GameSystem.Internal;

namespace Assembly.GameSystem.Input
{
  public class InputControl : DiBehavior
  {
    InputControlProvider _provider;
    InputControlProvider provider => _provider ?? (_provider = new InputControlProvider());

    public IObservable<float> HorizontalMoveInput => provider.HorizontalMoveInput;
    public IObservable<bool> GoUpInput => provider.GoUpInput;
    public IObservable<bool> BreathInput => provider.BreathInput;
    public IObservable<bool> PauseInput => provider.PauseInput;
    public IObservable<bool> InteractInput => provider.InteractInput;
    public IObservable<Vector2> MousePosInput => provider.MousePosInput;
    public IObservable<Vector3> MousePosStage => provider.MousePosStage;

    public float horizontalMoveInput => provider.HorizontalMoveInput.Value;
    public bool goUpInput => provider.GoUpInput.Value;
    public bool breathInput => provider.BreathInput.Value;
    public bool pauseInput => provider.PauseInput.Value;
    public bool interactInput => provider.InteractInput.Value;
    public Vector2 mousePosInput => provider.MousePosInput.Value;
    public Vector3 mousePosStage => provider.MousePosStage.Value;


    public IObservable<Unit> GoUpFixed => provider.GoUpFixed.BatchFrame(0, FrameCountType.FixedUpdate);
    public IObservable<Unit> BreathPressFixed => provider.BreathPressFixed.BatchFrame(0, FrameCountType.FixedUpdate);
    public IObservable<Unit> BreathReleaseFixed => provider.BreathReleaseFixed.BatchFrame(0, FrameCountType.FixedUpdate);
    public IObservable<Unit> PauseFixed => provider.PauseFixed.BatchFrame(0, FrameCountType.FixedUpdate);
    public IObservable<Unit> InteractFixed => provider.InteractFixed.BatchFrame(0, FrameCountType.FixedUpdate);

    protected override void Blueprint()
    {
      throw new NotImplementedException();
    }

#if UNITY_EDITOR
    void Awake()
    {
      Inspect();
    }

    [Serializable]
    struct Inspector
    {
      public float horizontalMove;
      public bool goUp;
      public bool breath;
      public bool pause;
      public bool interact;
      public Vector2 mousePos;
      public Vector2 mousePosStage;
    }
    [SerializeField]
    Inspector inspector;

    void Inspect()
    {
      HorizontalMoveInput.Subscribe(x => inspector.horizontalMove = x).AddTo(this);
      GoUpInput.Subscribe(x => inspector.goUp = x).AddTo(this);
      BreathInput.Subscribe(x => inspector.breath = x).AddTo(this);
      InteractInput.Subscribe(x => inspector.interact = x).AddTo(this);
      MousePosInput.Subscribe(x => inspector.mousePos = x).AddTo(this);
      MousePosStage.Subscribe(x => inspector.mousePosStage = x).AddTo(this);
      PauseInput.Subscribe(x => inspector.pause = x).AddTo(this);

    }
#endif
  }

}
