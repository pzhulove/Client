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
    public class MediumGuildDungeonMini : MonoBehaviour
    {
        [SerializeField]
        Image icon = null;

        [SerializeField]
        Text txtBossName = null;        

        [SerializeField]
        Text txtKillHpInfo = null;

        [SerializeField]
        Slider sldProcess = null;       

        [SerializeField]
        Image imgKilled = null;

        [SerializeField]
        Image imgVerifyBlood = null;

        ulong nDungeonID = 0;

        // Use this for initialization
        void Start()
        {
            
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

            if (txtKillHpInfo != null)
            {
                txtKillHpInfo.text = string.Format("{0} / {1}", GetColorString(damageInfo.nOddHp.ToString(), "ffd689ff"), damageInfo.nMaxHp);
            }

            if(sldProcess != null)
            {
                if(damageInfo.nMaxHp > 0)
                {
                    sldProcess.value = (float)damageInfo.nOddHp / (float)damageInfo.nMaxHp;
                }                
            }

            if (imgVerifyBlood != null && damageInfo.nMaxHp > 0)
            {
                imgVerifyBlood.fillAmount = (float)damageInfo.nVerifyBlood / (float)damageInfo.nMaxHp;                
            }

            icon.SafeSetImage(GuildDataManager.GetInstance().GetGuildDungeonMiniIconPath((uint)damageInfo.nDungeonID));
        }
    }
}


