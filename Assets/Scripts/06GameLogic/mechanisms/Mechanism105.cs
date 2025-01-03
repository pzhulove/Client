using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少触发效果表中或BUFF信息表中的BUFF等级（固定值）
class Mechanism105 : BeMechanism
{
    int[] effectIdArray;
    int[] buffinfoIdArray;
    int addLevel;
    int parentMechanism;

    public Mechanism105(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        effectIdArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
            effectIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        buffinfoIdArray = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Length; i++)
            buffinfoIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        addLevel = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        parentMechanism = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onChangeBuffLevel, args =>
        {
            var source = args.m_Obj as BeActor;
            if(source == null)
                return;
            
            if (parentMechanism != 0 && source.GetMechanism(parentMechanism) == null)
                return;

            int eId = args.m_Int;
            int bId = args.m_Int2;
            if (Array.IndexOf(effectIdArray, eId) != -1 || Array.IndexOf(buffinfoIdArray, bId) != -1)
            {
                /*var buffLevelArray = (int[])args[2];
                buffLevelArray[0] += addLevel;*/

                args.m_Int3 += addLevel;
            }
        });
    }

}