namespace Content.Server.GameObjects.Components.Construction.Devices
{
    public interface IIoDevice
    {
        public bool IsOn { get; set; }

        public void InputReceived(IoSignal signal);
    }

    //Cause this will get confusing to anyone that looks at it later - I'm creating IoInput now rather than later
    //as I plan for this 'IoDevice' system to be expanded upon greatly.
    //
    //IoInput is something that is sent to an IIoDevice, which then chooses how to handle that input. What it does
    //with that input doesn't matter - what matters is that there's a way to identify and handle the many different types of inputs
    //that a device might receive. They should be able to handle signals telling them to turn on, as well as signals carrying data
    //such as floats, bools or integers for components on that device.
    //
    //Currently, I only need the signal to let the device know to turn on and off, as well as the signal to 'activate' the device.
    //Please note that there is no strict implementation required for each signal, it just gives an easy, consistent and legible way
    //of reading them in code later.
    public class IoSignal
    {
        public enum Type
        {
            On,
            Activate
        }

        public IoSignal.Type SignalType { get; set; }

        public IoSignal(IoSignal.Type signalType)
        {
            SignalType = signalType;
        }
    }
}
