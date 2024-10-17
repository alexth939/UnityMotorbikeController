using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Awsome Tools")]
public sealed class AwsomeToolsToolbar : ToolbarOverlay
{
    public const string FamilyId = "Awsome Tools";

    private static readonly string[] ToolsIds;

    static AwsomeToolsToolbar()
    {
        var toolsIdsCollection = GatherFamilyTools();
        ToolsIds = toolsIdsCollection.ToArray();
    }

    private AwsomeToolsToolbar() : base(ToolsIds)
    {
    }

    private static IEnumerable<string> GatherFamilyTools()
    {
        TypeCache.TypeCollection loadedEditorToolbarElements =
            TypeCache.GetTypesWithAttribute<EditorToolbarElementAttribute>();

        const bool searchInheritanceChainRequired = false;

        IEnumerable<string> familyToolsIds;

        familyToolsIds = from type in loadedEditorToolbarElements
                         select GetCustomAttribute<EditorToolbarElementAttribute>(type, searchInheritanceChainRequired)
                         into suspectAttribute
                         where suspectAttribute.id.StartsWith(FamilyId)
                         let desiredAttribute = suspectAttribute
                         select desiredAttribute.id;

        return familyToolsIds;
    }

    private static T GetCustomAttribute<T>(MemberInfo element, bool inherit) where T : Attribute
    {
        return (T)Attribute.GetCustomAttribute(element, typeof(T), inherit);
    }
}

[EditorToolbarElement(Id, typeof(SceneView))]
internal class SceneCamerSnappingTool : EditorToolbarToggle
{
    public const string Id = AwsomeToolsToolbar.FamilyId + "/SceneCamerSnappingTool";

    private Vector3 _cameraLookDirectionInTargetSpace;
    private Button _cameraRotationLockButton;
    private Transform _followingTarget;
    private bool _isCamerRotationLocked = false;
    private Vector3 _rawCameraPositionOffset = Vector3.zero;

    public SceneCamerSnappingTool() : base((Texture2D)EditorGUIUtility.IconContent("BuildSettings.Lumin").image)
    {
        _cameraRotationLockButton = new Button(ToggleCameraRotationLock) { text = "ROT" };
        _cameraRotationLockButton.style.backgroundColor = _isCamerRotationLocked ? Color.red : Color.green;
        Add(_cameraRotationLockButton);

        (EditorApplication.isPlaying ? (Action)EnableAndReset : DisableAndReset).Invoke();

        EditorApplication.playModeStateChanged += DisableOnExitingPlayMode;
        EditorApplication.playModeStateChanged += EnableOnEnteredPlayMode;
    }

    ~SceneCamerSnappingTool()
    {
        EditorApplication.playModeStateChanged -= DisableOnExitingPlayMode;
        EditorApplication.playModeStateChanged -= EnableOnEnteredPlayMode;
    }

    private enum MouseButtons
    {
        Left = 0,
        Right = 1,
        Middle = 2,
    }

    // Use the setter as a callback from the user.
    // Don't forget to bind it to base's value property.
    public override bool value
    {
        get => base.value;
        set => (value ? (Action)TryActivate : Deactivate).Invoke();
    }

    private SceneView SceneViewWindow => EditorWindow.GetWindow<SceneView>();

    private Quaternion CalculateDesiredCameraRotation()
    {
        Quaternion forwardOfObject = Quaternion.LookRotation(_followingTarget.forward);
        Quaternion cameraRotationInTargetSpace = Quaternion.LookRotation(_cameraLookDirectionInTargetSpace);
        Quaternion desiredRotation = forwardOfObject * cameraRotationInTargetSpace;

        desiredRotation = DiscardZComponent(desiredRotation);

        return desiredRotation;
    }

    private void Deactivate()
    {
        UnsnapCamera();
        base.value = false;
        Selection.selectionChanged -= ResnapCameraToSelectedObject;
    }

    private void DisableAndReset()
    {
        SetEnabled(false);
        value = false;
    }

    private void DisableOnExitingPlayMode(PlayModeStateChange actualState)
    {
        if(actualState == PlayModeStateChange.ExitingPlayMode)
            DisableAndReset();
    }

    private void DiscardKeyboardEvent()
    {
        if(Event.current.isKey)
            Event.current.Use();
    }

    /// <summary>
    ///     Sets the z component of the rotation's Euler representation to 0.
    /// </summary>
    private Quaternion DiscardZComponent(Quaternion rotation)
    {
        Vector3 cameraRotationAsEuler = rotation.eulerAngles;
        cameraRotationAsEuler.z = 0;

        return Quaternion.Euler(cameraRotationAsEuler);
    }

    private void EnableAndReset()
    {
        SetEnabled(true);
        value = false;
    }

    private void EnableOnEnteredPlayMode(PlayModeStateChange actualState)
    {
        if(actualState == PlayModeStateChange.EnteredPlayMode)
            EnableAndReset();
    }

    private async void InvokeSnappingScenario()
    {
        _rawCameraPositionOffset = Vector3.zero;
        SceneViewWindow.LookAt(_followingTarget.position);
        SetEnabled(false);

        await Task.Delay(200);

        if(EditorApplication.isPlaying)
        {
            SetEnabled(true);
            SceneView.duringSceneGui += OnUpdateSceneView;
        }
    }

    private void LockCameraRotation()
    {
        if(value == false)
            return;

        _isCamerRotationLocked = true;
        _cameraRotationLockButton.style.backgroundColor = Color.red;

        //Vector3 worldCameraToObjectDirection = _followingTarget.position - SceneViewWindow.camera.transform.position;
        Vector3 lookTarget = _followingTarget.position + SceneViewWindow.rotation * _rawCameraPositionOffset;
        Vector3 worldCameraToObjectDirection = lookTarget - SceneViewWindow.camera.transform.position;
        _cameraLookDirectionInTargetSpace = _followingTarget.InverseTransformDirection(worldCameraToObjectDirection);
        SceneViewWindow.size = worldCameraToObjectDirection.magnitude / 2;
    }

    private void OnUpdateSceneView(SceneView view)
    {
        DiscardKeyboardEvent();
        RegisterPivotOffsetChanges();

        Vector3 lookTarget = _followingTarget.position + view.rotation * _rawCameraPositionOffset;

        view.pivot = lookTarget;

        if(_isCamerRotationLocked)
        {
            Quaternion cameraRotation = CalculateDesiredCameraRotation();
            view.LookAtDirect(lookTarget, cameraRotation);
        }
    }

    private void RegisterPivotOffsetChanges()
    {
        if(Event.current.isMouse && Event.current.button == (int)MouseButtons.Middle)
        {
            Vector3 mouseDeltaAsWorldCoords = new()
            {
                x = -Event.current.delta.x,
                y = Event.current.delta.y,
            };

            _rawCameraPositionOffset += mouseDeltaAsWorldCoords * 0.01f;
            Event.current.Use();
        }
    }

    private void ResnapCameraToSelectedObject()
    {
        UnsnapCamera();

        if(Application.isPlaying == false)
        {
            Debug.Log($"Enter play mode!");
            Deactivate();
            return;
        }

        if(Selection.gameObjects.Length != 1)
        {
            Debug.Log($"Select exactly one game object!");
            Deactivate();
            return;
        }

        _followingTarget = Selection.gameObjects[0].transform;

        InvokeSnappingScenario();
    }

    private void ToggleCameraRotationLock()
    {
        if(value == false)
            return;

        (_isCamerRotationLocked ? (Action)UnlockCameraRotation : LockCameraRotation).Invoke();
    }

    private void TryActivate()
    {
        bool isActivatedSuccessfully = TrySnapCameraToSelectedObject();
        base.value = isActivatedSuccessfully;

        if(isActivatedSuccessfully)
            Selection.selectionChanged += ResnapCameraToSelectedObject;
    }

    private bool TrySnapCameraToSelectedObject()
    {
        if(Application.isPlaying == false)
        {
            Debug.Log($"Select exactly one game object!");
            return false;
        }

        if(Selection.gameObjects.Length != 1)
        {
            Debug.Log($"Select exactly one game object!");
            return false;
        }

        _followingTarget = Selection.gameObjects[0].transform;

        InvokeSnappingScenario();

        return true;
    }

    private void UnlockCameraRotation()
    {
        _isCamerRotationLocked = false;
        _cameraRotationLockButton.style.backgroundColor = Color.green;
    }

    private void UnsnapCamera()
    {
        SceneView.duringSceneGui -= OnUpdateSceneView;
        UnlockCameraRotation();
    }
}
