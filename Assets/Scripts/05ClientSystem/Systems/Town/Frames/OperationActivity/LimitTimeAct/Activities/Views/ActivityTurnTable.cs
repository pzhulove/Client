using Network;
using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ActivityTurnTable : MonoBehaviour
    {
        int mTableID = -1;//传入的抽奖表id
        int tempRewardIndex = -1;//服务器返回的奖品下标
        List<int> rewardPoolTableId = new List<int>();//奖池表id
        GameObject[] rewardGo = new GameObject[8];//八个放入奖品的位置
       
      
        public void Init(int tableID)
        {
            _bindExUI();
            NetProcess.AddMsgHandler(SceneDrawPrizeRes.MsgID, OnSceneDrawPrizeRes);
            mTableID = tableID;
            initdata();
        }
        private void initdata()
        {
            rewardPoolTableId.Clear();
            InitIcon();

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
                if (rewardPooldata.DrawPrizeTableID == mTableID)
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
                for (int i = 0; i < rewardPoolTableId.Count; i++)
                {
                    if (i >= rewardGo.Length)
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
                    ComItem comitem = ComItemManager.Create(rewardGo[i]);
                    int result = (int)poolRewardTableData.ItemID;
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTips(result); });
                }
            }
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

        private void  OnDestroy()
        {
            NetProcess.RemoveMsgHandler(SceneDrawPrizeRes.MsgID, OnSceneDrawPrizeRes);
            clearData();
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
            mStartGray.enabled = true;
            mStart.interactable = false;
            SceneDrawPrizeReq req = new SceneDrawPrizeReq();
            req.id = (uint)mTableID;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 接收奖品
        /// </summary>
        /// <param name="data"></param>

        void OnSceneDrawPrizeRes(MsgDATA data)
        {
            SceneDrawPrizeRes ret = new SceneDrawPrizeRes();
            ret.decode(data.bytes);

            if ((ProtoErrorCode)ret.retCode == ProtoErrorCode.SUCCESS)
            {
                tempRewardIndex = (int)ret.rewardId;
                int result = -1;
                for (int i = 0; i < rewardPoolTableId.Count; i++)
                {
                    if (rewardPoolTableId[i] == tempRewardIndex)
                    {
                        result = i;
                    }
                }
                if (result != -1)
                {
                    mLuckyRoller.RotateUp(8, result, true, GetItem);
                }
                else
                {

                }
            }
            else
            {
                mStartGray.enabled = false;
                mStart.interactable = true;
            }
        }

        void GetItem()
        {
            mStartGray.enabled = false;
            mStart.interactable = true;
            var poolRewardTableData = TableManager.GetInstance().GetTableItem<RewardPoolTable>(tempRewardIndex);
            if (poolRewardTableData == null)
            {
                return;
            }
            var itemData = TableManager.GetInstance().GetTableItem<ItemTable>(poolRewardTableData.ItemID);
            if (itemData == null)
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
        protected  void _bindExUI()
        {
            ComCommonBind mBind = GetComponent<ComCommonBind>();
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
        }

        protected  void _unbindExUI()
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
