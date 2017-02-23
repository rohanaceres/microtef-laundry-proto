using Windows.Devices.Gpio;

namespace NekiIt.Proto.Laundry
{
    // TODO: Doc
    internal sealed class Machine
    {
        public string Name { get; private set; }
        public int PinNumber { get; private set; }
        private GpioController Controller { get; set; }
        private GpioPin Pin { get; set; }

        public Machine(string machineName, int pinNumber)
        {
            this.Name = machineName;
            this.PinNumber = pinNumber;

            this.Controller = GpioController.GetDefault();

            this.Pin = this.Controller.OpenPin(this.PinNumber);
            this.Pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void TurnOn ()
        {
            this.Pin.Write(GpioPinValue.Low);
        }
        public void TurnOff ()
        {
            this.Pin.Write(GpioPinValue.High);
        }
    }
}
