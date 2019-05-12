using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTLabs.Dragonboard
{

    public static class OnboardSensorFactory
    {
        public static ISensor<GpsLocationSensorState> CreateLocationSensor()
        {
            return new GpsLocationSensor();
        }
    }

    public static class GroveSensorFactory
    {

        public static ISensor<GroveMiniPIRMotionSensorState> CreateMiniPIRMotionSensorService(int interruptPinNumber = 24)
        {
            GroveMiniPIRMotionSensorService item = new GroveMiniPIRMotionSensorService(interruptPinNumber);
            return item;
        }

        public static ISensor<GroveDigitalAccelerometerState> CreateAccelerometerSensorService(byte i2caddress = 0x53, int i2cport = 1, int resolution = 1024, int dynamicRange = 8)
        {
            GroveDigitalAccelerometerService item = new GroveDigitalAccelerometerService();
            item.i2CPort = i2cport;
            item.i2CAddress = i2caddress;
            item.AccelerometerResolution = resolution;
            item.AccelerometerDynamicRange = dynamicRange;
            return item;
        }

        public static ISensor<GroveRedLedSensorState> CreateRedLedSensorService(int outputGpioPinNumber = 35)
        {
            GroveRedLedService item = new GroveRedLedService(outputGpioPinNumber);
            return item;
        }

        public static ISensor<GroveBaramoterSensorState> CreateBarometerSensorService(int i2caddress = 0x76)
        {
            GroveBaramoterSensorService item = new GroveBaramoterSensorService(i2caddress);
            return item;
        }

        public static ISensor<GroveButtonSensorState> CreateGroveButtonSensorService(int interruptPinNumber = 115)
        {
            GroveButtonSensorService item = new GroveButtonSensorService(interruptPinNumber);
            return item;
        }

    }
}
