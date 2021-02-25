using System.Linq;
using System.Threading.Tasks;
using Content.Server.GameObjects.Components.Construction.Devices;
using Content.Server.GameObjects.Components.MachineLinking;
using Content.Server.GameObjects.Components.MachineLinking.Signals;
using Content.Shared.Construction;
using Content.Shared.Interfaces.GameObjects.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.GameObjects.Components.Weapon.Explosive
{
    [RegisterComponent]
    public class CraftableGrenadeComponent : Component, IUse
    {
        public override string Name => "CraftableGrenade";

        private IContainer _triggerContainer;
        private IContainer _payloadContainer;

        public override void Initialize()
        {
            base.Initialize();

            _triggerContainer = ContainerManagerComponent.Ensure<Container>("trigger", Owner);
            _payloadContainer = ContainerManagerComponent.Ensure<Container>("payload", Owner);
        }

        private bool TryGetBombReceiver(out SignalReceiverComponent receiver)
        {
            foreach (IEntity bombCore in _payloadContainer.ContainedEntities)
            {
                if (bombCore.TryGetComponent(out SignalReceiverComponent bombReceiver))
                {
                    receiver = bombReceiver;
                    return true;
                }
            }

            receiver = null;
            return false;
        }

        private bool TryGetTriggerTransmitter(out SignalTransmitterComponent transmitter)
        {
            foreach (IEntity trigger in _triggerContainer.ContainedEntities)
            {
                if (trigger.TryGetComponent(out SignalTransmitterComponent triggerTransmittter))
                {
                    transmitter = triggerTransmittter;
                    return true;
                }
            }

            transmitter = null;
            return false;
        }

        //Basically, we see if any entities in the trigger container can take ToggleSignals. If they can, we send one.
        //I don't want to hardcode the types of entities that can be used as triggers so that it can be expanded upon.

        //This whole thing is messy as I'm waiting on Ram (Vera#3100) to create step actions for the construction graph.
        private void DeviceActivated()
        {
            if (TryGetBombReceiver(out SignalReceiverComponent bombReceiver) && TryGetTriggerTransmitter(out SignalTransmitterComponent triggerTransmitter))
            {
                bombReceiver.Subscribe(triggerTransmitter);
            }

            foreach (IEntity entity in _triggerContainer.ContainedEntities)
            {
                if (entity.TryGetComponent(out ISignalReceiver<ToggleSignal> receiver))
                {
                    receiver.TriggerSignal(new ToggleSignal());
                    return;
                }
            }
        }

        public bool UseEntity(UseEntityEventArgs eventArgs)
        {
            DeviceActivated();
            return true;
        }
    }
}
