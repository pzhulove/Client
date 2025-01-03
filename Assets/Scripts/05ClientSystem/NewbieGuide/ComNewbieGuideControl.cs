using UnityEngine;
using GameClient;
using System.Collections.Generic;
using UnityEngine.UI;
using ProtoTable;

[LoggerModel("NewbieGuide")]
public class ComNewbieGuideControl : MonoBehaviour
{
    const int ParamNum = 3;

    public enum ControlState
    {
        None,
        Guiding,
        Finish,
    }

    // 管理器
    protected NewbieGuideManager mGuideManager;

    // 当前引导控件
    private ComNewbieGuideBase mCurrentCom;

    // 当前引导数据
    protected NewbieGuideTable.eNewbieGuideTask mCurGuideTaskID = NewbieGuideTable.eNewbieGuideTask.None;   
    protected ControlState mCurState = ControlState.None;
    private int mCurrentIndex = 0;
    private NewbieGuideDataUnit mCurGuideUnit;

    public ComNewbieGuideControl()
    {
        GuideTaskID = NewbieGuideTable.eNewbieGuideTask.None;
        mCurState = ControlState.None;
        currentIndex = 0;
        mCurGuideUnit = null;
    }

    public void ClearData()
    {
        mCurGuideTaskID = NewbieGuideTable.eNewbieGuideTask.None;
        mCurState = ControlState.None;
        mCurrentIndex = 0;

        if(mCurGuideUnit != null)
        {
            mCurGuideUnit.ClearData();
        }

        if(mCurrentCom != null)
        {
            mCurrentCom.ClearData();
        }
    }

    void Start()
    {
        _start();
    }

    private void _start()
    {
        var list = mCurGuideUnit.newbieComList;
        if (list == null)
        {
            _DealFinishProcess();
        }
        else
        {
            curState = ControlState.Guiding;
            currentIndex = mCurGuideUnit.savePoint;

            _checkNext();
        }
    }

    private bool _bNeedCheckNext = false;
    private float fCheckTime = 0.0f;

    private bool needCheckNext
    {
        get { return _bNeedCheckNext; }
        set
        {
            if (_bNeedCheckNext != value)
            {
                //Reset Check Time;
                fCheckTime = 0.0f;
            }

            _bNeedCheckNext = value;
        }
    }

    private void PerformCheckNext()
    {
        if (needCheckNext)
        {
            fCheckTime += Time.deltaTime;

            if (fCheckTime <= 5.0f)
            {
                _checkNext();
            }
            else
            {
                needCheckNext = false;
                ControlException();
            }
        }
    }

    private bool _checkNext()
    {
        var list = mCurGuideUnit.newbieComList;

        if (list != null && currentIndex < list.Count)
        {
            Logger.LogProcessFormat("start with index {0}", currentIndex);

            ComNewbieData type = list[currentIndex];

            if(!CheckModifyData(ref type))
            {
                needCheckNext = true;
                return false;
            }

            needCheckNext = false;

            _DealGuidingProcess(type);

            // 新手引导埋点
            //GameStatisticManager.GetInstance().DoStatNewBieGuide(GuideTaskID.ToString(), currentIndex);

            return true;
        }
        else
        {
            Logger.LogProcessFormat("finish all");
            _DealFinishProcess();
        }

        // TODO Time out skip
        return false;
    }

    public void ControlComplete()
    {
        Logger.LogProcessFormat("complete");
        _deleteCom();

        //GameStatisticManager.GetInstance().DoStatNewBieGuide((int)GuideTaskID, currentIndex);

        fCheckTime = 0.0f;
        currentIndex += 1;
        mCurGuideUnit.savePoint = currentIndex;

        _setUnitData();

        _checkNext();
    }

    public void ControlWait()
    {
        Logger.LogProcessFormat("wait");

        _deleteCom();

        mCurGuideUnit.savePoint = currentIndex;

        _setUnitData();

        if (mGuideManager != null)
        {
            mGuideManager.ManagerWait();
        }
    }

    public void ControlException()
    {
        Logger.LogProcessFormat("ControlException");
        if (mGuideManager != null)
        {
            mGuideManager.ManagerException();
        }

        _DealFinishProcess();
    }

    public void FinishCurGuideControl()
    {
        if (ClientSystemManager.instance.CurrentSystem is ClientSystemBattle && BattleMain.instance != null)
        {
            BattleMain.instance.GetDungeonManager().ResumeFight();
            Logger.LogWarningFormat("新手引导暂停恢复 : {0}", mCurGuideTaskID);
        }

        _deleteCom();
        _setUnitData();
        curState = ControlState.Finish;
    }

    void Update()
    {
        switch (curState)
        {
            case ControlState.None:
                {
                    break;
                }
            case ControlState.Guiding:
                {
                    break;
                }
            case ControlState.Finish:
                {
                    break;
                }
        }

        if(curState == ControlState.Guiding)
        {
            NewbieGuideManager.GetInstance().SetPauseState(ClientSystemManager.GetInstance().IsFrameOpen<TaskDialogFrame>());
        }

        PerformCheckNext();
    }

    void _setUnitData()
    {
        if (mGuideManager == null)
        {
            return;
        }

        NewbieGuideTable tableData = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)mCurGuideUnit.taskId);
        if (tableData == null)
        {
            return;
        }

        var mUnit = mGuideManager.GetUnit(tableData);
        if (mUnit == null)
        {
            return;
        }

        mUnit.manager.mGuideControl = this;
        mUnit.manager.mGuideControl.mCurState = mCurState;
        mUnit.savePoint = mCurGuideUnit.savePoint;
        mUnit.AlreadySend = mCurGuideUnit.AlreadySend;
    }

    private void _DealGuidingProcess(ComNewbieData type)
    {
        GameObject go = null;

//         go = AssetLoader.instance.LoadResAsGameObject(ComNewbieGuideBase.kWaitTipsPath);
//         Utility.AttachTo(go, ClientSystemManager.instance.MiddleLayer);
//         go.transform.SetAsLastSibling();

        mCurrentCom = NewbieGuideComFactory.AddNewbieCom(gameObject, type);
        if(mCurrentCom == null)
        {
            _DealFinishProcess();
            return;
        }

        //if(go != null)
        {
            //mCurrentCom.AddToCachedObject(go);
        }

        mCurrentCom.SetTaskBaseNewbieGuideControl(this);

        if (mGuideManager != null && mCurGuideUnit != null && !mCurGuideUnit.AlreadySend && mCurrentCom != null && mCurrentCom.mSendSaveBoot && mCurGuideTaskID > NewbieGuideTable.eNewbieGuideTask.None)
        {
            mGuideManager.SendSaveBoot(mCurGuideTaskID);
            mCurGuideUnit.AlreadySend = true;
        }

        if (mCurrentCom != null && ClientSystemManager.instance.CurrentSystem is ClientSystemBattle && BattleMain.instance != null)
        {
            if (mCurrentCom.mTryPauseBattle)
            {
                BattleMain.instance.GetDungeonManager().PauseFight();
                Logger.LogWarningFormat("新手引导暂停 : {0}", mCurGuideTaskID);
            }
            else if (mCurrentCom.mTryResumeBattle)
            {
                BattleMain.instance.GetDungeonManager().ResumeFight();
                Logger.LogWarningFormat("新手引导暂停恢复 : {0}", mCurGuideTaskID);
            }
        }

        curState = ControlState.Guiding;

        NewbieGuideTable tblData = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)mCurGuideTaskID);
        if(tblData != null)
        {
            if(currentIndex >= 0 && currentIndex < tblData.AudioIDList.Count)
            {
                if(tblData.AudioIDList[currentIndex] > 0)
                {
                    SoundTable sounddata = TableManager.GetInstance().GetTableItem<SoundTable>(tblData.AudioIDList[currentIndex]);

                    if(sounddata != null)
                    {
                        if(sounddata.Path.Count > 0)
                        {
                            if(sounddata.Path[0] != "" && sounddata.Path[0] != "-")
                            {
                                #if !LOGIC_SERVER
                                AudioManager.instance.PlayGuideAudio(sounddata.Path[0]);
                                #endif
                            }         
                        }          
                    }           
                }
            }                    
        }

        // 新手引导埋点
        GameStatisticManager.GetInstance().DoStatNewBieGuide(GuideTaskID.ToString(), currentIndex);
        //Logger.LogErrorFormat("Test DoStatNewBieGuide ---- GuideTaskID = {0}, currentIndex = {1}", GuideTaskID, currentIndex);
    }

    private void _DealFinishProcess()
    {
        if (mGuideManager != null && mCurGuideTaskID > NewbieGuideTable.eNewbieGuideTask.None)
        {
            mGuideManager.ManagerFinishGuide(mCurGuideTaskID);
        }
    }

    private void _deleteCom()
    {
        if (mCurrentCom != null)
        {
            mCurrentCom.BaseComplete();
            //GameObject.Destroy(mCurrentCom);
            mCurrentCom = null;
        }
    }

    public void ControlSave()
    {
        if (mGuideManager != null)
        {
            mGuideManager.Save();
        }
    }

    public NewbieGuideTable.eNewbieGuideTask GuideTaskID
    {
        get
        {
            return mCurGuideTaskID;
        }

        set
        {
            if (mCurGuideTaskID != value)
            {
                mCurGuideTaskID = value;
            }
        }
    }

    public ControlState curState
    {
        get
        {
            return mCurState;
        }

        set
        {
            if (mCurState != value)
            {
                mCurState = value;
            }
        }
    }

    public int currentIndex
    {
        get
        {
            return mCurrentIndex;
        }

        set
        {
            if (value < 0)
            {
                mCurrentIndex = 0;
            }
            mCurrentIndex = value;
        }
    }

    public NewbieGuideManager guideManager
    {
        get
        {
            return mGuideManager;
        }

        set
        {
            mGuideManager = value;
        }
    }

    public void SetUnit(NewbieGuideDataUnit unit)
    {
        mCurGuideUnit = unit;
        mCurrentIndex = mCurGuideUnit.savePoint;

        if(mCurGuideUnit.manager.mGuideControl != null)
        {
            mCurState = mCurGuideUnit.manager.mGuideControl.mCurState;
        }        
    }

    public ComNewbieGuideBase GetCurGuideCom()
    {
        return mCurrentCom;
    }

    public NewbieGuideDataUnit GetControlUnit()
    {
        return mCurGuideUnit;
    }

    bool CheckModifyData(ref ComNewbieData type)
    {
        if (type.ModifyDataTypeList != null && type.ModifyDataTypeList.Count > 0)
        {
            for (int i = 0; i < type.ModifyDataTypeList.Count; i++)
            {
                NewbieModifyData ModifyData = type.ModifyDataTypeList[i];

                for (int j = 0; j < type.args.Length; j++)
                {
                    if (j != ModifyData.iIndex)
                    {
                        continue;
                    }

                    if (!SwitchDataForm(ModifyData, ref type.args[j]))
                    {
                        //ControlException();
                        return false;
                    }

                    break;
                }
            }
        }

        return true;
    }

    bool SwitchDataForm(NewbieModifyData ModifyData, ref object obj)
    {
        object[] ParamList = new object[ParamNum];

        bool bCanGuide = GetModifyRealDataByType(ModifyData.ModifyDataType, ref ParamList);

        if(bCanGuide)
        {
            if (obj is string)
            {
                string str = obj as string;
                obj = string.Format(str, ParamList);
            }
            else if (obj is int)
            {

            }
            else
            {

            }
        }

        return bCanGuide;
    }

    bool GetModifyRealDataByType(NewBieModifyDataType ModifyDataType, ref object[] ReturnParamList)
    {
        bool bCanGuide = false;

        if(ReturnParamList.Length < ParamNum)
        {
            return bCanGuide;
        }

        switch (ModifyDataType)
        {
            case NewBieModifyDataType.JobID:
                {
                    ReturnParamList[0] = PlayerBaseData.GetInstance().JobTableID;
                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.EquipInPackagePos:
                {
                    if (mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[EquipInPackagePos]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    IGameBind frame = ClientSystemManager.GetInstance().GetFrame(typeof(PackageNewFrame)) as IGameBind;
                    if (frame == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[EquipInPackagePos]IGameBind为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    RectTransform root = frame.GetComponent<RectTransform>(MainUIIconPath.PackageNewFrameItemPath);
                    if (root == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[EquipInPackagePos]root为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    List<ulong> EquipmentIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);

                    if (EquipmentIDs == null || EquipmentIDs.Count <= 0)
                    {
                        break;
                    }

                    List<ItemData> Temps = new List<ItemData>();

                    for (int i = 0; i < EquipmentIDs.Count; i++)
                    {
                        ItemData item = ItemDataManager.GetInstance().GetItem(EquipmentIDs[i]);

                        if (item == null)
                        {
                            continue;
                        }

                        if (PlayerBaseData.GetInstance().Level < item.LevelLimit)
                        {
                            continue;
                        }

                        if (mCurGuideUnit.taskId == NewbieGuideTable.eNewbieGuideTask.ForgeGuide || mCurGuideUnit.taskId == NewbieGuideTable.eNewbieGuideTask.QuickEquipGuide)
                        {
                            if (item.EquipWearSlotType != EEquipWearSlotType.EquipWeapon)
                            {
                                continue;
                            }
                        }

                        if (!item.IsOccupationFit())
                        {
                            continue;
                        }

                        if (item.Quality < ItemTable.eColor.BLUE)
                        {
                            continue;
                        }

                        Temps.Add(item);
                    }

                    for (int i = 0; i < Temps.Count; i++)
                    {
                        for (int j = i + 1; j < Temps.Count; j++)
                        {
                            if (Temps[j].Quality > Temps[i].Quality)
                            {
                                ItemData item = Temps[i];
                                Temps[i] = Temps[j];
                                Temps[j] = item;
                            }
                        }
                    }

                    if (Temps.Count <= 0)
                    {
                        break;
                    }

                    //int iIndex = 0;
                    ComGridBindItem[] binddata = root.GetComponentsInChildren<ComGridBindItem>();

                    for (int i = 0; i < binddata.Length; i++)
                    {
                        if(binddata[i] == null)
                        {
                            continue;
                        }

                        if (binddata[i].param1 == null || binddata[i].param2 == null)
                        {
                            continue;
                        }

                        if ((ulong)binddata[i].param2 != Temps[0].GUID)
                        {
                            continue;
                        }

//                         string name = (string)binddata[i].param1;
//                         string[] str = name.Split('_');
// 
//                         if (str.Length < 2)
//                         {
//                             continue;
//                         }
// 
//                         iIndex = int.Parse(str[1]);

                        ReturnParamList[0] = Temps[0].GUID;
                        ReturnParamList[1] = Temps[0].GUID;

                        mCurGuideUnit.NeedSaveParamsList.Add(Temps[0].GUID);

                        bCanGuide = true;

                        break;
                    }

                    break;
                }
            case NewBieModifyDataType.PackageEquipTipsGuidePos:
                {
                    if(mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null || mCurGuideUnit.NeedSaveParamsList.Count < 1)
                    {
                        Logger.LogErrorFormat("新手引导报错---[PackageEquipTipsGuidePos]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ItemData item = ItemDataManager.GetInstance().GetItem((ulong)mCurGuideUnit.NeedSaveParamsList[0]);

                    if (item == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[PackageEquipTipsGuidePos]ItemData为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ulong WearEquipUid = ItemDataManager.GetInstance().GetWearEquipBySlotType(item.EquipWearSlotType);

                    if (WearEquipUid != 0)
                    {
                        ReturnParamList[0] = 1;
                        mCurGuideUnit.NeedSaveParamsList.Add(WearEquipUid);
                    }
                    else
                    {
                        ReturnParamList[0] = 0;
                    }

                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.ChangedEquipInPackagePos:
                {
                    if (mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null || mCurGuideUnit.NeedSaveParamsList.Count < 2)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ChangedEquipInPackagePos]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    IGameBind frame = ClientSystemManager.GetInstance().GetFrame(typeof(PackageNewFrame)) as IGameBind;
                    if (frame == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ChangedEquipInPackagePos]IGameBind为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    RectTransform root = frame.GetComponent<RectTransform>(MainUIIconPath.PackageNewFrameItemPath);
                    if(root == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ChangedEquipInPackagePos]root为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    int iIndex = 0;
                    ComGridBindItem[] binddata = root.GetComponentsInChildren<ComGridBindItem>();

                    for (int i = 0; i < binddata.Length; i++)
                    {
                        if(binddata[i] == null)
                        {
                            continue;
                        }

                        if (binddata[i].param1 == null || binddata[i].param2 == null)
                        {
                            continue;
                        }

                        if ((ulong)binddata[i].param2 != (ulong)mCurGuideUnit.NeedSaveParamsList[1])
                        {
                            continue;
                        }

                        string name = (string)binddata[i].param1;
                        string[] str = name.Split('_');

                        if (str.Length < 2)
                        {
                            continue;
                        }

                        iIndex = int.Parse(str[1]);

                        ReturnParamList[0] = iIndex;
                        ReturnParamList[1] = mCurGuideUnit.NeedSaveParamsList[1];

                        bCanGuide = true;

                        break;
                    }

                    break;
                }
            case NewBieModifyDataType.MagicBoxPos:
                {
                    if (mCurGuideUnit == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ChangedEquipInPackagePos]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    IGameBind frame = ClientSystemManager.GetInstance().GetFrame(typeof(PackageNewFrame)) as IGameBind;
                    if (frame == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ChangedEquipInPackagePos]IGameBind为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    RectTransform root = frame.GetComponent<RectTransform>(MainUIIconPath.PackageNewFrameItemPath);
                    if (root == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ChangedEquipInPackagePos]root为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    List<ulong> ConsumableIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);

                    if (ConsumableIDs == null || ConsumableIDs.Count <= 0)
                    {
                        break;
                    }

                    ulong Temps = 0;

                    for (int i = 0; i < ConsumableIDs.Count; i++)
                    {
                        ItemData item = ItemDataManager.GetInstance().GetItem(ConsumableIDs[i]);

                        if (item == null)
                        {
                            continue;
                        }

                        if (PlayerBaseData.GetInstance().Level < item.LevelLimit)
                        {
                            continue;
                        }

                        if (item.SubType != (int)ItemTable.eSubType.MagicBox)
                        {
                            continue;
                        }

                        Temps = item.GUID;

                        break;
                    }

                    if(Temps == 0)
                    {
                        Logger.LogErrorFormat("Can not find MagicBox In Package");
                        break;
                    }
           
                    ComGridBindItem[] binddata = root.GetComponentsInChildren<ComGridBindItem>();

                    for (int i = 0; i < binddata.Length; i++)
                    {
                        if (binddata[i] == null)
                        {
                            continue;
                        }

                        if (binddata[i].param1 == null || binddata[i].param2 == null)
                        {
                            continue;
                        }

                        if ((ulong)binddata[i].param2 != Temps)
                        {
                            continue;
                        }                       

                        ReturnParamList[0] = Temps;
                        ReturnParamList[1] = Temps;

                        bCanGuide = true;

                        break;
                    }

                    break;
                }
            case NewBieModifyDataType.ActorShowEquipPos:
                {
                    if (mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null || mCurGuideUnit.NeedSaveParamsList.Count < 1)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ActorShowEquipPos]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    IGameBind frame = ClientSystemManager.GetInstance().GetFrame(typeof(PackageNewFrame)) as IGameBind;
                    if (frame == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ActorShowEquipPos]IGameBind为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    RectTransform root = frame.GetComponent<RectTransform>(MainUIIconPath.EquipItemPath);
                    if(root == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ActorShowEquipPos]root为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    RectTransform[] binddata = root.GetComponentsInChildren<RectTransform>();

                    for (int i = 0; i < binddata.Length; i++)
                    {
                        if(binddata[i] == null)
                        {
                            continue;
                        }

                        if (binddata[i].name != mCurGuideUnit.NeedSaveParamsList[0].ToString())
                        {
                            continue;
                        }

//                         RectTransform[] parent = binddata[i].GetComponentsInParent<RectTransform>();
//                         if(parent.Length < 2)
//                         {
//                             continue;
//                         }

                        //ReturnParamList[0] = parent[1].name;
                        ReturnParamList[0] = mCurGuideUnit.NeedSaveParamsList[0].ToString();

                        bCanGuide = true;

                        break;
                    }

                    break;
                }
            case NewBieModifyDataType.WelfareID:
                {
                    GameObject obj = Utility.FindGameObject(MainUIIconPath.activeFuli);
                    if(obj == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[WelfareID][MainUIIconPath.activeFuli]路径错误, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    OnOpenActiveFrame frm = obj.GetComponent<OnOpenActiveFrame>();
                    if(frm == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[WelfareID]OnOpenActiveFrame为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ReturnParamList[0] = frm.iActiveTypeID;

                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.SignInID:
                {
                    if (mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null || mCurGuideUnit.NeedSaveParamsList.Count <= 0)
                    {
                        Logger.LogErrorFormat("新手引导报错---[SignInID]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ReturnParamList[0] = mCurGuideUnit.NeedSaveParamsList[0];
                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.EntourageID:
                {
                    if(PlayerBaseData.GetInstance().JobTableID == 50)
                    {
                        ReturnParamList[0] = 1001;
                    }
                    else
                    {
                        ReturnParamList[0] = 1000;
                    }

                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.EnchantID:
                {
                    GameObject obj = Utility.FindGameObject(ClientSystemManager.instance.MiddleLayer, "Smithshop(Clone)/ScrollView/ViewPort/Content");
                    if(obj == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[EnchantID]Smithshop路径错误, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    Toggle[] toggles = obj.GetComponentsInChildren<Toggle>();
                    if(toggles == null || toggles.Length <= 0)
                    {
                        break;
                    }

                    ReturnParamList[0] = toggles[0].gameObject.name;
                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.EnchantMagicCardID:
                {
                    GameObject obj = Utility.FindGameObject(ClientSystemManager.instance.MiddleLayer, "Smithshop(Clone)/Magic/AddMagic/Right/ScrollView/ViewPort/Content");
                    if(obj == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[EnchantMagicCardID]Smithshop路径错误, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    Toggle[] toggles = obj.GetComponentsInChildren<Toggle>();
                    if (toggles == null || toggles.Length <= 0)
                    {
                        break;
                    }

                    ReturnParamList[0] = toggles[0].gameObject.name;
                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.AchievementPos:
                {
                    if (mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null || mCurGuideUnit.NeedSaveParamsList.Count <= 0)
                    {
                        Logger.LogErrorFormat("新手引导报错---[AchievementPos]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ReturnParamList[0] = mCurGuideUnit.NeedSaveParamsList[0];
                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.BranchMissionPos:
                {
                    if (mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null || mCurGuideUnit.NeedSaveParamsList.Count <= 0)
                    {
                        Logger.LogErrorFormat("新手引导报错---[BranchMissionPos]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ReturnParamList[0] = mCurGuideUnit.NeedSaveParamsList[0];
                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.DailyMissionPos:
                {
                    if (mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null || mCurGuideUnit.NeedSaveParamsList.Count <= 0)
                    {
                        Logger.LogErrorFormat("新手引导报错---[DailyMissionPos]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ReturnParamList[0] = mCurGuideUnit.NeedSaveParamsList[0];
                    bCanGuide = true;

                    break;
                }
            case NewBieModifyDataType.IconPath:
                {
                    if (mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null || mCurGuideUnit.NeedSaveParamsList.Count <= 0)
                    {
                        Logger.LogErrorFormat("新手引导报错---[IconPath]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    int iSkillID = (int)mCurGuideUnit.NeedSaveParamsList[0];

                    SkillTable SkillTableData = TableManager.GetInstance().GetTableItem<SkillTable>(iSkillID);

                    if (SkillTableData == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[IconPath]iSkillID错误, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ReturnParamList[0] = SkillTableData.Icon;

                    bCanGuide = true;
                    break;
                }
            case NewBieModifyDataType.IconName:
                {
                    if (mCurGuideUnit == null || mCurGuideUnit.NeedSaveParamsList == null || mCurGuideUnit.NeedSaveParamsList.Count <= 0)
                    {
                        Logger.LogErrorFormat("新手引导报错---[IconName]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    int iSkillID = (int)mCurGuideUnit.NeedSaveParamsList[0];

                    SkillTable SkillTableData = TableManager.GetInstance().GetTableItem<SkillTable>(iSkillID);

                    if (SkillTableData == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[IconName]iSkillID错误, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ReturnParamList[0] = SkillTableData.Name;

                    bCanGuide = true;
                    break;
                }
            case NewBieModifyDataType.PreJobSkill:
                {
                    if (mCurGuideUnit == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[PreJobSkill]当前引导Unit为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    JobTable JobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().PreChangeJobTableID);
                    if(JobData == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---预转职职业id不存在, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    SkillTable skilldata = TableManager.GetInstance().GetTableItem<SkillTable>(JobData.ProJobSkills);
                    if(skilldata == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---预转职职业技能id不存在, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ReturnParamList[0] = skilldata.Icon;

                    bCanGuide = true;
                    break;
                }
            case NewBieModifyDataType.PreJobName:
                {
                    if (mCurGuideUnit == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[PreJobSkill]当前引导Unit为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    JobTable JobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().PreChangeJobTableID);
                    if (JobData == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---预转职职业id不存在, PreChangeJobTableID = {0} CurGuideTaskID = {1}, mCurrentIndex = {2}, mCurState = {3}", PlayerBaseData.GetInstance().PreChangeJobTableID, mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    ReturnParamList[0] = JobData.Name;
                    ReturnParamList[1] = JobData.Name;

                    bCanGuide = true;
                    break;
                }
            case NewBieModifyDataType.FashionInPackagePos:
                {
                    if (mCurGuideUnit == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ChangedEquipInPackagePos]所需数据为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    IGameBind frame = ClientSystemManager.GetInstance().GetFrame(typeof(PackageNewFrame)) as IGameBind;
                    if (frame == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ChangedEquipInPackagePos]IGameBind为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    RectTransform root = frame.GetComponent<RectTransform>(MainUIIconPath.PackageNewFrameItemPath);
                    if (root == null)
                    {
                        Logger.LogErrorFormat("新手引导报错---[ChangedEquipInPackagePos]root为null, CurGuideTaskID = {0}, mCurrentIndex = {1}, mCurState = {2}", mCurGuideTaskID, mCurrentIndex, mCurState);
                        break;
                    }

                    List<ulong> FashionIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Fashion);

                    if (FashionIDs == null || FashionIDs.Count <= 0)
                    {
                        break;
                    }

                    ulong Temps = 0;

                    for (int i = 0; i < FashionIDs.Count; i++)
                    {
                        ItemData item = ItemDataManager.GetInstance().GetItem(FashionIDs[i]);

                        if (item == null)
                        {
                            continue;
                        }

                        if (PlayerBaseData.GetInstance().Level < item.LevelLimit)
                        {
                            continue;
                        }

                        if (item.SubType != (int)ItemTable.eSubType.FASHION_HEAD)
                        {
                            continue;
                        }

                        Temps = item.GUID;

                        break;
                    }

                    if (Temps == 0)
                    {
                        Logger.LogErrorFormat("Can not find Fashion In Package");
                        break;
                    }
        
                    ComGridBindItem[] binddata = root.GetComponentsInChildren<ComGridBindItem>();

                    for (int i = 0; i < binddata.Length; i++)
                    {
                        if (binddata[i] == null)
                        {
                            continue;
                        }

                        if (binddata[i].param1 == null || binddata[i].param2 == null)
                        {
                            continue;
                        }

                        if ((ulong)binddata[i].param2 != Temps)
                        {
                            continue;
                        }                       

                        ReturnParamList[0] = Temps;
                        ReturnParamList[1] = Temps;

                        bCanGuide = true;

                        break;
                    }

                    break;
                }
            default:
                {
                    break;
                }
        }

        return bCanGuide;
    }
}

