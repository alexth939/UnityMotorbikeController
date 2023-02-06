using UnityEngine;

public class RearLookAt: MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _position;

    private void Update()
    {
        Vector3 distance = transform.localPosition - _target.localPosition;
        Quaternion rotation = Quaternion.LookRotation(distance + _position);
        transform.localRotation = rotation;
    }
}
