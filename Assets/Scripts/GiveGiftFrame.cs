using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Protocol;
using Network;
using System;

namespace GameClient
{
    /// <summary>
    /// 赠送礼物
    /// </summary>
    class GiveGiftFrame : ClientFrame
    {

        List<GameObject> itemDatas = new List<GameObject>();
        Dictionary<string, ItemData> keyValuePairs = new Dictionary<string, ItemData>();
        ItemData currentItemData = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Vip/GiveGiftFrame";
        }

        protected override void _OnOpenFrame()
        {
            _bindExUI();
            InitItems();
        }

        private void InitItems()
        {
            for (var i = 0; i < itemDatas.Count; i++)
            {
                GameObject.Destroy(itemDatas[i]);
            }
            itemDatas.Clear();
            keyValuePairs.Clear();

            List<ulong> itemGuids = new List<ulong>();

            List<AuctionSellItemData> items1 =
                AuctionNewDataManager.GetInstance().GetAuctionSellItemDataByType(ActionNewSellTabType.AuctionNewSellEquipType);
            if (items1 != null)
            {
                for (var i = 0; i < items1.Count; i++)
                {
                    var sellItemData = items1[i];
                    itemGuids.Add(sellItemData.uId);
                }
            }
            
            List<AuctionSellItemData> items2 =
                AuctionNewDataManager.GetInstance().GetAuctionSellItemDataByType(ActionNewSellTabType.AuctionNewSellMaterialType);
            if (items2 != null)
            {
                for (var i = 0; i < items2.Count; i++)
                {
                    var sellItemData = items2[i];
                    itemGuids.Add(sellItemData.uId);
                }
            }
            
            {
                ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(600000001);
                int count = ItemDataManager.GetInstance().GetOwnedItemCount(600000001, false);
                int num = count / 10000;
                GameObject go = GameObject.Instantiate<GameObject>(detailItem, Content);
                go.name = "detailItem0";
                ComCommonBind tBind = go.GetComponent<ComCommonBind>();
                Text itemName = tBind.GetCom<Text>("itemName");
                Text itemCount = tBind.GetCom<Text>("itemCount");
                Image itemRoot = tBind.GetCom<Image>("itemRoot");
                Button itemBtn = tBind.GetCom<Button>("itemBtn");
                if (itemBtn!=null)
                {
                    itemBtn.onClick.AddListener(() => 
                    {
                        SetItem(go.name, detailItem); 
                    });
                }
                itemRoot.enabled = true;
                itemCount.gameObject.SetActive(true);
                itemName.text = itemData.Name;
                itemCount.text = num.ToString() + "万";
                ETCImageLoader.LoadSprite(ref itemRoot, itemData.Icon);
                keyValuePairs.Add(go.name, itemData);
                itemDatas.Add(go);
            }

            for (int i = 0; i < itemGuids.Count; i++)
            {
                var guid = itemGuids[i];
                ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
                int count = ItemDataManager.GetInstance().GetItemCountInPackage(itemData.TableID);

                GameObject go = GameObject.Instantiate<GameObject>(detailItem, Content);
                go.name = "detailItem" + (i + 1);
                ComCommonBind tBind = go.GetComponent<ComCommonBind>();
                Text itemName = tBind.GetCom<Text>("itemName");
                Text itemCount = tBind.GetCom<Text>("itemCount");
                Image itemRoot = tBind.GetCom<Image>("itemRoot");
                Button itemBtn = tBind.GetCom<Button>("itemBtn");
                if (itemBtn!=null)
                {
                    itemBtn.onClick.AddListener(() => 
                    {
                        SetItem(go.name, detailItem); 
                    });
                }
                itemRoot.enabled = true;
                itemCount.gameObject.SetActive(true);
                itemName.text = itemData.Name;
                itemCount.text = count.ToString();
                ETCImageLoader.LoadSprite(ref itemRoot, itemData.Icon);
                keyValuePairs.Add(go.name, itemData);
                itemDatas.Add(go);
            }
            //把右侧当前选择显示第一个道具
            if (itemDatas.Count != 0)
            {
                SetItem(itemDatas[0].name, detailItem);
            }
        }

        #region ExtraUIBind
        private RectTransform Content = null;
        private Button ButtonConfirm = null;
        private Button btClose = null;
        private Button ButtonMax = null;
        private InputField ButtonCountInput = null;
        private GameObject detailItem = null;
        private RectTransform Queding = null;
        private Button quedingBtn = null;
        private Button quxiaoBtn = null;
        private RectTransform actorShowMenuGo = null;
        private Text mLevel = null;
        private Image mPortrait = null;
        private Text mName = null;
        private Text mGuildName = null;
        private Text mVip = null;
        private Text mJobName = null;
        private Text mPkLevel = null;
        private Image mPkLevelImg = null;
        private Image mPkLevelNum = null;
        private UINumber mVipLv = null;
        MenuData m_kMenuData;
        private GameObject mVipParent = null;

        private void SetItem(string goName, GameObject go)
        {
            ComCommonBind tBind2 = go.GetComponent<ComCommonBind>();
            Text itemName2 = tBind2.GetCom<Text>("itemName");
            Text itemCount2 = tBind2.GetCom<Text>("itemCount");
            Image itemRoot2 = tBind2.GetCom<Image>("itemRoot");
            itemRoot2.enabled = true;
            itemCount2.gameObject.SetActive(true);
            ItemData itemData = keyValuePairs[goName];
            int count = 0;
            if (itemData.TableID == 600000001)
            {
                int count1 = ItemDataManager.GetInstance().GetOwnedItemCount(600000001, false);
                count = count1 / 10000;
            }
            else
            {
                count = ItemDataManager.GetInstance().GetItemCountInPackage(itemData.TableID);
            }
            itemName2.text = itemData.Name;
            if (itemData.TableID == 600000001)
            {
                itemCount2.text = count.ToString() + "万";
            }
            else
            {
                itemCount2.text = count.ToString();
            }
            ETCImageLoader.LoadSprite(ref itemRoot2, itemData.Icon);
            currentItemData = itemData;
        }

        protected override void _bindExUI()
        {
            Queding = mBind.GetCom<RectTransform>("Queding");
            Content = mBind.GetCom<RectTransform>("Content");
            ButtonConfirm = mBind.GetCom<Button>("ButtonConfirm");
            if (null != ButtonConfirm)
            {
                ButtonConfirm.onClick.AddListener(_onButtonConfirmClick);
            }
            ButtonMax = mBind.GetCom<Button>("ButtonMax");
            if (null != ButtonMax)
            {
                ButtonMax.onClick.AddListener(_ButtonMax);
            }
            ButtonCountInput = mBind.GetCom<InputField>("ButtonCountInput");
            detailItem = mBind.GetGameObject("detailItem");
            btClose = mBind.GetCom<Button>("Close");
            if (null != btClose)
            {
                btClose.onClick.AddListener(_onBtCloseButtonClick);
            }

            actorShowMenuGo = mBind.GetCom<RectTransform>("ActorShowMenu");

            ComCommonBind actorBind = actorShowMenuGo.GetComponent<ComCommonBind>();
            mLevel = actorBind.GetCom<Text>("level");
            mPortrait = actorBind.GetCom<Image>("portrait");
            mName = actorBind.GetCom<Text>("name");
            mGuildName = actorBind.GetCom<Text>("guildName");
            mVip = actorBind.GetCom<Text>("vip");
            mJobName = actorBind.GetCom<Text>("jobName");
            mPkLevel = actorBind.GetCom<Text>("pkLevel");
            mPkLevelImg = actorBind.GetCom<Image>("pkLevelImg");
            mPkLevelNum = actorBind.GetCom<Image>("pkLevelNum");
            mVipLv = actorBind.GetCom<UINumber>("vipLv");
            mVipParent = actorBind.GetGameObject("vipParent");
            
            _CreateMenuItemsFromData();

            NetProcess.AddMsgHandler(SceneGiveGiftRet.MsgID, OnRecvSceneGiveGiftRet);
        }

         void OnRecvSceneGiveGiftRet(MsgDATA msgData)
        {
            SceneGiveGiftRet kRet = new SceneGiveGiftRet();
            kRet.decode(msgData.bytes);

            if(kRet.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)kRet.code);
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("赠送成功");
                InitItems();
                ButtonCountInput.text = "1";
            }
        }

        void _CreateMenuItemsFromData()
        {
            m_kMenuData = PlayerBaseData.GetInstance().CurrentMenuData;
            if (m_kMenuData != null)
            {
                Debug.LogError("Lv." + m_kMenuData.level);
                mLevel.text = "Lv." + m_kMenuData.level;

                string path = "";
                var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(m_kMenuData.jobID);
                if (jobData != null)
                {
                    if (mJobName)
                        mJobName.text = jobData.Name;
                    ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobData.Mode);
                    if (resData != null)
                    {
                        path = resData.IconPath;
                    }
                }
                ETCImageLoader.LoadSprite(ref mPortrait, path);
                mName.text = m_kMenuData.name;

                if (m_kMenuData.HasGuild())
                    mGuildName.text = "公会 " + m_kMenuData.guildName;
                else
                    mGuildName.text = "";

                if (m_kMenuData.HasVip())
                {
                    mVipParent.CustomActive(true);
                    if (mVipLv)
                        mVipLv.Value = m_kMenuData.vip;
                }
                else
                {
                    mVipParent.CustomActive(false);
                }

                if (mPkLevel)
                    mPkLevel.text = SeasonDataManager.GetInstance().GetRankName(m_kMenuData.pkLevel);
                if (mPkLevelImg)
                {
                    ETCImageLoader.LoadSprite(ref mPkLevelImg, SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon(m_kMenuData.pkLevel));
                }
                if (mPkLevelNum)
                {
                    ETCImageLoader.LoadSprite(ref mPkLevelNum, SeasonDataManager.GetInstance().GetSubSeasonLevelIcon(m_kMenuData.pkLevel));
                }
            }
        }

        protected override void _unbindExUI()
        {
            Content = null;
            if (null != ButtonConfirm)
            {
                ButtonConfirm.onClick.RemoveListener(_onButtonConfirmClick);
            }
            ButtonConfirm = null;
            
            if (null != ButtonMax)
            {
                ButtonMax.onClick.RemoveListener(_ButtonMax);
            }
            ButtonMax = null;
            ButtonCountInput = null;
            detailItem = null;
            btClose = null;
            mLevel = null;
            mPortrait = null;
            mName = null;
            mGuildName = null;
            mVip = null;
            mJobName = null;
            mPkLevel = null;
            mPkLevelImg = null;
            mPkLevelNum = null;
            mVipLv = null;
            PlayerBaseData.GetInstance().CurrentMenuData = null;

            NetProcess.RemoveMsgHandler(SceneGiveGiftRet.MsgID, OnRecvSceneGiveGiftRet);
        }
        #endregion

        #region Callback
        private void _ButtonMax()
        {
            if (currentItemData != null)
            {
                int count = 0;
                if (currentItemData.TableID == 600000001)
                {
                    int count1 = ItemDataManager.GetInstance().GetOwnedItemCount(600000001, false);
                    count = count1 / 10000;
                }
                else
                {
                    count = ItemDataManager.GetInstance().GetItemCountInPackage(currentItemData.TableID);
                }
                ButtonCountInput.text = count.ToString();
            }
        }
        
        bool isClicked = false;

        private void _onButtonConfirmClick()
        {
            if (isClicked)
            {
                return;
            }
            isClicked = true;

            OnButtonConfirmClick();

            InvokeMethod.Invoke(this, 0.50f, () =>
            {
                isClicked = false;
            });
        }

        private void OnButtonConfirmClick()
        {
            int max = 0;
            if (currentItemData.TableID == 600000001)
            {
                int count1 = ItemDataManager.GetInstance().GetOwnedItemCount(600000001, false);
                max = count1 / 10000;
            }
            else
            {
                max = ItemDataManager.GetInstance().GetItemCountInPackage(currentItemData.TableID);
            }
            if(Convert.ToInt32(ButtonCountInput.text) > max)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("赠送数量超过物品最大数量");
                return;
            }
            if(Convert.ToInt32(ButtonCountInput.text) <= 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("赠送数量不能小于等于0");
                return;
            }
            if (currentItemData.TableID == 600000001 && Convert.ToInt32(ButtonCountInput.text) > 500)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("单次最高只能赠送500万金币");
                return;
            }
            
            SystemNotifyManager.SysNotifyMsgBoxOkCancel("赠送将消耗10点券,是否确认赠送?", () =>
            {
                _onquedingBtnButtonClick();
            }, () => { return; });
        }

        private void _onquedingBtnButtonClick()
        {
            UnityEngine.Debug.LogError("发送服务端："+ currentItemData.Name + ",数量:" + ButtonCountInput.text);
            UnityEngine.Debug.LogError("接收人：" + m_kMenuData.GUID);

            UnityEngine.Debug.LogError("物品GUID" + currentItemData.GUID);

            SceneGiveGiftReq kCmd = new SceneGiveGiftReq();
            kCmd.roleid = m_kMenuData.GUID;
            kCmd.guid = currentItemData.GUID;
            kCmd.count = Convert.ToUInt32(ButtonCountInput.text);
            if (currentItemData.TableID == 600000001)
            {
                kCmd.isGold = 1;
            }
            else
            {
                kCmd.isGold = 0;
            }
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kCmd);
        }

        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }

        #endregion
    }
}