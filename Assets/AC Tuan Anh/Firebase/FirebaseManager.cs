//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using AC.Core;
//using System;
//#if FIREBASE_SDK
//using Firebase;
//using Firebase.Extensions;
//#endif
//using System.Threading.Tasks;
//using Firebase.RemoteConfig;
//using AC.Attribute;

//public class FirebaseManager : Singleton<FirebaseManager>
//{
//#if FIREBASE_SDK
//    //FirebaseApp _firebaseApp;
//    //public FirebaseApp FirebaseApp => _firebaseApp;
//    [SerializeField, ReadOnlly]
//    private bool _isInitCompleted;

//    [SerializeField, ReadOnlly]
//    private bool _isFirebaseReady;
//    public bool IsFirebaseInitCompleted => _isInitCompleted;

//    public bool IsLoadAllRemoteData => _isFirebaseReady;
//    protected override void Awake()
//    {
//        base.Awake();
//    }
//    // Start is called before the first frame update
//    void Start()
//    {
//        //FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener += ConfigUpdateListenerEventHandler;
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    void OnDestroy()
//    {
//        //FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener
//          //-= ConfigUpdateListenerEventHandler;
//    }
//    public void Init()
//    {
//        #if FIREBASE_SDK
//        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//        {
//            var dependencyStatus = task.Result;
//            if (dependencyStatus == DependencyStatus.Available)
//            {
//                // Create and hold a reference to your FirebaseApp,
//                // where app is a Firebase.FirebaseApp property of your application class.
//                //_firebaseApp = FirebaseApp.DefaultInstance;

//                // Set a flag here to indicate whether Firebase is ready to use by your app.
                
//                SetRemoteConfigDefault();
//            }
//            else
//            {
//                UnityEngine.Debug.LogError(System.String.Format(
//                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//                // Firebase Unity SDK is not safe to use here.
//                _isInitCompleted = true;
//            }
//        });
//#endif
//    }
//#if FIREBASE_REMOTE_CFG
//    void SetRemoteConfigDefault()
//    {
//        Dictionary<string, object> defaults = new Dictionary<string, object>();

//        // These are the values that are used if we haven't fetched data from the
//        // server
//        // yet, or if we ask for values that the server doesn't have:
//        defaults.Add("ads_interval", 30);
//        defaults.Add("resume_ads", false);
//        defaults.Add("rating_popup", true);
//        defaults.Add("show_open_ads", true);
//        defaults.Add("show_open_ads_first_open", false);

//        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task=> {

//            //_isInitSuccessed = true;
//            FetchDataAsync();
//        });

//    }
//    public Task FetchDataAsync()
//    {
//        Debug.Log("Fetching data...");
//        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(
//            TimeSpan.Zero);
//        return fetchTask.ContinueWithOnMainThread(FetchComplete);
//    }
//    private void FetchComplete(Task fetchTask)
//    {
//        if (!fetchTask.IsCompleted)
//        {
//            Debug.LogError("Retrieval hasn't finished.");
//            _isInitCompleted = true;
//            _isFirebaseReady = false;
//            return;
//        }

//        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
//        var info = remoteConfig.Info;
//        if (info.LastFetchStatus != LastFetchStatus.Success)
//        {
//            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
//            _isInitCompleted = true;
//            _isFirebaseReady = false;
//            return;
//        }

//        // Fetch successful. Parameter values must be activated to use.
//        remoteConfig.ActivateAsync()
//          .ContinueWithOnMainThread(
//            task => {
//                _isInitCompleted = true;
//                _isFirebaseReady = true;
//                Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
//            });
//    }
//#endif
//    // Handle real-time Remote Config events.
//    //void ConfigUpdateListenerEventHandler(object sender, ConfigUpdateEventArgs args)
//    //{
//    //    if (args.Error != RemoteConfigError.None)
//    //    {
//    //        Debug.Log(string.Format("Error occurred while listening: {0}", args.Error));
//    //        return;
//    //    }
//    //    var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
//    //    Debug.Log("Updated keys: " + string.Join(", ", args.UpdatedKeys));
//    //    // Activate all fetched values and then display a welcome message.
//    //    remoteConfig.ActivateAsync().ContinueWithOnMainThread(tast => {
//    //        Debug.Log($"Remote data updated.");
//    //    });
//    //}
//#endif
//}
