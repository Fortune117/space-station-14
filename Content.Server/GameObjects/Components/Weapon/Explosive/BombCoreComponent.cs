﻿#nullable enable
using Content.Server.GameObjects.Components.Explosion;
using Content.Server.GameObjects.Components.MachineLinking;
using Content.Server.GameObjects.Components.MachineLinking.Signals;
using Content.Server.Utility;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Server.GameObjects.Components.Weapon.Explosive
{
    [RegisterComponent]
    public class BombCoreComponent : Component, ISignalReceiver<ToggleSignal>
    {
        public override string Name => "BombCore";

        //This should contain an entity with a bomb.
        private IContainer? _payloadContainer;

        [ComponentDependency]
        private readonly SignalReceiverComponent? _receiverComponent;

        public override void Initialize()
        {
            base.Initialize();

            _payloadContainer = ContainerManagerComponent.Ensure<Container>("payload", Owner);
        }

        //If any signal is received, we try to detonate.
        public void TriggerSignal(ToggleSignal signal)
        {
            if (_payloadContainer == null) return;

            foreach (IEntity entity in _payloadContainer.ContainedEntities)
            {
                if (entity.TryGetComponent(out ExplosiveComponent? explosiveComponent))
                {
                    explosiveComponent.Explosion();
                    return;
                }
            }

        }
    }
}
