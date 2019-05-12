using System;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;

namespace IoTLabs.Dragonboard.Common
{
    public class GpsLocationSensorState : ISensorState
    {
        public BasicGeoposition Position { get; set; }
        public bool HasPosition { get; set; } = false;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    }

    public class GpsLocationSensor : ISensor<GpsLocationSensorState>, IObservableSensor<GpsLocationSensorState>
    {
        Geolocator _geolocator = null;
        GpsLocationSensorState _lastState = null;

        public bool IsWriteable => false;


        internal GpsLocationSensor()
        {
        }

        public void Close()
        {
            try
            {
                if (_geolocator != null)
                {
                    _geolocator.PositionChanged -= OnPositionChanged;
                    _geolocator.StatusChanged -= OnStatusChanged;
                    _geolocator = null;

                }
            }
            catch
            {
            }
        }

        public GpsLocationSensorState GetState()
        {
            return _lastState;
        }

        public async Task<bool> Initialize()
        {
            try
            {
                var accessStatus = await Geolocator.RequestAccessAsync();

                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        _geolocator = new Geolocator { ReportInterval = 2000 };
                        _geolocator.PositionChanged += OnPositionChanged;
                        _geolocator.StatusChanged += OnStatusChanged;
                        return true;
                        break;

                    case GeolocationAccessStatus.Denied:
                        //Shouldn't happen on IoT Core unless sensor is not available
                        break;

                    case GeolocationAccessStatus.Unspecified:
                        //Shouldn't happen on IoT Core unless sensor is not available
                        break;
                }

            }
            catch
            {
            }
            
            return false;
        }

        private void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            
        }

        async private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            try
            {

                if (e.Position != null)
                    if (e.Position.Coordinate != null)
                        if (e.Position.Coordinate.Point != null)
                        {
                            _lastState = new GpsLocationSensorState() { Position = e.Position.Coordinate.Point.Position, HasPosition=true };
                            if (myAction != null)
                            {
                                myAction.Invoke(_lastState);
                            }
                            return;
                        }

                _lastState = new GpsLocationSensorState() { HasPosition = false };
                if (myAction != null)
                {
                    myAction.Invoke(_lastState);
                }
            }
            catch
            {
            }
        }


        #region "Observable"

        private Action<GpsLocationSensorState> myAction = null;


        public void Register(Action<GpsLocationSensorState> action)
        {
            myAction = action;
        }

        public void Unregister()
        {
            if (myAction != null)
                myAction = null;
        }

        public bool WriteState(GpsLocationSensorState payload)
        {
            //do nothing.
            return false;
        }

        #endregion
    }
}
