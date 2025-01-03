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
    public class GuildDungeonBuffAddUpInfoItem : MonoBehaviour
    {
        [SerializeField]
        Text txtBossName = null;

        [SerializeField]
        Text txtDungeonLv = null;

        [SerializeField]
        Image imgIcon = null;

        [SerializeField]
        Text txtAddUpInfo = null;

        [SerializeField]
        Text txtKillCountInfo = null;

        [SerializeField]
        Text txt100Place = null;

        [SerializeField]
        Text txt10Place = null;

        [SerializeField]
        Text txt1Place = null;

        ulong nDungeonID = 0;

        // Use this for initialization
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetUp(object data)
        {
            GuildDataManager.BuffAddUpInfo addUpInfo = data as GuildDataManager.BuffAddUpInfo;
            if (addUpInfo == null)
            {
                return;
            }

            nDungeonID = addUpInfo.nDungeonID;
            if(txtBossName != null)
            {
                txtBossName.text = GuildDataManager.GetInstance().GetGuildDungeonName((uint)nDungeonID);
            }

            if(txtDungeonLv != null)
            {
                txtDungeonLv.text = "Lv." + GuildDataManager.GetInstance().GetGuildDungeonLv((uint)nDungeonID).ToString();
            }

            if(txtAddUpInfo != null)
            {
                txtAddUpInfo.text = GuildDataManager.GetBuffAddUpInfo((int)addUpInfo.nBuffID, (int)addUpInfo.nBuffLv);
            }

            GuildDataManager.GuildDungeonActivityData activityData = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (activityData != null && activityData.juniorDungeonDamgeInfos != null)
            {
                for(int i = 0;i < activityData.juniorDungeonDamgeInfos.Count;i++)
                {
                    if(activityData.juniorDungeonDamgeInfos[i].nDungeonID == nDungeonID)
                    {
                        if (txtKillCountInfo != null)
                        {
                            txtKillCountInfo.text = string.Format("通关次数 {0}", activityData.juniorDungeonDamgeInfos[i].nKillCount);
                        }

                        uint n = (uint)activityData.juniorDungeonDamgeInfos[i].nKillCount;
                        uint unitPlace = (n / 1) % 10;
                        uint tenPlace = (n / 10) % 10;
                        uint hundredPlace = (n / 100) % 10;

                        if (txt100Place != null)
                        {
                            txt100Place.text = hundredPlace.ToString();
                        }

                        if(txt10Place != null)
                        {
                            txt10Place.text = tenPlace.ToString();
                        }

                        if(txt1Place != null)
                        {
                            txt1Place.text = unitPlace.ToString();
                        }

                        break;
                    }
                }
            }

            if (imgIcon != null)
            {
                ETCImageLoader.LoadSprite(ref imgIcon, GuildDataManager.GetInstance().GetGuildDungeonIconPath((uint)nDungeonID));
            }
        }
    }
}


