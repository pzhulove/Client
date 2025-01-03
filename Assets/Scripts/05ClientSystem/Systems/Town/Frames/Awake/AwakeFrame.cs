using UnityEngine;
using System.Collections;

namespace GameClient
{
    class AwakeFrame : ClientFrame
    {
        float time = 0.0f;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/UpLeveljuexing";
        }

        protected override void _OnOpenFrame()
        {
            var destroyDelay = frame.GetComponent<DestroyDelay>();
            StartCoroutine(closeDelay(destroyDelay.Delay));
            destroyDelay.enabled = false;

            InitInterface();
        }

        IEnumerator closeDelay(float time)
        {
            yield return new WaitForSeconds(time);
            Close();
            yield break;
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        void ClearData()
        {
            time = 0.0f;
        }

        void InitInterface()
        {
            PlayerBaseData.GetInstance().bNeedShowAwakeFrame = false;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            time += timeElapsed;

            if(time >= 1.0f)
            {
                mEffectRoot.CustomActive(true);
            }
        }

        #region ExtraUIBind
        private GameObject mEffectRoot = null;

        protected override void _bindExUI()
        {
            mEffectRoot = mBind.GetGameObject("EffectRoot");
        }

        protected override void _unbindExUI()
        {
            mEffectRoot = null;
        }
        #endregion
    }
}