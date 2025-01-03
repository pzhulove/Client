using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameClient;

public class DailyChargeRaffleTaskItem : MonoBehaviour 
{
    #region Model Params
    public DailyChargeRaffleModel model { get; set; }
    private List<ItemSimpleData> itemDatas = new List<ItemSimpleData>();

    #endregion
    
    #region  View Params
    [SerializeField]
    [Header("不同档位的抽奖券的ID集合")]
    private List<int> mRaffleTicketIds;
    [SerializeField]
    [Header("特效挂载偏移1单位值")]
    private int mFocusItemEffectOffset = 86;

    public Button mToPayBtn;
    public Text mToPayBtnText;
    //public GameObject mPayGotFlag;
    public Button mToRaffleBtn;
    public Text mToRaffleBtnText;
    public GameObject mAwardItemRoot;
    public GameObject mAddImgGo;
    public GameObject mFocusItemEffect;

    private List<ComItem> comItemList = new List<ComItem>();

    private string toRaffleTurnTableDesc = "";
    private string tr_charge_acc_limit_desc = "";

    private bool bInited = false;

    #endregion
    
    #region PRIVATE METHODS
    
    void Awake()
    {
        
    }
    
	void Start () 
    {
        if (mToPayBtn)
        {
            mToPayBtn.onClick.AddListener(OnToPayBtnClick);
        }

        if (mToRaffleBtn)
        {
            mToRaffleBtn.onClick.AddListener(OnToRaffleBtnClick);
        }

        toRaffleTurnTableDesc = TR.Value("daily_charge_raffle_button_goto_turntable");
        tr_charge_acc_limit_desc = TR.Value("daily_charge_raffle_acc_limit");
        InitRaffleBtnText();
	}
	
	void Update () 
    {
		
	}

    void OnDestroy()
    {
        if (model != null)
        {
            model.Clear();
            model = null;
        }
        if (itemDatas != null)
        {
            itemDatas.Clear();
        }

        if (comItemList != null)
        {
            for (int i = 0; i < comItemList.Count; i++)
            {
                ComItemManager.Destroy(comItemList[i]);
            }
            comItemList.Clear();
        }

        if (mToPayBtn)
        {
            mToPayBtn.onClick.RemoveListener(OnToPayBtnClick);
        }
        mToPayBtn = null;
        mToPayBtnText = null;
        //mPayGotFlag = null;
        if (mToRaffleBtn)
        {
            mToRaffleBtn.onClick.RemoveListener(OnToRaffleBtnClick);
        }
        mToRaffleBtn = null;
        mToRaffleBtnText = null;
        mAwardItemRoot = null;
        mAddImgGo = null;
		mFocusItemEffect = null;

        if (mRaffleTicketIds != null)
        {
            mRaffleTicketIds.Clear();
        }

        toRaffleTurnTableDesc = "";

        bInited = false;
    }

    void OnToPayBtnClick()
    {
        if (model == null)
        {
            Logger.LogError("Try to pay failed, model is null !");
            return;
        }

        if (model.accLimitChargeNum >= model.accLimitChargeMax)
        {
            SystemNotifyManager.SysNotifyTextAnimation(string.Format(tr_charge_acc_limit_desc, model.accLimitChargeMax));
            return;
        }

        DailyChargeRaffleDataManager.GetInstance().SendBuyDailyChargeReq(model);
    }

    void OnToRaffleBtnClick()
    {
        if (model == null)
        {
            Logger.LogError("Try to open turntable failed, model is null !");
            return;
        }

        DailyChargeRaffleDataManager.GetInstance().OpenRaffleTurnTableFrame(model.RaffleTableId);
    }

    void SetPayGotShow(bool bShow)
    {
        //if (mPayGotFlag)
        //{
        //    mPayGotFlag.CustomActive(bShow);
        //}
    }

    void SetRaffleBtnShow(bool bShow)
    {
        if (mToRaffleBtn)
        {
            mToRaffleBtn.gameObject.CustomActive(bShow);
        }
    }

    void SetToPayBtnShow(bool bShow)
    {
        if (mToPayBtn)
        {
            mToPayBtn.gameObject.CustomActive(bShow);
        }
    }

    void InitRaffleBtnText()
    {
        if (mToRaffleBtnText)
        {
            mToRaffleBtnText.text = toRaffleTurnTableDesc;
        }
    }
    
    #endregion
    
    #region  PUBLIC METHODS

    public void Initialize()
    {
        if (model == null)
        {
            return;
        }
        if (bInited)
        {
            return;
        }

        itemDatas = model.AwardItemDataList;
        if (itemDatas != null)
        {
            for (int i = 0; i < itemDatas.Count; i++)
            {
                ComItem comItem = ComItemManager.Create(mAwardItemRoot);
                comItem.CustomActive(true);
                var awardItemData = ItemDataManager.CreateItemDataFromTable(itemDatas[i].ItemID);
                if (awardItemData == null)
                {
                    Logger.LogErrorFormat("Please check item data id {0}", itemDatas[i].ItemID);
                    continue;
                }
                awardItemData.Count = itemDatas[i].Count;
                comItem.Setup(awardItemData, (go, itemdata) => {
                    ItemTipManager.GetInstance().ShowTip(itemdata);
                });

                if (comItem != null && !comItemList.Contains(comItem))
                {
                    comItemList.Add(comItem);
                }
            }
        }

        //set sibling
        if (mAddImgGo)
        {
            mAddImgGo.transform.SetAsLastSibling();
        }

        if (comItemList != null && mRaffleTicketIds != null)
        {
            for (int i = comItemList.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < mRaffleTicketIds.Count; j++)
                {
                    if (comItemList[i].ItemData.TableID == mRaffleTicketIds[j])
                    {
                        comItemList[i].gameObject.transform.SetAsLastSibling();
                        var comItem = comItemList[i];
                        var itemData = comItem.ItemData;
                        comItem.Setup(itemData, (go, data) =>
                        {
                            DailyChargeRaffleDataManager.GetInstance().OpenRaffleTurnTableFrame(model.RaffleTableId);
                        });
                        if (mFocusItemEffect)
                        {
                            var effectRect = mFocusItemEffect.GetComponent<RectTransform>();
                            if(effectRect == null)
                            {
                                return;
                            }
                            effectRect.anchoredPosition = new Vector2(mFocusItemEffectOffset, effectRect.anchoredPosition.y);
                        }
                        break;
                    }
                }
            }
        }

        bInited = true;
    }

    public void SetToPayBtnText(string desc)
    {
        if (mToPayBtnText)
        {
            mToPayBtnText.text = desc;
        }
    }

    public void SetTaskItemStatus(ComDailyChargeRaffle.DailyChargeTaskStatus status)
    {
        switch (status)
        {
            case ComDailyChargeRaffle.DailyChargeTaskStatus.ToCharge:
                SetToPayBtnShow(true);
                //SetPayGotShow(false);
                SetRaffleBtnShow(false);
                break;
            case ComDailyChargeRaffle.DailyChargeTaskStatus.BeCharged:
                SetToPayBtnShow(false);
                //SetPayGotShow(true);
                SetRaffleBtnShow(true);
                break;
        }
    }
    #endregion
}
