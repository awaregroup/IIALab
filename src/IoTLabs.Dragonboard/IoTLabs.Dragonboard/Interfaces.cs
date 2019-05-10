using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTLabs.Dragonboard
{

    public interface ISensor<T>
    {
        Task<bool> Initialize();
        T GetState();
        bool WriteState(T payload);
        void Close();
        bool IsWriteable { get; }
    }

    public interface IObservableSensor<T>
    {
        void Register(Action<T> action);
        void Unregister();
    }
    
    public interface IPollingSensor<T>
    {
        void Register(Action<T> action, long pollingIntervalMs);
        void Unregister();
    }

}
