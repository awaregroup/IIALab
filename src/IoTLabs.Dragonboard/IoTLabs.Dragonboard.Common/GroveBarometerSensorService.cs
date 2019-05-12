using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Glovebox.IoT.Devices.Sensors;

namespace IoTLabs.Dragonboard.Common
{

    public class GroveBarometerSensorState : ISensorState 
    {
        public double TemperatureCelcius { get; set; } = -272;
        public double TemperatureFahrenheit { get; set; } = -272;
        public double HumidityPercent { get; set; } = 0;
        public double PressureKilopascals { get; set; } = 0;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

    }

    //BME280
    public class GroveBarometerSensorService : ISensor<GroveBarometerSensorState>, IPollingSensor<GroveBarometerSensorState>
    {

        private BME280 _bme280Sensor;
        private int _i2cAddress = 0x76;
        private GroveBarometerSensorState _lastState = null;

        internal GroveBarometerSensorService() : this(0x76)
        {
        }

        internal GroveBarometerSensorService(int i2cAddress = 0x76)
        {
            _i2cAddress = i2cAddress;
        }

        public bool IsWriteable => false;

        public async Task<bool> Initialize()
        {
            try
            {
                _bme280Sensor = new BME280(_i2cAddress);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return false;
        }

        private GroveBarometerSensorState GetBarometerState()
        {
            try
            {
                return new GroveBarometerSensorState()
                {
                    Timestamp = DateTime.Now,
                    HumidityPercent = _bme280Sensor.Humidity,
                    PressureKilopascals = _bme280Sensor.Pressure.Kilopascals,
                    TemperatureCelcius = _bme280Sensor.Temperature.DegreesCelsius,
                    TemperatureFahrenheit = _bme280Sensor.Temperature.DegreesFahrenheit
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return null;
        }

        public void Close()
        {
            
        }

        public bool WriteState(GroveBarometerSensorState payload)
        {
            //sensor is read only
            return false;
        }

        public GroveBarometerSensorState GetState()
        {
            return _lastState;
        }


        #region "Polling"

        private System.Threading.Timer _tmrUpdate = null;
        private readonly TimeSpan timerDue = new TimeSpan(0, 0, 5);
        private Action<GroveBarometerSensorState> _myAction = null;

        public void Register(Action<GroveBarometerSensorState> action, long pollingIntervalMs)
        {
            try
            {
                if (pollingIntervalMs > 0)
                {
                    _myAction = action;
                    _tmrUpdate = new System.Threading.Timer(SensorUpdateTimerCallback, null, timerDue, TimeSpan.FromMilliseconds(pollingIntervalMs));
                }
            }
            catch
            {
            }
        }

        public void Unregister()
        {
            if (_tmrUpdate != null)
            {
                _tmrUpdate.Dispose();
                _myAction = null;
            }
        }


        private void SensorUpdateTimerCallback(object state)
        {
            if (_myAction != null)
            {
                try
                {
                    Action act = new Action(() =>
                    {
                        var tmp = GetBarometerState();
                        if (tmp != null)
                        {
                            _lastState = tmp;
                            if (_myAction!=null)
                                _myAction.Invoke(tmp);
                        }
                    });

                    Task.Run(act);

                }
                catch
                {
                }

            }
        }

        #endregion

        
    }

}
