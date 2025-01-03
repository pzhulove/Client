using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using GameClient;

#region  BUFF_POOL
public class BeBuffFactory
{
    public static BeBuff CreateBuff(int buffID, int buffLevel, int buffDuration, int buffAttack = 0, bool buffEffectAni = true, bool abnormalBuff = false)
    {
        BeBuff buff = null;

        if (abnormalBuff)
        {
            buff = new BeAbnormalBuff(buffID, buffLevel, buffDuration, buffAttack, buffEffectAni);
        }
        else
        {
            var buffType = GetBuffType(buffID);
            if (null != buffType)
                buff = (BeBuff)Activator.CreateInstance(buffType, buffID, buffLevel, buffDuration, buffAttack);
            else
            {
                buff = new BeBuff(buffID, buffLevel, buffDuration, buffAttack, buffEffectAni);
            }
        }

        return buff;
    }

    public static Type GetBuffType(int buffID)
    {
        return TypeTable.GetType("Buff" + buffID.ToString());
    }
}

public class BeBuffPoolImp : IObjectPool
{
#region poolInfo
    string poolKey = "BeBuffPool";
	string poolName = "BeBuff池子\n";

    public string GetPoolName ()
	{
		return poolName;
	}

	public string GetPoolInfo()
	{
        return GetBuffPoolDesc();
	}

    public string GetBuffPoolDesc()
    {
        int count = 0;
        string content = "";
        foreach(var item in buffPool)
        {
            var queue = item.Value as Queue<BeBuff>;
            if (queue == null)
                continue;
            count += queue.Count;
            content += string.Format("{0}/{1}\n", item.Key, queue.Count);
        }

        content += string.Format("总{0}/{1}\n", count, total);

        return content;
    }


	public string GetPoolDetailInfo ()
	{
		return "detailInfo";
	}
#endregion    

    Dictionary<Type, Queue<BeBuff>> buffPool = new Dictionary<Type, Queue<BeBuff>>();

    int total = 0;
    int sid = 0;

    Type abnormalBuffType = null;
    Type normalBuffType = null;

    public BeBuffPoolImp()
    {
        abnormalBuffType = typeof(BeAbnormalBuff);
        normalBuffType = typeof(BeBuff);

        buffPool.Add(abnormalBuffType, new Queue<BeBuff>(5));
        buffPool.Add(normalBuffType, new Queue<BeBuff>(100));

        CPoolManager.GetInstance ().RegisterPool (poolKey, this);
    }

    public int GenID()
    {
        return ++sid;
    }

    private Type GetBuffType(int buffID, bool abnormalBuff)
    {
        Type type = null;
        if (abnormalBuff)
        {
            type = abnormalBuffType;
        }
        else
        {
            type = BeBuffFactory.GetBuffType(buffID);
            if (type == null)
                type = normalBuffType;
        }

        return type;
    }

    public BeBuff GetBuff(int buffID, int buffLevel, int buffDuration, int buffAttack = 0, bool buffEffectAni = true, bool abnormalBuff = false)
    {
        BeBuff buff = null;
        bool newCreated = false;

        Type type = GetBuffType(buffID, abnormalBuff);

        if (buffPool.ContainsKey(type) && buffPool[type] != null && buffPool[type].Count > 0)
            buff = buffPool[type].Dequeue();
        else
            newCreated = true;

        if (newCreated)
        {
            buff = BeBuffFactory.CreateBuff(buffID, buffLevel, buffDuration, buffAttack, buffEffectAni, abnormalBuff);
            total++;
        }
        else 
        {
            buff.Init(buffID, buffLevel, buffDuration, buffAttack, buffEffectAni);
        }
        
        return buff;
    }

    public void PutBuff(BeBuff buff)
    {
        if (buff == null)
            return;

        buff.Reset();

        var type = buff.GetType();

        if (!buffPool.ContainsKey(type))
            buffPool.Add(type, new Queue<BeBuff>());
        
        //检查重复加入的情况
        if (buffPool[type].Contains(buff))
        {
            Logger.LogErrorFormat("[buffpool]出现了已经放进pool的buff实例:pid-{0} buffid:{1}", buff.PID, buff.buffID, type);
            return;
        }
        buffPool[type].Enqueue(buff);
    }

    public void Clear()
    {
        buffPool.Clear();
        total = 0;
        sid = 0;
    }
}

#endregion