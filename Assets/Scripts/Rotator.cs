using UnityEngine;

/// <summary>
/// Component used to make the attached GameObject rotate continuously around its center point.
/// </summary>
public class Rotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 50f, 0f);
    [SerializeField] private bool useWorldSpace = false;

    /// <summary>
    /// Execute the rotation logic every frame.
    /// </summary>
    void Update()
    {
        PerformRotation();
    }

    /// <summary>
    /// Calculates the required rotation for the current frame and applies it 
    /// based on the rotation space (World or Local).
    /// </summary>
    public void PerformRotation()
    {
        Vector3 rotationThisFrame = rotationSpeed * Time.deltaTime;

        if (useWorldSpace)
        {
            transform.Rotate(rotationThisFrame, Space.World);
        }
        else
        {
            transform.Rotate(rotationThisFrame, Space.Self);
        }
    }
}