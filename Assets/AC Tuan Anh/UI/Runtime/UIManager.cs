using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC.Core;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using AC.Attribute;
using DG.Tweening;

namespace AC.GameTool.UI
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("Content")]
        public Transform UIContent;
        [SerializeField] Transform _normalContent, _popUpContent, _alwaysTopContent;
        [SerializeField] bool _isShowLoadingUiInStart;
        [ConditionField("_isShowLoadingUiInStart")]
        [SerializeField] GameObject _loadingUIPrefab;
        [Header("List UI")]
        [SerializeField] List<UIItemLoadData> _listUIInGame;
        Dictionary<UITypeName, Stack<BaseUI>> _uiCacheList = new Dictionary<UITypeName, Stack<BaseUI>>();
        Dictionary<UITypeName, AsyncOperationHandle<GameObject>> _uiAssets = new Dictionary<UITypeName, AsyncOperationHandle<GameObject>>();
        [ReadOnlly]
        public CheckLoadCompleted CheckLoadCompleted;
        protected override void Awake()
        {
            base.Awake();
            CheckLoadCompleted = new CheckLoadCompleted { IsLoadCompleted = _listUIInGame.Count <= 0};
            CheckShowLoadingUIInStart();
            LoadAllUIAsset();
        }
        private void Update()
        {
            
        }
        private void OnDestroy()
        {
            foreach(var Uihandle in _uiAssets.Values)
            {
                Addressables.Release(Uihandle);
            }
        }
        #region Load UI Asset
        void CheckShowLoadingUIInStart()
        {
            if(_isShowLoadingUiInStart)
            {
                Stack<BaseUI> loadStack = new Stack<BaseUI>();
                LoadingUI loadUI = Instantiate(_loadingUIPrefab, _alwaysTopContent).GetComponent<LoadingUI>();
                loadStack.Push(loadUI);
                _uiCacheList.Add(UITypeName.Loading, loadStack);
            }
        }
        bool IsCheckAllUILoaded()
        {
            foreach (var uiAsset in _uiAssets)
            {
                if (!uiAsset.Value.IsDone)
                    return false;
            }
            return true;
        }

        
        void LoadAllUIAsset()
        {
            for(int i=0; i< _listUIInGame.Count; i++)
            {
                UIItemLoadData loadData = _listUIInGame[i];
                if(!_uiAssets.ContainsKey(loadData.TypeName))
                {
                    AsyncOperationHandle<GameObject> uiAssetHandle = loadData.AssetPrefabRef.LoadAssetAsync<GameObject>();
                    _uiAssets.Add(loadData.TypeName, uiAssetHandle);
                    uiAssetHandle.Completed += handle =>
                    {
                        CheckLoadCompleted.IsLoadCompleted = IsCheckAllUILoaded();
                    };
                    
                }
            }
        }
        #endregion

        #region Load & Show UI
        public BaseUI TryShowUI(UITypeName uiID, TransitionType transType = TransitionType.None, Ease easeType = Ease.Linear, float duration = 1f,Action onOpen = null, Action onClose = null, bool isAutoShow = true)
        {
            if(_uiCacheList.ContainsKey(uiID))
            {
                Stack<BaseUI> stackUI = _uiCacheList[uiID];
                if(stackUI.Count > 0)
                {
                    BaseUI ui = stackUI.Pop();
                    SetupUIAndShow(uiID, ui, onOpen, onClose, isAutoShow);
                    if (!ui.IsMultiUI)
                        stackUI.Push(ui);
                    return ui;
                }
            }
            if (_uiAssets.ContainsKey(uiID))
                return ShowNewUI(uiID, _uiAssets[uiID].Result, onOpen, onClose, isAutoShow);
            return null;
        }

        BaseUI ShowNewUI(UITypeName uiID, GameObject uiPrefab, Action onOpen, Action onClose, bool isAutoShow)
        {
            if(!_uiCacheList.ContainsKey(uiID))
            {
                Stack<BaseUI> newStackUI = new Stack<BaseUI>();
                _uiCacheList.Add(uiID, newStackUI);
            }
            Stack<BaseUI> stackUI = _uiCacheList[uiID];
            if(stackUI.Count <= 0)
            {
                BaseUI newUI = LoadNewUI(uiPrefab);
                stackUI.Push(newUI);
            }
            BaseUI ui = stackUI.Pop();
            SetupUIAndShow(uiID, ui, onOpen, onClose, isAutoShow);     
            if(!ui.IsMultiUI)
                stackUI.Push(ui);
            return ui;
        }

        void SetupUIAndShow(UITypeName uiID,BaseUI ui, Action onOpen, Action onClose, bool isAutoShow)
        {
            //ui.transform.SetAsLastSibling();
            ui.SetCallback(() =>
            {
                onOpen?.Invoke();
            }, () =>
            {
                CloseUI(uiID, ui);
                onClose?.Invoke();
            });
            if(isAutoShow)
            {
                ui.OnUiShow();
            }                
        }
        //public BaseUI TryShowUI(string uiPath, Action onOpen = null, Action onClose = null)
        //{
        //    Stack<BaseUI> stackUI;
        //    //Lay ngan xep
        //    if (!_uiCacheList.TryGetValue(uiPath, out stackUI))
        //    {
        //        stackUI = new Stack<BaseUI>();
        //        _uiCacheList.Add(uiPath, stackUI);
        //    }
        //    //Kiem tra ngan xep
        //    if (stackUI.Count <= 0) //Neu chua co Ptu thi Tao moi 1 UI va dua vao ngan xep
        //    {
        //        stackUI.Push(LoadNewUI(uiPath));
        //    }
        //    BaseUI uiObj = stackUI.Pop();//Lay UI tu ngan xep
        //    if (!uiObj.IsMultiUI) //Neu UI khong phai Mutil thi dua lai vao Stack
        //    {
        //        stackUI.Push(uiObj);
        //    }
        //    //Set Layer
        //    uiObj.transform.SetAsLastSibling();
        //    //Show Callback
        //    uiObj.SetCallback(() =>
        //    {
        //        onOpen?.Invoke();
        //    }, () =>
        //    {
        //        CloseUI(uiPath, uiObj);
        //        onClose?.Invoke();
        //    });
        //    uiObj.OnUiShow();
        //    Resources.UnloadUnusedAssets();
        //    return uiObj;
        //}
        BaseUI LoadNewUI(GameObject uiPrefab)
        {
            UILayer layer = uiPrefab.GetComponent<BaseUI>().UILayer;
            Transform parent = UIContent;
            switch (layer)
            {
                case UILayer.Normal:
                    parent = _normalContent;
                    break;
                case UILayer.Popup:
                    parent = _popUpContent;
                    break;
                case UILayer.AlwaysTop:
                    parent = _alwaysTopContent;
                    break;
            }
            return Instantiate(uiPrefab, parent).GetComponent<BaseUI>();
        }


        void CloseUI(UITypeName uiID, BaseUI ui)
        {
            if(_uiCacheList.ContainsKey(uiID))
            {
                Stack<BaseUI> stackUi = _uiCacheList[uiID];
                ui.RemoveAllCallback();
                stackUi.Push(ui);
            }
        }
        #endregion
       
    }

    [System.Serializable]
    public class UIItemLoadData
    {
        public UITypeName TypeName;
        public AssetReference AssetPrefabRef;
    }
}

