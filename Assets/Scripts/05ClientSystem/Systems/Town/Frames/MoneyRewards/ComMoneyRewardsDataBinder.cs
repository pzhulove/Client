using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComMoneyRewardsDataBinder : MonoBehaviour
    {
        public StateController mStateVS;
        public StateController[] mStateVsPlayers = new StateController[0];
        public Text[] mPlayerNames = new Text[0];
        public Image[] mPlayerHeads = new Image[0];
        public Text[] mLevels = new Text[0];
        public Button mButtonWatchPlayerInfo;
        public StateController mStateDesc;
        public string mUnLeaveStringHint = string.Empty;
        public TimeRefresh mTimeRefresh;

        string mStrEnable = "Enable";
        string mStrDisable = "Disable";
        public uint triggerTime = 16;

        public string eachVSFmtString = string.Empty;
        public Text eachVSAwardDesc;
        public Text eachVSGetAward;
        public string eachGetFmtString = string.Empty;
        public StateController mVsStatus;
        public Text awardsInPools;
        public string eachFixFmtString = string.Empty;
        public Text awardPerVS;

        public void SetVSAwardsDesc()
        {
            if(null != eachVSAwardDesc)
            {
                eachVSAwardDesc.text = string.Format(eachVSFmtString,MoneyRewardsDataManager.GetInstance().MaxAwardEachVS);
            }
        }

        public void SetVSEachFixedGetDesc()
        {
            if(null != awardPerVS)
            {
                awardPerVS.text = string.Format(eachFixFmtString, MoneyRewardsDataManager.GetInstance().FixedAwardEachVS);
            }
        }

        public void SetVSGetAwardsDesc()
        {
            int value = MoneyRewardsDataManager.GetInstance().vsAwards;
            if(null != eachVSGetAward)
            {
                eachVSGetAward.text = string.Format(eachGetFmtString, value);
            }
        }

        public void SetPoolAwards()
        {
            if(null != awardsInPools)
            {
                awardsInPools.text = MoneyRewardsDataManager.GetInstance().moneysInPool.ToString();
            }
        }

        public void SelectVSPanel(bool bEnable)
        {
            if (null != mVsStatus)
            {
                mVsStatus.Key = bEnable ? mStrEnable : mStrDisable;
            }
        }

        public void SetVSEnable(bool bEnable)
        {
            if(null != mStateVS)
            {
                mStateVS.Key = bEnable ? mStrEnable : mStrDisable;
            }
        }

        public void SetPlayerEnable(int iIndex, bool bEnable)
        {
            if (iIndex >= 0 && iIndex < mStateVsPlayers.Length)
            {
                var status = mStateVsPlayers[iIndex];
                if (null != status)
                {
                    status.Key = bEnable ? mStrEnable : mStrDisable;
                }
            }
        }

        public void SetPlayerData(int iIndex,object data)
        {
            var result = data as ComMoneyRewardsResultData;
            if(null != result)
            {
                SetPlayerName(iIndex, result.name);
                SetPlayerHead(iIndex, result.occu);
                SetPlayerLevel(iIndex, 1);
            }
        }

        public void SetPlayerName(int iIndex,string name)
        {
            if(iIndex >= 0 && iIndex < mPlayerNames.Length)
            {
                var text = mPlayerNames[iIndex];
                if(null != text)
                {
                    text.text = name;
                }
            }
        }

        public void SetPlayerHead(int iIndex, int occu)
        {
            if (iIndex >= 0 && iIndex < mPlayerHeads.Length)
            {
                var img = mPlayerHeads[iIndex];
                if (null != img)
                {
                    string path = "";
                    var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(occu);
                    if (null != jobItem)
                    {
                        ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                        if (resData != null)
                        {
                            path = resData.IconPath;
                        }
                    }
                    ETCImageLoader.LoadSprite(ref img, path);
                }
            }
        }

        public void SetPlayerLevel(int iIndex, int level)
        {
            if (iIndex >= 0 && iIndex < mLevels.Length)
            {
                var text = mLevels[iIndex];
                if (null != text)
                {
                    text.text = level.ToString();
                }
            }
        }

        public void RemoveWatchListener()
        {
            if(null != mButtonWatchPlayerInfo)
            {
                mButtonWatchPlayerInfo.onClick.RemoveListener(OnWatchPlayerInfo);
            }
        }

        public void AddWatchListener()
        {
            if (null != mButtonWatchPlayerInfo)
            {
                mButtonWatchPlayerInfo.onClick.AddListener(OnWatchPlayerInfo);
            }
        }

        public void SetWatchPlayerInfo(object other)
        {
            mOther = other as ComMoneyRewardsResultData;
        }

        ComMoneyRewardsResultData mOther = null;
        public void OnWatchPlayerInfo()
        {
            if(null != mOther)
            {
                if(mOther.recordId != PlayerBaseData.GetInstance().RoleID && mOther.recordId > 0)
                {
                    OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(mOther.recordId);
                }
            }
        }

        public void DoUnLeaveHint()
        {
            SystemNotifyManager.SysNotifyTextAnimation(mUnLeaveStringHint);
        }
        public void UpdateStatus()
        {
            if(null != mStateDesc)
            {
                switch (MoneyRewardsDataManager.GetInstance().eMoneyRewardsStatus)
                {
                    case MoneyRewardsStatus.MRS_INVALID:
                        {
                            mStateDesc.Key = "finish";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_READY:
                        {
                            mStateDesc.Key = "ready";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_8_RACE:
                        {
                            mStateDesc.Key = "8level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_PRE_4_RACE:
                        {
                            mStateDesc.Key = "4pre_level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_4_RACE:
                        {
                            mStateDesc.Key = "4level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_2_RACE:
                        {
                            mStateDesc.Key = "2level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_RACE:
                        {
                            mStateDesc.Key = "1level";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_END:
                        {
                            mStateDesc.Key = "finish";
                        }
                        break;
                }
            }
        }
        public void StartCounter(uint count)
        {
            if(null != mTimeRefresh)
            {
                mTimeRefresh.CustomActive(true);
                mTimeRefresh.Initialize();
                mTimeRefresh.Time = count;
            }
        }
        public void CloseCounter()
        {
            if (null != mTimeRefresh)
            {
                mTimeRefresh.Time = 0;
                mTimeRefresh.CustomActive(false);
            }
        }

        void OnDestroy()
        {
            RemoveWatchListener();
            mOther = null;
        }
    }
}