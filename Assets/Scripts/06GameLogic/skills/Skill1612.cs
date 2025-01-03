using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using ProtoTable;

public class Skill1612 : BeSkill
{
    struct _tmpBuffInfo
    {
        public List<int> skillIDs;
        public int buffID;
    };


    int buffEffectID;
    List<_tmpBuffInfo> buffInfos = new List<_tmpBuffInfo>();

    public Skill1612(int sid, int skillLevel):base(sid, skillLevel)
    {
        
    }

    public static void SkillPreloadRes(SkillTable tableData)
    {
#if !LOGIC_SERVER
        int effectId = TableManager.GetValueFromUnionCell(tableData.ValueA[0], 1);
        var effectTableData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(effectId);
        if (effectTableData != null)
            PreloadManager.PreloadBuffID(effectTableData.BuffID, null, null);

        PreloadManager.PreloadBuffID(TableManager.GetValueFromUnionCell(tableData.ValueB[0], 1), null, null);
        PreloadManager.PreloadBuffID(TableManager.GetValueFromUnionCell(tableData.ValueD[0], 1), null, null);
        PreloadManager.PreloadBuffID(TableManager.GetValueFromUnionCell(tableData.ValueF[0], 1), null, null);
#endif
    }

    public override void OnInit()
    {
        base.OnInit();

        buffInfos.Clear();

        buffEffectID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        for (int i = 0; i < 3; ++i)
        {
            ProtoTable.UnionCell v1 = null;

            IList<ProtoTable.UnionCell> v2 = null;

            if (i == 0)
            {
                v1 = skillData.ValueB[0];
                v2 = skillData.ValueC;
            }
            else if (i == 1)
            {
                v1 = skillData.ValueD[0];
                v2 = skillData.ValueE;
            }
            else if (i == 2)
            {
                v1 = skillData.ValueF[0];
                v2 = skillData.ValueG;
            }

            _tmpBuffInfo bf = new _tmpBuffInfo();
            bf.skillIDs = GetEffectSkills(v2, level);

            bf.buffID = TableManager.GetValueFromUnionCell(v1, level);

            buffInfos.Add(bf);
        }
    }

    public override void OnStart()
    {
        base.OnStart();

        DoEffect();
    }

    public void DoEffect()
    {
        if (owner != null)
        {
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(buffEffectID);
            if (data != null)
            {
                //上角色buff
                int duration = 0;
                if (BattleMain.IsChijiNeedReplaceHurtId(buffEffectID,battleType))
                {
                    var chijiEffectMapTable = TableManager.instance.GetTableItem<ProtoTable.ChijiEffectMapTable>(buffEffectID);
                    duration = TableManager.GetValueFromUnionCell(chijiEffectMapTable.AttachBuffTime, level);
                }
                else
                {
                    duration = TableManager.GetValueFromUnionCell(data.AttachBuffTime, level);
                }
                owner.buffController.TryAddBuff(data.BuffID, duration, level);

                //上skill buff
                for(int i=0; i< buffInfos.Count; ++i)
                {
                    var bf = buffInfos[i];
                    owner.buffController.AddBuffForSkill(bf.buffID, level, duration, bf.skillIDs);
                }
            }
        }
    }
}
