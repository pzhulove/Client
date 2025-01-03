using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class ActivityTimeLimitDayList : MonoBehaviour
{
    [SerializeField] private ComUIListScript mComUIList;
    [SerializeField] private TextEx mTextDay;

    private bool mIsInited = false;
    private bool mIsNeedBg = false;
    private string mStrDay = string.Empty;

    private const float perCharSizeX = 46;

    public void Init(string strDay, bool isNeedBg)
    {
        if (mComUIList == null)
        {
            return;
        }

        gameObject.CustomActive(true);

        if (!mIsInited)
        {
            mComUIList.Initialize();
            mComUIList.onBindItem = (go) =>
            {
                return go;
            };
            mComUIList.onItemVisiable = _OnItemVisiable;

            mIsInited = true;
        }

        mStrDay = strDay;
        mIsNeedBg = isNeedBg;

        mTextDay.CustomActive(!isNeedBg);
        mComUIList.CustomActive(isNeedBg);
        if (isNeedBg)
        {
            int maxDayNumPerLine = (int)(mComUIList.transform.rectTransform().rect.width / perCharSizeX);
            float offset = (mComUIList.transform.rectTransform().rect.width - maxDayNumPerLine * perCharSizeX) / 2;
            float paddingX = ((float)(maxDayNumPerLine - mStrDay.Length)) / 2 * perCharSizeX + offset;
            mComUIList.m_elementPadding = new Vector2(paddingX, 0);
            mComUIList.SetElementAmount(mStrDay.Length);       //生成周几开放的星期几的显示的滚动列表
        }
        else
        {
            mTextDay.SafeSetText(strDay);
        }
    }

    private void _OnItemVisiable(ComUIListElementScript go)
    {
        if (go == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(mStrDay))
        {
            return;
        }

        ComCommonBind comCommonBind = go.GetComponent<ComCommonBind>();
        if (comCommonBind == null)
        {
            return;
        }
        CanvasGroup canvasGroup = comCommonBind.GetCom<CanvasGroup>("bg");
        Text text = comCommonBind.GetCom<Text>("textDay");
        canvasGroup.CustomActive(mIsNeedBg);
        if (go.m_index >= 0 && go.m_index < mStrDay.Length && text != null)
        {
            text.SafeSetText(mStrDay[go.m_index].ToString());
        }
    }
}
