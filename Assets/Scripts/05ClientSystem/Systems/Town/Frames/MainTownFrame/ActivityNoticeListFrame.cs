using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    class ActivityNoticeListFrame : ClientFrame
    {
        string ActivityListElePath = "UIFlatten/Prefabs/MainFrameTown/ActivityNoticeEle";

        List<GameObject> EleObjList = new List<GameObject>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MainFrameTown/ActivityNoticeListFrame";
        }

        protected override void _OnOpenFrame()
        {
            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        void ClearData()
        {
            for(int i = 0; i < EleObjList.Count; i++)
            {
                if(EleObjList[i] == null)
                {
                    continue;
                }

                ComCommonBind bind = EleObjList[i].GetComponent<ComCommonBind>();
                if(bind == null)
                {
                    continue;
                }

                GameObject BtGo = bind.GetGameObject("BtGo");
                BtGo.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            EleObjList.Clear();
        }

        void OnGo(int index)
        {
            List<NotifyInfo> ActivityNoticeDataList = ActivityNoticeDataManager.GetInstance().GetActivityNoticeDataList();

            if (index < 0 || index >= ActivityNoticeDataList.Count)
            {
                return;
            }

            NotifyInfo data = ActivityNoticeDataList[index];

            if (data.type == (uint)NotifyType.NT_GUILD_BATTLE)
            {
                Utility.EnterGuildBattle();
            }
            else if(data.type == (uint)NotifyType.NT_GUILD_DUNGEON)
            {
                ClientSystemManager.GetInstance().OpenFrame<JoinGuildDungeonActivityFrame>();
            }
            else if(data.type == (uint)NotifyType.NT_BUDO)
            {
                BudoManager.GetInstance().TryBeginActive();
            }
            else if (data.type == (uint)NotifyType.NT_MONEY_REWARDS)
            {
                MoneyRewardsEnterFrame.CommandOpen(null);
            }
            else if(data.type == (uint)NotifyType.NT_JAR_OPEN)
            {
                ActivityJarData actJarData = new ActivityJarData();
                actJarData.nActivityID = (int)data.param;
                if (ClientSystemManager.GetInstance().IsFrameOpen<ActivityJarFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<ActivityJarFrame>();
                }
                ClientSystemManager.GetInstance().OpenFrame<ActivityJarFrame>(FrameLayer.Middle, actJarData);
                ActivityNoticeDataManager.GetInstance().DeleteActivityNotice(data);
            }
            else if (data.type == (uint)NotifyType.NT_JAR_SALE_RESET)
            {
                ActivityJarData actJarData = new ActivityJarData();
                actJarData.nActivityID = (int)data.param;
                if (ClientSystemManager.GetInstance().IsFrameOpen<ActivityJarFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<ActivityJarFrame>();
                }
                ClientSystemManager.GetInstance().OpenFrame<ActivityJarFrame>(FrameLayer.Middle, actJarData);
                ActivityNoticeDataManager.GetInstance().DeleteActivityNotice(data);
            }
            else if (data.type == (uint)NotifyType.NT_MAGIC_INTEGRAL_EMPTYING)
            {
               
                if (ClientSystemManager.GetInstance().IsFrameOpen<PocketJarFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<PocketJarFrame>();
                }
                ClientSystemManager.GetInstance().OpenFrame<PocketJarFrame>(FrameLayer.Middle);
                ActivityNoticeDataManager.GetInstance().DeleteActivityNotice(data);
            }
            else if (data.type == (uint)NotifyType.NT_MONTH_CARD_REWARD_EXPIRE_24H)
            {
                GameClient.ActiveManager.GetInstance().OpenActiveFrame(MonthCardRewardLockersDataManager.FULI_ACTIVITY_TYPE_ID, 
                                                                       MonthCardRewardLockersDataManager.FULI_ACTIVITY_TEMPLATE_TABLE_ID);
                ActivityNoticeDataManager.GetInstance().DeleteActivityNotice(data);
            }

            frameMgr.CloseFrame(this);
        }

        void InitInterface()
        {
            UpdateEleObjList();
        }

        void UpdateEleObjList()
        {
            List<NotifyInfo> ActivityNoticeDataList = ActivityNoticeDataManager.GetInstance().GetActivityNoticeDataList();

            //武道大会放在最前面
            for(int i = 0;i < ActivityNoticeDataList.Count;i++)
            {
                if(ActivityNoticeDataList[i].type == (uint)NotifyType.NT_MONEY_REWARDS)
                {
                    var tempData = ActivityNoticeDataList[0];
                    ActivityNoticeDataList[0] = ActivityNoticeDataList[i];
                    ActivityNoticeDataList[i] = tempData;
                }
            }
            if(ActivityNoticeDataList.Count > EleObjList.Count)
            {
                int iDiff = ActivityNoticeDataList.Count - EleObjList.Count;

                for (int i = 0; i < iDiff; i++)
                {
                    GameObject EleObj = AssetLoader.instance.LoadResAsGameObject(ActivityListElePath);
                    if (EleObj == null)
                    {
                        continue;
                    }

                    Utility.AttachTo(EleObj, mEleRoot);

                    EleObjList.Add(EleObj);
                }
            }

            for(int i = 0; i < EleObjList.Count; i++)
            {
                if(i < ActivityNoticeDataList.Count)
                {
                    ComCommonBind commonbind = EleObjList[i].GetComponent<ComCommonBind>();
                    if(commonbind == null)
                    {
                        EleObjList[i].SetActive(false);
                        continue;
                    }

                    // Sprite spr = null;
                    string spr = null;
                    GameObject icon = commonbind.GetGameObject("Icon");
                    GameObject des = commonbind.GetGameObject("Des");

                    if (ActivityNoticeDataList[i].type == (uint)NotifyType.NT_GUILD_BATTLE)
                    {
                        // spr = AssetLoader.instance.LoadRes("UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Gonghui", typeof(Sprite)).obj as Sprite;
                        spr = "UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Gonghui";
                        des.GetComponent<Text>().text = TR.Value("Activity_Guild_tip");
                    }
                    if (ActivityNoticeDataList[i].type == (uint)NotifyType.NT_GUILD_DUNGEON)
                    {
                        spr = "UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Gonghui";
                        des.GetComponent<Text>().text = TR.Value("Activity_Guild_Dungeon_tip");
                    }
                    else if(ActivityNoticeDataList[i].type == (uint)NotifyType.NT_BUDO)
                    {
                        // spr = AssetLoader.instance.LoadRes("UI/Image/Packed/p_MainUI01.png:UI_MainUI_Shousuo_Icon_Juedou", typeof(Sprite)).obj as Sprite;
                        spr = "UI/Image/Packed/p_MainUI01.png:UI_MainUI_Shousuo_Icon_Juedou";
                        des.GetComponent<Text>().text = TR.Value("Activity_Budo_tip");
                    }
                    else if (ActivityNoticeDataList[i].type == (uint)NotifyType.NT_MONEY_REWARDS)
                    {
                        //spr = AssetLoader.instance.LoadRes("UI/Image/Packed/p_MainUI01.png:UI_MainUI_Shousuo_Icon_Juedou", typeof(Sprite)).obj as Sprite;
                        spr = "UI/Image/Packed/p_MainUI01.png:UI_MainUI_Shousuo_Icon_Juedou";
                        des.GetComponent<Text>().text = TR.Value("Activity_MoneyRewards_tip");
                    }
                    else if (ActivityNoticeDataList[i].type == (uint)NotifyType.NT_JAR_OPEN)
                    {
                        ActivityJarTable table = TableManager.GetInstance().GetTableItem<ActivityJarTable>((int)ActivityNoticeDataList[i].param);
                        if (table != null)
                        {
                            JarData jarData = JarDataManager.GetInstance().GetJarData(table.JarID);
                            if (jarData != null)
                            {
                                // spr = AssetLoader.instance.LoadRes("UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Moguan", typeof(Sprite)).obj as Sprite;
                                spr = "UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Moguan";
                                des.GetComponent<Text>().text = TR.Value("notice_jar_open", jarData.strName);
                            }
                        }
                    }
                    else if (ActivityNoticeDataList[i].type == (uint)NotifyType.NT_JAR_SALE_RESET)
                    {
                        ActivityJarTable table = TableManager.GetInstance().GetTableItem<ActivityJarTable>((int)ActivityNoticeDataList[i].param);
                        if (table != null)
                        {
                            JarData jarData = JarDataManager.GetInstance().GetJarData(table.JarID);
                            if (jarData != null)
                            {
                                // spr = AssetLoader.instance.LoadRes("UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Moguan", typeof(Sprite)).obj as Sprite;
                                spr = "UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Moguan";
                                des.GetComponent<Text>().text = TR.Value("notice_jar_sale_reset", jarData.strName);
                            }
                        }
                    }
                    else if (ActivityNoticeDataList[i].type == (uint)NotifyType.NT_MAGIC_INTEGRAL_EMPTYING)
                    {

                        // spr = AssetLoader.instance.LoadRes("UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Moguan", typeof(Sprite)).obj as Sprite;
                        spr = "UI/Image/Packed/p_MainUIIcon.png:UI_MainUI_Tubiao_Moguan";
                        des.GetComponent<Text>().text = TR.Value("notice_mogicjar_integral_emptying");

                    }
                    else if (ActivityNoticeDataList[i].type == (uint)NotifyType.NT_MONTH_CARD_REWARD_EXPIRE_24H)
                    {
                        spr = "UI/Image/Packed/p_UI_Fuli.png:UI_Fuli_JiangLiZanCunXiang";
                        des.GetComponent<Text>().text = TR.Value("notice_month_card_high_grade_expire_24h");
                    }

                    if (spr != null)
                    {
                        // icon.GetComponent<Image>().sprite = spr;
                        Image iconImage = icon.GetComponent<Image>();
                        if (iconImage)
                        {
                            ETCImageLoader.LoadSprite(ref iconImage, spr);

                            //特殊尺寸ICON
                            if (ActivityNoticeDataList[i].type == (uint)NotifyType.NT_MONTH_CARD_REWARD_EXPIRE_24H)
                            {
                                iconImage.SetNativeSize();
                            }
                        }
                    }

                    GameObject go = commonbind.GetGameObject("BtGo");

                    Button btGo = go.GetComponent<Button>();
                    btGo.onClick.RemoveAllListeners();

                    int iIndex = i;
                    btGo.onClick.AddListener(() => { OnGo(iIndex); });

                    EleObjList[i].SetActive(true);
                }
                else
                {
                    EleObjList[i].SetActive(false);
                }
            }
        }

        #region ExtraUIBind
        private GameObject mEleRoot = null;
        private Button mBtClose = null;

        protected override void _bindExUI()
        {
            mEleRoot = mBind.GetGameObject("EleRoot");
            mBtClose = mBind.GetCom<Button>("BtClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mEleRoot = null;
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
