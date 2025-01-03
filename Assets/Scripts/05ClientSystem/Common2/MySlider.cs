using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
class MySlider : MonoBehaviour, IPointerDownHandler, IDragHandler,IEndDragHandler
{
    public RectTransform sliderZone;
    public RectTransform slider;
    public RectTransform canvas;
    public SliderType m_eSliderType;
    public delegate void OnValueChanged();
    public OnValueChanged onValueChanged;

    public Image imgSlider;
    public Image imgSliderZone;

    Vector2 offset;
    bool bDrag;

    public enum SliderType
    {
        ST_HORIZEN = 0,
        ST_VERTICAL,
    }
    float fValue = 0.0f;
    public float Value
    {
        get { return fValue; }
        set
        {
            fValue = value;
            _AlignSlider();

            if (onValueChanged != null)
            {
                onValueChanged.Invoke();
            }
        }
    }

    public void Start()
    {
        sliderZone.anchorMin = sliderZone.anchorMax;
        slider.anchorMin = slider.anchorMax;
        _AlignSlider();
        bDrag = false;
        offset = Vector2.zero;
    }

    void _AlignSlider()
    {
        if (m_eSliderType == SliderType.ST_HORIZEN)
        {
            var sx = canvas.sizeDelta.x * sliderZone.anchorMin.x + sliderZone.anchoredPosition.x - sliderZone.sizeDelta.x * sliderZone.pivot.x + fValue * sliderZone.sizeDelta.x;
            slider.anchoredPosition = new Vector2(sx - canvas.sizeDelta.x * slider.anchorMin.x, slider.anchoredPosition.y);
        }
        else
        {
            var sy = canvas.sizeDelta.y * sliderZone.anchorMin.x + sliderZone.anchoredPosition.y - sliderZone.sizeDelta.y * sliderZone.pivot.y + fValue * sliderZone.sizeDelta.y;
            slider.anchoredPosition = new Vector2(slider.anchoredPosition.x, sy - canvas.sizeDelta.y * slider.anchorMin.y);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 mouseDown = eventData.position;    //记录鼠标按下时的屏幕坐标
        Vector2 mouseUguiPos = new Vector2();   //定义一个接收返回的ugui坐标

        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, mouseDown, eventData.enterEventCamera, out mouseUguiPos);
        if (isRect)   //如果在
        {
            bDrag = true;
            //计算图片中心和鼠标点的差值
            if (m_eSliderType == SliderType.ST_HORIZEN)
            {
                offset = slider.anchoredPosition - new Vector2(mouseUguiPos.x, 0);
            }
            else
            {
                offset = slider.anchoredPosition - new Vector2(0, mouseUguiPos.y);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!bDrag)
        {
            return;
        }
        Vector2 mouseDrag = eventData.position;   //当鼠标拖动时的屏幕坐标
        Vector2 uguiPos = new Vector2();   //用来接收转换后的拖动坐标
                                           //和上面类似
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, mouseDrag, eventData.enterEventCamera, out uguiPos);
        if (isRect)
        {
            Vector2 min = sliderZone.offsetMin;
            Vector2 max = sliderZone.offsetMax;
            float fPreValue = fValue;

            //设置图片的ugui坐标与鼠标的ugui坐标保持不变
            if (m_eSliderType == SliderType.ST_HORIZEN)
            {
                var minx = sliderZone.anchoredPosition.x - sliderZone.pivot.x * sliderZone.sizeDelta.x;
                var maxx = minx + sliderZone.sizeDelta.x;
                uguiPos.x = Mathf.Clamp(uguiPos.x, minx, maxx);
                slider.anchoredPosition = offset + new Vector2(uguiPos.x, 0);
                fValue = (uguiPos.x - minx) / (maxx - minx);
            }
            else
            {
                var miny = sliderZone.anchoredPosition.y - sliderZone.pivot.y * sliderZone.sizeDelta.y;
                var maxy = miny + sliderZone.sizeDelta.y;
                uguiPos.y = Mathf.Clamp(uguiPos.y, miny, maxy);
                slider.anchoredPosition = offset + new Vector2(0, uguiPos.y);
                fValue = (uguiPos.y - miny) / (maxy - miny);
            }

            if(!Equals(fValue,fPreValue))
            {
                if (onValueChanged != null)
                {
                    onValueChanged.Invoke();
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        offset = Vector2.zero;
        bDrag = false;
    }
}