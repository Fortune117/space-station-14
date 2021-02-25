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
using Content.Server.GameObjects.Components.Items.Storage;
using Content.Server.GameObjects.Components.MachineLinking;
using Content.Server.GameObjects.Components.MachineLinking.Signals;
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
using Content.Shared.GameObjects.EntitySystems.ActionBlocker;
using Content.Shared.GameObjects.Verbs;
using Content.Shared.Interfaces;
using Content.Shared.Interfaces.GameObjects.Components;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
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
    [ComponentReference(typeof(ISignalReceiver<ToggleSignal>))]
    public class ServerTimerComponent : SharedTimerComponent, IUse, IDropped, IThrown, ISignalReceiver<ToggleSignal>
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
        private BoundUserInterface? UserInterface => Owner.GetUIOrNull(TimerUiKey.Key);
        public bool TimerActive { get; set; } = false;

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private CancellationToken _token;

        [ComponentDependency]
        private SignalTransmitterComponent? _transmitter;

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
            return new TimerComponentState(TimerDelay);
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

            ToggleTimer();

            Owner.PopupMessage(actor.Owner, "You start the timer.");

            return true;
        }

        [Verb]
        private sealed class ConfigureVerb : Verb<ServerTimerComponent>
        {
            protected override void GetData(IEntity user, ServerTimerComponent component, VerbData data)
            {
                if (!ActionBlockerSystem.CanInteract(user))
                {
                    data.Visibility = VerbVisibility.Invisible;
                    return;
                }

                data.Text = Loc.GetString("Configure");
            }

            protected override void Activate(IEntity user, ServerTimerComponent component)
            {
                IPlayerSession? playerSession = user.PlayerSession();

                if (playerSession != null)
                    component.OpenUserInterface(playerSession);
            }
        }

        [Verb]
        private sealed class StartTimerVerb : Verb<ServerTimerComponent>
        {
            protected override void GetData(IEntity user, ServerTimerComponent component, VerbData data)
            {
                if (!ActionBlockerSystem.CanInteract(user))
                {
                    data.Visibility = VerbVisibility.Invisible;
                    return;
                }

                data.Text = Loc.GetString("Start Timer");
            }

            protected override void Activate(IEntity user, ServerTimerComponent component)
            {
                IPlayerSession? playerSession = user.PlayerSession();

                if (playerSession != null)
                {
                   component.ToggleTimer();
                   playerSession.AttachedEntity.PopupMessage(playerSession.AttachedEntity, "You start the timer.");
                }
            }
        }

        public void OpenUserInterface(IPlayerSession playerSession)
        {
            UserInterface?.Toggle(playerSession);
        }

        private void OnUserInterfaceClosed(IPlayerSession playerSession)
        {
        }

        private void UpdateDelay(TimerUpdateDelayMessage message)
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
                case TimerUpdateDelayMessage updateDelayMessage:
                    UpdateDelay(updateDelayMessage);
                    break;
            }
        }

        public void StartTimer()
        {
            TimerActive = true;
            _token = _tokenSource.Token;
            Timer.Spawn(TimeSpan.FromSeconds(TimerDelay), TimerFinished, _token);
        }

        public void CancelTimer()
        {
            TimerActive = false;
           _tokenSource.Cancel();
        }

        public void TimerFinished()
        {
            TimerActive = false;
            Owner.PopupMessageEveryone("*BEEP BEEP BEEP*", range: 5);
            _transmitter?.TransmitSignal(new ToggleSignal());
        }

        public void ToggleTimer()
        {
            if (TimerActive)
            {
                CancelTimer();
            }
            else
            {
                StartTimer();
            }
        }

        public void TriggerSignal(ToggleSignal signal)
        {
            ToggleTimer();
        }
    }
}
