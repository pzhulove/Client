using DG.Tweening;
using ProtoTable;
using Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class PackageDecomposePanel : MonoBehaviour,IDisposable
    {
        public delegate void OnTogglesValueChanged(int id, bool isOn);
        public delegate void OnBtnClicked();
        [SerializeField] private Text mTextTitle;
        [SerializeField] private Text mTextTopTip;
        [SerializeField] private Text mTextEmptyTip;
        [SerializeField] private Text mTextResultTip;
        [SerializeField] private Text mTextConfirm;
        [SerializeField] private Text mTextPrecious;

        [SerializeField] private Button mComfirmBtn;
        [SerializeField] private Button mCloseBtn;
        //[SerializeField] private Button mCloseBtnFork;
        [SerializeField] private Button mBtnResultTip;
        [SerializeField] private Transform mToggleGroup;
        [SerializeField] private CanvasGroup mCanvasGroup;
        [SerializeField] private ComUIListScript mComUIListPrecious;
        [SerializeField] private ComUIListScript mComUIListResult;
        [SerializeField] private List<DOTweenAnimation> mOpenAnis;
        [SerializeField] private float mExpandHeight;
        [SerializeField] private int mCountFontSize = 26;
        public Rect HoleRect;
        private float mOriginHeight;

        //private PackageDisassembleMsgPanel disassembleMsgPanel;
        private List<ItemData> mPreciousItemList;
        //private Dictionary<int, RandData> mRandDataDic = new Dictionary<int, RandData>();
        private Dictionary<int, KeyValuePair<int, int>> mResultItemDic; 

        private Toggle[] mToggles;
	    private OnTogglesValueChanged mOnTogglesValueChanged;
        private bool mIsShowName = false;

        void Awake()
        {
            mOriginHeight = mComUIListPrecious.rectTransform().sizeDelta.y;
        }

		public void Init(string resultPrefabPath)
        {
            _InitToggles();
	        _RegisterToggleEvents();
            mComUIListPrecious.Initialize();
            mComUIListPrecious.onItemVisiable = _OnItemVisible;
            mComUIListPrecious.OnItemUpdate = _OnItemVisible;

            mComUIListResult.InitialLizeWithExternalElement(resultPrefabPath);
            mComUIListResult.onItemVisiable = _OnItemVisibleResult;
            mComUIListResult.OnItemUpdate = _OnItemVisibleResult;

        }

        public void SetEvents(OnTogglesValueChanged onToggleValueChanged, UnityAction onDecomposeBtnClicked, UnityAction onCloseBtnClicked)
        {
            var trans = (mComUIListPrecious.transform as RectTransform);
            var size = trans.sizeDelta;

            if (onToggleValueChanged == null)
            {
                mToggleGroup.gameObject.CustomActive(false);
                size.y = mExpandHeight;
            }
            else
            {
                mToggleGroup.gameObject.CustomActive(true);
                size.y = mOriginHeight;
            }
            trans.sizeDelta = size;

            mOnTogglesValueChanged = onToggleValueChanged;
            mComfirmBtn.SafeRemoveAllListener();
            mComfirmBtn.SafeAddOnClickListener(onDecomposeBtnClicked);
            mCloseBtn.SafeRemoveAllListener();
            mCloseBtn.SafeAddOnClickListener(onCloseBtnClicked);
            //mOnItemSelect = onItemSelect;
        }

        public void SetShowName(bool isShowName)
        {
            mIsShowName = isShowName;
        }

        private void _OnConfirmClick()
        {

        }

        public void SetTexts(string title, string topTip, string emptyTip, string resultTip, string preciousTip, string confirm, bool isShowResultTipButton)
        {
            mTextTitle.SafeSetText(title);
            mTextTopTip.SafeSetText(topTip);
            mTextEmptyTip.SafeSetText(emptyTip);
            mTextResultTip.SafeSetText(resultTip);
            mTextConfirm.SafeSetText(confirm);
            mTextPrecious.SafeSetText(preciousTip);
            mBtnResultTip.CustomActive(isShowResultTipButton);
        }

        public void SetToggleOn(int id, bool value)
        {
            if (mToggles != null && id >= 0 && id < mToggles.Length)
            {
                mToggles[id].isOn = value;
            }
        }

		public void Show()
		{
            if (gameObject != null && gameObject.activeInHierarchy)
            {
                _ResetFilter();
            }
            mCanvasGroup.CustomActive(true);
            _PlayOpenAnis();
        }

		public void Hide()
		{
            _PlayOpenAnis(false);
            _ClosePanel();
        }

		public bool IsActive()
		{
			return mCanvasGroup.alpha > Mathf.Epsilon;
		}

	    public void ClearFilter()
	    {
		    if (mToggles == null)
			    return;
		    _UnRegisterTogglesEvent();

			for (int i = 0; i < mToggles.Length; i++)
		    {
				if (mToggles[i] == null)
					continue;

			    mToggles[i].isOn = false;
		    }

		    _RegisterToggleEvents();
	    }

	    private void _RegisterToggleEvents()
	    {
		    for (int i = 0; i < mToggles.Length; i++)
		    {
                mToggles[i].SafeAddOnValueChangedListener(_ToggleListener(i));
            }
		}

        public void UpdateResultList(Dictionary<int, KeyValuePair<int, int>> dic)
        {
            mResultItemDic = dic;
            if (mResultItemDic == null)
            {
                mComUIListResult.SetElementAmount(0);
            }
            else
            {
                mComUIListResult.SetElementAmount(mResultItemDic.Count);
            }
        }

        public void UpdatePreciousItemList(List<ItemData> list)
        {
            mPreciousItemList = list;
            if (mPreciousItemList == null)
            {
                mComUIListPrecious.SetElementAmount(0);
                mTextPrecious.enabled = false;
            }
            else
            {
                mTextPrecious.enabled = mPreciousItemList.Count > 0 ? true : false;
                mComUIListPrecious.SetElementAmount(mPreciousItemList.Count);
            }
           
        }
		
		public void Dispose()
		{
			_UnRegisterEvent();
            mPreciousItemList?.Clear();
            mResultItemDic?.Clear();
            mResultItemDic = null;
            mPreciousItemList = null;
        }

        public int GetToggleCount()
        {
            return mToggles == null? 0 : mToggles.Length;
        }

        public bool GetToggleValue(int i)
        {
            return mToggles == null && i >= 0 && i < mToggles.Length ? false : mToggles[i].isOn;
        }

        private void _ResetFilter()
        {
            if (mToggles == null)
                return;

            if (mToggles.Length == 0)
                return;

            for (int i = 0; i < mToggles.Length; ++i)
            {
                mToggles[i].isOn = false;
            }

        }

		private UnityEngine.Events.UnityAction<bool> _ToggleListener(int id)
        {
            return (val) => _OnToggleValueChanged(val, id);
        }

		private void _OnToggleValueChanged(bool value, int id)
		{
			mOnTogglesValueChanged(id, value);
		}

        private void _PlayOpenAnis(bool isOpen = true)
        {
            if (mOpenAnis == null)
            {
                return;
            }

            foreach (var ani in mOpenAnis)
            {
                if (ani == null)
                {
                    continue;
                }

                if (isOpen)
                {
                    ani.DOPlayForward();
                }
                else
                {
                    ani.DOPlayBackwards();
                }
            }
        }

        private void _ClosePanel()
        {
            mResultItemDic?.Clear();
            mComUIListPrecious.SetElementAmount(0);
            mComUIListResult.SetElementAmount(0);
            mCanvasGroup.CustomActive(false);
            mTextPrecious.enabled = false;
            mBtnResultTip.CustomActive(false);
            ClearFilter();
        }

		private void _UnRegisterEvent()
		{
			_UnRegisterTogglesEvent();
            mComfirmBtn.SafeRemoveAllListener();
            mCloseBtn.SafeRemoveAllListener();
        }

	    private void _UnRegisterTogglesEvent()
	    {
		    for (int i = 0; i < mToggles.Length; i++)
		    {
                mToggles[i].RemoveAllCallback();
            }
		}

        private void _OnItemVisible(ComUIListElementScript item)
        {
            if (item == null || item.m_index >= mPreciousItemList.Count)
                return;

            var script = item.GetComponentInChildren<ComItemNew>();
            if (script == null)
                return;
            var model = mPreciousItemList[item.m_index];
            script.Setup(model, null, true);
        }

        private void _OnItemVisibleResult(ComUIListElementScript item)
        {
            if (item == null || item.m_index >= mResultItemDic.Count)
                return;

            var script = item.GetComponentInChildren<ComItemNew>();
            var data = mResultItemDic.ElementAt(item.m_index);
            if (script != null)
            {
                var model = ItemDataManager.GetInstance().CreateItem(data.Key, 1);
                script.Setup(model, null, true);

                if (data.Value.Key == data.Value.Value)
                {
                    if (data.Value.Key > 1)
                    {
                        script.SetCount(data.Value.Key.ToString());
                    }
                }
                else
                {
                    script.SetCount(TR.Value("package_decompose_material_count", data.Value.Key, data.Value.Value));
                }

                if(mIsShowName)
                {
                    script.ShowName();
                }

                script.SetCountSize(mCountFontSize);
            }
            else
            {
                var bind = item.GetComponent<ComCommonBind>();
                if (bind != null)
                {
                    var imageIcon = bind.GetCom<Image>("icon");
                    var textCount = bind.GetCom<Text>("cnt");
                    var tableData = TableManager.GetInstance().GetTableItem<ItemTable>(data.Key);
                    if (tableData != null)
                    {
                        ETCImageLoader.LoadSprite(ref imageIcon, tableData.Icon);
                    }
                    textCount.SafeSetText(data.Value.Key.ToString());
                }
            }
        }

        private void _InitToggles()
        {
            mToggles = mToggleGroup.GetComponentsInChildren<Toggle>();
        }
	}

}
