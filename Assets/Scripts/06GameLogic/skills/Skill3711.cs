using System.Collections.Generic;

//圣光沁盾
public class Skill3711 : BeSkill
{
    public Skill3711(int sid, int skillLevel) : base(sid, skillLevel) { }

    private int[] monsterIdArr = new int[2];    //盾的怪物ID列表(PVE|PVP)
    private List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    public override void OnInit()
    {
        base.OnInit();
        monsterIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueA[0],level);
        monsterIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueA[1], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        RemoveHandle();
        IBeEventHandle handle1 = owner.RegisterEventNew(BeEventType.onStartPassDoor, (args) =>
        {
            SetMonsterDead();
        });
        handleList.Add(handle1);
        IBeEventHandle handle2 = owner.RegisterEventNew(BeEventType.onDeadTowerEnterNextLayer, (args) =>
        {
            SetMonsterDead();
        });
        handleList.Add(handle2);
    }

    //过门和过塔的时候清除盾
    private void SetMonsterDead()
    {
        if (owner == null || owner.CurrentBeScene == null)
            return;
        int monsterId = BattleMain.IsModePvP(battleType) ? monsterIdArr[1] : monsterIdArr[0];
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorById2(list, monsterId);
        if (list != null)
        {
            for(int i = 0; i < list.Count; i++)
            {
                list[i].DoDead();
#if !LOGIC_SERVER
                list[i].m_pkGeActor.SetActorVisible(false);
#endif
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    private void RemoveHandle()
    {
        for(int i=0;i< handleList.Count; i++)
        {
            if (handleList[i] != null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }
        }
        handleList.Clear();
    }
}
