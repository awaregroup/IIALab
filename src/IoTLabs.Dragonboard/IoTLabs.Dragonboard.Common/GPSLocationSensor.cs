using System;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;

namespace IoTLabs.Dragonboard.Common
{
    public class GpsLocationSensorState : ISensorState
    {
        public Geoposition Position { get; set; } = null;
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public DateTimeOffset Timestamp { get; set; }
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
                        // Create Geolocator and define perodic-based tracking (2 second interval).
                        _geolocator = new Geolocator { ReportInterval = 2000 };

                        // Subscribe to the PositionChanged event to get location updates.
                        _geolocator.PositionChanged += OnPositionChanged;

                        // Subscribe to StatusChanged event to get updates of location status changes.
                        _geolocator.StatusChanged += OnStatusChanged;

                        //_rootPage.NotifyUser("Waiting for update...", NotifyType.StatusMessage);
                        //LocationDisabledMessage.Visibility = Visibility.Collapsed;
                        //StartTrackingButton.IsEnabled = false;
                        //StopTrackingButton.IsEnabled = true;

                        return true;

                        break;

                    case GeolocationAccessStatus.Denied:
                        //_rootPage.NotifyUser("Access to location is denied.", NotifyType.ErrorMessage);
                        //LocationDisabledMessage.Visibility = Visibility.Visible;
                        break;

                    case GeolocationAccessStatus.Unspecified:
                        //_rootPage.NotifyUser("Unspecificed error!", NotifyType.ErrorMessage);
                        //LocationDisabledMessage.Visibility = Visibility.Collapsed;
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
                _lastState = new GpsLocationSensorState() { Position = e.Position};
                if (myAction != null)
                {
                    myAction.Invoke(_lastState);
                }
            }
            catch
            {
            }

            //await System.ServiceModel.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    //_rootPage.NotifyUser("Location updated.", NotifyType.StatusMessage);
            //    //UpdateLocationData(e.Position);
            //});
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
