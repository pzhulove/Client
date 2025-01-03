using System.Collections.Generic;

using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    using WeakRef = System.WeakReference;
    internal class UnityAssetObject : ITMAssetObject
    {
        private readonly WeakRef m_AssetObjRef;
        private readonly GameObject m_AssetObject;
        private readonly bool m_IsInstAsset;
        private readonly List<object> m_ObjInstList;
        private UnityEngine.Object m_RefForLock;

        public UnityAssetObject(object assetObject)
        {
            m_IsInstAsset = assetObject is GameObject;
            if (m_IsInstAsset)
            {
                m_AssetObject = assetObject as GameObject;
                m_AssetObjRef = null;
            }
            else
            {
                m_AssetObject = null;
                m_AssetObjRef = new WeakRef(assetObject, false);
            }
            m_ObjInstList = new List<object>();
        }

        public object CreateAssetInst(AssetObjectDesc desc)
        {
            if (m_IsInstAsset)
            {
                Debugger.Assert(m_AssetObject is GameObject, "Asset is not a instance type!");
                GameObject assetInst = null;

                if (null != desc.Parent)
                {
                    GameObject parentObject = desc.Parent as GameObject;
                    if (null != parentObject)
                    {
                        assetInst = desc.OverrideTransform ?
                            UnityEngine.Object.Instantiate(m_AssetObject as GameObject,
                            desc.Transform.Position.ToVector3(),
                            desc.Transform.Rotation.ToQuaternion(), parentObject.transform) :
                            UnityEngine.Object.Instantiate(m_AssetObject as GameObject, parentObject.transform);
                    }
                    else
                    {
                        Debugger.LogWarning("Parent object must be a GameObject type, if it is null maybe it has been destroyed at same frame!");
                        assetInst = desc.OverrideTransform ?
                            UnityEngine.Object.Instantiate(m_AssetObject as GameObject,
                            desc.Transform.Position.ToVector3(),
                            desc.Transform.Rotation.ToQuaternion()) :
                            UnityEngine.Object.Instantiate(m_AssetObject as GameObject);
                    }
                }
                else
                {
                    assetInst = desc.OverrideTransform ?
                        UnityEngine.Object.Instantiate(m_AssetObject as GameObject,
                        desc.Transform.Position.ToVector3(),
                        desc.Transform.Rotation.ToQuaternion()) :
                        UnityEngine.Object.Instantiate(m_AssetObject as GameObject);
                }

                if (null != assetInst)
                    m_ObjInstList.Add(assetInst);

                return assetInst;
            }
            else
            {
                if (desc.OverrideTransform || null != desc.Parent)
                    Debugger.LogWarning("Current asset is not instance type,Only instance type asset can create with a parent and a override transform!");

                return m_AssetObjRef.Target;
            }
        }        

        public bool Lock()
        {
            if (!m_IsInstAsset)
            {
                m_RefForLock = m_AssetObjRef.Target as UnityEngine.Object;
                return null != m_RefForLock;
            }

            return true;
        }

        public void Unlock()
        {
            m_RefForLock = null;
        }

        public bool IsWeakRefAsset
        {
            get { return !m_IsInstAsset; }
        }

        public bool IsInUse
        {
            get
            {
                if (m_IsInstAsset)
                {
                    _CheckDeadInst();
                    return 0 != m_ObjInstList.Count;
                }
                else
                {
                    return null != m_AssetObjRef.Target as UnityEngine.Object;
                }
            }
        }

        public int SpawnCount
        {
            get
            {
                if (m_IsInstAsset)
                {
                    _CheckDeadInst();
                    return m_ObjInstList.Count;
                }
                else
                {
                    return IsInUse ? 1 : 0;
                }
            }
        }

        private void _CheckDeadInst()
        {
            m_ObjInstList.RemoveAll(
                inst =>
                {
                    return null == inst as UnityEngine.Object;
                }
            );
        }
    }
}

