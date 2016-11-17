using Microsoft.IoT.Lightning.Providers;
using System;
using System.Threading.Tasks;
using Windows.Devices.Pwm;

namespace Onlab_Final3.Devices
{

    namespace Onlab_Projekt2
    {
        /*
         * Class that represents the servo connected to the pi.
         * It must use lightning provider else it wont work. Furthermore proper power supply is required
         * because it wont work properly
         */
        class Servo : IDisposable
        {
            public readonly int PIN_NUMBER;
            public readonly int FREQUENCY;
            public readonly double MIN_PERIOD;
            public readonly double MAX_PERIOD;
            public readonly int MAX_ANGLE;
            private int signalDuration = 100; 


            PwmPin pin;
            PwmController controller;


            public bool IsInitialized { get; private set; }

            public Servo(int pin_number, int frequency = 50, double min_period = 0.7, double max_period = 2.6, int max_angle = 180)
            {
                this.PIN_NUMBER = pin_number;
                this.FREQUENCY = frequency;
                this.MIN_PERIOD = min_period;
                this.MAX_PERIOD = max_period;
                this.MAX_ANGLE = max_angle;
            }

            public async Task InitializeAsync()
            {
                if (!LightningProvider.IsLightningEnabled)
                {
                    throw new Exception("Servo can only be used with Lihtning provider");
                }

                controller = (await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider()))[1];
                pin = controller.OpenPin(PIN_NUMBER);
                controller.SetDesiredFrequency(FREQUENCY);

                // move to 'plant C' on start
                MoveToPeriod(MIN_PERIOD);
            }

            // not tested yet
            public void MoveToPercentage(int percentage)
            {
                if (percentage < 0 || percentage >= MAX_ANGLE)
                    return;
                var pulseWith = MIN_PERIOD + percentage * (MAX_PERIOD - MAX_PERIOD / 180);
                MoveToPeriod(pulseWith);
            }

            public void MoveToPeriod(double pulsewidth)
            {
                var percentage = pulsewidth / (1000.0 / FREQUENCY);
                pin.SetActiveDutyCyclePercentage(percentage);

                pin.Start();
                Task.Delay(signalDuration+100).Wait();
                pin.Stop();

                // there were some power supply issue so I move them twice 
                pin.Start();
                Task.Delay(signalDuration + 150).Wait();
                pin.Stop();
            }

            public void Dispose()
            {
                pin.Dispose();
            }
        }
    }
}

