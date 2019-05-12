using IoTLabs.Dragonboard.Common;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace IoTLabs.Dragonboard.App.Services
{

    public class IoTHubSensorPack
    {
        public Dictionary<string, ISensorState> AllPayloads = new Dictionary<string, ISensorState>();
    }

    public class IoTHubService
    {

        DeviceClient _deviceClient;
        DateTimeOffset _expiryTime = DateTimeOffset.MinValue;
        string _connectionString = "";


        public IoTHubService(string connectionString)
        {
            _connectionString = connectionString;
            
        }

        public async Task Close()
        {
            try
            {
                
                if (_deviceClient != null)
                {
                    try
                    {
                        await _deviceClient.CloseAsync();
                    }
                    catch
                    {
                    }
                    _deviceClient = null;
                }
            }
            catch
            {
            }

        }


        public async Task CheckInitializeDeviceClient()
        {
            //Close Connection if expired..
            try
            {
                if (DateTimeOffset.Now.Ticks > _expiryTime.Ticks)
                {
                    if (_deviceClient != null)
                    {
                        try
                        {
                            await _deviceClient.CloseAsync();
                        }
                        catch
                        {
                        }
                        _deviceClient = null;

                    }
                }
            }
            catch
            {
            }

            //Create New Client if needed..
            if (_deviceClient == null)
            {
                try
                {
                    _deviceClient = DeviceClient.CreateFromConnectionString(_connectionString);
                    _expiryTime = DateTimeOffset.Now.AddSeconds(240);
                }
                catch
                {
                }

            }

        }


        public async Task<bool> SubmitSensorsPayload(IoTHubSensorPack pack)
        {

            await CheckInitializeDeviceClient();            

            //Submit the results
            try
            {
                if (_deviceClient != null)
                {
                    string payload = JsonConvert.SerializeObject(pack);

                    Debug.WriteLine("Sending Payload : " + payload);
                    await _deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(payload)));
                    Debug.WriteLine("Sent Payload");
                    return true;
                }
            }
            catch (Exception e)
            {
                string err = e.Message;
                Debug.WriteLine("Error Sending Payload : " + e.Message);
            }
            return false;
        }


        

    }
}
