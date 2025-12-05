using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's inventory with slot limitation.
/// </summary>
public class LimitedInventory : Inventory
{
    private const int MAX_CAPACITY = 8;

    [Header("Slots")]
    [SerializeField] protected List<string> _inventorySlots = new();

    private void Start()
    {
        InitializeInventory(MAX_CAPACITY);
    }

    /// <summary>
    /// Attempts to add a new item to the inventory. 
    /// </summary>
    /// <param name="item">The Item to be added.</param>
    /// <returns>True if the item was successfully added; otherwise, false if the inventory is full.</returns>
    protected override bool AddItem(Item item)
    {
        if (_inventory.Count >= MAX_CAPACITY)
            return false;

        int emptySlotIndex = _inventorySlots.FindIndex(slot => string.IsNullOrEmpty(slot));

        if (emptySlotIndex == -1)
            return false;

        string inventoryID = Guid.NewGuid().ToString();

        _inventory.Add(inventoryID, item);

        _inventorySlots[emptySlotIndex] = inventoryID;

        if (_inventoryUI != null)
        {
            _inventoryUI.AddUIItem(emptySlotIndex, inventoryID, item);
        }

        return true;
    }

    /// <summary>
    /// Removes an item from the inventory and spawns its corresponding DroppedItem prefab
    /// in the game world.
    /// </summary>
    /// <param name="itemInventoryID">The Inventory ID of the item to drop.</param>
    public override void DropItem(string itemInventoryID)
    {
        int slotIndex = _inventorySlots.FindIndex(id => id == itemInventoryID);
        if (slotIndex == -1) return;

        Item item = _inventory.GetValueOrDefault(itemInventoryID);
        if (item == null) return;

        DroppedItem droppedItem = Instantiate(_droppedItemPrefab, transform.position, Quaternion.identity)
            .GetComponent<DroppedItem>();
        droppedItem.Initialize(item);

        _inventory.Remove(itemInventoryID);
        _inventorySlots[slotIndex] = null;
        _inventoryUI?.RemoveUIItem(slotIndex);
    }

    /// <summary>
    /// Initializes or resizes the inventory slots list to a specific size.
    /// </summary>
    /// <param name="slotCount">The desired number of inventory slots.</param>
    public void InitializeInventory(int slotCount)
    {
        if (slotCount <= 0)
            return;

        if (_inventorySlots != null && _inventorySlots.Count == slotCount)
            return;

        if (_inventorySlots != null && _inventorySlots.Count > 0)
        {
            var newSlots = new List<string>(new string[slotCount]);
            int copyCount = Math.Min(_inventorySlots.Count, slotCount);
            for (int i = 0; i < copyCount; i++)
                newSlots[i] = _inventorySlots[i];
            _inventorySlots = newSlots;
            return;
        }

        _inventorySlots = new List<string>(new string[slotCount]);
    }

    /// <summary>
    /// Retrieves the Item object stored in a specific inventory slot.
    /// </summary>
    /// <param name="slotIndex">The index of the slot to check.</param>
    /// <returns>
    /// The Item object if the slot is occupied; 
    /// null if the slot is empty, the index is invalid, or the item data is missing.
    /// </returns>
    public Item GetItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count)
            return null;

        string inventoryID = _inventorySlots[slotIndex];
        if (string.IsNullOrEmpty(inventoryID))
            return null;

        return _inventory.GetValueOrDefault(inventoryID);
    }

    /// <summary>
    /// Retrieves the unique Inventory ID string stored in a specific slot.
    /// </summary>
    /// <param name="slotIndex">The index of the slot to check.</param>
    /// <returns>
    /// The Inventory ID string if the slot is occupied; 
    /// null if the slot is empty or the index is invalid.
    /// </returns>
    public string GetInventoryID(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySlots.Count)
            return null;

        return _inventorySlots[slotIndex];
    }

    /// <summary>
    /// Swaps the item IDs between two inventory slots.
    /// </summary>
    /// <param name="sourceIndex">The index of the item being moved.</param>
    /// <param name="targetIndex">The index of the slot receiving the item (can be empty or occupied).</param>
    /// <returns>True if the swap was successfully executed; false if the indices are invalid or the source slot is empty.</returns>
    public bool SwapItems(int sourceIndex, int targetIndex)
    {
        if (sourceIndex < 0 || sourceIndex >= _inventorySlots.Count) return false;
        if (targetIndex < 0 || targetIndex >= _inventorySlots.Count) return false;
        if (sourceIndex == targetIndex) return false;

        string sourceID = _inventorySlots[sourceIndex];
        if (string.IsNullOrEmpty(sourceID)) return false;

        string targetID = _inventorySlots[targetIndex];

        _inventorySlots[sourceIndex] = targetID;
        _inventorySlots[targetIndex] = sourceID;

        _inventoryUI?.SwapUIItems(sourceIndex, targetIndex);
        return true;
    }
}