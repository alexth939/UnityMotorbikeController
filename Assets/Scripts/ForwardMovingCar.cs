using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ForwardMovingCar : MonoBehaviour
{
    [SerializeField] private float _movingSpeed = 1.0f;
    [SerializeField] Vector3 _movingDirection;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(transform.rotation * _movingDirection.normalized * _movingSpeed, ForceMode.Acceleration);
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, 5.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation*_movingDirection.normalized);
    }
}
