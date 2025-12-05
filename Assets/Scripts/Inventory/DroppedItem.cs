using System.Collections;
using UnityEngine;

/// <summary>
/// Manages an item that has been dropped or spawned in the game world.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DroppedItem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool autoStart;
    [SerializeField] private float _pickupDelay = 3f;

    [Header("State")]
    public Item Item;
    public bool PickedUp = false;

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        if (_collider != null)
            _collider.enabled = false;
    }

    /// <summary>
    /// Check whether the item should be initialized at the start of the game and, if so, proceed to initialize it. 
    /// </summary>
    private void Start()
    {
        if (autoStart && Item != null)
            Initialize(Item);
    }

    /// <summary>
    /// Initializes the dropped item with its data, instantiates the visual prefab, 
    /// and starts the pickup timer.
    /// </summary>
    /// <param name="item">The item that will be initialized.</param>
    public void Initialize(Item item)
    {
        Item = item;

        if (Item == null)
            return;

        if (Item.Prefab != null)
        {
            GameObject droppedItem = Instantiate(Item.Prefab, transform);
            Vector3 localOffset = new Vector3(0, 0.5f, 0);
            droppedItem.transform.SetLocalPositionAndRotation(localOffset, Quaternion.identity);
        }

        if (_collider == null)
            _collider = GetComponent<Collider>();

        if (_collider != null)
            _collider.enabled = false;

        StartCoroutine(EnablePickup(_pickupDelay));
    }

    /// <summary>
    /// Coroutine that disables the dropped item collider for a defined delay before the item can be obtained again.
    /// </summary>
    /// <param name="delay">The time (in seconds) to wait before enabling pickup.</param>
    private IEnumerator EnablePickup(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_collider == null)
            _collider = GetComponent<Collider>();

        if (_collider != null)
        {
            _collider.enabled = true;
        }
    }
}