using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    class TurnTable : ClientFrame
    {
        int tableID = -1;//传入的抽奖表id
        int tempRewardIndex = -1;//服务器返回的奖品下标
        List<int> rewardPoolTableId = new List<int>();//奖池表id
        GameObject[] rewardGo = new GameObject[8];//八个放入奖品的位置
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Turntable";
        }
        protected override void _OnOpenFrame()
        {
            if(userData == null)
            {
                Logger.LogErrorFormat("需要传入抽奖表id");
                return;
            }
            else
            {
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChanged);
                tableID = (int)userData;
                initdata();
            }
        }

        private void initdata()
        {
            rewardPoolTableId.Clear();
            InitIcon();
            UpdateTime();
        }

        /// <summary>
        /// 初始化奖品icon的显示
        /// </summary>
        private void InitIcon()
        {
            var rewardPoolTableData = TableManager.GetInstance().GetTable<RewardPoolTable>();
            var enumerator = rewardPoolTableData.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var rewardPooldata = enumerator.Current.Value as RewardPoolTable;
                if (rewardPooldata.DrawPrizeTableID == tableID)
                {
                    rewardPoolTableId.Add(rewardPooldata.ID);
                }
            }
            if (rewardPoolTableId.Count == 0)
            {
                Logger.LogErrorFormat("奖池表格数据出错");
            }
            else
            {
                for(int i=0;i< rewardPoolTableId.Count;i++)
                {
                    if(i >= rewardGo.Length)
                    {
                        continue;
                    }
                    var poolRewardTableData = TableManager.GetInstance().GetTableItem<RewardPoolTable>(rewardPoolTableId[i]);
                    ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(poolRewardTableData.ItemID);
                    if (null == ItemDetailData)
                    {
                        Logger.LogErrorFormat("ItemData is null");
                        return;
                    }
                    ItemDetailData.Count = poolRewardTableData.ItemNum;
                    ComItem comitem = CreateComItem(rewardGo[i]);
                    int result = (int)poolRewardTableData.ItemID;
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTips(result); });
                }
            }
        }


        private void UpdateTime()
        {
            var drawPrizeTableData = TableManager.GetInstance().GetTableItem<DrawPrizeTable>(tableID);
            if(drawPrizeTableData == null)
            {
                return;
            }
            mLotteryNum.text = CountDataManager.GetInstance().GetCount(drawPrizeTableData.GetCountKey).ToString();
        }
        /// <summary>
        /// 显示tips
        /// </summary>
        /// <param name="result"></param>
        void OnShowTips(int result)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result);
            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChanged);
            clearData();
        }

        private void OnCountValueChanged(UIEvent uiEvent)
        {
            UpdateTime();
        }

        private void clearData()
        {
            rewardPoolTableId.Clear();
        }

        /// <summary>
        /// 请求抽奖
        /// </summary>
        private void SendSceneDrawPrizeReq()
        {
            if(mStartGray!=null)
            {
                mStartGray.enabled = true;
            }
            if(mStart!=null)
            {
                mStart.interactable = false;
            }
            SceneDrawPrizeReq req = new SceneDrawPrizeReq();
            req.id = (uint)tableID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 接收奖品
        /// </summary>
        /// <param name="data"></param>
        [MessageHandle(SceneDrawPrizeRes.MsgID)]
        void OnSceneDrawPrizeRes(MsgDATA data)
        {
            SceneDrawPrizeRes ret = new SceneDrawPrizeRes();
            ret.decode(data.bytes);

            if ((ProtoErrorCode)ret.retCode == ProtoErrorCode.SUCCESS)
            {
                tempRewardIndex = (int)ret.rewardId;
                int result = -1;
                for(int i=0;i<rewardPoolTableId.Count;i++)
                {
                    if(rewardPoolTableId[i] == tempRewardIndex)
                    {
                        result = i;
                    }
                }
                if(result != -1)
                {
                    mLuckyRoller.RotateUp(8, result, true, GetItem);
                }
                else
                {
                    
                }
            }
            else
            {
                if(mStartGray!=null)
                {
                    mStartGray.enabled = false;
                }
                if(mStart!=null)
                {
                    mStart.interactable = true;
                }
              
            }
        }

        void GetItem()
        {
            if(mStartGray!=null)
            {
                mStartGray.enabled = false;
            }
            if(mStart!=null)
            {
                mStart.interactable = true;
            }
            var poolRewardTableData = TableManager.GetInstance().GetTableItem<RewardPoolTable>(tempRewardIndex);
            if(poolRewardTableData == null)
            {
                return;
            }
            var itemData = TableManager.GetInstance().GetTableItem<ItemTable>(poolRewardTableData.ItemID);
            if(itemData == null)
            {
                return;
            }
            //SystemNotifyManager.SysNotifyFloatingEffect(itemData.GetColorName() + "*" + poolItem.ItemNum, CommonTipsDesc.eShowMode.SI_QUEUE, poolItem.ItemID);
            SystemNotifyManager.SysNotifyFloatingEffect(itemData.Name + "*" + poolRewardTableData.ItemNum);
        }

        #region ExtraUIBind
        private Button mStart = null;
        private TheLuckyRoller mLuckyRoller = null;
        private UIGray mStartGray = null;
        private Text mLotteryNum = null;
        protected override void _bindExUI()
        {
            rewardGo[0] = mBind.GetGameObject("pos0");
            rewardGo[1] = mBind.GetGameObject("pos1");
            rewardGo[2] = mBind.GetGameObject("pos2");
            rewardGo[3] = mBind.GetGameObject("pos3");
            rewardGo[4] = mBind.GetGameObject("pos4");
            rewardGo[5] = mBind.GetGameObject("pos5");
            rewardGo[6] = mBind.GetGameObject("pos6");
            rewardGo[7] = mBind.GetGameObject("pos7");
            mStart = mBind.GetCom<Button>("start");
            mStart.onClick.AddListener(_onStartButtonClick);
            mStartGray = mBind.GetCom<UIGray>("startGray");
            mLuckyRoller = mBind.GetCom<TheLuckyRoller>("LuckyRoller");
            mLotteryNum = mBind.GetCom<Text>("LotteryNum");
        }

        protected override void _unbindExUI()
        {
            rewardGo[0] = null;
            rewardGo[1] = null;
            rewardGo[2] = null;
            rewardGo[3] = null;
            rewardGo[4] = null;
            rewardGo[5] = null;
            rewardGo[6] = null;
            rewardGo[7] = null;
            mStart.onClick.RemoveListener(_onStartButtonClick);
            mStart = null;
            mLuckyRoller = null;
            mStartGray = null;
            mLotteryNum = null;
        }
        #endregion

        #region Callback
        private void _onStartButtonClick()
        {
            /* put your code in here */
            SendSceneDrawPrizeReq();
            
        }
        #endregion
    }
}