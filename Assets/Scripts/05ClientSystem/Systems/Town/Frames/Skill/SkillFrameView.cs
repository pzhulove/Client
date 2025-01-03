using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SkillFrameView : MonoBehaviour
    {
        [SerializeField] private Button mClose = null;
        [SerializeField] private CommonTabToggleGroup mCommonVerticalTab = null;
        [SerializeField] private GameObject mSkillTreeRoot = null;
        [SerializeField] private GameObject mSkillPage = null;
        [SerializeField] private Toggle mPage1 = null;
        [SerializeField] private Toggle mPage2 = null;
        [SerializeField] private Button mBtnPlanLock = null;
        [SerializeField] private Text mSp = null;
        [SerializeField] private StateController mSkillTypeStateController = null;
        [SerializeField] private StateController mSkillPlanStateController = null;
        [SerializeField] private StateController mSkillTreeStateController = null;
        [SerializeField] private Drop_Me mSkillBack = null;
        [SerializeField] private int mSkillPlanItemId = 330000250;//解锁技能页2的道具id
        [SerializeField] private CommonTabData mFairDuelData;
        [SerializeField] private GameObject mObjChangeJob;
        [SerializeField] private List<CommonTabData> mSkillTabListDatas = new List<CommonTabData>();

        private bool bIsSwitchMainTabDrived = false;
        private Drop_Me skillTreeDropMe = null;
        private ScrollRect scroll = null;
        
        public void OnInit()
        {
            // 先把预制体加载出来，方便接受UIEvent
            _LoadSkillTreePrefab();
            // 初始化主页签
            _InitMainTab();
            // 初始化子页签
            _UpdatePageRoot();
            // 显示sp
            OnUpdateSp();
            // 更新技能页状态
            OnUpdatePlanStatus();
            // 显示转职按钮
            OnShowChangeJobBtn();
            if(mSkillBack != null)
                mSkillBack.ResponseDrop = SkillDataManager.GetInstance().DealDeleteDrop;
            if (SkillFrame.frameParam.frameType == SkillFrameType.Normal)
                mSkillTypeStateController.Key = "normal";
            else
                mSkillTypeStateController.Key = "fair";
            mSkillPlanStateController.Key = "normal";
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideEnd);

            _SetScrollViewContentPosition();
        }

        //显示转职按钮
        private void OnShowChangeJobBtn()
        {
            //最大等级限制
            int changeLimitLv = Utility.GetClientIntValue(ClientConstValueTable.eKey.CHANGE_JOB_LEVEL_LIMIT, 30);
            //完成转职任务并且在等级范围内才显示
            mObjChangeJob.CustomActive(Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.ChangeJob) && PlayerBaseData.GetInstance().Level <= changeLimitLv);
        }

        //更新技能页状态
        private void OnUpdatePlanStatus()
        {
            if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
            {
                mBtnPlanLock.CustomActive(!SkillDataManager.GetInstance().PVESkillPage2IsUnlock);
            }
            else if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP)
            {
                mBtnPlanLock.CustomActive(!SkillDataManager.GetInstance().PVPSkillPage2IsUnlock);
            }
        }

        //关闭配置界面
        public void OnCloseSkillConfigFrame()
        {
            mSkillPlanStateController.Key = "normal";
        }

        //解锁成功
        public void OnBuySkillPage2Sucess()
        {
            //购买成功 ，技能页二标签上播放解锁特效，同时出现飘字提示
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("Buy_SKill2Config_Sucess_Tip"));
            //技能页2解锁后，默认选中技能页2标签按钮，并打开技能页2的技能面板
            mPage2.isOn = true;
            OnUpdatePlanStatus();   
        }

        public void OnUninit()
        {
            bIsSwitchMainTabDrived = false;
            if(SkillFrame.frameParam != null)
                SkillFrame.frameParam.Clear();
            if (mSkillBack != null)
                mSkillBack.ResponseDrop = null;
            if(skillTreeDropMe != null)
            {
                skillTreeDropMe.ResponseDrop = null;
                skillTreeDropMe = null;
            }
            if (ClientSystemManager.GetInstance().IsFrameOpen<SkillBattleSettingFrame>())
                ClientSystemManager.GetInstance().CloseFrame<SkillBattleSettingFrame>();
            if (ClientSystemManager.GetInstance().IsFrameOpen<SkillConfigurationFrame>())
                ClientSystemManager.GetInstance().CloseFrame<SkillConfigurationFrame>();
        }

        //加载技能列表
        private void _LoadSkillTreePrefab()
        {
            // prefab文件路径
            string path = string.Format("UIFlatten/Prefabs/Skill/SkillTree_{0}", PlayerBaseData.GetInstance().JobTableID);

            GameObject SkillTreeObj = AssetLoader.instance.LoadResAsGameObject(path);
            if (SkillTreeObj == null)
            {
                Logger.LogErrorFormat("Load SkillTreeObj failed in SkillFrame, path = {0}", path);
                return;
            }

            // 绑定挂点
            SkillTreeObj.transform.SetParent(mSkillTreeRoot.transform, false);

            // 顺便绑定一下拖拽的响应回调
            skillTreeDropMe = Utility.FindComponent<Drop_Me>(SkillTreeObj, "Viewport");
            if (skillTreeDropMe != null)
            {
                skillTreeDropMe.ResponseDrop = SkillDataManager.GetInstance().DealDeleteDrop;
            }

            // 获取scrollrect
            scroll = SkillTreeObj.GetComponent<ScrollRect>();
        }

        //侧栏tablist
        private void _InitMainTab()
        {
            if (mCommonVerticalTab != null && SkillFrame.frameParam != null)
            {
                if (SkillFrame.frameParam.frameType == SkillFrameType.FairDuel)
                {
                    List<CommonTabData> TabListDatas = new List<CommonTabData>();
                    TabListDatas.Add(mFairDuelData);
                    mCommonVerticalTab.InitComTab(_MainTabOnClick, mFairDuelData.id, TabListDatas);
                }
                else
                {
                    for (int index = mSkillTabListDatas.Count - 1; index >= 0; --index)
                    {
                        if (mSkillTabListDatas[index].id == (int)SkillFrameTabType.PVP)
                        {
                            if (!Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Duel))
                            {
                                mSkillTabListDatas.Remove(mSkillTabListDatas[index]);
                                continue;
                            }
                        }
                    }
                    mCommonVerticalTab.InitComTab(_MainTabOnClick, (int)SkillFrame.frameParam.tabTypeIndex, mSkillTabListDatas);
                }
            }
        }
        private void _MainTabOnClick(CommonTabData tabData)
        {
            if (tabData == null)
            {
                Logger.LogError("tabData is null in SkillFrame");
                return;
            }

            SkillFrame.frameParam.tabTypeIndex = (SkillFrameTabType)tabData.id;
            bIsSwitchMainTabDrived = true;

            // 更新技能页状态
            OnUpdatePlanStatus();
            mSkillTreeStateController.Key = tabData.id < 2 ? "skillshow" : "skillhide";
            if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<SkillBattleSettingFrame>())
                    ClientSystemManager.GetInstance().CloseFrame<SkillBattleSettingFrame>();
                if (SkillDataManager.GetInstance().CurPVESKillPage == ESkillPage.Page1)
                {
                    if (mPage1 != null)
                    {
                        mPage1.isOn = false;
                        mPage1.isOn = true;
                    }
                }
                else
                {
                    if (mPage2 != null)
                    {
                        mPage2.isOn = false;
                        mPage2.isOn = true;
                    }
                }
            }
            else if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<SkillBattleSettingFrame>())
                    ClientSystemManager.GetInstance().CloseFrame<SkillBattleSettingFrame>();
                if (SkillFrame.frameParam.frameType != SkillFrameType.Normal)
                {
                    return;
                }

                if (SkillDataManager.GetInstance().CurPVPSKillPage == ESkillPage.Page1)
                {
                    if (mPage1 != null)
                    {
                        mPage1.isOn = false;
                        mPage1.isOn = true;
                    }
                }
                else
                {
                    if (mPage2 != null)
                    {
                        mPage2.isOn = false;
                        mPage2.isOn = true;
                    }
                }
            }
            else
            {
                //打开设置界面
                if (ClientSystemManager.GetInstance().IsFrameOpen<SkillConfigurationFrame>())
                    ClientSystemManager.GetInstance().CloseFrame<SkillConfigurationFrame>();
                ClientSystemManager.GetInstance().OpenFrame<SkillBattleSettingFrame>(FrameLayer.Middle);
            }
        }

        private void _UpdatePageRoot()
        {
            if (SkillFrame.frameParam != null)
            {
                if (mSkillPage != null)
                {
                    mSkillPage.CustomActive(SkillFrame.frameParam.frameType == SkillFrameType.Normal);
                }
            }
        }

        //更新sp点
        public void OnUpdateSp()
        {
            mSp.SafeSetText(SkillDataManager.GetInstance().GetCurSp().ToString());
        }

        //更新选中技能
        public void OnSelectSkillPage()
        {
            OnUpdateSp();

            if (SkillFrame.frameParam != null && !bIsSwitchMainTabDrived)
            {
                var equips = PlayerBaseData.GetInstance().GetEquipedEquipments();

                if (SkillFrame.frameParam.frameType == SkillFrameType.Normal && SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
                {
                    SkillDataManager.GetInstance().UpdateSkillLevelAddInfo(false, true);
                }
                else
                {
                   SkillDataManager.GetInstance().UpdateSkillLevelAddInfo(false, false);
                }
            }

            if (bIsSwitchMainTabDrived)
            {
                bIsSwitchMainTabDrived = false;
            }
        }

        //技能页1
        public void OnPage1ToggleValueChange(bool changed)
        {
            if(!changed)
                return;
            if(bIsSwitchMainTabDrived)
            {
                OnUpdateSp();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSelectSkillPage);
            }
            else
            {
                if(SkillFrame.frameParam.frameType == SkillFrameType.Normal)
                {
                    if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
                    {
                        SkillDataManager.GetInstance().SendChooseSkillPage(SkillConfigType.SKILL_CONFIG_PVE, (byte)ESkillPage.Page1);
                    }
                    else
                    {
                        SkillDataManager.GetInstance().SendChooseSkillPage(SkillConfigType.SKILL_CONFIG_PVP, (byte)ESkillPage.Page1);
                    }
                }
            }
        }

        //技能页2
        public void OnPage2ToggleValueChange(bool changed)
        {
            if(!changed)
                return;
            if (bIsSwitchMainTabDrived)
            {
                OnUpdateSp();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSelectSkillPage);
            }
            else
            {
                if (SkillFrame.frameParam.frameType == SkillFrameType.Normal)
                {
                    if (SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
                    {
                        SkillDataManager.GetInstance().SendChooseSkillPage(SkillConfigType.SKILL_CONFIG_PVE, (byte)ESkillPage.Page2);
                    }
                    else
                    {
                        SkillDataManager.GetInstance().SendChooseSkillPage(SkillConfigType.SKILL_CONFIG_PVP, (byte)ESkillPage.Page2);
                    }
                }
            } 
        }
        
        //点击打开技能配置
        public void OnBtSkillPlanButtonClick()
        {
            mSkillPlanStateController.Key = "back";
            ClientSystemManager.GetInstance().OpenFrame<SkillConfigurationFrame>(FrameLayer.Middle);
        }
        /// <summary>
        /// 技能配置2被锁住的点击
        /// </summary>
        public void OnLockPlan2Click()
        {
            if(!PlayerBaseData.IsJobChanged())
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("CnaNotBugSkillPage_Tip"));
                return;
            }
            var table = TableManager.GetInstance().GetTableItem<ItemTable>(mSkillPlanItemId);
            if (null == table)
            {
                Logger.LogErrorFormat("技能界面找不到id={0}的道具,检查预制体mSkillPlanItemId参数是否填写错误", mSkillPlanItemId);
                return;
            }
            CommonMsgBoxOkCancelNewParamData paraData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = string.Format(TR.Value("Unlock_SkillConfig2_Content"), table.Name, 1),
                IsShowNotify = false,
                LeftButtonText = TR.Value("Unlock_SkillConfig2_Cancel"),
                RightButtonText = TR.Value("Unlock_SkillConfig2_OK"),
                OnRightButtonClickCallBack = () => { OnPVPOrPVESureClick();}
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(paraData);
        }
        private void OnPVPOrPVESureClick()
        {
            var table = TableManager.GetInstance().GetTableItem<ItemTable>(mSkillPlanItemId);
            if (null == table)
            {
                Logger.LogErrorFormat("技能界面找不到id={0}的道具,检查预制体mSkillPlanItemId参数是否填写错误", mSkillPlanItemId);
                return;
            }
            if (ItemDataManager.GetInstance().GetOwnedItemCount(mSkillPlanItemId) < 1)
            {
                ItemComeLink.OnLink(mSkillPlanItemId, 0);
            }
            else
            {
                SkillDataManager.GetInstance().SendBuyCurSkillPageReq((byte)ESkillPage.Page2);
            }
        }

        //关闭界面
        public void OnCloseButtonClick()
        {
            ClientSystemManager.GetInstance().CloseFrame<SkillFrame>();
        }

        //选中技能页1/2 进行二次判断
        public void OnClickSelectPage1(bool isFrist)
        {
            CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
            if(isFrist)
                comconMsgBoxOkCancelParamData.ContentLabel = string.Format(TR.Value("SkillPageExchange_Tip_Content"));
            else
                comconMsgBoxOkCancelParamData.ContentLabel = string.Format(TR.Value("SkillPageExchange_Tip_Content2"));
           
            comconMsgBoxOkCancelParamData.IsShowNotify = false;
            comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("SkillPageExchange_Tip_Cancel");
            comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("SkillPageExchange_Tip_OK");
            comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack = ()=>{
                if (isFrist)
                    mPage1.isOn = true;
                else
                    mPage2.isOn = true;
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(comconMsgBoxOkCancelParamData);
        }

        private void _SetScrollViewContentPosition()
        {
            if(scroll != null)
            {
                if(PlayerBaseData.GetInstance().Level >= 30)
                {
                    scroll.verticalNormalizedPosition = 0.07f;
                }
                else if(PlayerBaseData.GetInstance().Level >= 15 && PlayerBaseData.GetInstance().Level < 30)
                {
                    scroll.verticalNormalizedPosition = 0.45f;
                }            
            }
        }

        //打开转职界面
        public void OnClickChangeJob()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ChangeJobSelectFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ChangeJobSelectFrame>();
            }

            ChangeJobType param = ChangeJobType.SwitchJob;

            ClientSystemManager.GetInstance().OpenFrame<ChangeJobSelectFrame>(FrameLayer.Middle, param);
        }

        public void OnClickGetSp()
        {
            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("skill_add_sp_tips"));
        }
    }
}
