using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    public class ActivityDungeonDeadTowerWipeData
    {
        public string roleId   { get; set; }
        public uint   wipetime { get; set; }
        public bool   hasvisit { get; set; }
    }

    public class ActivityDungeonPersistentData
    {
        public List<ActivityDungeonDeadTowerWipeData> deadtowerwipe = new List<ActivityDungeonDeadTowerWipeData>();
    }


    /// <summary>
    /// 活动副本持久化数据
    ///
    /// </summary>
    public class ActivityDungeonPersistentDataManager : Singleton<ActivityDungeonPersistentDataManager>
    {
        public const string kFileName = "activitydungeonpersistentdata.conf";

        private ActivityDungeonPersistentData mPData;

        private bool mDirty = false;

        private void _log(string msg)
        {
            UnityEngine.Debug.LogFormat("[ActivityDungeonPersistentDataManager] {0}", msg);
        }

        public void LoadData()
        {
            UnloadData();

            _mapJsonStr2Object();
           
            mDirty = false;
            _bindEvent();
        }

        private void _mapJsonStr2Object()
        {
            try 
            {
                mPData = LitJson.JsonMapper.ToObject<ActivityDungeonPersistentData>(_loadDataString());
                _log("从文件加载");
            }
            catch (System.Exception e)
            {
                _log(e.ToString());
            }

            if (null == mPData)
            {
                mPData = new ActivityDungeonPersistentData();
                _log("空的数据");
            }
        }

        public void UnloadData()
        {
            mPData = null;
            _unbindEvent();
        }

        private void _bindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDeadTowerWipeoutTimeChange, _onDeadTowerWipeoutTimeChange);
        }

        private void _unbindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDeadTowerWipeoutTimeChange, _onDeadTowerWipeoutTimeChange);
        }

        private void _onDeadTowerWipeoutTimeChange(UIEvent ui)
        {
            UpdateWipeEndTimeAndSave( 
                    PlayerBaseData.GetInstance().RoleID,
                    PlayerBaseData.GetInstance().DeathTowerWipeoutEndTime);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityDungeonDeadTowerWipeEnd);
        }

        public void Save()
        {
            if (mDirty)
            {
                string pdDataStr = LitJson.JsonMapper.ToJson(mPData);

                FileArchiveAccessor.SaveFileInPersistentFileArchive(kFileName, pdDataStr);

                mDirty = false;
                _log(string.Format("写入 : {0}", pdDataStr));
            }
            else 
            {
                _log("数据没有改变");
            }
        }

        
        private string _loadDataString()
        {
            string str = string.Empty;

            FileArchiveAccessor.LoadFileInPersistentFileArchive(kFileName, out str);

            if (string.IsNullOrEmpty(str))
            {
                str = "{}";
            }

            _log(str);

            return str;
        }


        public void UpdateWipeEndTimeAndSave(ulong roleId, uint wipeEndTime)
        {
            _log(string.Format("更新死亡之塔时间 {0}", wipeEndTime));

            string roleIdStr = roleId.ToString();
            ActivityDungeonDeadTowerWipeData data = _findActivityDungeonDeadTowerWipeData(roleIdStr);

            if (null != data)
            {
                if (wipeEndTime > data.wipetime)
                {
                    data.wipetime = wipeEndTime;
                    data.hasvisit = false;
                    mDirty        = true;
                }
            }
            else 
            {
                _addNewWipEndTime(roleIdStr, wipeEndTime);
            }

            Save();
        }

        ulong cachedRoleId = ~0u;
        string cachedRoleIdString = null;
        private ActivityDungeonDeadTowerWipeData _findActivityDungeonDeadTowerWipeData(ulong roleId)
        {
            if(cachedRoleId != roleId || string.IsNullOrEmpty(cachedRoleIdString))
            {
                cachedRoleId = roleId;
                cachedRoleIdString = cachedRoleId.ToString();
            }

            return _findActivityDungeonDeadTowerWipeData(cachedRoleIdString);
        }

        private List<ActivityDungeonDeadTowerWipeData> mFindDatas = new List<ActivityDungeonDeadTowerWipeData>();

        private ActivityDungeonDeadTowerWipeData _findActivityDungeonDeadTowerWipeData(string roleId)
        {
            _findAllActivityDungeonDeadTowerWipeData(roleId);
            return _getOneActivityDungeonDeadTowerWipeDataWithFilter();
        }

        private void _findAllActivityDungeonDeadTowerWipeData(string roleId)
        {
            if (null == mPData)
            {
                return ;
            }

            mFindDatas.Clear();

            for (int i = 0; i < mPData.deadtowerwipe.Count; ++i)
            {
                if (mPData.deadtowerwipe[i].roleId == roleId)
                {
                    mFindDatas.Add(mPData.deadtowerwipe[i]);
                }
            }
        }

        private ActivityDungeonDeadTowerWipeData _getOneActivityDungeonDeadTowerWipeDataWithFilter()
        {
            ActivityDungeonDeadTowerWipeData data = null;

            for (int i = 0; i < mFindDatas.Count; ++i)
            {
                if (null == data)
                {
                    data = mFindDatas[i];
                }
                else if (data.wipetime <= mFindDatas[i].wipetime)
                {
                    data = mFindDatas[i];
                }
            }

            for (int i = 0; i < mFindDatas.Count; ++i)
            {
                if (data != mFindDatas[i])
                {
                    _log("删除重复数据");
                    mPData.deadtowerwipe.Remove(mFindDatas[i]);
                    mDirty = true;
                }
            }

            mFindDatas.Clear();

            if (mDirty)
            {
                Save();
            }

            return data;
        }


        private void _addNewWipEndTime(string roleId, uint wipeEndTime)
        {
            if (null != _findActivityDungeonDeadTowerWipeData(roleId))
            {
                return ;
            }

            ActivityDungeonDeadTowerWipeData data = new ActivityDungeonDeadTowerWipeData();

            data.roleId   = roleId;
            data.wipetime = wipeEndTime;
            data.hasvisit = false;

            mPData.deadtowerwipe.Add(data);
            mDirty = true;
        }

        public uint GetWipeEndTime(ulong roleId)
        {
            ActivityDungeonDeadTowerWipeData data = _findActivityDungeonDeadTowerWipeData(roleId);

            if (null == data)
            {
                return 0;
            }

            return data.wipetime;
        }

        public bool HasWipeEndVisited(ulong roleId)
        {
            ActivityDungeonDeadTowerWipeData data = _findActivityDungeonDeadTowerWipeData(roleId);

            if (null == data)
            {
                return true;
            }

            return data.hasvisit;
        }

        public void SetWipeEndTimeVistedAndSave(ulong roleId)
        {
            ActivityDungeonDeadTowerWipeData data = _findActivityDungeonDeadTowerWipeData(roleId);

            if (null == data)
            {
                return ;
            }

            if (!data.hasvisit)
            {
                data.hasvisit = true;
                mDirty        = true;
            }

            Save();
        }
    }
}
