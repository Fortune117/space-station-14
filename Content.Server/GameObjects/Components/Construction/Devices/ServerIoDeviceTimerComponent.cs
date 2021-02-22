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
using Content.Server.Utility;
using Content.Shared.Damage;
using Content.Shared.GameObjects.Components.Body;
using Content.Shared.GameObjects.Components.Construction.Devices;
using Content.Shared.GameObjects.Components.Damage;
using Content.Shared.GameObjects.Components.Doors;
using Content.Shared.GameObjects.Components.Instruments;
using Content.Shared.GameObjects.Components.Interactable;
using Content.Shared.GameObjects.Components.Movement;
using Content.Shared.Interfaces.GameObjects.Components;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Players;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using Timer = Robust.Shared.Timing.Timer;


namespace Content.Server.GameObjects.Components.Construction.Devices
{

    [RegisterComponent]
    public class ServerIoDeviceTimerComponent : SharedIoDeviceTimerComponent, IUse, IDropped, IThrown
    {
        private float _timerDelay;
        // Make it a property so we can call Dirty()
        public float TimerDelay
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

        [ViewVariables]
        private BoundUserInterface? UserInterface => Owner.GetUIOrNull(IoDeviceTimerUiKey.Key);

        public override void Initialize()
        {
            base.Initialize();

            if (UserInterface != null)
            {
                UserInterface.OnClosed += OnUserInterfaceClosed;
            }

            TimerDelay = DefaultTimerDelaySeconds;
        }

        // Create the component state for the networking system.
        public override ComponentState GetComponentState(ICommonSession player)
        {
            return new IoDeviceTimerComponentState(TimerDelay);
        }

        void IDropped.Dropped(DroppedEventArgs eventArgs)
        {
            UserInterface?.CloseAll();
        }

        void IThrown.Thrown(ThrownEventArgs eventArgs)
        {
            UserInterface?.CloseAll();
        }

        bool IUse.UseEntity(UseEntityEventArgs eventArgs)
        {
            if (!eventArgs.User.TryGetComponent(out IActorComponent? actor))
                return false;

            OpenUserInterface(actor.playerSession);

            return true;
        }

        public void OpenUserInterface(IPlayerSession playerSession)
        {
            UserInterface?.Toggle(playerSession);
        }

        private void OnUserInterfaceClosed(IPlayerSession playerSession)
        {
        }

        private void UpdateDelay(IoDeviceTimerUpdateDelayMessage message)
        {
            TimerDelay = Math.Clamp(message.NewDelay, MinTimerDelay, MaxTimerDelay);

            //TODO: Implement proper localisation
            Owner.PopupMessageEveryone("The timer beeps in acknowledgement.", range: 5);
        }

        public override void HandleNetworkMessage(ComponentMessage message, INetChannel netChannel, ICommonSession? session = null)
        {
            base.HandleNetworkMessage(message, netChannel, session);

            switch (message)
            {
                case IoDeviceTimerUpdateDelayMessage updateDelayMessage:
                    UpdateDelay(updateDelayMessage);
                    break;
            }
        }
    }
}
