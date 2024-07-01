using AC.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.GameTool.Ads
{
    public interface IBaseAdsManager
    {
        CheckLoadCompleted CompletedChecking { get; }
        void Init();
        void ShowBanner(bool isShow);

        void ShowInterstitialAds(string placement, Action successed, Action<AdsErrorCode> failed);
        void ShowInterstitialAdsResume(string placement, Action successed, Action<AdsErrorCode> failed);

        void ShowRewardAds(string placement, Action successed, Action<AdsErrorCode> failed);
        //void ShowInterstitialRewardAds(string placement, Action successed, Action<string> failed);
    }
}

