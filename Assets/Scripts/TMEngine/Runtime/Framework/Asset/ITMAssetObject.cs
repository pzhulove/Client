
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public struct AssetObjectDesc
    {
        private Transform m_Transform;

        public AssetObjectDesc(object parent)
        {
            OverrideTransform = false;
            m_Transform = Transform.Identity;
            Parent = parent;
        }

        public Transform Transform
        {
            set
            {
                m_Transform = value;
                OverrideTransform = true;
            }
            get
            {
                return m_Transform;
            }
        }

        public bool OverrideTransform { private set; get; }
        public object Parent { set; get; }
    }

    public interface ITMAssetObject
    {
        object CreateAssetInst(AssetObjectDesc desc);

        bool Lock();
        void Unlock();

        bool IsWeakRefAsset
        {
            get;
        }

        bool IsInUse
        {
            get;
        }

        int SpawnCount
        {
            get;
        }
    }
}
