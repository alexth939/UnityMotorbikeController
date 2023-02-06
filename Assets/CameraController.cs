using UnityEngine;

/// <summary>
///     For tests.
/// </summary>
public class CameraController: MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private CameraState[] _cameraStates;

    private int _activeStateIndex;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
            ActivateNextCameraState();

        if(Input.GetKeyDown(KeyCode.KeypadMinus))
            ActivatePreviousCameraState();
    }

    private void LateUpdate()
    {
        _camera.transform.position = _cameraStates[_activeStateIndex].PositionTarget.position;
        _camera.transform.LookAt(_cameraStates[_activeStateIndex].LookingTarget.position);
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
