// Vocabulary:
// Handlebar - Руль байка
// Inclination - Наклон
// Steer - Поворот
// Rotation - Вращение
// Direction - Направление
// T - Интерполятор
// Torque - Крутящий момент
// Delta - Разница
// RPM - Кол. оборот. в минуту
// Wheelie - "Козлить"
// MagicValue - ХЗ
// 
// 
// 

using UnityEngine;

namespace Moto
{
    public class MyBikeControll : MonoBehaviour
    {
        private const float MagicValue1 = 2.7f;
        private const int MagicValue2 = 50;
        private const float MagicValue3 = 1.3f;
        private const int MagicValue4 = -10;
        private const float MagicValue5 = 1.0f;
        private const float MagicValue6 = 5500.0f;
        private const float MagicValue7 = 0.1f;
        private const float MagicValue8 = 0.9f;
        private const int MagicValue9 = 2;
        private const int MagicValue10 = 10;
        private const int MagicValue11 = 1;
        private const float MagicValue12 = 0.5f;
        private const float MagicValue13 = 0.25f;
        private const float MagicValue14 = 2.0f;
        private const float MagicValue15 = 0.01f;
        private const float MagicValue16 = 0.002f;
        private const float MagicValue17 = 0.02f;
        private const int MagicValue18 = 3000;
        private const float MagicValue19 = 0.02f;
        private const float MagicValue20 = 50.0f;
        private const float MagicValue21 = 10.0f;
        private const int MagicValue22 = 20;
        private const float MagicValue23 = 100.0f;
        private const float MagicValue24 = 5.0f;
        private const float MagicValue25 = 10.0f;
        private const float MagicValue26 = 0.2f;
        private const int MagicValue27 = -10000;
        private const float MagicValue28 = 0.95f;
        private const float MagicValue29 = 0.05f;
        private const float MagicValue30 = 5500.0f;
        private const float MagicValue31 = 5200.0f;
        private const float MagicValue32 = 0.9f;
        private const int MagicValue33 = 2000;
        private const float MagicValue34 = 1.0f;

        private const float MaxSteeringDelta = 0.1f;

        [SerializeField] private AnimationCurve _handlebarSteeringCurve;

        [SerializeField] private float _z_Rotation = 5;

        [SerializeField] private bool _isActiveControl = false;

        [SerializeField] private ConnectWheel _connectedWheels;
        [SerializeField] private WheelSettings _wheelSettings;

        [SerializeField] private bool _showNormalGizmos = false;
        [SerializeField] private Transform _mainBody;
        [SerializeField] private Transform _handlebar;

        [SerializeField] private BikeConfiguration _bikeConfiguration;

        private Quaternion _handlebarZeroRotation;

        /// <summary>
        ///     Around lowest point
        /// </summary>
        private float _bikeInclinationZ;

        /// <summary>
        ///     Clamped between -1.0f(left) and 1.0f(right)
        /// </summary>
        private float _handlebarSteeringT;

        private bool _isBrake;
        private float _slip = 0.0f;

        private bool _isBackward = false;

        private float _steer;

        /// <summary>
        ///     Clamped between -1.0f(backwards) and 1.0f(forward)
        /// </summary>
        private float _accelerationT = 0.0f;

        private float _currentTorque = 100f;
        private float _newTorque;

        private float _speed = 0.0f;

        //private int _currentGear = 1;

        private bool _isNeutralGear = false;

        private float _motorRPM = 0.0f;

        private float _wantedRPM = 0.0f;
        private float _w_rotate;

        private Rigidbody _rigidbody;

        private float _wheelie;

        private WheelComponent[] _wheels;

        [SerializeField] private WheelComponent _frontWheel;
        [SerializeField] private WheelComponent _rearWheel;

        private WheelComponent SetWheelComponent(
            Transform wheelTransform,
            Transform axleTransform,
            bool isEngineDriven,
            float maxSteer)
        {
            GameObject wheelCollider = new(wheelTransform.name + "WheelCollider");

            wheelCollider.transform.parent = transform;
            wheelCollider.transform.position = wheelTransform.position;
            wheelCollider.transform.eulerAngles = transform.eulerAngles;

            var wheelCollider0 = wheelCollider.AddComponent<WheelCollider>();

            return new WheelComponent()
            {
                IsEngineDriven = isEngineDriven,
                Transform = wheelTransform,
                Axle = axleTransform,
                Collider = wheelCollider0,
                MaxSteer = maxSteer,
                AxleDefaultLocalPosition = axleTransform.transform.localPosition
            };
        }

        private void Awake()
        {
            if(_bikeConfiguration.IsAutomaticGear)
                _isNeutralGear = false;

            _rigidbody = transform.GetComponent<Rigidbody>();

            _handlebarZeroRotation = _handlebar.localRotation;
            _wheels = new WheelComponent[2];

            _wheels[0] = SetWheelComponent(
                wheelTransform: _connectedWheels.WheelFront,
                axleTransform: _connectedWheels.AxleFront,
                isEngineDriven: false,
                maxSteer: _bikeConfiguration.MaxSteerAngle
                //maxSteer: _bikeConfiguration.MaxSteerngleCurve.Evaluate(_speed)
                /*pos_y: _connectedWheels.AxleFront.localPosition.y*/);

            _wheels[1] = SetWheelComponent(
                wheelTransform: _connectedWheels.WheelBack,
                axleTransform: _connectedWheels.AxleBack,
                isEngineDriven: true,
                maxSteer: 0
                /*pos_y: _connectedWheels.AxleBack.localPosition.y*/);

            _wheels[0].Collider.transform.localPosition = new Vector3()
            {
                y = _wheels[0].Collider.transform.localPosition.y,
                z = _wheels[0].Collider.transform.localPosition.z
            };

            _wheels[1].Collider.transform.localPosition = new Vector3()
            {
                y = _wheels[1].Collider.transform.localPosition.y,
                z = _wheels[1].Collider.transform.localPosition.z
            };

            foreach(WheelComponent wheel in _wheels)
            {
                WheelCollider collider = wheel.Collider;

                collider.suspensionDistance = _wheelSettings.Distance;
                JointSpring joint = collider.suspensionSpring;

                joint.spring = _bikeConfiguration.Springs;
                joint.damper = _bikeConfiguration.Dampers;
                collider.suspensionSpring = joint;

                collider.radius = _wheelSettings.Radius;
                collider.mass = _wheelSettings.Weight;

                WheelFrictionCurve frictionCurve = collider.forwardFriction;
                frictionCurve.asymptoteValue = 0.5f;
                frictionCurve.extremumSlip = 0.4f;
                frictionCurve.asymptoteSlip = 0.8f;
                frictionCurve.stiffness = _bikeConfiguration.Stiffness;
                collider.forwardFriction = frictionCurve;

                frictionCurve = collider.sidewaysFriction;
                frictionCurve.asymptoteValue = 0.75f;
                frictionCurve.extremumSlip = 0.1f;
                frictionCurve.asymptoteSlip = 0.5f;
                frictionCurve.stiffness = _bikeConfiguration.Stiffness;
                collider.sidewaysFriction = frictionCurve;
            }

            // заднее колесо соскальзывает при езде кругами по наклоненной поверхности.
            WheelFrictionCurve backWheelSidewaysFrictionCurve = _wheels[1].Collider.sidewaysFriction;

            {
                backWheelSidewaysFrictionCurve.extremumValue = 2.0f;
                backWheelSidewaysFrictionCurve.asymptoteSlip = 0.3f;
                backWheelSidewaysFrictionCurve.asymptoteValue = 10.0f;
            }

            _wheels[1].Collider.sidewaysFriction = backWheelSidewaysFrictionCurve;
        }

        private void Update()
        {
            _steer = Mathf.LerpAngle(
                a: _steer,
                b: _handlebarSteeringT * -_bikeConfiguration.MaxSteerAngle,
                t: Time.deltaTime * 10.0f);

            _bikeInclinationZ = Mathf.LerpAngle(
                a: _bikeInclinationZ,
                b: _steer * _bikeConfiguration.MaxTurn * Mathf.Clamp(_speed / _z_Rotation, 0.0f, 1.0f),
                t: Time.deltaTime * 5.0f);

            // this is 90 degrees around y axis
            //if(_handlebar)
            _handlebar.localRotation = _handlebarZeroRotation * Quaternion.Euler(
                x: 0,
                y: _wheels[0].Collider.steerAngle,
                z: 0);

            float _magicFlipRotate = (transform.eulerAngles.z is > 90 and < 270) ? 180.0f : 0.0f;

            _wheelie = Mathf.Clamp(_wheelie, 0, _bikeConfiguration.MaxWheelie);

            _wheelie = Mathf.MoveTowards(
                current: _wheelie,
                target: 0,
                maxDelta: _bikeConfiguration.SpeedWheelie * 2 * Time.deltaTime * MagicValue3);

            var mainBodyLocalRotation = Quaternion.Euler(new Vector3()
            {
                x = -_wheelie,
                z = _magicFlipRotate - transform.localEulerAngles.z + _bikeInclinationZ
            });

            var rigidbodyTargetRotationOffset = Quaternion.Euler(
                x: 0,
                y: 0,
                z: _magicFlipRotate - transform.localEulerAngles.z);

            _rigidbody.MoveRotation(_rigidbody.rotation * rigidbodyTargetRotationOffset);
            _mainBody.localRotation = mainBodyLocalRotation;
        }

        private void FixedUpdate()
        {
            _speed = _rigidbody.velocity.magnitude * MagicValue1;

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.centerOfMass = _bikeConfiguration.ShiftCenter;

            if(_isActiveControl)
            {
                _accelerationT = 0;
                _isBrake = false;

                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");

                _accelerationT = verticalInput;

                _handlebarSteeringT = Mathf.MoveTowards(
                    current: _handlebarSteeringT,
                    target: horizontalInput,
                    maxDelta: 0.2f);// MaxSteeringDelta);

                _isBrake = Input.GetKey(KeyCode.Space);
            }
            else
            {
                _accelerationT = 0.0f;
                _handlebarSteeringT = 0.0f;
                _isBrake = false;
            }

            _wantedRPM = MagicValue6 * _accelerationT * MagicValue7 + _wantedRPM * MagicValue8;

            float rpm = 0;
            int motorizedWheels = 0;

            foreach(WheelComponent wheel in _wheels)
            {
                WheelCollider collider = wheel.Collider;

                if(wheel.IsEngineDriven)
                {
                    if(!_isNeutralGear && _isBrake)
                    {
                        rpm += _accelerationT * _bikeConfiguration.IdleRPM;

                        float someMagicZ = rpm > MagicValue11 ? Mathf.PingPong(
                            t: Time.time * (_accelerationT * MagicValue10),
                            length: MagicValue12) - MagicValue13 : 0;

                        _bikeConfiguration.ShiftCenter = new Vector3()
                        {
                            x = _bikeConfiguration.ShiftCenter.x,
                            y = _bikeConfiguration.ShiftCenter.y,
                            z = someMagicZ
                        };
                    }
                    else
                    {
                        if(!_isNeutralGear)
                        {
                            rpm += collider.rpm;
                        }
                        else
                        {
                            rpm += _bikeConfiguration.IdleRPM * MagicValue14 * _accelerationT;
                        }
                    }

                    motorizedWheels++;
                }


                wheel.Collider.enabled = true;
                wheel.Transform.GetComponent<Collider>().enabled = false;

                if(_isBrake || _accelerationT < 0)
                {
                    if((_accelerationT < 0) || (_isBrake && wheel == _wheels[1]))
                    {
                        if(_isBrake && (_accelerationT > 0))
                        {
                            _slip = Mathf.Lerp(_slip, _bikeConfiguration.SlipBrake, _accelerationT * MagicValue15);
                        }
                        else if(_speed > 1.0f)
                        {
                            _slip = Mathf.Lerp(_slip, 1.0f, MagicValue16);
                        }
                        else
                        {
                            _slip = Mathf.Lerp(_slip, 1.0f, MagicValue17);
                        }

                        _wantedRPM = 0;
                        collider.brakeTorque = _bikeConfiguration.BrakePower;
                        wheel.Rotation = _w_rotate;
                    }
                }
                else
                {
                    collider.brakeTorque = _accelerationT == 0 ? MagicValue18 : 0;
                    _slip = Mathf.Lerp(_slip, 1.0f, MagicValue19);
                    _w_rotate = wheel.Rotation;
                }

                WheelFrictionCurve frictionCurve = collider.forwardFriction;

                if(wheel == _wheels[1])
                {
                    frictionCurve.stiffness = _bikeConfiguration.Stiffness / _slip;
                    collider.forwardFriction = frictionCurve;

                    frictionCurve = collider.sidewaysFriction;
                    frictionCurve.stiffness = _bikeConfiguration.Stiffness / _slip;
                    collider.sidewaysFriction = frictionCurve;
                }

                _currentTorque = _bikeConfiguration.BikePower;

                wheel.Rotation = Mathf.Repeat(
                    t: wheel.Rotation + Time.deltaTime * collider.rpm * 360.0f / 60.0f,
                    length: 360.0f);

                wheel.Transform.localRotation = Quaternion.Euler(wheel.Rotation, 0.0f, 0.0f);

                Vector3 localPosition = wheel.Axle.localPosition;

                if(collider.GetGroundHit(out WheelHit hit) && (wheel == _wheels[1] || (wheel == _wheels[0] && _wheelie == 0)))
                {
                    localPosition.y -= Vector3.Dot(
                        lhs: wheel.Transform.position - hit.point,
                        rhs: transform.TransformDirection(0, 1, 0)) - collider.radius;

                    localPosition.y = Mathf.Clamp(
                        value: localPosition.y,
                        min: wheel.AxleDefaultLocalPosition.y - _wheelSettings.Distance,
                        max: wheel.AxleDefaultLocalPosition.y + _wheelSettings.Distance);

                    _rigidbody.angularDrag = MagicValue25;

                    if(wheel.Collider.GetComponent<WheelSkidmarks>())
                        wheel.Collider.GetComponent<WheelSkidmarks>().enabled = true;
                }
                else
                {
                    if(wheel.Collider.GetComponent<WheelSkidmarks>())
                        wheel.Collider.GetComponent<WheelSkidmarks>().enabled = false;

                    localPosition.y = wheel.AxleDefaultLocalPosition.y - _wheelSettings.Distance;

                    if(!_wheels[0].Collider.isGrounded && !_wheels[1].Collider.isGrounded)
                    {
                        _rigidbody.centerOfMass = new Vector3(0, MagicValue26, 0);
                        _rigidbody.angularDrag = 1.0f;

                        _rigidbody.AddForce(0, MagicValue27, 0);
                    }
                }

                wheel.Axle.localPosition = localPosition;
            }

            if(motorizedWheels > 1)
                rpm /= motorizedWheels;

            _motorRPM = MagicValue28 * _motorRPM + MagicValue29 * Mathf.Abs(rpm * 9.0f);

            if(_motorRPM > MagicValue30)
                _motorRPM = MagicValue31;

            _newTorque = _currentTorque * 9.0f;// _bikeConfiguration.Gears[_currentGear];

            foreach(var wheel in _wheels)
            {
                WheelCollider collider = wheel.Collider;

                if(wheel.IsEngineDriven)
                {
                    if(Mathf.Abs(collider.rpm) > Mathf.Abs(_wantedRPM))
                    {
                        collider.motorTorque = 0;
                    }
                    else
                    {
                        float currentTorque = collider.motorTorque;

                        if(!_isBrake && _accelerationT != 0 && _isNeutralGear == false)
                        {
                            if(_speed < _bikeConfiguration.LimitForwardSpeed)
                            {
                                collider.motorTorque = currentTorque * MagicValue32 + _newTorque;
                            }
                            else
                            {
                                collider.motorTorque = 0;
                                collider.brakeTorque = MagicValue33;
                            }
                        }
                        else
                        {
                            collider.motorTorque = 0;
                        }
                    }
                }

                float steerAngle = Mathf.Clamp(
                    value: _speed / _bikeConfiguration.MaxSteerAngle,
                    min: MagicValue34,
                    max: _bikeConfiguration.MaxSteerAngle);

                collider.steerAngle = _handlebarSteeringT * (wheel.MaxSteer / steerAngle);

                if(wheel == _wheels[0])
                {
                    collider.steerAngle = _handlebarSteeringT * _handlebarSteeringCurve.Evaluate(_speed);
                }
            }
        }

        private void OnGUI()
        {
            GUI.color = Color.black;
            GUILayout.Label($"_slip: {_slip}");
            GUILayout.Label($"_steer: {_steer}");
            GUILayout.Label($"Wheelie: {_wheelie}");
            GUILayout.Label($"_bodyInclinationZ: {_bikeInclinationZ}");
            GUILayout.Label($"_handlebarSteeringT: {_handlebarSteeringT}");
            GUILayout.Label($"motorRPM: {_motorRPM}");
            GUILayout.Label($"curTorque: {_currentTorque}");
            GUILayout.Label($"_newTorque: {_newTorque}");
            //GUILayout.Label($"currentGear: {_currentGear}");
            GUILayout.Label($"wantedRPM: {_wantedRPM}");
            GUILayout.Label($"_clampedAcceleration: {_accelerationT}");
            GUILayout.Label($"speed: {_speed}");
            GUILayout.Toggle(_isBackward, $"Backward");
        }

        private void OnDrawGizmos()
        {
            if(!_showNormalGizmos || Application.isPlaying)
                return;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            Gizmos.matrix = rotationMatrix;
            Gizmos.color = new Color(1, 0, 0, 0.5f);

            Gizmos.DrawCube(Vector3.up / 1.6f, new Vector3(0.5f, 1.0f, 2.5f));
            Gizmos.DrawSphere(_bikeConfiguration.ShiftCenter, 0.2f);
        }
    }
}
