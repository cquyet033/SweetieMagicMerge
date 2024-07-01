using AC.Core;
#if SDK_ADMOB
using GoogleMobileAds.Api;
#endif
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace AC.GameTool.Ads
{
    [InitializeOnLoad]
    public class AdsSettingWindow : EditorWindow
    {
        private const string UrlAdmobVersionLatest = "https://api.github.com/repos/googleads/googleads-mobile-unity/releases/latest";
        private const string UrlMaxVersionLatest = "https://dash.applovin.com/docs/v1/unity_integration_manager?plugin_version=null";

        private const string UrlAdmobOpenDownloadLink = "https://github.com/googleads/googleads-mobile-unity/releases";
        private const string UrlMaxOpenDownloadLink = "https://dash.applovin.com/documentation/mediation/unity/getting-started/integration";
        [SerializeField]
        AdsData _adsData;

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        static Version _admobCurrentVer, _maxCurrentVer;
        static Label _txtAdmobCurrentVer, _txtMaxCurrentVer;
        Version _admobLatestVer, _maxLatestVer;
        Label _txtAdmobLatestVer, _txtMaxLatestVer;
        Label _lbProgessDownload;
        Button _btnAdmobUpdate, _btnMaxUpdate;
        PropertyField _adsFieldData;

        AdmobPackageData _admobPackageData;
        MaxPackageData _maxPackageData;
        static AdsSettingWindow instance;

        [MenuItem("Game Tool/Ads Setting")]
        public static void ShowExample()
        {
            AdsSettingWindow wnd = GetWindow<AdsSettingWindow>();
            wnd.minSize = new Vector2(800, 600);
            wnd.maxSize = wnd.minSize;
            wnd.titleContent = new GUIContent("Ads Setting Window");
            instance = wnd;
        }
        static AdsSettingWindow()
        {
            
            //EditorApplication.projectChanged += ProjectAssetChanged;
            AssemblyReloadEvents.afterAssemblyReload += () =>
            {
                if (instance != null)
                {
                    instance.Repaint();
                }
            };
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            _adsData = AdsDataSetting.LoadInstance().AdsData;
            
            root.Add(labelFromUXML);
            LoadAllElement(root);
            ShowAllSDKCurrentInfo();
            GetAllSDKLatestInfo();            
        }
        void LoadAllElement(VisualElement root)
        {
            _txtAdmobCurrentVer = root.Q<Label>("txtAdmob_CurrentVer");
            _txtMaxCurrentVer = root.Q<Label>("txtMax_CurrentVer");
            _txtAdmobLatestVer = root.Q<Label>("txtAdmob_LatestVer");
            _txtMaxLatestVer = root.Q<Label>("txtMax_LatestVer");
            _lbProgessDownload = root.Q<Label>("lbProcessBar");
            _btnAdmobUpdate = root.Q<Button>("btnAdmob_Update");
            _btnMaxUpdate = root.Q<Button>("btnMax_Update");
            _adsFieldData = root.Q<PropertyField>("AdsSettingData");                      
            BindingAndAddRegisterElement();
            
        }

        void BindingAndAddRegisterElement()
        {
            SerializedObject so = new SerializedObject(this);
            var test = so.FindProperty("_adsData");
            _adsFieldData.Bind(so);
            if(_btnAdmobUpdate != null)
            {
                _btnAdmobUpdate.clickable.clicked += BtnUpdateAdmob_Click;
            }
            if (_btnMaxUpdate != null)
            {
                _btnMaxUpdate.clickable.clicked += BtnUpdateMax_Click;
            }
            _adsFieldData.RegisterCallback<ChangeEvent<bool>>(OnToggleChanged);
            _adsFieldData.RegisterCallback<ChangeEvent<string>>(OnStringChanged);
            _adsFieldData.RegisterCallback<InputEvent>(OnInputChanged);
            so.ApplyModifiedProperties();
        }

    
        static void ProjectAssetChanged()
        {
            //Debug.Log("After Assembly Reload");
            ShowAllSDKCurrentInfo();
            if(instance != null)
            {
                instance.Repaint();
            }
        }

        static void AddDefineToSetting(string defineName)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            ConditionalCompilationUtils.AddDefineIfNecessary(defineName, buildTargetGroup);
        }
        static void RemoveDefineFromSetting(string defineName)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            ConditionalCompilationUtils.RemoveDefineIfNecessary(defineName, buildTargetGroup);
        }
        #region Show Current SDK Info
        static void ShowAllSDKCurrentInfo()
        {
            ShowAdmobCurrentInfo();
            ShowMaxCurrentInfo();
            CheckSDKInProject.CheckRemoteConfig();
        }

        static void ShowAdmobCurrentInfo()
        {
            bool admobExist = CheckSDKInProject.CheckGoogleAdmobExist();
            string verText = "N/A";
            _admobCurrentVer = new Version();
#if SDK_ADMOB
            if (admobExist)
            {
                verText = AdRequest.Version;
                _admobCurrentVer = new Version(verText);
            }
#endif
            if (_txtAdmobCurrentVer != null)
            {
                _txtAdmobCurrentVer.text = verText;
                _txtAdmobCurrentVer.AddToClassList(admobExist ? "found" : "notfound");
                _txtAdmobCurrentVer.RemoveFromClassList(admobExist ? "notfound" : "found");
            }
        }

        static void ShowMaxCurrentInfo()
        {
            bool maxExist = CheckSDKInProject.CheckMaxExist();
            string verText = "N/A";
            _maxCurrentVer = new Version();
#if SDK_MAX
            if (maxExist)
            {
                verText = MaxSdk.Version;
                _maxCurrentVer = new Version(verText);
            }
#endif
            if (_txtMaxCurrentVer != null)
            {
                _txtMaxCurrentVer.text = verText;
                _txtMaxCurrentVer.AddToClassList(maxExist ? "found" : "notfound");
                _txtMaxCurrentVer.RemoveFromClassList(maxExist ? "notfound" : "found");
            }
        }
        #endregion
        
        #region Get Latest Ver SDK

        void GetAllSDKLatestInfo()
        {
            GetGoogleLatestInfo();
            GetMaxLatestVersion();
        }
        void GetGoogleLatestInfo()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetGoogleAdmobLatestData());
        }
        IEnumerator GetGoogleAdmobLatestData()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(UrlAdmobVersionLatest))
            {
                //_admobLatestVer = new Version();
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        _admobPackageData = JsonUtility.FromJson<AdmobPackageData>(webRequest.downloadHandler.text);
                        string verText = Regex.Replace(_admobPackageData.tag_name, "[^\\d\\.]", "");
                        _admobLatestVer = new Version(verText);
                        if(_txtAdmobLatestVer != null)
                        {
                            _txtAdmobLatestVer.text = verText;
                            _txtAdmobLatestVer.AddToClassList("found");
                            _txtAdmobLatestVer.RemoveFromClassList("notfound");
                        }                       
                        break;
                    default:
                        if(_txtAdmobLatestVer != null)
                        {
                            _txtAdmobLatestVer.text = "N/A";
                            _txtAdmobLatestVer.AddToClassList("notfound");
                            _txtAdmobLatestVer.RemoveFromClassList("found");
                        }                       
                        break;
                }
                webRequest.Dispose();
                if (_admobLatestVer != null && _admobCurrentVer != null)
                {
                    bool isUpdate = _admobLatestVer > _admobCurrentVer;
                    if (_btnAdmobUpdate != null)
                    {
                        _btnAdmobUpdate.SetEnabled(isUpdate);
                        _btnAdmobUpdate.text = "Update";
                    }
                }
                else
                {
                    if (_btnAdmobUpdate != null)
                    {
                        _btnAdmobUpdate.SetEnabled(true);
                        _btnAdmobUpdate.text = "Link Download";
                    }
                }
            }
        }

        void GetMaxLatestVersion()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(GetMaxLatestData());
        }
        IEnumerator GetMaxLatestData()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(UrlMaxVersionLatest))
            {
                //_maxLatestVer = new Version();
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:                        
                        _maxPackageData = JsonUtility.FromJson<MaxPackageData>(webRequest.downloadHandler.text);
                        string verText = _maxPackageData.AppLovinMax.LatestVersions.Unity;
                        _maxLatestVer = new Version(verText);
                        if (_txtMaxLatestVer != null)
                        {
                            _txtMaxLatestVer.text = verText;
                            _txtMaxLatestVer.AddToClassList("found");
                            _txtMaxLatestVer.RemoveFromClassList("notfound");
                        }                       
                        break;
                    default:
                        if (_txtMaxLatestVer != null)
                        {
                            _txtMaxLatestVer.text = "N/A";
                            _txtMaxLatestVer.AddToClassList("notfound");
                            _txtMaxLatestVer.RemoveFromClassList("found");
                        }
                        break;
                }
                webRequest.Dispose();
                if (_maxLatestVer != null && _maxCurrentVer != null)
                {
                    bool isUpdate = _maxLatestVer > _maxCurrentVer;
                    if (_btnMaxUpdate != null)
                    {
                        _btnMaxUpdate.text = "Update";
                        _btnMaxUpdate.SetEnabled(isUpdate);
                    }
                }
                else
                {
                    if (_btnMaxUpdate != null)
                    {
                        _btnMaxUpdate.SetEnabled(true);
                        _btnMaxUpdate.text = "Link Download";
                    }
                }
            }
        }
        #endregion

        #region Update 
        void ResetBtnUpdate()
        {
            if (_admobLatestVer != null && _admobCurrentVer != null)
            {
                bool isUpdate = _admobLatestVer > _admobCurrentVer;
                if (_btnAdmobUpdate != null)
                {
                    _btnAdmobUpdate.SetEnabled(isUpdate);
                }
            }
            if (_maxLatestVer != null && _maxCurrentVer != null)
            {
                bool isUpdate = _maxLatestVer > _maxCurrentVer;
                if (_btnMaxUpdate != null)
                {
                    _btnMaxUpdate.SetEnabled(isUpdate);
                }
            }
        }
        void BtnUpdateAdmob_Click()
        {
            if(_admobLatestVer == null)
            {
                Application.OpenURL(UrlAdmobOpenDownloadLink);
                return;
            }
            _lbProgessDownload.style.width = 0;
            if (_admobLatestVer != null && _admobPackageData != null && _admobPackageData.assets.Count > 0)
            {
                string admobUrl = _admobPackageData.assets[0].browser_download_url;
                string filename = string.Format("Admob_{0}.unitypackage", _admobLatestVer.ToString());
                string pathPackage = Path.Combine(Application.temporaryCachePath, filename);
                _btnAdmobUpdate.SetEnabled(false);
                _btnMaxUpdate.SetEnabled(false);
                if (!File.Exists(pathPackage))
                {
                    EditorCoroutineUtility.StartCoroutineOwnerless(DownLoadFilePackage(admobUrl, pathPackage));
                }
                else
                {
                    _lbProgessDownload.style.width = 600;
                    AssetDatabase.ImportPackage(pathPackage, true);
                    LogManager.Log("Inport Package: " + pathPackage);
                    ResetBtnUpdate();
                }
                
            }
        }

        void BtnUpdateMax_Click()
        {
            if (_maxLatestVer == null)
            {
                Application.OpenURL(UrlMaxOpenDownloadLink);
                return;
            }
            _lbProgessDownload.style.width = 0;
            if (_maxLatestVer != null && _maxPackageData != null && _maxPackageData.AppLovinMax != null && !string.IsNullOrEmpty(_maxPackageData.AppLovinMax.DownloadUrl))
            {
                string admobUrl = _maxPackageData.AppLovinMax.DownloadUrl;
                string filename = string.Format("MaxAppLovin_{0}.unitypackage", _maxLatestVer.ToString());
                string pathPackage = Path.Combine(Application.temporaryCachePath, filename);
                _btnAdmobUpdate.SetEnabled(false);
                _btnMaxUpdate.SetEnabled(false);
                if (!File.Exists(pathPackage))
                {
                    EditorCoroutineUtility.StartCoroutineOwnerless(DownLoadFilePackage(admobUrl, pathPackage));
                }
                else
                {
                    _lbProgessDownload.style.width = 600;
                    AssetDatabase.ImportPackage(pathPackage, true);
                    LogManager.Log("Inport Package: " + pathPackage);
                    
                }
                ResetBtnUpdate();
            }
        }

        
        IEnumerator DownLoadFilePackage(string fileUrl, string fileName)
        {
            _lbProgessDownload.style.width = 0;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(fileUrl))
            {
                webRequest.downloadHandler = new DownloadHandlerFile(fileName);
                UnityWebRequestAsyncOperation downloadAsync = webRequest.SendWebRequest();
                while (!downloadAsync.isDone)
                {
                    //_progessDownload.value = Mathf.FloorToInt(downloadAsync.progress * 100);
                    _lbProgessDownload.style.width = downloadAsync.progress * 600;
                    yield return null;
                }
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    //Done
                    _lbProgessDownload.style.width = 600;
                    AssetDatabase.ImportPackage(fileName, true);
                    LogManager.Log("Inport Package: " + fileName);
                }
                ResetBtnUpdate();
                webRequest.Dispose();
            }

        }
        #endregion

        #region Data Setting Change

        private void OnInputChanged(InputEvent evt)
        {
            EditorUtility.SetDirty(AdsDataSetting.LoadInstance());
        }

        private void OnToggleChanged(ChangeEvent<bool> evt)
        {
            EditorUtility.SetDirty(AdsDataSetting.LoadInstance());
#if FIREBASE_REMOTE_CFG
            if(_adsData.AdSdkDataSetting.IsUseRemoteCfgAdsInterval)
            {
                AddDefineToSetting("USE_ADS_REMOTECONFIG");
            }
            else
            {
                RemoveDefineFromSetting("USE_ADS_REMOTECONFIG");
            }

#else
            RemoveDefineFromSetting("USE_ADS_REMOTECONFIG");
#endif

        }
        private void OnStringChanged(ChangeEvent<string> evt)
        {
            EditorUtility.SetDirty(AdsDataSetting.LoadInstance());
            switch (_adsData.Type)
            {
                case SDKType.None:
                    RemoveDefineFromSetting("USE_ADMOB");
                    RemoveDefineFromSetting("USE_MAX");
                    break;
#if SDK_ADMOB
                case SDKType.Admob:
                    AddDefineToSetting("USE_ADMOB");
                    RemoveDefineFromSetting("USE_MAX");
                    break;
#endif
#if SDK_MAX
                case SDKType.Max:
                    RemoveDefineFromSetting("USE_ADMOB");
                    AddDefineToSetting("USE_MAX");
                    break;
#endif
            }
        }

#endregion
    }
}

