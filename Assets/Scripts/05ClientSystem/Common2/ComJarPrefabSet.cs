using UnityEngine;
using System.Collections;

namespace GameClient
{
    public enum JarPrefabType
    {
        JPT_IDLE = 0,
        JPT_WIN = 1,
        JPT_NEW = 2,
    }

    class ComJarPrefabSet : MonoBehaviour
    {
        public JarPrefabType eJarPrefabType = JarPrefabType.JPT_IDLE;
        GameObject goJar = null;
        public bool bUsePre = false;

        // Use this for initialization
        void Start()
        {
            if(goJar == null)
            {
                var jarItem = bUsePre ? BudoManager.GetInstance().GetPreJarItemByTimes() : BudoManager.GetInstance().GetJarItemByTimes();
                if(jarItem != null)
                {
                    if(eJarPrefabType == JarPrefabType.JPT_IDLE)
                    {
                        goJar = AssetLoader.instance.LoadRes(jarItem.idles).obj as GameObject;
                    }
                    else if(eJarPrefabType == JarPrefabType.JPT_WIN)
                    {
                        goJar = AssetLoader.instance.LoadRes(jarItem.wins).obj as GameObject;
                    }
                    else if(eJarPrefabType == JarPrefabType.JPT_NEW)
                    {
                        goJar = AssetLoader.instance.LoadRes(jarItem.news).obj as GameObject;
                    }

                    if(goJar != null)
                    {
                        Utility.AttachTo(goJar, gameObject);
                        goJar.CustomActive(true);
                    }
                }
            }
        }

        void OnDestroy()
        {
            if(goJar != null)
            {
                goJar.transform.SetParent(null);
                Object.Destroy(goJar);
                goJar = null;
            }
        }
    }
}