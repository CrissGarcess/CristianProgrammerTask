using System.Collections.Generic;
using System;

/// <summary>
/// Serializable container class used for saving and loading Dictionary<string, int> data.
/// </summary>
/// 
[Serializable]
public class Data
{
    public List<string> ItemIDs = new List<string>();
    public List<int> SlotIndices = new List<int>();
}
