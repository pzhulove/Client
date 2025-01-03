using System.Collections.Generic;
using UnityEngine;
using GameClient;

public class LevelManager
{
    public LevelManager() { }

    /// <summary>
    /// 计数器类型ID
    /// </summary>
    public enum CounterTypeId
    {
        NONE = 0,
        WIND_DIR = 1,    //风向(默认-1 -1无风 0左倾斜 1右倾斜)
    }

    LevelAgent mAgent;
    public BaseBattle baseBattle
    { get; private set; }
    private bool mIsFight = false;

    // 关卡编辑器使用的计数器
    private Dictionary<int, int> mCounterDic = new Dictionary<int, int>();

    // 计时器
    private int countTime = 0;
    public int CountTime { get { return countTime; } set { countTime = value; } }

    // 关卡房间内运行时间（毫秒）
    private int roomRunningTime = 0;
    public int RoomRunningTime { get { return roomRunningTime; } set { roomRunningTime = value; } }

    /// <summary>
    /// 关卡内过门结束
    /// </summary>
    public void PassedDoor()
    {
        roomRunningTime = 0;
    }

    public bool Init(string levelFileName, BaseBattle baseBattle)
    {
        string path = levelFileName;
#if AI_USEXML && LOGIC_SERVER
        path = levelFileName.ToLower();
#endif

        this.baseBattle = baseBattle;
        mAgent = new LevelAgent();
        mIsFight = false;
        AgentBase.InitBehavior();
        bool bRet = mAgent.btload(path);
        if (bRet)
        {
            mAgent.btsetcurrent(path);
            mAgent.SetLevelMgr(this);
        }
        else
        {
            return false;
        }
        mIsFight = true;
        return true;
    }

    public void DeInit()
    {
        if (mAgent != null)
        {
#if BEHAVIAC_NOT_USE_MONOBEHAVIOUR
            mAgent.UnLoad();
#endif
            mAgent.SetLevelMgr(null);
            mAgent = null;
        }
        mIsFight = false;
    }

    public void Update(int deltaTime)
    {
        if (mIsFight && mAgent != null)
        {
            mAgent.Tick(deltaTime);
            countTime += deltaTime;
            roomRunningTime += deltaTime;
        }

    }

    /// <summary>
    /// 获取指定类型的计数器的值
    /// </summary>
    public int GetCounter(int id)
    {
        if (!mCounterDic.ContainsKey(id))
            return -1;
        return mCounterDic[id];
    }

    /// <summary>
    /// 设置指定计数器的值计数器
    /// </summary>
    public void SetCounter(int id, int value)
    {
        if (!mCounterDic.ContainsKey(id))
        {
            mCounterDic.Add(id, value);
        }
        else
        {
            mCounterDic[id] = value;
        }
        OnWindChange(id, value);
    }

    /// <summary>
    /// 风向改变
    /// </summary>
    /// <param name="id"></param>
    public void OnWindChange(int id, int value)
    {
        if (id != (int)CounterTypeId.WIND_DIR)
            return;
#if !LOGIC_SERVER
        GeSceneEx geScene = GetGeScene();
        if (geScene == null)
            return;
        GeSkyRotateEx skyRotateEx = new GeSkyRotateEx();
        geScene.GeSpecialSceneEx = skyRotateEx;
        skyRotateEx.Init(geScene.GetSceneObject());
        skyRotateEx.SetSkyRotateData(value);
#endif
    }

    /// <summary>
    /// 获取当前场景
    /// </summary>
    /// <returns></returns>
    public BeScene GetBeScene()
    {
        if (baseBattle == null)
            return null;
        if (baseBattle.dungeonManager == null)
            return null;
        return baseBattle.dungeonManager.GetBeScene();
    }

    /// <summary>
    /// 获取场景表现相关
    /// </summary>
    /// <returns></returns>
    public GeSceneEx GetGeScene()
    {
#if !LOGIC_SERVER
        if (baseBattle == null)
            return null;
        if (baseBattle.dungeonManager == null)
            return null;
        return baseBattle.dungeonManager.GetGeScene();
#else
        return null;
#endif
    }
}

