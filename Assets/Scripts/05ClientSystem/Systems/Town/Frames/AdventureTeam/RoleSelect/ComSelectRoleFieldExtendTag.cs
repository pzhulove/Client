using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComSelectRoleFieldExtendTag : MonoBehaviour
    {
        private UnityEngine.Coroutine waitToShowNewUnlockEffect = null;

        //test!!!
        private const string NEW_UNLOCK_EFFECT_RES_PATH = "Effects/UI/Prefab/EffUI_jueselanjiesuo/Prefab/EffUI_jueselanjiesuo";

        #region View Params

        [SerializeField]
        private Button lockBtn;
        [SerializeField]
        private Image lockImg;
        [SerializeField]
        private Image newUnlockImg;
        [SerializeField]
        private Button unlockBtn;
        [SerializeField]
        private Button addBtn;
        [SerializeField]
        private GameObject unOpenRoot;

        [SerializeField]
        private GameObject newUnlockEffectRoot;
        [Space(10)]
        [Header("新解锁特效延迟显示的时间")]
        [SerializeField]
        private float newUnlockEffectShowDelayTime = 0.3f;
        [Header("新解锁特效显示的阶段1_持续时间")]
        [SerializeField]
        private float newUnlockEffectDuration_1 = 0.3f;
        [Header("新解锁特效显示的阶段2_持续时间")]
        [SerializeField]
        private float newUnlockEffectDuration_2 = 0.9f;

        private GameObject newUnlockEffectGo;
        
        #endregion
        
        #region PRIVATE METHODS
       
        //Unity life cycle
        void Awake () 
        {
            if (lockBtn)
            {
                lockBtn.onClick.AddListener(_OnlockBtnClick);
            }
            if (unlockBtn)
            {
                unlockBtn.onClick.AddListener(_OnUnlockBtnClick);
            }

            if(addBtn != null)
            {
                addBtn.onClick.AddListener(_OnAddBtnClick);
            }

            if (newUnlockEffectGo == null)
            {
                newUnlockEffectGo = AssetLoader.instance.LoadResAsGameObject(NEW_UNLOCK_EFFECT_RES_PATH);
                newUnlockEffectGo.CustomActive(false);
                Utility.AttachTo(newUnlockEffectGo, newUnlockEffectRoot);
            }

            newUnlockImg.CustomActive(false);

            //test !!!
            //if (newUnlockEffectRoot)
            //{
            //    newUnlockEffectRoot.transform.Rotate(Vector3.forward, 90f);
            //}
        }
        
        //Unity life cycle
        void OnDestroy () 
        {
            if (lockBtn)
            {
                lockBtn.onClick.RemoveListener(_OnlockBtnClick);
            }
            if (unlockBtn)
            {
                unlockBtn.onClick.RemoveListener(_OnUnlockBtnClick);
            }

            if (addBtn != null)
            {
                addBtn.onClick.RemoveListener(_OnAddBtnClick);
            }

            if (waitToShowNewUnlockEffect != null)
            {
                StopCoroutine(waitToShowNewUnlockEffect);
                waitToShowNewUnlockEffect = null;
            }

            newUnlockEffectGo = null;
        }

        void _OnlockBtnClick()
        {
            string tr_select_role_lock_field_tip = TR.Value("select_role_lock_field_tip");
            SystemNotifyManager.SysNotifyMsgBoxOK(tr_select_role_lock_field_tip);
        }

        void _OnUnlockBtnClick()
        {
            string tr_selct_role_unlock_field_tip = 
                TR.Value("select_role_unlock_field_tip", ClientApplication.playerinfo.unLockedExtendRoleFieldNum, ClientApplication.playerinfo.extendRoleFieldNum);
            SystemNotifyManager.SysNotifyMsgBoxOK(tr_selct_role_unlock_field_tip);
        }

        void _OnAddBtnClick()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<CreateRoleFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<CreateRoleFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<CreateRoleFrame>();
        }

        IEnumerator _WaitToShowNewUnlockEffect()
        {
            yield return Yielders.GetWaitForSeconds(newUnlockEffectShowDelayTime);            
            newUnlockEffectRoot.CustomActive(true);
            newUnlockEffectGo.CustomActive(true);
            yield return Yielders.GetWaitForSeconds(newUnlockEffectDuration_1);
            unlockBtn.CustomActive(false);                                              //未解锁角色栏位 改变状态            
            //newUnlockImg.CustomActive(true);        
            yield return Yielders.GetWaitForSeconds(newUnlockEffectDuration_2);
            newUnlockEffectGo.CustomActive(false);
            addBtn.CustomActive(true);
        }
        
        #endregion
        
        #region  PUBLIC METHODS

        public void SetNotUnLockShow()
        {
            lockImg.CustomActive(false);
            newUnlockImg.CustomActive(false);

            unlockBtn.CustomActive(false);

            newUnlockEffectRoot.CustomActive(false);
            unOpenRoot.CustomActive(true);
        }

        /// <summary>
        /// 锁住 完成状态
        /// </summary>
        /// <param name="bShow"></param>
        public void SetLockShow()
        {
            unlockBtn.CustomActive(true);

            newUnlockImg.CustomActive(false);
            lockImg.CustomActive(false);

            newUnlockEffectRoot.CustomActive(false);
            addBtn.CustomActive(false);
            unOpenRoot.CustomActive(false);
        }

        public void SetDefaultShow()
        {
            lockImg.CustomActive(false);
            newUnlockImg.CustomActive(false);

            unlockBtn.CustomActive(false);

            newUnlockEffectRoot.CustomActive(false);
            addBtn.CustomActive(true);
            unOpenRoot.CustomActive(false);
        }

        /// <summary>
        /// 解锁 完成状态
        /// </summary>
        public void SetUnlockShow()
        {
            lockImg.CustomActive(false);
            newUnlockImg.CustomActive(false);

            unlockBtn.CustomActive(false);

            newUnlockEffectRoot.CustomActive(false);
            addBtn.CustomActive(false);
            unOpenRoot.CustomActive(false);
        }
        
        /// <summary>
        /// 锁住 标志状态
        /// </summary>
        /// <param name="bShow"></param>
        public void SetLockTagShow()
        {
            lockImg.CustomActive(true);
            newUnlockImg.CustomActive(false);

            unlockBtn.CustomActive(false);

            newUnlockEffectRoot.CustomActive(false);
            addBtn.CustomActive(false);
            unOpenRoot.CustomActive(false);
        }

        /// <summary>
        /// 刚解锁 仅控制 标志 展示
        /// </summary>
        /// <param name="bShow"></param>
        public void SetNewUnlockTagShow()
        {
            lockImg.CustomActive(false);

            unlockBtn.CustomActive(false);

            addBtn.CustomActive(false);
            unOpenRoot.CustomActive(false);

            if (waitToShowNewUnlockEffect != null)
            {
                StopCoroutine(waitToShowNewUnlockEffect);
            }
            waitToShowNewUnlockEffect = StartCoroutine(_WaitToShowNewUnlockEffect());
        }

        /// <summary>
        /// 刚解锁 控制 全部 展示
        /// </summary>
        public void SetNewUnlockTotalShow()
        {
            lockImg.CustomActive(false);

            unlockBtn.CustomActive(true);

            addBtn.CustomActive(false);
            unOpenRoot.CustomActive(false);

            if (waitToShowNewUnlockEffect != null)
            {
                StopCoroutine(waitToShowNewUnlockEffect);
            }
            waitToShowNewUnlockEffect = StartCoroutine(_WaitToShowNewUnlockEffect());
        }

        #endregion
    }
}