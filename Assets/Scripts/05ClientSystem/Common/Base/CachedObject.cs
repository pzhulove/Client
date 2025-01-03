namespace GameClient
{
    public class CachedObject
    {
        public virtual void OnCreate(object[] param) { }
        public virtual void OnDestroy()
        {
            //Logger.LogError("please override me!");
        }
        public virtual void OnRecycle() { }
        public virtual void OnDecycle(object[] param) { }
        public virtual void OnRefresh(object[] param) { }
        public virtual void Enable() { }
        public virtual void Disable() { }
        public virtual bool NeedFilter(object[] param) { return false; }

        public virtual void SetAsLastSibling() { }
    }
}