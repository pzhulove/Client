using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    class ComCache : MonoBehaviour
    {
        public delegate object OnItemCreate(GameObject go);
        public delegate void OnItemVisible(object script,object data);
        public delegate void OnItemRecycled(object script);
        public GameObject goPrefab;
        List<object> scripts = new List<object>();
        List<object> datas = new List<object>();
        List<object> recycleScripts = new List<object>();

        public OnItemCreate onItemCreate;
        public OnItemVisible onItemVisible;
        public OnItemRecycled onItemRecycled;

        public void SetItems(List<object> datas)
        {
            this.datas = datas;
            if(this.datas == null)
            {
                datas = this.datas = new List<object>();
            }

            if (datas.Count < scripts.Count)
            {
                for(int i = datas.Count; i < scripts.Count;++i)
                {
                    if(null != onItemRecycled)
                    {
                        onItemRecycled.Invoke(scripts[i]);
                    }
                    recycleScripts.Add(scripts[i]);
                }
                scripts.RemoveRange(datas.Count, scripts.Count - datas.Count);
            }
            else
            {
                int iRealCount = scripts.Count;
                int iCreateCount = datas.Count - scripts.Count;
                if (recycleScripts.Count == iCreateCount)
                {
                    scripts.InsertRange(scripts.Count,recycleScripts);
                    recycleScripts.Clear();
                }
                else if(recycleScripts.Count > iCreateCount)
                {
                    scripts.InsertRange(scripts.Count, recycleScripts.GetRange(0, iCreateCount));
                    recycleScripts.RemoveRange(0, iCreateCount);
                }
                else
                {
                    for (int i = recycleScripts.Count; i < iCreateCount; ++i)
                    {
                        GameObject current = GameObject.Instantiate(goPrefab);
                        if(null != onItemCreate)
                        {
                            scripts.Add(onItemCreate.Invoke(current));
                        }
                        Utility.AttachTo(current, goPrefab.transform.parent.gameObject);
                    }
                    scripts.InsertRange(scripts.Count, recycleScripts);
                    recycleScripts.Clear();
                }
            }

            if (null != onItemVisible)
            {
                for (int i = 0; i < datas.Count && i < scripts.Count; ++i)
                {
                    onItemVisible.Invoke(scripts[i], datas[i]);
                }
            }
        }
    }
}