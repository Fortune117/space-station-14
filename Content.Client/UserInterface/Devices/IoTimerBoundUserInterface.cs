using Content.Client.GameObjects.Components.Construction.Devices;
using Content.Shared.GameObjects.Components.Construction.Devices;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.UserInterface.Devices
{
    public class IoTimerBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private IoTimerMenu _timerMenu;
        public ClientIoDeviceTimerComponent TimerComponent { get; set; }

        public IoTimerBoundUserInterface([NotNull] ClientUserInterfaceComponent owner, [NotNull] object uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            if (!Owner.Owner.TryGetComponent<ClientIoDeviceTimerComponent>(out var timerComponent)) return;

            TimerComponent = timerComponent;
            _timerMenu = new IoTimerMenu(this);
            _timerMenu.OnClose += Close;
            _timerMenu.OpenCentered();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            _timerMenu?.Dispose();
        }
    }
}
