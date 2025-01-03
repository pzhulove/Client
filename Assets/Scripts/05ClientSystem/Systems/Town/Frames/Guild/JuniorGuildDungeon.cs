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
    public class JuniorGuildDungeon : MonoBehaviour
    {
        [SerializeField]
        Text txtBossName = null;

        [SerializeField]
        Text txtDungeonLv = null;

        [SerializeField]
        Text txtKillCount = null;

        [SerializeField]
        Button btnBk = null;

        [SerializeField]
        Image imgIcon = null;

        [SerializeField]
        Button btnHowToPlay = null;
        ulong nDungeonID = 0;

        // Use this for initialization
        void Start()
        {
            if(btnBk != null)
            {
                btnBk.onClick.RemoveAllListeners();
                btnBk.onClick.AddListener(() => 
                {
                    GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
                    if (data != null && data.nBossOddHp == 0)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildDungeonBossHaveKilledCanNotEnter"));
                        return;
                    }

                    ChapterBaseFrame.sDungeonID = (int)nDungeonID;
                    ClientSystemManager.instance.OpenFrame<GuildDungeonCityInfoFrame>();
                });
            }
            btnHowToPlay.SafeRemoveAllListener();
            btnHowToPlay.SafeAddOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildDungeonHowToPlayFrame>(FrameLayer.Middle, nDungeonID);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetUp(object data)
        {
            GuildDataManager.JuniorDungeonDamageInfo damageInfo = data as GuildDataManager.JuniorDungeonDamageInfo;
            if (damageInfo == null)
            {
                return;
            }

            nDungeonID = damageInfo.nDungeonID;

            if (txtBossName != null)
            {
                txtBossName.text = GuildDataManager.GetInstance().GetGuildDungeonName((uint)damageInfo.nDungeonID);
            }

            if (txtDungeonLv != null)
            {
                txtDungeonLv.text = "Lv. " + GuildDataManager.GetInstance().GetGuildDungeonLv((uint)damageInfo.nDungeonID).ToString();
            }

            if (txtKillCount != null)
            {
                txtKillCount.text = string.Format("通关次数 {0}", damageInfo.nKillCount);
            }

            if(imgIcon != null)
            {
                ETCImageLoader.LoadSprite(ref imgIcon, GuildDataManager.GetInstance().GetGuildDungeonIconPath((uint)nDungeonID));
            }

            GuildDataManager.GuildDungeonActivityData activityData = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if(activityData != null)
            {
                if (imgIcon != null)
                {
                    UIGray gray = imgIcon.gameObject.SafeAddComponent<UIGray>(false);
                    if (gray != null)
                    {
                        gray.enabled = activityData.nBossOddHp == 0;
                        gray.SetEnable(activityData.nBossOddHp == 0);
                    }
                }
            }            
        }
    }
}


