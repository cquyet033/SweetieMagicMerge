using AC.Attribute;

using System.IO;
using UnityEditor;
using UnityEngine;

namespace AC.GameTool.Ads
{
    public class AdsDataSetting : ScriptableObject
    {
        private const string AdsDataSettingResPath = "Assets/AC Tuan Anh/Ads/Resources";
        private const string fileName = "Ads Data Setting";
        [HideInInspector]
        public AdsData AdsData;

        public static AdsDataSetting LoadInstance()
        {
            //Read from resources.
            var instance = Resources.Load<AdsDataSetting>(fileName);

#if UNITY_EDITOR
            //Create instance if null.
            if (instance == null)
            {
                Directory.CreateDirectory(AdsDataSettingResPath);
                instance = ScriptableObject.CreateInstance<AdsDataSetting>();
                string assetPath = Path.Combine(
                    AdsDataSettingResPath,
                    fileName + ".asset");
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }
#endif
            return instance;
        }
    }

    [System.Serializable]
    public class AdsData
    {
        [Header("Ad Open App Setting:")]
#if SDK_ADMOB
        public bool IsUseOpenApp;
        //[ConditionField("IsUseOpenApp")]
        public string OpenAppIdAndroid;
        public string OpenAppIdIos;
#endif
        [Header("Ads Setting:")]
        public SDKType Type;
        //[ConditionField("Type", true, SDKType.None)]
#if USE_MAX
        [Space]
        public string MaxSdkKey;
        [Space]
#endif
        public AdSdkDataSetting AdSdkDataSetting;
    }

    [System.Serializable]
    public class AdSdkDataSetting
    {
        public bool IsTagForChild;
        [Header("Time Between Interstitial:")]

#if FIREBASE_REMOTE_CFG
        public bool IsUseRemoteCfgAdsInterval;
#endif
#if USE_ADS_REMOTECONFIG
        //[ConditionField("IsUseRemoteConfigTimeBetween", true)]
        public string AdsIntervalKey = "Key";
#else
        public int AdsInterval = 30;
#endif



        public bool AutoLoadBannerAdOnStartup = true;
        public AdsDataPlatform Android;
        public AdsDataPlatform IOS;
    }
    [System.Serializable]
    public class AdsDataPlatform
    {
        public string BanerUnitID;
        public string RewardUnitID;
        public string InterstitialUnitID;
        public string[] TestDevicesID;
        public string[] TestDevicesGAID;
    }

    public enum SDKType
    {
        None,
#if SDK_ADMOB
        Admob,
#endif
#if SDK_MAX
        Max
#endif
    }
}

