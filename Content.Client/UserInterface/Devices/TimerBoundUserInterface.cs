using Content.Client.GameObjects.Components.Construction.Devices;
using Content.Shared.GameObjects.Components.Construction.Devices;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.UserInterface.Devices
{
    public class TimerBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private TimerMenu _timerMenu;
        public ClientTimerComponent TimerComponent { get; set; }

        public TimerBoundUserInterface([NotNull] ClientUserInterfaceComponent owner, [NotNull] object uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            if (!Owner.Owner.TryGetComponent<ClientTimerComponent>(out var timerComponent)) return;

            TimerComponent = timerComponent;
            _timerMenu = new TimerMenu(this);
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
