using System;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;


namespace GameClient
{
    public class AccumulativeSignIn : MonoBehaviour
    {
        // Use this for initialization
        Text mSignInCount = null;
        Button mBoxBt = null;
        GameObject mTeXiao = null;
        bool IsOpen = false;
        void Start()
        {
            mSignInCount = null;
            mBoxBt = null;
            mTeXiao = null;
            ActiveManager.GetInstance().onActivityUpdate += UpdateSignInCount;


            ComCommonBind AccumulativeSignInCommonBind = this.gameObject.GetComponent<ComCommonBind>();
            if(AccumulativeSignInCommonBind == null)
            {
                Logger.LogErrorFormat("Can not find ComcomonBind in AccumulativeSighIn");
            }
            else
            {
                mTeXiao = AccumulativeSignInCommonBind.GetGameObject("TeXiao");
                if(mTeXiao == null)
                {
                    Logger.LogErrorFormat("mTeXiao is null");
                    return;
                }
                mTeXiao.CustomActive(false);
                mSignInCount = AccumulativeSignInCommonBind.GetCom<Text>("SignInCount");
                if (mSignInCount != null)
                {
                    int iValue = ActiveManager.GetInstance().GetActiveItemValue(1940, "tt_si", 0);
                    //var value = CountDataManager.GetInstance().GetCountInfo(CounterKeys.SIGN_IN_TT_COUNT);
                    //mSignInCount.text = CounterKeys.SIGN_IN_TT_COUNT + "/30";
                    mSignInCount.text = iValue + "/30";
                }

                mBoxBt = AccumulativeSignInCommonBind.GetCom<Button>("BoxBt");
                if(mBoxBt == null)
                {
                    Logger.LogErrorFormat("BoxBt is null");
                }
                else
                {
                    if(ActiveManager.GetInstance().GetActiveItemValue(1940, "tt_si", 0) == 30)
                    {
                        IsOpen = true;
                        //mBoxBt.onClick.RemoveAllListeners();
                        //mBoxBt.onClick.AddListener(() =>
                        //{
                        //    MissionScoreAwardFrame.Open(1940);
                        //    ActiveAwardFrameDataNew data = new ActiveAwardFrameDataNew();
                        //    List<AwardItemData> list = new List<AwardItemData>();
                        //    var activeData = TableManager.GetInstance().GetTableItem<ActiveTable>(1940);
                        //    if (activeData == null)
                        //    {
                        //        Logger.LogErrorFormat("Can not get activeTableData from id:1940");
                        //        return;
                        //    }
                        //    else
                        //    {
                        //        string[] result = activeData.Awards.Split('_');
                        //        AwardItemData awardData = new AwardItemData();
                        //        int resultID = -1;
                        //        int resultCount = -1;
                        //        int.TryParse(result[0], out resultID);
                        //        int.TryParse(result[1], out resultCount);
                        //        if (resultID == -1 || resultCount == -1)
                        //        {
                        //            Logger.LogErrorFormat("ActiveTableData is Error");
                        //            return;
                        //        }
                        //        awardData.ID = resultID;
                        //        awardData.Num = resultCount;
                        //        list.Add(awardData);
                        //    }
                        //    data.title = "签到额外奖励";
                        //    data.datas = list;

                        //    ClientSystemManager.GetInstance().OpenFrame<ActiveAwardFrameNew>(FrameLayer.Middle, data);
                        //});
                    }
                    else
                    {
                        IsOpen = false;
                        //mBoxBt.onClick.RemoveAllListeners();
                    }

                    AddBtState();
                }
            }
        }

        void OnDestroy()
        {
            ActiveManager.GetInstance().onActivityUpdate -= UpdateSignInCount;
            if (mBoxBt != null)
            {
                mBoxBt.onClick.RemoveAllListeners();
                mBoxBt = null;
            }
        }

        void AddBtState()
        {
            
            if(IsOpen)
            {
                mTeXiao.CustomActive(true);
            }
            else
            {
                mTeXiao.CustomActive(false);
            }

            mBoxBt.onClick.RemoveAllListeners();
            mBoxBt.onClick.AddListener(() =>
            {
                //MissionScoreAwardFrame.Open(1940);
                ActiveAwardFrameDataNew data = new ActiveAwardFrameDataNew();
                List<AwardItemData> list = new List<AwardItemData>();
                var activeData = TableManager.GetInstance().GetTableItem<ActiveTable>(1940);
                if (activeData == null)
                {

                    Logger.LogErrorFormat("Can not get activeTableData from id:1940");
                    return;
                }
                else
                {
                    string[] result = activeData.Awards.Split('_');
                    AwardItemData awardData = new AwardItemData();
                    int resultID = -1;
                    int resultCount = -1;
                    int.TryParse(result[0], out resultID);
                    int.TryParse(result[1], out resultCount);
                    if (resultID == -1 || resultCount == -1)
                    {
                        Logger.LogErrorFormat("ActiveTableData is Error");
                        return;
                    }
                    awardData.ID = resultID;
                    awardData.Num = resultCount;
                    list.Add(awardData);
                }
                data.title = "签到额外奖励";
                data.datas = list;
                data.canGet = IsOpen;

                ClientSystemManager.GetInstance().OpenFrame<ActiveAwardFrameNew>(FrameLayer.Middle, data);
            });
        }

        void UpdateSignInCount(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            if(data.ID == 1940)
            {
                int iValue = ActiveManager.GetInstance().GetActiveItemValue(1940, "tt_si", 0);
                //var value = CountDataManager.GetInstance().GetCountInfo(CounterKeys.SIGN_IN_TT_COUNT);
                //mSignInCount.text = CounterKeys.SIGN_IN_TT_COUNT + "/30";
                mSignInCount.text = iValue + "/30";
                if (data.status == (byte)TaskStatus.TASK_FINISHED)
                {
                    IsOpen = true;
                    
                }
                else
                {
                    IsOpen = false;
                }
                AddBtState();
            }
        }

    }
}
    
