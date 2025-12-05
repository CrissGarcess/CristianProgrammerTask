using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the visual representation and interaction logic of a single inventory slot.
/// It uses standard Unity UI events (Pointer Enter/Exit, Click, and Drag & Drop).
/// </summary>
public class ItemUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;

    [Header("Drag Settings")]
    [SerializeField] private float _dragAlpha = 0.6f;

    public static event Action<string, string> OnItemSlotSelected;

    public bool IsEmpty { get; private set; } = true;

    private string _inventoryID;
    private string _itemName;
    private string _itemDesc;
    private Item _equipableItem;
    private InventoryUI _inventoryUI;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private GameObject _dragVisual;

    private Action<string> _removeItemAction;
    public static event Action<Item> OnItemFocusChanged;

    /// <summary>
    /// Initialize essential component references needed for UI positioning and drag/drop functionality.
    /// </summary>
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// Cleanup operations related to drag and drop.
    /// </summary>
    private void OnDisable()
    {
        DestroyAllOwnerVisuals();

        _inventoryUI?.OnSlotEndDrag();
        _inventoryUI?.RefreshSlotByItemUI(this);
    }

    /// <summary>
    /// Ensures all cleanup operations are completed when the script is being destroyed.
    /// </summary>
    private void OnDestroy()
    {
        DestroyAllOwnerVisuals();
        if (_button != null)
            _button.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Registers the reference to the parent InventoryUI manager for drag/drop and refresh callbacks.
    /// </summary>
    public void RegisterInventoryUI(InventoryUI inventoryUI)
    {
        _inventoryUI = inventoryUI;
    }

    /// <summary>
    /// Initializes the slot with a specific item's data and assigns the removal callback.
    /// </summary>
    /// <param name="inventoryID">The ID of the item instance.</param>
    /// <param name="item">The Item data containing the icon and description.</param>
    /// <param name="removeItemAction">The action to invoke when the item needs to be removed.</param>
    public void Initialize(string inventoryID, Item item, Action<string> removeItemAction)
    {
        _inventoryID = inventoryID;
        _removeItemAction = removeItemAction;
        _equipableItem = item;

        if (_image != null)
        {
            _image.sprite = item?.Icon;
            _image.enabled = item?.Icon != null;
        }

        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => _removeItemAction?.Invoke(_inventoryID));
            _button.interactable = true;
        }

        _itemName = item?.ID;
        _itemDesc = item?.Description;
        IsEmpty = false;
    }

    /// <summary>
    /// Resets the slot to an empty state, clearing visual representations and internal data.
    /// </summary>
    public void SetEmpty()
    {
        IsEmpty = true;
        _inventoryID = null;
        _itemName = null;
        _itemDesc = null;
        _removeItemAction = null;

        if (_image != null)
        {
            _image.sprite = null;
            _image.enabled = false;
        }

        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
            _button.interactable = false;
        }

        OnItemSlotSelected?.Invoke(null, null);
    }

    /// <summary>
    /// Called when the pointer enters the bounds of the slot.
    /// Publishes the item name and description to show a tooltip if the slot is not empty.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsEmpty)
        {
            OnItemSlotSelected?.Invoke(_itemName, _itemDesc);
            OnItemFocusChanged?.Invoke(_equipableItem);
        }
            
    }

    /// <summary>
    /// Called when the pointer exits the bounds of the slot.
    /// Publishes null data to hide any active text.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        OnItemSlotSelected?.Invoke(null, null);
        OnItemFocusChanged?.Invoke(null);
    }

    /// <summary>
    /// Called when a drag operation is initiated on the slot.
    /// Creates a temporary visual proxy (DragVisual) for the dragged item image.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || IsEmpty)
            return;

        DestroyAllOwnerVisuals();

        _dragVisual = new GameObject("DragVisual");
        Transform parent = (_canvas != null) ? _canvas.transform : transform.root;
        _dragVisual.transform.SetParent(parent, false);
        _dragVisual.transform.SetAsLastSibling();

        Image dragImage = _dragVisual.AddComponent<Image>();
        dragImage.sprite = _image?.sprite;
        dragImage.raycastTarget = false;
        dragImage.preserveAspect = true;

        RectTransform dragRect = _dragVisual.GetComponent<RectTransform>();
        dragRect.sizeDelta = _rectTransform != null ? _rectTransform.sizeDelta : Vector2.zero;
        _dragVisual.transform.position = eventData.position;

        var tag = _dragVisual.AddComponent<DragVisualTag>();
        tag.Owner = this;

        if (_image != null)
        {
            Color tempColor = _image.color;
            tempColor.a = _dragAlpha;
            _image.color = tempColor;
        }

        _inventoryUI?.OnSlotBeginDrag(this);
        OnItemSlotSelected?.Invoke(null, null);
    }

    /// <summary>
    /// Called while the drag operation is active.
    /// Updates the position of the drag visual proxy to follow the mouse pointer.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (_dragVisual == null)
            return;
        _dragVisual.transform.position = eventData.position;
    }

    /// <summary>
    /// Called when the drag operation is completed.
    /// Destroys the drag visual proxy and restores the original slot's appearance.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        DestroyAllOwnerVisuals();

        _inventoryUI?.OnSlotEndDrag();
        _inventoryUI?.RefreshSlotByItemUI(this);
    }

    /// <summary>
    /// Called when an item is dropped onto this slot.
    /// Initiates the swap/move operation by notifying the InventoryUI manager.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        ItemUI draggedSlot = eventData.pointerDrag?.GetComponent<ItemUI>();
        if (draggedSlot != null && draggedSlot != this)
        {
            if (_inventoryUI == null)
                return;

            _inventoryUI.OnSlotDrop(this);
        }
    }

    /// <summary>
    /// Handles mouse clicks on the slot. 
    /// If the mouse buttons are clicked and the slot is not empty, it triggers the removal adn equip actions.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !IsEmpty)
            _removeItemAction?.Invoke(_inventoryID);
    }

    /// <summary>
    /// Destroy any persistent drag visuals associated with this specific ItemUI instance.
    /// </summary>
    private void DestroyAllOwnerVisuals()
    {
        DragVisualTag.DestroyForOwner(this);
        _dragVisual = null;
    }
}