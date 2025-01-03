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
    public class BossGuildDungeon : MonoBehaviour
    {
        [SerializeField]
        Text txtBossName = null;

        [SerializeField]
        Text txtDungeonLv = null;

        [SerializeField]
        Button btnBk = null;

        [SerializeField]
        Image imgIcon = null;

        [SerializeField]
        Text txtLockTip = null;

        [SerializeField]
        Image imgLock = null;

        [SerializeField]
        Slider process = null;

        [SerializeField]
        Text txtKillInfo = null;

        [SerializeField]
        Image imgOpenFlag0 = null;

        [SerializeField]
        Image imgOpenFlag1 = null;

        [SerializeField]
        Image imgOpenFlag2 = null;

        [SerializeField]
        Image imgKilled = null;

        [SerializeField]
        Image imgVerifyBlood = null;
        [SerializeField]
        Button btnHowToPlay = null;        
        ulong nDungeonID = 0;

        const int nMediumDungeonNum = 3;

        string[] openFlagStrs = new string[nMediumDungeonNum] 
        {
            "UI/Image/Packed/p_UI_Gonghuifuben.png:UI_Gonghuifuben_Baoshi03",
            "UI/Image/Packed/p_UI_Gonghuifuben.png:UI_Gonghuifuben_Baoshi02",
            "UI/Image/Packed/p_UI_Gonghuifuben.png:UI_Gonghuifuben_Baoshi01",
        }; 

        // Use this for initialization
        void Start()
        {
            if (btnBk != null)
            {
                btnBk.onClick.RemoveAllListeners();
                btnBk.onClick.AddListener(() =>
                {                    
                    //nDungeonID = 101000;

                    if(!GuildDataManager.GetInstance().IsGuildDungeonOpen((int)nDungeonID))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildDungeonNotOpenNow"));
                        return;
                    }

                    GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
                    if(data != null && data.nBossOddHp == 0)
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

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateActivityData);
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonUpdateActivityData, _OnUpdateActivityData);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetUp(object data)
        {
            GuildDataManager.BossDungeonDamageInfo damageInfo = data as GuildDataManager.BossDungeonDamageInfo;
            if (damageInfo == null)
            {
                return;
            }

            nDungeonID = damageInfo.nDungeonID;

            if(txtBossName != null)
            {
                txtBossName.text = GuildDataManager.GetInstance().GetGuildDungeonName((uint)damageInfo.nDungeonID);
            }

            if(txtDungeonLv != null)
            {
                txtDungeonLv.text = "Lv. " + GuildDataManager.GetInstance().GetGuildDungeonLv((uint)damageInfo.nDungeonID).ToString();
            }

            if (imgIcon != null)
            {
                ETCImageLoader.LoadSprite(ref imgIcon, GuildDataManager.GetInstance().GetGuildDungeonIconPath((uint)nDungeonID));
            }

            if(txtLockTip != null)
            {
                txtLockTip.CustomActive(!GuildDataManager.GetInstance().IsGuildDungeonOpen((int)nDungeonID));
            }

            if(imgLock != null)
            {
                imgLock.CustomActive(!GuildDataManager.GetInstance().IsGuildDungeonOpen((int)nDungeonID));
            }

            if(process != null)
            {
                process.CustomActive(GuildDataManager.GetInstance().IsGuildDungeonOpen((int)nDungeonID));
            }

            UpdateOpenFlagImgs();

            UpdateKillInfo();
        }

        private void _OnUpdateActivityData(UIEvent uiEvent)
        {
            UpdateKillInfo();
        }

        private void SafeLoadImage(ref Image img,string path)
        {
            if(img == null || path == null)
            {
                return;
            }

            ETCImageLoader.LoadSprite(ref img, path);
            return;
        }

        void UpdateOpenFlagImgs()
        {
            Image[] imgOpenFlags = new Image[nMediumDungeonNum] { imgOpenFlag0, imgOpenFlag1, imgOpenFlag2 };
            if(imgOpenFlags == null)
            {
                return;
            }

            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            int iIndex = 0;
            if (data != null && data.mediumDungeonDamgeInfos != null)
            {
                for(int i = 0;i < data.mediumDungeonDamgeInfos.Count;i++)
                {
                    GuildDataManager.MediumDungeonDamageInfo damageInfo = data.mediumDungeonDamgeInfos[i];
                    if(damageInfo != null && damageInfo.nOddHp == 0)
                    {
                        imgOpenFlags[i].CustomActive(true);

                        if (i < imgOpenFlags.Length && i < openFlagStrs.Length)
                        {
                            SafeLoadImage(ref imgOpenFlags[i], openFlagStrs[i]);
                        }
                    }
                    else
                    {
                        imgOpenFlags[i].CustomActive(false);
                    }
                }
            }
        }

        void UpdateKillInfo()
        {
            GuildDataManager.GuildDungeonActivityData data = GuildDataManager.GetInstance().GetGuildDungeonActivityData();
            if (data != null)
            {
                if (txtKillInfo != null)
                {
                    txtKillInfo.text = string.Format("{0}/{1}", data.nBossOddHp, data.nBossMaxHp);
                }

                if (process != null && data.nBossMaxHp > 0)
                {
                    process.value = (float)data.nBossOddHp / (float)data.nBossMaxHp;
                }

                if(imgKilled != null)
                {
                    imgKilled.CustomActive(data.nBossOddHp == 0);
                }

                if(imgVerifyBlood != null && data.nBossMaxHp > 0)
                {
                    imgVerifyBlood.fillAmount = (float)data.nVerifyBlood / (float)data.nBossMaxHp;
                }
                if(imgIcon != null)
                {
                    UIGray gray = imgIcon.gameObject.SafeAddComponent<UIGray>(false);
                    if(gray != null)
                    {
                        //gray.enabled = false;
                        gray.enabled = data.nBossOddHp == 0;
                        gray.SetEnable(data.nBossOddHp == 0);
                    }
                }
            }
        }
    }
}


