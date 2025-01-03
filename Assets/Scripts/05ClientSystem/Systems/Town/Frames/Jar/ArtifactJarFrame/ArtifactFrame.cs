using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;

namespace GameClient
{

    public class ArtifactFrame : ClientFrame
    {
        #region inner_def

        public enum MainTabType
        {
            MagicJarDiscount, // 魔罐折扣
            PrizeRecord,      // 派奖记录
        }

        public enum PrizeTabType
        {
            Invalid,
            MyPrize, // 我的派奖
            Record,  // 派奖记录
        }

        #endregion



        #region val
        MainTabType mainType = MainTabType.MagicJarDiscount;
        PrizeTabType prizeType = PrizeTabType.Invalid;

        //const string myPrizePrefabPath = "UIFlatten/Prefabs/ArtifactJar/ArtifactDailyReward"; // 我的奖励
        //const string prizeRecordPrefabPath = ""; // 奖励预览

        #endregion

        #region ui bind
        private Button btnClose = null;
        private Toggle ToggleMagicJar = null;
        private Toggle ToggleRecord = null;
        private GameObject magicJarRoot = null;
        private GameObject recordRoot = null;
        private Toggle ToggleMyPrize = null;
        private Toggle TogglePrizeRecord = null;
        private GameObject recordContent = null;
        private Text title = null;
        private GameObject mRewardRedPoint = null;
        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ArtifactJar/ArtifactFrame";
        }

        protected override void _OnOpenFrame()
        {           
            BindUIEvent();

            if(IsArtifactJarDiscountActivityOpen() && (IsArtifactJarRewardActivityOpen() || IsArtifactJarShowActivityOpen()))
            {
                SetShowContent(MainTabType.MagicJarDiscount, PrizeTabType.Invalid);
            }
            else if(IsArtifactJarDiscountActivityOpen())
            {
                ToggleRecord.CustomActive(false);
                SetShowContent(MainTabType.MagicJarDiscount, PrizeTabType.Invalid);
            }
            else if(IsArtifactJarRewardActivityOpen() || IsArtifactJarShowActivityOpen())
            {
                ToggleMagicJar.CustomActive(false);
                SetShowContent(MainTabType.PrizeRecord, PrizeTabType.MyPrize);
            }
            else
            {
                ToggleMagicJar.CustomActive(false);
                ToggleRecord.CustomActive(false);
            }

            _UpdateRewardToggleRedPoint();
            
            ArtifactDataManager.GetInstance().SendGASArtifactJarLotteryCfgReq();
        }

        protected override void _OnCloseFrame()
        {
            ActivityJarFrame.frameType = ActivityJarFrameType.Normal;
            if(ClientSystemManager.GetInstance().IsFrameOpen<ArtifactJarDailyRewardFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ArtifactJarDailyRewardFrame>();
            }
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            btnClose = mBind.GetCom<Button>("btnClose");
            btnClose.SafeRemoveAllListener();
            btnClose.SafeAddOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().CloseFrame<ActivityJarFrame>();
                frameMgr.CloseFrame(this);
            });

            ToggleMagicJar = mBind.GetCom<Toggle>("ToggleMagicJar");
            ToggleMagicJar.SafeAddOnValueChangedListener((bool bValue) => 
            {
                if(bValue)
                {
                    //ArtifactDataManager.GetInstance().SendArtifactJarDiscount();

                    mainType = MainTabType.MagicJarDiscount;
                    prizeType = PrizeTabType.Invalid;

                    UpdateMainTabContent();

                    title.SafeSetText(TR.Value("artifactJar"));

                    StaticUtility.SafeSetVisible(mBind, "Help1", true);
                    StaticUtility.SafeSetVisible(mBind, "Help2", false);
                }
            });

            ToggleRecord = mBind.GetCom<Toggle>("ToggleRecord");
            ToggleRecord.SafeAddOnValueChangedListener((bool bValue) => 
            {
                if(bValue)
                {
                    mRewardRedPoint.CustomActive(false);
                    ArtifactDataManager.GetInstance().isArtifactRecordNew = false;
                    mainType = MainTabType.PrizeRecord;
                    prizeType = PrizeTabType.MyPrize;

                    UpdateMainTabContent();

                    title.SafeSetText(TR.Value("dailyAward"));

                    UpdateMainTabContent();
                    if (!ClientSystemManager.GetInstance().IsFrameOpen<ArtifactJarDailyRewardFrame>())
                    {
                        ClientSystemManager.GetInstance().OpenFrame<ArtifactJarDailyRewardFrame>(recordRoot);
                    }
                    ArtifactDataManager.GetInstance().SendArtifactJar();
                    StaticUtility.SafeSetVisible(mBind, "Help1", false);
                    StaticUtility.SafeSetVisible(mBind, "Help2", true);
                }
                else
                {
                    //ClientSystemManager.GetInstance().CloseFrame<ArtifactJarDailyRewardFrame>();
                }
            });

            magicJarRoot = mBind.GetGameObject("magicJarRoot");
            recordRoot = mBind.GetGameObject("recordRoot");

            //ToggleMyPrize = mBind.GetCom<Toggle>("ToggleMyPrize");
            //ToggleMyPrize.SafeAddOnValueChangedListener((bool bValue) => 
            //{
            //    UpdateRecordContent(myPrizePrefabPath);
            //});

            //TogglePrizeRecord = mBind.GetCom<Toggle>("TogglePrizeRecord");
            //TogglePrizeRecord.SafeAddOnValueChangedListener((bool bValue) => 
            //{
            //    UpdateRecordContent(prizeRecordPrefabPath);
            //});

            recordContent = mBind.GetGameObject("recordContent");

            title = mBind.GetCom<Text>("title");

            mRewardRedPoint = mBind.GetGameObject("RewardRedPoint");
    }

        protected override void _unbindExUI()
        {
            btnClose = null;
            ToggleMagicJar = null;
            ToggleRecord = null;
            magicJarRoot = null;
            recordRoot = null;
            ToggleMyPrize = null;
            TogglePrizeRecord = null;
            recordContent = null;
            title = null;
            mRewardRedPoint = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            
        }

        void UnBindUIEvent()
        {
            
        }

        public void SetShowContent(MainTabType mainTabType,PrizeTabType prizeTabType)
        {
            mainType = mainTabType;
            prizeType = prizeTabType;

            ToggleMagicJar.SafeSetToggleOnState(mainTabType == MainTabType.MagicJarDiscount);
            ToggleRecord.SafeSetToggleOnState(mainTabType == MainTabType.PrizeRecord);           

            return;
        }

        private void SetShowPrizeRecord(PrizeTabType prizeTab)
        {
            ToggleMyPrize.SafeSetToggleOnState(prizeTab == PrizeTabType.MyPrize);
            TogglePrizeRecord.SafeSetToggleOnState(prizeTab == PrizeTabType.Record);
        }

        private void UpdateMainTabContent()
        {
            magicJarRoot.CustomActive(mainType == MainTabType.MagicJarDiscount);
            recordRoot.CustomActive(mainType == MainTabType.PrizeRecord);
            
            if(mainType == MainTabType.MagicJarDiscount)
            {
                if(magicJarRoot != null)
                {
                    //ClientSystemManager.GetInstance().OpenFrame<ActivityJarFrame>(magicJarRoot, ActivityJarFrameType.Artifact, "");
                    ActivityJarFrame.frameType = ActivityJarFrameType.Artifact;
                    ClientSystemManager.instance.OpenFrame(typeof(ActivityJarFrame), magicJarRoot);
                }
            }
            else
            {
                ClientSystemManager.GetInstance().CloseFrame<ActivityJarFrame>();
            }

            if(mainType == MainTabType.PrizeRecord)
            {
                SetShowPrizeRecord(prizeType);
            }

            return;
        }       

        private void _UpdateRewardToggleRedPoint()
        {
            if (ArtifactDataManager.GetInstance().isArtifactRecordNew)
            {
                mRewardRedPoint.CustomActive(true);
            }
            else
            {
                mRewardRedPoint.CustomActive(false);
            }
        }
        //private void UpdateRecordContent(string prefabPath)
        //{
        //    if(recordContent == null)
        //    {
        //        return;
        //    }

        //    for (int i = 0; i < recordContent.transform.childCount; ++i)
        //    {
        //        GameObject.Destroy(recordContent.transform.GetChild(i).gameObject);
        //    }

        //    GameObject objPrefab = AssetLoader.GetInstance().LoadRes(myPrizePrefabPath).obj as GameObject;
        //    if(objPrefab != null)
        //    {
        //        objPrefab.transform.SetParent(recordContent.transform, false);
        //        objPrefab.SetActive(true);
        //    }           

        //    return;
        //}

        public static bool IsArtifactJarShowActivityOpen()
        {
            var artifactAct = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_ARTIFACT_JAR_SHOW);
            return (artifactAct != null && artifactAct.state == (int)OpActivityState.OAS_IN);
        }
        public static bool IsArtifactJarDiscountActivityOpen()
        {
            if (ArtifactDataManager.GetInstance() == null)
            {
                return false;
            }

            OpActivityData data = ArtifactDataManager.GetInstance().getArtifactJarActData();
            if (data == null)
            {
                return false;
            }

            return data.state == (byte)OpActivityState.OAS_IN;
        }

        public static bool IsArtifactJarRewardActivityOpen()
        {
            if (ArtifactDataManager.GetInstance() == null)
            {
                return false;
            }

            OpActivityData data = ArtifactDataManager.GetInstance().getArtifactAwardActData();
            if (data == null)
            {
                return false;
            }

            return data.state == (byte)OpActivityState.OAS_IN;
        }

        #endregion
        
        #region ui event


        #endregion
    }
}
