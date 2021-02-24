#nullable enable
using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Robust.Shared.Physics;
using System.Collections.Generic;
using Robust.Shared.Timing;

namespace Content.Shared.GameObjects.Components.Construction.Devices
{
    public abstract class SharedTimerComponent : Component
    {
        public override string Name => "DeviceTimer";
        public override uint? NetID => ContentNetIDs.TIMER;

        public float DefaultTimerDelaySeconds = 3f;
        public float MaxTimerDelay = 300f;
        public float MinTimerDelay = 0f;

        public override void ExposeData(ObjectSerializer serializer)
        {
            base.ExposeData(serializer);

            serializer.DataField(ref DefaultTimerDelaySeconds, "defaultime", 3f);
            serializer.DataField(ref MaxTimerDelay, "maxtime", 300f);
            serializer.DataField(ref MinTimerDelay, "mintime", 0f);
        }
    }

    [Serializable, NetSerializable]
    public class TimerComponentState : ComponentState
    {
        public readonly float TimerDelay;

        public TimerComponentState(float timerDelay) : base(ContentNetIDs.TIMER)
        {
            TimerDelay = timerDelay;
        }
    }

    [Serializable, NetSerializable]
    public class TimerUpdateDelayMessage : ComponentMessage
    {
        public readonly float NewDelay;
        public TimerUpdateDelayMessage(float newDelay)
        {
            Directed = true;
            NewDelay = newDelay;
        }
    }

    [NetSerializable, Serializable]
    public enum TimerUiKey
    {
        Key,
    }
}
