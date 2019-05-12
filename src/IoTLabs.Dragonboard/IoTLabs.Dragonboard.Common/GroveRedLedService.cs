using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IoTLabs.Dragonboard.Common
{

    public class GroveRedLedSensorState : ISensorState
    {
        public GpioPinValue CurrentValue { get; set; } = GpioPinValue.Low;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        public static GroveRedLedSensorState GroveRedLedSensorStateLow()
        {
            return new GroveRedLedSensorState() {CurrentValue = GpioPinValue.Low};
        }
        public static GroveRedLedSensorState GroveRedLedSensorStateHigh()
        {
            return new GroveRedLedSensorState() { CurrentValue = GpioPinValue.High };
        }
    }


    public class GroveRedLedService : ISensor<GroveRedLedSensorState>
    {
        
        private int _outputGpioPinNumber = 35;
        private GpioPin _outputGpioPin = null;

        
        internal GroveRedLedService() : this(35)
        {
        }

        internal GroveRedLedService(int outputGpioPinNumber)
        {
            _outputGpioPinNumber = outputGpioPinNumber;
        }


        public async Task<bool> Initialize()
        {
            try
            {
                GpioController gpioController = GpioController.GetDefault();
                _outputGpioPin = gpioController.OpenPin(_outputGpioPinNumber);
                _outputGpioPin.SetDriveMode(GpioPinDriveMode.Output);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

        public GroveRedLedSensorState GetState()
        {
            try
            {
                return new GroveRedLedSensorState()
                {
                    CurrentValue = _outputGpioPin.Read()
                };
            }
            catch (Exception e)
            {
                
            }

            return null;
        }

        public bool IsWriteable => true;

        public bool WriteState(GroveRedLedSensorState payload)
        {
            try
            {
                if (payload != null)
                {
                    _outputGpioPin.Write(payload.CurrentValue);
                    return true;
                }
            }
            catch (Exception e)
            {

            }

            return false;
        }

        public void Close()
        {
            //no action required
        }

        
    }
}
