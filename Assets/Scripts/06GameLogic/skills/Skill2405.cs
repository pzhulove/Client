public class Skill2405 : BeSkill
{
    public Skill2405(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    private int[] m_BuffIdList;
    public override void OnInit()
    {
        base.OnInit();
        m_BuffIdList = new int[skillData.ValueA.Count];
        for (int i = 0; i < skillData.ValueA.Count; i++)
        {
            m_BuffIdList[i] = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
        }
    }

    /// <summary>
    /// 技能开始时清理场上的集火buff
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();
        if(owner.CurrentBeScene == null)
            return;
        
        var list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(list, owner, VInt.Float2VIntValue(100f));
        for (int i = 0; i < list.Count; i++)
        {
            var target = list[i];
            for (int j = 0; j < m_BuffIdList.Length; j++)
            {
                var buff = target.buffController.HasBuffByID(m_BuffIdList[j]);
                if(buff == null)
                    continue;
                if (buff.releaser != null && buff.releaser.IsSameTopOwner(owner))
                {
                    target.buffController.RemoveBuff(buff);
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }
}
