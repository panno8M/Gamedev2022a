using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Utilities;

namespace Assembly.Components
{
  public class DroneReactionSet : DiBehavior
  {
    public enum Kind { None, Question, Exclamation, GuruGuru }
    EzLerp lifetime = new EzLerp(1);
    Kind _kind;
    [SerializeField] GameObject uiQuestion;
    [SerializeField] GameObject uiExclamation;
    [SerializeField] GameObject uiGuruGuru;

    void Switch(Kind kind)
    {
      uiQuestion.SetActive(kind == Kind.Question);
      uiExclamation.SetActive(kind == Kind.Exclamation);
      uiGuruGuru.SetActive(kind == Kind.GuruGuru);
    }

    protected override void Blueprint()
    {
      this.FixedUpdateAsObservable()
        .Where(lifetime.isNeedsCalc)
        .Select(lifetime.UpdFactor)
        .Subscribe(fac =>
        {
          if (fac == 1) { kind = Kind.None; }
        });
      this.FixedUpdateAsObservable()
        .Where(_ => kind != Kind.None)
        .Subscribe(_ =>
        {
          transform.rotation = Camera.main.transform.rotation;
        });
      kind = Kind.None;
    }

    public Kind kind
    {
      get => _kind;
      set
      {
        _kind = value;
        Switch(_kind);
        lifetime.SetFactor0();
        lifetime.SetMode(increase: _kind != Kind.None);
      }
    }
  }
}