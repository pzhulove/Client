using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChangeJobNewItem : MonoBehaviour
    {
        [SerializeField] private GameObject unopenGo;
        [SerializeField] private GameObject openGo;
        [SerializeField] private GameObject checkMarkGo;
        [SerializeField] private GameObject unOpenCheckMarkGo;
        [SerializeField] private Toggle toggle;
        [SerializeField] private ButtonEx button;
        [SerializeField] private Image normalIcon;
        [SerializeField] private Image checkIcon;
        [SerializeField] private Image unOpenCheckIcon;
        [SerializeField] private UIGray normalIconGray;
        [SerializeField] private GameObject mObjMaskUnSelect;
        [SerializeField] private GameObject appointmentGo;
        [SerializeField] private Text mTextCurJobTip;
        [SerializeField] private string mEffectPath = "UI/UIEffects/Skill_UI_Changjue_01/Prefab/Skill_UI_Changjue_01";

        private OnSelectedJobClick onSelectedJobClick;
        private int jobId;
        private bool bIsSelected = false;
        /// <summary>
        /// 是否开放
        /// </summary>
        private bool bIsOpen = false;
        private GameObject mEffectGo;
        private void Awake()
        {
            if(toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(OnValueChangedClick);
            }

            if(button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnButtonClick);
            }
        }

        private void OnDestroy()
        {
            onSelectedJobClick = null;
            jobId = 0;
            bIsSelected = false;
            bIsOpen = false;
            mEffectGo = null;

            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        public void OnItemVisiable(int jobId, OnSelectedJobClick onSelectedJobClick,bool isSelected)
        {
            unopenGo.CustomActive(false);
            openGo.CustomActive(false);
            appointmentGo.CustomActive(false);
            if (jobId > 0)
            {
                this.jobId = jobId;
                this.onSelectedJobClick = onSelectedJobClick;
                
                var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(this.jobId);
                if(jobTable != null)
                {
                    if (string.IsNullOrEmpty(jobTable.JobHead))
                    {
                        // unopenGo.CustomActive(true);
                        return;
                    }

                    bIsOpen = jobTable.Open > 0 ? true : false;
                    if (normalIcon != null)
                    {
                        ETCImageLoader.LoadSprite(ref normalIcon, jobTable.JobHead);
                    }

                    if (checkIcon != null)
                    {
                        ETCImageLoader.LoadSprite(ref checkIcon, jobTable.JobHead);
                    }

                    if(unOpenCheckIcon != null)
                    {
                        ETCImageLoader.LoadSprite(ref unOpenCheckIcon, jobTable.JobHead);
                    }

                    if (normalIconGray != null)
                    {
                        normalIconGray.enabled = bIsOpen == true ? false : true;
                    }
                    if (null != mObjMaskUnSelect)
                    {
                        mObjMaskUnSelect.CustomActive(!bIsOpen);
                    }
                }
                
                appointmentGo.CustomActive(ClientApplication.playerinfo.GetRoleHasApponintmentOccu(jobId));
                openGo.CustomActive(true);

                if (isSelected)
                {
                    toggle.isOn = true;
                }
            }
            else
            {
                // unopenGo.CustomActive(true);
            }
        }

        public void onClickTipCantSelect()
        {
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("select_job_un_open"));
        }

        public void ShowCurJob(bool needShow)
        {
            mTextCurJobTip.CustomActive(needShow);
        }

        public void UpdateCheckMarkGo(bool value)
        {
            if (checkMarkGo != null)
                checkMarkGo.CustomActive(CreateRoleFrame.CurSelectedJobID == jobId);
        }

        private void OnValueChangedClick(bool value)
        {
            if(value == bIsSelected)
            {
                return;
            }

            bIsSelected = value;

            if(value)
            {
                if(onSelectedJobClick != null)
                {
                    onSelectedJobClick.Invoke(jobId);
                }

                if (checkMarkGo != null)
                    checkMarkGo.CustomActive(bIsOpen);

                if (unOpenCheckMarkGo != null)
                    unOpenCheckMarkGo.CustomActive(!bIsOpen);

                if(mEffectGo == null)
                {
                    LoadEffect();
                }
                else
                {
                    mEffectGo.CustomActive(true);
                }
            }
            else
            {
                if (checkMarkGo != null)
                    checkMarkGo.CustomActive(false);

                if (unOpenCheckMarkGo != null)
                    unOpenCheckMarkGo.CustomActive(false);

                if (mEffectGo != null)
                    mEffectGo.CustomActive(false);
            }
        }

        private void LoadEffect()
        {
            var go = AssetLoader.GetInstance().LoadResAsGameObject(mEffectPath);
            if(go != null)
            {
                mEffectGo = go;
                Utility.AttachTo(mEffectGo, bIsOpen == true ? checkMarkGo : unOpenCheckMarkGo);
                mEffectGo.transform.SetAsFirstSibling();
            }
        }
        
        private void OnButtonClick()
        {
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("create_role_un_open"));
        }
    }
}