using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;
using Scripts.UI;
using GameClient;
using Protocol;

public enum WarriorRecruitTabType
{
    None = 0,
    companion,
    accept,
}

public class WarriorRecruit : MonoBehaviour
{
    public Text ActivityName;
    public Text ActivityTime;
    public Button btCopyInviteCode;
    public Text CoinNum;
    public Button btRecruitList;
    public Button btRecruitShop;
    public Toggle tgToggle1;
    public Toggle tgToggle2;
    public ComUIListScript list;
    public GameObject InvoiteCodeRoot;
    public InputField InputInvoiteCode;
    public InputField InvoiteCodeInputField;
    public Button BindInvoiteCode;
    public GameObject BindInvoiteCodeRoot;
    public GameObject RecruitRoot;
    public GameObject AcceptRecruitRoot;
    public ScrollRect Rect;
    public GameObject RecruitmentBonusPreview_CompanionRoot;
    public GameObject RecruitmentBonusPreview_AcceptRoot;
    public GameObject RecruitmentBonusPreview_ComItemPrefab;
    public GameObject RecruitmentBonusPreview_Companion_ItemParent;
    public GameObject RecruitmentBonusPreview_Accept_ItemParent;
    public GameObject BindSuccessedRoot;

    private List<WarriorRecruitTaskDataModel> targetTaskList = new List<WarriorRecruitTaskDataModel>();
    private string sInvoiteCode = string.Empty;
    private int warriorRecruitActiveID = 8800;
    private WarriorRecruitTabType warriorRecruitTabType = WarriorRecruitTabType.None;

    private bool RecruitmentBonusPreview_Companion_IsCreat = false;
    private bool RecruitmentBonusPreview_Accept_IsCreat = false;
    private void Awake()
    {
        _RegistUIEventHandle();
        _InitTaskUIScriptList();

        if (btCopyInviteCode != null)
        {
            btCopyInviteCode.onClick.AddListener(_OnClickCopyCode);
        }

        if (btRecruitList != null)
        {
            btRecruitList.onClick.AddListener(_OnClickRecruitList);
        }

        if (btRecruitShop != null)
        {
            btRecruitShop.onClick.AddListener(_OnClickRecruitShop);
        }

        if (tgToggle1 != null)
        {
            tgToggle1.onValueChanged.AddListener(_OnClickToggle1);
        }

        if (tgToggle2 != null)
        {
            tgToggle2.onValueChanged.AddListener(_OnClickToggle2);
        }

        if (InputInvoiteCode != null)
        {
            InputInvoiteCode.onValueChanged.AddListener(_OnInputInvoiteCodeClick);
        }

        if (InvoiteCodeInputField != null)
        {
            InvoiteCodeInputField.onValueChanged.AddListener(_OnInvoiteCodeClick);
        }

        if (BindInvoiteCode != null)
        {
            BindInvoiteCode.onClick.AddListener(_OnBindInvoiteCodeClick);
        }

        _Init();
    }

    private void OnDestroy()
    {
        _UnRegistUIEventHandle();
        _UnInitTaskUIScriptList();

        if (btCopyInviteCode != null)
        {
            btCopyInviteCode.onClick.RemoveAllListeners();
        }

        if (btRecruitList != null)
        {
            btRecruitList.onClick.RemoveAllListeners();
        }

        if (btRecruitShop != null)
        {
            btRecruitShop.onClick.RemoveAllListeners();
        }

        if (tgToggle1 != null)
        {
            tgToggle1.onValueChanged.RemoveAllListeners();
        }

        if (tgToggle2 != null)
        {
            tgToggle2.onValueChanged.RemoveAllListeners();
        }

        if (targetTaskList != null)
        {
            targetTaskList.Clear();
        }

        if (InputInvoiteCode != null)
        {
            InputInvoiteCode.onValueChanged.RemoveAllListeners();
        }

        if (InvoiteCodeInputField != null)
        {
            InvoiteCodeInputField.onValueChanged.RemoveAllListeners();
        }

        if (BindInvoiteCode != null)
        {
            BindInvoiteCode.onClick.RemoveAllListeners();
        }

        sInvoiteCode = string.Empty;
        warriorRecruitTabType = WarriorRecruitTabType.None;
        RecruitmentBonusPreview_Companion_IsCreat = false;
        RecruitmentBonusPreview_Accept_IsCreat = false;
    }

    #region BindUIEvent

    private void _RegistUIEventHandle()
    {
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WarriorRecruitQueryTaskSuccessed, _WarriorRecruitQueryTaskSuccessed);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WarriorRecruitQueryIdentitySuccessed, _WarriorRecruitQueryIdentitySuccessed);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WarriorRecruitBindInviteCodeSuccessed, _WarriorRecruitBindInviteCodeSuccessed);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WarriorRecruitReceiveRewardSuccessed, _WarriorRecruitReceiveRewardSuccessed);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WarriorRecruitQueryHireAlreadyBindSuccessed, _WarriorRecruitQueryHireAlreadyBindSuccessed);
    }

    private void _UnRegistUIEventHandle()
    {
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AccountSpecialItemUpdate, _OnCountValueChanged);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WarriorRecruitQueryTaskSuccessed, _WarriorRecruitQueryTaskSuccessed);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WarriorRecruitQueryIdentitySuccessed, _WarriorRecruitQueryIdentitySuccessed);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WarriorRecruitBindInviteCodeSuccessed, _WarriorRecruitBindInviteCodeSuccessed);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WarriorRecruitReceiveRewardSuccessed, _WarriorRecruitReceiveRewardSuccessed);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WarriorRecruitQueryHireAlreadyBindSuccessed, _WarriorRecruitQueryHireAlreadyBindSuccessed);
    }

    private void _OnCountValueChanged(UIEvent uiEvent)
    {
        _RefreshWarriorRecruitCoin();
    }

    /// <summary>
    /// 查询任务成功
    /// </summary>
    /// <param name="uiEvent"></param>
    private void _WarriorRecruitQueryTaskSuccessed(UIEvent uiEvent)
    {
        //查询身份
        WarriorRecruitDataManager.GetInstance().SendHireInfoReq();
    }

    /// <summary>
    /// 查询身份成功
    /// </summary>
    /// <param name="uiEvent"></param>
    private void _WarriorRecruitQueryIdentitySuccessed(UIEvent uiEvent)
    {
        if (warriorRecruitTabType != WarriorRecruitTabType.None)
        {
            RefreshTaskState();
        }
        else
        {
            //如果身份为新玩家，请求在其他服有没有绑定
            if ((WarriorRecruitDataManager.identify & (int)RecruitIdentify.RI_NEWBIE) == (int)RecruitIdentify.RI_NEWBIE)
            {
                WarriorRecruitDataManager.GetInstance().SendWorldQueryHireAlreadyBindReq();
            }
            else
            {
                _RefreshAcceptRecruitmentToggle();
            }
           
            _InitInterface();
            // 默认选中第一个页签
            if (tgToggle1 != null)
            {
                tgToggle1.isOn = true;
            }
        }
    }

    private void _WarriorRecruitBindInviteCodeSuccessed(UIEvent uiEvent)
    {
        if (BindInvoiteCodeRoot != null)
        {
            BindInvoiteCodeRoot.CustomActive(false);
        }

        if (BindSuccessedRoot != null)
        {
            BindSuccessedRoot.CustomActive(true);
        }

        WarriorRecruitDataManager.GetInstance().SendQueryTaskStatusReq();
    }

    private void _WarriorRecruitReceiveRewardSuccessed(UIEvent uiEvent)
    {
        RefreshTaskState();
    }

    private void _WarriorRecruitQueryHireAlreadyBindSuccessed(UIEvent uiEvent)
    {
        _RefreshAcceptRecruitmentToggle();
    }

    private void RefreshTaskState()
    {
        if (warriorRecruitTabType == WarriorRecruitTabType.companion)
        {
            _UpdateCompanionData();
        }
        else
        {
            if (WarriorRecruitDataManager.isBindInviteCode)
            {
                RecruitmentBonusPreview_AcceptRoot.CustomActive(false);

                BindSuccessedRoot.CustomActive(true);

                _UpdateAcceptData();
            }
            else
            {
                _CreatAcceptBonusPreview();
            }
        }
    }
    #endregion

    #region taskUIlist

    private void _InitTaskUIScriptList()
    {
        if (list != null)
        {
            list.Initialize();
            list.onBindItem += _OnBindItemDelegate;
            list.onItemVisiable += _OnItemVisiableDelegate;
        }
    }

    private void _UnInitTaskUIScriptList()
    {
        if (list != null)
        {
            list.onBindItem -= _OnBindItemDelegate;
            list.onItemVisiable -= _OnItemVisiableDelegate;
        }
    }

    private WarriorRecruitItem _OnBindItemDelegate(GameObject itemObject)
    {
        return itemObject.GetComponent<WarriorRecruitItem>();
    }

    private void _OnItemVisiableDelegate(ComUIListElementScript item)
    {
        var warriorRecruitItem = item.gameObjectBindScript as WarriorRecruitItem;
        if (warriorRecruitItem != null && item.m_index >= 0 && item.m_index < targetTaskList.Count)
        {
            warriorRecruitItem.OnItemVisiable(targetTaskList[item.m_index],item.m_index);
        }
    }

    #endregion

    private void _Init()
    {
        
        if (CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_HIRE_RED_POINT) <= 0)
        {
            WarriorRecruitDataManager.GetInstance().SendWorldQueryHireRedPointReq();//招募红点
        }

        WarriorRecruitDataManager.GetInstance().SendQueryTaskStatusReq();

        if (ActivityName != null)
        {
            //ActivityName.text = ;
        }

        if (ActiveManager.GetInstance().allActivities.ContainsKey(warriorRecruitActiveID))
        {
            var info = ActiveManager.GetInstance().allActivities[warriorRecruitActiveID];
            if (info == null)
            {
                return;
            }

            if (ActivityTime != null)
            {
                ActivityTime.text = (string.Format("{0}", Function.GetTimeWithoutYearNoZero((int)info.startTime, (int)info.dueTime)));
            }
        }
    }

    private void SetScrollRectValue()
    {
        if (Rect != null)
        {
            Rect.verticalNormalizedPosition = 1;
        }
    }

    private void _RefreshItemListCount()
    {
        if (list != null)
        {
            list.SetElementAmount(targetTaskList.Count);
        }
    }

    private void _OnClickCopyCode()
    {

    }

    /// <summary>
    /// 招募玩家
    /// </summary>
    private void _OnClickRecruitList()
    {
        WarriorRecruitDataManager.GetInstance().SendQueryHireListReq();
    }

    private void _OnClickRecruitShop()
    {
        AccountShopDataManager.GetInstance().OpenAccountShop(34);
    }

    private void _OnClickToggle1(bool value)
    {
        if (!value)
        {
            return;
        }

        RecruitRoot.CustomActive(value);
        AcceptRecruitRoot.CustomActive(!value);
        btRecruitList.CustomActive(value);

        RecruitmentBonusPreview_CompanionRoot.CustomActive(false);
        RecruitmentBonusPreview_AcceptRoot.CustomActive(false);
        BindSuccessedRoot.CustomActive(false);
        list.CustomActive(false);

        _RefreshInvoiteCodeRoot();

        warriorRecruitTabType = WarriorRecruitTabType.companion;

        if (WarriorRecruitDataManager.isOtherBindMe)
        {
            _UpdateCompanionData();
            SetScrollRectValue();
        }
        else
        {
            _CreatCompanionBonusPreview();
        }
    }

    private void _OnClickToggle2(bool value)
    {
        if (!value)
        {
            return;
        }

        RecruitRoot.CustomActive(!value);
        AcceptRecruitRoot.CustomActive(value);
        btRecruitList.CustomActive(!value);

        RecruitmentBonusPreview_CompanionRoot.CustomActive(false);
        RecruitmentBonusPreview_AcceptRoot.CustomActive(false);
        BindSuccessedRoot.CustomActive(false);
        list.CustomActive(false);

        warriorRecruitTabType = WarriorRecruitTabType.accept;

        if (WarriorRecruitDataManager.isBindInviteCode)
        {
            BindSuccessedRoot.CustomActive(true);

            _UpdateAcceptData();
            SetScrollRectValue();
        }
        else
        {
            _CreatAcceptBonusPreview();
        }
    }

    private void _CreatCompanionBonusPreview()
    {
        if (RecruitmentBonusPreview_Companion_IsCreat == false)
        {
            RecruitmentBonusPreview_Companion_IsCreat = true;

            for (int i = 0; i < WarriorRecruitDataManager.GetInstance().mRecruitmentBonusPreview_OldPlayerList.Count; i++)
            {
                int itemId = WarriorRecruitDataManager.GetInstance().mRecruitmentBonusPreview_OldPlayerList[i];
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemId); 
                if (itemData == null)
                {
                    continue;
                }

                GameObject comItem = GameObject.Instantiate(RecruitmentBonusPreview_ComItemPrefab);
                if (comItem != null)
                {
                    Utility.AttachTo(comItem, RecruitmentBonusPreview_Companion_ItemParent);
                    ComCommonBind mBind = comItem.GetComponent<ComCommonBind>();
                    if (mBind != null)
                    {
                        Button iconBtn = mBind.GetCom<Button>("Iconbtn");
                        Image icon = mBind.GetCom<Image>("Icon");
                        Image background = mBind.GetCom<Image>("backgroud");

                        if (iconBtn != null)
                        {
                            iconBtn.onClick.RemoveAllListeners();
                            iconBtn.onClick.AddListener(() => { ItemTipManager.GetInstance().ShowTip(itemData); });
                        }

                        if (background != null)
                        {
                            ETCImageLoader.LoadSprite(ref background, itemData.GetQualityInfo().Background);
                        }

                        if (icon != null)
                        {
                            ETCImageLoader.LoadSprite(ref icon, itemData.Icon);
                        }
                    }

                    comItem.CustomActive(true);
                }
            }
        }

        RecruitmentBonusPreview_CompanionRoot.CustomActive(true);
    }

    private void _CreatAcceptBonusPreview()
    {
        if (RecruitmentBonusPreview_Accept_IsCreat == false)
        {
            RecruitmentBonusPreview_Accept_IsCreat = true;

            for (int i = 0; i < WarriorRecruitDataManager.GetInstance().mRecruitmentBonusPreview_NewPlayerList.Count; i++)
            {
                int itemId = WarriorRecruitDataManager.GetInstance().mRecruitmentBonusPreview_NewPlayerList[i];
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemId);
                if (itemData == null)
                {
                    continue;
                }

                GameObject comItem = GameObject.Instantiate(RecruitmentBonusPreview_ComItemPrefab);
                if (comItem != null)
                {
                    Utility.AttachTo(comItem, RecruitmentBonusPreview_Accept_ItemParent);
                    ComCommonBind mBind = comItem.GetComponent<ComCommonBind>();
                    if (mBind != null)
                    {
                        Button iconBtn = mBind.GetCom<Button>("Iconbtn");
                        Image icon = mBind.GetCom<Image>("Icon");
                        Image background = mBind.GetCom<Image>("backgroud");

                        if (iconBtn != null)
                        {
                            iconBtn.onClick.RemoveAllListeners();
                            iconBtn.onClick.AddListener(() => { ItemTipManager.GetInstance().ShowTip(itemData); });
                        }

                        if (background != null)
                        {
                            ETCImageLoader.LoadSprite(ref background, itemData.GetQualityInfo().Background);
                        }

                        if (icon != null)
                        {
                            ETCImageLoader.LoadSprite(ref icon, itemData.Icon);
                        }
                    }

                    comItem.CustomActive(true);
                }
            }
        }

        RecruitmentBonusPreview_AcceptRoot.CustomActive(true);
    }

    private void _UpdateCompanionData()
    {
        list.CustomActive(true);

        targetTaskList.Clear();
        targetTaskList.AddRange(WarriorRecruitDataManager.GetInstance().mRecruitCompanionsTaskList);
        targetTaskList.Sort(Cmp);

        _RefreshItemListCount();
    }

    private void _UpdateAcceptData()
    {
        list.CustomActive(true);

        targetTaskList.Clear();

        //新玩家
        if ((WarriorRecruitDataManager.identify & (int)RecruitIdentify.RI_NEWBIE) != 0)
        {
            targetTaskList = WarriorRecruitDataManager.GetInstance().FilterRecruiIdentifyTask((int)RecruitIdentify.RI_NEWBIE);
        }//回归玩家
        else if ((WarriorRecruitDataManager.identify & (int)RecruitIdentify.RI_RETURNMAN) != 0)
        {
            targetTaskList = WarriorRecruitDataManager.GetInstance().FilterRecruiIdentifyTask((int)RecruitIdentify.RI_RETURNMAN);
        }

        targetTaskList.Sort(Cmp);

        _RefreshItemListCount();
    }

    /// <summary>
    /// 刷新接受招募页签
    /// </summary>
    private void _RefreshAcceptRecruitmentToggle()
    {
        bool isShow = WarriorRecruitDataManager.GetInstance().IsAcceptRecruitTabShow();

        tgToggle2.CustomActive(isShow);
    }

    /// <summary>
    /// 刷新我的邀请码是否显示
    /// </summary>
    private void _RefreshInvoiteCodeRoot()
    {
        if (InvoiteCodeRoot != null)
        {
            InvoiteCodeRoot.CustomActive((WarriorRecruitDataManager.identify & (int)RecruitIdentify.RI_OLDMAN) != 0);
        }
    }

    private void _InitInterface()
    {
        if (InvoiteCodeInputField != null)
        {
            InvoiteCodeInputField.text = WarriorRecruitDataManager.inviteCode;
        }

        if (BindInvoiteCodeRoot != null)
        {
            BindInvoiteCodeRoot.CustomActive(!WarriorRecruitDataManager.isBindInviteCode);
        }
        
        _RefreshWarriorRecruitCoin();
    }

    private void _OnInputInvoiteCodeClick(string value)
    {
        sInvoiteCode = value;
    }

    private void _OnInvoiteCodeClick(string value)
    {
        InvoiteCodeInputField.text = WarriorRecruitDataManager.inviteCode;
    }

    private void _OnBindInvoiteCodeClick()
    {
        if (sInvoiteCode == string.Empty)
        {
            SystemNotifyManager.SysNotifyTextAnimation("请输入邀请码");
            return;
        }

        //str.Replace("\n", "").Replace(" ","").Replace("\t","").Replace("\r","");
        sInvoiteCode = sInvoiteCode.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");

        //不能绑定自己的邀请码
        if (sInvoiteCode == WarriorRecruitDataManager.inviteCode)
        {
            SystemNotifyManager.SysNotifyTextAnimation("不能邀请自己");
            return;
        }

        WarriorRecruitDataManager.GetInstance().SendUseHireCodeReq(sInvoiteCode);
    }

    private void _RefreshWarriorRecruitCoin()
    {
        if (CoinNum != null)
        {
            CoinNum.text = AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_COUNTER_HIRE_COIN).ToString();
        }
    }

    private int Cmp(WarriorRecruitTaskDataModel left , WarriorRecruitTaskDataModel right)
    {
        if (left.state != right.state)
        {
            return ActiveItemObject.ms_sort_order[left.state] - ActiveItemObject.ms_sort_order[right.state];
        }

        return left.taskId - right.taskId;
    } 
}