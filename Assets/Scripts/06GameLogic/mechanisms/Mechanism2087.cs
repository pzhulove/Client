using System;
using System.Collections.Generic;
using GameClient;
using ProtoTable;

//根据场上怪物数的不同，在每个怪物的位置产生实体，并且每个实体的触发效果，只对该怪物生效
//场上有2只怪的时候，每只怪物要打3次
//场上大于等于3只怪的时候，每只怪物最多打2次，总次数要达到6次
public class Mechanism2087 : BeMechanism
{
    List<int> m_entityIDSets = new List<int>();  //只有一个怪的时候实体集合
    List<VInt> m_entityOffsetSets = new List<VInt>();  //只有一个怪的时候创建实体偏移位置集合
    List<int> m_delayDamageTimeSets = new List<int>(); //只有一个怪的时候延迟创建实体时间集合
    List<int> m_delayHurtTimeSets = new List<int>(); //只有一个怪的时候创建实体后产生伤害时间集合
    List<int> m_entityIDSetsInMulti = new List<int>(); //多个怪的时候实体集合
    List<VInt> m_entityOffsetSetsInMulti = new List<VInt>(); //多个怪的时候创建实体偏移位置集合
    List<int> m_delayHurtTimeSetsInMulti = new List<int>();//多个怪的时候延迟创建实体时间集合
    List<int> m_delayDamageTimeSetsInMulti = new List<int>();//多个怪的时候延迟创建实体后产生伤害时间集合
    List<BeActor> m_monsterList = new List<BeActor>();  //场上所有怪物列表
    List<int> m_monsterSelectedIds = new List<int>();   //当有3个怪的时候，每个怪最多两次伤害的列表

    int m_durTime = 0;   //延迟创建时间戳
    //确定目标怪物以后需要的详细信息
    class EntityTargetInfo
    {
        public BeActor target;   //当产生实体的敌方单位
        public int entityId;    //待创建的实体id
        public VInt offset;     //创建实体的偏移
        public int hurtid;    //目标单位伤害时触发的效果id
        public int delayCreateTime; //待创建的实体的延迟时间
        public int delayHurtTime;   //待产生伤害的延迟时间
    };
    //创建实体后的延迟伤害信息
    public class HurtTargetInfo
    {
        public int timeStamp;  //创建实体时的时间戳
        public int delayHurtTime;  //延迟伤害时间
        public BeActor target;   //目标单位
        public int hurtid;  //目标单位伤害时的触发效果id
    };

    Queue<EntityTargetInfo> m_entityTargets = new Queue<EntityTargetInfo>();//选定要创建实体的目标列表
    Queue<HurtTargetInfo> m_hurtAttackTargets = new Queue<HurtTargetInfo>(); //要产生伤害的目标列表
    int m_hurtId = 0;
    int m_lastHurtId = 0;  //一个怪的情况下，最后一次伤害怪的触发效果id
    public Mechanism2087(int id, int level) : base(id, level) { }

    public static void MechanismPreloadRes(MechanismTable tabelData)
    {
#if !LOGIC_SERVER
        if (tabelData == null) return;

        for (int i = 0; i < tabelData.ValueA.Count; i++)
        {
            PreloadManager.PreloadResID(TableManager.GetValueFromUnionCell(tabelData.ValueA[i], 1), null, null, true);
        }
        for (int i = 0; i < tabelData.ValueD.Count; i++)
        {
            PreloadManager.PreloadResID(TableManager.GetValueFromUnionCell(tabelData.ValueD[i], 1), null, null, true);
        }

        PreloadManager.PreloadEffectID(TableManager.GetValueFromUnionCell(tabelData.ValueG[0], 1), null, null);
        PreloadManager.PreloadEffectID(TableManager.GetValueFromUnionCell(tabelData.ValueG[1], 1), null, null);
#endif
    }


    public override void OnInit()
    {
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_entityIDSets.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], 1));
            m_entityOffsetSets.Add(VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[i], 2),GlobalLogic.VALUE_1000));
            m_delayDamageTimeSets.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            m_delayHurtTimeSets.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
        
        for (int i = 0; i < data.ValueD.Count; i++)
        {
            m_entityIDSetsInMulti.Add(TableManager.GetValueFromUnionCell(data.ValueD[i], 1));
            m_entityOffsetSetsInMulti.Add(VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueD[i], 2), GlobalLogic.VALUE_1000));
            m_delayDamageTimeSetsInMulti.Add(TableManager.GetValueFromUnionCell(data.ValueE[i], level));
            m_delayHurtTimeSetsInMulti.Add(TableManager.GetValueFromUnionCell(data.ValueF[i], level));
        }
        m_hurtId = TableManager.GetValueFromUnionCell(data.ValueG[0], level);
        m_lastHurtId = TableManager.GetValueFromUnionCell(data.ValueG[1], level);
    }

    public override void OnReset()
    {
        m_entityIDSets.Clear();
        m_entityOffsetSets.Clear();
        m_delayDamageTimeSets.Clear();
        m_delayHurtTimeSets.Clear();
        m_entityIDSetsInMulti.Clear();
        m_entityOffsetSetsInMulti.Clear();
        m_delayHurtTimeSetsInMulti.Clear();
        m_delayDamageTimeSetsInMulti.Clear();
        m_monsterList.Clear();
        m_monsterSelectedIds.Clear();
        m_durTime = 0;
        m_entityTargets.Clear();
        m_hurtAttackTargets.Clear();
        m_hurtId = 0;
        m_lastHurtId = 0;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (m_entityTargets.Count <= 0 && m_hurtAttackTargets.Count <= 0) return;
        m_durTime += deltaTime;
        if(m_entityTargets.Count > 0)
        {
            var curInfo = m_entityTargets.Peek();
            if (curInfo.delayCreateTime <= m_durTime)
            {
                var pos = curInfo.target.GetPosition();
                pos.x = pos.x + curInfo.offset.i;
                var entity = owner.AddEntity(curInfo.entityId, pos);
                if(entity != null)
                {
                    entity.SetFace(true);
                }
                //延迟伤害
                m_hurtAttackTargets.Enqueue(new HurtTargetInfo
                {
                    target = curInfo.target,
                    delayHurtTime = curInfo.delayHurtTime,
                    timeStamp = m_durTime,
                    hurtid = curInfo.hurtid
                });
                m_entityTargets.Dequeue();

            }
        }
        if(m_hurtAttackTargets.Count > 0)
        {
            var curHurt = m_hurtAttackTargets.Peek();
            int curDurTime = m_durTime - curHurt.timeStamp;
            if (curHurt.delayHurtTime <= curDurTime)
            {
                if (curHurt.target != null && !curHurt.target.IsDead())
                {
                    owner._onHurtEntity(curHurt.target, curHurt.target.GetPosition(), curHurt.hurtid);
                }
                m_hurtAttackTargets.Dequeue();
            }
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        m_entityTargets.Clear();
        m_hurtAttackTargets.Clear();
        m_durTime = 0;
        if (owner == null || owner.CurrentBeScene == null) return;
        owner.CurrentBeScene.FindAllMonsters(m_monsterList,owner);
        if (m_monsterList.Count == 0) return;
        //选定怪物和实体id
        if (m_monsterList.Count > 1)
        {
            //只有两个怪的情况下，一个怪分一半的伤害
            if (m_monsterList.Count == 2)
            {
                var target1 = m_monsterList[0];
                int i = 0;
                int halfCount = m_entityIDSetsInMulti.Count / 2;
                for (i = 0; i< halfCount; i++)
                {

                    m_entityTargets.Enqueue(new EntityTargetInfo
                                            {
                                                target = target1,
                                                entityId = m_entityIDSetsInMulti[i],
                                                offset = m_entityOffsetSetsInMulti[i],
                                                delayCreateTime = m_delayDamageTimeSetsInMulti[i],
                                                hurtid = m_hurtId,
                                                delayHurtTime = m_delayHurtTimeSetsInMulti[i]
                    });
                }
                var target2 = m_monsterList[1];
                for(; i < m_entityIDSetsInMulti.Count;i++)
                {
                    m_entityTargets.Enqueue(new EntityTargetInfo
                    {
                        target = target2,
                        entityId = m_entityIDSetsInMulti[i],
                        offset = m_entityOffsetSetsInMulti[i],
                        delayCreateTime = m_delayDamageTimeSetsInMulti[i],
                        hurtid = m_hurtId,
                        delayHurtTime = m_delayHurtTimeSetsInMulti[i]
                    });
                }

            }
            else
            {
                //超过2个怪，那么每隔怪最多只能伤害2次
                m_monsterSelectedIds.Clear();
                int originCount = m_monsterList.Count;
                for (int i = 0; i < m_entityIDSetsInMulti.Count; i++)
                {
                    if (m_monsterList.Count <= 0)
                    {
                        Logger.LogErrorFormat("mechanism id {0} mutliCount {1} targetCount {2}", this.mechianismID, m_entityIDSetsInMulti.Count, originCount);
                        return;
                    }
                    int index = FrameRandom.InRange(0,m_monsterList.Count);
                    var target = m_monsterList[index];
                    int pid = target.GetPID();
                    if(m_monsterSelectedIds.Contains(pid))
                    {
                        m_monsterList.RemoveAt(index);
                    }
                    else
                    {
                        m_monsterSelectedIds.Add(pid);
                    }
                    m_entityTargets.Enqueue(new EntityTargetInfo
                    {
                        target = target,
                        entityId = m_entityIDSetsInMulti[i],
                        delayCreateTime = m_delayDamageTimeSetsInMulti[i],
                        offset = m_entityOffsetSetsInMulti[i],
                        hurtid = m_hurtId,
                        delayHurtTime = m_delayHurtTimeSetsInMulti[i]
                    });
                  
                }
            }

        }
        else
        {
            var target = m_monsterList[0];
            for (int i = 0; i < m_entityIDSets.Count; i++)
            {
                m_entityTargets.Enqueue(new EntityTargetInfo
                {
                    target = target,
                    entityId = m_entityIDSets[i],
                    offset = m_entityOffsetSets[i],
                    hurtid = i == m_entityIDSets.Count - 1 ? m_lastHurtId : m_hurtId,
                    delayCreateTime = m_delayDamageTimeSets[i],
                    delayHurtTime = m_delayHurtTimeSets[i]
                });
            }
        }
    }
}

