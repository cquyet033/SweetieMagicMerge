using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game Data/LevelData")]
public class LevelData : ScriptableObject
{
    public List<QuestData> ListQuestData;
}