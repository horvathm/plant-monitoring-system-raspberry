using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Onlab_Final3.Devices.I2c_devices
{
    // Enumeration for arduino commands. You can pick and send one of these commands. 
    enum MethodSelector : byte
    {
        RELAY_ON = 0x06, RELAY_OFF = 0x01,
        MOVE_SERVO_A = 0x02, MOVE_SERVO_B = 0x03,
        MEASURE_SOIL_A = 0x04, MEASURE_SOIL_B = 0x05
    }

    /*
     *  Class that represents the slave arduino device. 
     *  We use arduino device as a gateway to reach sensor data and perform commands. 
     *  It's useful when we want to use ADC or PWM.
     */
    class Arduino : IDisposable
    {
        #region Fields
        I2cDevice arduino;
        private readonly byte SLAVE_ADDRESS;
        #endregion

        #region Properties
        public bool RelayState { get; private set; } = false;

        public bool IsInitialized { get; private set; }
        #endregion

        public Arduino(byte slaveAddress = 0x40)
        {
            this.SLAVE_ADDRESS = slaveAddress;          // slave address can be given in the constructor 
                                                        // by default it's 0x40
        }

        public void Dispose()
        {
            arduino.Dispose();                          // free up the resources
        }

        public async Task InitializeAsync()
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("The I2C ads1115 sensor is already initialized.");
            }

            // gets the default controller for the system, can be the lightning or any provider
            I2cController controller = await I2cController.GetDefaultAsync();

            // gets the I2CDevice from the controller using the connection setting
            arduino = controller.GetDevice(new I2cConnectionSettings(SLAVE_ADDRESS));

            if (arduino == null)
                throw new Exception("I2C controller not available on the system");

            // moves the servo to a startup position witch is the plant B's direction
            moveServoToPosition(false);

            IsInitialized = true;
        }


        // method that turns relay on or off (represented by a boolean value)
        public void turnRelay(bool state)
        {
            byte[] command = new byte[1];
            if (state)
            {
                command[0] = (byte)MethodSelector.RELAY_ON;
                RelayState = true;
                arduino.Write(command);
            }
            else
            {
                command[0] = (byte)MethodSelector.RELAY_OFF;
                RelayState = false;
                arduino.Write(command);
            }
        }

        // method created for the manual mode when you can position the servo and control the relay manually
        public void changeRelayState()
        {
            RelayState = !RelayState;
            turnRelay(RelayState);
        }

        // move servo to a position (represented by a boolean value) where true is 'plant A' and false is 'plant B'
        public void moveServoToPosition(bool direction)
        {
            byte[] command = new byte[1];
            if (direction)
            {
                command[0] = (byte)MethodSelector.MOVE_SERVO_A;
                arduino.Write(command);
            }
            else
            {
                command[0] = (byte)MethodSelector.MOVE_SERVO_B;
                arduino.Write(command);
            }
        }

        // async function reads the humidity values (represented by a boolean value) where true is 'plant A' and false is 'plant B'
        public async Task<int> readSoilHumiditySensor(bool sensor)
        {
            byte[] response = new byte[2];
            byte[] command = new byte[1];
            if (sensor)
            {
                command[0] = (byte)MethodSelector.MEASURE_SOIL_A;
                arduino.Write(command);
                await Task.Delay(5);
                arduino.Read(response);
                Array.Reverse(response);
                return BitConverter.ToUInt16(response, 0);
            }
            else
            {
                command[0] = (byte)MethodSelector.MEASURE_SOIL_B;
                arduino.Write(command);
                await Task.Delay(5);
                arduino.Read(response);
                Array.Reverse(response);
                return BitConverter.ToUInt16(response, 0);
            }
        }
    }
}