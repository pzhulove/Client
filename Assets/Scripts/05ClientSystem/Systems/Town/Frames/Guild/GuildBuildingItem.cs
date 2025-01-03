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
using DG.Tweening;


namespace GameClient
{
    using UIItemData = AwardItemData;

    public class GuildBuildingItem : MonoBehaviour
    {
        [SerializeField]
        Image icon = null;

        [SerializeField]
        Image name = null;

        [SerializeField]
        Text lv = null;

        [SerializeField]
        Text desc = null;

        [SerializeField]
        Text mainLvLimit = null;

        [SerializeField]
        Text unlockNeedMainLv = null;

        [SerializeField]
        Slider process = null;

        [SerializeField]
        Text cost = null;

        [SerializeField]
        Button btnLvUp = null;

        [SerializeField]
        Text maxLv = null;

        [SerializeField]
        Text btnLvUpText = null;
        
        GuildBuildingType guildBuildingType = GuildBuildingType.MAIN;

        // Use this for initialization
        void Start()
        {
            
        }

        private void OnDestroy()
        {
                    
        }

        // Update is called once per frame
        void Update()
        {
           
        }
        
        bool CanLvUpBuildingByType(GuildBuildingType guildBuildingType)
        {
            GuildBuildingType guildBuildingTypeTemp = guildBuildingType;
            GuildBuildingData guildBuildingDataTemp = GuildDataManager.GetInstance().GetBuildingData(guildBuildingTypeTemp);
            if (guildBuildingDataTemp != null)
            {
                if (guildBuildingDataTemp.nUnlockMaincityLevel > 0 && guildBuildingDataTemp.nLevel >= guildBuildingDataTemp.nMaxLevel)
                {                   
                    return false;
                }

                if (GuildDataManager.GetInstance().GetMyGuildFund() < guildBuildingDataTemp.nUpgradeCost)
                {                    
                    return false;
                }

                return true;
            }

            return false;
        }

        public void SetUp(object data)
        {
            if(data == null)
            {
                return;
            }

            if(!(data is GuildBuildingData))
            {
                return;
            }

            GuildBuildingData guildBuildingData = data as GuildBuildingData;
            GuildBuildInfoTable guildBuildInfoTable = GuildDataManager.GetInstance().GetGuildBuildInfoTable(guildBuildingData.eType);
            if(guildBuildInfoTable == null)
            {
                return;
            }
            guildBuildingType = guildBuildingData.eType;

            icon.SafeSetImage(guildBuildInfoTable.buildIconPath);
            name.SafeSetImage(guildBuildInfoTable.buildNamePath);
            if(name != null)
            {
                name.SetNativeSize();
            }           

            desc.SafeSetText(guildBuildInfoTable.buildDesc);            

            int nCurrentLevel = guildBuildingData.nLevel;
            int nMaxLevel = guildBuildingData.nMaxLevel;
            if (nMaxLevel <= 0)
            {
                nCurrentLevel = 1;
                nMaxLevel = 1;
            }           

            lv.SafeSetText(TR.Value("guild_build_lv_info", nCurrentLevel));
            mainLvLimit.SafeSetText(TR.Value("guild_build_lv_up_need_main_lv", guildBuildingData.nUnlockMaincityLevel));

            int nFound = GuildDataManager.GetInstance().GetMyGuildFund();
            if (process != null)
            {
                float fValue = 0.0f;                
                if (guildBuildingData.nUpgradeCost > 0)
                {
                    fValue = Math.Min(1.0f, (float)nFound / (float)guildBuildingData.nUpgradeCost);
                }
                process.value = fValue;
            }

            cost.SafeSetText(string.Format("{0}/{1}",nFound,guildBuildingData.nUpgradeCost));

            bool bShowLvUpBtn = true;
            bool bNeedMainLvLimit = false;

            if (nCurrentLevel >= nMaxLevel)
            {
                if (guildBuildingData.nUnlockMaincityLevel > 0)
                {                    
                    bShowLvUpBtn = true;
                    bNeedMainLvLimit = true;
                }
                else
                {                   
                    bShowLvUpBtn = false;
                }
            }
            else
            {
                bShowLvUpBtn = true;
            }

            if(guildBuildingData.eType == GuildBuildingType.MAIN)
            {
                bNeedMainLvLimit = false;
            }
       
            btnLvUp.SafeSetOnClickListener(() =>
            {
                GuildBuildingType guildBuildingTypeTemp = guildBuildingType;
                GuildBuildingData guildBuildingDataTemp = GuildDataManager.GetInstance().GetBuildingData(guildBuildingTypeTemp);
                if (guildBuildingDataTemp != null)
                {
                    if (guildBuildingDataTemp.nUnlockMaincityLevel > 0 && guildBuildingDataTemp.nLevel >= guildBuildingDataTemp.nMaxLevel)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_build_lv_up_main_lv_not_enough"));
                        return;
                    }


                    if (GuildDataManager.GetInstance().GetMyGuildFund() < guildBuildingDataTemp.nUpgradeCost)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guild_build_lv_up_fund_not_enough"));
                        return;
                    }

                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("guild_upgrade_building_ask", guildBuildingDataTemp.nUpgradeCost), () =>
                    {
                        GuildDataManager.GetInstance().UpgradeBuilding(guildBuildingDataTemp.eType, guildBuildingDataTemp.nUpgradeCost);
                    });
                }
            });

            // 建筑是否解锁
            bool bBuildingUnLocked = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.MAIN) >= GuildDataManager.GetInstance().GetUnLockBuildingNeedMainCityLv(guildBuildingType);
            if(bBuildingUnLocked)
            {
                btnLvUpText.SafeSetText(TR.Value("guild_build_lv_up"));
            }
            else
            {
                btnLvUpText.SafeSetText(TR.Value("guild_build_lv_up_need_main_lv_to_unlcok", GuildDataManager.GetInstance().GetUnLockBuildingNeedMainCityLv(guildBuildingType)));
                unlockNeedMainLv.SafeSetText(TR.Value("guild_build_lv_up_need_main_lv_to_unlcok", GuildDataManager.GetInstance().GetUnLockBuildingNeedMainCityLv(guildBuildingType)));
            } 
            
            // 没有权限 或者 不满足升级条件则置灰按钮
            btnLvUp.SafeSetGray(!GuildDataManager.GetInstance().HasPermission(EGuildPermission.UpgradeBuilding) || !CanLvUpBuildingByType(guildBuildingType));

            btnLvUp.CustomActive(bShowLvUpBtn);
            maxLv.CustomActive(!bShowLvUpBtn);
            mainLvLimit.CustomActive(bNeedMainLvLimit);
            process.CustomActive(bShowLvUpBtn);
            if(bBuildingUnLocked == false)
            {
                process.CustomActive(false);
                mainLvLimit.CustomActive(false);
                unlockNeedMainLv.CustomActive(true);
                btnLvUp.CustomActive(false);
            }
            else
            {
                unlockNeedMainLv.CustomActive(false);
            }

            return;
        }
    }
}


