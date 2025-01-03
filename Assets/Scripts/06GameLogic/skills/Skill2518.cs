using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Skill2518 : Skill2522
{
    public Skill2518(int sid, int skillLevel) : base(sid, skillLevel) { }

    private List<int> effectIdList = new List<int>(new int[] { 25180,25181});   //女漫游的多重爆头伤害ID
    private List<int> curFrameFlagList = new List<int>(new int[] { 251801, 2518001, 2518101, 2518201, 2518401 });     //标记ID
    
    protected override void OnSkillCurFrame(BeEvent.BeEventParam args)
    {
        int id = 0;
        if(int.TryParse(args.m_String, out id))
        {
            if (curFrameFlagList.Contains(id) && leftBulletNum > 0)
            {
                leftBulletNum--;
                SetSilverBulletCount();
            }
        }
        else
        {
            Logger.LogError(string.Format("tryParse failed {0} skillId {1}", args.m_String, skillID));
        }
    }

    protected override void OnAfterFinalDamageNew(GameClient.BeEvent.BeEventParam param)
    {
        int id = param.m_Int2;
        BeActor target = param.m_Obj as BeActor;
        if (effectIdList.Contains(id) && target != null && leftBulletNum > 0)
        {
            owner.DoAttackTo(target, attachEffectId, false, true);
        }
    }
}
