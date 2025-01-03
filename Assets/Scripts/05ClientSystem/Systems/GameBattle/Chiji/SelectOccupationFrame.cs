using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Network;
using Protocol;
namespace GameClient
{
    public class SelectOccupationFrame : ClientFrame
    {
        #region ExtraUIBind
        private SelectOccupationView mSelectOccupationView = null;

        protected sealed override void _bindExUI()
        {
            mSelectOccupationView = mBind.GetCom<SelectOccupationView>("SelectOccupationView");
        }

        protected sealed override void _unbindExUI()
        {
            mSelectOccupationView = null;
        }
        #endregion

        List<int> selectOccuIDs = new List<int>();
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/SelectOccupationFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            if(userData != null)
            {
                UInt32[] occuList = userData as UInt32[];

                if(occuList != null)
                {
                    for(int i = 0; i < occuList.Length; i++)
                    {
                        selectOccuIDs.Add((int)occuList[i]);
                    }
                }
                else
                {
                    Logger.LogError("吃鸡选择职业界面报错，occuList == null");
                }
            }
            else
            {
                Logger.LogError("吃鸡选择职业界面报错，userData == null");
            }

            //InitJobList();

            if (mSelectOccupationView != null)
            {
                mSelectOccupationView.InitView(selectOccuIDs, _OnSelectJobClick);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();

            ClientSystemManager.GetInstance().OpenFrame<SelectMapAreaFrame>(FrameLayer.Middle);
        }

        private void InitJobList()
        {
            var self = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (self != null)
            {
                if (self.JobType != 0)
                {
                    selectOccuIDs.Add(self.ID);
                }
            }

            List<int> jobList = new List<int>();
            var jobTable = TableManager.GetInstance().GetTable<ProtoTable.JobTable>();
            if (jobTable != null)
            {
                var enumerator = jobTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var jobItem = enumerator.Current.Value as JobTable;

                    if (jobItem == null)
                    {
                        continue;
                    }

                    if (self.JobType != 0)
                    {
                        if (jobItem.ID == PlayerBaseData.GetInstance().JobTableID)
                        {
                            continue;
                        }
                    }
                    
                    //测试角色不添加到列表
                    if (jobItem.ID == 16)
                    {
                        continue;
                    }

                    if (jobItem.Open == 0)
                    {
                        continue;
                    }
                    
                    if (jobItem.JobType == 0)
                    {
                        continue;
                    }

                    jobList.Add(jobItem.ID);
                }
            }

            if (self.JobType != 0)
            {
                var index = RandomNumber(0, jobList.Count / 2);
                Add(jobList[index]);

                var index2 = RandomNumber(jobList.Count / 2, jobList.Count);
                Add(jobList[index2]);
            }
            else
            {
                var idx1 = RandomNumber(0, jobList.Count / 3);
                Add(jobList[idx1]);

                var idx2 = RandomNumber(jobList.Count / 3, 2 * (jobList.Count) / 3);
                Add(jobList[idx2]);

                var idx3 = RandomNumber(2 * (jobList.Count) / 3, jobList.Count);
                Add(jobList[idx3]);
            }
                
        }

        private int RandomNumber(int min , int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        private void Add(int jobId)
        {
            if (!selectOccuIDs.Contains(jobId))
            {
                selectOccuIDs.Add(jobId);
            }
        }

        private void _ClearData()
        {
            if(selectOccuIDs != null)
            {
                selectOccuIDs.Clear();
            }
        }
        
        protected void _OnSelectJobClick(int jobId)
        {
            ChijiDataManager.GetInstance().SendSelectJobId(jobId);

#if MG_TEST
            ExceptionManager.GetInstance().RecordLog("测试服吃鸡:选择职业单局开始");
            GameStatisticManager.GetInstance().UploadLocalLogToServer("测试服吃鸡:选择职业单局开始");
#endif

            ClientSystemManager.GetInstance().CloseFrame<SelectOccupationFrame>();
        }
    }
}
