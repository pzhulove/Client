using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComMoneyRewardsResultData
    {
        public ulong recordId;
        public string name;
        public int occu;
        public int winTimes;
        public int rank;
        public bool losed;
    }

    class ComMoneyRewardsResultInfo : MonoBehaviour
    {
        static string ms_key_lose = "lose";
        static string ms_key_win1 = "win1";
        static string ms_key_win2 = "win2";
        static string ms_key_win3 = "win3";
        static string ms_key_null = "null";
        static string ms_key_init = "unstarted";
        static string ms_key_win1_lose = "win1_lose";
        static string ms_key_win2_lose = "win2_lose";
        public Image Icon;
        public Text Name;
        public StateController comState;
        public Image LoseIcon;
        public void OnWatchRecord()
        {

        }
        ComMoneyRewardsResultData data = null;
        public void SetValue(ComMoneyRewardsResultData value)
        {
            this.data = value;
            if (null != data)
            {
                if(null != comState)
                {
                    if(data.winTimes < 0)
                    {
                        comState.Key = ms_key_init;
                    }
                    else if(data.winTimes == 0)
                    {
                        ComMoneyRewardsResultData otherValue = MoneyRewardsDataManager.GetInstance().GetOtherResultData(value);

                        if (null == otherValue)
                        {
                            comState.Key = ms_key_init;
                        }
                        else
                        {
                            if(otherValue.winTimes <= 0)
                            {
                                comState.Key = ms_key_init;
                            }
                            else
                            {
                                comState.Key = ms_key_lose;
                            }
                        }
                    }
                    else if (data.winTimes == 1)
                    {
                        if (!data.losed)
                        {
                            comState.Key = ms_key_win1;
                        }
                        else
                        {
                            comState.Key = ms_key_win1_lose;
                        }
                    }
                    else if (data.winTimes == 2)
                    {
                        if (!data.losed)
                        {
                            comState.Key = ms_key_win2;
                        }
                        else
                        {
                            comState.Key = ms_key_win2_lose;
                        }
                    }
                    else if (data.winTimes == 3)
                    {
                        comState.Key = ms_key_win3;
                    }
                    else
                    {
                        comState.Key = ms_key_win3;
                        Logger.LogErrorFormat("status error winTimes = {0}", data.winTimes);
                    }
                }

                if(null != Name)
                {
                    Name.text = data.name;
                }

                if(null != Icon)
                {
                    string path = "";
                    var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(data.occu);
                    if (null != jobItem)
                    {
                        ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                        if (resData != null)
                        {
                            path = resData.IconPath;
                        }
                    }
                    // Icon.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref Icon, path);
                }
            }
            else
            {
                if (null != comState)
                {
                    comState.Key = ms_key_null;
                }
            }
        }
        public void OnWatchPlayerInfo()
        {
            if(null != data && data.recordId > 0 && data.recordId != PlayerBaseData.GetInstance().RoleID)
            {
                OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(data.recordId);
            }
        }
    }
}