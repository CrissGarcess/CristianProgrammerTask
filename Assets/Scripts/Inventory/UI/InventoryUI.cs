using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// Central manager for the visual representation and user interaction logic of the inventory UI panel.
/// It handles synchronizing visual slots (ItemUI) with the authoritative Inventory system,
/// and resolving drag-and-drop operations.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected LimitedInventory _inventory;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _itemDesc;
    [SerializeField] private ItemUI[] _itemSlots = new ItemUI[8];

    [Header("State")]
    [SerializeField] protected SerializedDictionary<string, int> _inventorySlotMap = new();

    private ItemUI _draggedSlot;

    /// <summary>
    /// Initializes the UI components and ensures synchronization with the core inventory.
    /// </summary>
    private void Start()
    {
        // initialize slots and register this UI with each slot
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            if (_itemSlots[i] == null)
                continue;

            _itemSlots[i].SetEmpty();
            _itemSlots[i].RegisterInventoryUI(this);
        }

        _inventory?.InitializeInventory(_itemSlots.Length);

        RefreshAllSlots();
    }

    /// <summary>
    /// Subscribes to the static event for tooltip updates and forces a refresh of all slots.
    /// </summary>
    private void OnEnable()
    {
        ItemUI.OnItemSlotSelected += UpdateItemDetailsText;

        RefreshAllSlots();
    }

    /// <summary>
    /// Unsubscribes from the static event.
    /// </summary>
    private void OnDisable()
    {
        ItemUI.OnItemSlotSelected -= UpdateItemDetailsText;
    }

    /// <summary>
    /// Updates the Item Name and Description text fields based on the data received from 
    /// the ItemUI slot.
    /// </summary>
    /// <param name="itemName">The name of the item to display, or null to clear.</param>
    /// <param name="itemDesc">The description of the item to display, or null to clear.</param>
    private void UpdateItemDetailsText(string itemName, string itemDesc)
    {
        _itemNameText.text = itemName ?? string.Empty;
        _itemDesc.text = itemDesc ?? string.Empty;
    }

    /// <summary>
    /// Adds or updates a visual item representation in a specific slot.
    /// <param name="slotIndex">The index of the slot to update.</param>
    /// <param name="inventoryID">The ID of the item instance.</param>
    /// <param name="item">The Item data.</param>
    public virtual void AddUIItem(int slotIndex, string inventoryID, Item item)
    {
        if (slotIndex < 0 || slotIndex >= _itemSlots.Length || string.IsNullOrEmpty(inventoryID) || item == null)
            return;

        var slot = _itemSlots[slotIndex];
        if (slot == null)
            return;

        slot.gameObject.SetActive(true);

        slot.Initialize(inventoryID, item, _inventory.DropItem);
        _inventorySlotMap[inventoryID] = slotIndex;

        Canvas.ForceUpdateCanvases();
        var root = GetComponent<RectTransform>();
        if (root != null)
            LayoutRebuilder.MarkLayoutForRebuild(root);
    }

    /// <summary>
    /// Removes an item's visual representation from a specific slot, setting it to empty.
    /// </summary>
    /// /// <param name="slotIndex">The index of the slot to clear.</param>
    public virtual void RemoveUIItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _itemSlots.Length)
            return;

        string idToRemove = null;
        foreach (var kvp in _inventorySlotMap)
        {
            if (kvp.Value == slotIndex)
            {
                idToRemove = kvp.Key;
                break;
            }
        }

        if (idToRemove != null)
            _inventorySlotMap.Remove(idToRemove);

        var slot = _itemSlots[slotIndex];
        if (slot != null)
        {
            slot.SetEmpty();

            Canvas.ForceUpdateCanvases();
            var root = GetComponent<RectTransform>();
            if (root != null)
                LayoutRebuilder.MarkLayoutForRebuild(root);
        }
    }

    /// <summary>
    /// Synchronizes the visuals of two slots after a successful item swap has occurred in the Inventory.
    /// </summary>
    /// <param name="sourceIndex">The index of the source slot.</param>
    /// <param name="targetIndex">The index of the target slot.</param>
    public virtual void SwapUIItems(int sourceIndex, int targetIndex)
    {
        if (sourceIndex < 0 || sourceIndex >= _itemSlots.Length ||
            targetIndex < 0 || targetIndex >= _itemSlots.Length)
            return;

        string sourceID = _inventory.GetInventoryID(sourceIndex);
        string targetID = _inventory.GetInventoryID(targetIndex);

        Item sourceItem = _inventory.GetItem(sourceIndex);
        Item targetItem = _inventory.GetItem(targetIndex);

        if (sourceItem != null)
            _itemSlots[targetIndex].Initialize(sourceID, sourceItem, _inventory.DropItem);
        else
            _itemSlots[targetIndex].SetEmpty();

        if (targetItem != null)
            _itemSlots[sourceIndex].Initialize(targetID, targetItem, _inventory.DropItem);
        else
            _itemSlots[sourceIndex].SetEmpty();

        // Rebuild internal map to reflect the ID changes.
        _inventorySlotMap.Clear();
        for (int i = 0; i < _itemSlots.Length; i++)
        {
            string id = _inventory.GetInventoryID(i);
            if (!string.IsNullOrEmpty(id))
                _inventorySlotMap[id] = i;
        }

        Canvas.ForceUpdateCanvases();
        var root = GetComponent<RectTransform>();
        if (root != null)
            LayoutRebuilder.MarkLayoutForRebuild(root);
    }

    /// <summary>
    /// Forces a visual re-synchronization of a single slot.
    /// </summary>
    /// <param name="index">The index of the slot to refresh.</param>
    public void RefreshSlotByIndex(int index)
    {
        if (_inventory == null) return;
        if (index < 0 || index >= _itemSlots.Length) return;

        string id = _inventory.GetInventoryID(index);
        Item item = _inventory.GetItem(index);

        var slot = _itemSlots[index];
        if (slot == null) return;

        if (string.IsNullOrEmpty(id) || item == null)
        {
            slot.SetEmpty();

            // Clean up stale map entry that might point to this index.
            string stale = null;
            foreach (var kvp in _inventorySlotMap)
            {
                if (kvp.Value == index)
                {
                    stale = kvp.Key;
                    break;
                }
            }
            if (stale != null)
                _inventorySlotMap.Remove(stale);
        }
        // If the slot is occupied, initialize the UI slot with the item data and update the map.
        else
        {
            slot.Initialize(id, item, _inventory.DropItem);
            _inventorySlotMap[id] = index;
        }

        Canvas.ForceUpdateCanvases();
        var root = GetComponent<RectTransform>();
        if (root != null)
            LayoutRebuilder.MarkLayoutForRebuild(root);
    }

    /// <summary>
    /// Locates the index of the given ItemUI slot and calls RefreshSlotByIndex.
    /// </summary>
    /// <param name="slot">The ItemUI component to refresh.</param>
    public void RefreshSlotByItemUI(ItemUI slot)
    {
        int index = System.Array.IndexOf(_itemSlots, slot);
        if (index >= 0)
            RefreshSlotByIndex(index);
    }

    /// <summary>
    /// Stores the source slot for later drop resolution.
    /// </summary>
    /// <param name="slot">The ItemUI that initiated the drag.</param>
    public void OnSlotBeginDrag(ItemUI slot)
    {
        _draggedSlot = slot;
    }

    /// <summary>
    /// Central drag-and-drop resolution method.
    /// </summary>
    /// <param name="targetSlot">The ItemUI slot where the item was dropped.</param>
    public void OnSlotDrop(ItemUI targetSlot)
    {
        if (_draggedSlot == null || _draggedSlot == targetSlot)
        {
            _draggedSlot = null;
            return;
        }

        int sourceIndex = System.Array.IndexOf(_itemSlots, _draggedSlot);
        int targetIndex = System.Array.IndexOf(_itemSlots, targetSlot);

        if (sourceIndex == -1 || targetIndex == -1)
        {
            _draggedSlot = null;
            return;
        }

        bool success = _inventory.SwapItems(sourceIndex, targetIndex);

        if (success)
        {
            RefreshSlotByIndex(sourceIndex);
            RefreshSlotByIndex(targetIndex);
        }

        _draggedSlot = null;
    }

    /// <summary>
    /// Clears the stored source slot reference.
    /// </summary>
    public void OnSlotEndDrag()
    {
        _draggedSlot = null;
    }

    /// <summary>
    /// Forces a re-synchronization of all UI slots.
    /// </summary>
    public void RefreshAllSlots()
    {
        if (_inventory == null) return;
        for (int i = 0; i < _itemSlots.Length; i++)
            RefreshSlotByIndex(i);
    }
}