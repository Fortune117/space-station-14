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
    public abstract class SharedIoDeviceTimerComponent : SharedIoDeviceComponent
    {
        public override string Name => "IoTimer";

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
    public class IoDeviceTimerComponentState : ComponentState
    {
        public readonly float TimerDelay;

        public IoDeviceTimerComponentState(float timerDelay) : base(ContentNetIDs.IODEVICE)
        {
            TimerDelay = timerDelay;
        }
    }

    [Serializable, NetSerializable]
    public class IoDeviceTimerUpdateDelayMessage : ComponentMessage
    {
        public readonly float NewDelay;
        public IoDeviceTimerUpdateDelayMessage(float newDelay)
        {
            Directed = true;
            NewDelay = newDelay;
        }
    }

    [NetSerializable, Serializable]
    public enum IoDeviceTimerUiKey
    {
        Key,
    }
}
