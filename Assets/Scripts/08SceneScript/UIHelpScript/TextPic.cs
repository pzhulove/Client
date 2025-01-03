using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Reflection;

public class TextPic : Text, IPointerClickHandler
{
    /// <summary>
    /// 图片池
    /// </summary>
    private readonly List<Image> m_ImagesPool = new List<Image>();

    /// <summary>
    /// 图片的最后一个顶点的索引
    /// </summary>
    private readonly List<int> m_ImagesVertexIndex = new List<int>();

    /// <summary>
    /// 正则取出所需要的属性
    /// </summary>
    private static readonly Regex s_Regex =
        new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />", RegexOptions.Singleline);

    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();
        UpdateQuadImage();
    }

    /// <summary>
    /// 解析完最终的文本
    /// </summary>
    private string m_OutputText;


    protected void UpdateQuadImage()
    {
#if UNITY_EDITOR
        if (UnityEditor.PrefabUtility.GetPrefabType(this) == UnityEditor.PrefabType.Prefab)
        {
            return;
        }
#endif
        m_OutputText = Parse();
        m_ImagesVertexIndex.Clear();
        foreach (Match match in s_Regex.Matches(m_OutputText))
        {
            var picIndex = match.Index + match.Length - 1;
            var endIndex = picIndex * 4 + 3;
            m_ImagesVertexIndex.Add(endIndex);

            m_ImagesPool.RemoveAll(image => image == null);
            if (m_ImagesPool.Count == 0)
            {
                GetComponentsInChildren<Image>(m_ImagesPool);
            }
            if (m_ImagesVertexIndex.Count > m_ImagesPool.Count)
            {
                var resources = new DefaultControls.Resources();
                var go = DefaultControls.CreateImage(resources);
                go.layer = gameObject.layer;
                var rt = go.transform as RectTransform;
                if (rt)
                {
                    rt.SetParent(rectTransform);
                    rt.localPosition = Vector3.zero;
                    rt.localRotation = Quaternion.identity;
                    rt.localScale = Vector3.one;
                    rt.pivot = Vector2.zero;
                }
                m_ImagesPool.Add(go.GetComponent<Image>());
            }

            var spriteName = match.Groups[1].Value;
            var size = float.Parse(match.Groups[2].Value);
            var img = m_ImagesPool[m_ImagesVertexIndex.Count - 1];
            if (img.sprite == null || img.sprite.name != spriteName)
            {
                // img.sprite = AssetLoader.instance.LoadRes(spriteName, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref img, spriteName);
                img.rectTransform.pivot = Vector2.zero;
            }
            img.rectTransform.sizeDelta = new Vector2(size, size);
            img.enabled = true;
        }

        for (var i = m_ImagesVertexIndex.Count; i < m_ImagesPool.Count; i++)
        {
            if (m_ImagesPool[i])
            {
                m_ImagesPool[i].enabled = false;
            }
        }
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        var orignText = m_Text;
        m_Text = m_OutputText;
        base.OnPopulateMesh(toFill);
        m_Text = orignText;

        UIVertex vert = new UIVertex();
        for (var i = 0; i < m_ImagesVertexIndex.Count; i++)
        {
            var endIndex = m_ImagesVertexIndex[i];
            var rt = m_ImagesPool[i].rectTransform;
            var size = rt.sizeDelta;
            if (endIndex < toFill.currentVertCount)
            {
                toFill.PopulateUIVertex(ref vert, endIndex);
                rt.anchoredPosition = new Vector2(vert.position.x - rt.sizeDelta.x * rt.pivot.x, vert.position.y - rt.sizeDelta.y * rt.pivot.y);

                // 抹掉左下角的小黑点
                toFill.PopulateUIVertex(ref vert, endIndex - 3);
                var pos = vert.position;
                for (int j = endIndex, m = endIndex - 3; j > m; j--)
                {
                    toFill.PopulateUIVertex(ref vert, endIndex);
                    vert.position = pos;
                    toFill.SetUIVertex(vert, j);
                }
            }
        }

        if (m_ImagesVertexIndex.Count != 0)
        {
            m_ImagesVertexIndex.Clear();
        }

        // 处理超链接包围框
        foreach (var hrefInfo in m_HrefInfos)
        {
            hrefInfo.boxes.Clear();
            if (hrefInfo.startIndex >= toFill.currentVertCount)
            {
                continue;
            }

            // 将超链接里面的文本顶点索引坐标加入到包围框
            toFill.PopulateUIVertex(ref vert, hrefInfo.startIndex);
            var pos = vert.position;
            var bounds = new Bounds(pos, Vector3.zero);
            for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++)
            {
                if (i >= toFill.currentVertCount)
                {
                    break;
                }

                toFill.PopulateUIVertex(ref vert, i);
                pos = vert.position;
                if (pos.x < bounds.min.x) // 换行重新添加包围框
                {
                    hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                    bounds = new Bounds(pos, Vector3.zero);
                }
                else
                {
                    bounds.Encapsulate(pos); // 扩展包围框
                }
            }
            hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
        }
    }

    /// <summary>
    /// 超链接信息列表
    /// </summary>
    private readonly List<HrefInfo> m_HrefInfos = new List<HrefInfo>();

    /// <summary>
    /// 文本构造器
    /// </summary>
    private static readonly StringBuilder s_TextBuilder = new StringBuilder();

    /// <summary>
    /// 超链接正则
    /// </summary>
    private static readonly Regex s_HrefRegex =
        new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

    [Serializable]
    public class HrefClickEvent : UnityEvent<string> { }

    [SerializeField]
    private HrefClickEvent m_OnHrefClick = new HrefClickEvent();

    /// <summary>
    /// 超链接点击事件
    /// </summary>
    public HrefClickEvent onHrefClick
    {
        get { return m_OnHrefClick; }
        set { m_OnHrefClick = value; }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 lp;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out lp);

        foreach (var hrefInfo in m_HrefInfos)
        {
            var boxes = hrefInfo.boxes;
            for (var i = 0; i < boxes.Count; ++i)
            {
                if (boxes[i].Contains(lp))
                {
                    m_OnHrefClick.Invoke(hrefInfo.key + "|" + hrefInfo.value);
                    return;
                }
            }
        }
    }

    public bool bNeedParserColor = false;
    /// <summary>
    /// 获取超链接解析后的最后输出文本
    /// </summary>
    /// <returns></returns>
    private string Parse()
    {
        s_TextBuilder.Length = 0;
        m_HrefInfos.Clear();
        var indexText = 0;
        foreach (Match match in s_HrefRegex.Matches(text))
        {
            s_TextBuilder.Append(text.Substring(indexText, match.Index - indexText));
            Parser.ParserReturn parseResult = OnParse(match.Groups[1].Value, match.Groups[2].Value);

            if (bNeedParserColor)
            {
                s_TextBuilder.Append(string.Format("<color={0}>", parseResult.color));  // 超链接颜色
            }

            var group = match.Groups[1];
            var hrefInfo = new HrefInfo
            {
                startIndex = s_TextBuilder.Length * 4, // 超链接里的文本起始顶点索引
                endIndex = (s_TextBuilder.Length + parseResult.content.Length - 1) * 4 + 3,
                key = group.Value,
                value = parseResult.iId.ToString(),
            };
            m_HrefInfos.Add(hrefInfo);

            s_TextBuilder.Append(parseResult.content);
            if (bNeedParserColor)
            {
                s_TextBuilder.Append("</color>");
            }
            indexText = match.Index + match.Length;
        }
        s_TextBuilder.Append(text.Substring(indexText, text.Length - indexText));

        return s_TextBuilder.ToString();
    }

    /// <summary>
    /// 超链接信息类
    /// </summary>
    private class HrefInfo
    {
        public int startIndex;
        public int endIndex;
        public string key;
        public string value;

        public readonly List<Rect> boxes = new List<Rect>();
    }

    protected Parser.ParserReturn OnParse(string key, string value)
    {
        Parser.ParserReturn ret;
        ret.color = "0xFFFFFF";
        ret.content = "";
        ret.iId = 0;

        Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
        string parserName = "";
        if (key.Length > 0)
        {
            parserName = key.Substring(0, 1);
            parserName = parserName.ToUpper();

            if (key.Length > 1)
            {
                parserName += key.Substring(1, key.Length - 1);
            }
        }

        if (parserName.Length <= 0)
        {
            return ret;
        }

        string parserClassName = "Parser." + parserName + "Parser";

        object obj = assembly.CreateInstance(parserClassName); // 创建类的实例，返回为 object 类型，需要强制类型转换
        if (obj != null)
        {
            Parser.IParser parser = obj as Parser.IParser;
            if (parser != null)
            {
                return parser.OnParse(value);
            }
        }

        return ret;
    }
}