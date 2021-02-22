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
    public abstract class SharedIoDeviceComponent : Component
    {
        public override string Name => "IoDevice";
        public override uint? NetID => ContentNetIDs.IODEVICE;

    }
}
