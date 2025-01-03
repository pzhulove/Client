using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using GameClient;
using Protocol;
using System.IO;
using System;
using NUnit.Framework.Constraints;
using UnityEditor.MemoryProfiler;
using MemoryProfilerWindow;

public class LogicServerTest : EditorWindow
{
    byte[] _recordData = null;
    string[] _recordLog = null;
    [NonSerialized]
    UnityEditor.MemoryProfiler.PackedMemorySnapshot _snapshot;

    [SerializeField]
    PackedCrawlerData _packedCrawled;

    [NonSerialized]
    CrawledMemorySnapshot _unpackedCrawl;
    public enum RECORD_TYPE
    {
        PVP,
        PVE,
    }
    public enum RUNNING_STATE
    {
        READY_START,
        RUNNING,
        READY_END,
        END,
    }
    [NonSerialized]
    private bool _registered = false;
    private RUNNING_STATE _runningStat = RUNNING_STATE.END;
    public int BattleCount = 1;
    public int recordIndex = 0;
    public RECORD_TYPE recType = RECORD_TYPE.PVP;
    private float timeScaler = 1.0f;
    // Use this for initialization 
    [MenuItem("Tools/LogicServerTest")]
    static void Create()
    {
        EditorWindow.GetWindow<LogicServerTest>();

    }
    [MenuItem("Tools/MonoTest")]
    static void Test()
    {
        int[] a = new int[1024];
        Debug.LogFormat("a {0}",a);
        GC.Collect();

    }
    public void Initialize()
    {

        if (!_registered)
        {
            UnityEditor.MemoryProfiler.MemorySnapshot.OnSnapshotReceived += IncomingSnapshot;
            _registered = true;
            if (_recordData == null)
            {
                LogicApplication.Init();
                DirectoryInfo dirInfo = Directory.GetParent(Application.dataPath);
                string recorderDir = string.Format("{0}\\RecordLog", dirInfo.Parent.FullName);
                string[] recordFiles = Directory.GetFiles(recorderDir);
                List<string> validFiles = new List<string>();
                for (int i = 0; i < recordFiles.Length; i++)
                {
                    string extName = Path.GetExtension(recordFiles[i]);
                    if (extName.Length == 0)
                    {
                        validFiles.Add(recordFiles[i]);
                    }
                }
                _recordLog = validFiles.ToArray();
            }
        }


        if (_unpackedCrawl == null && _packedCrawled != null && _packedCrawled.valid)
            Unpack();
    }
    void IncomingSnapshot(PackedMemorySnapshot snapshot)
    {
        _snapshot = snapshot;
        if (_runningStat == RUNNING_STATE.READY_START || _runningStat == RUNNING_STATE.READY_END)
        {
            UnityEditor.EditorUtility.DisplayProgressBar("Take Snapshot", "Crawling Snapshot...", 0.33f);
            try
            {
                _packedCrawled = new Crawler().Crawl(_snapshot);

                UnityEditor.EditorUtility.DisplayProgressBar("Take Snapshot", "Unpacking Snapshot...", 0.67f);

                Unpack();
                DirectoryInfo dirInfo = Directory.GetParent(Application.dataPath);
                string memsnapshotDir = string.Format("{0}\\memsnapshot", dirInfo.Parent.FullName);
                if (!Directory.Exists(memsnapshotDir))
                {
                    Directory.CreateDirectory(memsnapshotDir);
                }
                string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string fullFileName = string.Format("{0}\\{1}.memsnap3", memsnapshotDir, fileName);
                PackedMemorySnapshotUtility.SaveToFile(fullFileName, _snapshot);
            }
            finally
            {
                UnityEditor.EditorUtility.ClearProgressBar();
            }
        }
        if (_runningStat == RUNNING_STATE.READY_START)
        {
            for (int i = 0; i < BattleCount; i++)
            {
                if (recType == RECORD_TYPE.PVE)
                {
                    LogicApplication.StartPVERecord(_recordData);
                }
                else
                {
                    LogicApplication.StartPVPRecord(_recordData);
                }
            }
            _runningStat = RUNNING_STATE.RUNNING;
            EditorApplication.update = OnUpdate;
        }
        else if (_runningStat == RUNNING_STATE.READY_END)
        {
            _runningStat = RUNNING_STATE.END;
            EditorApplication.update = null;
        }
    }
    void Unpack()
    {
        _unpackedCrawl = CrawlDataUnpacker.Unpack(_packedCrawled);
    }
    void OnGUI()
    {
        Initialize();
        GUILayout.BeginVertical();
        recType = (RECORD_TYPE)EditorGUILayout.EnumPopup("录像类型选择:", recType);
        EditorGUILayout.LabelField("录像选择");
        recordIndex = EditorGUILayout.Popup(recordIndex, _recordLog);
        EditorGUILayout.LabelField("开局数量");
        EditorGUILayout.IntField(BattleCount);
        EditorGUILayout.LabelField("时间缩放比");
        timeScaler =  EditorGUILayout.Slider(timeScaler,1.0f,100.0f);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("开始测试"))
        {
            try
            {
                StartServerTest();

            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        EditorGUI.BeginDisabledGroup(_snapshot == null);
        if (GUILayout.Button("终止测试"))
        {
            StopServerTest();
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField(LogicApplication.LogicCount > 0 ? "运行中" : "中止运行");
        GUILayout.EndVertical();
    }
    void StartServerTest()
    {
        if (_runningStat != RUNNING_STATE.END)
        {
            UnityEditor.EditorUtility.DisplayDialog("错误", "正在运行中", "确定");
            return;
        }

        if (_recordLog == null || recordIndex < 0 || _recordLog.Length <= recordIndex)
        {
            UnityEditor.EditorUtility.DisplayDialog("错误", "不存在录像文件或者超出选定范围", "确定");
            return;
        }
        if (!File.Exists(_recordLog[recordIndex]))
        {
            UnityEditor.EditorUtility.DisplayDialog("错误", "不存在录像文件", "确定");
            return;
        }
        _runningStat = RUNNING_STATE.READY_START;
        _recordData = File.ReadAllBytes(_recordLog[recordIndex]);

        UnityEditor.MemoryProfiler.MemorySnapshot.RequestNewSnapshot();

    }
    public void StopServerTest()
    {
        _runningStat = RUNNING_STATE.END;
        LogicApplication.DeInit();
        EditorApplication.update = null;
    }
    void OnUpdate()
    {
        if (_runningStat != RUNNING_STATE.RUNNING) return;
        float elapseTime = Time.deltaTime;
        if (LogicApplication.LogicCount <= 0)
        {
            _runningStat = RUNNING_STATE.READY_END;
            System.GC.Collect();
            UnityEditor.MemoryProfiler.MemorySnapshot.RequestNewSnapshot();
            return;
        }
        LogicApplication.Update((int)(elapseTime * timeScaler * GlobalLogic.VALUE_1000));

    }
}
