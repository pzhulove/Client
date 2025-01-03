using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
public class Skill1206 : BeSkill
{

    int weaponType = 0;
    List<int> effectSkills;
    int buffInfoID = 0;
    IBeEventHandle handler;
    public Skill1206(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        base.OnInit();
        weaponType = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        effectSkills = GetEffectSkills(skillData.ValueB, level);
        buffInfoID = BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(skillData.ValueC[1], level) : TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
    }

    public override void OnPostInit()
    {
        RemoveHandle();
        //test
        if (owner != null && owner.GetWeaponType() == weaponType)
        {
            DoEffect();
        }
    }

    public override void OnWeaponChange()
    {
        OnPostInit();
    }

    void RemoveHandle()
    {
        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }
    }

    void DoEffect()
    {
        handler = owner.RegisterEventNew(BeEventType.onCastSkill, args =>
        {
            int castSkillID = args.m_Int;
            if (castSkillID != 0 && effectSkills != null && effectSkills.Contains(castSkillID) && buffInfoID != 0)
            {
                BuffInfoData info = new BuffInfoData(buffInfoID, level);
                BeBuff buff = owner.buffController.TryAddBuff(info);
                if (buff != null)
                {
                    int num = owner.buffController.GetBuffCountByID(buff.buffID);
                    UpdateBuffInfo(false, num);

                    //注册buff结束更新UI
                    buff.RegisterEventNew(BeEventType.onBuffFinish, args2 =>
                    {
                        int thisBuffID = args2.m_Int;
                        int num2 = owner.buffController.GetBuffCountByID(thisBuffID);
                        num2--;

                        UpdateBuffInfo(false, num2);

                    });
                }

            }
        });

    }

    void UpdateBuffInfo(bool hide = false, int num = 0)
    {
#if !LOGIC_SERVER
        if (owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();;
            if (battleUI != null)
            {
                battleUI.SetBuffNum(num);
            }
        }
#endif
    }
}
