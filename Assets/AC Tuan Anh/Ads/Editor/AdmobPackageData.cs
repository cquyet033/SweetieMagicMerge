using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.GameTool.Ads
{
    [System.Serializable]
    public class AdmobPackageData
    {
        public int id;
        public string name;
        public string url;
        public string assets_url;
        public string tag_name;
        public List<AdmobPackageDataAsset> assets;
    }
    [System.Serializable]
    public class AdmobPackageDataAsset
    {
        public int id;
        public string name;
        public string url;
        public string browser_download_url;
    }
}

