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

    public class GuildDungeonBossDiffSetItem : MonoBehaviour
    {
        [SerializeField]
        Image icon = null;
 
        [SerializeField]
        Image diff = null;

        [SerializeField]
        Image selected = null;       

        [SerializeField]
        ComUIListScript awardItems = null;     

        [SerializeField]
        Button btnSelect = null; 

        [SerializeField]
        Text lvLimit = null;

        [SerializeField]
        Text description1 = null;

        [SerializeField]
        Text description2 = null;

        [SerializeField]
        Text description3 = null;

        [SerializeField]
        Text description4 = null;

        [SerializeField]
        GameObject descriptionRoot = null;

        [SerializeField]
        TextEx comingSoonText = null;

        [SerializeField]
        UIGray gray = null;

        [SerializeField]
        GameObject effuiSelect = null;

        List<AwardItemData> awardItemDataList = null;
        ClientFrame clientFrame = null;

        Dictionary<int, string> diffType2IconPath = null;
        Dictionary<int, string> diffType2NamePath = null;

        // Use this for initialization
        void Start()
        {
            
        }

        private void Awake()
        {
            diffType2IconPath = new Dictionary<int, string>();
            if (diffType2IconPath != null)
            {
                diffType2IconPath[1] = "UI/Image/Icon/Icon_Gonghui/Icon_Gonghui_Nandu_01.png";
                diffType2IconPath[2] = "UI/Image/Icon/Icon_Gonghui/Icon_Gonghui_Nandu_02.png";
                diffType2IconPath[3] = "UI/Image/Icon/Icon_Gonghui/Icon_Gonghui_Nandu_03.png";
                diffType2IconPath[4] = "UI/Image/Icon/Icon_Gonghui/Icon_Gonghui_Nandu_04.png";
            }

            diffType2NamePath = new Dictionary<int, string>();
            if (diffType2NamePath != null)
            {
                diffType2NamePath[1] = "UI/Image/NewPacked/Gonghui_common.png:Gonghui_Img_Bossshezhi_01";
                diffType2NamePath[2] = "UI/Image/NewPacked/Gonghui_common.png:Gonghui_Img_Bossshezhi_02";
                diffType2NamePath[3] = "UI/Image/NewPacked/Gonghui_common.png:Gonghui_Img_Bossshezhi_03";

            }
        }

        private void OnDestroy()
        {
            diffType2IconPath = null;
            diffType2NamePath = null;
        }

        // Update is called once per frame
        void Update()
        {
           
        }
        
        string GetIconPath(int iDiffType)
        {
            if(diffType2IconPath != null && diffType2IconPath.ContainsKey(iDiffType))
            {
                return diffType2IconPath[iDiffType];
            }

            return "";
        }

        string GetDiffPath(int iDiffType)
        {
            if (diffType2NamePath != null && diffType2NamePath.ContainsKey(iDiffType))
            {
                return diffType2NamePath[iDiffType];
            }

            return "";
        }

        void InitAwardItems()
        {
            if (awardItems == null)
            {
                return;
            }

            awardItems.Initialize();

            awardItems.onBindItem = (GameObject go) =>
            {
                PayRewardItem payItem = null;
                if (go)
                {
                    payItem = go.GetComponent<PayRewardItem>();
                }

                return payItem;
            };

            awardItems.onItemVisiable = (var1) =>
            {
                if (var1 == null)
                {
                    return;
                }

                int iIndex = var1.m_index;
                if (iIndex >= 0 && awardItemDataList != null && iIndex < awardItemDataList.Count)
                {
                    ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(awardItemDataList[iIndex].ID);
                    if (itemDetailData == null)
                    {
                        Logger.LogErrorFormat("GuildDungeonBossDiffSetItem Can not find item id in item table!!! Please Check item data id {0} !!!", awardItemDataList[iIndex].ID);
                        return;
                    }

                    itemDetailData.Count = awardItemDataList[iIndex].Num;
                    PayRewardItem payItem = var1.gameObjectBindScript as PayRewardItem;
                    if (payItem != null)
                    {
                        payItem.Initialize(clientFrame, itemDetailData, true, false);
                        payItem.RefreshView();
                    }
                }
            };
        }

        void CalAwardItemList(int iDiffType)
        {
            awardItemDataList = new List<AwardItemData>();
            if (awardItemDataList == null)
            {
                return;
            }

            GuildDungeonTypeTable dungeonTypeTable = TableManager.GetInstance().GetTableItem<GuildDungeonTypeTable>(iDiffType);
            if (dungeonTypeTable == null)
            {
                return;
            }

            for (int i = 0; i < dungeonTypeTable.rewardItemLength; i++)
            {
                string strReward = dungeonTypeTable.rewardItemArray(i);
                string[] reward = strReward.Split('_');
                if (reward.Length >= 2)
                {
                    AwardItemData data = new AwardItemData();
                    int.TryParse(reward[0], out data.ID);
                    int.TryParse(reward[1], out data.Num);
                    awardItemDataList.Add(data);
                }
            }

            return;
        }

        void UpdateAwardItems(int iDiffType)
        {
            GuildDungeonTypeTable dungeonTypeTable = TableManager.GetInstance().GetTableItem<GuildDungeonTypeTable>(iDiffType);
            if (dungeonTypeTable == null)
            {
                return;
            }

            if (awardItems == null)
            {
                return;
            }

            CalAwardItemList(iDiffType);

            if (awardItemDataList != null)
            {
                awardItems.SetElementAmount(awardItemDataList.Count);
                if(awardItems.m_scrollRect != null)
                {
                    awardItems.m_scrollRect.horizontalNormalizedPosition = 0.0f;
                }
            }
        }

        string GetBossDiffStr(int iDiffType)
        {
            string[] diffStr = new string[] {
                TR.Value("guild_build_boss_lv_set_diff_putong"),
                TR.Value("guild_build_boss_lv_set_diff_maoxian"),
                TR.Value("guild_build_boss_lv_set_diff_yongshi"),
                TR.Value("guild_build_boss_lv_set_diff_wangzhe"), };
            if(diffStr == null)
            {
                return "";
            }

            if(iDiffType >= 1 && iDiffType <= diffStr.Length)
            {
                return diffStr[iDiffType - 1];
            }

            return "";
        }

        public void SetUp(object data,ClientFrame frame)
        {
            if(data == null)
            {
                return;
            }

            if(frame == null)
            {
                return;
            }

            if(!(data is int))
            {
                return;
            }

            int iDiffType = (int)data;
            GuildDungeonTypeTable dungeonTypeTable = TableManager.GetInstance().GetTableItem<GuildDungeonTypeTable>(iDiffType);
            if(dungeonTypeTable == null)
            {
                return;
            }

            clientFrame = frame;            

            icon.SafeSetImage(GetIconPath(iDiffType));
            diff.SafeSetImage(GetDiffPath(iDiffType));
            selected.CustomActive(iDiffType == GuildDataManager.GetInstance().GetGuildDungeonDiffType());

            InitAwardItems();
            UpdateAwardItems(iDiffType);
      
            btnSelect.SafeSetOnClickListener(() => 
            {
                int iDiffTemp = iDiffType;
                int iOldDiff = GuildDataManager.GetInstance().GetGuildDungeonDiffType();
                SystemNotifyManager.SysNotifyMsgBoxCancelOk(TR.Value("guild_build_boss_lv_set_diff_msg_box",GetBossDiffStr(iOldDiff),GetBossDiffStr(iDiffTemp)), null, () => 
                {
                    GuildDataManager.GetInstance().SendWorldGuildSetDungeonTypeReq((uint)iDiffTemp);
                });                
            });


            btnSelect.SafeSetGray(!GuildDataManager.GetInstance().HasPermission(EGuildPermission.SetGuildDungeonBossDiff));

            btnSelect.CustomActive(iDiffType != GuildDataManager.GetInstance().GetGuildDungeonDiffType());

            description1.SafeSetText(TR.Value("guild_build_boss_lv_set_challenge_time", dungeonTypeTable.challengeTime));
            description2.SafeSetText(TR.Value("guild_build_boss_lv_set_recommend_lv", dungeonTypeTable.recommendLv));
            description3.SafeSetText(TR.Value("guild_build_boss_lv_set_recommend_equip", dungeonTypeTable.recommendEquip));
            description4.SafeSetText(TR.Value("guild_build_boss_lv_set_recommend_player_num", dungeonTypeTable.recommendPlayerNum));

            //descriptionRoot.CustomActive(iDiffType == GuildDataManager.GetInstance().GetGuildDungeonDiffType());

            lvLimit.SafeSetText(TR.Value("guild_build_boss_lv_set_unlock_by_fete_lv", dungeonTypeTable.buildLvl));

            bool bTypeNotOpen = dungeonTypeTable.buildLvl > GuildDataManager.currentMaxBuildLv; // 该难度是否 未开放           

            // 难度是否解锁
            bool isTypeUnLocked = GuildDataManager.GetInstance().GetBuildingLevel(GuildBuildingType.FETE) >= dungeonTypeTable.buildLvl;
            if(isTypeUnLocked == false || bTypeNotOpen)
            {
                //descriptionRoot.CustomActive(false);
                btnSelect.CustomActive(false);
            }          

            if(bTypeNotOpen) // 难度未开放则隐藏建筑等级限制，显示 敬请期待
            {
                lvLimit.CustomActive(false); 
                comingSoonText.CustomActive(true);
            }
            else // 难度开放了则隐藏 敬请期待 建筑等级限制根据是否解锁来显示或者隐藏
            {
                lvLimit.CustomActive(!isTypeUnLocked);
                comingSoonText.CustomActive(false);
            }

            if (gray != null)
            {
                gray.SetEnable(!bTypeNotOpen);
            }
            effuiSelect.CustomActive(iDiffType == GuildDataManager.GetInstance().GetGuildDungeonDiffType());
            return;
        }
    }
}


