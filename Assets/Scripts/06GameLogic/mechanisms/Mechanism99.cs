using System;
using System.Collections.Generic;
using UnityEngine;

//每隔一段时间上一个buff信息ID
class Mechanism99 : BeMechanism
{
    int startCD;
    int CD;
    int[] buffInfoArray;
#if !LOGIC_SERVER
    string textInfo = string.Empty;
    float textLastTime = 0;
#endif
    int timer;

    public Mechanism99(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
#if !LOGIC_SERVER
        textInfo = string.Empty;
        textLastTime = 0;
#endif
    }
    public override void OnInit()
    {
        startCD = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        CD = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        buffInfoArray = new int[data.ValueC.Length];
        for (int i = 0; i < data.ValueC.Length; i++)
        {
            buffInfoArray[i] = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
        }
#if !LOGIC_SERVER
        if (data.StringValueA.Length > 0)
        {
            textInfo = data.StringValueA[0];
        }
        if (data.ValueD.Length > 0)
        {
            textLastTime = TableManager.GetValueFromUnionCell(data.ValueD[0], level) / 1000.0f;
        }
#endif
    }

    public override void OnStart()
    {
        timer = startCD;
    }

    public override void OnUpdate(int deltaTime)
    {
        timer -= deltaTime;
        if (timer <= 0)
        {
            for (int i = 0; i < buffInfoArray.Length; i++)
            {
                owner.buffController.TryAddBuff(buffInfoArray[i]);
            }

            timer = CD;
#if !LOGIC_SERVER
            if (textInfo.Length > 0)
            {
                GameClient.SystemNotifyManager.SysDungeonSkillTip(textInfo, textLastTime);
            }
#endif
        }
    }
}
