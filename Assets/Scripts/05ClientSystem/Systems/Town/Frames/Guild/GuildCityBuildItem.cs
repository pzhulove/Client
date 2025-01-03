using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GuildCityBuildItem : MonoBehaviour
    {
        [SerializeField]
        private GuildBuildingType mBuildType;
        [SerializeField]
        private GameObject mLockGo;
        [SerializeField]
        private Text mLvTxt;
        [SerializeField]
        private Button mBtn;
        [SerializeField]
        private GameObject mRedPointGo;

        private void Awake()
        {
            _BindEvt();
            _UpdateCityBuildItem();
            _UpdateRedPoint();
        }


        private void OnDestroy()
        {
            _BindEvt();
        }

        private void _BindEvt()
        {
            mBtn.SafeAddOnClickListener(_OnBtnClick);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnUpgradeBuildingSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
        }

       

        private void _UnBindEvt()
        {
            mBtn.SafeRemoveOnClickListener(_OnBtnClick);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnUpgradeBuildingSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
        }

        private void _UpdateCityBuildItem()
        {
            if (!GuildDataManager.GetInstance().HasSelfGuild()) return;
            Dictionary<GuildBuildingType, GuildBuildingData> dicBuilds  = GuildDataManager.GetInstance().myGuild.dictBuildings;
            if (dicBuilds == null) return;
            //是否未开启
            bool isLock = GuildDataManager.GetInstance().GetGuildLv() < GuildDataManager.GetInstance().GetUnLockBuildingNeedMainCityLv(mBuildType);
            if(mBtn!=null)
            {
                mBtn.interactable = !isLock;
            }
            mLockGo.CustomActive(isLock);
            mLvTxt.CustomActive(!isLock);
            if(!isLock)//解锁了显示等级
            {
                GuildBuildingData data = null;
                if (dicBuilds.TryGetValue(mBuildType,out data))
                {
                    int nCurrentLevel = data.nLevel;
                    int nMaxLevel = data.nMaxLevel;
                    if (nMaxLevel <= 0)
                    {
                        nCurrentLevel = 1;
                        nMaxLevel = 1;
                    }
                    mLvTxt.SafeSetText(string.Format("Lv.{0}", nCurrentLevel));
                }
                else
                {
                    Logger.LogErrorFormat("为找到此类型：{0}建筑的数据",mBuildType);
                }
            }

        }
        void _OnUpgradeBuildingSuccess(UIEvent a_event)
        {
            _UpdateCityBuildItem();
            _UpdateRedPoint();
        }
        private void _OnBtnClick()
        {
           switch(mBuildType)
            {
                case GuildBuildingType.SHOP:
                    _ResponseShopBtnClick();
                    break;
                case GuildBuildingType.HONOUR:
                    _ResponseHonorTempletBtnClick();
                    break;
                case GuildBuildingType.FETE:
                    _ResponseWarArtBtnClick();
                    break;
                case GuildBuildingType.MAIN:
                    _ResponseMainCityBtnClick();
                    break;
            }
        }

        private void _ResponseShopBtnClick()
        {
            int nShopLevel = GuildDataManager.GetInstance().myGuild.dictBuildings[GuildBuildingType.SHOP].nLevel;
            ProtoTable.GuildBuildingTable buildingTable = TableManager.GetInstance().GetTableItem<ProtoTable.GuildBuildingTable>(nShopLevel);
            if (buildingTable != null)
            {
                ShopNewDataManager.GetInstance().OpenShopNewFrame(buildingTable.ShopId);
            }
        }

        private void _ResponseWarArtBtnClick()
        {
            if (!(GuildDataManager.GetInstance().GetGuildLv() >= GuildDataManager.GetGuildDungeonActivityGuildLvLimit()))
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_build_boss_lv_set_condition", GuildDataManager.GetGuildDungeonActivityGuildLvLimit()));
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<GuildDungeonBossDiffSetFrame>();
            if (mRedPointGo.activeSelf)
            {
                GuildDataManager.GetInstance().checkedSetBossDiff = true;
            }
            mRedPointGo.CustomActive(false);
        }

        private void _ResponseHonorTempletBtnClick()
        {
            if (!(GuildDataManager.GetInstance().GetGuildLv() >= GuildDataManager.GetInstance().GetEmblemLvUpGuildLvLimit()
                   && PlayerBaseData.GetInstance().Level >= GuildDataManager.GetInstance().GetEmblemLvUpPlayerLvLimit()))
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("emblem_unlock_condition", GuildDataManager.GetInstance().GetEmblemLvUpGuildLvLimit(), GuildDataManager.GetInstance().GetEmblemLvUpPlayerLvLimit()));
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<GuildEmblemUpFrame>();
            if (mRedPointGo.activeSelf)
            {
                GuildDataManager.GetInstance().checkedLvUpEmblem = true;
            }
        }

        private void _ResponseMainCityBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildBuildingManagerFrame>();
            if (mRedPointGo.activeSelf)
            {
                GuildDataManager.GetInstance().checkedLvUpBulilding = true;
            }
            mRedPointGo.CustomActive(false);
        }
        private void _OnRedPointChanged(UIEvent a_event)
        {
            _UpdateRedPoint();
        }

        private void _UpdateRedPoint()
        {
            ERedPoint eRedPoint = ERedPoint.Invalid;
            switch (mBuildType)
            {
                case GuildBuildingType.HONOUR:
                    eRedPoint = ERedPoint.GuildEmblem;
                    break;
                case GuildBuildingType.FETE:
                    eRedPoint = ERedPoint.GuildSetBossDiff;
                    break;
                case GuildBuildingType.MAIN:
                    eRedPoint = ERedPoint.GuildBuildingManager;
                    break;
                default:
                    eRedPoint = ERedPoint.Invalid;
                    break;
            }
            if(eRedPoint==ERedPoint.Invalid)
            {
                mRedPointGo.CustomActive(false);
            }
            else
            {
                mRedPointGo.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(eRedPoint));
            }
        }
    }
}

