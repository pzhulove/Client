using UnityEngine;
using System.Collections;
using ProtoTable;
using System;
using System.Reflection;
using GameClient;
using System.Collections.Generic;

public class BeMechanismPoolImp : IObjectPool
{
#region poolInfo
    string poolKey = "BeMechanismPool";
	string poolName = "BeMechanism池子\n";

    public string GetPoolName ()
	{
		return poolName;
	}

	public string GetPoolInfo()
	{
        return GetPoolDesc();
	}

    public string GetPoolDesc()
    {
        int count = 0;
        string content = "";
        foreach(var item in mechanismPool)
        {
            count += item.Value.Count;
            content += string.Format("{0}:{1}\n", item.Key, item.Value.Count);
        }
        content += string.Format("total:{0}/{1}", count, total);

        return content;
    }


	public string GetPoolDetailInfo ()
	{
		return "detailInfo";
	}
#endregion    
    Dictionary<Type, Queue<BeMechanism>> mechanismPool = new Dictionary<Type, Queue<BeMechanism>>();

    int total = 0;
    int sid = 0;

    Type normalMechansimType = null;

    public BeMechanismPoolImp()
    {
        normalMechansimType = typeof(BeMechanism);
        CPoolManager.GetInstance ().RegisterPool (poolKey, this);
    }

    public int GetID()
    {
        return ++sid;
    }

    private Type GetMechanismType(int mid)
    {
        var mData = TableManager.GetInstance().GetTableItem<MechanismTable>(mid);
        if (mData == null)
            return null;

        Type type = null;
        if (!string.IsNullOrEmpty(mData.BTPath))
        {
            type = typeof(BehaviorTreeMechanism.BTMechanism);
        }
        else
        {
            string name = "Mechanism" + mData.Index;    
            type = TypeTable.GetType(name);
        }
        if (type == null)
            type = normalMechansimType;

        return type;
    }

    public BeMechanism GetMechanism(int mid, int lv=1)
    {
        BeMechanism  mechanism = null;
        bool newCreated = false;

        Type type = GetMechanismType(mid);
        if (type == null)
        {
            Logger.LogErrorFormat("找不到该Id的机制:{0},请策划检查表格配置", mid);
            return null;
        }
        if (mechanismPool.ContainsKey(type) && mechanismPool[type] != null && mechanismPool[type].Count > 0)
            mechanism = mechanismPool[type].Dequeue();
        else
            newCreated = true;
        
        if (newCreated)
        {
            mechanism = BeMechanism.Create(type, mid, lv);
            total++;
        }
        else 
        {
            mechanism.Init(mid, lv);
        }

        return mechanism;
    }

    public void PutMechanism(BeMechanism mechanism)
    {
        if (mechanism == null)
            return;
        
        mechanism.Reset();

        var type = mechanism.GetType();
        if (!mechanismPool.ContainsKey(type))
            mechanismPool.Add(type, new Queue<BeMechanism>());
        
        if (mechanismPool[type].Contains(mechanism))
        {
            Logger.LogErrorFormat("[mechanismPool]出现了已经放进pool的机制实例:pid-{0} mechanismid:{1} index:{2} type:{3}", mechanism.PID, mechanism.mechianismID, mechanism.data.Index, type);
            return ;
        }

        mechanismPool[type].Enqueue(mechanism);
    }

   public void Clear()
   {
       mechanismPool.Clear();
       total = 0;
       sid = 0;
   }

}

