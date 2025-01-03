using GameClient;

/// <summary>
/// 催眠
/// 1、召唤兽会主动极快跑至角色身前（不会全部到一个位置，会有一个位置碰撞概念）
/// 2、无法对处于受击状态的召唤兽生效
/// 3、催眠成功后，召唤兽会进入恢复状态，持续恢复生命值
/// 4、每个召唤兽在睡眠状态都会获得一个护盾，可承受一定次数攻击不被打扰
/// 5、被打扰后的召唤兽会获得攻速和移速加成，且对攻击者额外有较高的仇恨值
///
/// 配置
/// A:承受攻击次数
/// B:随眠时间
/// C:发怒时间
/// D:指定寻路BuffID|恢复生命BuffInfoID|生气BuffID|整个流程的BuffID
/// </summary>
public class Mechanism3000 : BeMechanism
{
    public Mechanism3000(int mid, int lv) : base(mid, lv) { }

    private int mBlockCount;
    private int mCurBlockCount;
    private int mSleepTime;
    private int mCurSleepTime;
    private int mAngryTime;
    private int mCurAngryTime;
    private int mTogetherBuffInfoId;
    private int mSleepBuffInfoId;
    private int mAngryBuffInfoId;
    private int mThisBuffId;

    private BeActor topOwner;
    

    enum SleepState
    {
        Together,
        Sleep,
        GetUp,
        Angry
    }

    private SleepState mState;
    public override void OnInit()
    {
        mBlockCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mSleepTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        mAngryTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        
        mTogetherBuffInfoId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        mSleepBuffInfoId = TableManager.GetValueFromUnionCell(data.ValueD[1], level);
        mAngryBuffInfoId = TableManager.GetValueFromUnionCell(data.ValueD[2], level);
        mThisBuffId = TableManager.GetValueFromUnionCell(data.ValueD[3], level);
    }

    public override void OnReset()
    {
        mCurBlockCount = 0;
        mCurSleepTime = 0;
        mCurAngryTime = 0;
        topOwner = null;
        mState = SleepState.Together;
    }

    public override void OnStart()
    {
        mState = SleepState.Together;
        topOwner = owner.GetTopOwner(owner) as BeActor;
        mCurBlockCount = 0;
        DoTogether();
        handleA = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, OnHit);
        //handleA = owner.RegisterEvent(BeEventType.onAfterFinalDamageNew, OnHit);
        handleB = owner.RegisterEventNew(BeEventType.onAIMoveEnd, OnAIMoveEnd);

        owner.buffController?.TryAddBuffInfo(mTogetherBuffInfoId, owner, level);
    }

    private void OnAIMoveEnd(BeEvent.BeEventParam param)
    {
        if (mState == SleepState.Together)
        {
            DoSleep();
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        UpdateSleep(deltaTime);
        UpdateAngry(deltaTime);
    }

    private void UpdateAngry(int deltaTime)
    {
        if (mState != SleepState.Angry)
            return;
        
        mCurAngryTime += deltaTime;
        if (mAngryTime <= mCurAngryTime)
        {
            DoGetup();
        }   
    }

    private void UpdateSleep(int deltaTime)
    {
        if (mState != SleepState.Sleep)
            return;
        
        mCurSleepTime += deltaTime;
        if (mSleepTime <= mCurSleepTime)
        {
            owner.buffController?.RemoveBuff(mSleepBuffInfoId);
            DoGetup();
        }    
    }

    private void OnHit(BeEvent.BeEventParam param)
    {
        if (mState != SleepState.Sleep)
            return;

        mCurBlockCount++;
        param.m_Int = 1;
        if (mCurBlockCount >= mBlockCount)
        {
            DoAngry(param.m_Obj as BeEntity);
        }
    }

    /// <summary>
    /// 集合在一起
    /// </summary>
    private void DoTogether()
    {
        /*owner.aiManager.SetForceFollow(true);
        owner.aiManager.followTarget = topOwner;
        owner.aiManager.SetTarget(topOwner, true);
        owner.aiManager.warlike = 0;*/
    }
    
    private void DoSleep()
    {
        mCurSleepTime = 0;
        mState = SleepState.Sleep;
        owner.buffController?.TryAddBuffInfo(mSleepBuffInfoId, owner, level);
    }

    private void DoAngry(BeEntity attacker)
    {
        owner.buffController?.RemoveBuff(mSleepBuffInfoId);
        mCurAngryTime = 0;
        mState = SleepState.Angry;
        owner.aiManager?.SetTarget((BeActor) attacker, true);
        owner.buffController?.TryAddBuffInfo(mAngryBuffInfoId, owner, level);
    }

    private void DoGetup()
    {
        mState = SleepState.GetUp;
        owner.buffController?.RemoveBuff(mThisBuffId);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        owner.aiManager?.SetTarget(null);
        owner.buffController?.RemoveBuff(mThisBuffId);
        owner.buffController?.RemoveBuffByBuffInfoID(mSleepBuffInfoId);
        owner.buffController?.RemoveBuffByBuffInfoID(mTogetherBuffInfoId);
    }
}

