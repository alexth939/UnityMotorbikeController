using UnityEngine;
namespace Moto
{
    [System.Serializable]
    public class BikeWheels
    {
        [field: SerializeField] public ConnectWheel Wheels { get; private set; }
        [field: SerializeField] public WheelSettings Settings { get; set; }
    }
}
