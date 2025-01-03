using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Network;
using Protocol;
using UnityEngine.Events;

namespace GameClient
{
    class ItemComeLink : MonoBehaviour
    {
        public int iItemLinkID = 0;
        public bool bNotEnough = false;
        public ComLinkFrame.OnClick onClick;
        public UnityEvent onEvent;
        Button button;
        ClientFrameBinder comFrameBinder;
        // Use this for initialization

        static int multiple = 0;//积分倍数
        static int endTime = 0;//积分结束时间
        static bool isTimer = false;
        void Start()
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }
            if (button != null)
            {
                button.onClick.AddListener(OnClickLink);
            }

            comFrameBinder = GetComponentInParent<ClientFrameBinder>();
        }

        void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button = null;
            }
            onClick = null;
            ms_co_purchase = null;
        }

        public static UnityEngine.Coroutine ms_co_purchase = null;
        public static bool LinkCommonPurchase(int iNeedCount,int iLinkID,ComLinkFrame.OnClick onClick,bool bIgnoreConfirm,
            UnityEngine.Events.UnityAction onFailed)
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.QuickBuyTable>(iLinkID);
            if(item != null && iNeedCount > 0)
            {
                var srcItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(item.ID);
                var tarItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(item.CostItemID);
                if(srcItemData == null || tarItemData == null)
                {
                    if(onFailed != null)
                    {
                        onFailed.Invoke();
                    }
                    return false;
                }

                if(!bIgnoreConfirm)
                {
                    string msg = string.Format(TR.Value("common_link_hint"), srcItemData.GetColorName(), iNeedCount, iNeedCount * item.CostNum, tarItemData.GetColorName());

                    multiple = item.multiple;

                    MallItemMultipleIntegralData data = MallNewDataManager.GetInstance().CheckMallItemMultipleIntegral(item.ID);
                    if (data != null)
                    {
                        multiple += data.multiple;
                        endTime = data.endTime;
                    }

                    if (endTime > 0)
                    {
                        isTimer = (endTime - TimeManager.GetInstance().GetServerTime()) > 0;
                    }

                    if (multiple > 0)
                    {
                        int price = MallNewUtility.GetTicketConvertIntergalNumnber(iNeedCount * item.CostNum) * multiple;
                        string mContent = string.Empty;
                        if (multiple <= 1)
                        {
                            mContent = TR.Value("mall_fast_buy_intergral_single_multiple_desc", price);
                        }
                        else
                        {
                            mContent = TR.Value("mall_fast_buy_intergral_many_multiple_desc", price, multiple,string.Empty);
                        }

                        if (isTimer == true)
                        {
                            mContent = TR.Value("mall_fast_buy_intergral_many_multiple_desc", price, multiple, TR.Value("mall_fast_buy_intergral_many_multiple_remain_time_desc", Function.SetShowTimeDay(endTime)));
                        }

                        msg += mContent;
                    }

                    CommonPurChaseHintFrame.Open(msg, TR.Value("common_link_hint_ok"), TR.Value("common_link_hint_cancel"),
                        ()=> { _OnClickConfirm(iNeedCount,item,iLinkID,onClick, onFailed); },
                        ()=> {
                            if (onFailed != null)
                            {
                                onFailed.Invoke();
                            }
                        });
                }
                else
                {
                    _OnClickConfirm(iNeedCount, item, iLinkID, onClick, onFailed);
                }

                return true;
            }
            return false;
        }

        public static void _OnClickConfirm(int iNeedCount,ProtoTable.QuickBuyTable item,int iLinkID, ComLinkFrame.OnClick onClick,
            UnityEngine.Events.UnityAction onFailed)
        {
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(item.CostItemID);
            int iCostCount = iNeedCount * item.CostNum;
            if (iHasCount < iCostCount)
            {
                if (IsLinkMoney(item.CostItemID,onClick,onFailed))
                {

                }
                else
                {
                    if(onFailed != null)
                    {
                        onFailed.Invoke();
                    }
                }
            }
            else
            {
                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = item.CostItemID, nCount = iCostCount }, () =>
                {
                    if (multiple > 0)
                    {
                        string content = string.Empty;
                        //积分商城积分等于上限值
                        if ((int)PlayerBaseData.GetInstance().IntergralMallTicket == MallNewUtility.GetIntergralMallTicketUpper() &&
                             MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsEqual == false)
                        {
                            content = TR.Value("mall_buy_intergral_mall_score_equal_upper_value_desc");

                            MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsEqual, () => { StartCoroutineAnsyCommonPurchase(iNeedCount, iLinkID, onClick); });
                        }
                        else
                        {
                            int ticketConvertScoreNumber = MallNewUtility.GetTicketConvertIntergalNumnber(iCostCount) * multiple;

                            int allIntergralScore = (int)PlayerBaseData.GetInstance().IntergralMallTicket + ticketConvertScoreNumber;

                            //购买道具后商城积分超出上限值
                            if (allIntergralScore > MallNewUtility.GetIntergralMallTicketUpper() &&
                                (int)PlayerBaseData.GetInstance().IntergralMallTicket != MallNewUtility.GetIntergralMallTicketUpper() &&
                                MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsExceed == false)
                            {
                                content = TR.Value("mall_buy_intergral_mall_score_exceed_upper_value_desc",
                                                   (int)PlayerBaseData.GetInstance().IntergralMallTicket,
                                                   MallNewUtility.GetIntergralMallTicketUpper(),
                                                   MallNewUtility.GetIntergralMallTicketUpper() - (int)PlayerBaseData.GetInstance().IntergralMallTicket);

                                MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsExceed, () => { StartCoroutineAnsyCommonPurchase(iNeedCount, iLinkID, onClick); });
                            }
                            else
                            {//未超出
                                StartCoroutineAnsyCommonPurchase(iNeedCount, iLinkID, onClick);
                            }
                        }
                    }
                    else
                    {
                        StartCoroutineAnsyCommonPurchase(iNeedCount, iLinkID, onClick);
                    }
                },
                "common_money_cost",
                ()=>
                {
                    if(onFailed != null)
                    {
                        onFailed.Invoke();
                    }
                });
            }
        }

        static void StartCoroutineAnsyCommonPurchase(int iNeedCount, int iLinkID, ComLinkFrame.OnClick onClick)
        {
            if (ms_co_purchase == null)
            {
                ms_co_purchase = GameFrameWork.instance.StartCoroutine(_AnsyCommonPurchase(iNeedCount, iLinkID, onClick));
            }
        }

        static IEnumerator _AnsyCommonPurchase(int iNeedCount, int iLinkID, ComLinkFrame.OnClick onClick)
        {
            MessageEvents events = new MessageEvents();

            SceneQuickBuyReq req = new SceneQuickBuyReq();
            SceneQuickBuyRes res = new SceneQuickBuyRes();

            req.type = (byte)QuickBuyTargetType.QUICK_BUY_ITEM;
            req.param1 = (ulong)iLinkID;
            req.param2 = (uint)iNeedCount;

            yield return MessageUtility.Wait<SceneQuickBuyReq, SceneQuickBuyRes>(ServerType.GATE_SERVER, events, req, res, true, 10);

            if (events.IsAllMessageReceived())
            {
                if (0 != res.result)
                {
                    SystemNotifyManager.SystemNotify((int)res.result);
                    ms_co_purchase = null;
                    yield break;
                }
                else
                {
                    if (onClick != null)
                    {
                        onClick.Invoke();
                    }
                    ms_co_purchase = null;
                    yield break;
                }
            }
            else
            {
                Logger.LogErrorFormat("快速购买失败!! 网络连接超时!!!");
            }

            ms_co_purchase = null;
        }

        public static bool IsLinkMoney(int iItemLinkID,ComLinkFrame.OnClick onClick = null, UnityEngine.Events.UnityAction onFailed = null)
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iItemLinkID);
            if(item == null)
            {
                return false;
            }

            if(item.Type != ProtoTable.ItemTable.eType.INCOME)
            {
                return false;
            }

            if(item.SubType == ProtoTable.ItemTable.eSubType.BindPOINT ||
                item.SubType == ProtoTable.ItemTable.eSubType.POINT)
            {
                SystemNotifyManager.SystemNotify(1207, () =>
                {
                    ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.PAY);
                    if(onFailed != null)
                    {
                        onFailed.Invoke();
                    }
                },
                ()=>
                {
                    if (onFailed != null)
                    {
                        onFailed.Invoke();
                    }
                });

                return true;
            }
            else if (item.SubType == ProtoTable.ItemTable.eSubType.GOLD)
            {
                SystemNotifyManager.SystemNotify(2302005);

                return true;
            }

            return false;
        }

        public static bool TryDirectlyJump2Link(int iItemLinkID)
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iItemLinkID);
            if(item != null)
            {
                if(item.bNeedJump == 1 && item.ComeLink.Count > 0)
                {
                    var linkItem = TableManager.GetInstance().GetTableItem<ProtoTable.AcquiredMethodTable>(item.ComeLink[0]);
                    if (linkItem != null && linkItem.IsLink != 0)
                    {
                        ActiveManager.GetInstance().OnClickLinkInfo(linkItem.LinkInfo);
                        return true;
                    }
                }
            }
            return false;
        }

        public static void OnLink(int iItemLinkID,int iNeedCount,bool bNotEnough = true, ComLinkFrame.OnClick onClick = null,bool bLinkMoney = false,bool bTryJumpDirectly = false,bool bIgnoreCommonPurchaseConfirm = false,
            UnityEngine.Events.UnityAction onFailed = null,string titleContent = "")
        {
            if (iNeedCount > 0 && bNotEnough == true)
            {
                if(bLinkMoney && IsLinkMoney(iItemLinkID, onClick))
                {
                    return;
                }

                if (LinkCommonPurchase(iNeedCount, iItemLinkID, onClick, bIgnoreCommonPurchaseConfirm, onFailed))
                {
                    return;
                }
            }

            if(bTryJumpDirectly)
            {
                if (TryDirectlyJump2Link(iItemLinkID))
                {
                    return;
                }
            }

            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iItemLinkID);
            if (item != null)
            {
                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)item.ID);
                if(itemData != null)
                {
                    ComLinkFrame.ComLinkFrameData data = new ComLinkFrame.ComLinkFrameData();
                    data.item = item;
                    data.bOrgLink = true;
                    data.bNotEnough = bNotEnough;
                    data.onClick = onClick;
                    data.titleContent = titleContent;
                    if (!bNotEnough)
                    {
						data.title = string.Format("{0} <color=#FFFFFFFF>获取途径</color>", itemData.GetColorName());
                    }
                    else
                    {
						data.title = string.Format("{0} <color=#FFFFFFFF>不足</color>", itemData.GetColorName());
                    }
                    GameClient.ClientSystemManager.GetInstance().OpenFrame<ComLinkFrame>(GameClient.FrameLayer.Middle, data);
                }
                return;
            }

            var linkItem = TableManager.GetInstance().GetTableItem<ProtoTable.AcquiredMethodTable>(iItemLinkID);
            if (linkItem != null)
            {
                ComLinkFrame.ComLinkFrameData data = new ComLinkFrame.ComLinkFrameData();
                data.item = null;
                data.bOrgLink = false;
                data.linkItem = linkItem;
                data.bNotEnough = bNotEnough;
                data.onClick = onClick;
                if (!bNotEnough)
                {
					data.title = string.Format("<color=#00FF1Eff>{0}</color> <color=#FFFFFFFF>获取途径</color>", linkItem.Name);
                }
                else
                {
					data.title = string.Format("<color=#FFFFFFFF>{0}不足</color>", linkItem.Name);
                }

                GameClient.ClientSystemManager.GetInstance().OpenFrame<ComLinkFrame>(GameClient.FrameLayer.Middle, data);
                return;
            }

            Logger.LogErrorFormat("can not find item which id is {0}!", iItemLinkID);
        }

        public void OnClickLink()
        {
            if (null != onEvent)
            {
                onEvent.Invoke();
            }
            OnLink(this.iItemLinkID,0, this.bNotEnough, 
                ()=>{
                    if(onClick != null)
                    {
                        onClick.Invoke();
                    }
                    if (comFrameBinder != null)
                    {
                        comFrameBinder.CloseFrame();
                    }
                },
                false,true);
        }
    }
}
