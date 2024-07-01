using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC.Core;
using AC.Attribute;

using System;

#if SDK_ADMOB
using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Api;
#endif

namespace AC.GameTool.Ads
{
    public class AppOpenAdManager : Singleton<AppOpenAdManager>
    {
        AdsDataSetting _adsDataSetting;
        [SerializeField, ReadOnlly]
        private bool _isUseOpenAppAd;
        [SerializeField]
        private bool _isTest;
  
        [SerializeField, ReadOnlly]
        private string _adUnitIdTest;

        [SerializeField, ReadOnlly]
        private string _adUnitId;
        [SerializeField, ReadOnlly]
        private bool _isShowOpenAds;
        [SerializeField, ReadOnlly]
        private bool _isResumeAds;
        DateTime _expireTime;
        Action _showAdCompleted;
        public bool IsLoadingCompleted;

        public bool IsShowGdprCompleted;
#if SDK_ADMOB
        private AppOpenAd _appOpenAd;

        public bool IsAdAvailable
        {
            get
            {
                return _appOpenAd != null && DateTime.Now < _expireTime;
            }
        }
        private void Update()
        {
            Debug.Log(IsLoadingCompleted);
        }
        protected override void Awake()
        {
            base.Awake();
            IsLoadingCompleted = false;
#if UNITY_ANDROID
            _adUnitIdTest = "ca-app-pub-3940256099942544/3419835294";
#elif UNITY_IOS
            _adUnitIdTest = "ca-app-pub-3940256099942544/5662855259";
#endif
            //AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }
        private void OnDestroy()
        {
            //AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }
        // Start is called before the first frame update
        public void Init()
        {
            _adsDataSetting = AdsDataSetting.LoadInstance();
#if SDK_ADMOB
            _isUseOpenAppAd = _adsDataSetting.AdsData.IsUseOpenApp;
#if UNITY_ANDROID
            _adUnitId = _adsDataSetting.AdsData.OpenAppIdAndroid;
#elif UNITY_IOS
            _adUnitId = _adsDataSetting.AdsData.OpenAppIdIos;
#endif
#else
            _isUseOpenAppAd = false;
            _adUnitId = _adUnitIdTest;
#endif
            if (_isTest)
            {
                _adUnitId = _adUnitIdTest;
            }
            if (_isUseOpenAppAd)
            {
                //ConsentInformation.Reset();
                //ConsentRequestParameters request = new ConsentRequestParameters
                //{
                //    TagForUnderAgeOfConsent = false,
                //};
                //if (_isTest)
                //{
                //    var debugSettings = new ConsentDebugSettings
                //    {
                //        DebugGeography = DebugGeography.EEA,
                //        TestDeviceHashedIds = new List<string> { "56808B6090D96B9146DA5A085879C8AE", }
                //    };
                //    request = new ConsentRequestParameters
                //    {
                //        TagForUnderAgeOfConsent = false,
                //        ConsentDebugSettings = debugSettings,
                //    };
                //}

                // Check the current consent information status.
                //ConsentInformation.Update(request, OnConsentInfoUpdated);
                Debug.LogWarning("OnConsentInfoUpdated Failed");
                RequestConfiguration requestConfiguration = new RequestConfiguration();
                requestConfiguration.TestDeviceIds.AddRange(_adsDataSetting.AdsData.AdSdkDataSetting.Android.TestDevicesID);
                MobileAds.SetRequestConfiguration(requestConfiguration);
                //UnityEngine.Debug.LogError(consentError);
                MobileAds.Initialize(initStatus =>
                {
                    // This callback is called once the MobileAds SDK is initialized.
                    LoadAppOpenAd();
                });
                IsShowGdprCompleted = true;
            }

        }

        //void OnConsentInfoUpdated(FormError consentError)
        //{
        //    if (consentError != null)
        //    {
        //        // Handle the error.
        //        Debug.LogWarning("OnConsentInfoUpdated Failed");
        //        RequestConfiguration requestConfiguration = new RequestConfiguration();
        //        requestConfiguration.TestDeviceIds.AddRange(_adsDataSetting.AdsData.AdSdkDataSetting.Android.TestDevicesID);
        //        MobileAds.SetRequestConfiguration(requestConfiguration);
        //        //UnityEngine.Debug.LogError(consentError);
        //        MobileAds.Initialize(initStatus =>
        //        {
        //            // This callback is called once the MobileAds SDK is initialized.
        //            LoadAppOpenAd();
        //        });
        //        IsShowGdprCompleted = true;
                
        //        return;
        //    }

        //    // If the error is null, the consent information state was updated.
        //    // You are now ready to check if a form is available.
        //    ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        //    {
        //        if (formError != null)
        //        {
        //            Debug.LogWarning("LoadAndShowConsentFormIfRequired");
        //            // Consent gathering failed.
        //            UnityEngine.Debug.LogError(consentError);
        //            IsShowGdprCompleted = true;
                    
        //            return;
        //        }

        //        // Consent has been gathered.
        //        if (ConsentInformation.CanRequestAds())
        //        {
        //            RequestConfiguration requestConfiguration = new RequestConfiguration();
        //            requestConfiguration.TestDeviceIds.AddRange(_adsDataSetting.AdsData.AdSdkDataSetting.Android.TestDevicesID);
        //            MobileAds.SetRequestConfiguration(requestConfiguration);
        //            MobileAds.Initialize(initStatus =>
        //            {
        //                // This callback is called once the MobileAds SDK is initialized.
        //                LoadAppOpenAd();
        //            });
        //        }
        //        IsShowGdprCompleted = true;
        //    });
        //}

        /// <summary>
        /// Loads the app open ad.
        /// </summary>
        public void LoadAppOpenAd()
        {
            // Clean up the old ad before loading a new one.
            if (_appOpenAd != null)
            {
                _appOpenAd.Destroy();
                _appOpenAd = null;
            }

            Debug.Log("Loading the app open ad.");

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // send the request to load the ad.
            AppOpenAd.Load(_adUnitId, adRequest,
                (AppOpenAd ad, LoadAdError error) =>
                {
                    Debug.Log("AOA app ID: "+ _adUnitId);
                    // if error is not null, the load request failed.
                    if (error != null || ad == null)
                    {
                        Debug.LogError("app open ad failed to load an ad " +
                                       "with error : " + error);
                        return;
                    }

                    Debug.Log("App open ad loaded with response : "
                    + ad.GetResponseInfo());
                    _appOpenAd = ad;
                    _expireTime = DateTime.Now + TimeSpan.FromHours(4);
                    RegisterEventHandlers(ad);
                });
        }

        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("App open ad paid {0} {1}.",
                adValue.Value,
                    adValue.CurrencyCode));

                ResponseInfo responseInfo = ad.GetResponseInfo();
                AdapterResponseInfo loadedAdapterResponseInfo = responseInfo.GetLoadedAdapterResponseInfo();
                double earnValue = adValue.Value / 1000000f;
                GameAnalyticManager.SendAdRevenue(MediationType.Admob, "Admob", "US", _adUnitId, "AOA", "AOA", earnValue);
                Debug.Log(string.Format("App open ad Event AdRevenue {0} USD", earnValue));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("App open ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("App open ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("App open ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("App open ad full screen content closed.");
                _showAdCompleted?.Invoke();
                LoadAppOpenAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("App open ad failed to open full screen content " +
                               "with error : " + error);
                LoadAppOpenAd();
            };
        }
        /// <summary>
        /// Shows the app open ad.
        /// </summary>
        public void ShowAppOpenAd(Action showAdCompleted)
        {
            _showAdCompleted = showAdCompleted;
            Debug.Log("ShowAppOpenAd");
            if (_isShowOpenAds)
            {
                if (_appOpenAd != null && _appOpenAd.CanShowAd())
                {
                    Debug.Log("Showing app open ad.");
                    _appOpenAd.Show();
                }
                else
                {
                    Debug.LogError("App open ad is not ready yet.");
                    _showAdCompleted?.Invoke();
                }
            }
            else
            {
                _showAdCompleted?.Invoke();
            }
        }
        //private void OnAppStateChanged(AppState state)
        //{
        //    Debug.Log("App State changed to : " + state);

        //    // if the app is Foregrounded and the ad is available, show it.
        //    if (state == AppState.Foreground)
        //    {
        //        if (IsLoadingCompleted && _isResumeAds)
        //            AdsManager.Instance.ShowInterstitialAdsResume("foreground");
        //    }
        //}

        private void OnApplicationPause(bool pause)
        {
            if(!pause)
            {
                if (IsLoadingCompleted && _isResumeAds)
                    AdsManager.Instance.ShowInterstitialAdsResume("foreground");
            }
        }

        public void UpdateSettingRemoteConfig()
        {
#if FIREBASE_REMOTE_CFG
            _isShowOpenAds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("show_open_ads").BooleanValue;
            _isResumeAds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("resume_ads").BooleanValue;
#else
            _isShowOpenAds = true;
            _isResumeAds = false;
#endif
        }
#else
    public void Init()
    {
        
    }
    public void ShowAppOpenAd(Action showAdCompleted)
    {
            _showAdCompleted = showAdCompleted;
    }
#endif
        }
    }

