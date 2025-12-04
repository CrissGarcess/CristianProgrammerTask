using UnityEngine;

/// <summary>
/// Defines the static data and properties for an item 
/// that can be consumed, used, collected, or manipulated by the player.
/// </summary>
[CreateAssetMenu(fileName = "ItemSO_", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string ID;
    public string Description;
    public Sprite Icon;
    public GameObject Prefab;
}