using UnityEngine;

/// <summary>
/// Manages the opening and closing the Inventory UI in response to player input.
/// Handles the synchronization of game state elements like the cursor visibility 
/// and time scale when the inventory is toggled.
/// </summary>
public class InventoryController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private PlayerInputReader _playerInputReader;
    [SerializeField] private CursorController _cursosrController;
    [SerializeField] private TimeManager _timeManager;

    private bool isInventoryOpen = false;

    /// <summary>
    /// Initializes the UI state to closed and ensures the UI panel is hidden.
    /// </summary>
    void Start()
    {
        inventoryUI.SetActive(false);
        isInventoryOpen = false;
    }

    /// <summary>
    /// Subscribes to the inventory input event from the PlayerInputReader.
    /// </summary>
    void OnEnable()
    {
        _playerInputReader.OnInventoryPerformed += OnInventoryPerformed;
    }

    /// <summary>
    /// Unsubscribes from the inventory input event.
    /// </summary>
    void OnDisable()
    {
        _playerInputReader.OnInventoryPerformed -= OnInventoryPerformed;
    }

    /// <summary>
    /// Toggles the inventory's open state and synchronizes time and cursor systems.
    /// </summary>
    private void OnInventoryPerformed()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);
        _cursosrController.SetCursorState(!isInventoryOpen);

        if (isInventoryOpen)
        {
            _timeManager.StartSlowMotion();
        }
        else
        {
            _timeManager.StopSlowMotion();
        }
    }

}