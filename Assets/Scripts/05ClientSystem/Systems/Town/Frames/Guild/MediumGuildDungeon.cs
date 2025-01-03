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
    public class MediumGuildDungeon : MonoBehaviour
    {
        [SerializeField]
        Text txtBossName = null;

        [SerializeField]
        Text txtDungeonLv = null;

        [SerializeField]
        Text txtKillHpInfo = null;

        [SerializeField]
        Slider sldProcess = null;

        [SerializeField]
        Button btnBk = null;

        [SerializeField]
        Image imgIcon = null;

        [SerializeField]
        Text txtCurHp = null;

        [SerializeField]
        Text txtMaxHp = null;

        [SerializeField]
        Image imgKilled = null;

        [SerializeField]
        Button btnHowToPlay = null;
        [SerializeField]
        Image imgVerifyBlood = null;
        ulong nDungeonID = 0;
        float imgVerifyBloodInitWidth = 0;

        // Use this for initialization
        void Start()
        {
            imgVerifyBloodInitWidth = 0;
            if (btnBk != null)
            {
                btnBk.onClick.RemoveAllListeners();
                btnBk.onClick.AddListener(() =>
                {
                    GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
                    if (data != null && data.mediumDungeonDamgeInfos != null)
                    {
                        for(int i = 0;i < data.mediumDungeonDamgeInfos.Count;i++)
                        {
                            if(data.mediumDungeonDamgeInfos[i].nDungeonID == nDungeonID && data.mediumDungeonDamgeInfos[i].nOddHp == 0)
                            {
                                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildDungeonMonsterHaveKilledCanNotEnter"));
                                return;
                            }
                        }                       
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
            if(imgVerifyBlood != null)
            {
                imgVerifyBloodInitWidth = imgVerifyBlood.rectTransform.sizeDelta.x;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private string GetColorString(string text,string color)
        {
            return TR.Value("common_color_text", "#" + color, text);
        }

        public void SetUp(object data)
        {
            GuildDataManager.MediumDungeonDamageInfo damageInfo = data as GuildDataManager.MediumDungeonDamageInfo;
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

            if (txtKillHpInfo != null)
            {
                txtKillHpInfo.text = string.Format("{0} / {1}", GetColorString(damageInfo.nOddHp.ToString(), "dc5d5dFF"), damageInfo.nMaxHp);
            }

            if(sldProcess != null)
            {
                if(damageInfo.nMaxHp > 0)
                {
                    sldProcess.value = (float)damageInfo.nOddHp / (float)damageInfo.nMaxHp;
                }                
            }

            if (imgIcon != null)
            {
                ETCImageLoader.LoadSprite(ref imgIcon, GuildDataManager.GetInstance().GetGuildDungeonIconPath((uint)nDungeonID));
            }

            if(imgKilled != null)
            {
                imgKilled.CustomActive(damageInfo.nOddHp == 0);                
            }

            if (imgVerifyBlood != null && damageInfo.nMaxHp > 0)
            {
                imgVerifyBlood.CustomActive(true);
                Vector2 vec = imgVerifyBlood.rectTransform.sizeDelta;
                vec.x = imgVerifyBloodInitWidth * ((float)damageInfo.nVerifyBlood / (float)damageInfo.nMaxHp);
                imgVerifyBlood.rectTransform.sizeDelta = vec;
            }
            if(imgIcon != null)
            {
                if(damageInfo.nOddHp == 0)
                {
                    UIGray gray = imgIcon.gameObject.SafeAddComponent<UIGray>(false);
                    if (null != gray)
                    {
                        gray.enabled = true;
                        gray.SetEnable(true);
                    }
                }
            }
        }
    }
}


