using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AC.Core
{
    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField] ItemDatabase _itemDatabase;
        [SerializeField] List<PoolIten> _listPoolItem;
        Dictionary<object, Pool> _listPool = new Dictionary<object, Pool>();
        public CheckLoadCompleted IsLoadAssetCompleted = new CheckLoadCompleted();
        private void Start()
        {
            LoadAllPool();
        }

        public void LoadAllPool()
        {
            _listPool = new Dictionary<object, Pool>();
            for (int i=0; i< _listPoolItem.Count; i++)
            {
                PoolIten poolItem = _listPoolItem[i];
                CreatePool(poolItem.PrefabRef, poolItem.Count);
            }
            for (int i = 0; i < _itemDatabase.ListItemData.Count; i++)
            {
                CreatePool(_itemDatabase.ListItemData[i].AssetRef, 1);
            }
        }


        void CreatePool(AssetReference assetRef, int count)
        {
            GameObject newPool = new GameObject("New Pool", typeof(Pool));
            newPool.transform.SetParent(transform);
            Pool pool = newPool.GetComponent<Pool>();
            pool.CreatePool(assetRef, count);
            _listPool.Add(assetRef.RuntimeKey, pool);
        }
        public void CheckLoadAllPool()
        {
            if (IsLoadAssetCompleted.IsLoadCompleted) return;
            foreach (Pool pool in _listPool.Values)
            {
                if (!pool.IsCreateSuccessed)
                {
                    return;
                }
            }
            IsLoadAssetCompleted.IsLoadCompleted = true;
        }

        public GameObject Spawn(AssetReference assetRef)
        {
            if(assetRef == null)
            {
                LogManager.LogWarning("AssetReference is Null");
                return null;
            }
            if(!_listPool.ContainsKey(assetRef.RuntimeKey))
            {
                LogManager.LogWarning("Khong tim thay Pool chua AssetReference");
                return null;
            }
            return _listPool[assetRef.RuntimeKey].GetObjectInPool();
        }

        public void ReturnToPool(GameObject go)
        {
            foreach (Pool pool in _listPool.Values)
            {
                if(pool.ReturnItemToPool(go)) return;
            }
        }
    }

    [System.Serializable]
    public class PoolIten
    {
        public AssetReference PrefabRef;
        public int Count;
    }
}

