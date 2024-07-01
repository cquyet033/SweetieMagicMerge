using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AC.Core
{
    [InitializeOnLoadAttribute]
    public static class CheckSDKInProject
    {
        static CheckSDKInProject()
        {
            EditorApplication.projectChanged += CheckAllSdkInstall;
        }

        private static void CheckAllSdkInstall()
        {
            //LogManager.Log("CheckAllSdkInstall");
            CheckGoogleAdmobExist();
            CheckMaxExist();
            CheckFirebaseSDK();
            CheckRemoteConfig();
            CheckFirebaseAnalytic();
            CheckAppsFlyerAnalytic();
        }

        static void AddDefineToSetting(string defineName)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            ConditionalCompilationUtils.AddDefineIfNecessary(defineName, buildTargetGroup);
        }
        static void RemoveDefineFromSetting(string defineName)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            ConditionalCompilationUtils.RemoveDefineIfNecessary(defineName, buildTargetGroup);
        }

        #region Check Exist SDK
        public static bool CheckGoogleAdmobExist()
        {
            bool admobExist = FindFileInProject.DoesDLLExist("GoogleMobileAds.dll", Application.dataPath);
            if (admobExist)
            {
                AddDefineToSetting("SDK_ADMOB");
            }
            else
            {
                RemoveDefineFromSetting("SDK_ADMOB");
            }
            return admobExist;
        }
        public static bool CheckMaxExist()
        {
            bool maxExist = FindFileInProject.DoesScriptExist("MaxSdk");
            if (maxExist)
            {
                AddDefineToSetting("SDK_MAX");
            }
            else
            {
                RemoveDefineFromSetting("SDK_MAX");
            }
            return maxExist;
        }

        public static bool CheckFirebaseSDK()
        {
            bool remotecfgExist = FindFileInProject.DoesDLLExist("Firebase.App.dll", Application.dataPath);
            if (!remotecfgExist)
            {
                remotecfgExist = FindFileInProject.IsPackageInstalled("com.google.firebase.app");
            }
            if (remotecfgExist)
            {
                AddDefineToSetting("FIREBASE_SDK");
            }
            else
            {
                RemoveDefineFromSetting("FIREBASE_SDK");
            }
            return remotecfgExist;
        }
        public static bool CheckRemoteConfig()
        {
            bool remotecfgExist = FindFileInProject.DoesDLLExist("Firebase.RemoteConfig.dll", Application.dataPath);
            if (!remotecfgExist)
            {
                remotecfgExist = FindFileInProject.IsPackageInstalled("com.google.firebase.remote-config");
            }
            if (remotecfgExist)
            {
                AddDefineToSetting("FIREBASE_REMOTE_CFG");
            }
            else
            {
                RemoveDefineFromSetting("FIREBASE_REMOTE_CFG");
            }
            return remotecfgExist;
        }
        public static bool CheckFirebaseAnalytic()
        {
            bool firebaseAnalyticExist = FindFileInProject.DoesDLLExist("Firebase.Analytics.dll", Application.dataPath);
            if (!firebaseAnalyticExist)
            {
                firebaseAnalyticExist = FindFileInProject.IsPackageInstalled("com.google.firebase.analytics");
            }
            if (firebaseAnalyticExist)
            {
                AddDefineToSetting("FIREBASE_ANALYTIC");
            }
            else
            {
                RemoveDefineFromSetting("FIREBASE_ANALYTIC");
            }
            return firebaseAnalyticExist;
        }
        public static bool CheckAppsFlyerAnalytic()
        {
            bool appsflyerExist = FindFileInProject.DoesScriptExist("AppsFlyer");
            if (!appsflyerExist)
            {
                appsflyerExist = FindFileInProject.IsPackageInstalled("appsflyer-unity-plugin");
            }
            if (appsflyerExist)
            {
                AddDefineToSetting("APPSFLYER_SDK");
            }
            else
            {
                RemoveDefineFromSetting("APPSFLYER_SDK");
            }
            CheckAppsFlyerAdrevenue();
            return appsflyerExist;
        }

        static void CheckAppsFlyerAdrevenue()
        {
            bool appsflyerAdrevenue = FindFileInProject.DoesScriptExist("AppsFlyerAdRevenue");
            if (!appsflyerAdrevenue)
            {
                appsflyerAdrevenue = FindFileInProject.IsPackageInstalled("appsflyer-unity-adrevenue-generic-connector");
            }
            if (appsflyerAdrevenue)
            {
                AddDefineToSetting("APPSFLYER_ADREVENUE_ANALYTIC");
            }
            else
            {
                RemoveDefineFromSetting("APPSFLYER_ADREVENUE_ANALYTIC");
            }
        }

        #endregion
    }
}

