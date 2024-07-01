using AC.Attribute;
using AC.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.GameTool.Ads
{
    public class AdsManager :
#if USE_ADMOB
    AdmobSdkAdsManager
#elif USE_MAX
    MaxSdkAdsManager
#else
    EditorAdsManager
#endif
    {
        
        protected override void Awake()
        {
            base.Awake();
            
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public enum AdsErrorCode
    {
        Unknow,
        NoVideoAds,
        DisplayFaile,
        NoInternet
    }
}
