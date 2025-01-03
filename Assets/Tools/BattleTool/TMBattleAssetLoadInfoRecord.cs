using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using FFmpeg.AutoGen;

public enum BattleAssetLoadFlag
{
    None,
    PreLoad,
    PassingDoor,
    Fighting,
}

struct LoadAssetInfo
{
    public string Path;
    public float Time;
}

public interface ITMStopWatch
{
    string GetTimeInfo();
    void Reset(string name);
    string GetName();
    void ResetTagTime();
    string GetTimeFromLastTag();
}

/// <summary>
/// 代码性能检测器
/// </summary>
public class TMStopWatch : ITMStopWatch
{
    private string _name;
    private long _startTime;
    private long _lastTagTime;

    public TMStopWatch(string name)
    {
        Reset(name);
    }

    public void Reset(string name)
    {
        _name = name;
        _startTime = Tenmove.Runtime.Utility.Time.GetTicksNow();
        _lastTagTime = _startTime;
    }

    public string GetName()
    {
        return _name;
    }

    public void ResetTagTime()
    {
        _lastTagTime = Tenmove.Runtime.Utility.Time.GetTicksNow();
    }

    public string GetTimeFromLastTag()
    {
        float time = Tenmove.Runtime.Utility.Time.TicksToMicroseconds(Tenmove.Runtime.Utility.Time.GetTicksNow() - _lastTagTime);
        return string.Format("    耗时:{0}", time);
    }

    public string GetTimeInfo()
    {
        float time = Tenmove.Runtime.Utility.Time.TicksToMicroseconds(Tenmove.Runtime.Utility.Time.GetTicksNow() - _startTime);
        return string.Format("    耗时:{0}", time);
    }
}

/// <summary>
/// 记录战斗内加载的资源信息
/// </summary>
public class TMBattleAssetLoadRecord : MonoSingleton<TMBattleAssetLoadRecord>
{
    private BattleAssetLoadFlag _flag = BattleAssetLoadFlag.None;

    public BattleAssetLoadFlag AssetLoadFlag
    {
        set { _flag = value; }
    }

    public string InfoTag { get; set; }

#if TEST_RECORDASSET
    private List<LoadAssetInfo> _preLoadAssetPathList;
    private List<LoadAssetInfo> _passDoorAssetPathList;
    private List<LoadAssetInfo> _runLoadAssetPathList;

    private List<string> _assetPathKeyList;
    
    private StringBuilder _loadTimeInfoBuilder = new StringBuilder();
    private List<ITMStopWatch> _stopWatchPoolList;

    private StringBuilder _stackInfoBuilder = new StringBuilder();

    public override void Init()
    {
        base.Init();

        _preLoadAssetPathList = new List<LoadAssetInfo>();
        _passDoorAssetPathList = new List<LoadAssetInfo>();
        _runLoadAssetPathList = new List<LoadAssetInfo>();
        _assetPathKeyList = new List<string>();

        _stopWatchPoolList = new List<ITMStopWatch>();

        AssetLoader.OnLoadAsset += _OnLoadAsset;
        AssetLoader.OnLoadAssetPackage += _OnLoadAssetPackage;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        AssetLoader.OnLoadAsset -= _OnLoadAsset;
        AssetLoader.OnLoadAssetPackage -= _OnLoadAssetPackage;
    }

    private void _OnLoadAsset(string asset, float duration)
    {
        _LoadData(asset, duration);
    }

    private void _OnLoadAssetPackage(string assetPackage, float duration)
    {
        _LoadData("AssetPackage:" + assetPackage, duration);
    }

    private void _LoadData(string path,float time)
    {
        var extensionPath = CFileManager.EraseExtension(path);
        if (_assetPathKeyList.Contains(extensionPath)) return;
        _assetPathKeyList.Add(extensionPath);

        LoadAssetInfo loadAssetInfo = new LoadAssetInfo();
        loadAssetInfo.Path = InfoTag + extensionPath;
        loadAssetInfo.Time = time;

        if (_flag == BattleAssetLoadFlag.PreLoad)
        {
            if (!_preLoadAssetPathList.Contains(loadAssetInfo))
                _preLoadAssetPathList.Add(loadAssetInfo);
        }
        else if (_flag == BattleAssetLoadFlag.PassingDoor)
        {
            if (!_passDoorAssetPathList.Contains(loadAssetInfo))
                _passDoorAssetPathList.Add(loadAssetInfo);
        }
        else if (_flag == BattleAssetLoadFlag.Fighting)
        {
            if (!_runLoadAssetPathList.Contains(loadAssetInfo) 
                && !_passDoorAssetPathList.Contains(loadAssetInfo) 
                && !_preLoadAssetPathList.Contains(loadAssetInfo))
            {
                _runLoadAssetPathList.Add(loadAssetInfo);
                SaveStackInfo(path);
            }
        }
    }

    public void AddLoadTimeInfo(string info)
    {
        _loadTimeInfoBuilder.AppendFormat("\n {0}", info);
    }

    private void SaveStackInfo(string title)
    {
#if UNITY_EDITOR
        if (!Global.Settings.isDebug) return;

        StackTrace st = new StackTrace(true);
        _stackInfoBuilder.AppendFormat("\n  ResPath:{0}", title);
        _stackInfoBuilder.AppendFormat("\n{0}",st.ToString());
        _stackInfoBuilder.AppendFormat("\n");
#endif
    }

    public void SaveInfoToFile()
    {
        if (_flag == BattleAssetLoadFlag.None) return;

        _flag = BattleAssetLoadFlag.None;

        double fNowTime = GameClient.Function.ConvertDateTimeInt(DateTime.Now);

        string folderPath = GetFolderPath();
        string assetInfoPath = string.Format("{0}{1}_AssetInfo.txt", folderPath, fNowTime);
        string stackInfoPath = string.Format("{0}{1}_StackInfo.txt", folderPath, fNowTime);
        string loadTimeInfoPath = string.Format("{0}{1}_LoadTimeInfo.txt", folderPath, fNowTime);

        SaveAssetLoadInfoToTile(assetInfoPath);
        SaveStackInfoToTile(stackInfoPath);
        SaveLoadInfoToFile(loadTimeInfoPath);

        _ClearData();
    }

    private void _ClearData()
    {
        _runLoadAssetPathList.Clear();
        _passDoorAssetPathList.Clear();
        _preLoadAssetPathList.Clear();

        _assetPathKeyList.Clear();
        _loadTimeInfoBuilder.Clear();
        _stopWatchPoolList.Clear();

        _stackInfoBuilder.Clear();
    }

    private void SaveAssetLoadInfoToTile(string savePath)
    {
        StringBuilder builder = StringBuilderCache.Acquire();

        _runLoadAssetPathList.Sort(_AssetLoadInfoListCompare);
        _passDoorAssetPathList.Sort(_AssetLoadInfoListCompare);
        _preLoadAssetPathList.Sort(_AssetLoadInfoListCompare);

        builder.AppendFormat("运行时加载资源列表:\n");
        for (int i = 0; i < _runLoadAssetPathList.Count; i++)
        {
            builder.AppendFormat("{0} time:{1} \n", _runLoadAssetPathList[i].Path, _runLoadAssetPathList[i].Time);
        }

        builder.AppendFormat("过门时加载资源列表:\n");
        for (int i = 0; i < _passDoorAssetPathList.Count; i++)
        {
            builder.AppendFormat("{0} time:{1}\n", _passDoorAssetPathList[i].Path, _passDoorAssetPathList[i].Time);
        }

        builder.AppendFormat("预加载资源列表:\n");
        for (int i = 0; i < _preLoadAssetPathList.Count; i++)
        {
            builder.AppendFormat("{0} time:{1}\n", _preLoadAssetPathList[i].Path, _preLoadAssetPathList[i].Time);
        }

        BeUtility.SaveDataToFile(savePath, builder.ToString());
    }

    private void SaveStackInfoToTile(string savePath)
    {
#if UNITY_EDITOR
        if (!Global.Settings.isDebug) return;

        BeUtility.SaveDataToFile(savePath, _stackInfoBuilder.ToString());
#endif
    }

    private void SaveLoadInfoToFile(string savePath)
    {
        BeUtility.SaveDataToFile(savePath, _loadTimeInfoBuilder.ToString());
    }

    private string GetFolderPath()
    {
        string folderPath = null;
#if UNITY_EDITOR
        folderPath = Application.dataPath + "../../BattleAssetLoadInfo/";
#else
        folderPath = Application.persistentDataPath  + "/BattleAssetLoadInfo/";
#endif
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        return folderPath;
    }

    private int _AssetLoadInfoListCompare(LoadAssetInfo left,LoadAssetInfo right)
    {
        if (left.Time < right.Time)
            return 1;
        else if (left.Time == right.Time)
            return 0;
        return -1;
    }

    public ITMStopWatch CreateStopWatch(string name)
    {
        if (_stopWatchPoolList.Count > 0)
        {
            var watch = _stopWatchPoolList[0];
            watch.Reset(name);
            _stopWatchPoolList.RemoveAt(0);
            return watch;
        }
        return new TMStopWatch(name);
    }

    public void SaveWatch(string name,ITMStopWatch stopWatch)
    {
        if (stopWatch == null) return;
        string info = stopWatch.GetTimeInfo();
        AddLoadTimeInfo(name + info);
    }

    public void SaveAndRefreshTag(string name, ITMStopWatch stopWatch)
    {
        if (stopWatch == null) return;
        string info = string.Format("{0}   {1}   {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), name, stopWatch.GetTimeFromLastTag());
        AddLoadTimeInfo(info);
        stopWatch.ResetTagTime();
    }

    public void CloseAndSaveWatch(ITMStopWatch stopWatch)
    {
        if (stopWatch == null) return;
        string name = stopWatch.GetName();
        string info =string.Format("{0}   {1}   {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms"), name, stopWatch.GetTimeInfo());
        AddLoadTimeInfo(info);
        if (!_stopWatchPoolList.Contains(stopWatch))
            _stopWatchPoolList.Add(stopWatch);
    }

    public void SaveExtraInfo(string info)
    {
        AddLoadTimeInfo(info);
    }

#else
    public void SaveInfoToFile(){}

    public ITMStopWatch CreateStopWatch(string name)
    {
        return null;
    }
    public void SaveWatch(string name, ITMStopWatch stopWatch) { }
    public void SaveAndRefreshTag(string name, ITMStopWatch stopWatch) { }
    public void CloseAndSaveWatch(ITMStopWatch stopWatch) { }

#endif
}