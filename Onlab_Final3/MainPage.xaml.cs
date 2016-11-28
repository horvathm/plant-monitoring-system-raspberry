using Microsoft.IoT.Lightning.Providers;
using Onlab_Final3.Converters;
using Onlab_Final3.Devices;
using Onlab_Final3.Devices.I2c_devices;
using Onlab_Final3.Devices.Onlab_Projekt2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Onlab_Final3
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        #region Fields
        private DispatcherTimer meas_timer;         // do the measurements
        private DispatcherTimer react_timer;        // handle the servos and relays in auto mode
        private SensorADS1115 ads1115;
        private SensorBMP180 bmp180;
        private Arduino arduinoDue;
        private Relay relay;
        private Servo servo;
        #endregion

        // properties with the set method to notify the gui
        #region Properties
        // mode of the app where true = auto, false = manual
        public bool Mode
        {
            get { return _mode; }
            set { Set(ref _mode, value); }
        }
        private bool _mode = true;

        // represents the state of the arduino servo
        public bool ServoAB
        {
            get { return _servoAB; }
            set { Set(ref _servoAB, value); }
        }
        private bool _servoAB = false;

        // represent the state of the raspberry servo
        public bool ServoCD
        {
            get { return _servoCD; }
            set { Set(ref _servoCD, value); }
        }
        private bool _servoCD = true;

        // contains the measured temperature
        public double Temperature {
            get { return _temperature; }
            private set { Set(ref _temperature,value); }
        }
        private double _temperature;

        // contains the measured pressure
        public double BarometricPressure
        {
            get { return _barometricPressure; }
            private set { Set(ref _barometricPressure, value); }
        }
        private double _barometricPressure;

        // contains the measuret brightness
        public int Luminance
        {
            get { return _luminance; }
            private set { Set(ref _luminance, value); }
        }
        private int _luminance;

        // contains the mesasured humidity of 'plant A'
        public int HumidityA
        {
            get { return _humidityA; }
            private set { Set(ref _humidityA, value); }
        }
        private int _humidityA;

        // contains the mesasured humidity of 'plant B'
        public int HumidityB
        {
            get { return _humidityB; }
            private set { Set(ref _humidityB, value); }
        }
        private int _humidityB;

        // contains the mesasured humidity of 'plant C'
        public int HumidityC
        {
            get { return _humidityC; }
            private set { Set(ref _humidityC, value); }
        }
        private int _humidityC;

        // contains the mesasured humidity of 'plant D'
        public int HumidityD
        {
            get { return _humidityD; }
            private set { Set(ref _humidityD, value); }
        }
        private int _humidityD;
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            // if unchanged return false
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            // if PropertyChanged not null call the Invoke method
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public MainPage()
        {
            this.InitializeComponent();

            // Setting the DataContext
            this.DataContext = this;

            // Register for the unloaded event so we can clean up upon exit
            Unloaded += MainPage_Unloaded;

            // Set Lightning as the default provider
            if (LightningProvider.IsLightningEnabled)
                LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();

            // Initialize the DispatcherTimer
            meas_timer = new DispatcherTimer();
            meas_timer.Interval = TimeSpan.FromMilliseconds(1000);
            meas_timer.Tick += timer_tick_measurement;
            meas_timer.Start();

            // Initialize the sensors
            InitializeSensors();
        }

        private async void InitializeSensors()
        {
            // BMP180 initialize
            #region SensorBMP180
            try
            {
                bmp180 = new SensorBMP180();
                await bmp180.InitializeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Initialization has failed: " + ex);
            }
            #endregion

            // ADS1115 initialize

            
            #region SensorADS1115
            try
            {
                ads1115 = new SensorADS1115(AdcAddress.GND);
                await ads1115.InitializeAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Initialization has failed: " + ex);
            }
            #endregion
            
            // Arduino initialize
            #region Arduino
            try
            {
                arduinoDue = new Arduino();
                await arduinoDue.InitializeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Initialization has failed: " + ex);
            }
            #endregion
            
            // Relay initialize
            #region Relay
            relay = new Relay(18);
            relay.Initialize();
            #endregion

            // Servo initialize
            #region Servo
            servo = new Servo(5);
            await servo.InitializeAsync();
            #endregion

        }

        // sensor readings only
        private async void timer_tick_measurement(object sender, object e)
        {
            // read the BMP180 sensorData and write to the properties (Temperature, Pressure)
            #region SensorBMP180
            if (bmp180 != null && bmp180.IsInitialized)
            {
                try
                {
                    var sensorData = await bmp180.GetSensorDataAsync(Bmp180AccuracyMode.Standard);
                    Temperature = sensorData.Temperature;
                    BarometricPressure = sensorData.Pressure;
                }
                catch (Exception ex)
                {
                    throw new Exception("Read from BMP180 has failed: " + ex);
                }
            }
            #endregion
            
            // read the analog values with the ADC and write to the properties (Luminance, HumidityC, HumidityD)
            #region SensorADS1115
            if (ads1115 != null && ads1115.IsInitialized)
            {
                var settings = new ADS1115SensorSetting()
                {
                    Mode = AdcMode.SINGLESHOOT_CONVERSION,
                    Pga = AdcPga.G1,
                    DataRate = AdcDataRate.SPS860
                };

                try
                {
                    settings.Input = AdcInput.A1_SE;
                    HumidityC = (await ads1115.GetSingleSensorData(settings)).DecimalValue;
                    settings.Input = AdcInput.A2_SE;
                    HumidityD = (await ads1115.GetSingleSensorData(settings)).DecimalValue;
                    settings.Input = AdcInput.A0_SE;
                    settings.Pga = AdcPga.G2;
                    Luminance = (await ads1115.GetSingleSensorData(settings)).DecimalValue;
                }
                catch (Exception ex)
                {
                    throw new Exception("Read from ADS1115 has failed: " + ex);
                }



            }
            #endregion
            
            // read the soil moisture values from the arduino and write to the properties (HumudityA, HumidityB)
            #region Arduino
            if (arduinoDue != null && arduinoDue.IsInitialized)
            {
                try
                {
                    HumidityA = await arduinoDue.readSoilHumiditySensor(true);
                    HumidityB = await arduinoDue.readSoilHumiditySensor(false);
                }
                catch (Exception ex)
                {
                    throw new Exception("Read from BMP180 has failed: " + ex);
                }

               

            }
            #endregion
            
            // intit timer that handle servos and relays in auto mode

            if (react_timer == null && Mode)     // after the first measurement we still in auto mode then we init the react timer
            {
                react_timer = new DispatcherTimer();
                react_timer.Interval = TimeSpan.FromSeconds(15); //FromMinutes(10); wolud suit more for real usage
                react_timer.Tick += timer_tick_watermanagement;
                react_timer.Start();
            }
        }

        // auto mode 
        private void timer_tick_watermanagement(object sender, object e)
        {
            // read the category
            var a = HumidityConverterBase.ConvertValueToCategory(HumidityA);
            var b = HumidityConverterBase.ConvertValueToCategory(HumidityB);
            var c = HumidityConverterBase.ConvertValueToCategory(HumidityC);
            var d = HumidityConverterBase.ConvertValueToCategory(HumidityD);

            // if worse than 2 then
            if(a > 2 || b > 2)
            {
                // water that one witch has worse results (larger value means dry)
                if(HumidityA > HumidityB)
                {
                    // water 'plant A'
                    ServoAB = true;
                    Task.Run(() => {
                        arduinoDue.moveServoToPosition(true); // servo move 'plant A'
                        arduinoDue.turnRelay(true);             //relay on
                        Task.Delay(2000).Wait();                //wait 20 sec but for test it's only 2 sec
                        arduinoDue.turnRelay(false);            //relay off
                    });
                }
                else
                {
                    // water 'plant B'
                    ServoAB = false;
                    Task.Run(() => {
                        arduinoDue.moveServoToPosition(false); 
                        arduinoDue.turnRelay(true);     
                        Task.Delay(2000).Wait();        
                        arduinoDue.turnRelay(false); 
                    });
                }
            }

            if (c > 2 || d > 2)
            {
                if (HumidityC > HumidityD)
                {
                    // water 'plant C'
                    ServoCD = false;
                    servo.MoveToPeriod(2.6);
                    relay.Write(Windows.Devices.Gpio.GpioPinValue.High);
                    Task.Delay(4000).Wait();
                    relay.Write(Windows.Devices.Gpio.GpioPinValue.Low);
                }
                else
                {
                    // water 'plant D'
                    ServoCD = true;
                    servo.MoveToPeriod(0.7);
                    relay.Write(Windows.Devices.Gpio.GpioPinValue.High);
                    Task.Delay(4000).Wait();
                    relay.Write(Windows.Devices.Gpio.GpioPinValue.Low);
                    
                }
            }
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (bmp180 != null)
            {
                bmp180.Dispose();
                bmp180 = null;
            };
            
            if (ads1115 != null)
            {
                ads1115.Dispose();
                ads1115 = null;
            };

            if (arduinoDue != null)
            {
                arduinoDue.Dispose();
                arduinoDue = null;
            }
            
            if (relay != null)
            {
                relay.Dispose();
                relay = null;
            }
            
            if(servo != null)
            {
                servo.Dispose();
                servo = null;
            }

            meas_timer.Stop();
            meas_timer = null;
        }

        #region EventHandlers
        // mode changed event
        private void rp_modeAM_Click(object sender, RoutedEventArgs e)
        {
            if(Mode)
            {
                react_timer.Start();
            }
            else
            {
                react_timer.Stop();
                arduinoDue.turnRelay(false);
                relay.Write(Windows.Devices.Gpio.GpioPinValue.Low);
            }
        }
    
        private void rb_plantAB_Click(object sender, RoutedEventArgs e)
        {
            if (arduinoDue.RelayState == true)
                arduinoDue.turnRelay(false);

            if(ServoAB)
            {
                arduinoDue.moveServoToPosition(true);
            }
            else
            {
                arduinoDue.moveServoToPosition(false);
            }
        }

        private void rb_plantCD_Click(object sender, RoutedEventArgs e)
        {
            if (relay.RelayState == Windows.Devices.Gpio.GpioPinValue.High)
                relay.Write(Windows.Devices.Gpio.GpioPinValue.Low);

            if (ServoCD)
            {
                servo.MoveToPeriod(0.7);
            }
            else
            {
                servo.MoveToPeriod(2.6);
            }
        }

        private void bt_plantAB_Click(object sender, RoutedEventArgs e)
        {
            arduinoDue.changeRelayState();
        }

        private void bt_plantCD_Click(object sender, RoutedEventArgs e)
        {
            relay.changeRelayState();
        }
        #endregion

    }

}