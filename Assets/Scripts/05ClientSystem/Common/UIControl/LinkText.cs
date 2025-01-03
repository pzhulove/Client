using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LinkText : MonoBehaviour
{

    private Text linkText;

    void Awake()
    {
        linkText = transform.Find("Text").GetComponent<Text>();

    }
    void Start()
    {
        CreateLink(linkText);
    }

    public void CreateLink(Text text)
    {
        if (text == null)
            return;

        //��¡Text�������ͬ������  
        Text underline = Instantiate(text) as Text;
        underline.name = "Underline";

        underline.transform.SetParent(text.transform);
        RectTransform rt = underline.rectTransform;
        rt.localScale = new Vector3(1.0f ,1.0f, 1.0f);

        //�����»��������λ��  
        rt.anchoredPosition3D = Vector3.zero;
        rt.offsetMax = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;

        underline.text = "_";
        float perlineWidth = underline.preferredWidth;      //�����»��߿��  
        underline.alignment = TextAnchor.LowerCenter;
        underline.raycastTarget = false;
        //Debug.Log(perlineWidth);

        float width = text.preferredWidth;
        //Debug.Log(width);
        int lineCount = (int)Mathf.Round(width / perlineWidth);
        //Debug.Log(lineCount);
        for (int i = 1; i < lineCount; i++)
        {
            underline.text += "_";
        }
    }
}