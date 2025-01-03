using System;
using System.Collections.Generic;
using UnityEngine;

//根据武器类型替换技能配置文件
class Mechanism135 : BeMechanism
{
    int originalPhaseId = 0;                                //原来的技能阶段ID
    List<int[]> replacePhaseIdList = new List<int[]>();     //替换后的技能阶段列表
    
    public Mechanism135(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        replacePhaseIdList.Clear();
    }
    public override void OnInit()
    {
        originalPhaseId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        int[] array1 = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Count; i++)
            array1[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        replacePhaseIdList.Add(array1);

        int[] array2 = new int[data.ValueC.Length];
        for (int i = 0; i < data.ValueC.Count; i++)
            array2[i] = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
        replacePhaseIdList.Add(array2);

        int[] array3 = new int[data.ValueD.Length];
        for (int i = 0; i < data.ValueD.Count; i++)
            array3[i] = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
        replacePhaseIdList.Add(array3);

        int[] array4 = new int[data.ValueE.Length];
        for (int i = 0; i < data.ValueE.Count; i++)
            array4[i] = TableManager.GetValueFromUnionCell(data.ValueE[i], level);
        replacePhaseIdList.Add(array4);

        int[] array5 = new int[data.ValueF.Length];
        for (int i = 0; i < data.ValueF.Count; i++)
            array5[i] = TableManager.GetValueFromUnionCell(data.ValueF[i], level);
        replacePhaseIdList.Add(array5);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
        {
            //int[] skillArray = (int[])args[0];
            if (param.m_Int == originalPhaseId)
            {
                for (int i = 0; i < replacePhaseIdList.Count; i++)
                {
                    if (replacePhaseIdList[i][0] == owner.GetWeaponType())
                    {
                        int[] newPhaseArray = new int[replacePhaseIdList[i].Length - 1];
                        for (int j = 1; j < replacePhaseIdList[i].Length; j++)
                        {
                            newPhaseArray[j - 1] = replacePhaseIdList[i][j];
                        }
                        owner.skillController.SetCurrentSkillPhases(newPhaseArray);
                    }
                }
            }
        });
    }

}