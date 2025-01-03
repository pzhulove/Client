using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

//气功 - 螺旋丸
public class Skill3117 : BeSkill
{
    protected int m_EntityId = 69044;                //螺旋丸实体ID

    public Skill3117(int sid, int skillLevel): base(sid, skillLevel)
	{
        
	}

    public override void OnCancel()
    {
        RemoveEntity();
    }

    public override void OnFinish()
    {
        RemoveEntity();
    }

    //移除实体
    protected void RemoveEntity()
    {
        List<BeEntity> allEntity = GamePool.ListPool<BeEntity>.Get();
        owner.CurrentBeScene.GetEntitys2(allEntity);
        if (allEntity.Count > 0)
        {
            for (int i = 0; i < allEntity.Count; i++)
            {
                if (allEntity[i].m_iResID == m_EntityId && allEntity[i].GetOwner() == owner && !allEntity[i].IsDead())
                {
                    allEntity[i].OnRemove();
                }
            }
        }
        GamePool.ListPool<BeEntity>.Release(allEntity);
    }
}
