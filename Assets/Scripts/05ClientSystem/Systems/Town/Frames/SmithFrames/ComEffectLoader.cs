using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ComEffectLoader : MonoBehaviour
    {
        public string[] mEffectPools = new string[0];
        public GameObject[] mParents = new GameObject[0];
        GameObject[] mPoolsObject = null;

        public GameObject LoadEffect(int iIndex)
        {
            if (null == mPoolsObject)
            {
                mPoolsObject = new GameObject[mEffectPools.Length];
            }

            if (iIndex >= 0 && iIndex < mPoolsObject.Length)
            {
                if (null == mPoolsObject[iIndex])
                {
                    mPoolsObject[iIndex] = AssetLoader.instance.LoadRes(mEffectPools[iIndex], typeof(GameObject)).obj as GameObject;
                    if(iIndex >= 0 && iIndex < mParents.Length)
                    {
                        Utility.AttachTo(mPoolsObject[iIndex],mParents[iIndex]);
                    }
                }

                return mPoolsObject[iIndex];
            }

            return null;
        }

        public void ActiveEffect(int iIndex)
        {
            if (null != mPoolsObject && iIndex >= 0 && iIndex < mPoolsObject.Length)
            {
                mPoolsObject[iIndex].CustomActive(true);
            }
        }

        public void DeActiveEffect(int iIndex)
        {
            if (null != mPoolsObject && iIndex >= 0 && iIndex < mPoolsObject.Length)
            {
                mPoolsObject[iIndex].CustomActive(false);
            }
        }

        public void SetEffectPosition(int iIndex,Vector3 worldPos)
        {
            if (null != mPoolsObject && iIndex >= 0 && iIndex < mPoolsObject.Length)
            {
                if(null != mPoolsObject[iIndex])
                {
                    mPoolsObject[iIndex].transform.position = worldPos;
                }
            }
        }

        void OnDestroy()
        {
            if(null != mPoolsObject)
            {
                for (int i = 0; i < mPoolsObject.Length; ++i)
                {
                    if(null != mPoolsObject[i])
                    {
                        GameObject.Destroy(mPoolsObject[i]);
                        mPoolsObject[i] = null;
                    }
                }
            }
        }
    }
}