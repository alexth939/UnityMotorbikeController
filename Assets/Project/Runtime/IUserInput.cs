using System;
using UnityEngine;

public interface IInputRebind
{
    //todo: do it with unitasks. https://github.com/Cysharp/UniTask
    //System.Threading.Tasks.Task ListenForNewInput();
}

public interface IKeyInputAccessor
{
    event Action Pressed;

    /// <summary>
    ///     Gives u the <b>UnityEngine.Time.time</b> to make ur life easier.
    /// </summary>
    event Action<float> PressedWithScaledTimestamp;

    /// <summary>
    ///     Gives u the <b>UnityEngine.Time.unscaledTime</b> to make ur life easier.
    /// </summary>
    event Action<float> PressedWithUnscaledTimestamp;

    event Action Released;

    /// <summary>
    ///     Gives u the <b>UnityEngine.Time.time</b> to make ur life easier.
    /// </summary>
    event Action<float> ReleasedWithScaledTimestamp;

    /// <summary>
    ///     Gives u the <b>UnityEngine.Time.unscaledTime</b> to make ur life easier.
    /// </summary>
    event Action<float> ReleasedWithUnscaledTimestamp;

    bool IsDown { get; }

    /// <summary>
    ///     This is the time in seconds since the start of the application,<br/>
    ///     which <b>Time.timeScale</b> scales and <b>Time.maximumDeltaTime</b> adjusts.<br/>
    ///     Do compre it with <b>UnityEngine.Time.time</b>.
    /// </summary>
    float? LastScaledTimePressed { get; }

    /// <summary>
    ///     This is the time in seconds since the start of the application,<br/>
    ///     which <b>Time.timeScale</b> scales and <b>Time.maximumDeltaTime</b> adjusts.<br/>
    ///     Do compre it with <b>UnityEngine.Time.time</b>.
    /// </summary>
    float? LastScaledTimeReleased { get; }

    /// <summary>
    ///     Unlike <b>UnityEngine.Time.time</b> this value is not affected by
    ///     <b>Time.timeScale</b> and <b>Time.maximumDeltaTime</b>.<br/>
    ///     Do compare it with <b>UnityEngine.Time.unscaledTime</b>.
    /// </summary>
    float? LastUnscaledTimePressed { get; }

    /// <summary>
    ///     Unlike <b>UnityEngine.Time.time</b> this value is not affected by
    ///     <b>Time.timeScale</b> and <b>Time.maximumDeltaTime</b>.<br/>
    ///     Do compare it with <b>UnityEngine.Time.unscaledTime</b>.
    /// </summary>
    float? LastUnscaledTimeReleased { get; }

    /// <summary>
    ///     This is the time in seconds since the start of the application,<br/>
    ///     which <b>Time.timeScale</b> scales and <b>Time.maximumDeltaTime</b> adjusts.<br/>
    /// </summary>
    /// <returns>
    ///     Time since button was pressed last time.
    /// </returns>
    float? ScaledTimeSincePressed { get; }

    /// <summary>
    ///     This is the time in seconds since the start of the application,<br/>
    ///     which <b>Time.timeScale</b> scales and <b>Time.maximumDeltaTime</b> adjusts.<br/>
    /// </summary>
    /// // <returns>
    ///     Time since button was released last time.
    /// </returns>
    float? ScaledTimeSinceReleased { get; }

    /// <summary>
    ///     Unlike <b>UnityEngine.Time.time</b> this value is not affected by
    ///     <b>Time.timeScale</b> and <b>Time.maximumDeltaTime</b>.<br/>
    /// </summary>
    /// <returns>
    ///     Time since button was pressed last time.
    /// </returns>
    float? UnscaledTimeSincePressed { get; }

    /// <summary>
    ///     This is the time in seconds since the start of the application,<br/>
    ///     which <b>Time.timeScale</b> scales and <b>Time.maximumDeltaTime</b> adjusts.<br/>
    /// </summary>
    /// // <returns>
    ///     Time since button was released last time.
    /// </returns>
    float? UnscaledTimeSinceReleased { get; }
}

public interface ITwoDimensionInputAccessor
{
    /// <summary>
    ///     Coordinates range from (-1, -1) at the bottom-left corner
    ///     to (1, 1) at the top-right corner within the square area.
    /// </summary>
    event Action<Vector2> Changed;

    /// <summary>
    ///     Coordinates are converted to a unit vector, keeping the direction but with a length of one.
    /// </summary>
    event Action<Vector2> ChangedNormalized;

    /// <summary>
    ///     Coordinates have been reset to zero (0, 0).
    /// </summary>
    event Action ChangedToZero;
}

public interface IUserInput
{
    IKeyInputAccessor JumpButton { get; }
    ITwoDimensionInputAccessor MovementDirection { get; }
    IKeyInputAccessor PauseButton { get; }
}

public class KeyInputPresenter : IKeyInputAccessor, IInputRebind
{
    private float? _lastScaledTimePressed;

    public event Action Pressed;

    public event Action<float> PressedWithScaledTimestamp
    {
        add => throw default;
        remove => throw default;
    }

    public event Action<float> PressedWithUnscaledTimestamp;

    public event Action Released;

    public event Action<float> ReleasedWithScaledTimestamp;

    public event Action<float> ReleasedWithUnscaledTimestamp;

    public bool IsDown => throw new NotImplementedException();

    public float? LastScaledTimePressed => _lastScaledTimePressed;

    public float? LastScaledTimeReleased => throw new NotImplementedException();

    public float? LastUnscaledTimePressed => throw new NotImplementedException();

    public float? LastUnscaledTimeReleased => throw new NotImplementedException();

    public float? ScaledTimeSincePressed => throw new NotImplementedException();

    public float? ScaledTimeSinceReleased => throw new NotImplementedException();

    public float? UnscaledTimeSincePressed => throw new NotImplementedException();

    public float? UnscaledTimeSinceReleased => throw new NotImplementedException();
}

public class UserInputService : IUserInput
{
    public IKeyInputAccessor JumpButton => throw new NotImplementedException();
    public ITwoDimensionInputAccessor MovementDirection => throw default;
    public IKeyInputAccessor PauseButton => throw new NotImplementedException();
}
