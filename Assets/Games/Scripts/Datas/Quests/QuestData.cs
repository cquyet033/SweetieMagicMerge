using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "QuestData", menuName = "Game Data/Quest Data")]
public class QuestData : ScriptableObject
{
    public int ID;
    public List<ItemData> ListItemData;
    public int Coin;
    public int Exp;
}
