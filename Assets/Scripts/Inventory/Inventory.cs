using System;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Manages the player's inventory system, handling item pickup, storage, and dropping logic.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Inventory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected InventoryUI _inventoryUI;

    [Header("Prefabs")]
    [SerializeField] protected GameObject _droppedItemPrefab;

    [Header("Inventory List")]
    [SerializeField] protected SerializedDictionary<string, Item> _inventory = new();

    /// <summary>
    /// Handles collision with dropped items in the scene.
    /// </summary>
    /// <param name="other">The collider entering the trigger zone.</param>
    protected void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("DroppedItem"))
            return;

        if (!other.TryGetComponent<DroppedItem>(out var droppedItem))
            return;

        if (droppedItem.PickedUp)
            return;

        if (AddItem(droppedItem.Item))
        {
            droppedItem.PickedUp = true;
            Destroy(other.gameObject);
        }
        else
        {
            droppedItem.PickedUp = false;
        }
    }

    /// <summary>
    /// Removes an item from the inventory and spawns its corresponding DroppedItem prefab
    /// in the game world.
    /// </summary>
    /// <param name="itemInventoryID">The Inventory ID of the item to drop.</param>
    public virtual void DropItem(string itemInventoryID)
    {
        if (string.IsNullOrEmpty(itemInventoryID))
            return;

        if (!_inventory.TryGetValue(itemInventoryID, out var item) || item == null)
            return;

        // spawn dropped item in world
        if (_droppedItemPrefab != null)
        {
            DroppedItem droppedItem = Instantiate(_droppedItemPrefab, transform.position, Quaternion.identity)
                .GetComponent<DroppedItem>();
            droppedItem?.Initialize(item);
        }

        _inventory.Remove(itemInventoryID);
    }

    /// <summary>
    /// Attempts to add a new item to the inventory. 
    /// </summary>
    /// <param name="item">The Item to be added.</param>
    /// <returns>True if the item was successfully added.</returns>
    protected virtual bool AddItem(Item item)
    {
        if (item == null) return false;

        string inventoryID = Guid.NewGuid().ToString();
        _inventory.Add(inventoryID, item);
        return true;
    }
}