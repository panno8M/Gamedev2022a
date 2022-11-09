using System;
using UnityEngine;
using UniRx;
using Assembly.Components.Input.Internal;

namespace Assembly.Components.Input
{
  public class InputControl : UniqueBehaviour<InputControl>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { instance = null; }

    InputControlProvider _provider;
    InputControlProvider provider => _provider ?? (_provider = new InputControlProvider());

    public ReadOnlyReactiveProperty<float> HorizontalMoveInput => provider.HorizontalMoveInput;

    public ReadOnlyReactiveProperty<bool> GoUpInput => provider.GoUpInput;
    public IObservable<Unit> GoUp => provider.GoUp;

    public ReadOnlyReactiveProperty<bool> BreathInput => provider.BreathInput;
    public IObservable<Unit> BreathPress => provider.BreathPress;
    public IObservable<Unit> BreathRelease => provider.BreathRelease;

    public ReadOnlyReactiveProperty<bool> RespawnInput => provider.RespawnInput;
    public IObservable<Unit> Respawn => provider.Respawn;

    public ReadOnlyReactiveProperty<Vector2> MousePosInput => provider.MousePosInput;
    public ReadOnlyReactiveProperty<Vector3> MousePosStage => provider.MousePosStage;

    public ReadOnlyReactiveProperty<bool> InteractInput => provider.InteractInput;
    public IObservable<Unit> Interact => provider.Interact;


#if DEBUG
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
      public Vector2 mousePos;
      public bool interact;
    }
    [SerializeField]
    Inspector inspector;

    void Inspect()
    {
      HorizontalMoveInput.Subscribe(x => inspector.horizontalMove = x).AddTo(this);
      GoUpInput.Subscribe(x => inspector.goUp = x).AddTo(this);
      BreathInput.Subscribe(x => inspector.breath = x).AddTo(this);
      MousePosInput.Subscribe(x => inspector.mousePos = x).AddTo(this);
      InteractInput.Subscribe(x => inspector.interact = x).AddTo(this);
    }
#endif
  }

}
