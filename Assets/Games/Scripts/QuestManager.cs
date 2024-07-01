using AC.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class QuestManager : Singleton<QuestManager>
{
    //AsyncOperationHandle<GameObject> handle = new AsyncOperationHandle<GameObject>();

    [SerializeField] int _currentGameLevel;

    [SerializeField] List<Quest> _listQuests;
    [SerializeField] List<LevelData> _listLevelDatas;

    [SerializeField] GameObject _questUIContent;
    [SerializeField] GameObject _questPrefab;

    protected override void Awake()           
    {

        //for (int i = 0; i < _listQuestDatas.Count; i++)
        //{
        //    GameObject newQ = Instantiate(_questPrefab, _questUIContent.transform);
        //    for (int j = 0; j < _listQuestDatas[i].ListItemData.Count; j++)
        //    {
        //        //Debug.Log(_listQuestDatas[i].ListItemData[j].Prefab);

        //        GameObject newItem = _listQuestDatas[i].ListItemData[j].Prefab;
        //        Instantiate(newItem, newQ.transform);

        //    }
        //}
    }

    private void Start()
    {
        StartCoroutine(LoadQuestData());
    }

    IEnumerator LoadQuestData()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < _listLevelDatas[_currentGameLevel].ListQuestData.Count; i++)
        {
            GameObject newQ = Instantiate(_questPrefab, _questUIContent.transform);
            QuestData newQuestData = _listLevelDatas[_currentGameLevel].ListQuestData[i];
            newQ.GetComponent<Quest>().SetQuestData(newQuestData);
            _listQuests.Add(newQ.GetComponent<Quest>());

            for (int j = 0; j < _listLevelDatas[_currentGameLevel].ListQuestData[i].ListItemData.Count; j++)
            {
                AssetReference assetReference = _listLevelDatas[_currentGameLevel].ListQuestData[i].ListItemData[j].AssetRef;
                Debug.Log(assetReference);
                GameObject newItem = PoolManager.Instance.Spawn(assetReference);
                newItem.transform.SetParent(newQ.transform, false);
                newItem.SetActive(true);

                //GameObject newItem = _listQuestDatas[i].ListItemData[j].Prefab;
                //Instantiate(newItem, newQ.transform);

                //AsyncOperationHandle<GameObject> newItem = _listQuestDatas[i].ListItemData[j].AssetRef.LoadAssetAsync<GameObject>();
                //handle = assetReference.LoadAssetAsync<GameObject>(); ;
                //handle.Completed += LoadCompleted =>
                //{
                //    Debug.Log(handle.Result);
                //    GameObject loadedGameObject = handle.Result;
                //    Instantiate(loadedGameObject, newQ.transform);
                //};

                //Instantiate(newItem.Result, newQ.transform);
            }
        }

        yield return null;
    }

    public void UpdateAllQuestStatus()
    {
        for (int i = 0; i < _listQuests.Count; i++)
        {
            
        }
    }

    public void QuestCLick()
    {
        
    }
}