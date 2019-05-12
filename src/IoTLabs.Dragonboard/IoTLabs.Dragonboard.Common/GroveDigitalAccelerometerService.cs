using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace IoTLabs.Dragonboard.Common
{

    public class GroveDigitalAccelerometerState : ISensorState
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double Z { get; set; } = 0;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    }

    enum DigitalAccelerometerRegister : byte
    {
        PowerControl = 0x2D,
        DataFormat = 0x31,
        XAxisData = 0x32,
        YAxisData = 0x34,
        ZAxisData = 0x36,
    }

    //Adxl345
    public class GroveDigitalAccelerometerService : ISensor<GroveDigitalAccelerometerState>, IPollingSensor<GroveDigitalAccelerometerState>
    {
        private I2cDevice _i2CAccelerometer;

        internal byte i2CAddress = 0x53; // Default 7-bit I2C address of the ADXL345 
        internal int i2CPort = 1; // 10 bits of resolution
        internal int AccelerometerResolution = 1024; // 10 bits of resolution
        internal int AccelerometerDynamicRange = 8; // Total dynamic range of 8G, since we're configuring it to +-4G */
                
        internal GroveDigitalAccelerometerService()
        {
        }

        public bool IsWriteable => false;

        public async Task<bool> Initialize()
        {
            try
            {
                var settings = new I2cConnectionSettings(i2CAddress)
                {
                    BusSpeed = I2cBusSpeed.FastMode,
                };

                string aqs = I2cDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);
                _i2CAccelerometer = await I2cDevice.FromIdAsync(dis[i2CPort].Id, settings);
                if (_i2CAccelerometer == null)
                {
                    throw new ApplicationException("I2C device not found");
                }

                // 0x01 sets range to +- 4Gs 
                byte[] writeBufferDataFormat = new byte[] { (byte)DigitalAccelerometerRegister.DataFormat, 0x01 };
                _i2CAccelerometer.Write(writeBufferDataFormat);

                // 0x08 puts the accelerometer into measurement mode
                byte[] writeBufferPowerControl = new byte[] { (byte)DigitalAccelerometerRegister.PowerControl, 0x08 };
                _i2CAccelerometer.Write(writeBufferPowerControl);

                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }

        public GroveDigitalAccelerometerState GetState()
        {
            try
            {
                byte[] readBuffer = new byte[6];
                byte[] regAddressBuffer = new byte[] { (byte)DigitalAccelerometerRegister.XAxisData };

                _i2CAccelerometer.WriteRead(regAddressBuffer, readBuffer);

                /* In order to get the raw 16-bit data values, we need to concatenate two 8-bit bytes for each axis */
                short accelerationRawX = BitConverter.ToInt16(readBuffer, 0);
                short accelerationRawY = BitConverter.ToInt16(readBuffer, 2);
                short accelerationRawZ = BitConverter.ToInt16(readBuffer, 4);

                double UnitsPerG = ((double)AccelerometerResolution) / ((double)AccelerometerDynamicRange);

                /* Convert raw values to G's */
                GroveDigitalAccelerometerState accel = new GroveDigitalAccelerometerState()
                {
                    X = (double)accelerationRawX / UnitsPerG,
                    Y = (double)accelerationRawY / UnitsPerG,
                    Z = (double)accelerationRawZ / UnitsPerG,
                };

                return accel;
            }
            catch (Exception e)
            {

            }

            return null;
        }

        public void Close()
        {
            if (_i2CAccelerometer != null)
            {
                _i2CAccelerometer.Dispose();
            }
        }

        public bool WriteState(GroveDigitalAccelerometerState payload)
        {
            //Sensor is readonly
            return false;
        }


        #region "Polling"

        private System.Threading.Timer _tmrUpdate = null;
        private readonly TimeSpan timerDue = new TimeSpan(0, 0, 10);
        private Action<GroveDigitalAccelerometerState> _myAction = null;

        public void Register(Action<GroveDigitalAccelerometerState> action, long pollingIntervalMs)
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
                    var tmp = GetState();
                    if (tmp != null)
                        _myAction.Invoke(tmp);
                }
                catch
                {
                }

            }
        }

        #endregion

    }
}
