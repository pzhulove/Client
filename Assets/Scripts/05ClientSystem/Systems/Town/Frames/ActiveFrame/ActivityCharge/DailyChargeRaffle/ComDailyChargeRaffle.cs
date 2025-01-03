using GameClient;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComDailyChargeRaffle : MonoBehaviour 
{
    public enum DailyChargeTaskStatus
    {
        ToCharge,
        BeCharged,
    }

    #region Model Params

    private List<DailyChargeRaffleModel> mTaskModelList = new List<DailyChargeRaffleModel>();
    private int mFashionId = 0;

    #endregion
    
    #region  View Params

    private ComCommonBind mBind = null;
    private Text mTimeHint = null;
    private Text mHint0 = null;
    private Button mGoToTurnTableBtn = null;
    private Button mPreViewModelBtn = null;
    private ComUIListScript mScrollViewList = null;

    private List<DailyChargeRaffleTaskItem> mTaskItemList = new List<DailyChargeRaffleTaskItem>();

    private string toPayBtnTextDesc;

    #endregion
    
    #region PRIVATE METHODS
    
    void Awake()
    {
        
    }

    void OnEnable()
    {
        PluginManager.GetInstance().TryGetIOSAppstoreProductIds();
        //当界面显示时  重置红点  
        DailyChargeRaffleDataManager.GetInstance().ResetRedPoint();
        GameStatisticManager.GetInstance().DoStartFuilDailChargeRaffle();
    }
    
	void Start () 
    {
        InitData();
        InitTRDesc();
        InitView();
        BindEvent();
	}
	
	void Update () 
    {
		
	}

    void OnDestroy()
    {
        Clear();
        UnBindEvent();
    }

    void BindEvent()
    {
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DailyChargeResultNotify, OnDailyChargeResultNotify);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DailyChargeCounterChanged, _DailyChargeCounterChanged);
    }

    void UnBindEvent()
    {
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DailyChargeResultNotify, OnDailyChargeResultNotify);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DailyChargeCounterChanged, _DailyChargeCounterChanged);
    }

    void OnDailyChargeResultNotify(UIEvent _uiEvent)
    {
        if (_uiEvent == null)
        {
            return;
        }
        try
        {
            int taskId = (int)_uiEvent.Param1;
            int status = (int)_uiEvent.Param2;

            DailyChargeTaskStatus taskStatus = GetTaskStatus(status);
            bool bFind = false;

            if (mTaskItemList == null)
            {
                return;
            }
            for (int i = 0; i < mTaskItemList.Count; i++)
            {
                if (mTaskItemList[i].model != null && mTaskItemList[i].model.Id == taskId)
                {
                    mTaskItemList[i].SetTaskItemStatus(taskStatus);
                    bFind = true;
                    
                    //打开对应抽奖券的抽奖转盘
                    DailyChargeRaffleDataManager.GetInstance().OpenRaffleTurnTableFrame(mTaskItemList[i].model.RaffleTableId);

                    //请求刷新counter
                    DailyChargeRaffleDataManager.GetInstance().ReqDailyChargeCounter(taskId);
                }
            }
            if (bFind)
            {
                GameClient.ActiveManager.GetInstance().SendSevenDayTimeReq();
            }
        }
        catch (System.Exception e)
        {
            Logger.LogError("catch a convert error !");
        }
    }

    void _DailyChargeCounterChanged(UIEvent uIEvent)
    {
        if (null == uIEvent || null == uIEvent.Param1)
        {
            return;
        }
        var counter = uIEvent.Param1 as DailyChargeCounter;
        if (counter == null || counter.activityCounter == null)
        {
            return;
        }
        if (mTaskModelList == null)
        {
            return;
        }
        for (int i = 0; i < mTaskModelList.Count; i++)
        {
            var taskModel = mTaskModelList[i];
            if (taskModel == null)
                continue;
            if (taskModel.Id == counter.dailyChargeActivityId)
            {
                if (counter.activityCounter.CounterId != (uint)ActivityLimitTimeFactory.EActivityCounterType.QAT_SUMMER_DAILY_CHARGE)
                {
                    continue;
                }
                taskModel.accLimitChargeNum = (int)counter.activityCounter.CounterValue;
            }
        }
    }

    DailyChargeTaskStatus GetTaskStatus(int status)
    {
        DailyChargeTaskStatus taskStatus = DailyChargeTaskStatus.ToCharge;

        if (status <= (int)Protocol.TaskStatus.TASK_UNFINISH)
        {
            taskStatus = DailyChargeTaskStatus.ToCharge;
        }
        else if (status == (int)Protocol.TaskStatus.TASK_OVER)
        {
            taskStatus = DailyChargeTaskStatus.BeCharged;
        }
        return taskStatus;
    }


    void InitData()
    {
        mTaskModelList = DailyChargeRaffleDataManager.GetInstance().GetDailyChargeModels();

        if (mTaskModelList != null)
        {
            for (int i = 0; i < mTaskModelList.Count; i++)
            {
                var taskModel = mTaskModelList[i];
                if (taskModel == null)
                    continue;
                DailyChargeRaffleDataManager.GetInstance().ReqDailyChargeCounter(taskModel.Id);
            }
        }

        var systemValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DAILYGIFT_PREVIEW_ITEM_ID);
        if (systemValueTable != null)
        {
            mFashionId = systemValueTable.Value;
        }
    }

    void InitTRDesc()
    {
        toPayBtnTextDesc = TR.Value("daily_charge_raffle_button_desc");
    }

    void InitView()
    {
        mBind = this.gameObject.GetComponent<ComCommonBind>();
        if (mBind != null)
        {
            mTimeHint = mBind.GetCom<Text>("TimeHint");
            mHint0 = mBind.GetCom<Text>("Hint0");
            mGoToTurnTableBtn = mBind.GetCom<Button>("GoToTurnTable");
            if (mGoToTurnTableBtn)
            {
                mGoToTurnTableBtn.onClick.AddListener(OnGoToTurnTableClick);
            }
            mScrollViewList = mBind.GetCom<ComUIListScript>("ScrollViewList");

            mPreViewModelBtn = mBind.GetCom<Button>("preViewModel");
            if (mPreViewModelBtn != null)
            {
                mPreViewModelBtn.onClick.RemoveAllListeners();
                mPreViewModelBtn.onClick.AddListener(OnPreViewModelClick);
            }
        }
        InitComUIList();
    }

    void Clear()
    {
        mBind = null;
        mTimeHint = null;
        mHint0 = null;
        if (mGoToTurnTableBtn)
        {
            mGoToTurnTableBtn.onClick.RemoveListener(OnGoToTurnTableClick);
        }
        mGoToTurnTableBtn = null;

        if (mScrollViewList != null)
        {
            mScrollViewList.SetElementAmount(0);
            mScrollViewList.UnInitialize();
        }
        mScrollViewList = null;

        if (mTaskItemList != null)
        {
            mTaskItemList.Clear();
        }

        if (mTaskModelList != null)
        {
            for (int i = 0; i < mTaskModelList.Count; i++)
            {
                mTaskModelList[i].Clear();
            }
            mTaskModelList.Clear();
        }

        if (mPreViewModelBtn != null)
        {
            mPreViewModelBtn.onClick.RemoveListener(OnPreViewModelClick);
        }
        mPreViewModelBtn = null;
    }

    void OnGoToTurnTableClick()
    {
        DailyChargeRaffleDataManager.GetInstance().OpenRaffleTurnTableFrame(mTaskModelList);
    }

    void OnPreViewModelClick()
    {
        PreViewDataModel data = new PreViewDataModel();
        data.preViewItemList = new List<PreViewItemData>();

        var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(mFashionId);
        if (itemTable == null)
        {
            return;
        }
        else
        {
            if (itemTable.SubType == ItemTable.eSubType.GiftPackage)
            {
                var giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(itemTable.PackageID);
                if (giftPackTable == null)
                {
                    return;
                }

                var giftList = TableManager.GetInstance().GetGiftTableData(giftPackTable.ID);

                for (int i = 0; i < giftList.Count; i++)
                {
                    var giftTableData = giftList[i];
                    if (giftTableData == null)
                    {
                        continue;
                    }

                    if (!giftTableData.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                    {
                        continue;
                    }

                    PreViewItemData itemData = new PreViewItemData();
                    itemData.itemId = giftTableData.ItemID;

                    data.preViewItemList.Add(itemData);
                }
            }
        }
        
        ClientSystemManager.GetInstance().OpenFrame<PreviewModelFrame>(FrameLayer.Middle, data);
    }

    void InitComUIList()
    {
        if(mTaskModelList == null || mTaskModelList.Count == 0)
        {
            return;
        }
        if (mScrollViewList == null)
        {
            return;
        }
        if (!mScrollViewList.IsInitialised())
        {
            mScrollViewList.Initialize();
            mScrollViewList.onBindItem = (go) =>
            {
                if (go != null)
                {
                    return go.GetComponent<DailyChargeRaffleTaskItem>();
                }
                return null;
            };
        }
        mScrollViewList.onItemVisiable = (var) =>
        {
            if (var == null)
            {
                return;
            }

            int iIndex = var.m_index;
            if (iIndex >= 0 && iIndex < mTaskModelList.Count)
            {
                DailyChargeRaffleTaskItem item = var.gameObjectBindScript as DailyChargeRaffleTaskItem;
                if (item != null)
                {
                    item.model = mTaskModelList[iIndex];
                    item.Initialize();
                    item.SetToPayBtnText(string.Format(toPayBtnTextDesc,mTaskModelList[iIndex].ChargePrice));
                    int status = (int)mTaskModelList[iIndex].Status;
                    item.SetTaskItemStatus(GetTaskStatus(status));

                    if (mTaskItemList != null && !mTaskItemList.Contains(item))
                    {
                        mTaskItemList.Add(item);
                    }
                }
            }
        };
        mScrollViewList.SetElementAmount(mTaskModelList.Count);
        mScrollViewList.ResetContentPosition();
    }

    #endregion
    
    #region  PUBLIC METHODS
    #endregion
}
