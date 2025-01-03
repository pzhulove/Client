using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;


[CustomEditor(typeof(UIAudioProxy), true)]
public class UIAudioProxyInspector : Editor
{
    private SerializedObject m_Object;

    static readonly int INVALID_CALL_ID = -1;
    static readonly string INVALID_TAG = "";

    public class UIAudioDescItem
    {
        public AudioClip m_AudioClip = null;
        public int m_AudioType = (int)AudioType.AudioEffect;
        public string m_AudioTag = INVALID_TAG;
        public bool m_Loop = false;
        public int m_TriggerType = (int)UIAudioProxy.AudioTigger.OnPointerDown;
        public int m_CallID = INVALID_CALL_ID;
        public int m_TimeDelayMS = 0;
        public float m_Volume = 1.0f;
    }

    protected List<UIAudioDescItem> m_UIAudioDescList = new List<UIAudioDescItem>();

    static string[] m_AudioTypeString = new string[] { AudioType.AudioEffect.ToString(), AudioType.AudioStream.ToString(), AudioType.AudioVoice.ToString() };
    static string[] m_TriggerType = new string[] { UIAudioProxy.AudioTigger.None.ToString(), UIAudioProxy.AudioTigger.OnCall.ToString(), UIAudioProxy.AudioTigger.OnOpen.ToString(), UIAudioProxy.AudioTigger.OnPointerDown.ToString(), UIAudioProxy.AudioTigger.OnClose.ToString() };

    protected void OnEnable()
    {
        m_Object = new SerializedObject(target);
        _Reload();
    }

    protected void OnDisable()
    {
        _Resave();
    }

    public override bool HasPreviewGUI() { return true; }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        m_Object.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        bool bIsDirty = false;
        for(int i = 0,icnt = m_UIAudioDescList.Count; i < icnt;++i)
        {
            UIAudioDescItem curDesc = m_UIAudioDescList[i];
            bool dirty = false;
            bool remove = _DrawAudioDescItem(curDesc,ref dirty);
            bIsDirty |= dirty;

            if(remove)
            {
                bIsDirty = true;
                m_UIAudioDescList.RemoveAt(i);
                break;
            }
        }

        if(GUILayout.Button("添加新的触发器"))
        {
            m_UIAudioDescList.Add(new UIAudioDescItem());
            bIsDirty = true;
        }

        if(bIsDirty)
        {
            _Resave();
        }
    }

    protected bool _DrawAudioDescItem(UIAudioDescItem descItem,ref bool bIsDirty)
    {
        int nButtonWidth = 50;
        bool bRemove = false;
        EditorGUILayout.BeginHorizontal();
        AudioClip curClip = descItem.m_AudioClip;
        descItem.m_AudioClip = EditorGUILayout.ObjectField("声音文件：", curClip, typeof(AudioClip)) as AudioClip;
        if (curClip != descItem.m_AudioClip)
            bIsDirty = true;

        bRemove = GUILayout.Button("删除", GUILayout.Width(nButtonWidth));
        EditorGUILayout.EndHorizontal();

        string audioTag = descItem.m_AudioTag;
        descItem.m_AudioTag = EditorGUILayout.TextField("触发器标签：", audioTag);
        if (descItem.m_AudioTag != audioTag)
            bIsDirty = true;

        int audioType = descItem.m_AudioType;
        descItem.m_AudioType = EditorGUILayout.Popup("音频类型：", audioType, m_AudioTypeString);
        if (descItem.m_AudioType != audioType)
            bIsDirty = true;

        int triggerType = descItem.m_TriggerType;
        descItem.m_TriggerType = EditorGUILayout.Popup("触发方式：", triggerType, m_TriggerType);
        if (descItem.m_TriggerType != triggerType)
            bIsDirty = true;

        int callID = INVALID_CALL_ID;
        if ((int)UIAudioProxy.AudioTigger.OnCall == descItem.m_TriggerType)
        {
            callID = descItem.m_CallID;
            descItem.m_CallID = EditorGUILayout.IntField("调用触发ID：", callID);
            if (INVALID_CALL_ID == descItem.m_CallID)
            {
                descItem.m_CallID = callID;
                EditorGUILayout.HelpBox("调用触发ID不能为-1！", MessageType.Warning);
            }
        }
        else
            descItem.m_CallID = INVALID_CALL_ID;

        if (descItem.m_CallID != callID)
            bIsDirty = true;

        bool loop = descItem.m_Loop;
        descItem.m_Loop = EditorGUILayout.Toggle("是否循环：", descItem.m_Loop);
        if (loop != descItem.m_Loop)
            bIsDirty = true;

        int delayMS = descItem.m_TimeDelayMS;
        descItem.m_TimeDelayMS = EditorGUILayout.IntField("延迟播放（毫秒）：", delayMS);
        if (descItem.m_TimeDelayMS < 0)
            descItem.m_TimeDelayMS = 0;
        if (descItem.m_TimeDelayMS != delayMS)
            bIsDirty = true;

        float volume = descItem.m_Volume;
        descItem.m_Volume = EditorGUILayout.Slider("音量：", volume,0.0f,1.0f);
        if (descItem.m_Volume != volume)
            bIsDirty = true;

        EditorGUILayout.Space();

        return bRemove;
    }

    void _Reload()
    {
        UIAudioProxy targAudioProxy = (UIAudioProxy)target;
        
        for (int i = 0, icnt = targAudioProxy.UIAudioDescList.Length; i < icnt; ++i)
        {
            UIAudioDescItem curUIAudioDesc = new UIAudioDescItem();
            UIAudioProxy.UIAudioDesc cur = targAudioProxy.UIAudioDescList[i];

            curUIAudioDesc.m_AudioClip = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/",cur.m_AudioRes),typeof(AudioClip)) as AudioClip;
            curUIAudioDesc.m_AudioType = (int)cur.m_AudioType;
            curUIAudioDesc.m_AudioTag = cur.m_AudioTag;
            curUIAudioDesc.m_TimeDelayMS = cur.m_TimeDelayMS;
            curUIAudioDesc.m_TriggerType = (int)cur.m_Trigger;
            curUIAudioDesc.m_Volume = cur.m_Volume;
            curUIAudioDesc.m_Loop = cur.m_Loop;
            curUIAudioDesc.m_CallID = cur.m_CallID;

            m_UIAudioDescList.Add(curUIAudioDesc);
        }
    }

    void _Resave()
    {
        UIAudioProxy targAudioProxy = (UIAudioProxy)target;

        List<UIAudioProxy.UIAudioDesc> UIAudioDescList = new List<UIAudioProxy.UIAudioDesc>();
        for (int i = 0, icnt = m_UIAudioDescList.Count; i < icnt; ++i)
        {
            UIAudioProxy.UIAudioDesc curUIAudioDesc = new UIAudioProxy.UIAudioDesc();
            UIAudioDescItem cur = m_UIAudioDescList[i];

            curUIAudioDesc.m_AudioRes = AssetDatabase.GetAssetPath(cur.m_AudioClip).Replace("Assets/Resources/", null);
            curUIAudioDesc.m_AudioTag = cur.m_AudioTag;
            curUIAudioDesc.m_AudioType = (AudioType)cur.m_AudioType;
            curUIAudioDesc.m_TimeDelayMS = cur.m_TimeDelayMS;
            curUIAudioDesc.m_Trigger = (UIAudioProxy.AudioTigger)cur.m_TriggerType;
            curUIAudioDesc.m_Volume = cur.m_Volume;
            curUIAudioDesc.m_Loop = cur.m_Loop;
            curUIAudioDesc.m_CallID = cur.m_CallID;

            UIAudioDescList.Add(curUIAudioDesc);
        }

        targAudioProxy.UIAudioDescList = UIAudioDescList.ToArray();
    }


    [MenuItem("[TM工具集]/ArtTools/添加音量播放组件")]
    static public void SetupAudioProxy()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/UIFlatten/Prefabs" });
        //var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/UIFlatten/Prefabs/SelectRole" });
        int cnt = 0;
        float total = str.Length;
        List<UIAudioProxy.UIAudioDesc> ButtonAudioDescList = new List<UIAudioProxy.UIAudioDesc>();

        UIAudioProxy.UIAudioDesc clickDesc = new UIAudioProxy.UIAudioDesc();

        clickDesc.m_AudioRes = "Sound/UI/ui_general_focus_01.ogg";
        clickDesc.m_AudioType = AudioType.AudioEffect;
        clickDesc.m_AudioTag = INVALID_TAG;
        clickDesc.m_CallID = INVALID_CALL_ID;
        clickDesc.m_Loop = false;
        clickDesc.m_TimeDelayMS = 0;
        clickDesc.m_Trigger = UIAudioProxy.AudioTigger.OnPointerDown;
        clickDesc.m_Volume = 1;

        ButtonAudioDescList.Add(clickDesc);

        List<UIAudioProxy.UIAudioDesc> FrameAudioDescList = new List<UIAudioProxy.UIAudioDesc>();

        UIAudioProxy.UIAudioDesc openDesc = new UIAudioProxy.UIAudioDesc();
        openDesc.m_AudioRes = "Sound/UI/ui_window_open.ogg";
        openDesc.m_AudioType = AudioType.AudioEffect;
        openDesc.m_AudioTag = "Frame";
        openDesc.m_CallID = INVALID_CALL_ID;
        openDesc.m_Loop = false;
        openDesc.m_TimeDelayMS = 0;
        openDesc.m_Trigger = UIAudioProxy.AudioTigger.OnOpen;
        openDesc.m_Volume = 1;
        FrameAudioDescList.Add(openDesc);

        UIAudioProxy.UIAudioDesc closeDesc = new UIAudioProxy.UIAudioDesc();
        closeDesc.m_AudioRes = "Sound/UI/ui_window_close.ogg";
        closeDesc.m_AudioType = AudioType.AudioEffect;
        closeDesc.m_AudioTag = "Frame";
        closeDesc.m_CallID = INVALID_CALL_ID;
        closeDesc.m_Loop = false;
        closeDesc.m_TimeDelayMS = 0;
        closeDesc.m_Trigger = UIAudioProxy.AudioTigger.OnClose;
        closeDesc.m_Volume = 1;

        FrameAudioDescList.Add(closeDesc);

        foreach (var guid in str)
        {
            EditorUtility.DisplayProgressBar("添加音频组件", "正在为第" + cnt + "个资源添加组件...", ((cnt++) / total));

            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            if (null != temp)
            {
                UIAudioProxy frameAudioProxy = temp.GetComponent<UIAudioProxy>();
                if (null != frameAudioProxy)
                    DestroyImmediate(frameAudioProxy);

                //if (null == frameAudioProxy)
                //{
                //    frameAudioProxy = temp.AddComponent<UIAudioProxy>();
                //    frameAudioProxy.UIAudioDescList = FrameAudioDescList.ToArray();
                //}

                List<Selectable> controlLst = new List<Selectable>();
                controlLst.AddRange(temp.GetComponentsInChildren<Button>());
                controlLst.AddRange(temp.GetComponentsInChildren<Toggle>());

                for(int i = 0,icnt = controlLst.Count;i<icnt;++i)
                {
                    Selectable cur = controlLst[i];
                    if(null == cur) continue;

                    UIAudioProxy proxy = cur.gameObject.GetComponent<UIAudioProxy>();
                    //if (null != proxy) DestroyImmediate(proxy);
                    if (null != proxy) continue;
                    //if (null == proxy)
                    proxy = cur.gameObject.AddComponent<UIAudioProxy>();
                    proxy.UIAudioDescList = ButtonAudioDescList.ToArray();
                }
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
            GameObject.DestroyImmediate(data);
        }

        EditorUtility.ClearProgressBar();
    }
}
