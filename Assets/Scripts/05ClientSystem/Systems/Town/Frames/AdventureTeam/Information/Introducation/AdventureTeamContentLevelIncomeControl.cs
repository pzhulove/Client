using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamContentLevelIncomeControl : MonoBehaviour
    {
        [Space(15)]
        [HeaderAttribute("Left Content")]

        [HeaderAttribute("Title")]
        [SerializeField] private Text mAdventureTeamName;
        [SerializeField] private Button mAdventureTeamChangeNameBtn;
        [SerializeField] private Text mRoleNumFormat;
        [SerializeField] private Text mAdventureTeamGrade;
        [SerializeField] private Text mAdentureTeamRank;
        [SerializeField] private Button mAdventureTeamRankHelpBtn;
        [SerializeField] private ComArtLettering mAdventureTeamLevel;
        [SerializeField] private ComExpBar mAdventureTeamExpBar;

        [Space(5)] [HeaderAttribute("Content")]
        [SerializeField] private Text mAdventureTeamLevelTitle;
        [SerializeField] private Text propertyIncomeDesc;

        [Space(5)] [HeaderAttribute("Button")]
        [SerializeField] private Button minusButton;
        [SerializeField] private UIGray minusButtonGray;
        [SerializeField] private Button addButton;
        [SerializeField] private UIGray addButtonGray;
        [SerializeField] private ComDotController dotRoot;

        [Space(15)]
        [HeaderAttribute("Right Content")]

        [Space(5)]
        [HeaderAttribute("BlessShop")]
        [SerializeField]
        private Text blessCrystalNameLabel;
        [SerializeField]
        private Text blessCrystalOwnCount;
        [SerializeField]
        private Image blessCrystalIcon;
        [SerializeField]
        private Text blessCrystalIntroductionTitle;
        [SerializeField]
        private GameObject blessCrystalItemRoot;
        [SerializeField]
        private ComExpBar blessCrystalExpBar;
        [SerializeField]
        private Button openBlessShopBtn;

        [Space(10)]
        [HeaderAttribute("BountyShop")]
        [SerializeField]
        private Text bountyNameLabel;
        [SerializeField]
        private Text bountyOwnCount;
        [SerializeField]
        private Image bountyIcon;
        [SerializeField]
        private GameObject bountyItemRoot;
        [SerializeField]
        private Text bountyIntroductionTitle;
        [SerializeField]
        private Button openBountyShopBtn;
        

        private int _adventureTeamLevel = 0;
        private ComItem _blessCrystalComItem = null;
        private ComItem _bountyComItem = null;

        private string tr_adventure_team_name_format = "";
        private string tr_adventure_team_grade_color = "";
        private string tr_adventure_team_not_in_rank = "";
        private string tr_adventure_team_role_field_count_format = "";
        private string tr_bless_crystal_count_format = "";
        //private string tr_bless_crystal_item_count_format = "";
        private string tr_bless_crystal_introduction = "";
        private string tr_bounty_count_format = "";
        private string tr_bounty_introduction = "";
        private string tr_adventure_team_grade_with_score = "";

        private void Awake()
        {
            BindEvents();
            _InitTR();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            _ClearView();
        }

        private void BindEvents()
        {
            if (minusButton != null)
            {
                minusButton.onClick.AddListener(OnMinusButtonClickedCallBack);
            }
            if (addButton != null)
            {
                addButton.onClick.AddListener(OnAddButtonClickedCallBack);
            }
            if (openBlessShopBtn)
            {
                openBlessShopBtn.onClick.AddListener(OnOpenBlessShopBtnClick);
            }
            if (openBountyShopBtn)
            {
                openBountyShopBtn.onClick.AddListener(OnOpenBountyShopBtnClick);
            }

            if (mAdventureTeamChangeNameBtn)
            {
                mAdventureTeamChangeNameBtn.onClick.AddListener(_OnChangeNameBtnClick);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamBlessCrystalCountChanged, _OnBlessCrystalCountChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamBlessCrystalExpChanged, _OnBlessCrystalExpChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamBountyCountChanged, _OnBountyCountChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamBaseInfoRes, _OnBaseInfoRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamBlessCrystalInfoRes, _OnBlessCrystalInfoRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamRenameSucc, _OnAdventureTeamRenameSucc);
        }

        private void UnBindEvents()
        {
            if (minusButton != null)
            {
                minusButton.onClick.RemoveListener(OnMinusButtonClickedCallBack);
            }
            if (addButton != null)
            {
                addButton.onClick.RemoveListener(OnAddButtonClickedCallBack);
            }
            if (openBlessShopBtn)
            {
                openBlessShopBtn.onClick.RemoveListener(OnOpenBlessShopBtnClick);
            }
            if (openBountyShopBtn)
            {
                openBountyShopBtn.onClick.RemoveListener(OnOpenBountyShopBtnClick);
            }

            if (mAdventureTeamChangeNameBtn)
            {
                mAdventureTeamChangeNameBtn.onClick.RemoveListener(_OnChangeNameBtnClick);
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamBlessCrystalCountChanged, _OnBlessCrystalCountChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamBlessCrystalExpChanged, _OnBlessCrystalExpChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamBountyCountChanged, _OnBountyCountChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamBaseInfoRes, _OnBaseInfoRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamBlessCrystalInfoRes, _OnBlessCrystalInfoRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamRenameSucc, _OnAdventureTeamRenameSucc);
        }

        private void _InitTR()
        {
            tr_adventure_team_name_format = TR.Value("adventure_team_role_select_nameinfo");
            tr_adventure_team_grade_color = TR.Value("adventure_team_grade_color");
            tr_adventure_team_not_in_rank = TR.Value("adventure_team_not_in_rank");
            tr_adventure_team_role_field_count_format = TR.Value("adventure_team_role_field_count");
            tr_bless_crystal_count_format = TR.Value("adventure_team_bless_crystal_count");
            //tr_bless_crystal_item_count_format = TR.Value("adventure_team_bless_crystal_itemcount");
            tr_bless_crystal_introduction = TR.Value("adventure_team_bless_crystal_introducation");
            tr_bounty_count_format = TR.Value("adventure_team_bounty_count");
            tr_bounty_introduction = TR.Value("adventure_team_bounty_introduction");
            tr_adventure_team_grade_with_score = TR.Value("adventure_team_grade_with_score");
        }

        private void _ClearView()
        {
            tr_adventure_team_name_format = "";
            tr_adventure_team_grade_color = "";
            tr_adventure_team_not_in_rank = "";
            tr_adventure_team_role_field_count_format = "";
            tr_bless_crystal_count_format = "";
            //tr_bless_crystal_item_count_format = "";
            tr_bless_crystal_introduction = "";
            tr_bounty_count_format = "";
            tr_bounty_introduction = "";
            tr_adventure_team_grade_with_score = "";

            if (_blessCrystalComItem != null)
            {
                ComItemManager.Destroy(_blessCrystalComItem);
                _blessCrystalComItem = null;
            }
            if (_bountyComItem != null)
            {
                ComItemManager.Destroy(_bountyComItem);
                _bountyComItem = null;
            }
        }

        private void _InitAdventureTeamBaseInfo()
        {
            if (mAdventureTeamName)
            {
                tr_adventure_team_name_format = tr_adventure_team_name_format.Replace("\n", "");
                mAdventureTeamName.text = string.Format(tr_adventure_team_name_format, AdventureTeamDataManager.GetInstance().GetColorAdventureTeamName());
            }
            if (mRoleNumFormat)
            {
                if (ClientApplication.playerinfo != null && ClientApplication.playerinfo.roleinfo != null)
                {
                    int hasRoleNum = RecoveryRoleCachedObject.HasOwnedRoles;
                    int baseRoleFieldNum = RecoveryRoleCachedObject.EnabledRoleField;
                    mRoleNumFormat.text = string.Format(tr_adventure_team_role_field_count_format, hasRoleNum, baseRoleFieldNum);
                }
            }
            if (mAdventureTeamLevel)
            {
                mAdventureTeamLevel.SetNum(AdventureTeamDataManager.GetInstance().GetAdventureTeamLevel());
            }
            if (mAdventureTeamExpBar)
            {
                mAdventureTeamExpBar.SetExp((ulong)AdventureTeamDataManager.GetInstance().GetAdventureTeamCurrExp(), true, (exp) =>
                {
                    return AdventureTeamDataManager.GetInstance().GetAdventureTeamCurrExpWithUpLevelExp(exp);
                });
            }

            if (mAdventureTeamGrade)
            {
                string gradeStr = AdventureTeamDataManager.GetInstance().GetColorAdventureTeamGrade();
                uint score = AdventureTeamDataManager.GetInstance().GetAdventureTeamScore();
                mAdventureTeamGrade.text = string.Format(tr_adventure_team_grade_with_score, gradeStr, score.ToString());
            }
            if (mAdentureTeamRank)
            {
                uint rank = AdventureTeamDataManager.GetInstance().GetAdventureTeamRank();
                if (rank == 0)
                {
                    mAdentureTeamRank.text = tr_adventure_team_not_in_rank;
                }
                else
                {
                    mAdentureTeamRank.text = rank.ToString();
                }
            }
        }

        private void _UpdateBaseInfo()
        {
            if (mAdventureTeamName)
            {
                tr_adventure_team_name_format = tr_adventure_team_name_format.Replace("\n", "");
                mAdventureTeamName.text = string.Format(tr_adventure_team_name_format, AdventureTeamDataManager.GetInstance().GetColorAdventureTeamName());
            }
        }

        private void _InitIncomeInfo()
        {
            int teamLevel = AdventureTeamDataManager.GetInstance().GetAdventureTeamLevel();

            if (teamLevel < AdventureTeamDataManager.GetInstance().AdventureTeamLevelMinimum || teamLevel > AdventureTeamDataManager.GetInstance().AdventureTeamLevelMaximum)
            {
                _adventureTeamLevel = AdventureTeamDataManager.GetInstance().AdventureTeamLevelMinimum;
            }
            else
            {
                _adventureTeamLevel = teamLevel;
            }

            if (dotRoot)
            {
                dotRoot.InitDots(AdventureTeamDataManager.GetInstance().AdventureTeamLevelMaximum);
                dotRoot.CustomActive(false);
            }

            UpdateIncomeContent();
        }

        private void UpdateIncomeContent()
        {
            UpdateIncomeInfo();
            UpdateIncomeButton();

            if (dotRoot)
            {
                int maxLevel = AdventureTeamDataManager.GetInstance().AdventureTeamLevelMaximum;
                dotRoot.CustomActive(maxLevel >= 2);
                dotRoot.SetDots(_adventureTeamLevel, maxLevel);
            }
        }

        private void UpdateIncomeInfo()
        {
            if (mAdventureTeamLevelTitle)
            {
                mAdventureTeamLevelTitle.text = Utility.GetUnitNumWithHeadZero(_adventureTeamLevel, false);
            }
            if (propertyIncomeDesc)
            {
                propertyIncomeDesc.text = AdventureTeamDataManager.GetInstance().GetAdventureTeamIncomeDescByLevel(_adventureTeamLevel);
            }
        }

        private void UpdateIncomeButton()
        {
            //最小值
            if (_adventureTeamLevel <= AdventureTeamDataManager.GetInstance().AdventureTeamLevelMinimum)
            {
                SetMinusButton(false);
                SetAddButton(true);
            }
            else if (_adventureTeamLevel >= AdventureTeamDataManager.GetInstance().AdventureTeamLevelMaximum)
            {
                //最大值
                SetMinusButton(true);
                SetAddButton(false);
            }
            else
            {
                //中间数值
                SetMinusButton(true);
                SetAddButton(true);
            }
        }

        private void SetMinusButton(bool flag)
        {
            if (minusButton != null)
            {
                minusButton.interactable = flag;
            }

            if (minusButtonGray != null)
            {
                minusButtonGray.enabled = !flag;
            }
        }

        private void SetAddButton(bool flag)
        {
            if (addButton != null)
            {
                addButton.interactable = flag;
            }

            if (addButtonGray != null)
            {
                addButtonGray.enabled = !flag;
            }
        }

        #region Bless Crystal Shop Info

        private void _InitBlessCrystalContent()
        {
            var _blessCrystalModel = AdventureTeamDataManager.GetInstance().BlessCrystalModel;
            if (_blessCrystalModel == null)
            {
                return;
            }

            if (blessCrystalNameLabel)
            {
                blessCrystalNameLabel.text = _blessCrystalModel.itemName;
            }

            if (blessCrystalIcon && !string.IsNullOrEmpty(_blessCrystalModel.itemIconPath))
            {
                ETCImageLoader.LoadSprite(ref blessCrystalIcon, _blessCrystalModel.itemIconPath);
            }

            if (blessCrystalItemRoot)
            {
                ItemData blessCrystalItemData = ItemDataManager.CreateItemDataFromTable((int)_blessCrystalModel.itemTableId);
                if (_blessCrystalComItem == null)
                {
                    _blessCrystalComItem = ComItemManager.Create(blessCrystalItemRoot);
                }
                _blessCrystalComItem.Setup(blessCrystalItemData, (go, itemData) => {
                    ItemTipManager.GetInstance().ShowTip(itemData);
                });
                //_blessCrystalComItem.SetCountFormatter((var) =>
                //{
                //    return string.Format(tr_bless_crystal_count_format, _blessCrystalModel.currOwnCount.ToString());
                //}); 
                _blessCrystalComItem.CustomActive(true);
            }

            if (blessCrystalIntroductionTitle)
            {
                blessCrystalIntroductionTitle.text = string.Format(tr_bless_crystal_introduction, _blessCrystalModel.itemName);
            }

            _UpdateBlessCrystalContent();
        }

        private void _UpdateBlessCrystalContent()
        {
            var _blessCrystalModel = AdventureTeamDataManager.GetInstance().BlessCrystalModel;
            if (_blessCrystalModel == null)
            {
                return;
            }
            //if (_blessCrystalComItem != null)
            //{
            //    _blessCrystalComItem.SetCountFormatter((var) =>
            //    {
            //        return string.Format(tr_bless_crystal_item_count_format,
            //            _blessCrystalModel.currOwnCount.ToString(),
            //            _blessCrystalModel.currNumMaximum.ToString());
            //    }); 
            //}
            if (blessCrystalOwnCount)
            {
                blessCrystalOwnCount.text = string.Format(tr_bless_crystal_count_format, _blessCrystalModel.currOwnCount.ToString(), _blessCrystalModel.currNumMaximum.ToString());
            }

            if (blessCrystalExpBar)
            {
                blessCrystalExpBar.SetExp((ulong)_blessCrystalModel.currOwnExp, true, (exp) =>
                {
                    return AdventureTeamDataManager.GetInstance().GetBlessCrystalShopCurrExpWithMaxExp(exp);
                });
            }
        }

        #endregion

        #region Bounty Shop Info

        private void _InitBountyContent()
        {
            var _bountyModel = AdventureTeamDataManager.GetInstance().BountyModel;
            if (_bountyModel == null)
            {
                return;
            }

            if (bountyNameLabel)
            {
                bountyNameLabel.text = _bountyModel.itemName;
            }

            if (blessCrystalIcon && !string.IsNullOrEmpty(_bountyModel.itemIconPath))
            {
                ETCImageLoader.LoadSprite(ref blessCrystalIcon, _bountyModel.itemIconPath);
            }

            if (bountyItemRoot)
            {
                ItemData bountyItemData = ItemDataManager.CreateItemDataFromTable((int)_bountyModel.itemTableId);
                if (_bountyComItem == null)
                {
                    _bountyComItem = ComItemManager.Create(bountyItemRoot);
                }
                _bountyComItem.Setup(bountyItemData, (go, itemData) =>
                {
                    ItemTipManager.GetInstance().ShowTip(itemData);
                });
                _bountyComItem.CustomActive(true);
            }

            if (bountyIntroductionTitle)
            {
                bountyIntroductionTitle.text = string.Format(tr_bounty_introduction, _bountyModel.itemName, _bountyModel.itemName);
            }

            _UpdateBountyContent();
        }

        private void _UpdateBountyContent()
        {
            var _bountyModel = AdventureTeamDataManager.GetInstance().BountyModel;
            if (_bountyModel == null)
            {
                return;
            }

            //if (_bountyComItem != null)
            //{
            //    _bountyComItem.SetCountFormatter((var) =>
            //    {
            //        if (_bountyModel.currOwnCount <= 0)
            //        {
            //            return "";
            //        }
            //        return _bountyModel.currOwnCount.ToString();
            //    });
            //}
            if (bountyOwnCount)
            {
                bountyOwnCount.text = string.Format(tr_bounty_count_format, _bountyModel.currOwnCount.ToString());
            }
        }

        #endregion

        #region UI Callback

        private void OnMinusButtonClickedCallBack()
        {
            if (_adventureTeamLevel <= AdventureTeamDataManager.GetInstance().AdventureTeamLevelMinimum)
                return;

            _adventureTeamLevel -= 1;
            if (_adventureTeamLevel < AdventureTeamDataManager.GetInstance().AdventureTeamLevelMinimum)
                _adventureTeamLevel = AdventureTeamDataManager.GetInstance().AdventureTeamLevelMinimum;

            UpdateIncomeContent();
        }

        private void OnAddButtonClickedCallBack()
        {
            if (_adventureTeamLevel >= AdventureTeamDataManager.GetInstance().AdventureTeamLevelMaximum)
                return;

            _adventureTeamLevel += 1;
            if (_adventureTeamLevel > AdventureTeamDataManager.GetInstance().AdventureTeamLevelMaximum)
                _adventureTeamLevel = AdventureTeamDataManager.GetInstance().AdventureTeamLevelMaximum;

            UpdateIncomeContent();
        }

        private void OnOpenBlessShopBtnClick()
        {
            AccountShopDataManager.GetInstance().OpenAccountShop(AccountShopDataManager.ADVENTURE_TEAM_SHOP_ID, AccountShopDataManager.ADVENTURE_TEAM_BLESS_CRYSTAL_CHILD_SHOP_ID);
        }

        private void OnOpenBountyShopBtnClick()
        {
            AccountShopDataManager.GetInstance().OpenAccountShop(AccountShopDataManager.ADVENTURE_TEAM_SHOP_ID, AccountShopDataManager.ADVENTURE_TEAM_BOUNTY_CHILD_SHOP_ID);
        }

        private void _OnChangeNameBtnClick()
        {
            AdventureTeamRenameModel model = new AdventureTeamRenameModel();
            ClientSystemManager.GetInstance().OpenFrame<AdventureTeamChangeNameFrame>(FrameLayer.Middle, model);
        }

        private void _OnBlessCrystalInfoRes(UIEvent uiEvent)
        {
            _InitBlessCrystalContent();
        }

        private void _OnBaseInfoRes(UIEvent uiEvent)
        {
            _InitAdventureTeamBaseInfo();
            _InitIncomeInfo();
            _InitBountyContent();
        }

        private void _OnBlessCrystalCountChanged(UIEvent uiEvent)
        {
            _UpdateBlessCrystalContent();
        }

        private void _OnBlessCrystalExpChanged(UIEvent uiEvent)
        {
            _UpdateBlessCrystalContent();
        }

        private void _OnBountyCountChanged(UIEvent uiEvent)
        {
            _UpdateBountyContent();
        }

        private void _OnAdventureTeamRenameSucc(UIEvent _uiEvent)
        {
            _UpdateBaseInfo();
        }

        #endregion

        #region PUBLIC METHOD

        public void TryInitBaseInfoView()
        {
            _InitAdventureTeamBaseInfo();
            _InitIncomeInfo();
            _InitBountyContent();
            _InitBlessCrystalContent();
        }
       
        #endregion
    }

}