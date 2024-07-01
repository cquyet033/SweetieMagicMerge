using AC.Attribute;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AC.GameTool.UI
{   
    public abstract class BaseUI : MonoBehaviour
    {

        [SerializeField]
        public abstract UILayer UILayer { get; }
        public bool IsMultiUI;
        protected Action _onOpen, _onClose;
        [Header("Transition")]
        [SerializeField]
        protected CanvasGroup _mainCanvasGroup;
        [SerializeField]
        protected TransitionType _transitionTypeOpen = TransitionType.None;
        [SerializeField]
        protected Ease _easeTypeOpen = Ease.Linear;
        [SerializeField]
        protected TransitionType _transitionTypeClose = TransitionType.None;
        [SerializeField]
        protected Ease _easeTypeClose = Ease.Linear;
        [SerializeField]
        protected float _transDuration = 1f;
        [SerializeField, ReadOnlly]
        RectTransform _rectMainCanvasGroup;
        [SerializeField, ReadOnlly]
        Vector2 _mainCanvasGroupAnchoPos;
        Vector2 _anchoPosLeft, _anchoPosRight, _anchoPosTop, _anchoPosBottom;

        protected virtual void Awake()
        {
            if(_mainCanvasGroup != null)
            {
                _rectMainCanvasGroup = _mainCanvasGroup.GetComponent<RectTransform>();
                _mainCanvasGroupAnchoPos = _rectMainCanvasGroup.anchoredPosition;
                _anchoPosLeft = new Vector2(-Screen.width, _mainCanvasGroupAnchoPos.y);
                _anchoPosRight = new Vector2(Screen.width, _mainCanvasGroupAnchoPos.y);
                _anchoPosTop = new Vector2(_mainCanvasGroupAnchoPos.x, Screen.height);
                _anchoPosBottom = new Vector2(_mainCanvasGroupAnchoPos.x, -Screen.height);
            }           
        }
        private void OnDestroy()
        {
            RemoveAllCallback();
        }

        public void SetCallback(Action openCallback, Action closeCallback)
        {
            _onOpen += openCallback;
            _onClose += closeCallback;
        }

        public virtual void OnUiShow()
        {
            gameObject.SetActive(true);
            _onOpen?.Invoke();
            RunTransitionShowUI(true, _transitionTypeOpen, _easeTypeOpen, null);
        }

        public virtual void OnCloseUI()
        {
            _onClose?.Invoke();
            RunTransitionShowUI(false, _transitionTypeClose, _easeTypeClose, () =>  gameObject.SetActive(false));
        }

        public void RemoveAllCallback()
        {
            _onOpen = null;
            _onClose = null;
        } 

        public void SetTransitionType(TransitionType transTypeOpen, Ease easeTypeOpen, TransitionType transTypeClose, Ease easeTypeClose, float duration)
        {
            _transitionTypeOpen = transTypeOpen;
            _easeTypeOpen = easeTypeOpen;
            _transitionTypeClose = transTypeClose;
            _easeTypeClose = easeTypeClose;
            _transDuration = duration;
        }
        public void SetTransitionType(TransitionType transType, Ease easeType, float duration)
        {
            _transitionTypeOpen = transType;
            _easeTypeOpen = easeType;
            _transitionTypeClose = transType;
            _easeTypeClose = easeType;
            _transDuration = duration;
        }
        void RunTransitionShowUI(bool isShow, TransitionType transitionType, Ease ease, Action complete)
        {
            if (_mainCanvasGroup == null)
            {
                complete?.Invoke();
                return;
            }
            switch (transitionType)
            {
                case TransitionType.None:
                    ResetCanvasGroup();
                    complete?.Invoke();
                    break;
                case TransitionType.Fade:
                    ResetCanvasGroup();
                    
                    float start = isShow ? 0f : 1f;
                    float end = isShow ? 1f : 0f;
                    FadeCanvasGroup(start, end, _transDuration, ease, complete);
                    break;
                case TransitionType.Pop:
                    ResetCanvasGroup();
                    Vector3 startScale = isShow ? Vector3.zero : Vector3.one;
                    Vector3 endScale = isShow ? Vector3.one : Vector3.zero;
                    PopTransition(startScale, endScale, _transDuration, ease, complete);
                    break;
                case TransitionType.MoveFromLeft:                    
                    ResetCanvasGroup();
                    Vector2 startMoveLeft = isShow ? _anchoPosLeft : _mainCanvasGroupAnchoPos;
                    Vector2 startEndLeft = isShow ? _mainCanvasGroupAnchoPos : _anchoPosLeft;
                    MoveTransition(startMoveLeft, startEndLeft, _transDuration, ease, complete);
                    break;
                case TransitionType.MoveFromRight:
                    ResetCanvasGroup();
                    Vector2 startMoveRight = isShow ? _anchoPosRight : _mainCanvasGroupAnchoPos;
                    Vector2 startEndRight = isShow ? _mainCanvasGroupAnchoPos : _anchoPosRight;
                    MoveTransition(startMoveRight, startEndRight, _transDuration, ease, complete);
                    break;
                case TransitionType.MoveFromTop:
                    ResetCanvasGroup();
                    Vector2 startMoveTop = isShow ? _anchoPosTop : _mainCanvasGroupAnchoPos;
                    Vector2 startEndTop = isShow ? _mainCanvasGroupAnchoPos : _anchoPosTop;
                    MoveTransition(startMoveTop, startEndTop, _transDuration, ease, complete);
                    break;
                case TransitionType.MoveFromBottom:
                    ResetCanvasGroup();
                    Vector2 startMoveBottom = isShow ? _anchoPosBottom : _mainCanvasGroupAnchoPos;
                    Vector2 startEndBottom = isShow ? _mainCanvasGroupAnchoPos : _anchoPosBottom;
                    MoveTransition(startMoveBottom, startEndBottom, _transDuration, ease, complete);
                    break;
            }
        }
        
        void ResetCanvasGroup()
        {
            if (_mainCanvasGroup == null) return;
            _mainCanvasGroup.blocksRaycasts = true;
            _mainCanvasGroup.DOKill();
            _mainCanvasGroup.alpha = 1;
            _rectMainCanvasGroup.anchoredPosition = _mainCanvasGroupAnchoPos;
        }

        void FadeCanvasGroup(float start, float end, float duration, Ease ease, Action complete)
        {
            if (_mainCanvasGroup == null) return;
            _mainCanvasGroup.blocksRaycasts = false;
            _mainCanvasGroup.alpha = start;
            _mainCanvasGroup.DOFade(end, duration).SetEase(ease).OnComplete(()=> { _mainCanvasGroup.alpha = end; _mainCanvasGroup.blocksRaycasts = true; complete?.Invoke();  });
        }

        void PopTransition(Vector3 startScale, Vector3 endScale, float duration, Ease ease, Action complete)
        {
            if (_mainCanvasGroup == null) return;
            _mainCanvasGroup.blocksRaycasts = false;
            _mainCanvasGroup.transform.localScale = startScale;
            _mainCanvasGroup.transform.DOScale(endScale, duration).SetEase(ease).OnComplete(()=> { _mainCanvasGroup.transform.localScale = endScale; _mainCanvasGroup.blocksRaycasts = true; complete?.Invoke(); });
        }

        void MoveTransition(Vector2 startPos, Vector2 endPos, float duration, Ease ease, Action complete)
        {
            if (_rectMainCanvasGroup == null) return;
            _mainCanvasGroup.blocksRaycasts = false;
            _rectMainCanvasGroup.anchoredPosition = startPos;
            _rectMainCanvasGroup.DOAnchorPos(endPos, duration).SetEase(ease).OnComplete(()=> { _rectMainCanvasGroup.anchoredPosition = endPos; _mainCanvasGroup.blocksRaycasts = true; complete?.Invoke(); });
        }
        
#if UNITY_EDITOR
        public void AutoReferencedInUI()
        {
            FieldInfo[] listFields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in listFields)
            {
                System.Attribute serializeFieldFlag = field.GetCustomAttribute(typeof(SerializeField), false);
                System.Attribute readOnllyFlag = field.GetCustomAttribute(typeof(ReadOnllyAttribute), false);
                System.Attribute hideInInspectorFlag = field.GetCustomAttribute(typeof(HideInInspector), false);
                if (((field.IsPrivate && serializeFieldFlag != null) || (field.IsPublic && hideInInspectorFlag == null)) && readOnllyFlag == null)
                {
                    if (field.FieldType.IsArray)
                    {
                        Type type = field.FieldType.GetElementType();
                        if (type == typeof(GameObject))
                        {
                            Transform[] listChild = transform.GetComponentsInChildren<Transform>(true);
                            List<GameObject> listObjResult = new List<GameObject>();
                            string nameField = field.Name.ToUpper();
                            nameField = nameField[nameField.Length - 1] == 'S' ? nameField.Substring(0, nameField.Length - 1) : nameField;
                            nameField = nameField.Replace("LIST", "");
                            for (int i = 0; i < listChild.Length; i++)
                            {
                                string nameChild = listChild[i].name.ToUpper().Replace(" ", "");
                                //nameChild = nameChild[nameChild.Length - 1] == 's' ? nameChild.Substring(0, nameChild.Length -1) : nameChild;
                                if (nameChild.IndexOf(nameField) == 0 || ("_" + nameChild).IndexOf(nameField) == 0)
                                {
                                    listObjResult.Add(listChild[i].gameObject);
                                }
                            }
                            Array instanceArr = Array.CreateInstance(type, listObjResult.Count);
                            for (int i = 0; i < listObjResult.Count; i++)
                            {
                                instanceArr.SetValue(listObjResult[i], i);
                            }
                            if (instanceArr.Length > 0)
                            {
                                field.SetValue(this, instanceArr);
                            }
                        }
                        else
                        {

                            if (type.IsSubclassOf(typeof(Component)))
                            {
                                Component[] listChild = transform.GetComponentsInChildren(type, true);
                                List<Component> listObjResult = new List<Component>();
                                string nameField = field.Name.ToUpper();
                                nameField = nameField[nameField.Length - 1] == 'S' ? nameField.Substring(0, nameField.Length - 1) : nameField;
                                nameField = nameField.Replace("LIST", "");
                                for (int i = 0; i < listChild.Length; i++)
                                {
                                    string nameChild = listChild[i].name.ToUpper().Replace(" ", "");
                                    if (nameChild.IndexOf(nameField) == 0 || ("_" + nameChild).IndexOf(nameField) == 0)
                                    {
                                        listObjResult.Add(listChild[i]);
                                    }
                                }
                                Array instanceArr = Array.CreateInstance(type, listObjResult.Count);
                                for (int i = 0; i < listObjResult.Count; i++)
                                {
                                    instanceArr.SetValue(listObjResult[i], i);
                                }
                                if (instanceArr.Length > 0)
                                {
                                    field.SetValue(this, instanceArr);
                                }
                            }
                        }
                    }
                    else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        //Debug.Log("List: " + field.FieldType.GetGenericArguments()[0]);
                        Type type = field.FieldType.GetGenericArguments()[0];
                        if (type == typeof(GameObject))
                        {
                            var instancedList = (IList)typeof(List<>).MakeGenericType(type).GetConstructor(System.Type.EmptyTypes).Invoke(null);
                            Transform[] listChild = transform.GetComponentsInChildren<Transform>(true);
                            string nameField = field.Name.ToUpper();
                            nameField = nameField[nameField.Length - 1] == 'S' ? nameField.Substring(0, nameField.Length - 1) : nameField;
                            nameField = nameField.Replace("LIST", "");
                            for (int i = 0; i < listChild.Length; i++)
                            {
                                string nameChild = listChild[i].name.ToUpper().Replace(" ", "");
                                //nameChild = nameChild[nameChild.Length - 1] == 's' ? nameChild.Substring(0, nameChild.Length - 1) : nameChild;
                                if (nameChild.IndexOf(nameField) == 0 || ("_" + nameChild).IndexOf(nameField) == 0)
                                {
                                    instancedList.Add(listChild[i].gameObject);
                                }
                            }
                            if (instancedList.Count > 0)
                            {
                                field.SetValue(this, instancedList);
                            }
                        }
                        else
                        {

                            if (type.IsSubclassOf(typeof(Component)))
                            {
                                Component[] listChild = transform.GetComponentsInChildren(type, true);
                                var instancedList = (IList)typeof(List<>).MakeGenericType(type).GetConstructor(System.Type.EmptyTypes).Invoke(null);
                                string nameField = field.Name.ToUpper();
                                nameField = nameField[nameField.Length - 1] == 'S' ? nameField.Substring(0, nameField.Length - 1) : nameField;
                                nameField = nameField.Replace("LIST", "");
                                for (int i = 0; i < listChild.Length; i++)
                                {
                                    string nameChild = listChild[i].name.ToUpper().Replace(" ", "");
                                    if (nameChild.IndexOf(nameField) == 0 || ("_" + nameChild).IndexOf(nameField) == 0)
                                    {
                                        instancedList.Add(listChild[i]);
                                    }
                                }
                                if (instancedList.Count > 0)
                                {
                                    field.SetValue(this, instancedList);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (field.FieldType == typeof(GameObject))
                        {
                            Transform[] listChild = transform.GetComponentsInChildren<Transform>(true);
                            for (int i = 0; i < listChild.Length; i++)
                            {
                                string nameChild = listChild[i].name.ToUpper().Replace(" ", "");
                                if (field.Name.ToUpper().Equals(nameChild) || field.Name.ToUpper().Equals(("_" + nameChild)))
                                {
                                    field.SetValue(this, listChild[i].gameObject);
                                    break;
                                }
                            }
                        }
                        else
                        {

                            if (field.FieldType.IsSubclassOf(typeof(Component)))
                            {
                                Type type = field.FieldType;
                                Component[] listChild = transform.GetComponentsInChildren(type, true);
                                for (int i = 0; i < listChild.Length; i++)
                                {
                                    string nameChild = listChild[i].name.ToUpper().Replace(" ", "");
                                    if (field.Name.ToUpper().Equals(nameChild) || field.Name.ToUpper().Equals(("_" + nameChild)))
                                    {
                                        field.SetValue(this, listChild[i]);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            EditorUtility.SetDirty(this);
        }

        public void SaveUI(TextAsset textUiInfo)
        {
            string pathUIResourceFolde = "Game/Prefabs/UI/" + UILayer.ToString();
            string path = Path.Combine(Application.dataPath, pathUIResourceFolde);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string uiPath = "Assets/" + pathUIResourceFolde + "/" + gameObject.name + ".prefab";
            uiPath = AssetDatabase.GenerateUniqueAssetPath(uiPath);
            bool saveSuccessed;
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, uiPath, InteractionMode.UserAction, out saveSuccessed);
            /*if (saveSuccessed)
            {
                string textUIInfo = textUiInfo.text;
                string filePath = uiPath.Replace("Assets/Resources/", "").Replace(".prefab", "");
                string nameOffset = string.Empty;
                switch (UILayer)
                {
                    case UILayer.Normal:
                        nameOffset = "UI";
                        break;
                    case UILayer.Popup:
                        nameOffset = "Popup";
                        break;
                    case UILayer.AlwaysTop:
                        nameOffset = "OnTop";
                        break;
                }
                string fileName = filePath.Replace("UI/" + UILayer.ToString() + "/", "").Replace(" ", "");
                string newUIToUiInfo = string.Format("public const string {0} = \"{1}\";", fileName + "_" + nameOffset, filePath);
                textUIInfo = textUIInfo.Replace("//#$#AddNewUI#$#", newUIToUiInfo + "\r\n\t\t//#$#AddNewUI#$#");
                Debug.Log(textUIInfo);
                string pathClassInfo = Path.Combine(Application.dataPath, "AC Tuan Anh/UI/Runtime/UIUtility.cs");
                File.WriteAllText(pathClassInfo, textUIInfo);
                AssetDatabase.Refresh();
            }*/
        }
#endif

    }

    public enum UILayer
    {
        Normal,
        Popup,
        AlwaysTop
    }
}

