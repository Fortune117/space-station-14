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
    public class ClientTimerComponent : SharedTimerComponent
    {
        public float TimerDelay { get; set; }

        public override void HandleComponentState(ComponentState curState, ComponentState nextState)
        {
            if (curState is TimerComponentState componentState)
            {
                TimerDelay = componentState.TimerDelay;
            }
        }

        public void UpdateTimerDelay(float delay)
        {
            SendNetworkMessage(new TimerUpdateDelayMessage(delay));
        }

    }
}
