//using UnityEngine;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityToolbarExtender;
//using System.IO;
//using UnityEditor.AddressableAssets.Settings;
//using System.Collections.Generic;
//using UnityEditor.AddressableAssets;


//[InitializeOnLoad]
//public class StartupSettings
//{
//    public const string k_PreferenceSection = "Editor";
//    const string k_LockStartupSceneKey = "LockStartupScene";

//    static bool LockStartupScene
//    {
//        get
//        {
//            if (!PlayerPrefs.HasKey(k_LockStartupSceneKey))
//            {
//                PlayerPrefs.SetInt(k_LockStartupSceneKey, 1);
//            }
//            return PlayerPrefs.GetInt(k_LockStartupSceneKey,1) > 0;
//        }
//        set => PlayerPrefs.SetInt(k_LockStartupSceneKey, value ? 1 : 0);
//    }


//    static StartupSettings()
//    {
//        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarLeftGUI);
//        ToolbarExtender.RightToolbarGUI.Add(OnToolbarRightGUI);
//        EditorApplication.delayCall += SetStartupScene;
//    }

    
//    static void OnToolbarLeftGUI()
//    {
//        GUILayout.FlexibleSpace();

//        var lockStartup = LockStartupScene;
//        if (EditorBuildSettings.scenes.Length > 0)
//        {
//            string sceneName = Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[0].path);
//            lockStartup = GUILayout.Toggle(lockStartup, new GUIContent(string.Format("Start {0} scene", sceneName), null,string.Format("Always start play mode with {0} scene", sceneName)));
//            LockStartupScene = lockStartup;
//            if (lockStartup)
//            {
//                SetStartupScene();
//            }
//            else
//            {
//                UnsetStartupScene();
//            }
//        }       
//    }

//    private const string UnknownString = "<-unknown->";
//    static void OnToolbarRightGUI()
//    {
//        GUILayout.Label("Open Scene:");
//        List<AddressableAssetEntry> listAsset = GetAllSceneEntrys();
//        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
//        List<string> listScenePath = new List<string>();
//        List<string> listNameScene = new List<string>();
//        string currentScenePath = EditorSceneManager.GetActiveScene().path;

//        for (int i=0; i< scenes.Length; i++)
//        {
//            string pathScene = scenes[i].path;
//            listScenePath.Add(pathScene);
//            string nameScene = Path.GetFileNameWithoutExtension(pathScene);
//            listNameScene.Add(nameScene);
//        }
//        for(int i=0; i< listAsset.Count; i++)
//        {
//            string pathAsset = listAsset[i].AssetPath;
//            if (!listScenePath.Contains(pathAsset))
//            {
//                listScenePath.Add(pathAsset);
//                string nameScene = Path.GetFileNameWithoutExtension(pathAsset);
//                listNameScene.Add(nameScene);
//            }
//        }

//        int currentIndex = listScenePath.FindIndex(path => path == currentScenePath);

//        currentIndex = currentIndex < 0 ? 0 : currentIndex;
//        listNameScene.Add(UnknownString);
//        var sceneIndex = EditorGUILayout.Popup(
//                currentIndex, listNameScene.ToArray(), GUILayout.Width(150));
//        //Debug.Log(currentScenePath);
//        if (sceneIndex != currentIndex && sceneIndex < listScenePath.Count)
//        {
//            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
//            {
//                EditorSceneManager.OpenScene(listScenePath[sceneIndex]);
//            }
//        }
//        GUILayout.FlexibleSpace();
//    }



//    #region Left Methord
//    static void SetStartupScene()
//    {
//        if (LockStartupScene)
//        {
//            if(EditorBuildSettings.scenes.Length > 0)
//            {
//                var scenePath = EditorBuildSettings.scenes[0].path;

//                if (!string.IsNullOrEmpty(scenePath))
//                {
//                    SetPlayModeStartScene(scenePath);
//                }
//            }            
//        }
//    }

//    static void UnsetStartupScene()
//    {
//        EditorSceneManager.playModeStartScene = null;
//    }

//    static void SetPlayModeStartScene(string scenePath)
//    {
//        SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
//        if (myWantedStartScene != null)
//            EditorSceneManager.playModeStartScene = myWantedStartScene;
//        else
//            Debug.LogError("Could not find Scene " + scenePath);
//    }
//    #endregion

//    #region Right Methord
//    public static List<AddressableAssetEntry> GetAllSceneEntrys()
//    {
//        var setting = AddressableAssetSettingsDefaultObject.GetSettings(false);
//        var entrys = new List<AddressableAssetEntry>();

//        if (setting != default)
//        {
//            foreach (var group in setting.groups)
//            {
//                var _entrys = new List<AddressableAssetEntry>();
//                group.GatherAllAssets(_entrys, true, true, true, e=>e.IsScene);
//                entrys.AddRange(_entrys);
//            }
//        }
//        return entrys;
//    }
//    #endregion
//}

