using DG.Tweening;
using Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class PackageOperateConfirmView : MonoBehaviour,IDisposable
    {
        public delegate void OnTogglesValueChanged(int id, bool isOn);
        public delegate void OnBtnClicked();
        [SerializeField] private Text mTextTitle;
        [SerializeField] private Text mTextTopTip;

        //[SerializeField] private Button mCloseBtnFork;
        [SerializeField] private ComUIListScript mComUIList;

        private Action mOnConfrimClicked;
        private Action mOnCancelClicked;
        private Dictionary<int, KeyValuePair<int, int>> mResultItemDic;

        public void Init(Dictionary<int, KeyValuePair<int, int>> itemDic, string title, string tip, Action onConfrimClick, Action onCancelClick)
        {
            mResultItemDic = itemDic;
            mComUIList.Initialize();
            mComUIList.onItemVisiable = _OnItemVisible;
            mComUIList.OnItemUpdate = _OnItemVisible;
            mTextTitle.SafeSetText(title);
            mTextTopTip.SafeSetText(tip);
            mOnConfrimClicked = onConfrimClick;
            mOnCancelClicked = onCancelClick;
            mComUIList.SetElementAmount(mResultItemDic.Count);
        }

        public void Dispose()
		{
			_UnRegisterEvent();

            mResultItemDic?.Clear();
            mResultItemDic = null;
        }

        public void OnConfirmClick()
        {
            mOnConfrimClicked?.Invoke();
        }

        public void OnCloseClick()
        {
            mOnCancelClicked?.Invoke();
        }

        private void _ClosePanel()
        {
            mComUIList.SetElementAmount(0);
        }

		private void _UnRegisterEvent()
		{
        }

        private void _OnItemVisible(ComUIListElementScript item)
        {
            if (item == null || item.m_index >= mResultItemDic.Count)
                return;

            var script = item.GetComponentInChildren<ComItemNew>();
            if (script == null)
                return;
            var data = mResultItemDic.ElementAt(item.m_index);
            var model = ItemDataManager.GetInstance().CreateItem(data.Key, 1);
            script.Setup(model, null, true);
            if (data.Value.Key == data.Value.Value)
            {
                script.SetCount(data.Value.Key.ToString());
            }
            else
            {
                script.SetCount(TR.Value("package_decompose_material_count", data.Value.Key, data.Value.Value));
            }
        }
	}

}
