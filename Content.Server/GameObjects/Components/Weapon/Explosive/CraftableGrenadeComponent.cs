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

        private void DeviceActivated()
        {
            IContainer container = ContainerManagerComponent.Ensure<Container>("trigger", Owner);

            foreach (IEntity entity in container.ContainedEntities)
            {

            }
        }
        public bool UseEntity(UseEntityEventArgs eventArgs)
        {
            DeviceActivated();
            return true;
        }
    }
}
