using System.Collections;
using System.Collections.Generic;
//using BehaviorDesigner.Runtime.Tasks.Basic.UnityParticleSystem;
using Network;
using UnityEngine;

namespace ProtoTable
{
    public interface ITable
    {
        int GetKey();
    }

    [System.Serializable]
    public class TableData : ScriptableObject
    {
        public virtual void Init()
        {
        }

        public virtual void InitItems()
        {
        }

        public virtual void BuildData(object[] data)
        {
        }

        public virtual IEnumerable<object> GetData()
        {
            yield break;
        }

        public virtual object[] GetAll()
        {
            return null;
        }

        public virtual Dictionary<int, object> GetMap()
        {
            return null;
        }

        public virtual bool IsEmpty()
        {
            return true;
        }
    }

    public class TableDic<T> : TableData
    {
        public T[] data;
        private Dictionary<int, object> dataMap = new Dictionary<int, object>();
        protected TableDic<T> InstanceDic;

        public override void Init()
        {
            InstanceDic = this;
            if (InstanceDic != null)
                InitInternal();
        }

        public override void BuildData(object[] data)
        {
            this.data = new T[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                var obj = (T)data[i];
                this.data[i] = obj;
           }
        }

        public override IEnumerable<object> GetData()
        {
            if(InstanceDic.data == null)
                yield break;

            foreach (T obj in InstanceDic.data)
            {
                yield return (object)obj;
            }

            yield break;
        }

        public new T[] GetAll()
        {
            return this.data;
        }

        public override Dictionary<int, object> GetMap()
        {
            return InstanceDic.dataMap;
        }

        public override bool IsEmpty()
        {
            return InstanceDic.data == null || InstanceDic.data.Length == 0;
        }

        private bool InitInternal()
        {
            InstanceDic.dataMap.Clear();
            int row = 0;
            foreach (object dataEntry in InstanceDic.data)
            {
                row++;
                int entry = ((ITable) dataEntry).GetKey();
                if (InstanceDic.dataMap.ContainsKey(entry))
                {
                   Logger.LogWarning("");
                }
                InstanceDic.dataMap.Add(entry, dataEntry);
            }
            OnInitComplete();
            return true;
        }


        protected virtual void OnInitComplete()
        {
        }
       
        public bool HasItem(int entry)
        {
            return this.dataMap.ContainsKey(entry);
        }
    }

}


