using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EDragGroup
{
    None = 0,
    SkillTreeGroup,
    SkillConfigGroup,
}

[RequireComponent(typeof(Image))]
public class Drag_Me : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int id = 0;  // 拖拽物体的附带参数,使用者自定义含义,一般为id
    public EDragGroup DragGroup = EDragGroup.None; // 拖拽的来源分组.
    public int GroupIndex = -1; // 如果有拖拽的来源分组，那么初始化时，可以在预制体里即可将其赋值,也可以代码里赋值初始化.
    public Image OtherDragImg = null; // 除了拖拽的图标以外，如果子物体也要拖拽，那么可以在编辑器里将其赋值，如果要拖拽很多子物体，以后可以再扩充.
    public bool ForbidDrag = false; // 强制禁止拖拽
    public bool dragOnSurfaces = true;

    public delegate bool OnResDrag(PointerEventData eventData);
    public OnResDrag ResponseDrag;

    private Dictionary<int,GameObject> m_DraggingIcons = new Dictionary<int, GameObject>();
	private Dictionary<int, RectTransform> m_DraggingPlanes = new Dictionary<int, RectTransform>();
    private bool _isForceEndDrag = false;       //是否在某个拖动的操作中，强制结束了拖动。比如：OnDrag的时候

	public void OnBeginDrag(PointerEventData eventData)
	{
        if (ForbidDrag)
        {
            return;
        }

        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
        {
            return;
        }

        bool bAdapterCanvas = false;
        int  iCanvasLayer = 0;

        if(canvas.isRootCanvas == false && canvas.overrideSorting == true)
        {
            bAdapterCanvas = true;
            iCanvasLayer = canvas.sortingOrder;
            canvas = FindRootCanvas(gameObject);
        }

        GameObject iconback = new GameObject("iconback");
        GameObject icon = new GameObject("icon");

        GameObject preObj;
        if (m_DraggingIcons.TryGetValue(eventData.pointerId * 100,out preObj))
        {
            GameObject.Destroy(preObj);
        }
        if (m_DraggingIcons.TryGetValue(eventData.pointerId * 100 + 1,out preObj))
        {
            GameObject.Destroy(preObj);
        }

        m_DraggingIcons[eventData.pointerId*100] = iconback;
        m_DraggingIcons[eventData.pointerId*100 + 1] = icon;

        GameObject ui2DRoot = GameObject.Find("UIRoot/UI2DRoot");

        iconback.transform.SetParent(ui2DRoot.transform, false);
        icon.transform.SetParent(ui2DRoot.transform, false);

        icon.transform.SetAsLastSibling();

        var imageback = iconback.AddComponent<Image>();
        var image = icon.AddComponent<Image>();

        var groupback = iconback.AddComponent<CanvasGroup>();
        groupback.blocksRaycasts = false;

        var group = icon.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        if (OtherDragImg != null)
        {
            image.sprite = OtherDragImg.sprite;
            image.material = OtherDragImg.material;
            image.rectTransform.sizeDelta = OtherDragImg.rectTransform.sizeDelta;
        }

        Image srcImage = GetComponent<Image>();
        imageback.sprite = srcImage.sprite;
        imageback.material = srcImage.material;
        imageback.rectTransform.sizeDelta = srcImage.rectTransform.sizeDelta;

        if (dragOnSurfaces)
        {
            m_DraggingPlanes[eventData.pointerId * 100] = transform as RectTransform;
        }
        else
        {
            m_DraggingPlanes[eventData.pointerId * 100] = canvas.transform as RectTransform;
        }

        if (bAdapterCanvas)
        {
            Utility.AddUICanvasCom(iconback, iCanvasLayer);
            Utility.AddUICanvasCom(icon, iCanvasLayer);
        }

        SetDraggedPosition(eventData);

	    _isForceEndDrag = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
        //已经强制结束了拖拽
        if (_isForceEndDrag == true)
            return;

        if (ForbidDrag)
        {
            return;
        }

        GameObject objIcon = null;
        m_DraggingIcons.TryGetValue(eventData.pointerId*100, out objIcon);

        if(objIcon == null)
        {
            Logger.LogErrorFormat("[Drag_Me] The given key was not present in the dictionary.key = {0}", eventData.pointerId);
        }
        else
        {
            if(ResponseDrag != null)
            {
                if(!ResponseDrag(eventData))
                {
                    OnEndDrag(eventData);
                    //强制结束拖拽
                    _isForceEndDrag = true;
                    return;
                }        
            }

            SetDraggedPosition(eventData);
        }			
	}

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ForbidDrag)
        {
            return;
        }

        if (m_DraggingIcons[eventData.pointerId*100] != null)
        {
            Destroy(m_DraggingIcons[eventData.pointerId*100]);
        }

        m_DraggingIcons[eventData.pointerId * 100] = null;

        if (m_DraggingIcons[eventData.pointerId * 100 + 1] != null)
        {
            Destroy(m_DraggingIcons[eventData.pointerId * 100 + 1]);
        }

        m_DraggingIcons[eventData.pointerId * 100 + 1] = null;
        // ResponseDrag = null;
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        if (dragOnSurfaces && eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
        {
            m_DraggingPlanes[eventData.pointerId * 100] = eventData.pointerEnter.transform as RectTransform;
        }

        var rtback = m_DraggingIcons[eventData.pointerId * 100].GetComponent<RectTransform>();
        Vector3 globalMousePosBack;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId * 100], eventData.position, eventData.pressEventCamera, out globalMousePosBack))
        {
            rtback.position = globalMousePosBack;
            rtback.rotation = m_DraggingPlanes[eventData.pointerId * 100].rotation;
        }

        var rt = m_DraggingIcons[eventData.pointerId * 100 + 1].GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId * 100], eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlanes[eventData.pointerId * 100].rotation;
        }
    }

    static public T FindInParents<T>(GameObject go) where T : Component
	{
        if (go == null)
        {
            return null;
        }

		var comp = go.GetComponent<T>();
		if (comp != null)
        {
            return comp;
        }
			
		var t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}

		return comp;
	}

    static public Canvas FindRootCanvas(GameObject go) 
	{
        if (go == null)
        {
            return null;
        }

		var canvas = go.GetComponent<Canvas>();
		if (canvas != null && canvas.isRootCanvas)
        {
            return canvas;
        }
			
		var t = go.transform.parent;
		while (t != null)
		{
			canvas = t.gameObject.GetComponent<Canvas>();
            if(canvas != null && canvas.isRootCanvas)
            {
                break;
            }
			t = t.parent;
		}

		return canvas;
	}
}
