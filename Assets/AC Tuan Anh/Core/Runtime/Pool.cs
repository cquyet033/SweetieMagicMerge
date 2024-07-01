using AC.Attribute;
using AC.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Pool : MonoBehaviour
{
    AsyncOperationHandle<GameObject> _loadItem = new AsyncOperationHandle<GameObject>();
    Stack<GameObject> _listItemInPool = new Stack<GameObject>();
    //List<GameObject> _listItemInPool = new List<GameObject>();
    [SerializeField, ReadOnlly]
    List<GameObject> _listItemActived = new List<GameObject>();
    public bool IsCreateSuccessed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePool(AssetReference itemRef, int count)
    {
        IsCreateSuccessed = false;
        _loadItem = itemRef.LoadAssetAsync<GameObject>();
        _loadItem.Completed += handle =>
        {
            IsCreateSuccessed = true;
            for(int i=0; i< count; i++)
            {
                CreatItemInPool();
            }
            gameObject.name = handle.Result.name;
            PoolManager.Instance.CheckLoadAllPool();
        };
    }

    void CreatItemInPool()
    {
        GameObject go = Instantiate(_loadItem.Result, transform);
        go.SetActive(false);
        _listItemInPool.Push(go);
    }

    public GameObject GetObjectInPool()
    {
        if(_listItemInPool.Count <= 0)
            CreatItemInPool();
        GameObject go = _listItemInPool.Pop();
        _listItemActived.Add(go);
        return go;
    }

    public bool ReturnItemToPool(GameObject go)
    {
        if(_listItemActived.Contains(go))
        {
            _listItemActived.Remove(go);          
            go.SetActive(false);
            go.transform.SetParent(transform);
            _listItemInPool.Push(go);
            return true;
        }
        return false;
    }
}
