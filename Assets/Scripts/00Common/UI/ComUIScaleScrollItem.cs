using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{


    public class ComUIScaleScrollItem : MonoBehaviour
    {
		[SerializeField] private Text mText;
		[SerializeField] private Text mTextSelect;
		[SerializeField] private Image mImgBg;

		public void Init(string content)
		{
			mText.SafeSetText(content);
			mTextSelect.SafeSetText(content);
			mText.CustomActive(true);
			mTextSelect.CustomActive(false);
		}
		
		public void SetTextAlpha(float alpha, bool isSelect)
		{
			mTextSelect.CustomActive(isSelect);
			mText.CustomActive(!isSelect);
			var color = mText.color;
			color.a = alpha;
			mText.color = color;
		}
		
		public void SetImgAlpha(float alpha)
		{
			var color = mImgBg.color;
			color.a = alpha;
			mImgBg.color = color;
		}

    }
}

