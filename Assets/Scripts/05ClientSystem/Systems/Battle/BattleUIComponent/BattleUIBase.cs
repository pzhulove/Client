using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 战斗UI组件基类
    /// </summary>
    public class BattleUIBase
    {
        protected ComCommonBind mBind = null;
        protected GameObject RootObj = null;

        public void Init(GameObject root)
        {
            RootObj = root;
            AttachPrefab(root);
            _bindExUI();
            OnInit();
        }

        public bool NeedUpdate()
        {
            return IsNeedUpdate();
        }

        public void Update(float timeElapsed)
        {
            OnUpdate(timeElapsed);
        }

        public void DeInit()
        {
            if (mBind != null)
            {
                _unbindExUI();
                if (mBind.gameObject != null)
                    GameObject.Destroy(mBind.gameObject);
                mBind = null;
            }

            if (RootObj != null)
            {
                RootObj = null;
            }
        }

        public void Enter()
        {
            OnEnter();
        }
        public void Enable(bool bEnable)
        {
            if(RootObj != null)
            {
                RootObj.CustomActive(bEnable);
            }
        }

        /// <summary>
        /// Start在Enter后面调用
        /// </summary>
        public void Start()
        {
            OnStart();
        }

        public void Exit()
        {
            OnExit();
        }

        /// <summary>
        /// 初始化并且挂在主UI上面
        /// </summary>
        protected void AttachPrefab(GameObject root)
        {
            if (root == null)
                return;
            string prefabPath = GetPrefabPath();
            if (string.IsNullOrEmpty(prefabPath))
                return;
            var obj = AssetLoader.instance.LoadResAsGameObject(prefabPath);
            if (obj == null)
                return;
            Utility.AttachTo(obj, root);
            var component = obj.GetComponent<ComCommonBind>();
            if (component == null)
                return;
            mBind = component;
        }
        
        public void SetInputSettingData(Transform obj, InputSettingItem item)
        {
            obj.localPosition = item.position;
            obj.localScale = item.scale;
            var canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = obj.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = item.alpha;
        }

        protected virtual string GetPrefabPath() { return null;}

        protected virtual void OnEnter() { }
        protected virtual void OnStart() { }
        protected virtual void OnExit() { }

        protected virtual void OnInit() { }
        protected virtual void _bindExUI() { }
        protected virtual bool IsNeedUpdate() { return false; }
        protected virtual void OnUpdate(float timeElapsed) { }
        protected virtual void _unbindExUI() { }
        protected virtual void OnDeInit() { }
    }
}