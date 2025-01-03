using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;


[CustomEditor(typeof(EffectAudioProxy), true)]
public class EffectAudioProxyInspector : Editor
{
    private SerializedObject m_Object;

    static readonly int INVALID_CALL_ID = -1;
    static readonly string INVALID_TAG = "";

    public class EffAudioDescItem
    {
        public AudioClip m_AudioClip = null;
        public int m_AudioType = (int)AudioType.AudioEffect;
        public string m_AudioTag = INVALID_TAG;
        public bool m_Loop = false;
        public int m_TriggerType = (int)EffectAudioProxy.AudioTigger.OnPlay;
        public int m_CallID = INVALID_CALL_ID;
        public int m_TimeDelayMS = 0;
        public float m_Volume = 1.0f;
    }

    protected List<EffAudioDescItem> m_EffAudioDescList = new List<EffAudioDescItem>();

    static string[] m_AudioTypeString = new string[] { AudioType.AudioEffect.ToString(), AudioType.AudioStream.ToString(), AudioType.AudioVoice.ToString() };
    static string[] m_TriggerType = new string[] { EffectAudioProxy.AudioTigger.None.ToString(), EffectAudioProxy.AudioTigger.OnCall.ToString(), EffectAudioProxy.AudioTigger.OnPlay.ToString()};

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
        m_Object.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        bool bIsDirty = false;
        for(int i = 0,icnt = m_EffAudioDescList.Count; i < icnt;++i)
        {
            EffAudioDescItem curDesc = m_EffAudioDescList[i];
            bool dirty = false;
            bool remove = _DrawAudioDescItem(curDesc,ref dirty);
            bIsDirty |= dirty;

            if(remove)
            {
                bIsDirty = true;
                m_EffAudioDescList.RemoveAt(i);
                break;
            }
        }

        if(GUILayout.Button("添加新的触发器"))
        {
            m_EffAudioDescList.Add(new EffAudioDescItem());
            bIsDirty = true;
        }

        if(bIsDirty)
        {
            _Resave();
        }
    }

    protected bool _DrawAudioDescItem(EffAudioDescItem descItem,ref bool bIsDirty)
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

        if ((int)EffectAudioProxy.AudioTigger.OnCall == descItem.m_TriggerType)
        {
            int callID = descItem.m_CallID;
            descItem.m_CallID = EditorGUILayout.IntField("调用触发ID：", callID);
            if( INVALID_CALL_ID == descItem.m_CallID)
            {
                descItem.m_CallID = callID;
                EditorGUILayout.HelpBox("调用触发ID不能为-1！", MessageType.Warning);
            }

            if (descItem.m_CallID != callID)
                bIsDirty = true;
        }

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
        EffectAudioProxy targAudioProxy = (EffectAudioProxy)target;
        
        for (int i = 0, icnt = targAudioProxy.EffAudioDescList.Length; i < icnt; ++i)
        {
            EffAudioDescItem curEffAudioDesc = new EffAudioDescItem();
            EffectAudioProxy.EffAudioDesc cur = targAudioProxy.EffAudioDescList[i];

            curEffAudioDesc.m_AudioClip = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets/Resources/",cur.m_AudioRes),typeof(AudioClip)) as AudioClip;
            curEffAudioDesc.m_AudioType = (int)cur.m_AudioType;
            curEffAudioDesc.m_AudioTag = cur.m_AudioTag;
            curEffAudioDesc.m_TimeDelayMS = cur.m_TimeDelayMS;
            curEffAudioDesc.m_TriggerType = (int)cur.m_Trigger;
            curEffAudioDesc.m_Volume = cur.m_Volume;
            curEffAudioDesc.m_Loop = cur.m_Loop;
            curEffAudioDesc.m_CallID = cur.m_CallID;

            m_EffAudioDescList.Add(curEffAudioDesc);
        }
    }

    void _Resave()
    {
        EffectAudioProxy targAudioProxy = (EffectAudioProxy)target;

        List<EffectAudioProxy.EffAudioDesc> EffAudioDescList = new List<EffectAudioProxy.EffAudioDesc>();
        for (int i = 0, icnt = m_EffAudioDescList.Count; i < icnt; ++i)
        {
            EffectAudioProxy.EffAudioDesc curEffAudioDesc = new EffectAudioProxy.EffAudioDesc();
            EffAudioDescItem cur = m_EffAudioDescList[i];

            curEffAudioDesc.m_AudioRes = AssetDatabase.GetAssetPath(cur.m_AudioClip).Replace("Assets/Resources/", null);
            curEffAudioDesc.m_AudioTag = cur.m_AudioTag;
            curEffAudioDesc.m_AudioType = (AudioType)cur.m_AudioType;
            curEffAudioDesc.m_TimeDelayMS = cur.m_TimeDelayMS;
            curEffAudioDesc.m_Trigger = (EffectAudioProxy.AudioTigger)cur.m_TriggerType;
            curEffAudioDesc.m_Volume = cur.m_Volume;
            curEffAudioDesc.m_Loop = cur.m_Loop;
            curEffAudioDesc.m_CallID = cur.m_CallID;

            EffAudioDescList.Add(curEffAudioDesc);
        }

        targAudioProxy.EffAudioDescList = EffAudioDescList.ToArray();
    }


    [MenuItem("[TM工具集]/ArtTools/添加音量播放组件")]
    static public void SetupAudioProxy()
    {
        var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/UIFlatten/Prefabs" });
        //var str = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources/UIFlatten/Prefabs/SelectRole" });
        int cnt = 0;
        float total = str.Length;
        List<EffectAudioProxy.EffAudioDesc> ButtonAudioDescList = new List<EffectAudioProxy.EffAudioDesc>();

        EffectAudioProxy.EffAudioDesc clickDesc = new EffectAudioProxy.EffAudioDesc();

        clickDesc.m_AudioRes = "Sound/UI/ui_general_focus_01.ogg";
        clickDesc.m_AudioType = AudioType.AudioEffect;
        clickDesc.m_AudioTag = INVALID_TAG;
        clickDesc.m_CallID = INVALID_CALL_ID;
        clickDesc.m_Loop = false;
        clickDesc.m_TimeDelayMS = 0;
        clickDesc.m_Trigger = EffectAudioProxy.AudioTigger.OnPlay;
        clickDesc.m_Volume = 1;

        ButtonAudioDescList.Add(clickDesc);

        foreach (var guid in str)
        {
            EditorUtility.DisplayProgressBar("添加音频组件", "正在为第" + cnt + "个资源添加组件...", ((cnt++) / total));

            var path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            GameObject temp = PrefabUtility.InstantiatePrefab(data) as GameObject;
            if (null != temp)
            {
                EffectAudioProxy frameAudioProxy = temp.GetComponent<EffectAudioProxy>();
                if (null != frameAudioProxy)
                    DestroyImmediate(frameAudioProxy);
            }

            AssetDatabase.SaveAssets();
            PrefabUtility.ReplacePrefab(temp, data, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(temp);
            GameObject.DestroyImmediate(data);
        }

        EditorUtility.ClearProgressBar();
    }
}
