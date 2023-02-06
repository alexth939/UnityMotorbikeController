using System;
using UnityEngine;

namespace Moto
{
    [Serializable]
    public class ConnectWheel
    {
        [field: SerializeField] public Transform WheelFront { get; private set; }
        [field: SerializeField] public Transform WheelBack { get; private set; }

        [field: SerializeField] public Transform AxleFront { get; private set; }
        [field: SerializeField] public Transform AxleBack { get; private set; }
    }
}
