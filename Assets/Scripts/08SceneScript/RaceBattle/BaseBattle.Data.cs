using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameClient
{
    public partial class BaseBattle
    {
        private Dictionary<int, Dictionary<string, behaviac.UserData>> mAllUserData = new Dictionary<int, Dictionary<string, behaviac.UserData>>();

        public void SetUserData(int id, string key, behaviac.UserData data)
        {
            if (!mAllUserData.ContainsKey(id))
            {
                mAllUserData.Add(id, new Dictionary<string, behaviac.UserData>());
            }

            if (mAllUserData.ContainsKey(id))
            {
                var temp = mAllUserData[id];
                if (temp.ContainsKey(key))
                {
                    temp[key] = data;
                }
                else
                {
                    temp.Add(key, data);
                }
            }
        }

        public behaviac.UserData GetUserData(int id, string key)
        {
            if (mAllUserData.ContainsKey(id))
            {
                if (mAllUserData[id].ContainsKey(key))
                {
                    return mAllUserData[id][key];
                }
            }

            return new behaviac.UserData();
        }

        public void ClearUserData(int id, string key = "", bool isMatch = false)
        {
            if (!mAllUserData.ContainsKey(id))
            {
                return;
            }

            if (key == string.Empty)
            {
                mAllUserData[id].Clear();
            }
            else
            {
                mAllUserData[id].Remove(key);
            }
        }

        public void ClearAllUserData()
        {
            mAllUserData.Clear();
        }
    }
}
