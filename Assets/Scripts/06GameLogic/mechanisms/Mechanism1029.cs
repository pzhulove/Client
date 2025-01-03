using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 修改异常BUFF的攻击力
/// </summary>
public class Mechanism1029 : BeMechanism
{

    private int type;
    private int[] buffTypes;
    private int[] rates;
    private int[] addLevels;
    protected List<int> targetList = new List<int>();
    protected List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    public Mechanism1029(int id, int level) : base(id, level) { }

    public override void OnInit()
    {
        base.OnInit();
        type = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        buffTypes = new int[data.ValueB.Length];
        for (int i = 0; i < buffTypes.Length; i++)
        {
            buffTypes[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }
        rates = new int[data.ValueC.Length];
        addLevels = new int[data.ValueC.Length];
        for (int i = 0; i < rates.Length; i++)
        {
            rates[i] = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            addLevels[i] = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
        }
    }

    public override void OnReset()
    {
        RemoveHandle();
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onBeforeHit, args =>
        //handleA = owner.RegisterEvent(BeEventType.onBeforeHit, args =>
        {
            var target = args.m_Obj as BeActor;
            if (target != null && !targetList.Contains(target.GetPID()))
            {
                if (type == 0)
                {
                    var handle = target.RegisterEventNew(BeEventType.onBuffBeforePostInit, args1 =>
                    {
                        BeBuff buff = args1.m_Obj as BeBuff;
                        int index = Array.IndexOf(buffTypes, (int)buff.buffType);
                        if (buff != null && index != -1)
                        {
                            if (type == 0)
                            {
                                buff.buffAttack *= (VFactor.one + VFactor.NewVFactor(rates[index], 1000));
                            }
                        }
                    });
                    handleList.Add(handle);
                }
                else
                {
                    var handle = target.RegisterEventNew(BeEventType.OnChangeAbnormalBuffLevel, args1 =>
                    {
                        int type = args1.m_Int2;
                        int index = Array.IndexOf(buffTypes, type);
                        if (index != -1)
                        {
                            if (this.type == 1)
                            {
                                args1.m_Int = addLevels[index];
                            }
                        }
                    });
                    handleList.Add(handle);

                }           
                targetList.Add(target.GetPID());
                
            }
        });
    }

    public override void OnFinish()
    {
        targetList.Clear();
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        for (int i = 0; i < handleList.Count; i++)
        {
            handleList[i].Remove();
            handleList[i] = null;
        }
        handleList.Clear();
    }
}
