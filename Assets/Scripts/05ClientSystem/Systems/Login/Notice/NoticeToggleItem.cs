using System;
using UnityEngine;
using UnityEngine.UI;


namespace GameClient
{
    public class NoticeToggleItem : MonoBehaviour
    {
		[SerializeField] private Text mTextDesc;
        [SerializeField] private Text mTextMarkDesc;
        [SerializeField] private Image mSelect;
        [SerializeField] private Image mNormal;

        public void Init(NoticeToggleData data)
		{
            if (data == null)
            {
                return;
            }

            mTextDesc.SafeSetText(data.toggleName);
            mTextMarkDesc.SafeSetText(data.toggleName);
        }

        public void Selected(bool isSelect)
        {
            mSelect.CustomActive(isSelect);
            mNormal.CustomActive(!isSelect);
        }
    }
}