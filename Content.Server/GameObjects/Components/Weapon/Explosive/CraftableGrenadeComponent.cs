using Content.Server.GameObjects.Components.Construction.Devices;
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

        public override void Initialize()
        {
            base.Initialize();

            _triggerContainer = ContainerManagerComponent.Ensure<Container>("trigger", Owner);
        }

        private void DeviceActivated()
        {
        }
        
        public bool UseEntity(UseEntityEventArgs eventArgs)
        {
            DeviceActivated();
            return true;
        }
    }
}
