using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//当自身有某个BUFF时，将自身的力量或者智力，换算一部分加持到力量或者智力上
public class Mechanism2036 : BeMechanism
{
    private int buffIDCondition;
    private int srcAttrID;
    private VFactor factor = VFactor.zero;
    private int desAttrID;
    private int totalAddValue = 0;
    private int addCount = 0;
    //(0:力量 1: 智力 2:体力 3:精神)
    private AttributeType[] attrTypes = new AttributeType[] { AttributeType.baseAtk, AttributeType.baseInt, AttributeType.baseSta, AttributeType.baseSpr, AttributeType.maxHp, AttributeType.maxMp };
    public Mechanism2036(int mid, int lv) : base(mid, lv)
    {
    }
    public override void OnInit()
    {
        base.OnInit();
        buffIDCondition = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        srcAttrID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        int intFactor = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        factor = VFactor.NewVFactor(intFactor, GlobalLogic.VALUE_1000);
        desAttrID = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnReset()
    {
        totalAddValue = 0;
        addCount = 0;
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onAddBuff, (args) =>
        {
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff == null || buff.buffID != buffIDCondition) return;
            if (owner == null || owner.buffController == null) return;
            if (owner.buffController.GetBuffCountByID(buff.buffID) > 1)
            {
                return;
            }

            if (srcAttrID < 0 || srcAttrID >= attrTypes.Length)
            {
                Logger.LogErrorFormat("mechanism id {0} srcAttrID  is out of Range {1}", mechianismID, srcAttrID);
                return;
            }
            var srcAttrType = attrTypes[srcAttrID];
            var srcAttrValue = owner.attribute.GetAttributeValue(srcAttrType);
            if (srcAttrType == AttributeType.maxHp)
            {
                srcAttrValue = owner.attribute.GetMaxHP() * GlobalLogic.VALUE_1000;
            }
            else if (srcAttrType == AttributeType.maxMp)
            {
                srcAttrValue = owner.attribute.GetMaxMP() * GlobalLogic.VALUE_1000;
            }
            if (desAttrID < 0 || desAttrID >= attrTypes.Length)
            {
                Logger.LogErrorFormat("mechanism id {0} desAttrID  is out of Range {1}", mechianismID, srcAttrID);
                return;
            }
            var desAttrType = attrTypes[desAttrID];
            var desAttrValue = owner.attribute.GetAttributeValue(desAttrType);
            var addValue = srcAttrValue * factor;
            totalAddValue += addValue;
          //  Logger.LogErrorFormat("[onAddBuff]mechanism id {0} srcAttrID {1} srcAttrValue {2} desAttrID {3} desAttrValue {4} addValue {5} buffID {6}", mechianismID, srcAttrType, srcAttrValue, desAttrType, desAttrValue, addValue,buffIDCondition);
            owner.attribute.SetAttributeValue(desAttrType, desAttrValue + addValue);
        });

        handleB = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
        {
            int buffId = (int)args.m_Int;
            if (owner == null || owner.buffController == null) return;
            if (buffId != buffIDCondition) return;
            if (owner.buffController.GetBuffCountByID(buffId) > 1)
            {
                return;
            }
            if (desAttrID < 0 || desAttrID >= attrTypes.Length)
            {
                Logger.LogErrorFormat("onRemoveBuff mechanism id {0} desAttrID  is out of Range {1}", mechianismID, srcAttrID);
                return;
            }
            var srcAttrType = attrTypes[srcAttrID];
            var srcAttrValue = owner.attribute.GetAttributeValue(srcAttrType);
            var desAttrType = attrTypes[desAttrID];
            var desAttrValue = owner.attribute.GetAttributeValue(desAttrType);
        //    Logger.LogErrorFormat("[onRemoveBuff]mechanism id {0} srcAttrID {1} srcAttrValue {2} desAttrID {3} desAttrValue {4} addValue {5} buffID {6}", mechianismID, srcAttrType, srcAttrValue,desAttrType, desAttrValue, totalAddValue,buffIDCondition);
            owner.attribute.SetAttributeValue(desAttrType, desAttrValue < totalAddValue ? 0 : desAttrValue - totalAddValue); //保护
            totalAddValue = 0;
        });

    }
}
