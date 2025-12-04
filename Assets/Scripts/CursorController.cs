using UnityEngine;

/// <summary>
/// Manages cursor state.
/// </summary>
public class CursorController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool lockOnStart = true;

    /// <summary>
    /// Initializes the cursor state based on the configuration settings.
    /// </summary>
    void Start()
    {
        if (lockOnStart)
        {
            SetCursorState(true);
        }
    }

    /// <summary>
    /// Sets the cursor lock state and visibility.
    /// </summary>
    /// <param name="isLocked">If true, cursor is locked to center and hidden. 
    /// If false, it is free and visible.</param>
    private void SetCursorState(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}