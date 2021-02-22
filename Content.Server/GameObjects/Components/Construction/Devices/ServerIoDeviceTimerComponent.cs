#nullable enable
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.GameObjects.Components.Access;
using Content.Server.GameObjects.Components.Atmos;
using Content.Server.GameObjects.Components.GUI;
using Content.Server.GameObjects.Components.Interactable;
using Content.Server.GameObjects.Components.Mobs;
using Content.Server.GameObjects.EntitySystems;
using Content.Server.Interfaces.GameObjects.Components.Doors;
using Content.Shared.Damage;
using Content.Shared.GameObjects.Components.Body;
using Content.Shared.GameObjects.Components.Construction.Devices;
using Content.Shared.GameObjects.Components.Damage;
using Content.Shared.GameObjects.Components.Doors;
using Content.Shared.GameObjects.Components.Interactable;
using Content.Shared.GameObjects.Components.Movement;
using Content.Shared.Interfaces.GameObjects.Components;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Players;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Serilog;
using Timer = Robust.Shared.Timing.Timer;


namespace Content.Server.GameObjects.Components.Construction.Devices
{

    [RegisterComponent]
    public class ServerIoDeviceTimerComponent : SharedIoDeviceTimerComponent , IUse
    {
        private TimeSpan _timerDelay;
        // Make it a property so we can call Dirty()
        public TimeSpan TimerDelay
        {
            get => _timerDelay;
            set
            {
                _timerDelay = value;
                // Dirty() signals to the networking system that this component's state has changed
                // and needs to be re-sent to clients.
                Dirty();
            }
        }

        // Create the component state for the networking system.
        public override ComponentState GetComponentState(ICommonSession player)
        {
            return new IoDeviceTimerComponentState(TimerDelay);
        }

        bool IUse.UseEntity(UseEntityEventArgs eventArgs)
        {
            Logger.Info("Weeeadad");

            SendNetworkMessage(new IoDeviceTimerRequestGuiMessage(), (INetChannel?)eventArgs.User.PlayerSession());
            return true;
        }
    }
}
