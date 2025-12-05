using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles saving and loading game data, synchronizing a Dictionary<string, int> 
/// (ItemID to SlotIndex) in a Json file.
/// </summary>
public class DataReadWrite : MonoBehaviour
{
    [HideInInspector] public Dictionary<string, int> ItemData;
    private const string SAVE_FILE_NAME = "/savefile.json";

    /// <summary>
    /// Saves the provided ItemData dictionary to the persistent data path.
    /// Converts the Dictionary into two parallel Lists (keys and values) 
    /// for serialization with JsonUtility.
    /// </summary>
    /// <param name="itemData">The Dictionary<string, int> containing the current ItemID and SlotIndex.</param>
    public void SaveData(Dictionary<string, int> itemData)
    {
        Data data = new Data();
        data.ItemIDs = itemData.Keys.ToList();
        data.SlotIndices = itemData.Values.ToList();

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + SAVE_FILE_NAME, json);
    }

    /// <summary>
    /// Attempts to load the ItemData dictionary from the save file.
    /// Converts the two parallel serialized lists back into a functional Dictionary.
    /// </summary>
    public void LoadData()
    {
        string path = Application.persistentDataPath + SAVE_FILE_NAME;

        if (!File.Exists(path))
        {
            ItemData = new Dictionary<string, int>();
            return;
        }

        string json = File.ReadAllText(path);
        Data data = JsonUtility.FromJson<Data>(json);

        if (data.ItemIDs != null && data.SlotIndices != null && data.ItemIDs.Count == data.SlotIndices.Count)
        {
            ItemData = data.ItemIDs
                .Zip(data.SlotIndices, (key, value) => new { Key = key, Value = value })
                .ToDictionary(x => x.Key, x => x.Value);
        }
        else
        {
            ItemData = new Dictionary<string, int>();
        }
    }
}