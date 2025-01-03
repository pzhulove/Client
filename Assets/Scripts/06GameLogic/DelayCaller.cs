using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct DelayCallUnitHandle
{
    public DelayCallUnit  unit;

    public uint           id;

    public bool IsValid()
    {
        if (null == unit)
        {
            return false;
        }

        if (unit.id != id)
        {
            return false;
        }
        
        return true;
    }

    public void SetRemove(bool isRemoved)
    {
        if (!IsValid())
        {
            return ;
        }

        unit.SetRemove(isRemoved);
    }
}

public class DelayCallUnit : PooledClassObj
{
    
    protected DelayCaller.Del func;
    // the time to first call
    protected int delayTime;

    protected int tacc = 0;
    protected bool canRemove = false;
    protected int sumtime = 0;

    // repeat time to call the 
    protected int repeatTime = 0;
    // repeat count 
    protected int repeatCount = 0;

    protected bool firstCall = true;
	protected bool needCallBeforeClear = false;

    public uint id;

	public DelayCallUnit()
    {
    }


    public void Init(int delayTime, DelayCaller.Del func, int repeatCount = 0, int repeatTime = 0, bool needClear=false)
    {
        this.func = func;
        this.delayTime = delayTime;
        this.repeatCount = repeatCount;
        this.repeatTime = repeatTime;
        this.needCallBeforeClear = needClear;

        firstCall = true;
        canRemove = false;
    }

    public override void OnReused()
    {
    }

    public override void OnRecycle()
    {
        func = null;
        delayTime = 0;

        tacc = 0;
        canRemove = false;
        sumtime = 0;

        repeatTime = 0;
        repeatCount = 0;

        firstCall = true;
        needCallBeforeClear = false;

        id = uint.MaxValue;
    }

    public void Update(int delta)
    {
        if (canRemove)
            return;

        sumtime += delta;
        tacc += delta;

        if (firstCall && tacc >= delayTime)
        {
            DoCall();
            tacc -= delayTime;
            firstCall = false;

            canRemove = repeatCount == 0;
            return;
        }
        else if (!firstCall && repeatCount != 0 && tacc >= repeatTime)
        {
            DoCall();
            --repeatCount;
            tacc -= repeatTime;
            canRemove = repeatCount == 0;
            return;
        }
// 
//         //保护处理
//         if (tacc >= delayTime)
//             canRemove = true;
    }

    public void DoCall()
    {
        if (func != null)
        {
            func();
        }
    }

    public bool CanRemove()
    {
        return canRemove;
    }

    public void SetRemove(bool flag)
    {
        canRemove = flag;
    }

	public bool NeedCallBeforeClear()
	{
		return needCallBeforeClear;
	}
}

public class DelayCaller : ObjectLeakDitector
{
    ClassObjListPool<DelayCallUnit> m_DelayCallUnitPool = new ClassObjListPool<DelayCallUnit>();

    public delegate void Del();

    List<DelayCallUnit> trunkList = new List<DelayCallUnit>();
    List<DelayCallUnit> pendingList = new List<DelayCallUnit>();

    public DelayCaller()
    {
    }

    bool CanDelayCallRemove(DelayCallUnit unit)
    {
        return unit.CanRemove();
    }

    public void Update(int deltaTime)
    {
        //trunkList.CopyTo(pendingList.ToArray());
        trunkList.AddRange(pendingList);
        pendingList.Clear();

        bool bCanRemove = false;
        for(int i = 0; i < trunkList.Count; ++i)
        {
            var item = trunkList[i];

            if (item == null)
            {
#if MG_TEST
                Logger.LogErrorFormat("DelayCaller:{0}","取出来的数据为空");
#endif
                continue;
            }

            item.Update(deltaTime);
            if(item.CanRemove())
            {
                bCanRemove = true;
            }
        }

        if(bCanRemove)
        {
            //trunkList.RemoveAll(CanDelayCallRemove);
            _RecycleUnusedUnit(trunkList);
        }
    }

    public void Cancel()
    {
        //trunkList.Clear();
        _ClearAndRecycleList(trunkList);
    }

    public void StopItem(DelayCallUnitHandle unit)
    {
        unit.SetRemove(true);
    }

    private static uint sHandleId = 0;

	public DelayCallUnitHandle DelayCall(int ms, Del del, int repeatcount = 0, int repeattime = 0, bool needCallBeforeClear=false)
    {
        sHandleId++;

        //var item = new DelayCallUnit(ms, del, repeatcount, repeattime, needCallBeforeClear);
        var item = m_DelayCallUnitPool.GetPooledObject();
        item.Init(ms, del, repeatcount, repeattime, needCallBeforeClear);
        item.id = sHandleId;
        pendingList.Add(item);

        DelayCallUnitHandle handle;
        handle.unit = item;
        handle.id = sHandleId;

        return handle;
    }

    public DelayCallUnitHandle RepeatCall(int ms, Del del, int repeatcount=9999999, bool immediate=false)
    {
        if (immediate)
            del();

        sHandleId++;
        //var item = new DelayCallUnit(ms, del, repeatcount, ms);
        var item = m_DelayCallUnitPool.GetPooledObject();
        item.Init(ms, del, repeatcount, ms);
        item.id = sHandleId;
        pendingList.Add(item);

        DelayCallUnitHandle handle;
        handle.unit = item;
        handle.id = sHandleId;

        return handle;
    }

    public void Clear()
    {
		for(int i=0; i<trunkList.Count; ++i)
		{
			var item = trunkList[i];
			if (!item.CanRemove() && item.NeedCallBeforeClear())
			{
				item.DoCall();
				item.SetRemove(true);
			}
		}

        //trunkList.Clear();
        //pendingList.Clear();

        _ClearAndRecycleList(trunkList);
        _ClearAndRecycleList(pendingList);
    }

    protected void _ClearAndRecycleList(List<DelayCallUnit> list)
    {
        for (int i = 0, icnt = list.Count; i < icnt; ++i)
        {
            m_DelayCallUnitPool.RecyclePooledObject(list[i]);
            list[i].SetRemove(false);
            list[i].id = uint.MaxValue;
        }

        list.Clear();
    }

    protected void _RecycleUnusedUnit(List<DelayCallUnit> list)
    {
        for (int i = list.Count -1; i >= 0; --i)
        {
            DelayCallUnit cur = list[i];
            if(CanDelayCallRemove(cur))
            {
                m_DelayCallUnitPool.RecyclePooledObject(cur);
                cur.SetRemove(false);
                cur.id = uint.MaxValue;

                list.RemoveAt(i);
            }
        }
    }
}

