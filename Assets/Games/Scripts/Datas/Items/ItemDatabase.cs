using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Item Database", menuName = "Game Data/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> ListItemData;
}

[System.Serializable]
public class ItemData
{
    public ItemType ItemType;
    public int LevelID;
    public Sprite Icon;
    public AssetReference AssetRef;
    public GameObject Prefab;
}

public enum ItemType
{
    Producing,
    Ingredient
}
