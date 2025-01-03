using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class BuffPrayActivityNewView : MonoBehaviour, IActivityView
    {
        [SerializeField]
        private Text[] mTitleTxtArry = new Text[2];
        [SerializeField]
        private Text[] mDestxtArry = new Text[2];
        [SerializeField]
        private Text mTimeTxt;
        [SerializeField]
        private GameObject mEffectRoot;

        [SerializeField] Button mBuffGo1Btn;
        [SerializeField] Button mBuffGo2Btn;

        private ILimitTimeActivityModel mModel;
        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if(model!=null)
            {
                mModel = model;
                UpdateInfo(model);
            }

            mBuffGo1Btn.SafeRemoveAllListener();
            mBuffGo1Btn.SafeAddOnClickListener(OnBuffGo1BtnClick);

            mBuffGo2Btn.SafeRemoveAllListener();
            mBuffGo2Btn.SafeAddOnClickListener(OnBuffGo2BtnClick);
        }

        void OnBuffGo1BtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<StrengthenTicketMergeFrame>();
            CloseActivityLimitTimeFrame();
        }
        
        void OnBuffGo2BtnClick()
        {
            ActivityDungeonFrame.OpenLinkFrame("2003000|2");
            CloseActivityLimitTimeFrame();
        }


        public void Show()
        {
            if(mModel!=null)
            {
                UpdateData(mModel);
            }
        }

        public void UpdateData(ILimitTimeActivityModel data)
        {
            UpdateInfo(data);
        }


        public void Close()
        {
            mModel = null;
            Destroy(gameObject);
        }

        public void Dispose()
        {

        }

        public void Hide()
        {

        }
        private void UpdateInfo(ILimitTimeActivityModel model)
        {
            if (model.TaskDatas.Count > mTitleTxtArry.Length) return;

            for (int i = 0; i < model.TaskDatas.Count; i++)
            {
                ILimitTimeActivityTaskDataModel taskData = model.TaskDatas[i];
                if (taskData.State != Protocol.OpActTaskState.OATS_FINISHED) continue;
                if (mTitleTxtArry[i] != null)
                {
                    mTitleTxtArry[i].text = taskData.taskName;
                }
                if (mDestxtArry[i] != null)
                {
                    mDestxtArry[i].text = taskData.Desc;
                }

            }
            if (mTimeTxt != null)
            {
                mTimeTxt.SafeSetText(string.Format(TR.Value("activity_qi_xi_que_qiao_time"), Function.GetTimeWithoutYearNoZero((int)model.StartTime, (int)model.EndTime)));
            }
            if(mEffectRoot!=null)
            {
                Utility.ClearChild(mEffectRoot);
                SetEffects(Utility.GetBuffPrayActivityEffectPath(0));
            }
           
        }

        public void SetEffects(string sEffectPath)
        {
            if (sEffectPath != "")
            {
                GameObject mEffectGo = AssetLoader.GetInstance().LoadResAsGameObject(sEffectPath);
                Utility.AttachTo(mEffectGo, mEffectRoot);
                mEffectGo.CustomActive(true);
            }
        }

        private void CloseActivityLimitTimeFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
        }
    }
}
