using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper component to tag drag visual GameObjects created by ItemUI.
/// Tracks active instances so they can be robustly destroyed when needed.
/// </summary>
public sealed class DragVisualTag : MonoBehaviour
{
    public ItemUI Owner;

    private static readonly List<DragVisualTag> s_active = new List<DragVisualTag>();

    /// <summary>
    /// Registers this new drag visual tag with the static active list.
    /// </summary>
    private void Awake()
    {
        s_active.Add(this);
    }

    /// <summary>
    /// Unregisters this tag from the static active list to maintain an accurate record of active visuals.
    /// </summary>
    private void OnDestroy()
    {
        s_active.Remove(this);
    }

    /// <summary>
    /// Destroys all active drag visual GameObjects owned by the provided ItemUI.
    /// </summary>
    public static void DestroyForOwner(ItemUI owner)
    {
        if (owner == null) return;
        var copy = s_active.ToArray();
        foreach (var tag in copy)
        {
            if (tag == null) continue;
            try
            {
                if (tag.Owner == owner)
                {
                    if (tag.gameObject != null)
                        Destroy(tag.gameObject);
                }
            }
            catch { }
        }
    }
}
