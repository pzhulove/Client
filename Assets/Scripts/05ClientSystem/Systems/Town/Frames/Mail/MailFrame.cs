using System;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class MailFrame : WndFrame
    {
        const string WndRoot = "Content/MailPanel(Clone)/";
        string RootObjPath = WndRoot + "MailList/Scroll View/Viewport/Content";
        string RwardRootObjPath = WndRoot + "RewardsMailList/Scroll View/Viewport/Content";
        string RewardItemRootPath = WndRoot + "MainInfomation/infomationback/Items";
        string MailItemPath = "UIFlatten/Prefabs/Mail/MailItem";
        string MailUnReadPath = "UI/Image/Packed/p_UI_Mail.png:UI_Youjian_Tubiao_Youjianguan";
        string MailReadPath = "UI/Image/Packed/p_UI_Mail.png:UI_Youjian_Tubiao_YoujianKai";

        
        int ItemsNum = 0;
        double MailLastsTime = 30 * 24 * 60 * 60;

        const int BaseTabNum = 2;


        List<GameObject> MailItemObjList = new List<GameObject>();
        List<GameObject> RewardMailItemObjList = new List<GameObject>();
        List<ComItem> Items = new List<ComItem>();
        List<ItemData> ShowTipItemData = new List<ItemData>();

        int CreatMailObjNum = 0;
        int CreatRewardMailObjNum = 0;
        UInt64 CurSelMailID = 0;
        bool CreateFinish = false;
        float fUpdateInterval = 0f;

        [UIControl(WndRoot, typeof(StateController))]
        StateController mStateContrl;
        string mShow = "show";
        string mHide = "hide";

        protected override void _OnOpenFrame()
        {
            CreateFinish = false;
            fUpdateInterval = 0f;

            var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_MAIL_ITEM_MAXNUM);
            ItemsNum = SystemValueTableData.Value;

           
            UpdateOneKeyBtn();

            // 显示邮件列表信息
            CreateMailListPreferb();

            CreateRewardMailListPreferb();

            InitPackageItems();

            UpdateNoMailTip();

            CreateFinish = true;

            //if (PlayerBaseData.GetInstance().mails.mailList.Count > 0 )
            //{
            //    OnClickMail(0);
            //}

            helpBtn.gameObject.SetActive(false);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NewMailNotify, _OnUpdateMailItemList);
        }

        protected override void _OnCloseFrame()
        {
            bool bCloseNewMailFrame = true;

            if (PlayerBaseData.GetInstance().mails.OneKeyReceiveNum > 0)
            {
                bCloseNewMailFrame = false;
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NewMailNotify, _OnUpdateMailItemList);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.MailFrameClose, "NewMail", bCloseNewMailFrame);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.NewMailNotify);
           
        }

        //public override string GetPrefabPath()
        public override string GetContentPath()
        {
            return "UIFlatten/Prefabs/Mail/MailPanel";
        }

        void _OnUpdateMailItemList(UIEvent iEvent)
        {
            UpdateRewardMailListPreferb();
        }
//         [UIEventHandle("btClose")]
        void OnClose()
        {
            PlayerBaseData.GetInstance().mails.SortMailList();
            PlayerBaseData.GetInstance().mails.SortRewardMailList();
            ClearData();
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Title/Close")]
        void OnClickClose()
        {
            OnClose();
        }

        [UIEventHandle("Title/Help")]
        void OnClickHelp()
        {
            //frameMgr.OpenFrame<HelpFrame>(GameClient.FrameLayer.TopMost, _GetHelpType());
        }

        public override string GetTitle()
        {
            return "邮件";
        }

        void ClearData()
        {
            for (int i = 0; i < MailItemObjList.Count; i++)
            {
                if (MailItemObjList[i] == null)
                {
                    continue;
                }

                Button[] MailBtn = MailItemObjList[i].GetComponentsInChildren<Button>();

                for (int j = 0; j < MailBtn.Length; j++)
                {
                    if (MailBtn[j].name == "NormalBack")
                    {
                        MailBtn[j].onClick.RemoveAllListeners();
                        break;
                    }
                }
            }

            for (int i = 0; i < RewardMailItemObjList.Count; i++)
            {
                if (RewardMailItemObjList[i] == null)
                {
                    continue;
                }

                Button[] MailBtn = RewardMailItemObjList[i].GetComponentsInChildren<Button>();

                for (int j = 0; j < MailBtn.Length; j++)
                {
                    if (MailBtn[j].name == "NormalBack")
                    {
                        MailBtn[j].onClick.RemoveAllListeners();
                        break;
                    }
                }
            }

            ItemsNum = 0;
            CreatMailObjNum = 0;
            CreatRewardMailObjNum = 0;
            CurSelMailID = 0;
            CreateFinish = false;
            fUpdateInterval = 0f;

            Items.Clear();
            ShowTipItemData.Clear();
            MailItemObjList.Clear();
            RewardMailItemObjList.Clear();
        }

        void OnClickMail(List<MailTitleInfo> mailList, DictionaryView<UInt64, GameMailData.MailData> mailDics,int iIndex,string rootPath)
        {
            MailTitleInfo MailInfo = mailList[iIndex];

            if (CurSelMailID == MailInfo.id)
            {
                return;
            }

            CurSelMailID = MailInfo.id;

            if (MailInfo.status == 0)
            {
                OnSendReadMailReq(mailList,iIndex);
            }
            else
            {
                GameMailData.MailData MailDetailData = PlayerBaseData.GetInstance().mails.FindMailData(mailDics,MailInfo.id);

                if (MailDetailData == null)
                {
                    OnSendReadMailReq(mailList,iIndex);
                }
                else
                {
                    // 已经读取过的邮件直接刷新本地数据
                    UpdateMailDetail(MailDetailData);
                    UpdateSelMailLeftTime(mailList, rootPath);
                }
            }
        }

        //added by Jermaine 2017-3-6
        void OnClickTakeMail(int iIndex)
        {
            //OnClickMail(iIndex);

            //OnClickReceive();
        }

        //[UIEventHandle("bottom/BtnAccpet")]
        void OnClickReceive(List<MailTitleInfo> mailList)
        {
            MailTitleInfo CurSelMailInfo = PlayerBaseData.GetInstance().mails.FindMailTitleInfo(mailList,CurSelMailID);
            if (CurSelMailInfo == null)
            {
                SystemNotifyManager.SystemNotify(1022);
                return;
            }

            if (CurSelMailInfo.hasItem == 0)
            {
                SystemNotifyManager.SystemNotify(1021);
                return;
            }

            OnSendReceiveMailReq(false);
        }

        //[UIEventHandle(WndRoot + "bottom/BtnAcceptAll")]
        //void OnClickReceiveAll()
        //{
        //    if (PlayerBaseData.GetInstance().mails.OneKeyReceiveNum <= 0)
        //    {
        //        SystemNotifyManager.SystemNotify(1022);
        //        return;
        //    }

        //    OnSendReceiveMailReq(true);
        //}

        [UIEventHandle(WndRoot+"Rewardsbottom/BtnAcceptAll")]
        void OnAcceptAllClickReceiveAll()
        {
            if (PlayerBaseData.GetInstance().mails.OneKeyReceiveRewardNum <= 0)
            {
                SystemNotifyManager.SystemNotify(1022);
                return;
            }
            OnSendReceiveMailReq(true);
        }

        [UIEventHandle(WndRoot + "bottom/BtnDelete")]
        void OnClickDelete()
        {

            OnDeleteMail(PlayerBaseData.GetInstance().mails.mailList);
        }

        [UIEventHandle(WndRoot+ "Rewardsbottom/Buttons/BtnDelete")]
        void OnClickBtnDeleteClick()
        {
            OnDeleteMail(PlayerBaseData.GetInstance().mails.rewardMailList);
        }

        void OnDeleteMail(List<MailTitleInfo> mailList)
        {
            MailTitleInfo CurSelMailInfo = PlayerBaseData.GetInstance().mails.FindMailTitleInfo(mailList, CurSelMailID);
            if (CurSelMailInfo == null)
            {
                SystemNotifyManager.SystemNotify(1024);
                return;
            }

            if (CurSelMailInfo.hasItem == 1)
            {
                SystemNotifyManager.SystemNotify(1023);
                return;
            }

            OnSendDeleteMailReq(false, mailList);
        }


        [UIEventHandle(WndRoot + "bottom/BtnDeleteAll")]
        void OnClickDeleteAll()
        {
            if (PlayerBaseData.GetInstance().mails.OneKeyDeleteNum <= 0)
            {
                SystemNotifyManager.SystemNotify(1024);
                return;
            }

            OnSendDeleteMailReq(true, PlayerBaseData.GetInstance().mails.mailList);
        }

        [UIEventHandle(WndRoot+"Rewardsbottom/BtnDeleteAll")]
        void OnClickBtnDeleteAllCLick()
        {
            if (PlayerBaseData.GetInstance().mails.OneKeyDeleteRewardNum <= 0)
            {
                SystemNotifyManager.SystemNotify(1024);
                return;
            }
            OnSendDeleteMailReq(true, PlayerBaseData.GetInstance().mails.rewardMailList);
        }

        // 阅读邮件请求（邮件状态的返回走同步邮件状态消息）
        void OnSendReadMailReq(List<MailTitleInfo> mailList, int iIndex)
        {
            WorldReadMailReq req = new WorldReadMailReq();
            req.id = mailList[iIndex].id;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        // 领取附件请求（领取附件返回走同步邮件状态消息）
        void OnSendReceiveMailReq(bool bReceiveAll)
        {
            WorldGetMailItems req = new WorldGetMailItems();

            if (bReceiveAll)
            {
                req.type = 1;
                req.id = 0;
            }
            else
            {
                req.type = 0;
                req.id = CurSelMailID;
            }

            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        // 删除邮件请求
        void OnSendDeleteMailReq(bool bDeleteAll, List<MailTitleInfo> mailList)
        {
            WorldDeleteMail req = new WorldDeleteMail();

            if (bDeleteAll)
            {
                List<UInt64> DeleteMailList = PlayerBaseData.GetInstance().mails.GetDeleteMailList(mailList);

                req.ids = new UInt64[DeleteMailList.Count];

                for (int i = 0; i < DeleteMailList.Count; i++)
                {
                    req.ids[i] = DeleteMailList[i];
                }
            }
            else
            {
                req.ids = new UInt64[1];
                req.ids[0] = CurSelMailID;
            }

            NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);
        }

        // 同步邮件状态
        [MessageHandle(WorldSyncMailStatus.MsgID)]
        void OnWorldSyncMailStatusRes(MsgDATA msg)
        {
            WorldSyncMailStatus res = msg.decode<WorldSyncMailStatus>();

            Logger.Log("[MailSystem] Recived OnWorldSyncMailStatusRes.. \n");

            if (BaseTypes[0].isOn)
            {
                OnWorldSyncMailStatusRess(PlayerBaseData.GetInstance().mails.mailList, res, RootObjPath, PlayerBaseData.GetInstance().mails.mailDics);
            }
            else
            {
                OnWorldSyncMailStatusRess(PlayerBaseData.GetInstance().mails.rewardMailList, res, RwardRootObjPath, PlayerBaseData.GetInstance().mails.rewardMailDics);
            }
        }

        void OnWorldSyncMailStatusRess(List<MailTitleInfo> mailList, WorldSyncMailStatus res,string rootPath, DictionaryView<UInt64,GameMailData.MailData> mailDics)
        {
            bool bFind = false;
            for (int i = 0; i < mailList.Count; i++)
            {
                if (mailList[i].id != res.id)
                {
                    continue;
                }

                // 刷新图标
                string Path = rootPath + string.Format("/MailItem{0}", i);

                GameObject MailItemobj = Utility.FindGameObject(Path);
                if (MailItemobj == null)
                {
                    Logger.LogError("can't find obj : MailItemobj");
                    return;
                }

                // 同步阅读状态
                if (mailList[i].status != res.status)
                {
                    // 更新数据
                    mailList[i].status = res.status;

                    GameMailData.MailData MailDetailData = PlayerBaseData.GetInstance().mails.FindMailData(mailDics,res.id);
                    if (MailDetailData != null)
                    {
                        MailDetailData.info.status = res.status;
                    }

                    UpdateMailStatus(ref MailItemobj, mailList[i]);
                }

                // 同步物品领取状态
                if (mailList[i].hasItem != res.hasItem)
                {
                    //added by Jermain 2017-3-6
                    GameObject takeObj = Utility.FindGameObject(MailItemobj, "Take");

                    UIGray gray = takeObj.GetComponent<UIGray>();
                    if (gray == null)
                    {
                        gray = takeObj.AddComponent<UIGray>();
                    }

                    gray.enabled = true;
                    takeObj.GetComponent<Button>().interactable = false;

                    Text txt = takeObj.GetComponentInChildren<Text>();
                    txt.text = "已领取";

                    //added by mjx 2017-7-10
                    GameObject attachIcon = Utility.FindGameObject(MailItemobj, "AttachIcon");
                    attachIcon.CustomActive(false);

                    // 更新数据
                    mailList[i].hasItem = res.hasItem;

                    if (mailList == PlayerBaseData.GetInstance().mails.mailList)
                    {
                        PlayerBaseData.GetInstance().mails.UpdateOneKeyNum();
                    }
                    else
                    {
                        PlayerBaseData.GetInstance().mails.UpdateOneKeyRewardNum();
                    }
                   

                    if (res.hasItem == 0)
                    {
                        GameMailData.MailData MailDetailData = PlayerBaseData.GetInstance().mails.FindMailData(mailDics,res.id);
                        if (MailDetailData != null)
                        {
                            MailDetailData.items.Clear();
                            MailDetailData.detailItems.Clear();
                        }

                        // 刷新图标
                        if (CurSelMailID == res.id)
                        {
                            ClearItemsIcon();
                        }
                    }

                    UpdateOneKeyBtn();
                }

                bFind = true;
                break;
            }

            if (!bFind)
            {
                Logger.Log(ObjectDumper.Dump(res));
            }
        }

        // 阅读邮件请求返回详细信息
        [MessageHandle(WorldReadMailRet.MsgID)]
        void OnWorldReadMailRes(MsgDATA msg)
        {
            WorldReadMailRet res = msg.decode<WorldReadMailRet>();

            Logger.Log("[MailSystem] Recived OnWorldReadMailRet.. \n");

            if (BaseTypes[0].isOn)
            {
                OnWorldReadMailRess(PlayerBaseData.GetInstance().mails.UnreadNum, PlayerBaseData.GetInstance().mails.mailList, PlayerBaseData.GetInstance().mails.mailDics, res, RootObjPath);
            }
            else
            {
                OnWorldReadMailRess(PlayerBaseData.GetInstance().mails.UnreadRewardNum, PlayerBaseData.GetInstance().mails.rewardMailList, PlayerBaseData.GetInstance().mails.rewardMailDics, res, RwardRootObjPath);
            }
        }

        void OnWorldReadMailRess(int UnReadNum, List<MailTitleInfo> mailList, DictionaryView<UInt64,GameMailData.MailData> mailDics, WorldReadMailRet res,string rootPath)
        {
            UnReadNum -= 1;

            // 同步新读取的邮件详细信息
            GameMailData.MailData data = new GameMailData.MailData();

            data.info = PlayerBaseData.GetInstance().mails.FindMailTitleInfo(mailList,res.id);
            data.content = res.content;
            data.items = new List<ItemReward>(res.items);
            data.detailItems = res.detailItems;

            GameMailData.MailData MailDetailData = PlayerBaseData.GetInstance().mails.FindMailData(mailDics,res.id);
            if (MailDetailData == null)
            {
                mailDics.Add(res.id, data);
                //MailDetailData = PlayerBaseData.GetInstance().mails.FindMailData(mailDics,res.id);
            }

            MailDetailData = data;

            // 新读取的邮件要刷新详细信息
            UpdateMailDetail(MailDetailData);
            UpdateSelMailLeftTime(mailList, rootPath);

            Logger.Log(ObjectDumper.Dump(res));
        }

        // 删除邮件返回
        [MessageHandle(WorldNotifyDeleteMail.MsgID)]
        void OnWorldSyncDeleteMailRes(MsgDATA msg)
        {
            WorldNotifyDeleteMail res = msg.decode<WorldNotifyDeleteMail>();

            Logger.Log("[MailSystem] Recived OnWorldSyncDeleteMailRes.. \n");

            if (BaseTypes[0].isOn)
            {
                // 同步数据
                for (int i = 0; i < res.ids.Length; i++)
                {
                    PlayerBaseData.GetInstance().mails.DeleteMailTitleInfo(res.ids[i]);
                    PlayerBaseData.GetInstance().mails.DeleteMailData(res.ids[i]);
                }

                PlayerBaseData.GetInstance().mails.SortMailList();
                PlayerBaseData.GetInstance().mails.UpdateOneKeyNum();

                UpdateMailList(CreatMailObjNum,PlayerBaseData.GetInstance().mails.mailList, PlayerBaseData.GetInstance().mails.mailDics,RootObjPath);

                UpdateChoosenBaseType(0);
            }
            else
            {
                // 同步数据
                for (int i = 0; i < res.ids.Length; i++)
                {
                    PlayerBaseData.GetInstance().mails.DeleteRewardMailTitleInfo(res.ids[i]);
                    PlayerBaseData.GetInstance().mails.DeleteRewardMailData(res.ids[i]);
                }

                PlayerBaseData.GetInstance().mails.SortRewardMailList();
                PlayerBaseData.GetInstance().mails.UpdateOneKeyRewardNum();

                UpdateMailList(CreatRewardMailObjNum, PlayerBaseData.GetInstance().mails.rewardMailList, PlayerBaseData.GetInstance().mails.rewardMailDics, RwardRootObjPath);

                UpdateChoosenBaseType(1);
            }
            
            // 刷新界面
            //UpdateNoMailTip();
            //UpdateOneKeyBtn();
            
            //ClearMailDetail();

            SystemNotifyManager.SystemNotify(1026);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (!CreateFinish)
            {
                return;
            }

            fUpdateInterval += timeElapsed;

            if (fUpdateInterval <= 60f)
            {
                return;
            }

            fUpdateInterval = 0f;

            if (BaseTypes[0].isOn)
            {
                UpdateLeftTime(CreatMailObjNum,PlayerBaseData.GetInstance().mails.mailList,RootObjPath);
            }
            else
            {
                UpdateLeftTime(CreatRewardMailObjNum, PlayerBaseData.GetInstance().mails.rewardMailList, RwardRootObjPath);
            }
        }

        void CreateMailListPreferb()
        {
            CreatMailObjNum = 0;

            // 找到挂载点
            GameObject objRoot = Utility.FindGameObject(frame, RootObjPath);
            if (objRoot == null)
            {
                Logger.LogError("can't find obj : RootObjPath");
                return;
            }

            MailItemObjList.Clear();

            for (int i = 0; i < PlayerBaseData.GetInstance().mails.mailList.Count; i++)
            {
                MailTitleInfo MailListInfo = PlayerBaseData.GetInstance().mails.mailList[i];

                // 创建挂载对象
                GameObject MailItemobj = AssetLoader.instance.LoadResAsGameObject(MailItemPath);
                if (MailItemobj == null)
                {
                    Logger.LogError("can't create obj in MailFrame");
                    return;
                }

                // 命名
                MailItemobj.name = string.Format("MailItem{0}", i);

                // 绑定挂点
                Utility.AttachTo(MailItemobj, objRoot);

                MailItemObjList.Add(MailItemobj);

                CreatMailObjNum++;

                // 界面显示
                UpdateMailPerferbInterface(ref MailItemobj, MailListInfo);
                UpdateMailStatus(ref MailItemobj, MailListInfo);

                // 绑定事件
                UpdateMailPerferbBtnBind(ref MailItemobj,PlayerBaseData.GetInstance().mails.mailList,PlayerBaseData.GetInstance().mails.mailDics,RootObjPath, i);
                
            }
        }

        void CreateRewardMailListPreferb()
        {
            CreatRewardMailObjNum = 0;

            // 找到挂载点
            GameObject objRoot = Utility.FindGameObject(frame, RwardRootObjPath);
            if (objRoot == null)
            {
                Logger.LogError("can't find obj : RootObjPath");
                return;
            }

            RewardMailItemObjList.Clear();

            for (int i = 0; i < PlayerBaseData.GetInstance().mails.rewardMailList.Count; i++)
            {
                MailTitleInfo MailListInfo = PlayerBaseData.GetInstance().mails.rewardMailList[i];

                // 创建挂载对象
                GameObject MailItemobj = AssetLoader.instance.LoadResAsGameObject(MailItemPath);
                if (MailItemobj == null)
                {
                    Logger.LogError("can't create obj in MailFrame");
                    return;
                }

                // 命名
                MailItemobj.name = string.Format("MailItem{0}", i);

                // 绑定挂点
                Utility.AttachTo(MailItemobj, objRoot);

                RewardMailItemObjList.Add(MailItemobj);

                CreatRewardMailObjNum++;

                // 界面显示
                UpdateMailPerferbInterface(ref MailItemobj, MailListInfo);
                UpdateMailStatus(ref MailItemobj, MailListInfo);

                // 绑定事件
                UpdateMailPerferbBtnBind(ref MailItemobj, PlayerBaseData.GetInstance().mails.rewardMailList, PlayerBaseData.GetInstance().mails.rewardMailDics, RwardRootObjPath, i);
                
            }
        }
        
        void UpdateRewardMailListPreferb()
        {
            // 找到挂载点
            GameObject objRoot = Utility.FindGameObject(frame, RwardRootObjPath);
            if (objRoot == null)
            {
                Logger.LogError("can't find obj : RootObjPath");
                return;
            }

            if (PlayerBaseData.GetInstance().mails != null)
            {
                if (PlayerBaseData.GetInstance().mails.rewardMailList != null)
                {
                    for (int i = 0; i < PlayerBaseData.GetInstance().mails.rewardMailList.Count; i++)
                    {
                        MailTitleInfo MailListInfo = PlayerBaseData.GetInstance().mails.rewardMailList[i];
                        if (MailListInfo != null)
                        {
                            if (i < RewardMailItemObjList.Count)
                            {
                                GameObject MailItemobj = RewardMailItemObjList[i];
                                if (MailItemobj == null)
                                {
                                    Logger.LogError("can't create obj in MailFrame");
                                    return;
                                }

                                // 命名
                                MailItemobj.name = string.Format("MailItem{0}", i);
                                // 界面显示
                                UpdateMailPerferbInterface(ref MailItemobj, MailListInfo);
                                UpdateMailStatus(ref MailItemobj, MailListInfo);

                                // 绑定事件
                                UpdateMailPerferbBtnBind(ref MailItemobj, PlayerBaseData.GetInstance().mails.rewardMailList, PlayerBaseData.GetInstance().mails.rewardMailDics, RwardRootObjPath, i);
                            }
                            else
                            {
                                // 创建挂载对象
                                GameObject MailItemobj = AssetLoader.instance.LoadResAsGameObject(MailItemPath);
                                if (MailItemobj == null)
                                {
                                    Logger.LogError("can't create obj in MailFrame");
                                    return;
                                }

                                // 命名
                                MailItemobj.name = string.Format("MailItem{0}", i);

                                // 绑定挂点
                                Utility.AttachTo(MailItemobj, objRoot);

                                RewardMailItemObjList.Add(MailItemobj);

                                CreatRewardMailObjNum++;

                                // 界面显示
                                UpdateMailPerferbInterface(ref MailItemobj, MailListInfo);
                                UpdateMailStatus(ref MailItemobj, MailListInfo);

                                // 绑定事件
                                UpdateMailPerferbBtnBind(ref MailItemobj, PlayerBaseData.GetInstance().mails.rewardMailList, PlayerBaseData.GetInstance().mails.rewardMailDics, RwardRootObjPath, i);
                            }
                        }
                    }
                }
            }
        }

        void InitPackageItems()
        {
            GameObject RewardItemRoot = Utility.FindGameObject(frame, RewardItemRootPath);
            if (RewardItemRoot == null)
            {
                Logger.LogError("can't find componnent : RewardItemRootPath");
                return;
            }

            RectTransform[] Positions = RewardItemRoot.GetComponentsInChildren<RectTransform>();

            int iIndex = 0;
            for (int i = 0; i < Positions.Length; i++)
            {
                if (Positions[i].name != string.Format("Pos{0}", iIndex + 1))
                {
                    continue;
                }

                ComItem ShowItem = CreateComItem(Positions[i].gameObject);

                if (iIndex < ItemsNum)
                {
                    Items.Add(ShowItem);
                }

                iIndex++;
            }
        }

        void UpdateMailList(int creatMailObj, List<MailTitleInfo> mailList, DictionaryView<UInt64, GameMailData.MailData> mailDics,string rootPath)
        {
            if (creatMailObj >= mailList.Count)
            {
                for (int i = 0; i < creatMailObj; i++)
                {
                    string Path = rootPath + string.Format("/MailItem{0}", i);

                    GameObject MailItemobj = Utility.FindGameObject(Path);
                    if (MailItemobj == null)
                    {
                        Logger.LogError("can't find obj : MailItemobj");
                        return;
                    }

                    if (i < mailList.Count)
                    {
                        MailTitleInfo MailListInfo = mailList[i];

                        UpdateMailPerferbInterface(ref MailItemobj, MailListInfo);
                        UpdateMailStatus(ref MailItemobj, MailListInfo);
                        UpdateMailPerferbBtnBind(ref MailItemobj, mailList, mailDics, rootPath, i);

                        MailItemobj.SetActive(true);
                    }
                    else
                    {
                        MailItemobj.SetActive(false);
                    }
                }
            }
            else
            {
                //UpdateMailPerferbBtnBind();
            }
        }

        void UpdateMailPerferbInterface(ref GameObject MailItemobj, MailTitleInfo MailInfo)
        {
            Text[] MailText = MailItemobj.GetComponentsInChildren<Text>();

            int iCount = 0;
            for (int j = 0; j < MailText.Length; j++)
            {
                if (MailText[j].name == "MailTitle")
                {
                    MailText[j].text = MailInfo.title;
                    iCount++;
                }
                else if (MailText[j].name == "MailSender")
                {
                    MailText[j].text = MailInfo.sender;
                    iCount++;
                }
                else if (MailText[j].name == "SendTime")
                {
                    MailText[j].text = Function.GetBeginTimeStr(MailInfo.date);
                    iCount++;
                }
                else if (MailText[j].name == "TimeLeft")
                {
                    if (MailInfo.deadline - MailInfo.date > 0)
                    {
                        MailText[j].text = Function.GetLeftTimeStr(MailInfo.date, MailInfo.deadline - MailInfo.date);
                    }
                    iCount++;
                }

                if (iCount == 4)
                {
                    break;
                }
            }

        }

        void UpdateLeftTime(int creatMailNum, List<MailTitleInfo> mailList,string rootPath)
        {
            if (creatMailNum >= mailList.Count)
            {
                for (int i = 0; i < creatMailNum; i++)
                {
                    string Path = rootPath + string.Format("/MailItem{0}", i);

                    GameObject MailItemobj = Utility.FindGameObject(Path);
                    if (MailItemobj == null)
                    {
                        Logger.LogError("can't find obj : MailItemobj");
                        return;
                    }

                    if (i < mailList.Count)
                    {
                        CalSelMailLeftTime(ref MailItemobj,mailList, i);

                        MailItemobj.SetActive(true);
                    }
                    else
                    {
                        MailItemobj.SetActive(false);
                    }
                }
            }
            else
            {
                //UpdateMailPerferbBtnBind();
            }
        }

        void UpdateSelMailLeftTime(List<MailTitleInfo> mailList,string rootPath)
        {
            for (int i = 0; i < mailList.Count; i++)
            {
                if (mailList[i].id != CurSelMailID)
                {
                    continue;
                }

                // 刷新图标
                string Path = rootPath + string.Format("/MailItem{0}", i);

                GameObject MailItemobj = Utility.FindGameObject(Path);
                if (MailItemobj == null)
                {
                    Logger.LogError("can't find obj : MailItemobj");
                    return;
                }

                CalSelMailLeftTime(ref MailItemobj, mailList, i);

                break;
            }
        }

        void UpdateMailStatus(ref GameObject MailItemobj, MailTitleInfo MailInfo)
        {
            Image[] MailImage = MailItemobj.GetComponentsInChildren<Image>(true);

            int iCount = 0;
            for (int i = 0; i < MailImage.Length; i++)
            {
                if (MailImage[i].name == "Icon")
                {
                    // Sprite Icon;
                    string spritePath;

                    if (MailInfo.status == 0)
                    {
                        // Icon = AssetLoader.instance.LoadRes(MailUnReadPath,typeof(Sprite)).obj as Sprite;
                        spritePath = MailUnReadPath;
                    }
                    else
                    {
                        // Icon = AssetLoader.instance.LoadRes(MailReadPath, typeof(Sprite)).obj as Sprite;
                        spritePath = MailReadPath;
                    }

                    // MailImage[i].sprite = Icon;
                    ETCImageLoader.LoadSprite(ref MailImage[i], spritePath);
                    iCount++;
                }
                else if (MailImage[i].name == "NewMailPrompt")
                {
                    if (MailInfo.status == 0)
                    {
                        MailImage[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        MailImage[i].gameObject.SetActive(false);
                    }
                    iCount++;
                }

                if (iCount == 2)
                {
                    break;
                }
            }

            Button[] btns = MailItemobj.GetComponentsInChildren<Button>();
            for (int i = 0; i < btns.Length; i++)
            {
                if (btns[i].name == "Take")
                {
                    UIGray gray = btns[i].gameObject.GetComponent<UIGray>();
                    if (gray == null)
                    {
                        gray = btns[i].gameObject.AddComponent<UIGray>();
                    }

                    Text txt = btns[i].gameObject.GetComponentInChildren<Text>();

                    if (MailInfo.hasItem == 0)
                    {
                        gray.enabled = true;
                        btns[i].interactable = false;
                        txt.text = "已领取";
                    }
                    else
                    {
                        gray.enabled = false;
                        btns[i].interactable = true;
                        txt.text = "领取";
                    }
                    break;
                }
            }

            //added by mjx on 2017-7-10
            GameObject attachIcon = Utility.FindGameObject(MailItemobj, "AttachIcon");
            if (MailInfo.hasItem == 0)
                attachIcon.CustomActive(false);
            else
                attachIcon.CustomActive(true);


        }

        void UpdateMailPerferbBtnBind(ref GameObject MailItemobj, List<MailTitleInfo> mailList, DictionaryView<UInt64, GameMailData.MailData> mailDics,string rootPath, int iBtnIdx)
        {
            Button[] MailBtn = MailItemobj.GetComponentsInChildren<Button>();

            for (int i = 0; i < MailBtn.Length; i++)
            {
                if (MailBtn[i].name == "NormalBack")
                {
                    MailBtn[i].onClick.RemoveAllListeners();

                    int index = iBtnIdx;
                    MailBtn[i].onClick.AddListener(() => { OnClickMail(mailList, mailDics, index, rootPath); });

                }
                else if (MailBtn[i].name == "Take")
                {
                    MailBtn[i].onClick.RemoveAllListeners();

                    int index = iBtnIdx;
                    MailBtn[i].onClick.AddListener(() => { OnClickTakeMail(index); });

                }
            }
        }

        void UpdateMailDetail(GameMailData.MailData MailDetailData)
        {
            if(MailDetailData == null || MailDetailData.info == null || MailDetailData.items == null || MailDetailData.detailItems == null)
            {
                if(MailDetailData == null)
                {
                    Logger.LogError("MailDetailData is null");
                }
                else if (MailDetailData.info == null)
                {
                    Logger.LogError("MailDetailData.info is null");
                }
                else if (MailDetailData.items == null)
                {
                    Logger.LogError("MailDetailData.items is null");
                }
                else if (MailDetailData.detailItems == null)
                {
                    Logger.LogError("MailDetailData.detailItems is null");
                }

                return;
            }

            ShowTipItemData.Clear();

            MailTitle.text = MailDetailData.info.title;
            SenderName.text = MailDetailData.info.sender;
            //MailContent.text = MailDetailData.content.Replace("\\n", "\n");
            if (MailContent != null)
            {
                string mailContent = MailDetailData.content.Replace("\\n", "\n");
                //mailContent += string.Format("{{U {0} {1} {2}}}", "前往", "http://ald.xy.com", "4");
                MailContent.SetText(mailContent);
            }

            ClearItemsIcon();

            if (MailDetailData.info.hasItem == 1)
            {
                int iIndex = 0;
                for (int i = 0; i < MailDetailData.items.Count && i < ItemsNum; i++)
                {
                    ItemData data = ItemDataManager.CreateItemDataFromTable((int)MailDetailData.items[i].id);
                    if (data == null)
                    {
                        continue;
                    }

                    data.Count = (int)MailDetailData.items[i].num;
                    data.StrengthenLevel = MailDetailData.items[i].strength;
                    SetUpItem(data, i);

                    iIndex++;
                }

                for (int i = 0; i < MailDetailData.detailItems.Count && (iIndex + i) < ItemsNum; i++)
                {
                    ItemData data = ItemDataManager.GetInstance().CreateItemDataFromNet(MailDetailData.detailItems[i]);
                    if (data == null)
                    {
                        continue;
                    }

                    data.Count = MailDetailData.detailItems[i].num;

                    SetUpItem(data, iIndex + i);
                }

                mStateContrl.Key = mShow;
            }
            else
            {
                mStateContrl.Key = mHide;
            }
        }

        void UpdateNoMailTip()
        {
            //modified by mjx on 2017-7-10
            if (PlayerBaseData.GetInstance().mails.mailList.Count > 0 && PlayerBaseData.GetInstance().mails.rewardMailList.Count > 0 ||
                PlayerBaseData.GetInstance().mails.mailList.Count > 0 && PlayerBaseData.GetInstance().mails.rewardMailList.Count <= 0)
             {
                 UpdateChoosenBaseType(0);
             }
             else if (PlayerBaseData.GetInstance().mails.mailList.Count <= 0 && PlayerBaseData.GetInstance().mails.rewardMailList.Count > 0)
             {
                UpdateChoosenBaseType(1);
            }
             else
             {
                 UpdateChoosenBaseType(0);
            }
        }

        void UpdateOneKeyBtn()
        {
            //OneKeyDelete.text = string.Format("一键删除<color=#7FD0E1FF>({0})</color>", PlayerBaseData.GetInstance().mails.OneKeyDeleteNum);
            //OneKeyReceive.text = string.Format("一键领取<color=#7FD0E1FF>({0})</color>", PlayerBaseData.GetInstance().mails.OneKeyReceiveNum);
        }

        void SetUpItem(ItemData data, int Index)
        {
            if(Items == null || Index >= Items.Count)
            {
                return;
            }

            int idx = Index;
            Items[Index].Setup(data, (GameObject obj, ItemData item) => { OnShowItemTip(idx); });

            ShowTipItemData.Add(data);
        }

        void ClearItemsIcon()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] != null)
                    Items[i].Setup(null, null);
            }
        }

        void ClearMailDetail()
        {
            MailTitle.text = "";
            SenderName.text = "";
            //MailContent.text = "";
            if (MailContent != null)
            {
                MailContent.SetText("");
            }
            ClearItemsIcon();
        }

        void OnShowItemTip(int iIndex)
        {
            if (iIndex >= ShowTipItemData.Count)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(ShowTipItemData[iIndex]);
        }

        void CalSelMailLeftTime(ref GameObject MailItemobj, List<MailTitleInfo> mailList, int index)
        {
            MailTitleInfo MailInfo = mailList[index];

            Text[] MailText = MailItemobj.GetComponentsInChildren<Text>();

            for (int i = 0; i < MailText.Length; i++)
            {
                if (MailText[i].name == "TimeLeft")
                {
                    if (MailInfo.deadline - MailInfo.date > 0)
                    {
                        MailText[i].text = Function.GetLeftTimeStr(MailInfo.date, MailInfo.deadline - MailInfo.date);
                    }
//                     if (MailInfo.id == CurSelMailID)
//                     {
//                         LeftTime.text = MailText[i].text;
//                     }

                    break;
                }
            }
        }

        [UIEventHandle(WndRoot+"Toggles/toggle{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, BaseTabNum)]
        void OnSwitchBaseType(int iIndex, bool value)
        {
            if (iIndex < 0 || !value)
            {
                return;
            }

            if (iIndex == 0)
            {
                if (PlayerBaseData.GetInstance().mails.mailList.Count <= 0)
                {
                    noMailTipGo.gameObject.CustomActive(true);
                    mailListRect.gameObject.CustomActive(false);
                    rewardsMailListRect.gameObject.CustomActive(false);
                    bottonImg.gameObject.CustomActive(false);
                    rewardsBottomRect.gameObject.CustomActive(false);
                    mailInfoRect.gameObject.CustomActive(false);
                }
                else
                {
                    OnClickMail(PlayerBaseData.GetInstance().mails.mailList, PlayerBaseData.GetInstance().mails.mailDics, 0, RootObjPath);
                    noMailTipGo.gameObject.CustomActive(false);
                    mailListRect.gameObject.CustomActive(true);
                    rewardsMailListRect.gameObject.CustomActive(false);
                    bottonImg.gameObject.CustomActive(true);
                    rewardsBottomRect.gameObject.CustomActive(false);
                    mailInfoRect.gameObject.CustomActive(true);
                }
                
            }
            else
            {
                if (PlayerBaseData.GetInstance().mails.rewardMailList.Count <= 0)
                {

                    noMailTipGo.gameObject.CustomActive(true);
                    rewardsMailListRect.gameObject.CustomActive(false);
                    bottonImg.gameObject.CustomActive(false);
                    mailListRect.gameObject.CustomActive(false);
                    rewardsBottomRect.gameObject.CustomActive(false);
                    mailInfoRect.gameObject.CustomActive(false);
                }
                else
                {

                    OnClickMail(PlayerBaseData.GetInstance().mails.rewardMailList, PlayerBaseData.GetInstance().mails.rewardMailDics, 0, RwardRootObjPath);
                    noMailTipGo.gameObject.CustomActive(false);
                    rewardsMailListRect.gameObject.CustomActive(true);
                    bottonImg.gameObject.CustomActive(false);
                    mailListRect.gameObject.CustomActive(false);
                    rewardsBottomRect.gameObject.CustomActive(true);
                    mailInfoRect.gameObject.CustomActive(true);
                }
               
            }

        }


        //         [UIControl("NoMailTip")]
        //         protected Text NoMailTi
        [UIControl(WndRoot + "MainInfomation/titileback/title")]
        protected Text MailTitle;

        [UIControl(WndRoot + "MainInfomation/senderContent/senderback/sendername")]
        protected Text SenderName;

        [UIControl(WndRoot + "MainInfomation/infomationback/Units/SerViewPort/Content/infomation")]
        protected LinkParse MailContent;
        //         [UIControl("bottom/BtnDeleteAll/Text")]
        //         protected Text OneKeyDelete;

        [UIControl(WndRoot + "bottom/BtnAcceptAll/Text")]
        protected Text OneKeyReceive;

        [UIControl("Title/Help")]
        protected Button helpBtn;

        [UIControl(WndRoot + "bottom/BtnAccpet")]
        Button announcementAccpetBtn;

        //added by mjx on 2017-7-10
        [UIEventHandle(WndRoot + "bottom/BtnAccpet")]
        void OnAcceptBtnClick()
        {
            if (announcementAccpetBtn != null)
            {
                announcementAccpetBtn.enabled = false;

                InvokeMethod.Invoke(this, 0.30f, () => 
                {
                    if (announcementAccpetBtn != null)
                    {
                        announcementAccpetBtn.enabled = true;
                    }
                });
            }

            OnClickReceive(PlayerBaseData.GetInstance().mails.mailList);
        }

        [UIControl(WndRoot + "Rewardsbottom/Buttons/BtnAccpet")]
        Button rewardsAccpetBtn;

        [UIEventHandle(WndRoot+ "Rewardsbottom/Buttons/BtnAccpet")]
        void OnAcceptButtonClick()
        {
            if (rewardsAccpetBtn != null)
            {
                rewardsAccpetBtn.enabled = false;

                InvokeMethod.Invoke(this, 0.30f, () => 
                {
                    if (rewardsAccpetBtn != null)
                    {
                        rewardsAccpetBtn.enabled = true;
                    }
                });
            }

            OnClickReceive(PlayerBaseData.GetInstance().mails.rewardMailList);
        }
        [UIControl(WndRoot + "NoMailTip",typeof(Text))]
        Text noMailTipGo;
        [UIControl(WndRoot + "bottom", typeof(Image))]
        Image bottonImg;
        [UIControl(WndRoot + "Rewardsbottom")]
        RectTransform rewardsBottomRect;

        [UIControl(WndRoot + "MainInfomation", typeof(RectTransform))]
        RectTransform mailInfoRect;

        [UIControl(WndRoot + "MailList", typeof(RectTransform))]
        RectTransform mailListRect;

        [UIControl(WndRoot + "RewardsMailList", typeof(RectTransform))]
        RectTransform rewardsMailListRect;


        [UIControl(WndRoot+"Toggles/toggle{0}", typeof(Toggle), 1)]
        Toggle[] BaseTypes = new Toggle[BaseTabNum];

        void UpdateChoosenBaseType(int iIndex)
        {
            for (int i = 0; i < BaseTypes.Length; i++)
            {
                BaseTypes[i].isOn = false;
                if (i == iIndex)
                {
                    BaseTypes[i].isOn = true;
                    break;
                }
            }
        }

    }
}
