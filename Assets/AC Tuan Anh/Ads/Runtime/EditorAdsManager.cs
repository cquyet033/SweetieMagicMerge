using AC.Attribute;
using AC.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.GameTool.Ads
{
    public abstract class EditorAdsManager : Singleton<EditorAdsManager>, IBaseAdsManager
    {
        [Header("Editor SDK:")]
        [SerializeField, ReadOnlly]
        protected CheckLoadCompleted _completedChecking = new CheckLoadCompleted();

   
        public CheckLoadCompleted CompletedChecking => _completedChecking;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public virtual void Init()
        {
            this._completedChecking.IsLoadCompleted = true;
            Debug.Log("Init Ads Editor Successed");
        }

        public virtual void ShowBanner(bool isShow)
        {
            Debug.Log(string.Format("*Editor* Show Banner: {0} (Editor is Hide Banner)", isShow));
        }

        public virtual void ShowInterstitialAds(string placement = null, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            Debug.Log(string.Format("Show Interstitial Ads in {0} Successed.", placement));
            successed?.Invoke();
        }
        public virtual void ShowInterstitialAdsResume(string placement = null, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            Debug.Log(string.Format("Show Interstitial Ads in {0} Successed.", placement));
            successed?.Invoke();
        }

        public virtual void ShowInterstitialRewardAds(string placement = null, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            Debug.Log(string.Format("Show Interstitial Reward Ads in {0} Successed.", placement));
            successed?.Invoke();
        }

        public virtual void ShowRewardAds(string placement = null, Action successed = null, Action<AdsErrorCode> failed = null)
        {
            Debug.Log(string.Format("Show Reward Ads in {0} Successed.", placement));
            successed?.Invoke();
        }

        public void UpdateIntervalAd()
        {

        }
    }
}

