using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{

    public class AdventureTeamExpeditionCharacterSelectUnit : MonoBehaviour
    {
        [SerializeField] private Text mRoleOccu;
        [SerializeField] private Text mRoleName;
        [SerializeField] private Text mRoleLv;
        [SerializeField] private Text mUnEnableText;
        [SerializeField] private GameObject mUnEnableObj;
        [SerializeField] private Button mChangeExpeditionRoleBtn;
        [SerializeField] private GameObject mIsSelectImage;
        [SerializeField] private Image mRoleIcon;

        [SerializeField] private ExpeditionMemberInfo tempRoleInfo;
        [SerializeField] private ExpeditionRoleState tempRoleState;

        private string tr_expedition_role_select_lv_limit = "";
        private string tr_expedition_role_select_dispatch = "";

        private void Awake()
        {
            _InitTR();
            ClearData();
        }

        private void OnDestroy()
        {
            _ClearTR();
            ClearData();
        }

        private void _InitTR()
        {
            tr_expedition_role_select_lv_limit = TR.Value("adventure_team_expedition_role_select_lv_limit");
            tr_expedition_role_select_dispatch = TR.Value("adventure_team_expedition_role_select_dispatch");
        }

        private void _ClearTR()
        {
            tr_expedition_role_select_lv_limit = "";
            tr_expedition_role_select_dispatch = "";
        }

        public void InitItemView(ExpeditionMemberInfo roleInfo, ExpeditionRoleState roleState)
        {
            tempRoleInfo = roleInfo;
            tempRoleState = roleState;
        }

        public void UpdateItemInfo()
        {
            switch (tempRoleState)
            {
                case ExpeditionRoleState.PREPARE:

                    mUnEnableObj.SetActive(false);
                    mIsSelectImage.SetActive(false);
                    mChangeExpeditionRoleBtn.onClick.AddListener(_OnChangeRoleBtnClick);
                    mChangeExpeditionRoleBtn.enabled = true;
                    break;

                case ExpeditionRoleState.SELECT:

                    mUnEnableObj.SetActive(false);
                    mIsSelectImage.SetActive(true);
                    mChangeExpeditionRoleBtn.onClick.AddListener(_OnChangeRoleBtnClick);
                    mChangeExpeditionRoleBtn.enabled = true;
                    break;

                case ExpeditionRoleState.LEVEL_LIMIT:

                    mUnEnableObj.SetActive(true);
                    mUnEnableText.text = tr_expedition_role_select_lv_limit;
                    mIsSelectImage.SetActive(false);
                    mChangeExpeditionRoleBtn.enabled = false;
                    break;

                case ExpeditionRoleState.EXPEDITION:

                    mUnEnableObj.SetActive(true);
                    mUnEnableText.text = tr_expedition_role_select_dispatch;
                    mIsSelectImage.SetActive(false);
                    mChangeExpeditionRoleBtn.enabled = false;
                    break;
                default:
                    break;
            }
            mRoleLv.text = "Lv." + tempRoleInfo.level.ToString();
            mRoleName.text = tempRoleInfo.name;
            JobTable job = TableManager.instance.GetTableItem<JobTable>(tempRoleInfo.occu);
            if (job == null)
            {
                Logger.LogErrorFormat("can not find JobTable with id:{0}", tempRoleInfo.occu);
            }
            else
            {
                ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(job.Mode);
                ETCImageLoader.LoadSprite(ref mRoleIcon, resData.IconPath);
                mRoleOccu.text = job.Name;
            }
        }

        private void _OnChangeRoleBtnClick()
        {
            if(tempRoleState == ExpeditionRoleState.PREPARE)
            {
                bool succeed = AdventureTeamDataManager.GetInstance().AddExpeditionRole(tempRoleInfo);
                if (succeed)
                {
                    tempRoleState = ExpeditionRoleState.SELECT;
                    mIsSelectImage.SetActive(true);
                }
            }
            else if(tempRoleState == ExpeditionRoleState.SELECT)
            {
                bool succeed = AdventureTeamDataManager.GetInstance().RemoveExpeditionRole(tempRoleInfo);
                if (succeed)
                {
                    tempRoleState = ExpeditionRoleState.PREPARE;
                    mIsSelectImage.SetActive(false);
                }
            }
        }

        public void OnItemRecycle()
        {
            ClearData();
        }

        private void ClearData()
        {
            tempRoleInfo = null;
            mChangeExpeditionRoleBtn.onClick.RemoveListener(_OnChangeRoleBtnClick);
        }

        
    }
}
