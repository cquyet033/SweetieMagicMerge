using AC.Core;
#if APPSFLYER_SDK
using AppsFlyerSDK;
#endif
#if FIREBASE_ANALYTIC
using Firebase.Analytics;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameAnalyticManager 
{
    public static void SendAdRevenue(MediationType mediationType,string networdName,string country, string adUnitID, string adsFormat, string placement, double value)
    {
        Dictionary<string, string> additionalParams = new Dictionary<string, string>();
#if APPSFLYER_ADREVENUE_ANALYTIC
        additionalParams.Add(AFAdRevenueEvent.COUNTRY, country);
        additionalParams.Add(AFAdRevenueEvent.AD_UNIT, adUnitID);
        additionalParams.Add(AFAdRevenueEvent.AD_TYPE, adsFormat);
        additionalParams.Add(AFAdRevenueEvent.PLACEMENT, placement);
        additionalParams.Add(AFAdRevenueEvent.ECPM_PAYLOAD, "encrypt");
        string adPlatform = string.Empty;
        AppsFlyerAdRevenueMediationNetworkType type = AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob;
        switch (mediationType)
        {
            case MediationType.Admob:
                adPlatform = "AdMob";
                type = AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob;
                break;
            case MediationType.Max:
                adPlatform = "Applovin";
                type = AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax;
                break;
        }
        AppsFlyerAdRevenue.logAdRevenue(adPlatform, type, value, "USD", additionalParams);
#endif
#if FIREBASE_ANALYTIC
        Parameter[] impressionParameters = new[] {
            new Parameter("ad_platform", adPlatform),
            new Parameter("ad_source", networdName),
            new Parameter("ad_unit_name", adUnitID),
            new Parameter("ad_format", adsFormat),
            new Parameter("value", value),
            new Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        };       
        FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
#endif
    }

    public static void SendStartLevel(int levelIndex)
    {
        if (levelIndex >= 100) return;
#if FIREBASE_ANALYTIC
        FirebaseAnalytics.LogEvent(string.Format("start_level_{0}", levelIndex + 1));
#endif
        LogManager.Log(string.Format("start_level_{0}", levelIndex + 1));
    }
    public static void SendWinLevel(int levelIndex)
    {
        if (levelIndex >= 100) return;
#if FIREBASE_ANALYTIC
        FirebaseAnalytics.LogEvent(string.Format("win_level_{0}", levelIndex + 1));
#endif
#if APPSFLYER_SDK
        if(levelIndex < 20)
            AppsFlyer.sendEvent(string.Format("completed_level_{0}", levelIndex + 1), null);
#endif
        LogManager.Log(string.Format("win_level_{0}", levelIndex + 1));
    }

    public static void SendButtonAdInterClick()
    {
#if APPSFLYER_SDK
        AppsFlyer.sendEvent("af_inters_ad_eligible", null);
#endif
    }
    public static void SendAdInterLoaded()
    {
#if APPSFLYER_SDK
        AppsFlyer.sendEvent("af_inters_api_called", null);
#endif
    }

    public static void SendAdInterShowCount(int count)
    {
        if (count > 20) return;
#if APPSFLYER_SDK
        AppsFlyer.sendEvent(string.Format("af_inters_displayed_{0}", count), null);
#endif
    }
    public static void SendAdInterDisplay()
    {
#if APPSFLYER_SDK
        AppsFlyer.sendEvent("af_inters_displayed", null);
#endif
    }
    public static void SendButtonAdRewardClick()
    {
#if APPSFLYER_SDK
        AppsFlyer.sendEvent("af_rewarded_ad_eligible", null);
#endif
    }
    public static void SendAdRewardLoaded()
    {
#if APPSFLYER_SDK
        AppsFlyer.sendEvent("af_rewarded_api_called", null);
#endif
    }
    public static void SendAdRewardDisplay()
    { 
#if APPSFLYER_SDK
        AppsFlyer.sendEvent("af_rewarded_ad_displayed", null);
#endif
    }
}

public enum MediationType
{
    Admob,
    Max
}
