using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class Drop_Me : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image containerImage;
	public Image receivingImage;
    public bool bHighLight = false; // 是否高亮
    public bool bAutoReplace = false; // 是否自动置换图片,一般没有逻辑处理的时候置为true
    public bool bIsDropDelete = false; // 拖拽丢弃的时候勾选上
    public Color highlightColor = Color.yellow;

    public delegate bool OnResDrop(PointerEventData DragData, GameObject ReceiveImgObj);// 参数1为拖拽的物体，参数2为接受拖拽的物体
    public OnResDrop ResponseDrop;

    private Color normalColor;

    public void OnEnable ()
	{
		if (containerImage != null)
        {
            normalColor = containerImage.color;
        }		
	}
	
	public void OnDrop(PointerEventData data)
	{
        if(containerImage!=null)
		containerImage.color = normalColor;
		
		if (receivingImage == null)
        {
            return;
        }		
		    
		Sprite dropSprite = GetDropSprite (data);

		if (dropSprite != null)
        {
            if(bAutoReplace)
            {
                receivingImage.sprite = dropSprite;
            }

            // 需要什么样参数的委托函数都可以自定义设置，然后在这里触发
            if (ResponseDrop != null)
            {
                ResponseDrop(data, receivingImage.gameObject);
            }

            //ResponseDrop = null;
        }
    }

	public void OnPointerEnter(PointerEventData data)
	{
		if (containerImage == null)
        {
            return;
        }			
		
		Sprite dropSprite = GetDropSprite (data);

		if (dropSprite != null && bHighLight)
        {
            containerImage.color = highlightColor;
        }		
	}

	public void OnPointerExit(PointerEventData data)
	{
		if (containerImage == null)
        {
            return;
        }
			
		containerImage.color = normalColor;
	}
	
	private Sprite GetDropSprite(PointerEventData data)
	{
		var originalObj = data.pointerDrag;
		if (originalObj == null)
        {
            return null;
        }		
		
		var dragMe = originalObj.GetComponent<Drag_Me>();
		if (dragMe == null)
        {
            return null;
        }		
		
		var srcImage = originalObj.GetComponent<Image>();
		if (srcImage == null)
        {
            return null;
        }		
		
		return srcImage.sprite;
	}
}
