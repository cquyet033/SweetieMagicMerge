#if USE_MAX && SDK_MAX
using AC.Attribute;
using AC.Core;
using AudienceNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.GameTool.Ads
{
    public abstract class MaxSdkAdsManager : Singleton<MaxSdkAdsManager>, IBaseAdsManager
    {
        [SerializeField]
        bool _isCheatMode;
        [SerializeField]
        bool _isTestMode;
        [Header("Max SDK:")]
        [SerializeField, ReadOnlly] 
        protected AdsData _adData;
        [SerializeField, ReadOnlly]        
        protected CheckLoadCompleted _completedChecking = new CheckLoadCompleted();
        public CheckLoadCompleted CompletedChecking => _completedChecking;

        public bool IsCheatMode { get => _isCheatMode; set { _isCheatMode = value; ShowBanner(!_isCheatMode); } }

        [Header("Banner:")]
        [SerializeField, ReadOnlly]
        protected string _bannerID = "Banner ID";
        [Header("Interstitial:")]
        [SerializeField, ReadOnlly]
        protected string _interID = "Interstitial ID";
        [SerializeField, ReadOnlly]
        protected long _timeBetweenIntersAd = 0;
        [SerializeField, ReadOnlly]
        protected string _interPlacement;
        protected int _interstitialRetryAttempt;
        [SerializeField, ReadOnlly]
        private bool _isShowInterstitialAd;
        [SerializeField, ReadOnlly]
        protected long _currentInterstitialAdInterval;
        private DateTime _timeShowInter;
        //protected Coroutine _interstitialInterval_Timer;
        [SerializeField, ReadOnlly]
        protected int _interCountShow;
        [Header("Reward:")]
        [SerializeField, ReadOnlly]
        protected string _rewardID = "Reward ID";
        protected int _rewardedRetryAttempt;
        protected string _rewardPlacement;
        protected Action _rewardSuccessed;
        protected Action<AdsErrorCode> _rewardFailed;
        DateTime _timeAdsReward;

        protected override void Awake()
        {
            base.Awake();
            _adData = AdsDataSetting.LoadInstance().AdsData;
            _timeBetweenIntersAd = 30;


            SetAllAdsID();
            //_interCountShow = 0;
        }
        void SetAllAdsID()
        {

#if UNITY_ANDROID
            _bannerID = _adData.AdSdkDataSetting.Android.BanerUnitID;
            _interID = _adData.AdSdkDataSetting.Android.InterstitialUnitID;
            _rewardID = _adData.AdSdkDataSetting.Android.RewardUnitID;
#elif UNITY_IOS
            _bannerID = _adData.AdSdkDataSetting.IOS.BanerUnitID;
            _interID = _adData.AdSdkDataSetting.IOS.InterstitialUnitID;
            _rewardID = _adData.AdSdkDataSetting.IOS.RewardUnitID;
#endif
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init()
        {
            MetaAdSettings.SetDataProcessingOptions(new string[] { });
            MaxSdk.SetTestDeviceAdvertisingIdentifiers(_adData.AdSdkDataSetting.Android.TestDevicesGAID);
            MaxSdk.SetHasUserConsent(true);
            MaxSdk.SetIsAgeRestrictedUser(false);
            MaxSdk.SetDoNotSell(false);
            MaxSdk.SetSdkKey(_adData.MaxSdkKey);
            MaxSdk.InitializeSdk();
            MaxSdk.SetVerboseLogging(true);
            
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
#if UNITY_IOS || UNITY_IPHONE
                if (MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5") != MaxSdkUtils.VersionComparisonResult.Lesser)
                {
                    AudienceNetwork.MetaAdSettings.SetAdvertiserTrackingEnabled(true);
                }
#endif

                Debug.Log("MAX SDK Initialized");
                InitializeBannerAds();
                InitializeInterstitialAds();
                InitializeRewardedAds();
                _completedChecking.IsLoadCompleted = true;
                if (_isTestMode)
                    MaxSdk.ShowMediationDebugger();
                //MaxSdk.SetVerboseLogging(true);
            };
            
        }

        #region Banner
        void InitializeBannerAds()
        {
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(_bannerID, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.SetBannerExtraParameter(_bannerID, "adaptive_banner", "true");
            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(_bannerID, Color.white);

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            if(_adData.AdSdkDataSetting.AutoLoadBannerAdOnStartup)
            {
                ShowBanner(true);
            }
        }
        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
            Debug.Log("Banner ad loaded");
        }

        private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) {
            Debug.Log("Banner ad failed to load with error code: " + errorInfo.Code);
        }

        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
            Debug.Log("Banner ad clicked");
        }

        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
            Debug.Log("Banner ad revenue paid");

            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
            GameAnalyticManager.SendAdRevenue(MediationType.Max, networkName, countryCode, adUnitIdentifier, adInfo.AdFormat, placement, revenue);
        }

        public void ShowBanner(bool isShow)
        {
            if(isShow)
                MaxSdk.ShowBanner(_bannerID);
            else
                MaxSdk.HideBanner(_bannerID);
        }
        #endregion

        #region Interstitial
        void InitializeInterstitialAds()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(_interID);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
            Debug.Log("Interstitial loaded");
            GameAnalyticManager.SendAdInterLoaded();
            // Reset retry attempt
            _interstitialRetryAttempt = 0;
        }

        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            _interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _interstitialRetryAttempt));

            Debug.Log("Interstitial failed to load with error code: " + errorInfo.Code);

            Invoke(nameof(LoadInterstitial), (float)retryDelay);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            Debug.Log("Interstitial failed to display with error code: " + errorInfo.Code);
            LoadInterstitial();
        }

        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad
            Debug.Log("Interstitial dismissed");
            LoadInterstitial();
            //if (_interstitialInterval_Timer != null)
            //{
            //    StopCoroutine(_interstitialInterval_Timer);
            //    _interstitialInterval_Timer = null;
            //}
            //_interstitialInterval_Timer = StartCoroutine(InsterstitialInterval());
            _timeShowInter = DateTime.UtcNow;
        }

        private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Interstitial revenue paid");

            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
            GameAnalyticManager.SendAdRevenue(MediationType.Max, networkName, countryCode, adUnitIdentifier, adInfo.AdFormat, placement, revenue);
        }
        public void ShowInterstitialAds(string placement = null, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            if(_isCheatMode)
            {
                successed?.Invoke();
                return;
            }
            if (MaxSdk.IsInterstitialReady(_interID))
            {
                if ((DateTime.UtcNow - _timeShowInter).TotalSeconds > _timeBetweenIntersAd)
                {
                    _isShowInterstitialAd = false;
                }
                if (!_isShowInterstitialAd)
                {
                    _interPlacement = placement;
                    MaxSdk.ShowInterstitial(_interID);
                    successed?.Invoke();
                    _isShowInterstitialAd = true;
                    _interCountShow = PlayerPrefs.GetInt("ADS_INTER_COUNT", 0);
                    _interCountShow += 1;
                    PlayerPrefs.SetInt("ADS_INTER_COUNT", _interCountShow);
                    GameAnalyticManager.SendAdInterDisplay();
                    GameAnalyticManager.SendAdInterShowCount(_interCountShow);
                }                 
            }
            else
            {
                failed?.Invoke(AdsErrorCode.NoVideoAds);
            }
        }
        public void ShowInterstitialAdsResume(string placement = null, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            if (_isCheatMode)
            {
                successed?.Invoke();
                return;
            }
            if (MaxSdk.IsInterstitialReady(_interID))
            {
                _interPlacement = placement;
                MaxSdk.ShowInterstitial(_interID);
                successed?.Invoke();
                _isShowInterstitialAd = true;
                //_interCountShow = PlayerPrefs.GetInt("ADS_INTER_COUNT", 0);
                //_interCountShow += 1;
                //PlayerPrefs.SetInt("ADS_INTER_COUNT", _interCountShow);
                GameAnalyticManager.SendAdInterDisplay();
                //GameAnalyticManager.SendAdInterShowCount(_interCountShow);
            }
            else
            {
                failed?.Invoke(AdsErrorCode.NoVideoAds);
            }
        }
        //IEnumerator InsterstitialInterval()
        //{
        //    _currentInterstitialAdInterval = _timeBetweenIntersAd;
        //    while(_currentInterstitialAdInterval > 0)
        //    {
        //        yield return new WaitForSecondsRealtime(1);
        //        _currentInterstitialAdInterval -= 1;
        //    }
        //    _isShowInterstitialAd = false;
        //}

        //public void SetInterAdsCoolDown()
        //{
        //    _isShowInterstitialAd = true;
        //    //if (_interstitialInterval_Timer != null)
        //    //{
        //    //    StopCoroutine(_interstitialInterval_Timer);
        //    //    _interstitialInterval_Timer = null;
        //    //}
        //    //_interstitialInterval_Timer = StartCoroutine(InsterstitialInterval());
        //    _timeShowInter = DateTime.Now;
        //}
        #endregion

        #region Reward
        void InitializeRewardedAds()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

            // Load the first RewardedAd
            LoadRewardedAd();
        }

        void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(_rewardID);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
            Debug.Log("Rewarded ad loaded");
            GameAnalyticManager.SendAdRewardLoaded();
            // Reset retry attempt
            _rewardedRetryAttempt = 0;
        }

        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
            _rewardedRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _rewardedRetryAttempt));

            Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

            Invoke(nameof(LoadRewardedAd), (float)retryDelay);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. We recommend loading the next ad
            Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
            _rewardFailed?.Invoke(AdsErrorCode.DisplayFaile);
            LoadRewardedAd();
            _timeShowInter = DateTime.UtcNow.AddSeconds(-(_timeAdsReward - _timeShowInter).TotalSeconds);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad displayed");
            GameAnalyticManager.SendAdRewardDisplay();
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded ad clicked");
        }

        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            Debug.Log("Rewarded ad dismissed");
            
            LoadRewardedAd();
            _timeShowInter = DateTime.UtcNow.AddSeconds(-(_timeAdsReward - _timeShowInter).TotalSeconds);
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad was displayed and user should receive the reward
            Debug.Log("Rewarded ad received reward");
            _rewardSuccessed?.Invoke();
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Rewarded ad revenue paid");

            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
            GameAnalyticManager.SendAdRevenue(MediationType.Max, networkName, countryCode, adUnitIdentifier, adInfo.AdFormat, placement, revenue);
        }
        public void ShowRewardAds(string placement = null, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            if (_isCheatMode)
            {
                successed?.Invoke();
                return;
            }
            if (!CheckInternet())
            {
                failed?.Invoke(AdsErrorCode.NoInternet);
                return;
            }
            _rewardPlacement = placement;
            _rewardSuccessed = successed;
            _rewardFailed = failed;           
            if(MaxSdk.IsRewardedAdReady(_rewardID))
            {               
                MaxSdk.ShowRewardedAd(_rewardID);
                _timeAdsReward = DateTime.UtcNow;
            }
            else
            {
                failed?.Invoke(AdsErrorCode.NoVideoAds);
                Debug.Log("Reward Ad not ready");
            }
        }
        #endregion

        public bool CheckInternet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public void UpdateIntervalAd()
        {
#if USE_ADS_REMOTECONFIG
            _timeBetweenIntersAd = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(_adData.AdSdkDataSetting.AdsIntervalKey).LongValue;
#else
            _timeBetweenIntersAd = _adData.AdSdkDataSetting.AdsInterval;
#endif
        }
    }
}

#endif