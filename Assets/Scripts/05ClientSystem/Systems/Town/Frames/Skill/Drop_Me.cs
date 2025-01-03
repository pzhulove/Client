using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class Drop_Me : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image containerImage;
	public Image receivingImage;
    public bool bHighLight = false; // �Ƿ����
    public bool bAutoReplace = false; // �Ƿ��Զ��û�ͼƬ,һ��û���߼������ʱ����Ϊtrue
    public bool bIsDropDelete = false; // ��ק������ʱ��ѡ��
    public Color highlightColor = Color.yellow;

    public delegate bool OnResDrop(PointerEventData DragData, GameObject ReceiveImgObj);// ����1Ϊ��ק�����壬����2Ϊ������ק������
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

            // ��Ҫʲô��������ί�к����������Զ������ã�Ȼ�������ﴥ��
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
