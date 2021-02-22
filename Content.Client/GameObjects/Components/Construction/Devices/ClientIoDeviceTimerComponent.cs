using System;
using Content.Shared.GameObjects.Components.Construction.Devices;
using Content.Shared.Interfaces.GameObjects.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Players;

namespace Content.Client.GameObjects.Components.Construction.Devices
{
    [RegisterComponent]
    public class ClientIoDeviceTimerComponent : SharedIoDeviceTimerComponent
    {

        public TimeSpan TimerDelay { get; set; }

        public override void HandleComponentState(ComponentState curState, ComponentState nextState)
        {
            if (curState is IoDeviceTimerComponentState componentState)
            {
                TimerDelay = componentState.TimerDelay;
            }
        }


        public override void HandleNetworkMessage(ComponentMessage message, INetChannel netChannel, ICommonSession? session = null)
        {
            base.HandleNetworkMessage(message, netChannel, session);
        }
    }
}
