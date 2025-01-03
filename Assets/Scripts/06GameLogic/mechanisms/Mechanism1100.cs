using System;
using System.Collections.Generic;
using GameClient;

//刷怪机制
public class Mechanism1100 : BeMechanism
{
    struct MonsterData
    {
        public int summonId;
        public int level;
        public int x;
        public int y;
    }

    private List<MonsterData> mMonsterTableData = new List<MonsterData>();
    private List<int> mIgnoreMonsterId = new List<int>();

    private int mSummonInterval = 10000;
    private int mTimer = 0;

    private BeEvent.BeEventHandleNew[] mMonsterEventHandle;//怪物死亡事件数组
    private int mExistSummonNum = 0;//场上存在的计数召唤物个数

    private int mNextSumMechanismId = 0;

    public Mechanism1100(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        //基本数据获取以及哪一波数量最多的计算
        mMonsterTableData.Clear();
        int length = Math.Min(data.ValueA.Length, data.ValueB.Length);
        for(int i = 0; i < length; ++i)
        {
            var tableData = new MonsterData();
            tableData.summonId = data.ValueA[i].fixInitValue;
            tableData.level = data.ValueA[i].fixLevelGrow;
            tableData.x = data.ValueB[i].fixInitValue;
            tableData.y = data.ValueB[i].fixLevelGrow;
            mMonsterTableData.Add(tableData);
        }
        mMonsterEventHandle = new BeEvent.BeEventHandleNew[length];

        mNextSumMechanismId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);

        mIgnoreMonsterId.Clear();
        for (int i = 0; i < data.ValueD.Count; ++i)
        {
            mIgnoreMonsterId.Add(TableManager.GetValueFromUnionCell(data.ValueD[i], level));
        }

        mSummonInterval = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnReset()
    {
        mMonsterTableData.Clear();
        mIgnoreMonsterId.Clear();
        mTimer = 0;
        ClearEventHandle();
        mExistSummonNum = 0;
    }

    public override void OnStart()
    {
        if (owner != null && owner.CurrentBeScene != null)
        {
            for (int i = 0; i < mMonsterTableData.Count; ++i)//召唤怪物
            {
                var monster = owner.CurrentBeScene.SummonMonster(mMonsterTableData[i].summonId + mMonsterTableData[i].level * 100, new VInt3(mMonsterTableData[i].x, mMonsterTableData[i].y, 0), owner.GetCamp());
                if (!mIgnoreMonsterId.Contains(mMonsterTableData[i].summonId))
                {
                    if (mExistSummonNum < mMonsterEventHandle.Length)
                    {
                        mMonsterEventHandle[mExistSummonNum] = monster.RegisterEventNew(BeEventType.onDead, OnMonsterDead);
                        mExistSummonNum++;
                    }
                }
            }
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        mTimer += deltaTime;
        if (mTimer >= mSummonInterval)  
        {
            DoNextSummon();
        }
    }

    public override void OnFinish()
    {
        ClearEventHandle();
    }

    private void DoNextSummon()
    {
        var tempMechanism = owner.GetMechanism(mNextSumMechanismId) as Mechanism1101;
        if (tempMechanism != null)
        {
            tempMechanism.TryAddOnceMechanism();
        }

        owner.RemoveMechanism(this);
    }

    private void ClearEventHandle()
    {
        for (int i = 0; i < mMonsterEventHandle.Length; ++i)
        {
            if (mMonsterEventHandle[i] != null)
            {
                mMonsterEventHandle[i].Remove();
                mMonsterEventHandle[i] = null;
            }
        }
    }

    private void OnMonsterDead(BeEvent.BeEventParam eventParam)
    {
        mExistSummonNum--;
        if (mExistSummonNum <= 0)
        {
            DoNextSummon();
        }
    }
}

