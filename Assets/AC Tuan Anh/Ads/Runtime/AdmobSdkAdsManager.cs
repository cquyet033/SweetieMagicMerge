#if SDK_ADMOB && USE_ADMOB

using AC.Attribute;
using AC.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.GameTool.Ads
{
    public class AdmobSdkAdsManager : Singleton<AdmobSdkAdsManager>, IBaseAdsManager
    {
        [Header("Admob SDK:")]
        [SerializeField, ReadOnlly] protected AdsData _adData;
        [SerializeField, ReadOnlly]
        protected CheckLoadCompleted _completedChecking = new CheckLoadCompleted();
        
        public CheckLoadCompleted CompletedChecking => _completedChecking;

        CheckLoadCompleted IBaseAdsManager.CompletedChecking => throw new NotImplementedException();

        protected override void Awake()
        {
            base.Awake();
            _adData = AdsDataSetting.LoadInstance().AdsData;
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
            
        }

        public void ShowBanner(bool isShow)
        {
            
        }

        public void ShowInterstitialAds(string placement, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            
        }

        public void ShowInterstitialAdsResume(string placement, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            
        }

        public void ShowRewardAds(string placement, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            
        }
    }
}

#endif