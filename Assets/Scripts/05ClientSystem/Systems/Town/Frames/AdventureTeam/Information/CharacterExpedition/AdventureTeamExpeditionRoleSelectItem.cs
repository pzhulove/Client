using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{

    public class AdventureTeamExpeditionRoleSelectItem : MonoBehaviour
    {
        private ExpeditionMemberInfo tempRoleInfo;

        [SerializeField] private Text mTitleText;
        [SerializeField] private Text mLevelext;
        [SerializeField] private Text mOccuText;
        [SerializeField] private Text mRoleNameText;
        [SerializeField] private Button mRoleSelectButton;
        [SerializeField] private GeAvatarRendererEx mModelAvater;
        [SerializeField] private GameObject mModelRoot;
        [SerializeField] private GameObject mAddRoleImage;

        [SerializeField] private int ModelLayer = 17;

        public void InitItemView(int index, ExpeditionMemberInfo roleInfo)
        {
            if (mTitleText)
            {
                mTitleText.text = (index + 1).ToString();
            }
            tempRoleInfo = roleInfo;
            if (tempRoleInfo == null)
            {
                if (mAddRoleImage)
                {
                    mAddRoleImage.SetActive(true);
                }
                if (mModelAvater)
                {
                    mModelAvater.ClearAvatar();
                    mModelAvater.LoadAvatar("Effects/Scene_effects/EffectUI/EffUI_chuangjue_fazhen_JS_jingtai", ModelLayer + index);
                }
            }
            else
            {
                if (mAddRoleImage)
                {
                    mAddRoleImage.SetActive(false);
                }
                if (mModelRoot)
                {
                    mModelRoot.SetActive(true);
                }
                _InitRoleModel(index);
            }
            _UpdateRoleSelectBtn();
        }

        public void OnItemRecycle()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void Awake()
        {
            ClearData();
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void ClearData()
        {
            tempRoleInfo = null;
			if (mModelAvater)
            {
            	mModelAvater.ClearAvatar();
			}
        }

        private void UnBindUiEventSystem()
        {
            if (mRoleSelectButton != null)
            {
                mRoleSelectButton.onClick.RemoveListener(_OnRoleSelectBtnClick);
            }
        }

        

        private void _InitRoleModel(int index)
        {
            if (mLevelext)
            {
                mLevelext.text = "Lv." + tempRoleInfo.level;
            }
            if (mRoleNameText)
            {
                mRoleNameText.text = tempRoleInfo.name;
            }
            JobTable job = TableManager.instance.GetTableItem<JobTable>(tempRoleInfo.occu);
            if (job == null)
            {
                Logger.LogErrorFormat("can not find JobTable with id:{0}", tempRoleInfo.occu);
            }
            else
            {
                if (mOccuText)
                {
                    mOccuText.text = Utility.GetJobName(tempRoleInfo.occu);
                }

                ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

                if (res == null)
                {
                    Logger.LogErrorFormat("can not find ResTable with id:{0}", job.Mode);
                }
                else
                {
                    if (null != mModelAvater)
                    {
                        mModelAvater.LoadAvatar(res.ModelPath, ModelLayer + index);

                        PlayerBaseData.GetInstance().AvatarEquipFromItems(mModelAvater,
                        tempRoleInfo.avatar.equipItemIds,
                        tempRoleInfo.occu,
                        (int)(tempRoleInfo.avatar.weaponStrengthen),
                        null,
                        false,
                        tempRoleInfo.avatar.isShoWeapon);
                        mModelAvater.ChangeAction("Anim_Show_Idle", 1.0f, true);

                        mModelAvater.AttachAvatar("DisSelect",
                "Effects/Scene_effects/EffectUI/EffUI_chuangjue_fazhen_JS_jingtai",
                "[actor]Orign", false);
                    }
                }
            }
        }

        private void _UpdateRoleSelectBtn()
        {
            if (AdventureTeamDataManager.GetInstance().IsExpeditionMapEnable())
            {
                if (mRoleSelectButton)
                {
                    mRoleSelectButton.enabled = true;
                    mRoleSelectButton.onClick.AddListener(_OnRoleSelectBtnClick);
                }
            }
            else
            {
                if (mRoleSelectButton)
                {
                    mRoleSelectButton.enabled = false;
                }
            }
        }

        private void _OnRoleSelectBtnClick()
        {
            AdventureTeamDataManager.GetInstance().TryOpenExpeditionRoleSelectFrame(tempRoleInfo);
        }

    }
}
