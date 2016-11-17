using System;
using Windows.Devices.Gpio;

namespace Onlab_Final3.Devices
{
    /*
     * Relay class replesents the relay connected to the raspberry
     */
    class Relay : IDisposable
    {
        private readonly int RELAY_PIN;
        private GpioPin pin;

        public GpioPinValue RelayState { get; private set; } = GpioPinValue.Low;

        public Relay(int pin)
        {
            RELAY_PIN = pin;
        }

        public void Initialize()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                pin = null;
                return;
            }

            pin = gpio.OpenPin(RELAY_PIN);

            pin.Write(GpioPinValue.Low);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        // turn relay on/off depends on the value
        public void Write(GpioPinValue value)
        {
            pin.Write(value);
            RelayState = value;
        }

        // changes the relay state on -> off / off -> on
        public void changeRelayState()
        {
            if (RelayState == GpioPinValue.High)
            {
                RelayState = GpioPinValue.Low;
                pin.Write(RelayState);
            }
            else
            {
                RelayState = GpioPinValue.High;
                pin.Write(RelayState);
            }
        }

        public void Dispose()
        {
            pin.Dispose();
        }
    }
}
