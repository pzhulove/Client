

namespace Tenmove.Runtime
{
    public abstract class Referable
    {
        public abstract void OnSpawn();

        public abstract void OnUnspawn();

        public abstract void OnRelease();

        public abstract void Lock(bool bLock);

        public abstract string Name
        {
            get;
        }

        public abstract int NameHashCode
        {
            get;
        }

        public abstract bool IsInUse
        {
            get;
        }

        public abstract bool IsLocked
        {
            get;
        }

        public abstract long LastUseTime
        {
            get;
        }

        public abstract int SpawnCount
        {
            get;
        }
    }
}