#if APPSFLYER_SDK
using AppsFlyerSDK;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupAppsFlyerManager : MonoBehaviour
#if APPSFLYER_SDK
    , IAppsFlyerConversionData
#endif
{
    [SerializeField]
    string _devkey;
    [SerializeField]
    string _appID;
    [SerializeField]
    bool isDebug;
    [SerializeField]
    bool getConversionData;


    // Start is called before the first frame update
    void Start()
    {
#if APPSFLYER_SDK
        AppsFlyer.setIsDebug(isDebug);
        AppsFlyer.initSDK(_devkey, _appID, getConversionData ? this: null);
        AppsFlyer.startSDK();
#if UNITY_IOS && !UNITY_EDITOR
        AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
#endif
#endif
#if APPSFLYER_ADREVENUE_ANALYTIC
        AppsFlyerAdRevenue.start();
        AppsFlyerAdRevenue.setIsDebug(isDebug);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#if APPSFLYER_SDK
    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("didReceiveConversionData", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        // add deferred deeplink logic here
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }
#endif
}
