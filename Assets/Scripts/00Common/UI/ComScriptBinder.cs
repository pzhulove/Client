using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace GameClient
{
    [System.Serializable]
    public class ScriptBinderItem : IComparable
    {
        public int bindIndex;
        public UnityEngine.Object component;
        public string varName;
        public bool locked;
        public int CompareTo(object obj)
        {
            if (obj is ScriptBinderItem)
            {
                return bindIndex - (obj as ScriptBinderItem).bindIndex;
            }

            if(obj is int)
            {
                return bindIndex - (int)obj;
            }

            throw new ArgumentException("object can not compare with ScriptBinderItem !!!");
        }

    }
    [System.Serializable]
    public class ScriptStateItem : IComparable
    {
        public int bindIndex;
        public UnityEvent action;
        public int CompareTo(object obj)
        {
            if (obj is ScriptStateItem)
            {
                return bindIndex - (obj as ScriptStateItem).bindIndex;
            }

            if (obj is int)
            {
                return bindIndex - (int)obj;
            }

            throw new ArgumentException("object can not compare with ScriptStateItem !!!");
        }
    }

    public class ComScriptBinder : MonoBehaviour
    {
        void Awake()
        {
            if(useBinarySearch)
            {
                if (scriptItems.Length > 0)
                {
                    System.Array.Sort(scriptItems);
                }
                if (scriptStatus.Length > 0)
                {
                    System.Array.Sort(scriptStatus);
                }
            }
        }

        protected bool useBinarySearch = true;
        [HideInInspector]
        public string labelSpace = string.Empty;
        public string[] argumentsString = new string[0];
        public int[] argumentsInt = new int[0];
        public float[] argumentsFloat = new float[0];
        [HideInInspector]
        public ScriptBinderItem[] scriptItems = new ScriptBinderItem[0];
        [HideInInspector]
        public ScriptStateItem[] scriptStatus = new ScriptStateItem[0];

        protected int findItemLabel(int label)
        {
            if(useBinarySearch)
            {
                return System.Array.BinarySearch(scriptItems, label);
            }
            return label;
        }

        protected int findStatusLabel(int label)
        {
            if (useBinarySearch)
            {
                return System.Array.BinarySearch(scriptStatus, label);
            }
            return label;
        }

        public void SetText(int label,string value)
        {
            var find = findItemLabel(label);
            if (find >= 0 && find < scriptItems.Length)
            {
                var text = scriptItems[find].component as Text;
                if(null != text)
                {
                    text.text = value;
                    return;
                }
            }

            Logger.LogErrorFormat("ComScriptBinder SetText label = {0} error !!!", label);
        }
        public void SetAction(int label)
        {
#if UNITY_EDITOR
            if (scriptStatus.Length > 0)
            {
                System.Array.Sort(scriptStatus);
            }
#endif
            var find = findStatusLabel(label);
            if(find >= 0 && find < scriptStatus.Length)
            {
                if(null != scriptStatus[find].action)
                {
                    scriptStatus[find].action.Invoke();
                    return;
                }
            }

            Logger.LogErrorFormat("ComScriptBinder SetAction label = {0} is failed!!!", label);
        }

        public void SetText(int label,int argumentsIndex)
        {
            if(argumentsIndex >= 0 && argumentsIndex < argumentsString.Length)
            {
                SetText(label, argumentsString[argumentsIndex]);
                return;
            }
            Logger.LogErrorFormat("ComScriptBinder SetText argumentsIndex = {0} is out of range !!!", argumentsIndex);
        }

        public void SetImage(int label,string value)
        {
            var find = findItemLabel(label);
            if (find >= 0 && find < scriptItems.Length)
            {
                var image = scriptItems[find].component as Image;
                if(null != image)
                {
                    image.sprite = AssetLoader.instance.LoadRes(value, typeof(Sprite)).obj as Sprite;
                    return;
                }
            }
            Logger.LogErrorFormat("ComScriptBinder SetImage label = {0} error !!!",label);
        }

        public void SetImage(int label, int argumentsIndex)
        {
            if (argumentsIndex >= 0 && argumentsIndex < argumentsString.Length)
            {
                SetImage(label, argumentsString[argumentsIndex]);
                return;
            }
            Logger.LogErrorFormat("ComScriptBinder SetText argumentsIndex = {0} is out of range !!!", argumentsIndex);
        }

        public T GetScript<T>(int label) where T : Component
        {
            var find = findItemLabel(label);
            if (find >= 0 && find < scriptItems.Length)
            {
                return scriptItems[find].component as T;
            }

            Logger.LogErrorFormat("ComScriptBinder GetScript label = {0} error !!!", label);
            return null;
        }

        public UnityEngine.Object GetObject(int label)
        {
            var find = findItemLabel(label);
            if (find >= 0 && find < scriptItems.Length)
            {
                return scriptItems[find].component;
            }
            return null;
        }
        public int GetIntArgv(int index)
        {
            if (index >= 0 && index < argumentsInt.Length)
            {
                return argumentsInt[index];
            }

            Logger.LogErrorFormat("GetIntArgv index ={0} is out of range len = {1}!", index, argumentsInt.Length);
            return 0;
        }

        public string GetStringArgv(int index)
        {
            if (index >= 0 && index < argumentsString.Length)
            {
                return argumentsString[index];
            }

            Logger.LogErrorFormat("GetStringArgv index = {0} is out of range len = {1}!", index, argumentsString.Length);
            return string.Empty;
        }

        public float GetFloatArgv(int index)
        {
            if (index >= 0 && index < argumentsFloat.Length)
            {
                return argumentsFloat[index];
            }

            Logger.LogErrorFormat("GetFloatArgv index = {0} is out of range len = {1}!",index, argumentsFloat.Length);
            return 0.0f;
        }

        List<KeyValuePair<EUIEventID, ClientEventSystem.UIEventHandler>> mRegistertedEvents = null;
        public void RegisterEvent(EUIEventID id, ClientEventSystem.UIEventHandler handle)
        {
            if(null == mRegistertedEvents)
            {
                mRegistertedEvents = new List<KeyValuePair<EUIEventID, ClientEventSystem.UIEventHandler>>(16);
            }
            UIEventSystem.GetInstance().RegisterEventHandler(id, handle);
            mRegistertedEvents.Add(new KeyValuePair<EUIEventID, ClientEventSystem.UIEventHandler>(id,handle));
        }
        public void UnRegisterEvent(EUIEventID id, ClientEventSystem.UIEventHandler handle)
        {
            if(null != mRegistertedEvents)
            {
                for (int i = 0; i < mRegistertedEvents.Count; ++i)
                {
                    if(id == mRegistertedEvents[i].Key && handle == mRegistertedEvents[i].Value)
                    {
                        UIEventSystem.GetInstance().UnRegisterEventHandler(id, handle);
                        mRegistertedEvents.RemoveAt(i--);
                        break;
                    }
                }
            }
        }
        public void RegisterButtonEvent(int label,UnityAction callback)
        {
            Button button = GetObject(label) as Button;
            if(null != button && null != callback)
            {
                button.onClick.AddListener(callback);
            }
        }
        public void UnRegisterButtonEvent(int label, UnityAction callback)
        {
            Button button = GetObject(label) as Button;
            if (null != button && null != callback)
            {
                button.onClick.RemoveListener(callback);
            }
        }
        public void RegisterToggleEvent(int label,UnityAction<bool> callback)
        {
            Toggle toggle = GetObject(label) as Toggle;
            if(null != toggle)
            {
                toggle.onValueChanged.AddListener(callback);
            }
        }
        public void UnRegisterToggleEvent(int label, UnityAction<bool> callback)
        {
            Toggle toggle = GetObject(label) as Toggle;
            if (null != toggle)
            {
                toggle.onValueChanged.RemoveListener(callback);
            }
        }

        public void OnClickLinkInfo(string argv)
        {
            if(!string.IsNullOrEmpty(argv))
            {
                ActiveManager.GetInstance().OnClickLinkInfo(argv);
            }
        }

        void OnDestroy()
        {
            if(null != mRegistertedEvents)
            {
                for (int i = 0; i < mRegistertedEvents.Count; ++i)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(mRegistertedEvents[i].Key, mRegistertedEvents[i].Value);
                }
                mRegistertedEvents.Clear();
            }

            for(int i = 0; i < scriptItems.Length; ++i)
            {
                var scriptItem = scriptItems[i];
                if(null != scriptItem)
                {
                    if(scriptItem.component is Button)
                    {
                        (scriptItem.component as Button).onClick.RemoveAllListeners();
                    }
                    else if(scriptItem.component is Toggle)
                    {
                        (scriptItem.component as Toggle).onValueChanged.RemoveAllListeners();
                    }
                }
                scriptItems[i] = null;
            }
        }
    }
}