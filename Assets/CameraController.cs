using UnityEngine;

/// <summary>
///     For tests.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private CameraState[] _cameraStates;
    [SerializeField] private Vector3 _positionOffset;

    private int _activeStateIndex;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
            ActivateNextCameraState();

        if(Input.GetKeyDown(KeyCode.KeypadMinus))
            ActivatePreviousCameraState();
    }

    private float _cameraEulerXOffset;
    [SerializeField] private float _minXRotationOffset;
    [SerializeField] private float _maxXRotationOffset;
    [SerializeField] private float _xRotationSpeed = 1.0f;

    private void LateUpdate()
    {
        float mouseHorizontalDelta = Input.GetAxis("Mouse Y");
        mouseHorizontalDelta *= Time.deltaTime * _xRotationSpeed;
        _cameraEulerXOffset += mouseHorizontalDelta;
        _cameraEulerXOffset = Mathf.Clamp(_cameraEulerXOffset, _minXRotationOffset, _maxXRotationOffset);
        //var mousePositionDelta = new Vector2()
        //{
        //    x = -Input.GetAxis("Mouse Y"),
        //    y = Input.GetAxis("Mouse X"),
        //};

        //_cameraStates[_activeStateIndex].LookingTarget.position =
        //    _cameraStates[_activeStateIndex].PositionTarget.position +
        //    Quaternion.Euler(mousePositionDelta) *
        //    (_cameraStates[_activeStateIndex].LookingTarget.position -
        //    _cameraStates[_activeStateIndex].PositionTarget.position);

        _camera.transform.position = _cameraStates[_activeStateIndex].PositionTarget.position +
            _cameraStates[_activeStateIndex].PositionTarget.rotation * _positionOffset;

        _camera.transform.LookAt(_cameraStates[_activeStateIndex].LookingTarget.position);

        _camera.transform.Rotate(-_cameraEulerXOffset, 0, 0, Space.Self);
    }

    private void OnGUI()
    {
        GUI.color = Color.black;
        GUILayout.BeginHorizontal();
        GUILayout.Space(100);
        GUILayout.Label($"_activeStateIndex: {_activeStateIndex}");
        GUILayout.EndHorizontal();
    }

    private void ActivateNextCameraState()
    {
        _activeStateIndex = (int)Mathf.Repeat(++_activeStateIndex, _cameraStates.Length);
    }

    private void ActivatePreviousCameraState()
    {
        _activeStateIndex = (int)Mathf.Repeat(--_activeStateIndex, _cameraStates.Length);
    }
}

[System.Serializable]
public class CameraState
{
    [field: SerializeField] public Transform PositionTarget { get; private set; }
    [field: SerializeField] public Transform LookingTarget { get; private set; }
}
