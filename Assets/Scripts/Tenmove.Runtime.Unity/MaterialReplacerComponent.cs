using UnityEngine;
using GamePool;

namespace Tenmove.Runtime.Unity
{
    public class MaterialReplacerComponent : MonoBehaviour
    {
        public Renderer[] m_Renders;

        //接口触发替换材质
        public void SetMaterial(Material mat)
        {
            if (m_Renders == null)
            {
                m_Renders = this.GetComponentsInChildren<Renderer>(true);
            }
            if (m_Renders == null)
            {
                return;
            }
            foreach (var render in m_Renders)
            {
                render.material = mat;
            }
        }

        public void SetDoorMaterial(Material mat)
        {
            var effPrefix = Global.Settings.doorEffPrefix;
            string[] doorEffPrefix = string.IsNullOrEmpty(effPrefix) ? null : effPrefix.Split('|');
            var parents = ListPool<Transform>.Get();
            parents.Add(this.transform);
            int count = 1;
            int index = 0;
            while(parents != null && parents.Count > 0)
            {
                var parent = parents[index];
                int childCount = parent.childCount;
                for(int i = 0; i < childCount; i++)
                {
                    var tran = parent.GetChild(i);
                    if(!_IsNeedSetMaterial(tran, doorEffPrefix))
                    {
                        continue;
                    }
                    parents.Add(tran);
                    var render = tran.GetComponent<Renderer>();
                    if(render != null)
                    {
                        render.material = mat;
                    }
                }
                index++;
                if(index >= count)
                {
                    parents.RemoveRange(0, count);
                    index = 0;
                    count = parents.Count;
                }
            }

            parents.Clear();
            ListPool<Transform>.Release(parents);
        }

        private bool _IsNeedSetMaterial(Transform tran, string[] doorEffPrefix)
        {
            if(tran == null)
            {
                return false;
            }
            if(doorEffPrefix == null || doorEffPrefix.Length <= 0)
            {
                return true;
            }
            var name = tran.name;
            for(int i = 0; i < doorEffPrefix.Length; i++)
            {
                if (name.ToLower().StartsWith(doorEffPrefix[i].ToLower()))
                {
                    return false;
                }
            }
            return true;
            
        }
    }
}
