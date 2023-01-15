using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using Assembly.GameSystem;
using Assembly.GameSystem.ObjectPool;
using Assembly.GameSystem.Message;
using Assembly.Components.Pools;

namespace Assembly.Components
{
  public class SignalLineDrawer : DiBehavior
  {
    public List<MessageDispatcher> dispatchers = new List<MessageDispatcher>();
    SignalLinePool signalLinePool;
    [Zenject.Inject]
    public void DepsInject(
      SignalLinePool signalLinePool)
    {
      this.signalLinePool = signalLinePool;
    }

    SignalLineParticle.CreateInfo signalLineCI = new SignalLineParticle.CreateInfo
    {
      transformUsage = new TransformUsage
      {
        spawnSpace = eopSpawnSpace.Global,
        referenceUsage = eopReferenceUsage.Global,
      },
    };

    protected override void Blueprint()
    {
      signalLineCI.transformInfo = new TransformInfo
      {
        reference = transform,
      };

      this.FixedUpdateAsObservable()
        .Sample(TimeSpan.FromSeconds(3))
        .Subscribe(_ =>
        {
          for (int i_dsp = 0; i_dsp < dispatchers.Count; i_dsp++)
          {
            MessageDispatcher dispatcher = dispatchers[i_dsp];
            if (dispatcher == null) { continue; }

            for (int i = 0; i < dispatcher.receivers.Count; i++)
            {
              if (!dispatcher.receivers[i]) { continue; }
              signalLineCI.targetTrs = dispatcher.receivers[i].transform;
              signalLinePool.Spawn(signalLineCI);
            }
          }
        });
    }

  }
}