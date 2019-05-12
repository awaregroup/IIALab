using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IoTLabs.Dragonboard.Common
{

    public class GroveMiniPIRMotionSensorState : ISensorState
    {
        public GpioPinEdge CurrentEdge { get; set; } = GpioPinEdge.RisingEdge;
        public int PinNumber { get; set; } = -1;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    }

    //PIR
    public class GroveMiniPIRMotionSensorService  : ISensor<GroveMiniPIRMotionSensorState>, IObservableSensor<GroveMiniPIRMotionSensorState>
    {
        private int InterruptPinNumber = 24;
        private GpioPin _interruptGpioPin = null;
        private GroveMiniPIRMotionSensorState _lastState = null;


        internal GroveMiniPIRMotionSensorService() : this(24)
        {
        }

        internal GroveMiniPIRMotionSensorService(int interruptPinNumber = 24)
        {
            InterruptPinNumber = interruptPinNumber;
        }

 

      
        public async Task<bool> Initialize()
        {
            try
            {
                GpioController gpioController = GpioController.GetDefault();

                _interruptGpioPin = gpioController.OpenPin(InterruptPinNumber);
                _interruptGpioPin.SetDriveMode(GpioPinDriveMode.InputPullDown);
                _interruptGpioPin.ValueChanged += InterruptGpioPinOnValueChanged;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public GroveMiniPIRMotionSensorState GetState()
        {
            return _lastState;
        }

        public bool WriteState(GroveMiniPIRMotionSensorState payload)
        {
            //Sensor is readonly
            return false;
        }
        
        public bool IsWriteable => false;


        public void Close()
        {
            try
            {
                if (_interruptGpioPin != null)
                    _interruptGpioPin.ValueChanged -= InterruptGpioPinOnValueChanged;
            }
            catch (Exception e)
            {

            }
        }

        private void InterruptGpioPinOnValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {

            try
            {
                _lastState = new GroveMiniPIRMotionSensorState()
                {
                    CurrentEdge = args.Edge,
                    PinNumber = sender.PinNumber
                };

                if (myAction != null)
                    myAction.Invoke(_lastState);

            }
            catch
            {
            }

        }

        #region "Observable"

        private Action<GroveMiniPIRMotionSensorState> myAction = null;

        
        public void Register(Action<GroveMiniPIRMotionSensorState> action)
        {
            myAction = action;
        }

        public void Unregister()
        {
            myAction = null;
        }
        
        #endregion

        
    }
}
