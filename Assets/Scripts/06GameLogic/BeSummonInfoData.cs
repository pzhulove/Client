using System.Collections;
using System.Collections.Generic;
using GameClient;
using ProtoTable;

public enum ForceFollowType
{
    NONE = 0,           //不跟随
    FORCE,              //强制跟随
    AVOID_MONSTER,      //避怪跟随
}

public class BeSummonInfoData
{
    public int summonID;
    public int summonDisplay;
    public bool isSummonEnterVisionForbid;//召唤渐显是否禁止
    public int summonNum;
    public int summonLevel;
    public int singleNumLimit;//此召唤数量上限（0表示没有上限）
    public int groupNumLimit; //召唤组数量上限（0表示没有上限）
    public int summonGroup;//召唤数量组ID（0表示没有）
    public bool related;
    public List<int> summonRandList;
    public bool killLastSummon; //是否将上次召唤的怪物杀死    
    public ForceFollowType forceFollow;//强制跟随选项

    public int summonMonsterSkillLevel = 0;

    public int summonPosType;
    public IList<int> summonPosType2;
    public VInt3 summonPos;
    public VInt radius;
    public VInt radiusAngle;
    public VInt2 offset;
    public int dir;

    public bool considerBlock;

    public int skillID;
    public string birthActionName;
    public int lifeTime;

    public bool useCampInTable;//使用表里的阵营
    public VInt3 basePostion = default(VInt3);
    public bool baseFace;
    public bool IsNotShowHpBar;
    public bool IsNameNotShowLv;

    public bool useSummonOwnerLevel = false;//使用召唤者的等级
    public bool useSummonTable = false;

    public BeSummonInfoData(int summonInfoID, int skillLevel = 0,int skillID = 0)
    {
        SetInfoData(summonInfoID, skillLevel, skillID);
    }

    public BeSummonInfoData(EffectTable hurtData, int skillLevel, BattleType battleType = BattleType.BattlegroundPVE)
    {
        if (0 != hurtData.SummonInfoID)
        {
            SetInfoData(hurtData.SummonInfoID, skillLevel, hurtData.SkillID);
        }
        else
        {
            summonID = hurtData.SummonID;
            if (BattleMain.IsChijiNeedReplaceHurtId(hurtData.ID, battleType))
            {
                var chijiEffectMapTable = TableManager.instance.GetTableItem<ChijiEffectMapTable>(hurtData.ID);
                summonLevel = TableManager.GetValueFromUnionCell(chijiEffectMapTable.SummonLevel, skillLevel);
            }
            else
            {
                summonLevel = TableManager.GetValueFromUnionCell(hurtData.SummonLevel, skillLevel);
            }
            summonPosType = (int)hurtData.SummonPosType;
            summonPosType2 = hurtData.SummonPosType2;
            summonNum = TableManager.GetValueFromUnionCell(hurtData.SummonNum, skillLevel);
            singleNumLimit = hurtData.SummonNumLimit;
            groupNumLimit = TableManager.GetValueFromUnionCell(hurtData.SummonGroupNumLimit, skillLevel);
            summonGroup = hurtData.SummonGroup;
            killLastSummon = hurtData.KillLastSummon != 0;
            related = hurtData.SummonRelation != 0;
            summonMonsterSkillLevel = 0;
            if (related)
                summonMonsterSkillLevel = skillLevel;
            if (summonLevel <= 0)
                summonLevel = skillLevel;
            summonDisplay = hurtData.SummonDisplay;

            if (hurtData.SummonRandList[0] > 0)
            {
                summonRandList = new List<int>();
                for (int i = 0; i < hurtData.SummonRandList.Count; ++i)
                    summonRandList.Add(hurtData.SummonRandList[i]);
            }

            birthActionName = "";
            lifeTime = 0;
            isSummonEnterVisionForbid = false;
            considerBlock = true;
            forceFollow = ForceFollowType.NONE;
            useCampInTable = false;
            IsNotShowHpBar = false;
            IsNameNotShowLv = false;
            dir = 0;
            useSummonOwnerLevel = false;
            offset = VInt2.zero;
            summonPos = VInt3.zero;
        }
    }


    private void SetInfoData(int summonInfoID, int level, int sid)
    {
        var summonData = TableManager.GetInstance().GetTableItem<ProtoTable.SummonInfoTable>(summonInfoID);
        if (summonData == null)
            return;
        useSummonTable = true;
        skillID = sid;
        birthActionName = summonData.BirthActionName;
        summonID = summonData.SummonID;
        lifeTime = summonData.LifeTime;

        summonLevel = TableManager.GetValueFromUnionCell(summonData.SummonLevel, level);
        summonNum = TableManager.GetValueFromUnionCell(summonData.SummonNum, level);

        singleNumLimit = summonData.SummonNumLimit;
        summonGroup = summonData.SummonGroup;
        groupNumLimit = TableManager.GetValueFromUnionCell(summonData.SummonGroupNumLimit, level);

        summonDisplay = summonData.SummonDisplay;
        isSummonEnterVisionForbid = summonData.isSummonEnterVisionForbid;

        related = summonData.SummonRelation != 0;
        considerBlock = summonData.ConsiderBlock > 0;
        killLastSummon = summonData.KillLastSummon != 0;
        forceFollow = (ForceFollowType)summonData.ForceFollow;
        useCampInTable = summonData.UseCampInTable;

        IsNotShowHpBar = summonData.IsNotShowHpBar;
        IsNameNotShowLv = summonData.IsNameNotShowLv;

        if (related)
            summonMonsterSkillLevel = level;
        if (summonLevel == -1)
            useSummonOwnerLevel = true;
        else if (summonLevel <= 0)
            summonLevel = level;

        if (summonData.SummonRandList[0] > 0)
        {
            summonRandList = new List<int>();
            for (int i = 0; i < summonData.SummonRandList.Count; ++i)
                summonRandList.Add(summonData.SummonRandList[i]);
        }

        dir = summonData.SummonDir;

        summonPosType = (int)summonData.SummonPosType;
        summonPosType2 = null;
        if (summonPosType == (int)ProtoTable.SummonInfoTable.eSummonPosType.FACE_OFFSET)
        {
            offset = new VInt2(1, 0);
            if (summonData.SummonParam.Count > 0 && summonData.SummonParam[0] != 0)
            {

                offset.x = new VInt(summonData.SummonParam[0] / (float)GlobalLogic.VALUE_1000).i;
            }
            if (summonData.SummonParam.Count > 1 && summonData.SummonParam[1] != 0)
            {
                offset.y = new VInt(summonData.SummonParam[1] / (float)GlobalLogic.VALUE_1000).i;
            }
        }
        else if (summonPosType == (int)ProtoTable.SummonInfoTable.eSummonPosType.RANDOM)
        {
            radius = VInt.one;
            if (summonData.SummonParam.Count > 0 && summonData.SummonParam[0] > 0)
            {
                radius = new VInt(summonData.SummonParam[0] / (float)GlobalLogic.VALUE_1000);
            }

            radiusAngle = new VInt(360);
            if (summonData.SummonParam.Count > 1 && summonData.SummonParam[1] > 0 && summonData.SummonParam[1] <= 360)
            {
                radiusAngle = new VInt(summonData.SummonParam[1]);
            }
        }
        else if (summonPosType == (int)ProtoTable.SummonInfoTable.eSummonPosType.POSITION)
        {
            summonPos = VInt3.zero;
            if (summonData.SummonParam.Count >= 3)
            {
                summonPos = new VInt3(
                    summonData.SummonParam[0] / (float)GlobalLogic.VALUE_1000,
                    summonData.SummonParam[1] / (float)GlobalLogic.VALUE_1000,
                    summonData.SummonParam[2] / (float)GlobalLogic.VALUE_1000);
            }
        }
    }
}

