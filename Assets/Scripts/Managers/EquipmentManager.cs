using UnityEngine;

/// <summary>
/// Manages the equipment system, focusing on handling item previews when items are focused or clicked.
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    
    [SerializeField] private Transform previewHolder;

    private Item _focusedItem;
    private GameObject currentPreview;

    /// <summary>
    /// Subscribes to the OnItemFocusChanged event to handle item focus changes.
    /// </summary>
    private void OnEnable()
    {
        ItemUI.OnItemFocusChanged += HandleFocusChanged;
    }

    /// <summary>
    /// Unsubscribes from the OnItemFocusChanged event to avoid memory leaks.
    /// </summary>
    private void OnDisable()
    {
        ItemUI.OnItemFocusChanged -= HandleFocusChanged;
    }

    /// <summary>
    /// Handles the event when the focused item changes.
    /// If the focused item has a prefab, it spawns the preview; otherwise, it clears any existing previews.
    /// </summary>
    /// <param name="item">The item that is currently focused.</param>
    private void HandleFocusChanged(Item item)
    {
        _focusedItem = item;

        if (_focusedItem != null && _focusedItem.Prefab != null)
        {
            SpawnItemPreview(_focusedItem);
        }
        else
        {
            ClearPreview();
        }
    }

    /// <summary>
    /// Instantiates the prefab of the given item in the preview holder.
    /// Clears any previous preview before spawning the new one.
    /// </summary>
    /// <param name="item">The item whose prefab is to be instantiated.</param>
    private void SpawnItemPreview(Item item)
    {
        ClearPreview();

        currentPreview = Instantiate(item.Prefab, previewHolder);

        currentPreview.transform.localPosition = Vector3.zero;
        currentPreview.transform.localRotation = Quaternion.identity;
        currentPreview.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Clears the current preview by destroying all child objects under the preview holder.
    /// </summary>
    private void ClearPreview()
    {
        if (currentPreview != null)
        {
            foreach (Transform child in previewHolder.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    /// Called when the item is clicked. It clears the current preview if it exists.
    /// TODO: Implement dequipping when an item is clicked.
    /// </summary>
    public void OnItemClick()
    {
        if (currentPreview != null)
        {
            ClearPreview();
        }
    }
}
