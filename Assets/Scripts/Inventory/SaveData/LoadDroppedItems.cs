using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Disables world items that have already been collected and restores them to the inventory.
/// </summary>
public class LoadDroppedItems : MonoBehaviour
{
 
    [SerializeField] private GameObject _droppedItemsList;
    [SerializeField] private DataReadWrite _dataReadWrite;
    [SerializeField] private LimitedInventory _inventory;

    /// <summary>
    /// Ensures necessary component references are established.
    /// </summary>
    private void Awake()
    {
        if (_dataReadWrite == null)
            _dataReadWrite = GetComponent<DataReadWrite>();;
    }

    /// <summary>
    /// Initiates the data loading and inventory restoration process.
    /// </summary>
    private void Start()
    {
        if (_dataReadWrite == null || _inventory == null)
            return;

        _dataReadWrite.LoadData();
        LoadAndDisableDroppedItems(_dataReadWrite.ItemData);
    }

    /// <summary>
    /// Iterates a list of DroppedItem's in the world, restores them to the 
    /// inventory if their ID is found in the saved data, and then disables the world object.
    /// </summary>
    /// <param name="itemData">The Dictionary<string, int> containing the current ItemID and SlotIndex.</param>
    private void LoadAndDisableDroppedItems(Dictionary<string, int> itemData)
    {
        if (itemData == null || itemData.Count == 0)
            return;

        foreach (Transform childTransform in _droppedItemsList.transform)
        {
            GameObject droppedItemObject = childTransform.gameObject;

            if (droppedItemObject.TryGetComponent<DroppedItem>(out var droppedItemComponent))
            {
                if (droppedItemComponent.Item != null)
                {
                    string droppedItemID = droppedItemComponent.Item.ID;

                    if (itemData.ContainsKey(droppedItemID))
                    {
                        int targetSlotIndex = itemData[droppedItemID];
                        _inventory.AddItemAtSlot(droppedItemComponent.Item, targetSlotIndex);

                        droppedItemObject.SetActive(false);
                    }
                }
            }
        }
    }
}