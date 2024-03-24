using System;
using UnityEngine;

namespace Moto
{
    [Serializable]
    public class BikeConfiguration
    {
        [field: SerializeField] public Vector3 ShiftCenter { get; set; } = new Vector3(0.0f, -0.6f, 0.0f);
        [field: SerializeField] public float MaxWheelie { get; private set; } = 40.0f;
        [field: SerializeField] public float SpeedWheelie { get; private set; } = 30.0f;
        [field: SerializeField] public float SlipBrake { get; private set; } = 3.0f;
        [field: SerializeField] public float Springs { get; private set; } = 280000.0f;
        [field: SerializeField] public float Dampers { get; private set; } = 4000.0f;
        [field: SerializeField] public float BikePower { get; private set; } = 30;
        [field: SerializeField] public float ShiftPower { get; private set; } = 150;
        [field: SerializeField] public float BrakePower { get; private set; } = 8000;
        [field: SerializeField] public float ShiftDownRPM { get; private set; } = 1500.0f;
        [field: SerializeField] public float ShiftUpRPM { get; private set; } = 4000.0f;
        [field: SerializeField] public float IdleRPM { get; private set; } = 700.0f;
        [field: SerializeField] public float Stiffness { get; private set; } = 1.0f;
        [field: SerializeField] public bool IsAutomaticGear { get; private set; } = true;
        [field: SerializeField] public float LimitBackwardSpeed { get; private set; } = 3.0f;
        [field: SerializeField] public float LimitForwardSpeed { get; private set; } = 220.0f;
        [field: SerializeField] public float MaxSteerAngle { get; private set; } = 50.0f;
        [field: SerializeField] public float MaxTurn { get; private set; } = 0.3f;
    }
}
