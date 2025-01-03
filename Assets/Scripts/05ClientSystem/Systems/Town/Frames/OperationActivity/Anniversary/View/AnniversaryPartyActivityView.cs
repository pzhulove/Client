using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AnniversaryPartyActivityView : MonoBehaviour, IActivityView
    {

        [SerializeField]
        private Button mGoBtn;

        [SerializeField]
        private Text mTimeTxt;

        [SerializeField]
        private Text mRuleDesTxt;
        

        [SerializeField]
        private int mSplitNum = 2;
        
        [SerializeField]
        private ComChapterInfoDrop mLeftDrop;

        [SerializeField]
        private ComChapterInfoDrop mRightDrop;

        [SerializeField]
        private Transform mPreviewTrans;

        [SerializeField]
        private Vector2 mPreviewItemSize = new Vector2(60, 60);

        [SerializeField]
        private Button mPreviewBtn;

        private int mPreviewItemId = 0;

        private List<int> mLeftDropList = new List<int>();

        private List<int> mRightDropList = new List<int>();


        public void Init(ILimitTimeActivityModel data, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (data.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            mTimeTxt.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(data.StartTime), _TransTimeStampToStr(data.EndTime)));
            mRuleDesTxt.SafeSetText(data.RuleDesc.Replace('|', '\n'));
            mGoBtn.SafeAddOnClickListener(_OnGoBtnClick);
            mPreviewBtn.SafeAddOnClickListener(_OnPreviewBtnClick);
            _InitItems(data);
        }

      

        public void UpdateData(ILimitTimeActivityModel data)
        {
           
        }

        public void Show()
        {
            gameObject.CustomActive(true);
        }
        public void Close()
        {
            Dispose();
            mPreviewItemId = 0;
            mGoBtn.SafeRemoveOnClickListener(_OnGoBtnClick);
            mPreviewBtn.SafeRemoveOnClickListener(_OnPreviewBtnClick);
            Destroy(gameObject);
        }

        public void Dispose()
        {
           
        }

        public void Hide()
        {
            gameObject.CustomActive(false);
        }


        private void _InitItems(ILimitTimeActivityModel model)
        {
            if (model.ParamArray != null)
            {
                mLeftDropList.Clear();
                mRightDropList.Clear();
                for (int i = 0; i < model.ParamArray.Length-1; i++)
                {
                    int itemId =(int) model.ParamArray[i];
                    if(i< mSplitNum)
                    {
                        mLeftDropList.Add(itemId);
                    }
                    else
                    {
                        mRightDropList.Add(itemId);
                    }
                 
                    if(mLeftDrop!=null)
                    {
                        mLeftDrop.SetDropList(mLeftDropList, 0);
                    }
                    if(mRightDrop!=null)
                    {
                        mRightDrop.SetDropList(mRightDropList,0);
                    }
                }
                int previewItemId =(int)model.ParamArray[model.ParamArray.Length - 1];
                mPreviewItemId = previewItemId;
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(mPreviewItemId);
                ComItem comItem = ComItemManager.Create(mPreviewTrans.gameObject);

                if(itemData!=null&&comItem!=null)
                {
                    comItem.GetComponent<RectTransform>().sizeDelta = mPreviewItemSize;
                    comItem.Setup(itemData, Utility.OnItemClicked);
                }

            }
        
        }


        private void _OnGoBtnClick()
        {
            Parser.NpcParser.OnClickLinkByNpcId(2095);
            ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
        }

        private void _OnPreviewBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, mPreviewItemId);
        }

        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
        }


    }
}
    
