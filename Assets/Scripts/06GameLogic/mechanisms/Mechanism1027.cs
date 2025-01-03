using System.Collections.Generic;

/// <summary>
/// 指定ID的怪物全部死亡以后 机制拥有者释放某个技能
/// </summary>
public class Mechanism1027 : BeMechanism
{
    public Mechanism1027(int mid, int lv) : base(mid, lv) { }

    private List<int> monsterIdList = new List<int>();  //怪物ID列表
    private int skillId = 0;    //释放的技能ID
    private int existMonsterCount = 0;    //怪物存活数量

    private bool canUseSkillFlag = false;

    public override void OnInit()
    {
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            monsterIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        skillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        existMonsterCount = TableManager.GetValueFromUnionCell(data.ValueC[0],level);
    }

    public override void OnReset()
    {
        monsterIdList.Clear();
        canUseSkillFlag = false;
    }

    public override void OnStart()
    {
        canUseSkillFlag = true;
        RegisterMonsterSummon();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateMonsterState();
    }

    /// <summary>
    /// 监听怪物召唤
    /// </summary>
    private void RegisterMonsterSummon()
    {
        if (owner.CurrentBeScene == null)
            return;
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onSummon, (args) =>
        {
            BeActor monster = (BeActor)args.m_Obj;
            if (monster != null && ContainMonsterId(monster))
            {
                canUseSkillFlag = true;
            }
        });
    }

    /// <summary>
    /// 检测指定ID的怪物死亡 释放指定的技能
    /// </summary>
    private void UpdateMonsterState()
    {
        if (!canUseSkillFlag)
            return;
        if (owner.CurrentBeScene == null)
            return;
        if (GetMonstersCount() <= existMonsterCount)
        {
            owner.UseSkill(skillId, true);
            canUseSkillFlag = false;
        }
    }

    private bool ContainMonsterId(BeActor monster)
    {
        bool flag = false;
        for(int i = 0; i < monsterIdList.Count; i++)
        {
            if (monster.GetEntityData().MonsterIDEqual(monsterIdList[i]))
            {
                flag = true;
                break;
            }
        }
        return flag;
    }

    private int GetMonstersCount()
    {
        int count = 0;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        for(int i = 0; i < monsterIdList.Count; i++)
        {
            owner.CurrentBeScene.FindActorById2(list, monsterIdList[i]);
            count += list.Count;
        }
        GamePool.ListPool<BeActor>.Release(list);
        return count;
    }
}
