using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IObjectPool
{
	string GetPoolName ();

	string GetPoolInfo();
	string GetPoolDetailInfo ();
}

public class CPoolManager : Singleton<CPoolManager>  {
	struct PoolDesc
	{
		public string name;
		public IObjectPool pool;
	}

	List<PoolDesc> pools = new List<PoolDesc>();


	bool HasPool(string name)
	{
		for(int i=0; i<pools.Count; ++i)
		{
			if (pools[i].name == name)
				return true;
		}

		return false;
	}

	public void RegisterPool(string name, IObjectPool pool)
	{
		if (!HasPool(name))
		{
			PoolDesc newDesc = new PoolDesc ();
			newDesc.name = name;
			newDesc.pool = pool;
			
			this.pools.Add (newDesc);
		}
	}

	public string GetPoolsInfo()
	{
		string info = "";
		for(int i=0; i<pools.Count; ++i)
		{
			var pool = pools [i];
			info += string.Format ("{0}:{1}\n", pool.pool.GetPoolName (), pool.pool.GetPoolInfo ());
		}

		return info;
	}

	public void Clear()
	{
		pools.Clear();
	}
}
