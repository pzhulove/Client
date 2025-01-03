using UnityEngine;
using Tenmove.Runtime;
using System.IO;
using Tenmove.Runtime.Unity;
using System;
using System.Collections.Generic;

public class UnitTest_2020_08_15_GameProfilerTest : MonoBehaviour,IEquatable<UnitTest_2020_08_15_GameProfilerTest>
{
    ITMUnityGameProfileClient m_Simulator;

    private bool m_Display;
    private string m_IPAddress;

    private List<GameObject> m_AssetList;
    private List<uint> m_LoadingAssetList;

    private void Awake()
    {
        Tenmove.Runtime.Utility.Thread.SetMainThread();
        m_Display = false;
        m_AssetList = new List<GameObject>();
        m_LoadingAssetList = new List<uint>();
    }

    // Use this for initialization
    void Start()
    {
        m_Simulator = ModuleManager.GetModule<ITMUnityGameProfileClient>();
    }

    // Update is called once per frame
    void Update()
    {
        ModuleManager.Update(Time.deltaTime, Time.unscaledTime);
        
    }

   
    private void OnGUI()
    {
        Rect rect = new Rect(0, Screen.height - Screen.dpi * 0.5f, Screen.width * 0.5f, Screen.dpi * 0.5f);
        GUILayout.BeginArea(rect);

        int originButtonFontSize = GUI.skin.button.fontSize;
        GUI.skin.button.fontSize = 28;

        GUILayout.BeginHorizontal();
        string text = m_Display ? "隐藏配置◀" : "显示配置▶";
        if (GUILayout.Button(text, GUILayout.Width(Screen.dpi * 1.6f), GUILayout.Height(Screen.dpi * 0.5f)))
            m_Display = !m_Display;
        GUI.skin.button.fontSize = originButtonFontSize;

        if (m_Display)
            _DisplayConnectConfig();

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void _DisplayConnectConfig()
    {
        int originButtonFontSize = GUI.skin.button.fontSize;
        GUI.skin.button.fontSize = 28;

        int originTextFeildFontSize = GUI.skin.textField.fontSize;
        GUI.skin.textField.fontSize = 28;

        int originBoxFontSize = GUI.skin.box.fontSize;
        GUI.skin.box.fontSize = 28;

        GUILayout.Box("请输入控制端IP：", GUILayout.Width(Screen.dpi * 3), GUILayout.Height(Screen.dpi * 0.5f));
        m_IPAddress = GUILayout.TextField(m_IPAddress, GUILayout.Height(Screen.dpi * 0.5f));

        NetIPAddress netIPAddress = NetIPAddress.InvalidAddress;
        if (NetIPAddress.IsValidIPPattern(m_IPAddress))
            netIPAddress = new NetIPAddress(m_IPAddress);

        bool orginDisable = GUI.enabled;
        GUI.enabled = NetIPAddress.InvalidAddress != netIPAddress;

        if (GUILayout.Button("链接", GUILayout.Width(Screen.dpi * 1.5f), GUILayout.Height(Screen.dpi * 0.5f)))
        {
            if (null != m_Simulator)
            {
                _ClearAssets();
                m_Simulator.EndConnect();
            }

            m_Simulator.BeginConnect(netIPAddress, 9527);
            m_Simulator.RegisterMessageProcessor<GameProfileCreateAsset, UnitTest_2020_08_15_GameProfilerTest>(_CreateAssetProcessor, this);
            m_Simulator.RegisterMessageProcessor<GameProfileClearAllAssets, UnitTest_2020_08_15_GameProfilerTest>(_ClearAllAssetsProcessor, this);
        }

        GUI.enabled = orginDisable;
        GUI.skin.button.fontSize = originButtonFontSize;
        GUI.skin.textField.fontSize = originTextFeildFontSize;
        GUI.skin.box.fontSize = originBoxFontSize;
    }

    private static void _CreateAssetProcessor(GameProfileCreateAsset message, UnitTest_2020_08_15_GameProfilerTest _this)
    {
        for(int i = 0,icnt = message.InstCount;i<icnt;++i)
            _this.m_LoadingAssetList.Add(_this.m_Simulator.LoadAsset<GameObject, UnitTest_2020_08_15_GameProfilerTest>(message.AssetName, _this, _OnAssetLoadFinished));
    }

    private static void _ClearAllAssetsProcessor(GameProfileClearAllAssets message, UnitTest_2020_08_15_GameProfilerTest _this)
    {
        _this._ClearAssets();
    }

    private static void _OnAssetLoadFinished(uint requestID, string assetPath, GameObject assetObject, UnitTest_2020_08_15_GameProfilerTest _this)
    {
        Tenmove.Runtime.Debugger.Assert(null != _this, "Parameter '_this' can not be null!");
        if(_this.m_Simulator.InvalidAssetHandle != requestID)
        {
            for(int i = 0,icnt = _this.m_LoadingAssetList.Count;i<icnt;++i)
            {
                if(requestID == _this.m_LoadingAssetList[i])
                {
                    _this.m_AssetList.Add(assetObject);
                    _this.m_LoadingAssetList.RemoveAt(i);
                    Tenmove.Runtime.Debugger.LogInfo("Asset '{0}' has load success!", assetPath);
                    return;
                }
            }
        }

        if (null != assetObject)
            GameObject.Destroy(assetObject);
    }

    private void _ClearAssets()
    {
        for(int i = 0,icnt = m_AssetList.Count;i<icnt;++i)
        {
            GameObject cur = m_AssetList[i];
            if (null != cur)
                GameObject.Destroy(cur);
        }

        m_AssetList.Clear();
    }

    private void OnDestroy()
    {
        if (null != m_Simulator)
            m_Simulator.EndConnect();
    }

    public bool Equals(UnitTest_2020_08_15_GameProfilerTest other)
    {
        return GetInstanceID() == other.GetInstanceID();
    }
}
