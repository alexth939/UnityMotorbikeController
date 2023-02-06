using UnityEngine;

namespace Moto
{
    public class WheelComponent
    {
        public Transform Transform { get; set; }
        public Transform Axle { get; set; }
        public WheelCollider Collider { get; set; }
        public Vector3 StartPos { get; set; }
        public float Rotation { get; set; }
        public float MaxSteer { get; set; }
        public bool IsEngineDriven { get; set; }
        public float Pos_y { get; set; }
    }
}
