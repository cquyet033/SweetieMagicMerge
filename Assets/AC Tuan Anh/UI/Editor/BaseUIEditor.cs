using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace AC.GameTool.UI
{
    [CustomEditor(typeof(BaseUI), true)]
    public class BaseUIEditor : Editor
    {
        [SerializeField] TextAsset _textUiInfo;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            BaseUI baseUI = (BaseUI)target;
            EditorGUILayout.HelpBox("-Bam Auto Referenced de tu dong tham chieu den cac thanh phan trong UI.\n-Bam Save UI de luu lai UI", MessageType.Info);
            if (GUILayout.Button("Auto Referenced"))
            {
                baseUI.AutoReferencedInUI();
            }
            if (GUILayout.Button("Save UI"))
            {
                baseUI.SaveUI(_textUiInfo);
            }
        }
    }
}

