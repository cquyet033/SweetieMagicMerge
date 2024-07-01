using AC.GameTool.SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    [SerializeField] int _id;
    [SerializeField] QuestStatus _questStatus;
    [SerializeField] QuestData _questData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStatus(QuestStatus questStatus)
    {
        _questStatus = questStatus;
    }

    public void SetQuestData(QuestData questData)
    {
        _id = questData.ID;
        _questData = questData;
    }

    public void QuestUIClick()
    {
        if (_questStatus == QuestStatus.Pending) return;

        QuestManager.Instance.QuestCLick();

        RewardQuest();
        
    }

    public void RewardQuest()
    {
        SaveManager.Instance.GameData.Coin += _questData.Coin;
        SaveManager.Instance.GameData.Exp += _questData.Exp;

        SaveManager.Instance.SaveGameData();
    }
}
