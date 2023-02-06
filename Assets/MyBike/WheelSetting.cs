using System;
using UnityEngine;

namespace Moto
{
    [Serializable]
    public class WheelSettings
    {
        [field: SerializeField] public float Radius { get; private set; } = 0.3f;
        [field: SerializeField] public float Weight { get; private set; } = 1000.0f;

        /// <summary>
        ///     Maximum extension distance of wheel suspension, measured in local space.
        /// </summary>
        [field: SerializeField] public float Distance { get; private set; } = 0.2f;
    }
}
