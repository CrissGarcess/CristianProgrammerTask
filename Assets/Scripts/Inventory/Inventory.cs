using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Manages the player's inventory system, handling item pickup, storage, and dropping logic.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Inventory : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject _droppedItemPrefab;

    [Header("Inventory List")]
    [SerializeField] private SerializedDictionary<string, Item> _inventory = new();

    /// <summary>
    /// Handles collision with dropped items in the scene.
    /// </summary>
    /// <param name="other">The collider entering the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DroppedItem"))
        {
            if (other.TryGetComponent<DroppedItem>(out var droppedItem))
            {
                if (droppedItem.PickedUp)
                    return;

                droppedItem.PickedUp = true;
                AddItem(droppedItem.Item);
                Destroy(other.gameObject);
            }
        }
    }

    /// <summary>
    /// Removes an item from the inventory and spawns its physical representation in the world.
    /// </summary>
    /// <param name="itemInventoryID">The ID of the item in the inventory dictionary.</param>
    public void DropItem(string itemInventoryID)
    {
        DroppedItem droppedItem = Instantiate(_droppedItemPrefab, transform.position, 
            Quaternion.identity).GetComponent<DroppedItem>();
        Item item = _inventory.GetValueOrDefault(itemInventoryID);
        droppedItem.Initialize(item);
        _inventory.Remove(itemInventoryID);
    }

    /// <summary>
    /// Adds a new item to the internal dictionary.
    /// Generates a unique GUID to allow multiple instances of the same item type.
    /// </summary>
    /// <param name="item">The item to add.</param>
    private void AddItem(Item item)
    {
        string inventoryID = Guid.NewGuid().ToString();
        _inventory.Add(inventoryID, item);
    }

}