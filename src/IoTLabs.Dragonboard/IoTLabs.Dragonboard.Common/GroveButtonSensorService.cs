using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;


namespace IoTLabs.Dragonboard.Common
{

    public class GroveButtonSensorState : ISensorState
    {
        public bool IsPressed { get; set; } = false;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    }

    public class GroveButtonSensorService : ISensor<GroveButtonSensorState>, IObservableSensor<GroveButtonSensorState>
    {
        private int _interruptPinNumber = 115;
        private GpioPin _interruptGpioPin = null;
        private GroveButtonSensorState _lastState = null;


        internal GroveButtonSensorService() : this(115)
        {
        }

        internal GroveButtonSensorService(int interruptPinNumber)
        {
            _interruptPinNumber = interruptPinNumber;
        }

        public async Task<bool> Initialize()
        {
            try
            {
                GpioController gpioController = GpioController.GetDefault();

                _interruptGpioPin = gpioController.OpenPin(_interruptPinNumber);
                _interruptGpioPin.SetDriveMode(GpioPinDriveMode.InputPullDown);
                _interruptGpioPin.ValueChanged += InterruptGpioPinOnValueChanged;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }
        public bool IsWriteable => false;

        public GroveButtonSensorState GetState()
        {
            return _lastState;
        }

        public bool WriteState(GroveButtonSensorState payload)
        {
            //Not Writeable
            return false;
        }

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
                _lastState = new GroveButtonSensorState()
                {
                    IsPressed = (args.Edge == GpioPinEdge.RisingEdge)
                };

                if (myAction != null)
                    myAction.Invoke(_lastState);

            }
            catch
            {
            }

        }

        #region "Observable"

        private Action<GroveButtonSensorState> myAction = null;

        public void Register(Action<GroveButtonSensorState> action)
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
