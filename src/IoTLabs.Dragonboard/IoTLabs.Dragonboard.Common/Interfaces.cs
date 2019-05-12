using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTLabs.Dragonboard.Common
{

    public interface ISensor<T> where T : ISensorState
    {
        Task<bool> Initialize();
        T GetState();
        bool WriteState(T payload);
        void Close();
        bool IsWriteable { get; }
    }

    public interface IObservableSensor<T> where T : ISensorState
    {
        void Register(Action<T> action);
        void Unregister();
    }

    public interface IPollingSensor<T> where T : ISensorState
    {
        void Register(Action<T> action, long pollingIntervalMs);
        void Unregister();
    }

    public interface ISensorState
    {
        DateTimeOffset Timestamp { get; set; }
    }

}
