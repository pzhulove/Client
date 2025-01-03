using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComSelectRoleField : MonoBehaviour
    {
        #region Model Params

        RoleSelectFieldState fieldState = RoleSelectFieldState.Default;
        //显示过一次后即不显示 界面展示有效 需要和新解锁栏位数 一起判断
        bool hasNewTagShowed = false;               

        #endregion

        #region View Params

        private ComCommonBind mBind;

        GameObject name = null;
        GameObject level = null;
        GameObject job = null;
        GameObject avatar = null;
        GameObject imgAdd = null;
        GameObject imgDisSelect = null;
        GameObject imgSelect = null;
        //Toggle toggle = null;
        GameObject imgDi = null;
        GameObject goBookingActivity = null;
        GameObject mOldPlayer = null;
        GameObject mLike = null;

        ComSelectRoleFieldExtendTag mFieldRoot = null;

        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Awake () 
        {
            mBind = this.GetComponent<ComCommonBind>();
            if (mBind != null)
            {
                name = mBind.GetGameObject("name");
                level = mBind.GetGameObject("level");
                job = mBind.GetGameObject("job");
                avatar = mBind.GetGameObject("avatar");
                imgAdd = mBind.GetGameObject("imgAdd");
                imgDisSelect = mBind.GetGameObject("imgDisSelect");
                imgSelect = mBind.GetGameObject("imgSelect");
                //toggle = mBind.GetCom<Toggle>("toggle");
                imgDi = mBind.GetGameObject("imgDi");
                goBookingActivity = mBind.GetGameObject("bookingActivity");
                mOldPlayer = mBind.GetGameObject("OldPlayer");
                mLike = mBind.GetGameObject("like");

                mFieldRoot = mBind.GetCom<ComSelectRoleFieldExtendTag>("FieldRoot");
                mFieldRoot.CustomActive(true);
            }
        }
        
        //Unity life cycle
        void OnDestroy () 
        {
            mBind = null;

            name = null;
            level = null;
            job = null;
            avatar = null;
            imgAdd = null;
            imgDisSelect = null;
            imgSelect = null;
            //toggle = null;
            imgDi = null;
            goBookingActivity = null;
            mOldPlayer = null;
            mLike = null;

            mFieldRoot = null;
            fieldState = RoleSelectFieldState.Default;
            hasNewTagShowed = false;
        }

        /// <summary>
        ///  设置空栏位
        /// </summary>
        /// <param name="bNone">是否是无效栏位</param>
        private void _SetDefaultStateShow(bool bNone = false)
        {
            name.CustomActive(false);
            level.CustomActive(false);
            job.CustomActive(false);
            avatar.CustomActive(false);
            imgAdd.CustomActive(false);
            imgDisSelect.CustomActive(!bNone);
            imgSelect.CustomActive(false);
            //toggle.onValueChanged.RemoveAllListeners();
            //toggle.onValueChanged.AddListener(CreateRole);

            imgDi.CustomActive(!bNone);

            goBookingActivity.CustomActive(false);
            mOldPlayer.CustomActive(false);
            mLike.CustomActive(false);
        }

        /// <summary>
        /// 设置非空栏位
        /// </summary>
        private void _SetBaseHasRoleStateShow()
        {
            name.CustomActive(true);
            level.CustomActive(true);
            job.CustomActive(true);
            avatar.CustomActive(true);
            imgAdd.CustomActive(false);
            imgDisSelect.CustomActive(true);
            imgSelect.CustomActive(true);
            //toggle.onValueChanged.RemoveAllListeners();
            imgDi.CustomActive(true);
            goBookingActivity.CustomActive(false);
            mOldPlayer.CustomActive(false);
        }
        
        #endregion
        
        #region  PUBLIC METHODS

        public void SetNoneStateShow()
        {
            _SetDefaultStateShow(true);

            if (mFieldRoot != null)
            {
                mFieldRoot.SetNotUnLockShow();
            }
        }

        public void SetDefaultStateShow()
        {
            _SetDefaultStateShow(false);

            if (mFieldRoot != null)
            {
                mFieldRoot.SetDefaultShow();
            }
        }

        public void SetBaseHasRoleStateShow()
        {
            _SetBaseHasRoleStateShow();

            if (mFieldRoot != null)
            {
                mFieldRoot.SetUnlockShow();
            }
        }

        public void SetNewUnlockHasRoleStateShow()
        {
            _SetBaseHasRoleStateShow();

            if (mFieldRoot != null)
            {
                if (!hasNewTagShowed)
                {
                    mFieldRoot.SetNewUnlockTagShow();
                }
                hasNewTagShowed = true;
            }
        }

        public void SetNewUnlockNoRoleStateShow()
        {
            _SetDefaultStateShow();

            if (mFieldRoot != null)
            {
                if (!hasNewTagShowed)
                {
                    mFieldRoot.SetNewUnlockTotalShow();
                }
                else
                {
                    mFieldRoot.SetDefaultShow();
                }
                hasNewTagShowed = true;
            }
        }

        public void SetLockHasRoleStateShow()
        {
            _SetBaseHasRoleStateShow();

            if (mFieldRoot != null)
            {
                mFieldRoot.SetLockTagShow();
            }
        }

        public void SetLockNoRoleStateShow()
        {
            _SetDefaultStateShow();

            if (mFieldRoot != null)
            {
                mFieldRoot.SetLockShow();
            }
        }

        public void SetRoleSelectFieldState(RoleSelectFieldState state)
        {
            this.fieldState = state;
        }

        public RoleSelectFieldState GetRoleSelectFieldState()
        {
            return fieldState;
        }
        
        #endregion
    }
}