using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AC.GameTool.Ads
{
    [System.Serializable]
    public class MaxPackageData
    {
        public MaxPluginData AppLovinMax;
    }

    [System.Serializable]
    public class MaxPluginData
    {
        public string Name;
        public string DisplayName;
        public string DownloadUrl;
        public string DependenciesFilePath;
        public string[] PluginFilePaths;
        public PackageVersionData LatestVersions;
    }

    [System.Serializable]
    public class PackageVersionData 
    {
        public string Unity;
        public string Android;
        public string Ios;
    }
}

