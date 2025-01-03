using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GameClient
{
    //[Serializable]
    //public class AdventureTeamExpFlyEffectConfig
    //{
    //    public GameObject flyTarget;                           //经验飞行目标位置
    //    public float flyDuration = 1f;                         //经验飞行时间
    //    public float flyPathRadian = 100f;                     //经验飞行轨迹弧度
    //}

    public class AdventureTeamContentPassingView : AdventureTeamContentBaseView
    {
        //[SerializeField] private Text mRichText;
        [HeaderAttribute("Item")]
        [SerializeField] private Text mItemName;
        [SerializeField] private Image mItemIcon;
        [SerializeField] private Text mItemNumCount;
        [SerializeField] private Text mCrystalIntroucation;

        [HeaderAttribute("Exp")]
        [SerializeField] private ComExpBar mExpSlider;
        [SerializeField] private GameObject mExpRewardRoot;
        [SerializeField] private Text mExpReward;
        [SerializeField] private Button mExpBtn;
        [SerializeField] private UIGray mExpBtnUIGray;
        [SerializeField] private SetComButtonCD mExpBtnCD;
        [SerializeField] private Text mExpBtnText;

        [HeaderAttribute("Player")]
        [SerializeField] private Image mPlayerIcon;
        [SerializeField] private Text mPlayerName;
        [SerializeField] private Text mPlayerLV;

        [HeaderAttribute("Other")]
        [SerializeField] private ComUIListScript mItemListRoot;
        [SerializeField] private Text mPlayerLevelNotify;
        [SerializeField] private Text mResetTimeDesc;

        [HeaderAttribute("Effect")]
        [SerializeField] private AdventureTeamPassBlessExpPoolBind mExpPoolEffectBind;      

        [SerializeField] private CommonFrameButtonBuryPoint mBuryPoint;  

        int rewardMaxExp = 0;               //使用一个成长药剂 能获得的角色经验值
        int rewardUnitExp = 0;              //使用一个单元的成长药剂转换池 能获得的角色经验值

        ulong inheritBlessUnitExp = 0;      //成长药剂转换池  生成一个单元 所需要的经验值
        ulong inheritBlessMaxExp = 0;       //成长药剂转换池  充满  所需要的经验值

        uint passBlessItemOwnCount = 0;
        uint passBlessItemMaxCount = 0;

        private string tr_pass_bless_exp_can_get_format = "";
        private string tr_pass_bless_count_format = "";
        private string tr_pass_bless_exp_accumulate_format = "";
        private string tr_pass_bless_role_level_format = "";
        private string tr_pass_bless_get_exp_level_info = "";
        private string tr_pass_bless_reset_time_desc = "";
        private string tr_pass_bless_exp_btn_desc_get_one = "";
        private string tr_pass_bless_exp_btn_desc_get_oneten = "";
        private string tr_pass_bless_exp_btn_desc_get_some = "";
        private string tr_pass_bless_exp_btn_is_playing_anim = "";

        private int currEmptyExpFlyTargetIndex = 0;
        private AdventureTeamPassBlessExpPotionBind currEmptyExpFlyTargetBind = null;
        int lastExpPercent = 0;
        int currExpPercent = 0;
        bool isEffectPlaying = false;

        bool isSkipAnim = false;

        private void Awake()
        {
            BindEvents();

            _InitTR();
            _InitPassingBlessInfo();
            _InitExpItemScrollListBind();
            _InitEffectHandler();
        }

        private void OnDestroy()
        {
            UnBindEvents();

            _ClearTR();
            _ClearExpItemScrollListBind();
            _UnInitEffectHandler();

            currEmptyExpFlyTargetBind = null;
            AdventureTeamDataManager.GetInstance().ResetUiTempPassBlessModel();
            isEffectPlaying = false;
        }

        private void BindEvents()
        {
            if (null != mExpBtn)
            {
                mExpBtn.onClick.AddListener(ExpRewardMsgBox);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAdventureTeamInheritBlessInfoRes, _InitPassingBlessNetInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExpChanged, _RefreshPassBlessContent);
        }

        private void UnBindEvents()
        {
            if (null != mExpBtn)
            {
                mExpBtn.onClick.RemoveListener(ExpRewardMsgBox);
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAdventureTeamInheritBlessInfoRes, _InitPassingBlessNetInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExpChanged, _RefreshPassBlessContent);
        }

        private void _InitTR()
        {
            tr_pass_bless_exp_can_get_format = TR.Value("adventure_team_pass_bless_Exp_Can_Get");
            tr_pass_bless_count_format = TR.Value("adventure_team_pass_bless_count");
            tr_pass_bless_exp_accumulate_format = TR.Value("adventure_team_pass_bless_CumulativeEXP");
            tr_pass_bless_role_level_format = TR.Value("adventure_team_pass_bless_role_level");
            tr_pass_bless_get_exp_level_info = TR.Value("adventure_team_pass_bless_get_exp_level_info");
            tr_pass_bless_reset_time_desc = TR.Value("adventure_team_pass_bless_reset_time_desc");
            tr_pass_bless_exp_btn_desc_get_one = TR.Value("adventure_team_pass_bless_btn_get_one");
            tr_pass_bless_exp_btn_desc_get_oneten = TR.Value("adventure_team_pass_bless_btn_get_oneten");
            tr_pass_bless_exp_btn_desc_get_some = TR.Value("adventure_team_pass_bless_btn_get_someexp");
            tr_pass_bless_exp_btn_is_playing_anim = TR.Value("adventure_team_pass_bless_btn_is_effect_playing");
        }

        private void _ClearTR()
        {
            tr_pass_bless_exp_can_get_format = "";
            tr_pass_bless_count_format = "";
            tr_pass_bless_exp_accumulate_format = "";
            tr_pass_bless_role_level_format = "";
            tr_pass_bless_get_exp_level_info = "";
            tr_pass_bless_reset_time_desc = "";
            tr_pass_bless_exp_btn_desc_get_one = "";
            tr_pass_bless_exp_btn_desc_get_oneten = "";
            tr_pass_bless_exp_btn_desc_get_some = "";
            tr_pass_bless_exp_btn_is_playing_anim = "";
        }

        void _InitExpItemScrollListBind()
        {
            if (mItemListRoot != null && mItemListRoot.IsInitialised() == false)
            {
                mItemListRoot.Initialize();
                mItemListRoot.onItemVisiable += _OnExpItemVisible;
                mItemListRoot.OnItemUpdate += _OnExpItemUpdate;
            }
        }

        void _ClearExpItemScrollListBind()
        {
            if (mItemListRoot == null)
            {
                mItemListRoot.onItemVisiable -= _OnExpItemVisible;
                mItemListRoot.OnItemUpdate -= _OnExpItemUpdate;
                mItemListRoot.UnInitialize();
            }
        }

        private void _OnExpItemVisible(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            if (item.m_index < 0 || item.m_index >= passBlessItemMaxCount)
            {
                return;
            }
            var bind = item.GetComponent<AdventureTeamPassBlessExpPotionBind>();
            if (null == bind)
            {
                return;
            }
            
            string index_str = Utility.GetUnitNumWithHeadZero(item.m_index, true);

            bool bEmpty = false;
            if (isSkipAnim)
            {
                bEmpty = passBlessItemOwnCount > item.m_index ? false : true;
            }
            else
            {
                //使用缓存的数据
                int expFlyTimes = AdventureTeamDataManager.GetInstance().CheckNeedFlyExpTimes();
                if (expFlyTimes > 0)
                {
                    var lastPassBlessModel = AdventureTeamDataManager.GetInstance().UiTempInheritBlessModel;
                    if (lastPassBlessModel != null)
                    {
                        bEmpty = lastPassBlessModel.ownInheritBlessNum > item.m_index ? false : true;
                    }
                }
                else
                {
                    bEmpty = passBlessItemOwnCount > item.m_index ? false : true;
                }
            }

            bind.InitView(index_str, bEmpty);


            //将最早遍历到的空位置存起来
            if (bind.GetDrugIsEmpty())
            {
                //初始化
                if (currEmptyExpFlyTargetBind == null)
                {
                    currEmptyExpFlyTargetBind = bind;
                    currEmptyExpFlyTargetIndex = item.m_index;
                }
                else if (currEmptyExpFlyTargetBind.GetDrugIsEmpty() == false)
                {
                    currEmptyExpFlyTargetBind = bind;
                    currEmptyExpFlyTargetIndex = item.m_index;
                }
                else if (item.m_index < currEmptyExpFlyTargetIndex &&
                    bind != currEmptyExpFlyTargetBind)
                {
                    currEmptyExpFlyTargetBind = bind;
                    currEmptyExpFlyTargetIndex = item.m_index;
                }
            }
            //Logger.LogErrorFormat("{0} visable : currEmptyExpFlyTargetBind index is {1} ",item.m_index, currEmptyExpFlyTargetIndex);
        }

        private void _OnExpItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }
            if (item.m_index < 0 || item.m_index >= passBlessItemMaxCount)
            {
                return;
            }
            var bind = item.GetComponent<AdventureTeamPassBlessExpPotionBind>();
            if (null == bind)
            {
                return;
            }

            bool bEmpty = false;
            if (isSkipAnim)
            {
                bEmpty = passBlessItemOwnCount > item.m_index ? false : true;
            }
            else
            {
                //使用缓存的数据
                int expFlyTimes = AdventureTeamDataManager.GetInstance().CheckNeedFlyExpTimes();
                if (expFlyTimes > 0)
                {
                    var lastPassBlessModel = AdventureTeamDataManager.GetInstance().UiTempInheritBlessModel;
                    if (lastPassBlessModel != null)
                    {
                        bEmpty = lastPassBlessModel.ownInheritBlessNum > item.m_index ? false : true;
                    }
                }
                else
                {
                    bEmpty = passBlessItemOwnCount > item.m_index ? false : true;
                }
            }

            if (bEmpty)
            {
                bind.Useup();
            }
            else
            {
                bind.Fillup();
            }

            //将最早遍历到的空位置存起来
            if (bind.GetDrugIsEmpty())
            {
                //初始化
                if (currEmptyExpFlyTargetBind == null)
                {
                    currEmptyExpFlyTargetBind = bind;
                    currEmptyExpFlyTargetIndex = item.m_index;
                }
                else if (currEmptyExpFlyTargetBind.GetDrugIsEmpty() == false)
                {
                    currEmptyExpFlyTargetBind = bind;
                    currEmptyExpFlyTargetIndex = item.m_index;
                }
                else if (item.m_index < currEmptyExpFlyTargetIndex &&
                    bind != currEmptyExpFlyTargetBind)
                {
                    currEmptyExpFlyTargetBind = bind;
                    currEmptyExpFlyTargetIndex = item.m_index;
                }
            }
            //Logger.LogErrorFormat("{0} update : currEmptyExpFlyTargetBind index is {1} ", item.m_index, currEmptyExpFlyTargetIndex);
        }

        //客户端表格数据
        private void _InitPassingBlessInfo()
        {
            //道具获取
            var itemName = "";
            var itemNeedLevel = "";
            var itemMaxLevel = "";
            //var itemSpritePath = "";
            var itemId = 0;
            var item = AdventureTeamDataManager.GetInstance().PassBlessItem;
            if (null != item)
            {
                itemName = item.Name;
                itemNeedLevel = item.NeedLevel.ToString();
                itemMaxLevel = item.MaxLevel.ToString();
                //itemSpritePath = item.Icon;
                itemId = item.ID;
            }
            //最大等级
            var maxLevel = AdventureTeamDataManager.GetInstance().PlayerMaxLevel;
            //10% = 100,000 ?
            var multiple = 10;
            var itemRewardMaxExp = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_INHERIT_BLESS_USE_REWARD_PLAYER_EXP);
            var itemRewardUnitExp = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_INHERIT_BLESS_USE_UNIT_EXP_REWARD_PLAYER_EXP);
            if(null != itemRewardMaxExp && null != itemRewardUnitExp && itemRewardUnitExp.Value != 0)
            {
                rewardMaxExp = itemRewardMaxExp.Value;
                rewardUnitExp = itemRewardUnitExp.Value;

                if (rewardMaxExp != 0)
                {
                    multiple = rewardMaxExp / rewardUnitExp;
                }
                
            }
            //if (null != mRichText)
            //{
            //    mRichText.text  = TR.Value("adventure_team_pass_bless_richtext1", maxLevel.ToString(), itemName, multiple.ToString());
            //    mRichText.text += TR.Value("adventure_team_pass_bless_richtext2", itemName);
            //    mRichText.text += TR.Value("adventure_team_pass_bless_richtext3", multiple.ToString(),Utility.ToThousandsSeparator((ulong)rewardUnitExp), Utility.ToThousandsSeparator((ulong)rewardMaxExp));
            //    mRichText.text += TR.Value("adventure_team_pass_bless_richtext4", itemName, itemNeedLevel, itemMaxLevel);
            //    mRichText.text = mRichText.text.Replace("\\n", "\n");
            //}
            if (null != mItemName)
            {
                mItemName.text = itemName;
            }

            if (mPlayerLevelNotify)
            {
                // mPlayerLevelNotify.text = string.Format(tr_pass_bless_get_exp_level_info, itemNeedLevel.ToString(), itemMaxLevel.ToString());
                mPlayerLevelNotify.text = string.Format(tr_pass_bless_get_exp_level_info, "50", itemMaxLevel.ToString());
            }

            if (mResetTimeDesc)
            {
                mResetTimeDesc.text = tr_pass_bless_reset_time_desc;
            }

            //if (null != mItemIcon)
            //{
            //    ETCImageLoader.LoadSprite(ref mItemIcon, itemSpritePath);
            //}
            //TODO 加载exp图片
            //if(null != mEXPImage)
            //{
            //    ETCImageLoader.LoadSprite(ref mEXPImage, "");
            //}
        }

        private void _RefreshPassBlessBtnInfo(bool hasInited = false)
        {
            var _inheritBlessModel = AdventureTeamDataManager.GetInstance().InheritBlessModel;
            if (_inheritBlessModel == null)
            {
                return;
            }
            var _inheritExpModel = AdventureTeamDataManager.GetInstance().InheritExpModel;
            if (_inheritExpModel == null)
            {
                return;
            }
            passBlessItemOwnCount = _inheritBlessModel.ownInheritBlessNum;

            //from _InitPassingBlessNetInfo(UIEvent ui)
            passBlessItemMaxCount = _inheritBlessModel.inheritBlessMaxNum;

            var ownExp = _inheritExpModel.ownInheritBlessExp;

            //from _InitPassingBlessNetInfo(UIEvent ui)
            var unitExp = _inheritExpModel.inheritBlessUnitExp;
            var maxExp = _inheritExpModel.inheritBlessMaxExp;
            inheritBlessUnitExp = unitExp;
            inheritBlessMaxExp = maxExp;

            //刷新经验要状态状态
            if (!hasInited)
            {
                if (mItemListRoot)
                {
                    mItemListRoot.SetElementAmount((int)passBlessItemMaxCount);
                }
            }
            else
            {
                if (mItemListRoot)
                {
                    mItemListRoot.UpdateElement();
                }
            }

            _UpdateExpBtnState(ownExp, inheritBlessUnitExp);
            _UpdatePlayerInfo();
        }

        void _UpdateExpBtnState(ulong ownExp,ulong unitExp)
        {
            if (null != mItemNumCount)
            {
                mItemNumCount.text = string.Format(tr_pass_bless_count_format, passBlessItemOwnCount.ToString(), passBlessItemMaxCount.ToString());
            }

            if (null != mCrystalIntroucation)
            {
                mCrystalIntroucation.text = string.Format(tr_pass_bless_exp_accumulate_format, Utility.ToThousandsSeparator(ownExp), Utility.ToThousandsSeparator(inheritBlessMaxExp));
            }
            if (passBlessItemOwnCount > 0)
            {
                _SetRewardExpGetDesc(Utility.ToThousandsSeparator((ulong)rewardMaxExp));
                _SetExpBtnDesc(tr_pass_bless_exp_btn_desc_get_one);
            }
            else if (passBlessItemOwnCount <= 0 && ownExp > unitExp)
            {
                _SetRewardExpGetDesc(Utility.ToThousandsSeparator((ulong)(ownExp * 0.1)));
                _SetExpBtnDesc(tr_pass_bless_exp_btn_desc_get_oneten);
            }
            else
            {
                _SetRewardExpGetDesc("0");
                _SetExpBtnDesc(tr_pass_bless_exp_btn_desc_get_some);
            }

            bool isEnableToUse = AdventureTeamDataManager.GetInstance().IsEnableToUsePassBless();
            if (!isEnableToUse)
            {
                _SetExpBtnEnable(false);
                _SetExpBtnActive(false);
            }
			else if(ownExp < unitExp && passBlessItemOwnCount == 0)
			{
				_SetExpBtnEnable(false);
                _SetExpBtnActive(true);
			}
            else
            {
                _SetExpBtnEnable(true);
                _SetExpBtnActive(true);
            }
        }

        private void _SetExpBtnEnable(bool bEnable)
        {
            if (mExpBtn)
            {
                mExpBtn.enabled = bEnable;
            }
            if (mExpBtnUIGray)
            {
                mExpBtnUIGray.enabled = !bEnable;
            }
        }

        private void _SetExpBtnActive(bool bActive)
        {
            if (mExpBtn)
            {
                mExpBtn.CustomActive(bActive);
            }
            if (mPlayerLevelNotify)
            {
                mPlayerLevelNotify.CustomActive(!bActive);
            }
            if (mExpReward)
            {
                mExpReward.CustomActive(bActive);
                mExpRewardRoot.CustomActive(bActive);
            }
        }

        private void _SetRewardExpGetDesc(string rewardExpStr)
        {
            if (mExpReward)
            {
                mExpReward.text = string.Format(tr_pass_bless_exp_can_get_format, rewardExpStr);
            }
        }

        private void _SetExpBtnDesc(string desc)
        {
            if (mExpBtnText)
            {
                mExpBtnText.text = desc;
            }
        }

        private void _InitPlayerBaseInfo()
        {
            SetPlayerIcon();
            SetPlayerName();
            _UpdatePlayerInfo();
        }

        private void _UpdatePlayerInfo()
        {
            var level = PlayerBaseData.GetInstance().Level;
            var currLevelExp = TableManager.GetInstance().GetExpByLevel(level);
            var curExp = PlayerBaseData.GetInstance().CurExp;
            if (mPlayerLV)
            {
                mPlayerLV.text = string.Format(tr_pass_bless_role_level_format,level.ToString());
            }
            if (mExpSlider)
            {
                mExpSlider.SetExp(curExp, true, (exp) =>
                {
                    return new KeyValuePair<ulong, ulong>(exp, currLevelExp);
                });
            }
        }

        void SetPlayerName()
        {
            if (mPlayerName)
                mPlayerName.text = PlayerBaseData.GetInstance().Name;
        }

        void SetPlayerIcon()
        {
            string path = "";

            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobData != null)
            {
                ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                {
                    path = resData.IconPath;
                }
            }
            if (mPlayerIcon && !string.IsNullOrEmpty(path))
            {
                ETCImageLoader.LoadSprite(ref mPlayerIcon, path);
            }
        }

        private void _TryQueryPassBlessData()
        {
            _SetExpBtnEnable(false);
            _SetExpBtnActive(true);
            AdventureTeamDataManager.GetInstance().ReqPassBlessInfo();
        }

        public override void InitData()
        {
            _InitEffectView();
            _InitPlayerBaseInfo();
            _TryQueryPassBlessData();
            AdventureTeamDataManager.GetInstance().OnFirstCheckPassBlessFlag = false;
        }

        public override void OnEnableView()
        {
            _TryQueryPassBlessData();
            AdventureTeamDataManager.GetInstance().OnFirstCheckPassBlessFlag = false;
        }

        public override void OnDisableView()
        {
            AdventureTeamDataManager.GetInstance().ResetUiTempPassBlessModel();
        }

        #region Callback

        private void _InitPassingBlessNetInfo(UIEvent ui)
        {
            //传承祝福道具数据
            //var _inheritBlessModel = AdventureTeamDataManager.GetInstance().InheritBlessModel;
            //if (_inheritBlessModel == null)
            //{
            //    return;
            //}
            //var _inheritExpModel = AdventureTeamDataManager.GetInstance().InheritExpModel;
            //if (_inheritExpModel == null)
            //{
            //    return;
            //}

            //passBlessItemOwnCount = _inheritBlessModel.ownInheritBlessNum;
            //passBlessItemCount = _inheritBlessModel.inheritBlessMaxNum;

            //var ownExp = _inheritExpModel.ownInheritBlessExp;
            //var unitExp = _inheritExpModel.inheritBlessUnitExp;
            //var maxExp = _inheritExpModel.inheritBlessMaxExp;
            //inheritBlessUnitExp = unitExp;
            //inheritBlessMaxExp = maxExp;

            //mItemListRoot.SetElementAmount((int)passBlessItemCount);
            //_UpdateExpBtnState(ownExp, unitExp);
            //_UpdatePlayerInfo();

            _RefreshPassBlessBtnInfo();
            _RefreshEffectView();
        }

        private void _RefreshPassBlessContent(UIEvent uiEvent)
        {
            _RefreshPassBlessBtnInfo(true);
            _RefreshEffectView();
        }

        private void ExpRewardMsgBox()
        {
            if (isEffectPlaying)
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_pass_bless_exp_btn_is_playing_anim);
                return;
            }

            string notify = string.Empty;
            var curExp = PlayerBaseData.GetInstance().CurExp;
            var rewardExp = passBlessItemOwnCount > 0 ? rewardMaxExp : rewardUnitExp;
            ushort addUpLevel = 0;
            var playerLevel = PlayerBaseData.GetInstance().Level;

            double tempExp = (double)rewardExp + curExp - TableManager.GetInstance().GetExpByLevel(playerLevel + addUpLevel++);
            double percent = 0;

            if (tempExp < 0)
            {
                double currLevel = (double)TableManager.GetInstance().GetExpByLevel(playerLevel);
                if(currLevel != 0)
                {
                    percent = ((ulong)rewardExp + curExp) * 100 / currLevel;
                    addUpLevel--;
                }
            }
            else
            {
                while (tempExp > TableManager.GetInstance().GetExpByLevel(playerLevel + addUpLevel))
                {
                    tempExp -= TableManager.GetInstance().GetExpByLevel(playerLevel + addUpLevel++);
                }

                double currLevel = (double)TableManager.GetInstance().GetExpByLevel(playerLevel + addUpLevel);
                if (currLevel != 0)
                {
                    percent = tempExp * 100 / currLevel;
                }
            }
            notify = TR.Value("adventure_team_pass_bless_button_tips", rewardExp, playerLevel + addUpLevel, percent);
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(notify, OnExpRewardBtnClick);
        }

        private void OnExpRewardBtnClick()
        {
            if (mExpBtnCD != null && mExpBtnCD.IsBtnWork())
            {
                AdventureTeamDataManager.GetInstance().ReqUsePassBlessExp();

                //埋点
                if(mBuryPoint != null)
                {
                    mBuryPoint.OnSendBuryingPoint();
                }
            }
        }

        #endregion

        #region Effect Bind

        private void _InitEffectView()
        {
            if (mExpPoolEffectBind != null)
            {
                mExpPoolEffectBind.InitExpPoolIdleEffect();
                mExpPoolEffectBind.InitExpPoolFullFlyingEffect();
                mExpPoolEffectBind.InitExpPoolFillupEffect();
                mExpPoolEffectBind.InitExpPoolRiseupEffect();
            }
        }
        private void _InitEffectHandler()
        {
            if (mExpPoolEffectBind != null)
            {
                mExpPoolEffectBind.ExpRiseupToFullHandler += _OnExpRiseUpToFull;
                mExpPoolEffectBind.ExpFlyToTargetHandler += _OnExpFlyToTarget;
                mExpPoolEffectBind.ExpRiseupStartHandler += _OnExpStartRiseup;
                mExpPoolEffectBind.ExpRiseupEndHandler += _OnExpEndRiseUp;
            }
        }

        private void _UnInitEffectHandler()
        {
            if (mExpPoolEffectBind != null)
            {
                mExpPoolEffectBind.ExpRiseupToFullHandler -= _OnExpRiseUpToFull;
                mExpPoolEffectBind.ExpFlyToTargetHandler -= _OnExpFlyToTarget;
                mExpPoolEffectBind.ExpRiseupStartHandler -= _OnExpStartRiseup;
                mExpPoolEffectBind.ExpRiseupEndHandler -= _OnExpEndRiseUp;
            }
        }

        private void _RefreshEffectView()
        {
            var lastPassBlessModel =  AdventureTeamDataManager.GetInstance().UiTempInheritBlessModel;
            var lasetPassBlessExpModel = AdventureTeamDataManager.GetInstance().UiTempInheritExpModel;
            int lastPassBlessNum = 0;
            ulong lastPassBlessExp = 0;
            if (lastPassBlessModel != null)
            {
                lastPassBlessNum = (int)lastPassBlessModel.ownInheritBlessNum;
            }
            if (lasetPassBlessExpModel != null)
            {
                lastPassBlessExp = lasetPassBlessExpModel.ownInheritBlessExp;
            }
            int currPassBlessNum = 0;
            ulong currPassBlessExp = 0;
            var _inheritBlessModel = AdventureTeamDataManager.GetInstance().InheritBlessModel;
            if (_inheritBlessModel != null)
            {
                currPassBlessNum = (int)_inheritBlessModel.ownInheritBlessNum;
            }
            var _inheritExpModel = AdventureTeamDataManager.GetInstance().InheritExpModel;
            if (_inheritExpModel != null)
            {
                currPassBlessExp = _inheritExpModel.ownInheritBlessExp;
            }

            if (inheritBlessMaxExp == 0)
            {
                return;
            }


            //第一次打开界面
            if (lastPassBlessModel == null || lasetPassBlessExpModel == null)
            {
                double currPercent = currPassBlessExp * 100 / (double)inheritBlessMaxExp;
                currExpPercent = (int)currPercent;
                if (mExpPoolEffectBind != null)
                {
                    mExpPoolEffectBind.StartExpRiseupToHeight(0, currExpPercent);
                }
            }
            else
            {
                double lastPercent = lastPassBlessExp * 100 / (double)inheritBlessMaxExp;
                double currPercent = currPassBlessExp * 100 / (double)inheritBlessMaxExp;
                lastExpPercent = (int)lastPercent;
                currExpPercent = (int)currPercent;

                if (isSkipAnim)
                {
                    if (mExpPoolEffectBind != null)
                    {
                        mExpPoolEffectBind.StartExpRiseupToHeight(lastExpPercent, currExpPercent);
                    }
                }
                else
                {
                    int expFlyTimes = AdventureTeamDataManager.GetInstance().CheckNeedFlyExpTimes();
                    if (expFlyTimes > 0)
                    {
                        if (mExpPoolEffectBind != null)
                        {
                            mExpPoolEffectBind.StartExpRiseupToHeight(currExpPercent, 100, true);
                        }
                    }
                    else
                    {
                        if (mExpPoolEffectBind != null)
                        {
                            mExpPoolEffectBind.StartExpRiseupToHeight(lastExpPercent, currExpPercent);
                        }
                    }
                }
            }
        }

        private void _OnExpRiseUpToFull()
        {
            if (mExpPoolEffectBind != null && currEmptyExpFlyTargetBind != null)
            {
                mExpPoolEffectBind.SetExpPoolFillupShow(true);
                mExpPoolEffectBind.StartFullExpFlyingToTarget(currEmptyExpFlyTargetBind.GetEmptyExpFlyTarget());
            }
        }

        private void _OnExpFlyToTarget()
        {
            //界面缓存数据自增1
            AdventureTeamDataManager.GetInstance().AddupOneExpTempNum();
            //刷新当前经验药状态
            if (mItemListRoot)
            {
                mItemListRoot.UpdateElement();
            }

            int expFlyTimes = AdventureTeamDataManager.GetInstance().CheckNeedFlyExpTimes();
            if (expFlyTimes > 0)
            {
                if (mExpPoolEffectBind != null && currEmptyExpFlyTargetBind != null)
                {
                    mExpPoolEffectBind.SetExpPoolFillupShow(true);
                    mExpPoolEffectBind.StartFullExpFlyingToTarget(currEmptyExpFlyTargetBind.GetEmptyExpFlyTarget());
                }
            }
            else
            {
                if (mExpPoolEffectBind != null)
                {
                    mExpPoolEffectBind.StartExpRiseupToHeight(0, currExpPercent);
                }
            }
        }

        private void _OnExpStartRiseup()
        {
            if (mExpPoolEffectBind != null)
            {
                mExpPoolEffectBind.SetExpPoolRiseupShow(true);
            }
            isEffectPlaying = true;
        }

        private void _OnExpEndRiseUp()
        {
            //这里是真正的结束
            AdventureTeamDataManager.GetInstance().ResetUiTempPassBlessModel();
            isEffectPlaying = false;
        }

        #endregion
    }
}