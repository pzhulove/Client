using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    public enum ExpeditionRoleState
    {
        PREPARE,
        SELECT,
        LEVEL_LIMIT,
        EXPEDITION,
    }

    public class AdventureTeamExpeditionCharacterSelectView : MonoBehaviour
    {
        #region MODEL PARAMS
        private string tr_title_adventure_team_expedition_select_name = "";

        private List<ExpeditionMemberInfo>[] rolesList = new List<ExpeditionMemberInfo>[4];//0：已选择 1：可选择 2：等级不足 3：远征状态
        private int rolesNum = 0;


        
        #endregion

        #region VIEW PARAMS
        [SerializeField] private Text mTitleLabel;
        [SerializeField] private Button mCloseButton;
        [SerializeField] private ComUIListScript mRolesRoot;
        [SerializeField] private Button mBackGroundBtn;
        #endregion


        #region PIRVATE METHODS

        private void Awake()
        {
            _BindUIEvents();
            _InitTR();
            _InitRolesScrollListBind();
        }

        private void OnDestroy()
        {
            _UnBindUIEvents();
        }

        private void _bindEvents()
        {

        }

        private void _unBindEvente()
        {

        }

        private void _BindUIEvents()
        {
            if (mCloseButton != null)
            {
                mCloseButton.onClick.AddListener(_OnCloseButtonClickCallBack);
            }
            if (mBackGroundBtn != null)
            {
                mBackGroundBtn.onClick.AddListener(_OnCloseButtonClickCallBack);
            }
        }

        private void _UnBindUIEvents()
        {
            if (mCloseButton != null)
            {
                mCloseButton.onClick.RemoveListener(_OnCloseButtonClickCallBack);
            } 
            if(mBackGroundBtn != null)
            {
                mBackGroundBtn.onClick.RemoveListener(_OnCloseButtonClickCallBack);
            }
        }

        private void _InitTR()
        {
            rolesList = new List<ExpeditionMemberInfo>[4];
            tr_title_adventure_team_expedition_select_name = TR.Value("adventure_team_expedition_select_name");
        }

        private void _ClearView()
        {
            tr_title_adventure_team_expedition_select_name = "";
            rolesList = null;
            rolesNum = 0;
        }

        private void _InitBaseData()
        {
            if (mTitleLabel != null)
            {
                mTitleLabel.text = tr_title_adventure_team_expedition_select_name;
            }
            _UpdateBaseDate();
            for (int i = 0; i < rolesList.Length; i++)
            {
                rolesNum += rolesList[i].Count;
                //rolesNum += rolesList[i].Count();
            }
            mRolesRoot.SetElementAmount(rolesNum);
        }

        private void _CloseFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<AdventureTeamExpeditionCharacterSelectFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AdventureTeamExpeditionCharacterSelectFrame>();
            }
        }
        #endregion

        #region UI
        void _InitRolesScrollListBind()
        {
            mRolesRoot.Initialize();

            mRolesRoot.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    _UpdateRolesScrollListBind(item);
                }
            };

            mRolesRoot.OnItemRecycle = (item) =>
            {
                if (item == null) return;
                AdventureTeamExpeditionCharacterSelectUnit mUnit = item.GetComponent<AdventureTeamExpeditionCharacterSelectUnit>();
                if(mUnit != null)
                {
                    mUnit.OnItemRecycle();
                }
            };
        }

        void _UpdateRolesScrollListBind(ComUIListElementScript item)
        {
            if (item == null) return;
            AdventureTeamExpeditionCharacterSelectUnit mUnit = item.GetComponent<AdventureTeamExpeditionCharacterSelectUnit>();
            if (mUnit == null) return;
            if (item.m_index < 0 || item.m_index >= rolesNum)
            {
                return;
            }
            ExpeditionRoleState mState;
            ExpeditionMemberInfo tempInfo = GetRoleInfoByIndex(item.m_index, out mState);
            if (AdventureTeamDataManager.GetInstance().IsRolesInExpeditionList(tempInfo))
            {
                mState = ExpeditionRoleState.SELECT;
            }
            else if(mState == ExpeditionRoleState.SELECT)
            {
                mState = ExpeditionRoleState.PREPARE;
            }

            mUnit.InitItemView(tempInfo, mState);
            mUnit.UpdateItemInfo();
        }

        ExpeditionMemberInfo GetRoleInfoByIndex(int index,out ExpeditionRoleState state)
        {
            if (index < rolesList[0].Count)
            {
                state = ExpeditionRoleState.SELECT;
                return rolesList[0][index];
            }
            else if (index < (rolesList[0].Count + rolesList[1].Count)) 
            {
                state = ExpeditionRoleState.PREPARE;
                return rolesList[1][index - rolesList[0].Count];
            }
            else if(index < (rolesList[0].Count + rolesList[1].Count + rolesList[2].Count))
            {
                state = ExpeditionRoleState.LEVEL_LIMIT;
                return rolesList[2][index - rolesList[0].Count - rolesList[1].Count];
            }
            else
            {
                state = ExpeditionRoleState.EXPEDITION;
                return rolesList[3][index - rolesList[0].Count - rolesList[1].Count - rolesList[2].Count];
            }
        }

        void _UpdateBaseDate()
        {
            rolesList[0] = new List<ExpeditionMemberInfo>();

            if(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo!=null&& AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.roles!=null)
            {
                for (int i = 0; i < AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.roles.Count; i++)
                {
                    rolesList[0].Add(AdventureTeamDataManager.GetInstance().ExpeditionMapNetInfo.roles[i]);
                }
            }
          
            List<ExpeditionMemberInfo>[] tempList = AdventureTeamDataManager.GetInstance().GetExpeditionRolesList();
            if(tempList!=null)
            {
                for (int i = 1; i < rolesList.Length; i++)
                {
                    rolesList[i] = tempList[i - 1];
                }
            }
          
        }

        private void _OnCloseButtonClickCallBack()
        {
            _CloseFrame();
        }
        #endregion

        #region PUBLIC METHODS
        public void InitData()
        {
            _InitBaseData();
        }

        public void Clear()
        {
            _ClearView();
        }
        #endregion
    }
}