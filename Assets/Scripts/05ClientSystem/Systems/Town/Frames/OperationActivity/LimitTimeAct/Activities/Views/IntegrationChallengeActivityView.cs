using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class IntegrationChallengeActivityView : MonoBehaviour, IActivityView
    {

        [SerializeField]
        private Text mTimeTxt;
        [SerializeField]
        private Text mRuleTxt;


        [SerializeField]
        private Button mIntegrationBtn;
        [SerializeField]
        private Button mGoBtn;
      
        [SerializeField]
        private Transform mItemRoot;
        [SerializeField] private Text mNum;

        private uint scoreActivityId = 0;
        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            scoreActivityId = model.Param;
            mTimeTxt.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(model.StartTime), _TransTimeStampToStr(model.EndTime)));
            mRuleTxt.SafeSetText(model.RuleDesc.Replace('|', '\n'));
            mNum.SafeSetText(CountDataManager.GetInstance().GetCount(CounterKeys.TOTAL_CHALLENGE_SCORE).ToString());

            mIntegrationBtn.SafeAddOnClickListener(_OnIntegrationBtnClick);
            mGoBtn.SafeAddOnClickListener(_OnGoBtnClick);
            _InitItems(model);
        }

       

      


        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void Dispose()
        {
            mIntegrationBtn.SafeRemoveOnClickListener(_OnIntegrationBtnClick);
            mGoBtn.SafeRemoveOnClickListener(_OnGoBtnClick);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示任务数据
        /// </summary>
        /// <param name="model"></param>
        private void _InitItems(ILimitTimeActivityModel data)
        {

            GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
            if (go == null)
            {
                Logger.LogError("加载预制体失败，路径:" + data.ItemPath);
                return;
            }

            if (go.GetComponent<IActivityCommonItem>() == null)
            {
                Destroy(go);
                Logger.LogError("预制体上找不到IActivityCommonItem的脚本，预制体路径是:" + data.ItemPath);
                return;
            }
            for (int i = 0; i < data.TaskDatas.Count; i++)
            {
                ILimitTimeActivityTaskDataModel taskData = data.TaskDatas[i];
                if (taskData==null)
                {
                    continue;
                }
                _AddItem(go,i, data);
            }
          

            Destroy(go);
        }

        private void _AddItem(GameObject go, int id, ILimitTimeActivityModel data)
        {
            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoot, false);
            item.GetComponent<IActivityCommonItem>().Init(data.TaskDatas[id].DataId, data.Id, data.TaskDatas[id], null);
          

        }
        /// <summary>
        /// 前往积分商城
        /// </summary>
        private void _OnIntegrationBtnClick()
        {
            LimitTimeActivityFrame limitTimeActivityFrame= ClientSystemManager.GetInstance().GetFrame(typeof(LimitTimeActivityFrame)) as LimitTimeActivityFrame;
            if(limitTimeActivityFrame!=null)
            {
                limitTimeActivityFrame.OpenFrameByActivityId(scoreActivityId);
            }
        }

        /// <summary>
        /// 前往挑戰
        /// </summary>
        private void _OnGoBtnClick()
        {
            ItemComeLink.OnLink(800001235, 0, true, null, false, false, false,
             null, TR.Value("IntegrationChallenge_GoChallengeTitle"));
        }
        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}年{1}月{2}日{3:HH:mm}",dt.Year, dt.Month, dt.Day, dt);
        }


        public void Show()
        {

        }

        public void UpdateData(ILimitTimeActivityModel data)
        {

        }

    }
}
