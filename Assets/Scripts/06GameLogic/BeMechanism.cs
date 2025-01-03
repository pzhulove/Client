using UnityEngine;
using System.Collections;
using ProtoTable;
using System;
using System.Reflection;
using GameClient;
using System.Collections.Generic;


public enum MechanismSourceType
{
    NONE = 0,
    EQUIP,
    SKILL
}

public class BeMechanism {
    protected int _id;
    public int PID { //唯一ID
        get { return _id;}
        set {_id = value;}
    }
	public CrypticInt32 level=1;
	public int mechianismID;
	public bool canRemove = true;
	public MechanismTable data;
	public BeActor owner { get; set; }//拥有者
    public MechanismSourceType sourceType = MechanismSourceType.NONE;

    // public BeBuff attachBuff = null;      //机制附着的Buff

    public int attachBuffPID = 0;
    public bool isRunning = false;  //机制是否还在运行中

    public BeBuff GetAttachBuff()
    {
        BeBuff ret = null;
        if (owner != null)
            return owner.buffController.GetBuffByPID(attachBuffPID);

        return ret;
    }
    public void RemoveAttachBuff()
    {
        if (owner != null)
            owner.buffController.RemoveBuffByPID(attachBuffPID);
    }

    protected IBeEventHandle handleA = null;
    protected IBeEventHandle handleB = null;
    protected IBeEventHandle handleC = null;
    protected IBeEventHandle handleD = null;

    protected IBeEventHandle handleNewA = null;

    protected IBeEventHandle sceneHandleA = null;
    protected IBeEventHandle sceneHandleB = null;

    protected IBeEventHandle SceneHandleNewA = null;

    protected List<IBeEventHandle> handleList = null;

    protected IBeEventHandle OwnerRegisterEventNew(BeEventType type, BeEvent.BeEventHandleNew.Function fn)
    {
        return RegisterEventNew(owner, type, fn);
    }

    protected IBeEventHandle RegisterEventNew(BeEntity t, BeEventType type, BeEvent.BeEventHandleNew.Function fn)
    {
        if (null == t)
        {
            return null;
        }

        return t.RegisterEventNew(type, fn);
    }

    protected IBeEventHandle OwnerRegisterEvent(BeEventType type,  BeEvent.BeEventHandleNew.Function fn)
    {
        return RegisterEvent(owner, type, fn);
    }

    protected IBeEventHandle RegisterEvent(BeEntity t, BeEventType type,  BeEvent.BeEventHandleNew.Function fn)
    {
        if (null == t)
        {
            return null;
        }

        return t.RegisterEventNew(type, fn);
    }


    protected DelayCaller ThisDelayCaller
    {
        get {
            if (delayCaller == null)
                delayCaller = new DelayCaller();
            return delayCaller;
        }
    }
    private DelayCaller delayCaller = null; //= new DelayCaller();  //添加机制自己的delaycall
    protected int runningTime = 0;      //运行时间
    
    protected int duration = 0; //机制存在时间
    
    
    private int curTimeAcc = 0; // 当前的时间间隔
    private int timeAcc = 0;// 更新时间间隔

    public FrameRandomImp FrameRandom
    {
        get
        {
#if !SERVER_LOGIC 
            if(owner.FrameRandom == null)
            {
                return BeSkill.randomForTown;
            }

 #endif

            return owner.FrameRandom;
        }
    }

    //测试用，会删掉
    public static Dictionary<string, int> mechanismCreatedRecord = new Dictionary<string, int>();

	public static BeMechanism Create(int mid, int lv=1)
	{
		BeMechanism mechanism = null;

		var mData = TableManager.GetInstance().GetTableItem<MechanismTable>(mid);

		if (mData != null)
        {
            string name = string.Empty;
            if (!string.IsNullOrEmpty(mData.BTPath))
            {
                name = "Mechanism-" + mData.BTPath;
                mechanism = new BehaviorTreeMechanism.BTMechanism(mid, lv);
            }
            else
            {
                name = "Mechanism" + mData.Index;
            
                Type type = TypeTable.GetType(name);
                if (null != type)
                    mechanism = (BeMechanism)Activator.CreateInstance(type, mid, lv);
                else
                    mechanism = new BeMechanism(mid, lv);
            }

            if (!mechanismCreatedRecord.ContainsKey(name))
                mechanismCreatedRecord.Add(name, 0);
            mechanismCreatedRecord[name]++;
			
        }

		return mechanism;
	}

    public static BeMechanism Create(Type type, int mid, int lv = 1)
    {
        BeMechanism mechanism = null;
        if (null != type)
            mechanism = (BeMechanism)Activator.CreateInstance(type, mid, lv);
        else
            mechanism = new BeMechanism(mid, lv);
        return mechanism;
    }

	public BeMechanism(int mid, int lv)
	{
		Init(mid, lv);
	}

    public void Init(int mid, int lv)
    {
        owner = null;
        level = lv;
		mechianismID = mid;
		if (mid > 0)
		{
			data = TableManager.GetInstance().GetTableItem<MechanismTable>(mid);
            SetCanRemoveFlag();
        }
    }

    public BattleType battleType
    {
        get
        {
            return owner.battleType;
        }
    }

    public void Reset()
    {
        level = 1;
        mechianismID = 0;
        canRemove = true;
        data = null;
        // owner = null;
        sourceType = MechanismSourceType.NONE;
        attachBuffPID = 0;
        isRunning = false;
        handleA = null;
        handleB = null;
        handleC = null;
        handleD = null;
        sceneHandleA = null;
        sceneHandleB = null;
        if (handleList != null)
            handleList.Clear();
        if (delayCaller != null)
            delayCaller.Clear();
        runningTime = 0; 
        duration = 0;
        curTimeAcc = 0;
        timeAcc = 0;

        OnReset();
    }

	public void Init(int time = 0)
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeMechanism.Init"))
        {
#endif
        duration = time;
        OnInit();
#if ENABLE_PROFILER
        }
#endif
	}

	public void Update(int deltaTime)
	{
        if (!isRunning)
            return;
        OnUpdate(deltaTime);
        UpdateDelayCall(deltaTime);
        UpdateTimeAcc(deltaTime);
        UpdateLifeTime(deltaTime);
    }

    /// <summary>
    /// 受攻速影响的机制Update
    /// </summary>
    /// <param name="deltaTime"></param>
    public void UpdateImpactBySpeed(int deltaTime)
    {
        if (!isRunning)
            return;
        OnUpdateImpactBySpeed(deltaTime);
    }
    
    /// <summary>
    /// 检测存活的生命周期
    /// </summary>
    private void UpdateLifeTime(int deltaTime)
    {
        //Buff添加的机制不进行处理
        if (duration <= 0)
            return;
        runningTime += deltaTime;
        if (runningTime >= duration)
        {
            Finish();
        }
    }

    public void UpdateGraphic(int deltaTime)
	{
		OnUpdateGraphic(deltaTime);
	}

	public void Start()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeMechanism.Start"))
        {
#endif
        RemoveAllHandles();
        RemoveSceneHandle();
        isRunning = true;
        runningTime = 0;
        OnStart();
#if ENABLE_PROFILER
        }
#endif
	}

	public void Finish()
	{
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeMechanism.Finish"))
        {
#endif
        if (!isRunning)
            return;
        isRunning = false;
        RemoveAllHandles();
        RemoveSceneHandle();
        RemoveDelayCall();
        OnFinish();
#if ENABLE_PROFILER
        }
#endif
	}

    /// <summary>
    /// 初始化计时器
    /// </summary>
    /// <param name="updateTime">更新频率</param>
    protected void InitTimeAcc(int updateTime)
    {
        timeAcc = updateTime;
    }

    /// <summary>
    /// 公用计时器
    /// </summary>
    private void UpdateTimeAcc(int deltaTIme)
    {
        if (timeAcc <= 0)
            return;
        curTimeAcc -= deltaTIme;
        if (curTimeAcc <= 0)
        {
            curTimeAcc += timeAcc;
            OnUpdateTimeAcc();
        }
    }

    public void UpdateDelayCall(int deltaTime)
    {
        if (delayCaller != null)
            delayCaller.Update(deltaTime);
    }

    private void RemoveDelayCall()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeMechanism.RemoveDelayCall"))
        {
#endif
        if (delayCaller != null)
        {
            delayCaller.Clear();
            delayCaller = null;
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void RemoveAllHandles()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeMechanism.RemoveAllHandles"))
        {
#endif
        if (handleA != null)
        {
            handleA.Remove();
            handleA = null;
        }

        if (handleB != null)
        {
            handleB.Remove();
            handleB = null;
        }

        if (handleC != null)
        {
            handleC.Remove();
            handleC = null;
        }

        if(handleD != null)
        {
            handleD.Remove();
            handleD = null;
        }

        if(handleNewA != null)
        {
            handleNewA.Remove();
            handleNewA = null;
        }

        if (handleList != null)
        {
            for (int i = 0; i < handleList.Count; i++)
            {
                handleList[i].Remove();
            }
            handleList.Clear();
            handleList = null;
        }

#if ENABLE_PROFILER
        }
#endif
    }

    protected void RemoveSceneHandle()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeMechanism.RemoveSceneHandle"))
        {
#endif
        if (owner.CurrentBeScene == null)
            return;

        if(sceneHandleA != null)
        {
            owner.CurrentBeScene.RemoveHandle(sceneHandleA);
            sceneHandleA = null;
        }

        if (sceneHandleB != null)
        {
            owner.CurrentBeScene.RemoveHandle(sceneHandleB);
            sceneHandleB = null;
        }

        if (SceneHandleNewA != null)
        {
            owner.CurrentBeScene.RemoveHandle(SceneHandleNewA);
            SceneHandleNewA = null;
        }
#if ENABLE_PROFILER
        }
#endif
    }

    protected void AddHandle(IBeEventHandle handle)
    {
        if (handleList == null)
        {
            handleList = new List<IBeEventHandle>();
        }
        handleList.Add(handle);
    }

	protected void SetCanRemoveFlag()
    {
        if (data == null)
            return;
        if (data.RemoveFlag == 0)
            return;
        canRemove = data.RemoveFlag == 1 ? true:false;
    }

    //获取机制运行时长
    public int GetRunningTime()
    {
        return runningTime;
    }

    //获取机制Buff的释放者
    public BeActor GetAttachBuffReleaser()
    {
     /*   if (attachBuff == null)
            return null;*/

        var buff = GetAttachBuff();
        if (buff == null)
            return null;
        return buff.releaser;
    }

    public void DealDead()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeMechanism.DealDead"))
        {
#endif
        OnDead();
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 获取机制序号
    /// </summary>
    /// <returns></returns>
    public int GetMechanismIndex()
    {
        if (data == null)
            return -1;
        return data.Index;
    }
    
    /// <summary>
    /// 获取最初始的召唤主
    /// </summary>
    public BeActor GetTopOwner()
    {
        return owner.GetTopOwner(owner) as BeActor;
    }

    public virtual void OnReset(){}
    public virtual void OnInit(){}
	//public virtual void OnPostInit(){}
	public virtual void OnStart(){}
	public virtual void OnFinish(){}
	public virtual void OnUpdate(int deltaTime){}
	public virtual void OnUpdateGraphic(int deltaTime){}
    public virtual void OnDead() { }
    public virtual void OnUpdateTimeAcc() { }
    public virtual void OnUpdateImpactBySpeed(int deltaTime) { }
}
