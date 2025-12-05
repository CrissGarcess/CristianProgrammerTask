using UnityEngine;

/// <summary>
/// Manages global time control (Time.timeScale).
/// </summary>
public class TimeManager : MonoBehaviour
{
    [Range(0.01f, 1f)]
    public float slowMotionFactor = 0.1f;
    public float slowDownDuration = 0.5f;
    private const float NORMAL_TIME_SCALE = 1.0f;

    /// <summary>
    /// Instantly slows down the game time to the speed defined by slowMotionFactor.
    /// Adjusts the physics step frequency to keep physics consistent 
    /// </summary>
    public void StartSlowMotion()
    {
        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    /// <summary>
    /// Instantly restores the game time to normal speed.
    /// Restore the physics step frequency to its default value.
    /// </summary>
    public void StopSlowMotion()
    {
        Time.timeScale = NORMAL_TIME_SCALE;
        Time.fixedDeltaTime = 0.02f;
    }
}