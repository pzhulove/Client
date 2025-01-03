
using GameClient;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.UI
{
	public class ComUIScaleScrollListParam
	{
		public string Content;
		public bool IsEnable;
	}

    public class ComUIScaleScrollList : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
		[SerializeField] private Transform mRoot;
		[SerializeField] private Transform mTrSelectImg;
		[SerializeField] private string mItemPrefabPath;
		[SerializeField] private float mScale = 0.8f;
		[SerializeField] private float mRateTextAlpha = 0.8f;
		[SerializeField] private float mRateImgAlpha = 0.8f;
		[SerializeField] private float mTextBaseAlpha = 1f;
		[SerializeField] private float mImgBaseAlpha = 1f;
		private GameObject mTemplate;

		private List<ComUIScaleScrollListParam> mDataList;
		private List<RectTransform> mItemTrans;
		// private GameObject mTemplate;
		private float mBeginPos;
		
		private Vector2 mOrginSize;
		private int mMiddleId;
		public Action<int> OnValueChanged;
		public Action<int, GameObject> OnValueChangedWithGO;
		public Action<int, GameObject, bool> OnItemVisible;
		private bool isFrist = true;
		private int mDefaultNum = 0;
		private int mMaxNum = 0;

		public void Init(List<ComUIScaleScrollListParam> dataList)
		{
			isFrist = false;
			if (dataList == null)
				return;
			mMiddleId = 0;
			//mDataList = dataList;
			// var obj = Resources.Load("UI/UiProject/Common/ComScareScrollItem.prefab", typeof(GameObject));
			// _OnAssetLoadSuccess(mItemPrefabPath, mTemplate, 0, 0, null);
			UIManager.GetInstance().LoadObject(GetComponentInParent<ComClientFrame>()?.GetClientFrame() as ClientFrame, mItemPrefabPath, null, _OnAssetLoadSuccess, typeof(GameObject));
        }

        public void Init(int minNum, int maxNum, Action<int> onValuechanged)
        {
			OnValueChanged = onValuechanged;
			if(minNum > maxNum)
				minNum = maxNum;
			mDefaultNum = minNum;
			mMaxNum = maxNum;
            List<ComUIScaleScrollListParam> dataList = new List<ComUIScaleScrollListParam>();
            for (int index = 1; index <= maxNum; ++index)
            {
                ComUIScaleScrollListParam param = new ComUIScaleScrollListParam();
                param.Content = index.ToString();
                param.IsEnable = mDefaultNum <= index;
                dataList.Add(param);
            }
			mDataList = dataList;
			if(isFrist)
            	Init(dataList);
			else
				_SetDefaultNum();
        }
		private int mDefaultSetId = -1;
		public void UpdateData(int minNum)
		{
			if(isFrist && null == mDataList)
				return;
            for (int index = 0 ;index < mDataList.Count ;++index)
			{
				var data = mDataList[index];
				var count = mDataList.Count;
				if(int.TryParse(data.Content,out count))
				{
					data.IsEnable = count >= minNum;
				}
            }
            mDefaultNum = minNum > mMaxNum ? mMaxNum : minNum;
            _SetDefaultNum();
		}

        public int GetMiddleId()
		{
			return mMiddleId;
		}

        public void OnBeginDrag(PointerEventData eventData)
        {
			mBeginPos = Input.mousePosition.y;
        }

        public void OnDrag(PointerEventData eventData)
        {
			var pressY = Input.mousePosition.y;
			if (pressY - mBeginPos >= mOrginSize.y / 2 && mMiddleId < mDataList.Count - 1)
			{
				mBeginPos = pressY;
				_UpdateScroll(true);
			}
			else if (pressY - mBeginPos <= -mOrginSize.y / 2 && mMiddleId > 0)
			{
				mBeginPos = pressY;
				_UpdateScroll(false);
			}
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

		private void _UpdateScroll(bool isUp)
		{
			int tempId = mMiddleId;
			if(isUp)
				++tempId;
			else
				--tempId;
			if(!mDataList[tempId].IsEnable)
				return;
			if (isUp)
			{
				_UpdateItems(mMiddleId++);
			}
			else
			{
				_UpdateItems(mMiddleId--);
			}

			if(null != OnValueChanged)
				OnValueChanged(mMiddleId + 1);
				
			if (OnValueChangedWithGO != null)
				OnValueChangedWithGO(mMiddleId, mItemTrans[mMiddleId].gameObject);
		}

		private void _UpdateItems(int oldId)
		{
			for (int i = 0; i < mItemTrans.Count; ++i)
			{
				var pos = mItemTrans[i].anchoredPosition;
				var scale = Mathf.Pow(mScale, Mathf.Abs(i - mMiddleId));
				var oldScale = Mathf.Pow(mScale, Mathf.Abs(i - oldId));
				var itemSize = mItemTrans[i].sizeDelta * oldScale;
				if (i > mMiddleId)
				{
					pos.y = mPosYList[Mathf.Abs(i - mMiddleId)];// mOrginSize.y * (scale + oldScale) / 2;
				}
				else
				{
					pos.y = 2 * mPosYList[0] - mPosYList[Mathf.Abs(i - mMiddleId)];// mOrginSize.y * (scale + oldScale) / 2;
				}
				mItemTrans[i].anchoredPosition = pos;
				var canvasGroup = mItemTrans[i].gameObject.GetOrAddComponent<CanvasGroup>();
				canvasGroup.alpha = scale;
				var localScale = Vector3.one * scale;
				localScale.x = 1f;
				mItemTrans[i].localScale = localScale;

                var itemScript = mItemTrans[i].GetComponent<ComUIScaleScrollItem>();
                if (null != itemScript)
                {
                    var textAlpha = mTextBaseAlpha * Mathf.Pow(mRateTextAlpha, Mathf.Abs(i - mMiddleId));
                    var imgAlpha = mImgBaseAlpha * Mathf.Pow(mRateImgAlpha, Mathf.Abs(i - mMiddleId));
                    itemScript.SetTextAlpha(textAlpha, i == mMiddleId);
                    itemScript.SetImgAlpha(imgAlpha);
                }
            }
			mTrSelectImg.position = mItemTrans[mMiddleId].position;
		}

		private void Clear()
		{

		}

		private List<float> mPosYList;
        protected virtual void _OnAssetLoadSuccess(string path, object asset, object userData)
        {
            if (asset != null && asset is GameObject)
            {
                var root = asset as GameObject;
                mTemplate = root;
                mTemplate.transform.SetParent(mRoot, false);

                if (mDataList != null)
				{
					mPosYList = new List<float>(mDataList.Count);
					mOrginSize = (mTemplate.transform as RectTransform).sizeDelta;
					mItemTrans = new List<RectTransform>(mDataList.Count);
					var scale = 1f;
					float y = 0f;
					for (int i = 0; i < mDataList.Count; ++i)
					{
						var obj = GameObject.Instantiate(mTemplate, mRoot);
						var script = obj.GetComponent<ComUIScaleScrollItem>();
						script.Init(mDataList[i].Content);
                        var textAlpha = mTextBaseAlpha * Mathf.Pow(mRateTextAlpha, Mathf.Abs(i));
                        var imgAlpha = mImgBaseAlpha * Mathf.Pow(mRateImgAlpha, Mathf.Abs(i));
                        script.SetTextAlpha(textAlpha, i == 0);
                        script.SetImgAlpha(imgAlpha);
						var localScale = Vector3.one * scale;
						localScale.x = 1f;
						obj.transform.localScale = localScale;

						var pos = (obj.transform as RectTransform).anchoredPosition;
						pos.y = y;
						mPosYList.Add(pos.y);
						(obj.transform as RectTransform).anchoredPosition = pos;
						y = pos.y - mOrginSize.y * (scale * (1 + mScale)) / 2;

						var canvasGroup = obj.GetOrAddComponent<CanvasGroup>();
						canvasGroup.alpha = scale;
						mItemTrans.Add(obj.transform as RectTransform);
						if (OnItemVisible != null)
						{
							OnItemVisible(i, obj, mDataList[i].IsEnable);
						}
						scale *= mScale;
					}
				}
				mTemplate.CustomActive(false);
				if (mDefaultSetId != -1)
				{
					UpdateData(mDefaultSetId);
				}
				else
				{
					_SetDefaultNum();
				}
            }
        }

        private void _SetDefaultNum()
        {
			if (null == mItemTrans)
			{
				mDefaultSetId = mDefaultNum;
				return;
			}
            while (mDefaultNum > mMiddleId + 1)
            {
                _UpdateItems(mMiddleId++);
            }
            while (mDefaultNum < mMiddleId + 1)
            {
                _UpdateItems(mMiddleId--);
            }
        }
    }
}

