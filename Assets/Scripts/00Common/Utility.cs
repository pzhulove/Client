//using Apollo;
//using Assets.Scripts.Framework;
//using ResData;
using System;
using System.Globalization;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using GameClient;
using System.Text.RegularExpressions;
using Protocol;
using ProtoTable;
using DG.Tweening;
///////删除linq

public static class FixK
{
    public static List<T> ToList<T>(this IList<T> list)
    {
        return new List<T>(list);
    }

    public static List<T> ToList<T>(this IEnumerable<T> collection)
    {
        List<T> newList = new List<T>();
        if (collection != null)
        {
            var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item == null)
                    continue;
                newList.Add(item);
            }
        }
        return newList;
    }

    public static T[] ToArray<T>(this IList<T> list)
    {
        if (list == null || list.Count <= 0)
        {
            return default(T[]);
        }
        T[] newArray = new T[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            newArray[i] = list[i];
        }
        return newArray;
    }

    public static List<T> Distinct<T>(this IList<T> list)
    {
        List<T> cpList = new List<T>();
        if (list != null && list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var rawItem = list[i];
                if (!cpList.Contains(rawItem))
                {
                    cpList.Add(rawItem);
                }
            }
        }
        return cpList;
    }

    public static List<T> Intersect<T>(this IList<T> a, IList<T> b)
    {
        List<T> theSameList = new List<T>();

        if (a != null && a.Count > 0 &&
            b != null && b.Count > 0)
        {
            for (int i = 0; i < a.Count; i++)
            {
                var aItem = a[i];
                for (int j = 0; j < b.Count; j++)
                {
                    var bItem = b[j];
                    if (aItem.Equals(bItem) && !theSameList.Contains(aItem))
                    {
                        theSameList.Add(aItem);
                        break;
                    }
                }
            }
        }
        return theSameList;
    }

    public static List<T> Except<T>(this IList<T> a, IList<T> b)
    {
        List<T> theDiffList = new List<T>();

        if (a != null && a.Count > 0 &&
            b != null && b.Count > 0)
        {
            for (int i = 0; i < a.Count; i++)
            {
                var aItem = a[i];
                for (int j = 0; j < b.Count; j++)
                {
                    var bItem = b[j];
                    if (!aItem.Equals(bItem) && !theDiffList.Contains(aItem))
                    {
                        theDiffList.Add(aItem);
                        break;
                    }
                }
            }
        }
        return theDiffList;
    }
}



public static class StaticUtility
{
    public static bool Contains(this string source, string value, System.StringComparison comparisonType)
    {
        return (source.IndexOf(value, comparisonType) >= 0);
    }

    public static void CustomActive(this MonoBehaviour com, bool bActive)
    {
        if (com == null)
        {
            return;
        }

        com.gameObject.CustomActive(bActive);
    }

    public static void CustomActive(this Transform transform, bool bActive)
    {
        if (transform == null)
        {
            return;
        }

        transform.gameObject.CustomActive(bActive);
    }

    public static void CustomActive(this CanvasGroup canvasGroup, bool bActive)
    {
        if (null == canvasGroup)
        {
            return;
        }

        if ((canvasGroup.alpha == 0 && bActive) || (canvasGroup.alpha == 1 && !bActive))
        {
            canvasGroup.alpha = bActive ? 1.0f : 0.0f;
            canvasGroup.blocksRaycasts = bActive;
        }
    }

    public static void CustomActive(this GameObject gameObject, bool bActive)
    {
        if (gameObject == null)
        {
            return;
        }

        if (gameObject.activeSelf != bActive)
        {
            gameObject.SetActive(bActive);
        }
    }

    public static void CustomActiveAlpha(this Image img, bool bActive)
    {
        if (img == null)
        {
            return;
        }

        var color = img.color;
        color.a = bActive ? 1f : 0f;
        img.color = color;
    }

    public static void CustomActiveAlpha(this Text text, bool bActive)
    {
        if (text == null)
        {
            return;
        }

        var color = text.color;
        color.a = bActive ? 1f : 0f;
        text.color = color;
    }

    public static void SafeAdd<T1, T2>(this Dictionary<T1, T2> dic, T1 key, T2 value)
    {
        if (dic == null)
        {
            return;
        }

        if (dic.ContainsKey(key))
        {
            dic[key] = value;
        }
        else
        {
            dic.Add(key, value);
        }
    }
    public static bool IsNull(this System.Object obj)
    {
        return System.Object.ReferenceEquals(obj, null);
    }

    public static bool IsAnonymousFunction(UnityEngine.Events.UnityAction action)
    {
        if (action == null)
        {
            return false;
        }

        return action.Method.ToString().IndexOf("<") != -1;
    }

    public static void SafeRemove<T1, T2>(this Dictionary<T1, T2> dic, T1 key)
    {
        if (dic == null)
        {
            return;
        }
        if (dic.ContainsKey(key))
        {
            dic.Remove(key);
        }
    }

    public static T2 SafeGetValue<T1, T2>(this Dictionary<T1, T2> dic, T1 key)
    {
        if (dic == null)
        {
            return default(T2);
        }
        if (dic.ContainsKey(key))
        {
            return dic[key];
        }
        return default(T2);
    }
    public static void SafeSetBtnCallBack(ComCommonBind bind, string name, UnityEngine.Events.UnityAction callBack)
    {
        if (bind == null)
        {
            return;
        }

        Button btn = bind.GetCom<Button>(name);
        if (btn != null && callBack != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(callBack);
        }
    }

    public static void SafeRmvBtnCallBack(ComCommonBind bind, string name, UnityEngine.Events.UnityAction callBack)
    {
        if (bind == null)
        {
            return;
        }

        Button btn = bind.GetCom<Button>(name);
        if (btn != null && callBack != null)
        {
            btn.onClick.RemoveListener(callBack);
        }
    }

    public static void SafeSetText(ComCommonBind bind, string name, string text)
    {
        if (bind == null)
        {
            return;
        }

        Text txt = bind.GetCom<Text>(name);
        if (txt != null)
        {
            txt.text = text;
        }
    }

    public static void SafeSetVisible<T>(ComCommonBind bind, string name, bool bVisible) where T : Component
    {
        if (bind == null)
        {
            return;
        }

        T t = bind.GetCom<T>(name);
        if (t != null)
        {
            t.gameObject.CustomActive(bVisible);
        }
    }

    public static void SafeSetVisible(ComCommonBind bind, string name, bool bVisible)
    {
        if (bind == null)
        {
            return;
        }

        GameObject go = bind.GetGameObject(name);
        if (go != null)
        {
            go.CustomActive(bVisible);
        }
    }

    public static void SafeSetImage(ComCommonBind bind, string name, string path)
    {
        if (bind == null)
        {
            return;
        }

        Image img = bind.GetCom<Image>(name);
        if (img != null)
        {
            ETCImageLoader.LoadSprite(ref img, path);
        }
    }

    public static void ReCalculateRectSizeByLayout(RectTransform rect, RectTransform.Axis axis)
    {
        rect.SetSizeWithCurrentAnchors(axis, LayoutUtility.GetPreferredSize(rect, (int)axis));
    }

    public static T SafeAddComponent<T>(this GameObject gameObject, bool deleteOrigin = true)
        where T : MonoBehaviour
    {
        if (null == gameObject)
        {
            return default(T);
        }

        T com = gameObject.GetComponent<T>();

        if (null != com)
        {
            if (deleteOrigin)
            {
                GameObject.Destroy(com);
                com = null;
            }
            else
            {
                return com;
            }

        }

        return gameObject.AddComponent<T>();
    }

    public static T SafeGet<T>(this T[] array, int index) where T : class
    {
        if (index < 0 || index >= array.Length)
        {
            return null;
        }

        return array[index];
    }

    public static void SafeSetText(this Text text, string str, ItemTable.eColor color = ItemTable.eColor.CL_NONE)
    {
        if (text == null)
        {
            return;
        }

        text.text = str;
        if (ItemTable.eColor.CL_NONE != color)
        {
            text.color = GameUtility.Item.GetItemColor(color);
        }
    }

    public static void SafeSetColor(this Text text, Color color)
    {
        if (text == null)
        {
            return;
        }
        text.color = color;
    }

    public static void SafeSetFontSize(this Text text, int fontSize)
    {
        if (text == null)
        {
            return;
        }
        text.fontSize = fontSize;
    }
    public static void SafeSetValueChangeListener(this Slider slider, UnityEngine.Events.UnityAction<float> callBack)
    {
        if (slider == null)
        {
            return;
        }
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(callBack);
    }
    public static void SafeSetValue(this Slider slider, float value)
    {
        if (slider == null)
        {
            return;
        }
        slider.value = value;
    }
    public static void SafeSetEnable(this ComButtonEnbale comButtonEnbale, bool bEnable)
    {
        if (comButtonEnbale == null)
        {
            return;
        }
        comButtonEnbale.SetEnable(bEnable);
    }
    public static void SafeSetImage(this Image img, string path, bool setNativeSize = false)
    {
        if (img == null || path == null || path == "")
        {
            return;
        }
        ETCImageLoader.LoadSprite(ref img, path);
        if (setNativeSize)
        {
            img.SetNativeSize();
        }
        return;
    }
    public static void SafeAddOnClickListener(this Button button, UnityEngine.Events.UnityAction callBack)
    {
        if (button == null || callBack == null)
        {
            return;
        }

        if (IsAnonymousFunction(callBack))
        {
            Debug.LogErrorFormat("unsafe!!!! 不要使用匿名lambda表达式，请随手改掉,谢谢!!! method = {0},target = {1}", callBack.Method, callBack.Target);
        }

        button.onClick.RemoveListener(callBack);
        button.onClick.AddListener(callBack);
    }
    public static void SafeSetOnClickListener(this Button button, UnityEngine.Events.UnityAction callBack)
    {
        if (button == null || callBack == null)
        {
            return;
        }
        ButtonCD buttonCD = button.GetComponent<ButtonCD>();
        if (buttonCD != null)
        {
            buttonCD.SetCallBack(callBack);
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(callBack);
    }

    public static void SafeSetGray(this Slider slider, bool bGray)
    {
        if (slider == null)
        {
            return;
        }
        UIGray uIGray = slider.gameObject.SafeAddComponent<UIGray>(false);
        if (uIGray == null)
        {
            return;
        }
        uIGray.enabled = false;
        uIGray.enabled = bGray;
        slider.interactable = !bGray;
        return;
    }
    public static void SafeSetGray(this Button button, bool bGray, bool bCanNotClick = true)
    {
        if (button == null)
        {
            return;
        }
        UIGray uIGray = button.gameObject.SafeAddComponent<UIGray>(false);
        if (uIGray == null)
        {
            return;
        }
        uIGray.enabled = false;
        uIGray.enabled = bGray;
        if (bGray && bCanNotClick)
        {
            button.interactable = false;
            button.image.raycastTarget = false;
        }
        else
        {
            button.interactable = true;
            button.image.raycastTarget = true;
        }
        return;
    }
    public static void SafeSetGray(this Toggle toggle, bool bGray, bool bCanNotClick = true)
    {
        if (toggle == null)
        {
            return;
        }
        UIGray uIGray = toggle.gameObject.SafeAddComponent<UIGray>(false);
        if (uIGray == null)
        {
            return;
        }
        uIGray.enabled = false;
        uIGray.enabled = bGray;
        if (bGray && bCanNotClick)
        {
            toggle.interactable = false;
            toggle.image.raycastTarget = false;
        }
        else
        {
            toggle.interactable = true;
            toggle.image.raycastTarget = true;
        }
        return;
    }
    public static void SafeRemoveOnClickListener(this Button button, UnityEngine.Events.UnityAction callBack)
    {
        if (button == null || callBack == null)
        {
            return;
        }

        button.onClick.RemoveListener(callBack);
    }

    public static void SafeRemoveAllListener(this Button button)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
    }

    public static void SafeAddOnValueChangedListener(this Toggle toggle, UnityEngine.Events.UnityAction<bool> callBack)
    {
        if (toggle == null || callBack == null)
        {
            return;
        }

        toggle.onValueChanged.RemoveListener(callBack);
        toggle.onValueChanged.AddListener(callBack);
    }

    public static void SafeSetOnValueChangedListener(this Toggle toggle, UnityEngine.Events.UnityAction<bool> callBack)
    {
        if (toggle == null || callBack == null)
        {
            return;
        }

        ComToggleEx comToggleEx = toggle.GetComponent<ComToggleEx>();
        if (comToggleEx != null)
        {
            callBack += comToggleEx._UpdateCheckMask;
        }

        ToggleCD toggleCD = toggle.GetComponent<ToggleCD>();
        if (toggleCD != null)
        {
            toggleCD.SetCallBack(callBack);
            return;
        }
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(callBack);
    }
    public static void SafeRemoveOnValueChangedListener(this Toggle toggle, UnityEngine.Events.UnityAction<bool> callBack)
    {
        if (toggle == null || callBack == null)
        {
            return;
        }
        toggle.onValueChanged.RemoveListener(callBack);
    }

    public static void RemoveAllCallback(this Toggle toggle)
    {
        if (toggle == null)
        {
            return;
        }
        toggle.onValueChanged.RemoveAllListeners();
    }

    public static void SafeSetToggleOnState(this Toggle toggle, bool isOn)
    {
        if (toggle == null)
        {
            return;
        }
        toggle.isOn = isOn;
        return;
    }

    private static TextGenerator mTextGenerator;
    private static TextGenerationSettings mTextGenerationSettings;

    public static List<string> GetTextContents(this Text text, string content)
    {
        List<string> strs = new List<string>();

        if (mTextGenerator == null)
        {
            mTextGenerator = new TextGenerator();
        }

        if (text == null)
        {
            return strs;
        }

        float preferredHeight = GetTextPreferredHeight(text, content);
        text.transform.rectTransform().sizeDelta = new Vector2(text.rectTransform().rect.width, preferredHeight);

        mTextGenerationSettings.textAnchor = text.alignment;
        mTextGenerationSettings.pivot = text.rectTransform.pivot;
        mTextGenerationSettings.richText = text.supportRichText;
        mTextGenerationSettings.font = text.font;
        mTextGenerationSettings.fontSize = text.fontSize;
        mTextGenerationSettings.fontStyle = text.fontStyle;
        mTextGenerationSettings.verticalOverflow = text.verticalOverflow;
        mTextGenerationSettings.horizontalOverflow = text.horizontalOverflow;
        mTextGenerationSettings.generationExtents = new Vector2(text.rectTransform.rect.width, text.rectTransform.rect.height);
        mTextGenerationSettings.lineSpacing = text.lineSpacing;
        mTextGenerationSettings.alignByGeometry = text.alignByGeometry;
        mTextGenerationSettings.resizeTextForBestFit = text.resizeTextForBestFit;
        mTextGenerationSettings.scaleFactor = 1.0f;
        mTextGenerationSettings.updateBounds = false;

        mTextGenerator.Populate(content, mTextGenerationSettings);

        var lines = mTextGenerator.GetLinesArray();
        for (int i = 0; i < lines.Length; i++)
        {
            UILineInfo uILineInfo = lines[i];

            if (i + 1 >= lines.Length)
            {
                if (mTextGenerator.characterCount > 0)
                {
                    strs.Add(content.Substring(uILineInfo.startCharIdx, mTextGenerator.characterCount - 1 - uILineInfo.startCharIdx));
                }
                break;
            }

            strs.Add(content.Substring(uILineInfo.startCharIdx, lines[i + 1].startCharIdx - uILineInfo.startCharIdx));
        }

        mTextGenerator = null;

        return strs;
    }

    public static Vector2 CalEventTextSize(this Text text, string content)
    {
        if (mTextGenerator == null)
        {
            mTextGenerator = new TextGenerator();
        }

        mTextGenerationSettings.textAnchor = text.alignment;
        mTextGenerationSettings.pivot = text.rectTransform.pivot;
        mTextGenerationSettings.richText = text.supportRichText;
        mTextGenerationSettings.font = text.font;
        mTextGenerationSettings.fontSize = text.fontSize;
        mTextGenerationSettings.fontStyle = text.fontStyle;
        mTextGenerationSettings.verticalOverflow = text.verticalOverflow;
        mTextGenerationSettings.horizontalOverflow = text.horizontalOverflow;
        mTextGenerationSettings.generationExtents = new Vector2(text.rectTransform.rect.width, text.rectTransform.rect.height);
        mTextGenerationSettings.lineSpacing = text.lineSpacing;
        mTextGenerationSettings.alignByGeometry = text.alignByGeometry;
        mTextGenerationSettings.resizeTextForBestFit = text.resizeTextForBestFit;
        mTextGenerationSettings.scaleFactor = 1.0f;
        mTextGenerationSettings.updateBounds = false;

        Vector2 size = Vector2.one;
        size.x = mTextGenerator.GetPreferredWidth(content, mTextGenerationSettings);
        size.y = mTextGenerator.GetPreferredHeight(content, mTextGenerationSettings);

        return size;
    }

    public static float GetTextPreferredHeight(this Text text, string content)
    {
        if (mTextGenerator == null)
        {
            mTextGenerator = new TextGenerator();
        }

        mTextGenerationSettings.textAnchor = text.alignment;
        mTextGenerationSettings.pivot = text.rectTransform.pivot;
        mTextGenerationSettings.richText = text.supportRichText;
        mTextGenerationSettings.font = text.font;
        mTextGenerationSettings.fontSize = text.fontSize;
        mTextGenerationSettings.fontStyle = text.fontStyle;
        mTextGenerationSettings.verticalOverflow = text.verticalOverflow;
        mTextGenerationSettings.horizontalOverflow = text.horizontalOverflow;
        mTextGenerationSettings.generationExtents = new Vector2(text.rectTransform.rect.width, text.rectTransform.rect.height);
        mTextGenerationSettings.lineSpacing = text.lineSpacing;
        mTextGenerationSettings.alignByGeometry = text.alignByGeometry;
        mTextGenerationSettings.resizeTextForBestFit = text.resizeTextForBestFit;
        mTextGenerationSettings.scaleFactor = 1.0f;
        mTextGenerationSettings.updateBounds = false;

        return mTextGenerator.GetPreferredHeight(content, mTextGenerationSettings);
    }

    public static float GetTextPreferredWidth(this Text text, string content)
    {
        if (mTextGenerator == null)
        {
            mTextGenerator = new TextGenerator();
        }

        mTextGenerationSettings.textAnchor = text.alignment;
        mTextGenerationSettings.pivot = text.rectTransform.pivot;
        mTextGenerationSettings.richText = text.supportRichText;
        mTextGenerationSettings.font = text.font;
        mTextGenerationSettings.fontSize = text.fontSize;
        mTextGenerationSettings.fontStyle = text.fontStyle;
        mTextGenerationSettings.verticalOverflow = text.verticalOverflow;
        mTextGenerationSettings.horizontalOverflow = text.horizontalOverflow;
        mTextGenerationSettings.generationExtents = new Vector2(text.rectTransform.rect.width, text.rectTransform.rect.height);
        mTextGenerationSettings.lineSpacing = text.lineSpacing;
        mTextGenerationSettings.alignByGeometry = text.alignByGeometry;
        mTextGenerationSettings.resizeTextForBestFit = text.resizeTextForBestFit;
        mTextGenerationSettings.scaleFactor = 1.0f;
        mTextGenerationSettings.updateBounds = false;

        return mTextGenerator.GetPreferredWidth(content, mTextGenerationSettings);
    }

    /// <summary>
    /// 如果rt的其中一个顶点包含在这个rectTransform中，则返回true
    /// </summary>
    /// <param name="container"></param>
    /// <param name="rt"></param>
    /// <returns></returns>
    public static bool ContainsRect(this RectTransform container, RectTransform rt)
    {
        if (container == null || rt == null) return false;
        // 获取容器的四个顶点
        Vector3[] containerCorners = new Vector3[4];   
        container.GetWorldCorners(containerCorners);
        // 获取容器宽高
        float width = Mathf.Abs(containerCorners[2].x - containerCorners[0].x);
        float height = Mathf.Abs(containerCorners[2].y - containerCorners[0].y);
        Rect rect = new Rect(containerCorners[0].x, containerCorners[0].y, width, height);

        // 获取要判断UI的四个顶点
        Vector3[] rtCorners = new Vector3[4];
        rt.GetWorldCorners(rtCorners);
        // 依次判断四个顶点是否都在矩形中
        foreach (var corner in rtCorners)
        {
            if (rect.Contains(corner))
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 获取数字位数
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static int Digits(this int n)
    {
        if (n >= 0)
        {
            if (n < 10) return 1;
            if (n < 100) return 2;
            if (n < 1000) return 3;
            if (n < 10000) return 4;
            if (n < 100000) return 5;
            if (n < 1000000) return 6;
            if (n < 10000000) return 7;
            if (n < 100000000) return 8;
            if (n < 1000000000) return 9;
            return 10;
        }
        if (n > -10) return 2;
        if (n > -100) return 3;
        if (n > -1000) return 4;
        if (n > -10000) return 5;
        if (n > -100000) return 6;
        if (n > -1000000) return 7;
        if (n > -10000000) return 8;
        if (n > -100000000) return 9;
        if (n > -1000000000) return 10;
        return 11;
    }
}

public partial class Utility
{
    private static ulong[] _DW = new ulong[] { 0x2540be400L, 0x5f5e100L, 0xf4240L, 0x2710L, 100L };
    private static readonly int CHINESE_CHAR_END = Convert.ToInt32("9fff", 0x10);
    private static readonly int CHINESE_CHAR_START = Convert.ToInt32("4e00", 0x10);
    private static readonly int MAX_CHINESE_NAME_LEN = 12;
    private static readonly int MAX_EN_NAME_LEN = 9;
    private static readonly int MIN_CHINESE_NAME_LEN = 4;
    private static readonly int MIN_EN_NAME_LEN = 3;
    public static uint s_daySecond = 0x15180;
    public const long UTC_OFFSET_LOCAL = 0x7080L;
    public const long UTCTICK_PER_SECONDS = 0x989680L;

    private const float PRECISION = 0.000001f;

    public const string kRawDataPrefix = "RawData";
    public const string kRawDataExtension = ".bytes";

    static private readonly int[] layerTbl = new int[] { 12, 15, 16, 17 };

    private static CultureInfo mCultureInfo = new CultureInfo("zh-CN");

    public static CultureInfo cultureInfo
    {
        get
        {
            return mCultureInfo;
        }
    }

    public enum SoundKind
    {
        SK_ACCEPT_TASK = 0,
        SK_COMPLETE_TASK,
        SK_ABANDON_TASK,
        SK_ACQUIRE_AWARD,
        SK_OPEN_FRAME,
        SK_CLOSE_FRAME,
    }

    public static float I2FloatBy1000(int value)
    {
        return value / (float)GlobalLogic.VALUE_1000; 
    }

    public static float I2FloatBy10000(int value)
    {
        return value / (float)GlobalLogic.VALUE_10000;
    }

    public static float I2Float(int value)
    {
        return (float)value;
    }

    /// <summary>
    /// 道具属性的百分比到等级的转换 
    ///
    /// 命中等级
    /// 闪避等级
    /// 物暴等级
    /// 魔暴等级
    /// 城镇移速等级
    /// 攻速等级
    /// 吟唱等级
    /// 移速等级
    ///
    /// </summary>
    public static float ConvertItemDataRateValue2IntLevel(float rate)
    {
        return 5 * rate;
    }


    public static Vec3 IRepeate2Vector(FlatBufferArray<int> value)
    {
        return new Vec3(I2FloatBy1000(value[0]), I2FloatBy1000(value[1]), 0);
    }

    public static Vec3 IRepeate2Vector(global::System.Collections.Generic.List<CrypticInt32> value)
    {
        return new Vec3(I2FloatBy1000(value[0]),I2FloatBy1000(value[1]),0);
    }

    public static Vec3 IRepeate2Vector(global::System.Collections.Generic.List<int> value)
    {
        return new Vec3(I2FloatBy1000(value[0]),I2FloatBy1000(value[1]),0);
    }


    public static string GetIPAddress()
    {
        try
        {
            string hostName = System.Net.Dns.GetHostName();

            System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(hostName);

            if (null != entry)
            {
                System.Net.IPAddress[] addresses = entry.AddressList;

                for (int i = 0; i < addresses.Length; ++i)
                {
                    if (addresses[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return addresses[i].ToString();
                    }
                }
            }
        }
        catch
        {

        }

        return string.Empty;
    }

    public static string GetSoundPath(SoundKind eSoundKind)
    {
        switch (eSoundKind)
        {
            case SoundKind.SK_COMPLETE_TASK:
                return "Sound/SE/qcomplete1";
            case SoundKind.SK_ABANDON_TASK:
                return "Sound/SE/qabandon";
            case SoundKind.SK_ACCEPT_TASK:
                return "Sound/SE/quest_accept";
            case SoundKind.SK_ACQUIRE_AWARD:
                return "Sound/SE/qcomplete2";
            case SoundKind.SK_OPEN_FRAME:
                return "Sound/SE/winshow";
            case SoundKind.SK_CLOSE_FRAME:
                return "Sound/SE/winclose";
        }
        return null;
    }

    public static void CreateRoleActor(GeAvatarRendererEx actor, int iModeId)
    {
        if (actor == null)
        {
            Logger.LogErrorFormat("actor is null!");
            return;
        }

        ProtoTable.ResTable res = TableManager.instance.GetTableItem<ProtoTable.ResTable>(iModeId);
        if (res == null)
        {
            Logger.LogErrorFormat("角色模型无法找到 ProtoTable.ResTable ID = [{0}]", iModeId);
            return;
        }

        actor.LoadAvatar(res.ModelPath);
        actor.ChangeAction("Anim_Idle01");
    }

    public static void CreateUnitActor(GeAvatarRendererEx actor, Int32 iUnitID, int soltID, int iWidth = 619, int iHeight = 817, bool needAureole = false)
    {
        if (actor == null)
        {
            return;
        }

        ProtoTable.UnitTable unitItem = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(iUnitID);
        if (unitItem == null)
        {
            return;
        }

        ProtoTable.ResTable res = TableManager.instance.GetTableItem<ProtoTable.ResTable>(unitItem.Mode);
        if (res == null)
        {
            Logger.LogErrorFormat("角色模型无法找到 ProtoTable.ResTable ID = [{0}]", unitItem.Mode);
            return;
        }

        //actor.Init("SelectRole" + soltID.ToString(), iWidth, iHeight, layerTbl[soltID]);
        actor.LoadAvatar(res.ModelPath, layerTbl[soltID]);
        //actor.InitAvatar(res.ModelPath, "SelectRole" + soltID.ToString(), iWidth, iHeight, layerTbl[soltID]);
        actor.ChangeAction("Anim_Idle01", 1.0f, true);
        //actor.SetCameraOrthoSize(1.2f);
        //actor.SetViewDirection(new Vector3(15,0,0));
        //actor.SetCameraPos(new Vector3(0,1, -0.5f));

        if (needAureole)
        {
            actor.AttachAvatar(
            "Aureole",
            "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS",
            "[actor]Orign");
        }
        //设置光照
        //actor.SetLightDir(global::Global.Settings.avatarLightDir);

    }

    public static void CreateActor(GeAvatarRendererEx actor, Int32 iJobID, int soltID, int iWidth = 619, int iHeight = 817, bool needAureole = false)
    {
        ProtoTable.JobTable jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(iJobID);
        if (jobData == null)
        {
            Logger.LogError("职业ID找不到 ID = [" + iJobID + "]\n");
            return;
        }

        if (actor == null)
        {
            Logger.LogError("actor == null ?");
            return;
        }

        ProtoTable.ResTable res = TableManager.instance.GetTableItem<ProtoTable.ResTable>(jobData.Mode);
        if (res == null)
        {
            Logger.LogErrorFormat("角色模型无法找到 ProtoTable.ResTable ID = [{0}]", jobData.Mode);
            return;
        }

        //actor.Deinit();
        //actor.InitAvatar(res.ModelPath, "SelectRole" + soltID.ToString(), iWidth, iHeight, layerTbl[soltID]);
        //actor.Init("SelectRole" + soltID.ToString(), iWidth, iHeight, layerTbl[soltID]);

        //现在客户端是没有动态回收layer的机制，像这样在某几个系统里动态修改layer导致预制体里预设的layer失效,目前这样做是不合适的，要这样改的话那也要全部统一改才行
        //actor.LoadAvatar(res.ModelPath, layerTbl[soltID]);
        actor.LoadAvatar(res.ModelPath);

        actor.ChangeAction("Anim_Show_Idle", 1.0f, true);
        //actor.SetCameraOrthoSize(1.2f);
        //actor.SetViewDirection(new Vector3(15, 0, 0));
        //actor.SetCameraPos(new Vector3(0, 1, -0.5f));

        if (needAureole)
        {
            actor.AttachAvatar(
                "Aureole",
                "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS",
                "[actor]Orign");
        }

        /*//挂载武器
        if (res.ModelPath.Contains("Swordsman"))
        {
            actor.AttachAvatar(
            "RWeapon",
            "Actor/Hero_Swordsman/Weapon/Prefabs/p_Sword_02_skin",
            "[actor]RWeapon");
        }
*/
        //设置光照
        //actor.SetLightDir(global::Global.Settings.avatarLightDir);
    }

    public static void LoadDemoActor(GeDemoActor actor, Int32 iJobID, bool isAsync = false)
    {
        ProtoTable.JobTable jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(iJobID);
        if (jobData == null)
        {
            Logger.LogError("职业ID找不到 ID = [" + iJobID + "]\n");
            return;
        }

        if (actor == null)
        {
            Logger.LogError("actor == null ?");
            return;
        }

        ProtoTable.ResTable res = TableManager.instance.GetTableItem<ProtoTable.ResTable>(jobData.Mode);
        if (res == null)
        {
            Logger.LogErrorFormat("角色模型无法找到 ProtoTable.ResTable ID = [{0}]", jobData.Mode);
            return;
        }

        actor.LoadAvatar(res.ModelPath, isAsync);
    }


    public static bool IsStringValid(string str)
    {
        return str != null && str.Length > 0 && str != "-";
    }

    public static string GetPathLastName(string fullPath)
    {
        var tmp = fullPath.Split('/');
        string folderName = tmp[tmp.Length - 1];

        return folderName;
    }

    public static bool IsDateFullDay(UInt32 endtime, UInt32 starttime)
    {
        var dt = endtime - starttime;
        return dt >= (60 * 60 * 24 - 1);
    }

    public static void PrintType(Type type, object obj)
    {
        string printOut = string.Format("Print {0}\n", type.Name);
        var feildInfos = type.GetFields();
        foreach (var fi in feildInfos)
        {
            printOut += string.Format("{0}:{1}\n", fi.Name, fi.GetValue(obj));
        }
        printOut += "print done";

        Logger.LogError(printOut);
    }

	public static string GetTypeInfoString(Type type, object obj)
	{
		string printOut = string.Format("Print {0}\n", type.Name);
		var feildInfos = type.GetFields();
		foreach (var fi in feildInfos)
		{
			printOut += string.Format("{0}:{1}\n", fi.Name, fi.GetValue(obj));
		}
		printOut += "print done";

		return printOut;
	}

    /*
     * 递归复制组件内容，给编辑器用，不要在游戏里用 by ssj
     */
    public static void CopyRecursion<T>(GameObject src, GameObject target) where T : Component
    {
#if UNITY_EDITOR && !LOGIC_SERVER
        if (null == src || null == target || src == target)
        {
            return;
        }

        var leftParent = src.transform.parent;
        var rightParent = target.transform.parent;

        var left = src.GetComponent<T>();
        var right = target.GetComponent<T>();
        if (null != left)
        {
            if (null == right)
            {
                right = target.AddComponent<T>();
            }

            if (UnityEditorInternal.ComponentUtility.CopyComponent(left))
            {
                UnityEditorInternal.ComponentUtility.PasteComponentValues(right);
            }
        }

        for (int i = 0; i < src.transform.childCount; ++i)
        {
            var current = src.transform.GetChild(i);
            if (null == current)
            {
                continue;
            }

            var find = target.transform.Find(current.name);
            if (null == find)
            {
                continue;
            }

            CopyRecursion<T>(current.gameObject, find.gameObject);
        }
#endif
    }

    public static T CopyComponent<T>(T original, GameObject destination, bool bNeedRepeat = false) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!bNeedRepeat)
        {
            if (!dst)
                dst = destination.AddComponent(type) as T;
        }
        else
        {
            dst = destination.AddComponent(type) as T;
        }

        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }

    public static void OnPopupTaskChangedMsg(string kMsg, Int32 iTaskID)
    {
        MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iTaskID);
        if (missionItem != null)
        {
            SystemNotifyManager.SysNotifyFloatingEffect(string.Format("{0}\t[{1}]!", kMsg, missionItem.TaskName));
        }
    }


    public static bool SetValue(Type type, object obj, string var, int value, bool add = false)
    {
        var fieldInfo = type.GetField(var);
        if (fieldInfo != null)
        {
            if (add)
                fieldInfo.SetValue(obj, value + (int)fieldInfo.GetValue(obj));
            else
            {
                //Logger.LogErrorFormat("type:{0}", fieldInfo.GetValue(obj).GetType());

                if (fieldInfo.GetValue(obj).GetType().FullName == "System.Boolean")
                {
                    fieldInfo.SetValue(obj, value > 0 ? true : false);
                }
                else
                {
                    fieldInfo.SetValue(obj, value);
                }
            }

            return true;
        }

        return false;
    }




    public static bool SetValue2(Type type, object obj, string var, float value, bool add = false)
    {
        var fieldInfo = type.GetField(var);
        if (fieldInfo != null)
        {
            if (add)
                fieldInfo.SetValue(obj, value + (float)fieldInfo.GetValue(obj));
            else
            {
                //Logger.LogErrorFormat("type:{0}", fieldInfo.GetValue(obj).GetType());

                if (fieldInfo.GetValue(obj).GetType().FullName == "System.Boolean")
                {
                    fieldInfo.SetValue(obj, value > 0 ? true : false);
                }
                else
                {
                    fieldInfo.SetValue(obj, value);
                }
            }

            return true;
        }

        return false;
    }

    public static bool SetValueForProperty(Type type, object obj, string var, int value, bool add = false)
    {
        var proInfo = type.GetProperty(var);
        if (proInfo != null)
        {
            int curValue = (int)proInfo.GetGetMethod().Invoke(obj, null);

            if (add)
                proInfo.GetSetMethod().Invoke(obj, new object[] { value + curValue });
            else
                proInfo.GetSetMethod().Invoke(obj, new object[] { value });

            return true;
        }

        return false;
    }

    public static int GetValue(Type type, object obj, string var, bool isField = true)
    {
        int ret = 0;
        try
        {
            

            if (isField)
            {
                var fieldInfo = type.GetField(var);
                if (fieldInfo != null)
                {
                    var retObj = fieldInfo.GetValue(obj);

                    if (retObj.GetType() == typeof(CrypticInt32))
                        ret = (int)(CrypticInt32)retObj;
                    else
                        ret = (int)retObj;
                }
            }
            else
            {
                var proInfo = type.GetProperty(var);
                if (proInfo != null)
                {
                    ret = (int)proInfo.GetGetMethod().Invoke(obj, null);
                }
            }
        }catch(Exception e)
        {
            Logger.LogErrorFormat("get value error:{0} {1}", type.Name, e.ToString());
        }
        

        return ret;
    }



    public static bool IsFloatZero(float f)
    {
        return Mathf.Abs(f) <= PRECISION;
    }

    public static List<int> toList(global::System.Collections.Generic.List<CrypticInt32> list)
    {
        var item = new List<int>();

        if (list != null)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                item.Add(list[i]);
            }
        }

        return item;
    }

    /// <summary>
    /// 仅仅为加密结构做兼容
    /// </summary>
    public static List<int> toList(global::System.Collections.Generic.List<int> list)
    {
        return list;
    }

    /*
    指定名字或路径
    */
    public static GameObject FindChild(string name, GameObject parent)
    {
        if (parent == null)
        {
            return null;
        }
        Transform child = parent.transform.Find(name);
        if (child == null)
            return null;
        return child.gameObject;
    }

    /*
    递归寻找child,只要给定名字
    */
    public static GameObject FindThatChild(string name, GameObject parent, bool includeInactive = false)
    {
        if (parent == null)
            return null;

        Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);
        foreach (Transform child in children)
        {
            if (child.name == name)
                return child.gameObject;
        }
        return null;
    }

    public static GameObject FindGameObject(string path, bool bMustExist = true)
    {
        GameObject obj = GameObject.Find(path);

        if (obj == null && bMustExist)
        {
            Logger.LogError("!!!FindGameObject Error: " + path + "\n");
#if UNITY_EDITOR && !LOGIC_SERVER
            //Debug.Break();
#endif
        }

        return obj;
    }

    public static GameObject FindGameObject(GameObject root, string path, bool bMustExist = true)
    {
        GameObject obj = null;
        if (root != null && root.transform != null)
        {
            Transform t = root.transform.Find(path);

            if (t != null)
            {
                obj = root.transform.Find(path).gameObject;
            }

            if (obj == null && bMustExist)
            {
                //Logger.LogError("!!!FindGameObject Error: " + path + "\n");
#if UNITY_EDITOR && !LOGIC_SERVER
                //Debug.Break();
#endif
            }
        }
        return obj;
    }

    public static Int32 GetItemCount(ulong id)
    {
        Int32 iCount = 0;

        var itemData = GameClient.ItemDataManager.GetInstance().GetItem(id);
        if (itemData != null)
        {
            return itemData.Count;
        }

        return iCount;
    }

    //转之后的职业，即小职业
    private static List<ProtoTable.JobTable> betterJobIds = new List<ProtoTable.JobTable>();
    public static List<ProtoTable.JobTable> BettleJobIds
    {
        get
        {
            if (betterJobIds.Count == 0)
            {
                var jobTables = TableManager.GetInstance().GetTable<ProtoTable.JobTable>();
                if (jobTables != null)
                {
                    var iter = jobTables.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        var current = iter.Current.Value as ProtoTable.JobTable;
                        //专职后的职业
                        if (current != null && current.Open == 1 && current.JobType == 1)
                            betterJobIds.Add(current);
                    }
                }
            }

            return betterJobIds;
        }
    }

    static List<ProtoTable.JobTable> ms_akAllJobsID = new List<ProtoTable.JobTable>();
    public static List<ProtoTable.JobTable> OrgJobTables
    {
        get
        {
            if (ms_akAllJobsID.Count == 0)
            {
                var jobTable = TableManager.GetInstance().GetTable<ProtoTable.JobTable>();
                if (jobTable != null)
                {
                    var iterator = jobTable.GetEnumerator();
                    while (iterator.MoveNext())
                    {
                        var current = iterator.Current.Value as ProtoTable.JobTable;
                        if (current != null && current.JobType == 0 && current.Open == 1)
                        {
                            ms_akAllJobsID.Add(current);
                        }
                    }
                }
            }
            return ms_akAllJobsID;
        }
    }

    public static Vector3 GetScreen2Position(Camera camera, Vector3 postioin)
    {
        if (camera != null)
        {
            Vector3 kScreenPos = camera.WorldToScreenPoint(postioin);
            return kScreenPos;
        }
        return default(Vector3);
    }

    public static T FindComponent<T>(GameObject root, string path, bool bMustExist = true) where T : Component
    {
        GameObject obj = FindGameObject(root, path, bMustExist);

        if (obj == null)
        {
            return null;
        }

        T com = obj.GetComponent<T>();

        if (com == null)
        {
            Logger.LogError("!!!FindComponent Error: " + path + ".." + typeof(T).ToString() + "\n");
#if UNITY_EDITOR && !LOGIC_SERVER
            //Debug.Break();
#endif
        }

        return com;
    }

    public static Component FindComponent(GameObject root, Type type, string path, bool bMustExist = true)
    {
        GameObject obj = FindGameObject(root, path, bMustExist);

        if (obj == null)
        {
            return null;
        }

        Component com = obj.GetComponent(type);

        if (com == null)
        {
            Logger.LogError("!!!FindComponent Error: " + path + ".." + type.ToString() + "\n");
#if UNITY_EDITOR && !LOGIC_SERVER
            //Debug.Break();
#endif
        }

        return com;
    }

    public static T FindComponent<T>(string path, bool bMustExist = true) where T : Component
    {
        GameObject obj = FindGameObject(path, bMustExist);

        if (obj == null)
        {
            return null;
        }

        T com = obj.GetComponent<T>();

        if (com == null)
        {
            Logger.LogError("!!!FindComponent Error: " + path + ".." + typeof(T).ToString() + "\n");
#if UNITY_EDITOR && !LOGIC_SERVER
            //Debug.Break();
#endif
        }

        return com;
    }

    public static void AttachTo(GameObject go, GameObject parent, bool keepPos = false)
    {
        if (parent == null)
        {
            Logger.LogWarning("AttachTo parent is nil");
            return;
        }

        if (go == null)
        {
            Logger.LogWarning("AttachTo go is nil");
            return;
        }

        var goRect = go.GetComponent<RectTransform>();
        var goTransform = go.transform;

        var offsetMin = Vector2.zero;
        var offsetMax = Vector2.zero;
        var anchorMax = Vector2.zero;
        var anchorMin = Vector2.zero;

        var lscale = goTransform.transform.localScale;
        var lpos = goTransform.transform.localPosition;
        var lrotation = goTransform.transform.localRotation;

        if (goRect != null)
        {
            offsetMin = goRect.offsetMin;
            offsetMax = goRect.offsetMax;
            anchorMin = goRect.anchorMin;
            anchorMax = goRect.anchorMax;
        }

        go.transform.SetParent(parent.transform, true);

        goTransform.localScale = lscale;
        goTransform.localRotation = lrotation;
        goTransform.localPosition = lpos;

        if (goRect != null)
        {
            goRect.offsetMin = offsetMin;
            goRect.offsetMax = offsetMax;
            goRect.anchorMin = anchorMin;
            goRect.anchorMax = anchorMax;
        }
    }

    public static byte[] BytesConvert(string s)
    {
        return Encoding.UTF8.GetBytes(s.TrimEnd(new char[1]));
    }

    public static Color GetMissionTypeColor(ProtoTable.MissionTable.eTaskType eTaskType)
    {
        Color[] colorArray ={
            new Color(0xfe, 0xc5, 0x00, 0xff),
            new Color(0x20, 0xff, 0x4f, 0xff),
            new Color(0xff, 0xff, 0xff, 0xff),
            new Color(0xff, 0x78, 0x00, 0xff),
            new Color(0xff, 0x78, 0x00, 0xff),
            new Color(0xff, 0x78, 0x00, 0xff),
            new Color(0xff, 0x78, 0x00, 0xff),
            new Color(0xff, 0x78, 0x00, 0xff),
        };
        if (eTaskType >= 0 && (int)eTaskType < colorArray.Length)
        {
            return colorArray[(int)eTaskType];
        }
        return colorArray[0];
    }

    public static void SetMissionTypeIcon(GameObject goTypeIcon, ProtoTable.MissionTable.eTaskType eTaskType, bool bNeedSetNativeSize = false)
    {
        var imgTypeIcon = goTypeIcon.transform.GetComponent<UnityEngine.UI.Image>();
        if (imgTypeIcon == null)
        {
            return;
        }

        string[] spriteName =
        {
            "UIPacked/p-Mission.png:Taskbook_typeOutside",//TT_DIALY
			"UIPacked/p-Mission.png:Taskbook_iconMain.png",//Main
			"UIPacked/p-Mission.png:Taskbook_typeSystem",//TT_SYSTEM
			"UIPacked/p-Mission.png:Taskbook_iconAchievement.png",//TT_ACHIEVEMENT
			"UIPacked/p-Mission.png:Taskbook_iconBranch.png",//TT_BRANCH
			"UIPacked/p-Mission.png:Taskbook_typeActivity",//TT_ACTIVITY
			"UIPacked/p-Mission.png:Taskbook_typeOutside",//TT_EXTENTION
			"UIPacked/p-Mission.png:Taskbook_changeJob",//TT_CHANGEJOB
        };

        if (eTaskType >= 0 && (int)eTaskType < spriteName.Length)
        {
            // imgTypeIcon.sprite = createSprite(spriteName[(int)eTaskType]);
            createSprite(spriteName[(int)eTaskType], ref imgTypeIcon);

            if (bNeedSetNativeSize)
            {
                imgTypeIcon.SetNativeSize();
            }
        }
    }

    //public static void SetNpcFunctionIcon(GameObject goTypeIcon, ProtoTable.NpcTable.eFunction eFunction)
    //{
    //    var imgTypeIcon = goTypeIcon.transform.GetComponent<UnityEngine.UI.Image>();
    //    if (imgTypeIcon == null)
    //    {
    //        return;
    //    }
    //
    //    string[] spriteName =
    //    {
    //		"UI/Image/Mission/NpcFunction/make",//生产
    //		"UI/Image/Mission/NpcFunction/strength",//强化
    //		"UI/Image/Mission/NpcFunction/deal",//交易
    //		"UI/Image/Mission/NpcFunction/decompose",//分解
    //    };
    //
    //    if ((Int32)eFunction >= 0 && (Int32)eFunction < spriteName.Length)
    //    {
    //        imgTypeIcon.sprite = AssetLoader.instance.LoadRes(spriteName[(Int32)eFunction],typeof(Sprite)).obj as Sprite;
    //        imgTypeIcon.SetNativeSize();
    //    }
    //}

    public static string GetNpcFunctionName(Int32 iNpcID)
    {
        string ret = "未知的职业";

        ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
        if (npcItem != null)
        {
            ret = npcItem.Function.ToString();
        }

        return ret;
    }

    //public static string GetNpcFunctionIcon(Int32 iNpcID)
    //{
    //    string ret = "";
    //
    //    ProtoTable.NpcTable npcItem = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(iNpcID);
    //    if (npcItem != null)
    //    {
    //        ret = npcItem.NpcTitle;
    //    }
    //
    //    return ret;
    //}

    enum MissionKeyType
    {
        MKT_KEY,//只是替换KEY
        MKT_KEY_VALUE,//替换KEY/VALUE 并判断KEY与VALUE大小，决定文字色
        MKT_KEY_KEY,//替换KEY/VALUE 并判断KEY与VALUE大小，决定文字色
        MKT_COUNT,
    }
    static Regex[] ms_missionkey_regex = new Regex[(int)MissionKeyType.MKT_COUNT]
        {
            new Regex(@"<key>key=(\w+)</key>"),
            new Regex(@"<key>key=(\w+)/value=(\d+)</key>"),
            new Regex(@"<key>key=(\w+)/key=(\w+)</key>"),
        };

    public class ContentProcess
    {
        public ContentProcess()
        {
            content = null;
            iPreValue = 0;
            iAftValue = 1;
            bFinish = false;
            fAmount = 0.0f;
        }
        public string content;
        public int iPreValue;
        public int iAftValue;
        public bool bFinish;
        public float fAmount;
    }

    static void _TryAddDesc(ref string text, string desc)
    {
        if (string.IsNullOrEmpty(desc) == false)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = desc;
            }
            else
            {
                text += "\r\n" + desc;
            }
        }
    }

    static void _TryAddDesc(ref string text, List<string> desc)
    {
        for (int i = 0; i < desc.Count; ++i)
        {
            _TryAddDesc(ref text, desc[i]);
        }
    }

    public class TipContent
    {
        public enum TipContentType
        {
            TCT_INVALID = -1,
            TCT_LEVEL_LIMIT,
            TCT_BASEATTR,
            TCT_STRENGTH_DESC,
            TCT_PHYSIC_ATTACK,
            TCT_FOUR_ATTR,
            TCT_SKILL_ATTR,
            TCT_ATTACH_ATTR,
            TCT_COM_ATTR,
            TCT_TIMESTAMP_ATTR,
            TCT_INTERESTING_DESC,
            TCT_SOURCE_DESC,
            TCT_BLANK_DESC,
            TCT_COUNT,
        }

        public bool IsNull
        {
            get
            {
                return string.IsNullOrEmpty(left) && string.IsNullOrEmpty(right);
            }
        }

        public int iParam0 = 0;
        public string left = "";
        public string right = "";
        string prefabs = "";
        public string Prefabpath
        {
            get { return prefabs; }
        }
        TipContentType eTipContentType;
        public TipContentType ETipContentType
        {
            get
            {
                return eTipContentType;
            }
            set
            {
                eTipContentType = value;
                if (eTipContentType > TipContentType.TCT_INVALID && eTipContentType < TipContentType.TCT_COUNT)
                {
                    prefabs = ms_prefabspaths[(int)eTipContentType];
                }
                else
                {
                    prefabs = "";
                }
            }
        }

        public static string[] ms_prefabspaths = new string[(int)TipContentType.TCT_COUNT]
        {
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefabLWFixed",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartPrefab",
            "UIFlatten/Prefabs/TitleBookFrame/SmartBlank",
        };
    }

    static void _AddBlank(ref List<TipContent> tipItems, int iHeight = 20)
    {
        if (tipItems != null)
        {
            TipContent kTipContent = new TipContent();
            kTipContent.iParam0 = iHeight;
            kTipContent.ETipContentType = TipContent.TipContentType.TCT_BLANK_DESC;
            tipItems.Add(kTipContent);
        }
    }

    public static string _GetDifferenceDesc(int a_value0, int a_value1)
    {
        if (a_value0 == a_value1)
        {
            return "";
        }
        else if (a_value0 > a_value1)
        {
            string desc = string.Format("(+{0})", a_value0 - a_value1);
            return TR.Value("color_green", desc);
        }
        else
        {
            string desc = string.Format("({0})", a_value0 - a_value1);
            return TR.Value("color_red", desc);
        }
    }

    public static List<string> _GetBaseMainPropDescs(EquipProp a_Prop, EquipProp a_compareProp)
    {
        List<string> descs = new List<string>();

        EEquipProp[] fourProps =
        {
                EEquipProp.PhysicsAttack,
                EEquipProp.MagicAttack,
                EEquipProp.PhysicsDefense,
                EEquipProp.MagicDefense
            };

        string temp;
        for (int i = 0; i < fourProps.Length; ++i)
        {
            temp = a_Prop.GetPropFormatStr(fourProps[i]);
            if (string.IsNullOrEmpty(temp) == false)
            {
                if (a_compareProp != null)
                {
                    int index = (int)fourProps[i];
                    temp += " ";
                    temp += _GetDifferenceDesc(a_Prop.props[index], a_compareProp.props[index]);
                }
                descs.Add(temp);
            }
        }

        return descs;
    }

    public static List<TipContent> GetTitleTipItemList(ItemData item)
    {
        List<TipContent> tipItems = new List<TipContent>();
        string m_stretch = "stretch";
        TipContent kContent = null;

#region base attr
        {
            kContent = new TipContent();
            kContent.ETipContentType = TipContent.TipContentType.TCT_BASEATTR;

            _TryAddDesc(ref kContent.left, item.GetLevelLimitDesc());
            _TryAddDesc(ref kContent.left, item.GetTimeLeftDesc());
            _TryAddDesc(ref kContent.left, item.GetOccupationLimitDesc());

            _TryAddDesc(ref kContent.right, item.GetQualityDesc());
            _TryAddDesc(ref kContent.right, item.GetBindStateDesc());

            if (!kContent.IsNull)
            {
                tipItems.Add(kContent);
                _AddBlank(ref tipItems);
            }
        }
#endregion



#region strengthen
        kContent = new TipContent();
        kContent.ETipContentType = TipContent.TipContentType.TCT_STRENGTH_DESC;
        _TryAddDesc(ref kContent.left, item.GetStrengthenDescs());
        if (!kContent.IsNull)
        {
            tipItems.Add(kContent);
            _AddBlank(ref tipItems);
        }
#endregion

#region physicalAttack
        kContent = new TipContent();
        kContent.ETipContentType = TipContent.TipContentType.TCT_PHYSIC_ATTACK;
        _TryAddDesc(ref kContent.left, _GetBaseMainPropDescs(item.BaseProp, null));
        if (!kContent.IsNull)
        {
            tipItems.Add(kContent);
            _AddBlank(ref tipItems);
        }
#endregion

#region four attr
        kContent = new TipContent();
        kContent.ETipContentType = TipContent.TipContentType.TCT_FOUR_ATTR;
        _TryAddDesc(ref kContent.left, item.GetFourAttrDescs());
        _TryAddDesc(ref kContent.left, item.GetBeadDescs());
        if (!kContent.IsNull)
        {
            tipItems.Add(kContent);
            _AddBlank(ref tipItems);
        }
#endregion

#region skill CD/MP attr
        //称号的Tips不显示武器的物理技能和魔法技能相关信息
        //kContent = new TipContent();
        //kContent.ETipContentType = TipContent.TipContentType.TCT_SKILL_ATTR;
        //_TryAddDesc(ref kContent.left, item.GetSkillMPAndCDDescs());
        //if (!kContent.IsNull)
        //{
        //    tipItems.Add(kContent);
        //    _AddBlank(ref tipItems);
        //}
#endregion

#region additional attr
        kContent = new TipContent();
        kContent.ETipContentType = TipContent.TipContentType.TCT_ATTACH_ATTR;
        _TryAddDesc(ref kContent.left, item.GetAttachAttrDescs());
        if (!kContent.IsNull)
        {
            tipItems.Add(kContent);
            _AddBlank(ref tipItems);
        }
#endregion

#region complex attr
        kContent = new TipContent();
        kContent.ETipContentType = TipContent.TipContentType.TCT_COM_ATTR;
        _TryAddDesc(ref kContent.left, item.GetComplexAttrDescs());
        if (!kContent.IsNull)
        {
            tipItems.Add(kContent);
            _AddBlank(ref tipItems);
        }
#endregion

#region dead timestamp description
        kContent = new TipContent();
        kContent.ETipContentType = TipContent.TipContentType.TCT_TIMESTAMP_ATTR;
        _TryAddDesc(ref kContent.left, item.GetDeadTimestampDesc());
        if (!kContent.IsNull)
        {
            tipItems.Add(kContent);
            _AddBlank(ref tipItems);
        }
#endregion

        //#region dead timestamp description
        //kContent = new TipContent();
        //kContent.ETipContentType = TipContent.TipContentType.TCT_TIMESTAMP_ATTR;
        //_TryAddDesc(ref kContent.left, item.GetDeadTimestampDesc());
        //if (!kContent.IsNull)
        //{
        //    tipItems.Add(kContent);
        //    _AddBlank(ref tipItems);
        //}
        //#endregion

#region Interesting description
        kContent = new TipContent();
        kContent.ETipContentType = TipContent.TipContentType.TCT_INTERESTING_DESC;
        _TryAddDesc(ref kContent.left, item.Description);
        if (!kContent.IsNull)
        {
            tipItems.Add(kContent);
        }
#endregion

#region source description
        //kContent = new TipContent();
        //kContent.ETipContentType = TipContent.TipContentType.TCT_SOURCE_DESC;
        //_TryAddDesc(ref kContent.left, item.SourceDescription);
        //tipItems.Add(kContent);
#endregion

        //去掉尾部的换行
        while (tipItems.Count > 0 && tipItems[tipItems.Count - 1].ETipContentType == TipContent.TipContentType.TCT_BLANK_DESC)
        {
            tipItems.RemoveAt(tipItems.Count - 1);
        }

        return tipItems;
    }

    public static ContentProcess ParseMissionProcess(Int32 taskId, bool bCondition = false)
    {
        string totalString = "";

        int iPreValue = 0;
        int iAftValue = 0;
        ContentProcess kContentProcess = new ContentProcess();

        var missionValue = MissionManager.GetInstance().GetMission((uint)taskId);
        if (missionValue != null)
        {
            string[] singleTaskInfo = null;

            string missionDesc = null;
            if ((int)Protocol.TaskStatus.TASK_INIT == missionValue.status)
            {
                missionDesc = missionValue.missionItem.TaskInitText;
            }
            else if ((int)Protocol.TaskStatus.TASK_UNFINISH == missionValue.status)
            {
                missionDesc = missionValue.missionItem.TaskAcceptedText;
            }
            else
            {
                missionDesc = missionValue.missionItem.TaskFinishText;
            }

            if (!bCondition)
            {
                singleTaskInfo = (missionValue.missionItem.TaskInformationText + "\n" + missionDesc).ToString().Split(new char[] { '\r', '\n' });
            }
            else
            {
                singleTaskInfo = missionDesc.ToString().Split(new char[] { '\r', '\n' });
            }

            StringBuilder kStringBuilder = StringBuilderCache.Acquire();
            for (int i = 0; i < singleTaskInfo.Length; ++i)
            {
                var indexText = 0;
                for (int j = 0; j < (int)MissionKeyType.MKT_COUNT; ++j)
                {
                    foreach (Match match in ms_missionkey_regex[j].Matches(singleTaskInfo[i]))
                    {
                        kStringBuilder.Append(singleTaskInfo[i].Substring(indexText, match.Index - indexText));

                        switch ((MissionKeyType)j)
                        {
                            case MissionKeyType.MKT_KEY:
                                {
                                    string value = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)taskId, match.Groups[1].Value);
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        kStringBuilder.Append(value);
                                    }
                                }
                                break;
                            case MissionKeyType.MKT_KEY_VALUE:
                                {
                                    Int32 iPre = 0;
                                    Int32 iAft = 0;
                                    string value = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)taskId, match.Groups[1].Value);
                                    try
                                    {
                                        iPre = Int32.Parse(value);
                                        iAft = Int32.Parse(match.Groups[2].Value);
                                    }
                                    catch (Exception e)
                                    {
                                        string Error = string.Format("[Task ParseMissionText]Task ID {0} TaskInfo Key [{1}] Parse Value [{2}] to Int Error \n {3}", taskId, match.Groups[1].Value, match.Groups[2].Value, singleTaskInfo[i]);
                                        Logger.LogWarning(Error);
                                        break;
                                    }

                                    if (iPre >= iAft)
                                    {
                                        kStringBuilder.Append("<color=grey>");
                                    }
                                    else
                                    {
                                        kStringBuilder.Append("<color=white>");
                                    }
                                    kStringBuilder.AppendFormat("{0}/{1}", iPre, iAft);
                                    kStringBuilder.Append("</Color>");

                                    iPreValue += iPre;
                                    iAftValue += iAft;
                                }
                                break;
                            case MissionKeyType.MKT_KEY_KEY:
                                Int32 iKey0 = 0;
                                Int32 iKey1 = 0;
                                string value0 = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)taskId, match.Groups[1].Value);
                                string value1 = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)taskId, match.Groups[2].Value);
                                try
                                {
                                    iKey0 = Int32.Parse(value0);
                                    iKey1 = Int32.Parse(value1);
                                }
                                catch (Exception e)
                                {
                                    string Error = string.Format("[Task ParseMissionText]Task ID {0} TaskInfo Key0 [{1}] Key1 [{2}] to Int Error \n {3}", taskId, match.Groups[1].Value, match.Groups[2].Value, singleTaskInfo[i]);
                                    Logger.LogWarning(Error);
                                    break;
                                }
                                if (iKey0 >= iKey1)
                                {
                                    kStringBuilder.Append("<color=grey>");
                                }
                                else
                                {
                                    kStringBuilder.Append("<color=white>");
                                }
                                kStringBuilder.AppendFormat("{0}/{1}", iKey0, iKey1);
                                kStringBuilder.Append("</Color>");
                                iPreValue += iKey0;
                                iAftValue += iKey1;
                                break;
                        }

                        indexText = match.Index + match.Length;
                    }
                }
                kStringBuilder.Append(singleTaskInfo[i].Substring(indexText, singleTaskInfo[i].Length - indexText));
                if (i != singleTaskInfo.Length - 1)
                {
                    kStringBuilder.Append("\r\n");
                }
            }
            totalString = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);
        }

        kContentProcess.iAftValue = iAftValue;
        kContentProcess.iPreValue = iPreValue;
        kContentProcess.content = totalString;
        kContentProcess.fAmount = iAftValue <= 0 ? 1.0f : iPreValue * 1.0f / iAftValue;
        kContentProcess.bFinish = iAftValue <= iPreValue;
        return kContentProcess;
    }

    public class DailyProveTaskConfig
    {
        public string title;
        public int iPreValue;
        public int iAftValue;
        public float Amount
        {
            get
            {
                float fAmount = 0.0f;
                if (iAftValue != 0)
                {
                    fAmount = iPreValue / (float)iAftValue;
                    fAmount = Mathf.Clamp01(fAmount);
                }
                return fAmount;
            }
        }
    }

    public static DailyProveTaskConfig GetDailyProveTaskConfig(Int32 taskId)
    {
        var taskInfo = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(taskId);
        if (taskInfo != null && !string.IsNullOrEmpty(taskInfo.TaskFinishText))
        {
            var tokens = taskInfo.TaskFinishText.Split(new char[] { ':' });
            if (tokens.Length == 2)
            {
                var match = ms_missionkey_regex[(int)MissionKeyType.MKT_KEY_VALUE].Match(tokens[1]);
                if (match != null && !string.IsNullOrEmpty(match.Groups[0].Value))
                {
                    Int32 iPre = 0;
                    Int32 iAft = 0;
                    string value = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)taskId, match.Groups[1].Value);
                    try
                    {
                        iPre = Int32.Parse(value);
                        iAft = Int32.Parse(match.Groups[2].Value);
                        var proveConfig = new DailyProveTaskConfig
                        {
                            title = tokens[0],
                            iPreValue = iPre,
                            iAftValue = iAft,
                        };
                        return proveConfig;
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e.ToString());
                    }
                }
            }
        }
        return null;
    }
    //用于只知道任务数据，没有在loading时加入到taskgroup里面，获取任务进度文本
    public static string  ParseMissionTextForMissionInfo(MissionInfo missionInfo, bool bCondition = false, bool bMarkDefault = false, bool onlySchedule = false)
    {
        bool haveFinish = false;
        MissionManager.SingleMissionInfo kSingleMissionInfo = new MissionManager.SingleMissionInfo();
        kSingleMissionInfo.taskID = missionInfo.taskID;
        kSingleMissionInfo.status = missionInfo.status;
        kSingleMissionInfo.finTime = missionInfo.finTime;
        kSingleMissionInfo.submitCount = missionInfo.submitCount;
        kSingleMissionInfo.missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((int)missionInfo.taskID);
        string totalString = "";
        var missionValue = kSingleMissionInfo;
        if (missionValue != null)
        {
            string missionDesc = null;
            if ((int)Protocol.TaskStatus.TASK_INIT == missionValue.status)
            {
                missionDesc = missionValue.missionItem.TaskInitText;
            }
            else if ((int)Protocol.TaskStatus.TASK_UNFINISH == missionValue.status)
            {
                missionDesc = missionValue.missionItem.TaskAcceptedText;
            }
            else
            {
                missionDesc = missionValue.missionItem.TaskFinishText;
            }
            

            string[] singleTaskInfo = null;

            if (!bCondition)
            {
                singleTaskInfo = (missionValue.missionItem.TaskInformationText + "\n" + missionDesc).ToString().Split(new char[] { '\r', '\n' });
            }
            else
            {
                singleTaskInfo = missionDesc.ToString().Split(new char[] { '\r', '\n' });
            }

            StringBuilder kStringBuilder = StringBuilderCache.Acquire();
            string missionSchedule = "";
            for (int i = 0; i < singleTaskInfo.Length; ++i)
            {
                if (onlySchedule)
                {
                    if (i >= 1)
                    {
                        continue;
                    }
                }
                var indexText = 0;
                for (int j = 0; j < (int)MissionKeyType.MKT_COUNT; ++j)
                {
                    foreach (Match match in ms_missionkey_regex[j].Matches(singleTaskInfo[i]))
                    {
                        kStringBuilder.Append(singleTaskInfo[i].Substring(indexText, match.Index - indexText));

                        switch ((MissionKeyType)j)
                        {
                            case MissionKeyType.MKT_KEY:
                                {
                                    //string value = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)missionInfo.taskID, match.Groups[1].Value);
                                    string tempValue = "0";
                                    
                                    for (int h = 0;h<missionInfo.akMissionPairs.Length;h++)
                                    {
                                        if(missionInfo.akMissionPairs[h].key == match.Groups[1].Value)
                                        {
                                            tempValue = missionInfo.akMissionPairs[h].value;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(tempValue))
                                    {
                                        kStringBuilder.Append(tempValue);
                                    }
                                }
                                break;
                            case MissionKeyType.MKT_KEY_VALUE:
                                {
                                    Int32 iPre = 0;
                                    Int32 iAft = 0;
                                    //string value = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)missionInfo.taskID, match.Groups[1].Value);
                                    string tempValue = "0";

                                    for (int h = 0; h < missionInfo.akMissionPairs.Length; h++)
                                    {
                                        if (missionInfo.akMissionPairs[h].key == match.Groups[1].Value)
                                        {
                                            tempValue = missionInfo.akMissionPairs[h].value;
                                        }
                                    }
                                    //try
                                    //{
                                    //    iPre = Int32.Parse(tempValue);
                                    //    iAft = Int32.Parse(match.Groups[2].Value);
                                    //}
                                    //catch (Exception e)
                                    //{
                                    //    string Error = string.Format("[Task ParseMissionText]Task ID {0} TaskInfo Key [{1}] Parse Value [{2}] to Int Error \n {3}", missionInfo.taskID, match.Groups[1].Value, match.Groups[2].Value, singleTaskInfo[i]);
                                    //    Logger.LogWarning(Error);
                                    //    break;
                                    //}
                                    iPre = Int32.Parse(tempValue);
                                    iAft = Int32.Parse(match.Groups[2].Value);
                                    if (bMarkDefault)
                                    {
                                        iPre = iAft;
                                    }

                                    if (iPre >= iAft)
                                    {
                                        haveFinish = true;
                                        kStringBuilder.Append("<color=grey>");
                                    }
                                    else
                                    {
                                        kStringBuilder.Append("<color=white>");
                                    }
                                    kStringBuilder.AppendFormat("{0}/{1}", iPre, iAft);
                                    kStringBuilder.Append("</Color>");
                                    missionSchedule = string.Format("{0}/{1}", iPre, iAft);
                                }
                                break;
                            case MissionKeyType.MKT_KEY_KEY:
                                Int32 iKey0 = 0;
                                Int32 iKey1 = 0;

                                //string value = "";

                                //for (int h = 0; h < missionInfo.akMissionPairs.Length; h++)
                                //{
                                //    if (missionInfo.akMissionPairs[h].key == match.Groups[1].Value)
                                //    {
                                //        value = missionInfo.akMissionPairs[h].value;
                                //    }
                                //}
                                //try
                                //{
                                //    iKey0 = Int32.Parse(value);
                                //    iKey1 = Int32.Parse(match.Groups[2].Value);
                                //}
                                //catch (Exception e)
                                //{
                                //    string Error = string.Format("[Task ParseMissionText]Task ID {0} TaskInfo Key [{1}] Parse Value [{2}] to Int Error \n {3}", missionInfo.taskID, match.Groups[1].Value, match.Groups[2].Value, singleTaskInfo[i]);
                                //    Logger.LogWarning(Error);
                                //    break;
                                //}
                                string value0 = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)missionInfo.taskID, match.Groups[1].Value);
                                string value1 = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)missionInfo.taskID, match.Groups[2].Value);
                                try
                                {
                                    iKey0 = Int32.Parse(value0);
                                    iKey1 = Int32.Parse(value1);
                                }
                                catch (Exception e)
                                {
                                    string Error = string.Format("[Task ParseMissionText]Task ID {0} TaskInfo Key0 [{1}] Key1 [{2}] to Int Error \n {3}", missionInfo.taskID, match.Groups[1].Value, match.Groups[2].Value, singleTaskInfo[i]);
                                    Logger.LogWarning(Error);
                                    break;
                                }
                                if (iKey0 >= iKey1)
                                {
                                    haveFinish = true;
                                    kStringBuilder.Append("<color=grey>");
                                }
                                else
                                {
                                    kStringBuilder.Append("<color=white>");
                                }
                                kStringBuilder.AppendFormat("{0}/{1}", iKey0, iKey1);
                                kStringBuilder.Append("</Color>");
                                missionSchedule = string.Format("{0}/{1}", iKey0, iKey1);
                                break;
                        }

                        indexText = match.Index + match.Length;
                    }
                }
                kStringBuilder.Append(singleTaskInfo[i].Substring(indexText, singleTaskInfo[i].Length - indexText));
                if (i != singleTaskInfo.Length - 1)
                {
                    kStringBuilder.Append("\r\n");
                }
            }
            if (onlySchedule)
            {
                totalString = "" + missionSchedule;
            }
            else
            {
                totalString = kStringBuilder.ToString();
                StringBuilderCache.Release(kStringBuilder);
            }

        }
        if(haveFinish)
        {
            return "已完成";
        }
        else
        {
            return totalString;
        }
    }
    public static string ParseMissionText(Int32 taskId, bool bCondition = false, bool bMarkDefault = false , bool onlySchedule = false)
    {
        string totalString = "";
        var missionValue = MissionManager.GetInstance().GetMission((uint)taskId);
        if (missionValue != null)
        {
            string missionDesc = null;
            if ((int)Protocol.TaskStatus.TASK_INIT == missionValue.status)
            {
                missionDesc = missionValue.missionItem.TaskInitText;
            }
            else if ((int)Protocol.TaskStatus.TASK_UNFINISH == missionValue.status)
            {
                missionDesc = missionValue.missionItem.TaskAcceptedText;
            }
            else
            {
                missionDesc = missionValue.missionItem.TaskFinishText;
            }

            //攻城怪物类型的任务，获得怪物NPC的名字，内容拼接直接返回
            //在任务初始化，和接受任务的时候，这样解析，其他的情况按照以前来解析
            //以前的超链接解析太过麻烦，重复计算过多，完全没有必要，可以优化。。。。。
            if (missionValue.missionItem.SubType == MissionTable.eSubType.SummerNpc)
            {
                if (missionValue.status == (int) Protocol.TaskStatus.TASK_UNFINISH
                    || missionValue.status == (int) Protocol.TaskStatus.TASK_INIT)
                {
                    var missionNpcName = AttackCityMonsterDataManager.GetInstance()
                        .GetMissionNpcName(missionValue.taskContents);
                    var finalMissionDesc = string.Format(missionDesc, missionNpcName);

                    return finalMissionDesc;
                }
            }

            string[] singleTaskInfo = null;

            if (!bCondition)
            {
                singleTaskInfo = (missionValue.missionItem.TaskInformationText + "\n" + missionDesc).ToString().Split(new char[] { '\r', '\n' });
            }
            else
            {
                singleTaskInfo = missionDesc.ToString().Split(new char[] { '\r', '\n' });
            }

            StringBuilder kStringBuilder = StringBuilderCache.Acquire();
            for (int i = 0; i < singleTaskInfo.Length; ++i)
            {
                var indexText = 0;
                for (int j = 0; j < (int)MissionKeyType.MKT_COUNT; ++j)
                {
                    foreach (Match match in ms_missionkey_regex[j].Matches(singleTaskInfo[i]))
                    {
                        kStringBuilder.Append(singleTaskInfo[i].Substring(indexText, match.Index - indexText));

                        switch ((MissionKeyType)j)
                        {
                            case MissionKeyType.MKT_KEY:
                                {
                                    string value = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)taskId, match.Groups[1].Value);
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        kStringBuilder.Append(value);
                                    }
                                }
                                break;
                            case MissionKeyType.MKT_KEY_VALUE:
                                {
                                    Int32 iPre = 0;
                                    Int32 iAft = 0;
                                    string value = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)taskId, match.Groups[1].Value);
                                    try
                                    {
                                        iPre = Int32.Parse(value);
                                        iAft = Int32.Parse(match.Groups[2].Value);
                                    }
                                    catch (Exception e)
                                    {
                                        string Error = string.Format("[Task ParseMissionText]Task ID {0} TaskInfo Key [{1}] Parse Value [{2}] to Int Error \n {3}", taskId, match.Groups[1].Value, match.Groups[2].Value, singleTaskInfo[i]);
                                        Logger.LogWarning(Error);
                                        break;
                                    }

                                    if (bMarkDefault)
                                    {
                                        iPre = iAft;
                                    }

                                    if (iPre >= iAft)
                                    {
                                        kStringBuilder.Append("<color=grey>");
                                    }
                                    else
                                    {
                                        kStringBuilder.Append("<color=white>");
                                    }
                                    kStringBuilder.AppendFormat("{0}/{1}", iPre, iAft);
                                    kStringBuilder.Append("</Color>");
                                }
                                break;
                            case MissionKeyType.MKT_KEY_KEY:
                                Int32 iKey0 = 0;
                                Int32 iKey1 = 0;
                                string value0 = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)taskId, match.Groups[1].Value);
                                string value1 = GameClient.MissionManager.GetInstance().GetMissionValueByKey((uint)taskId, match.Groups[2].Value);
                                try
                                {
                                    iKey0 = Int32.Parse(value0);
                                    iKey1 = Int32.Parse(value1);
                                }
                                catch (Exception e)
                                {
                                    string Error = string.Format("[Task ParseMissionText]Task ID {0} TaskInfo Key0 [{1}] Key1 [{2}] to Int Error \n {3}", taskId, match.Groups[1].Value, match.Groups[2].Value, singleTaskInfo[i]);
                                    Logger.LogWarning(Error);
                                    break;
                                }
                                if (iKey0 >= iKey1)
                                {
                                    kStringBuilder.Append("<color=grey>");
                                }
                                else
                                {
                                    kStringBuilder.Append("<color=white>");
                                }
                                kStringBuilder.AppendFormat("{0}/{1}", iKey0, iKey1);
                                kStringBuilder.Append("</Color>");
                                break;
                        }

                        indexText = match.Index + match.Length;
                    }
                }
                kStringBuilder.Append(singleTaskInfo[i].Substring(indexText, singleTaskInfo[i].Length - indexText));
                if (i != singleTaskInfo.Length - 1)
                {
                    kStringBuilder.Append("\r\n");
                }
            }
            totalString = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);
        }
        return totalString;
    }

    public static void SetMissionTypeIcon(UnityEngine.UI.Image imgTypeIcon, ProtoTable.MissionTable.eTaskType eTaskType, bool bNeedSetNativeSize = false)
    {
        if (imgTypeIcon == null)
        {
            return;
        }
        string[] spriteName =
        {
            "UI/Image/Mission/Taskbook_typeOutside",//TT_DIALY
			"UI/Image/Mission/Taskbook_typeMain",//Main
			"UI/Image/Mission/Taskbook_typeSystem",//TT_SYSTEM
			"UI/Image/Mission/Taskbook_typeAchievement",//TT_ACHIEVEMENT
			"UI/Image/Mission/Taskbook_typeOutside",//TT_BRANCH
			"UI/Image/Mission/Taskbook_typeActivity",//TT_ACTIVITY
			"UI/Image/Mission/Taskbook_typeOutside",//TT_EXTENTION
        };

        if (eTaskType >= 0 && (int)eTaskType < spriteName.Length)
        {
            // imgTypeIcon.sprite = Resources.Load<Sprite>(spriteName[(int)eTaskType]);
            ETCImageLoader.LoadSprite(ref imgTypeIcon, spriteName[(int)eTaskType]);

            if (bNeedSetNativeSize)
            {
                imgTypeIcon.SetNativeSize();
            }
        }
    }

    public static void SetImageIcon(GameObject goImage, string icon, bool bSetNativeSize = false)
    {
        if (goImage != null)
        {
            UnityEngine.UI.Image kImage = goImage.GetComponent<UnityEngine.UI.Image>();
            if (kImage != null)
            {
                // kImage.sprite = AssetLoader.instance.LoadRes(icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref kImage, icon);
                if (bSetNativeSize)
                {
                    kImage.SetNativeSize();
                }
            }
        }
    }

    public static void SetChildTextContent(Transform parent, string name, string content, bool bVisble = true)
    {
        if (parent != null && name != null)
        {
            Transform child = parent.Find(name);
            if (child != null)
            {
                UnityEngine.UI.Text text = child.GetComponent<UnityEngine.UI.Text>();
                if (text != null)
                {
                    text.gameObject.SetActive(bVisble);
                    text.text = content;
                }
            }
        }
    }

    public static void BindCachedNetMsg<T>(T target, GameClient.MessageEvents msgEvents)
    {
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        List<MessageHandleAttribute> akAttributes = null;
        for (int i = 0; i < methods.Length; ++i)
        {
            object[] oats = methods[i].GetCustomAttributes(typeof(MessageHandleAttribute), false);
            if (oats.Length > 0)
            {
                MessageHandleAttribute msgAttribute = oats[0] as MessageHandleAttribute;
                if (msgAttribute != null && msgAttribute.bNeedCache)
                {
                    if (akAttributes == null)
                    {
                        akAttributes = new List<MessageHandleAttribute>();
                    }
                    akAttributes.Add(msgAttribute);
                }
            }
        }

        if (akAttributes != null)
        {
            akAttributes.Sort((x, y) =>
            {
                return x.order - y.order;
            });
            for (int i = 0; i < akAttributes.Count; ++i)
            {
                msgEvents.AddMessage(akAttributes[i].id);
            }
        }
    }

    class OrderMethod
    {
        public MessageHandleAttribute attr;
        public List<Network.MsgDATA> datas;
        public MethodInfo method;
        public object target;
        public void Invoke()
        {
            for (int j = 0; j < datas.Count; ++j)
            {
                method.Invoke(target, new object[] { datas[j] });
            }
        }
    }

    public static void UnBindCachedNetMsg<T>(T target, GameClient.MessageEvents msgEvents)
    {
        List<OrderMethod> methodList = null;
        Type type = target.GetType();
        MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        for (int i = 0; i < methods.Length; ++i)
        {
            object[] oats = methods[i].GetCustomAttributes(typeof(MessageHandleAttribute), false);
            if (oats.Length > 0)
            {
                MessageHandleAttribute msgAttribute = oats[0] as MessageHandleAttribute;
                if (msgAttribute.bNeedCache)
                {
                    List<Network.MsgDATA> datas = msgEvents.GetMessageDatas(msgAttribute.id);
                    if (datas != null && datas.Count > 0)
                    {
                        if (methodList == null)
                        {
                            methodList = new List<OrderMethod>();
                        }
                        OrderMethod orderMethod = new OrderMethod();
                        orderMethod.attr = msgAttribute;
                        orderMethod.datas = datas;
                        orderMethod.method = methods[i];
                        orderMethod.target = target;
                        methodList.Add(orderMethod);
                    }
                }
            }
        }

        if (methodList != null)
        {
            methodList.Sort((x, y) =>
            {
                return x.attr.order - y.attr.order;
            });

            for (int i = 0; i < methodList.Count; ++i)
            {
                methodList[i].Invoke();
            }
        }
    }

    public static void SetChildTextColor(Transform parent, string name, Color color)
    {
        if (parent != null)
        {
            Transform child = parent.Find(name);
            if (child != null)
            {
                UnityEngine.UI.Text text = child.GetComponent<UnityEngine.UI.Text>();
                if (text != null)
                {
                    text.color = color;
                }
            }
        }
    }

    public static object BytesToObject(byte[] Bytes)
    {
        using (MemoryStream stream = new MemoryStream(Bytes))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
    }

    public static NameResult CheckRoleName(string inputName)
    {
        int byteCount = 0;
        byteCount = GetByteCount(inputName);
        bool flag = true;
        if (string.IsNullOrEmpty(inputName))
        {
            return NameResult.Null;
        }
        for (int i = 0; i < inputName.Length; i++)
        {
            if (!IsQuanjiaoChar(inputName.Substring(i, 1)))
            {
                flag = false;
            }
        }
        if (flag)
        {
            if ((byteCount > MAX_CHINESE_NAME_LEN) || (byteCount < MIN_CHINESE_NAME_LEN))
            {
                return NameResult.OutOfLength;
            }
        }
        else if ((byteCount > MAX_EN_NAME_LEN) || (byteCount < MIN_EN_NAME_LEN))
        {
            return NameResult.OutOfLength;
        }
        return NameResult.Vaild;
    }

    public static string CreateMD5Hash(string input)
    {
        MD5 md = MD5.Create();
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        byte[] buffer2 = md.ComputeHash(bytes);
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < buffer2.Length; i++)
        {
            builder.Append(buffer2[i].ToString("X2"));
        }
        return builder.ToString();
    }

    public static string DateTimeFormatString(DateTime dt, enDTFormate fm)
    {
        if (fm == enDTFormate.DATE)
        {
            return string.Format("{0:D4}-{1:D2}-{2:D2}", dt.Year, dt.Month, dt.Day);
        }
        if (fm == enDTFormate.TIME)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", dt.Hour, dt.Minute, dt.Second);
        }
        return (string.Format("{0:D4}-{1:D2}-{2:D2}", dt.Year, dt.Month, dt.Day) + " " + string.Format("{0:D2}:{1:D2}:{2:D2}", dt.Hour, dt.Minute, dt.Second));
    }

    public static GameObject FindChild(GameObject p, string path)
    {
        if (p != null)
        {
            Transform transform = p.transform.Find(path);
            return ((transform == null) ? null : transform.gameObject);
        }
        return null;
    }

    public static GameObject FindChildByName(Component component, string childpath)
    {
        return FindChildByName(component.gameObject, childpath);
    }

    public static GameObject FindChildByName(GameObject root, string childpath)
    {
        GameObject obj2 = null;
        char[] separator = new char[] { '/' };
        string[] strArray = childpath.Split(separator);
        GameObject gameObject = root;
        foreach (string str in strArray)
        {
            bool flag = false;
            IEnumerator enumerator = gameObject.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform)enumerator.Current;
                    if (current.gameObject.name == str)
                    {
                        gameObject = current.gameObject;
                        flag = true;
                        goto Label_0098;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        Label_0098:
            if (!flag)
            {
                break;
            }
        }
        if (gameObject != root)
        {
            obj2 = gameObject;
        }
        return obj2;
    }

    public static GameObject FindChildSafe(GameObject p, string path)
    {
        if (p != null)
        {
            Transform transform = p.transform.Find(path);
            if (transform != null)
            {
                return transform.gameObject;
            }
        }
        return null;
    }

    public static float FrameToTime(int frame)
    {
        return (frame * Time.fixedDeltaTime);
    }

    public static int GetByteCount(string inputStr)
    {
        int num = 0;
        for (int i = 0; i < inputStr.Length; i++)
        {
            if (IsQuanjiaoChar(inputStr.Substring(i, 1)))
            {
                num += 2;
            }
            else
            {
                num++;
            }
        }
        return num;
    }

    public static T GetComponetInChild<T>(GameObject p, string path) where T : Component
    {
        if ((p == null) || (p.transform == null))
        {
            return null;
        }
        Transform transform = p.transform.Find(path);
        if (transform == null)
        {
            return null;
        }
        return transform.GetComponent<T>();
    }

    private static int GetCpuClock(string cpuFile)
    {
        string s = getFileContent(cpuFile);
        int result = 0;
        if (!int.TryParse(s, out result))
        {
            result = 0;
        }
        return Mathf.FloorToInt((float)(result / 0x3e8));
    }

    public static int ToInt(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            return 0;
        }
        int iValue = 0;
        int.TryParse(text, out iValue);
        return iValue;
    }
    public static uint ToUInt(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }
        uint iValue = 0;
        uint.TryParse(text, out iValue);
        return iValue;
    }
    public static long ToLong(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }
        long iValue = 0;
        long.TryParse(text, out iValue);
        return iValue;
    }
    public static ulong ToULong(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }
        ulong iValue = 0;
        ulong.TryParse(text, out iValue);
        return iValue;
    }   
    public static int GetCpuCurrentClock()
    {
        return GetCpuClock("/sys/devices/system/cpu/cpu0/cpufreq/scaling_cur_freq");
    }

    public static int GetCpuMaxClock()
    {
        return GetCpuClock("/sys/devices/system/cpu/cpu0/cpufreq/cpuinfo_max_freq");
    }

    public static int GetCpuMinClock()
    {
        return GetCpuClock("/sys/devices/system/cpu/cpu0/cpufreq/cpuinfo_min_freq");
    }

    private static string getFileContent(string path)
    {
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception.Message);
            return null;
        }
    }

    public static Vector3 GetGameObjectSize(GameObject obj)
    {
        Vector3 zero = Vector3.zero;
        if (obj.GetComponent<Renderer>() != null)
        {
            zero = obj.GetComponent<Renderer>().bounds.size;
        }
        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
        {
            Vector3 size = renderer.bounds.size;
            zero.x = Math.Max(zero.x, size.x);
            zero.y = Math.Max(zero.y, size.y);
            zero.z = Math.Max(zero.z, size.z);
        }
        return zero;
    }

    public static uint GetNewDayDeltaSec(int nowSec)
    {
        DateTime time = ToUtcTime2Local((long)nowSec);
        DateTime time2 = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan span = (TimeSpan)(time2.AddSeconds(86400.0) - time);
        return (uint)span.TotalSeconds;
    }



    public static System.Type GetType(string typeName)
    {
        if (!string.IsNullOrEmpty(typeName))
        {
            System.Type type = System.Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
        }
        return null;
    }

    public static long GetZeroBaseSecond(long utcSec)
    {
        DateTime time = new DateTime(0x7b2, 1, 1);
        DateTime time2 = time.AddTicks((utcSec + 0x7080L) * 0x989680L);
        DateTime time3 = new DateTime(time2.Year, time2.Month, time2.Day, 0, 0, 0);
        TimeSpan span = (TimeSpan)(time3 - time);
        return (((long)span.TotalSeconds) - 0x7080L);
    }

    public static int Hours2Second(int hour)
    {
        return ((hour * 60) * 60);
    }

    public static bool IsChineseChar(char key)
    {
        int num = Convert.ToInt32(key);
        return ((num >= CHINESE_CHAR_START) && (num <= CHINESE_CHAR_END));
    }

    public static bool IsOverOneDay(int timeSpanSeconds)
    {
        TimeSpan span = new TimeSpan(timeSpanSeconds * 0x989680L);
        return (span.Days > 0);
    }

    public static bool IsQuanjiaoChar(string inputStr)
    {
        return (Encoding.Default.GetByteCount(inputStr) > 1);
    }

    public static bool IsSpecialChar(char key)
    {
        return ((!IsChineseChar(key) && !char.IsLetter(key)) && !char.IsNumber(key));
    }

    public static bool IsValidText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (IsSpecialChar(text[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static int Minutes2Seconds(int min)
    {
        return (min * 60);
    }

    public static byte[] ObjectToBytes(object obj)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            new BinaryFormatter().Serialize(stream, obj);
            return stream.GetBuffer();
        }
    }

    public static byte[] ReadFile(string path)
    {
        FileStream stream = null;
        if (CFileManager.IsFileExist(path))
        {
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                int length = (int)stream.Length;
                byte[] array = new byte[length];
                stream.Read(array, 0, length);
                stream.Close();
                stream.Dispose();
                return array;
            }
            catch (Exception)
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
        return null;
    }

    public static byte[] SByteArrToByte(sbyte[] p)
    {
        byte[] buffer = new byte[p.Length];
        for (int i = 0; i < p.Length; i++)
        {
            buffer[i] = (byte)p[i];
        }
        return buffer;
    }

    public static DateTime SecondsToDateTime(int y, int m, int d, int secondsInDay)
    {
        int hour = secondsInDay / 0xe10;
        secondsInDay = secondsInDay % 0xe10;
        return new DateTime(y, m, d, hour, secondsInDay / 60, secondsInDay % 60);
    }

    public static string SecondsToTimeText(uint secs)
    {
        uint num = secs / 0xe10;
        secs -= num * 0xe10;
        uint num2 = secs / 60;
        secs -= num2 * 60;
        return string.Format("{0:D2}:{1:D2}:{2:D2}", num, num2, secs);
    }

    public static void SetChildrenActive(GameObject root, bool active)
    {
        IEnumerator enumerator = root.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform current = (Transform)enumerator.Current;
                if (current != root.transform)
                {
                    current.gameObject.SetActive(active);
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
    }

    public static void StringToByteArray(string str, ref byte[] buffer)
    {
        byte[] bytes = Encoding.Default.GetBytes(str);
        Array.Copy(bytes, buffer, Math.Min(bytes.Length, buffer.Length));
        buffer[buffer.Length - 1] = 0;
    }

    public static DateTime StringToDateTime(string dtStr, DateTime defaultIfNone)
    {
        ulong num;
        if (ulong.TryParse(dtStr, out num))
        {
            return ULongToDateTime(num);
        }
        return defaultIfNone;
    }

    public static int TimeToFrame(float time)
    {
        return (int)Math.Ceiling((double)(time / Time.fixedDeltaTime));
    }

    public static uint ToUtcSeconds(DateTime dateTime)
    {
        DateTime time = new DateTime(0x7b2, 1, 1);
        if (dateTime.CompareTo(time) >= 0)
        {
            TimeSpan span = (TimeSpan)(dateTime - time);
            if (span.TotalSeconds > 28800.0)
            {
                TimeSpan span2 = (TimeSpan)(dateTime - time);
                return (((uint)span2.TotalSeconds) - 0x7080);
            }
        }
        return 0;
    }

    public static DateTime ToUtcTime2Local(long secondsFromUtcStart)
    {
        DateTime time = new DateTime(0x7b2, 1, 1);
        return time.AddTicks((secondsFromUtcStart + 0x7080L) * 0x989680L);
    }

    public static DateTime ULongToDateTime(ulong dtVal)
    {
        ulong[] numArray = new ulong[6];
        for (int i = 0; i < _DW.Length; i++)
        {
            numArray[i] = dtVal / _DW[i];
            dtVal -= numArray[i] * _DW[i];
        }
        numArray[_DW.Length] = dtVal;
        return new DateTime((int)numArray[0], (int)numArray[1], (int)numArray[2], (int)numArray[3], (int)numArray[4], (int)numArray[5]);
    }

    public static string UTF8Convert(string str)
    {
        return str;
    }

    public static string UTF8Convert(byte[] p)
    {
        return StringHelper.UTF8BytesToString(ref p);
    }

    public static string UTF8Convert(sbyte[] p, int len)
    {
        return UTF8Convert(SByteArrToByte(p));
    }

    public static bool WriteFile(string path, byte[] bytes)
    {
        FileStream stream = null;
        try
        {
            if (!CFileManager.IsFileExist(path))
            {
                stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }
            else
            {
                stream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
            }
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();
            stream.Dispose();
            return true;
        }
        catch (Exception)
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
            }
            return false;
        }
    }

    public enum enDTFormate
    {
        FULL,
        DATE,
        TIME
    }

    public enum NameResult
    {
        Vaild,
        Null,
        OutOfLength,
        InVaildChar
    }

    // TODO  move to Utility.cs or some FileManager
    public static string FormatString(string name)
    {
        return name.Replace('\\', '/');
    }

    public static string ProtocolErrorString(uint error)
    {
        return ((Protocol.ProtoErrorCode)error).ToString();
    }

    public static GameClient.TaskGuideArrow.TaskGuideDir GetPreDirByTargetPos(Vector3 src, Vector3 target)
    {
        GameClient.TaskGuideArrow.TaskGuideDir eDir = GameClient.TaskGuideArrow.TaskGuideDir.TGD_INVALID;

        var current = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
        if (current != null)
        {
            Vector3 dir = current.GetPathFindingDirection(src, target);
            if (dir != Vector3.zero)
            {
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
                {
                    if (dir.x < 0)
                    {
                        eDir = GameClient.TaskGuideArrow.TaskGuideDir.TGD_LEFT;
                    }
                    else
                    {
                        eDir = GameClient.TaskGuideArrow.TaskGuideDir.TGD_RIGHT;
                    }
                }
                else
                {
                    if (dir.z > 0)
                    {
                        eDir = GameClient.TaskGuideArrow.TaskGuideDir.TGD_TOP;
                    }
                    else
                    {
                        eDir = GameClient.TaskGuideArrow.TaskGuideDir.TGD_BOTTOM;
                    }
                }
            }
        }

        return eDir;
    }

    public static GameClient.TaskGuideArrow.TaskGuideDir GetCommandMoveNpcDir(Int32 iNpcID)
    {
        GameClient.TaskGuideArrow.TaskGuideDir eDir = GameClient.TaskGuideArrow.TaskGuideDir.TGD_INVALID;
        var current = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
        if (current != null)
        {
            return GetPreDirByTargetPos(GetMainRolePosition(), current.GetNpcPostion(iNpcID) - new Vector3(0.0f, 0.0f, 1.0f));
        }
        return eDir;
    }


    public static Vector3 GetMainRolePosition()
    {
        Vector3 position = Vector3.zero;

        var current = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
        if (current == null)
        {
            return position;
        }

        if (current.MainPlayer == null)
        {
            return position;
        }

        position = current.MainPlayer.ActorData.MoveData.Position;

        return position;
    }

    public static string GetPathByPkPoints(uint pkPoints, ref int RemainPoints, ref int TotalPoints, ref int pkIndex, ref bool bMaxLv)
    {
        Dictionary<int, object> pkLvTable = TableManager.GetInstance().GetTable<PkLevelTable>();

        int pkID = -1;
        int NextMinPkValue = -1;
        int iIndex = 0;

        foreach (var pkValueID in pkLvTable.Keys)
        {
            var pkData = TableManager.GetInstance().GetTableItem<PkLevelTable>(pkValueID);

            if (pkData == null)
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(string.Format(" pk经验表 没有ID为 {0} 的条目", pkValueID));
                return "";
            }

            if (pkPoints < pkData.MinPkValue)
            {
                pkID = (pkValueID - 1);
                RemainPoints = pkData.MinPkValue - (int)pkPoints;
                NextMinPkValue = pkData.MinPkValue;
                pkIndex = iIndex;

                break;
            }

            iIndex++;
        }

        // 已达到最高级
        if (pkID == -1)
        {
            pkID = pkLvTable.Count;
            RemainPoints = 0;

            TotalPoints = 1;
            bMaxLv = true;
            pkIndex = iIndex;
        }
        else
        {
            bMaxLv = false;
        }

        var pkCurData = TableManager.GetInstance().GetTableItem<PkLevelTable>(pkID);
        if (pkCurData == null)
        {
            return "";
        }

        if (!bMaxLv)
        {
            TotalPoints = NextMinPkValue - pkCurData.MinPkValue;
        }

        return pkCurData.Path;
    }

    public static string GetNameByPkPoints(uint pkPoints, ref int iLevelType)
    {
        Dictionary<int, object> pkLvTable = TableManager.GetInstance().GetTable<PkLevelTable>();

        int iId = -1;

        foreach (var pkValueID in pkLvTable.Keys)
        {
            var pkData = TableManager.GetInstance().GetTableItem<PkLevelTable>(pkValueID);

            if (pkData == null)
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(string.Format(" pk经验表 没有ID为 {0} 的条目", pkValueID));
                return "";
            }

            if (pkPoints < pkData.MinPkValue)
            {
                iId = pkValueID - 1;
                break;
            }
        }

        if (iId == -1)
        {
            iId = pkLvTable.Count;
        }

        var pkCurData = TableManager.GetInstance().GetTableItem<PkLevelTable>(iId);
        if (pkCurData == null)
        {
            return "";
        }

        iLevelType = pkCurData.PkLevelType;

        return pkCurData.Name;
    }

    public static void GetPKValueNumAndColor(int index, ref Color color1, ref Color color2)
    {
        color1 = new Color32(0xc2, 0xad, 0x6f, 0xff);
        color1 = new Color32(0xc2, 0xad, 0x6f, 0xff);

        switch (index)
        {
            case 1:
                {
                    color1 = new Color32(0xc0, 0xc0, 0xc0, 0xff);
                    color2 = new Color32(0xc0, 0xc0, 0xc0, 0xff);
                }
                break;
            case 2:
                {
                    color1 = new Color32(0xff, 0xc5, 0x1b, 0xff);
                    color2 = new Color32(0xff, 0xc5, 0x1b, 0xff);
                }
                break;
            case 3:
                {
                    color1 = new Color32(0xff, 0xfe, 0xda, 0xff);
                    color2 = new Color32(0xff, 0xf1, 0x58, 0xff);
                }
                break;
            case 4:
                {
                    color1 = new Color32(0xf7, 0xe9, 0x0d, 0xff);
                    color2 = new Color32(0xe1, 0x00, 0x19, 0xff);
                }
                break;
        }
    }

    /// <summary>  
    /// 获取当前本地时间戳  
    /// </summary>  
    /// <returns></returns>        
    public static UInt32 GetCurrentTimeUnix()  
    {  
        TimeSpan cha = (DateTime.Now - Function.sStartTime);  
        return (UInt32)cha.TotalSeconds;  
    }

    public static double GetTimeStamp()
    {
        TimeSpan cha = (DateTime.Now - Function.sStartTime);
        return cha.TotalSeconds;
    }
    public static System.DateTime GetDateTimeByUnixTime(double d)
    {
        return Function.sStartTime.AddSeconds(d);
    }

    public static bool CheckCanChangeJob()
    {
        ClientSystemTown clientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
        if (clientSystem == null)
        {
            return false;
        }

        if (PlayerBaseData.GetInstance().Level > 30)
        {
            return false;
        }

        if (!MissionManager.GetInstance().HasAcceptedChangeJobMainMission())
        {
            return false;
        }

        if (MissionManager.GetInstance().HasAcceptedChangeJobMission())
        {
            return false;
        }

        return true;
    }

    public static bool CheckCanAwake()
    {
        ClientSystemTown clientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
        if (clientSystem == null)
        {
            return false;
        }

        if (PlayerBaseData.GetInstance().Level < ClientSystemTown.Awakelevel)
        {
            return false;
        }

        JobTable JobTableData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
        if (JobTableData == null)
        {
            return false;
        }

        if (JobTableData.JobType < 1 || PlayerBaseData.GetInstance().AwakeState > 0 || MissionManager.GetInstance().HasAcceptedAwakeMission())
        {
            return false;
        }

        return true;
    }

    public static bool CheckShowAwakeTaskBtn()
    {
        ClientSystemTown clientSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
        if (clientSystem == null)
        {
            return false;
        }

        if (PlayerBaseData.GetInstance().Level < ClientSystemTown.Awakelevel)
        {
            return false;
        }

        ProtoTable.JobTable JobTableData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
        if (JobTableData == null)
        {
            return false;
        }

        if (JobTableData.JobType < 1 || PlayerBaseData.GetInstance().AwakeState > 0 || !MissionManager.GetInstance().HasAcceptedAwakeMission())
        {
            return false;
        }

        return true;
    }

    //public static Sprite createSprite(string path)
    //{
    //    if (path == null)
    //    {
    //        return null;
    //    }
    //    return AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
    //}

    public static void createSprite(string path, ref Image img)
    {
        if (path == null)
        {
            return;
        }
        // return AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
        ETCImageLoader.LoadSprite(ref img, path);
    }

    public static bool IsDailyMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_DIALY)
        {
            return true;
        }
        return false;
    }

    public static bool IsDailyNormal(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_DIALY
            && missionItem.SubType == ProtoTable.MissionTable.eSubType.Daily_Task)
        {
            return true;
        }
        return false;
    }

    public static bool IsDailyProve(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_DIALY
            && missionItem.SubType == ProtoTable.MissionTable.eSubType.Daily_Prove)
        {
            return true;
        }
        return false;
    }

    public static bool IsChangeJobTask(UInt32 iMissionID)
    {
        MissionTable missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == MissionTable.eTaskType.TT_CHANGEJOB)
        {
            return true;
        }

        return false;
    }

    public static bool IsAwakeTask(UInt32 iMissionID)
    {
        MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_AWAKEN)
        {
            return true;
        }

        return false;
    }

    public static bool IsMainMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN)
        {
            return true;
        }
        return false;
    }

    public static bool IsBranchMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_BRANCH)
        {
            return true;
        }
        return false;
    }

    public static bool IsAchievementMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT)
        {
            return true;
        }
        return false;
    }

    public static bool IsAccountAchievementMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TASK_ACCOUNT_ACHIEVEMENT)
        {
            return true;
        }
        return false;
    }

    public static bool IsAdventureTeamAccountWeeklyMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TASK_ADVENTURE_TEAM_ACCOUNT_WEEKLY)
        {
            return true;
        }
        return false;
    }

    public static bool IsLegendMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_LEGEND)
        {
            return true;
        }
        return false;
    }

    public static bool IsLegendSeriesOverOnce(int iTableId, int finishedMissionId)
    {
        var mainItem = TableManager.GetInstance().GetTableItem<ProtoTable.LegendMainTable>(iTableId);
        if (null == mainItem)
            return false;

        for (var i = 0; i < mainItem.missionIds.Count; i++)
        {
            var curMissionId = mainItem.missionIds[i];
            var curMissionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(curMissionId);
            if(null == curMissionItem || 1 != curMissionItem.MissionOnOff)
                continue;

            var missionValue = MissionManager.GetInstance().GetMission((uint) curMissionId);
            
            if (curMissionId == finishedMissionId)
            {
                //当前任务不是第一次完成,如果不是，则返回false；如果是则判断其他的任务
                if (null == missionValue || 1 !=  missionValue.submitCount)
                {
                    return false;
                }
            }
            else
            {
                //其他任务存在没有完成的情况
                if (null == missionValue || missionValue.submitCount < 1)
                    return false;
            }
        }
        
        return true;
    }

    public static bool IsLegendSeriesOver(int iTableID)
    {
        var mainItem = TableManager.GetInstance().GetTableItem<ProtoTable.LegendMainTable>(iTableID);
        if (null == mainItem)
        {
            return false;
        }

        for (int i = 0; i < mainItem.missionIds.Count; ++i)
        {
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(mainItem.missionIds[i]);
            if (null == missionItem || missionItem.MissionOnOff != 1)
            {
                continue;
            }

            var missionValue = MissionManager.GetInstance().GetMission((uint)mainItem.missionIds[i]);
            if (null == missionValue || missionValue.status != (int)Protocol.TaskStatus.TASK_OVER)
            {
                return false;
            }
        }

        return true;
    }

    public static int GetLegendMainStatus(ProtoTable.LegendMainTable mainItem)
    {
        if (Utility.IsLegendSeriesOver(mainItem.ID))
        {
            return 2;//TASK_OVER
        }

        if (PlayerBaseData.GetInstance().Level < mainItem.UnLockLevel)
        {
            return 1;//TASK_INIT
        }

        return 0;//TASK_UN_FINISH
    }

    public static bool IsAchievementMissionNormal(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT &&
            missionItem.SubType == MissionTable.eSubType.Daily_Null)
        {
            return true;
        }
        return false;
    }

    public static bool IsChangeJobMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_CHANGEJOB)
        {
            return true;
        }
        return false;
    }

    public static bool IsAwakeMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_AWAKEN)
        {
            return true;
        }
        return false;
    }

    public static bool IsNormalMission(UInt32 iMissionID)
    {
        ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
        if (missionItem != null && missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT && missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_DIALY)
        {
            return true;
        }
        return false;
    }

    public static int ceil(float fValue)
    {
        return (int)(fValue + 0.50f);
    }

    public static string GetEnumDescription<T>(T enumValue)
    {
        string str = enumValue.ToString();
        System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
        if (field != null)
        {
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs != null && objs.Length > 0)
            {
                System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
                str = da.Description;
            }

            objs = field.GetCustomAttributes(typeof(UIPropertyAttribute), false);
            if (objs != null && objs.Length > 0)
            {
                UIPropertyAttribute da = (UIPropertyAttribute)objs[0];
                str = da.name + da.formatString;
            }
        }
        return str;
    }

    public static string GetEnumCommonValue<T>(T enumValue, int iIndex = 0)
    {
        System.Reflection.FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
        if (field != null)
        {
            object[] objs = field.GetCustomAttributes(typeof(EnumCommonAttribute), false);
            if (objs != null && objs.Length > 0)
            {
                var attr = objs[0] as EnumCommonAttribute;
                return attr.GetValueByIndex(iIndex);
            }
        }
        return "";
    }

    public static Attribute GetEnumAttribute<T, Attribute>(T enumValue)
    {
        string str = enumValue.ToString();
        System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
        if (field != null)
        {
            object[] objs = field.GetCustomAttributes(typeof(Attribute), false);
            if (objs != null && objs.Length > 0)
            {
                return (Attribute)objs[0];
            }
        }
        return default(Attribute);
    }

    public static List<int> GetAwakeTaskList()
    {
        List<int> AwakeTaskList = new List<int>();

        var MissionTable = TableManager.GetInstance().GetTable<MissionTable>();
        var enumerator = MissionTable.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var Item = enumerator.Current.Value as MissionTable;

            if (Item.TaskType != ProtoTable.MissionTable.eTaskType.TT_AWAKEN)
            {
                continue;
            }

            if (Item.JobID != PlayerBaseData.GetInstance().JobTableID && Item.JobID != 0)
            {
                continue;
            }

            AwakeTaskList.Add(Item.ID);
        }

        return AwakeTaskList;
    }

    public static List<int> GetChangeJobTaskList()
    {
        List<int> ChangeJobTaskList = new List<int>();

        var changeJobList = MissionManager.GetInstance().GetDiffTask(ProtoTable.MissionTable.eTaskType.TT_CHANGEJOB);
        if (changeJobList == null || changeJobList.Count < 1)
        {
            return ChangeJobTaskList;
        }

        var MissionTable = TableManager.GetInstance().GetTable<MissionTable>();
        var enumerator = MissionTable.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var Item = enumerator.Current.Value as MissionTable;

            if (Item.TaskType == ProtoTable.MissionTable.eTaskType.TT_CHANGEJOB)
            {
                for (int i = 0; i < changeJobList.Count; i++)
                {
                    if (changeJobList[i].missionItem.MissionParam == Item.MissionParam && changeJobList[i].status > 0)
                    {
                        ChangeJobTaskList.Add(Item.ID);
                    }
                }
            }
        }

        return ChangeJobTaskList;
    }

    public static bool IsUnLockFunc(int iID)
    {
        bool bUnLock = false;

        for (int i = 0; i < PlayerBaseData.GetInstance().UnlockFuncList.Count; i++)
        {
            if (PlayerBaseData.GetInstance().UnlockFuncList[i] == iID)
            {
                bUnLock = true;
                break;
            }
        }

        return bUnLock;
    }

    public static bool IsUnLockArea(int iAreaID)
    {
        bool bUnLock = false;

        for (int i = 0; i < PlayerBaseData.GetInstance().UnlockAreaList.Count; i++)
        {
            if (PlayerBaseData.GetInstance().UnlockAreaList[i] == iAreaID)
            {
                bUnLock = true;
                break;
            }
        }

        return bUnLock;
    }

    public static int GetMallRealPrice(MallItemInfo iteminfo)
    {
        int RealPrice = (int)iteminfo.price;

        if (iteminfo.discountprice > 0)
        {
            RealPrice = (int)iteminfo.discountprice;
        }

        return RealPrice;
    }

    public static int GetMallRealPrice(MallItemDetailInfo iteminfo)
    {
        int RealPrice = (int)iteminfo.price;

        if (iteminfo.discountPrice > 0)
        {
            RealPrice = (int)iteminfo.discountPrice;
        }

        return RealPrice;
    }

    public enum EItemLimitType
    {
        None,//无(永久
        Monthly,//每月
        Weekly,//每周
        Daily,//每日
        Activityly,//活动时间内
        Mystery,//神秘商人
        Season,//每赛季
    }
    
    //获取商城道具的购买上限
    public static int GetMallItemBuyLimit(MallItemInfo mMallItemData)
    {
        int MaxNum = 0;
        var costItemTable = MallNewDataManager.GetInstance().GetCostItemTableByCostType(mMallItemData.moneytype);
        var ownerCostNumber = ItemDataManager.GetInstance().GetOwnedItemCount(costItemTable.ID);
        var iPrice = GetMallRealPrice(mMallItemData);
        //五一礼包标记并且全民团购活动开启
        if (mMallItemData.tagType == 2 && ActivityDataManager.GetInstance().CheckGroupPurchaseActivityIsOpen())
        {
            var voucherTicketData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemData.deductionCouponId);
            var discountTicketData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemData.discountCouponId);
            int voucherTicketCount = 0;
            if (voucherTicketData != null)
                voucherTicketCount = ItemDataManager.GetInstance().GetOwnedItemCount(voucherTicketData.ID);
            int discountTicketCount = 0;
            if (discountTicketData != null)
                discountTicketCount = ItemDataManager.GetInstance().GetOwnedItemCount(discountTicketData.ID);
            int totalPrice = 0;
            while (true)
            {
                //乘以最终折扣系数
                int price = (int)Math.Floor(Convert.ToDecimal(iPrice * ActivityDataManager.LimitTimeGroupBuyDiscount * 1.0f / 100));
                if (discountTicketData != null)
                {  
                    //计算折扣券
                    if (MaxNum < discountTicketCount)
                        price = (int)Math.Floor(Convert.ToDecimal(price * discountTicketData.DiscountCouponProp * 1.0f / 100));
                }

                if (voucherTicketData != null)
                {
                    //计算抵扣券
                    if (MaxNum < voucherTicketCount)
                        price = price - voucherTicketData.DiscountCouponProp;
                }
                totalPrice += price;
                //总价格大于已有点券 
                if (totalPrice > ownerCostNumber)
                    break;
                MaxNum++;
            }
        }
        else
        {
            if (!ActivityDataManager.GetInstance().IsShowFirstDiscountDes(mMallItemData.id))
            {
                if (costItemTable != null)
                {
                    var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)mMallItemData.discountCouponId);
                    if (itemTableData != null && itemTableData.DiscountCouponProp != 0)
                    {
                        //折扣卷的数量
                        int itemCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)mMallItemData.discountCouponId);
                        //折扣之后的价格
                        int tempDisPrice = (int)Math.Floor(Convert.ToDecimal(iPrice * itemTableData.DiscountCouponProp * 1.0f / 100));
                        int canBuyCount;
                        if (tempDisPrice != 0)
                            canBuyCount = ownerCostNumber / tempDisPrice;//全用打折券可以买多少个
                        else
                        {
                            Logger.LogErrorFormat("tempDisPrice = 0为分母 其中iPrice = {0},itemID = {1}", iPrice, itemTableData.ID);
                            canBuyCount = 1;
                        }
                        if (canBuyCount <= itemCount)
                            MaxNum = canBuyCount;
                        else
                        {
                            if (iPrice != 0)
                            {
                                //打折的数量+不打折的数量
                                MaxNum = itemCount + (ownerCostNumber - (tempDisPrice * itemCount)) / iPrice;
                            }
                            else
                            {
                                Logger.LogErrorFormat("price = 0 其中itemID = {0}", itemTableData.ID);
                                MaxNum = 1;
                            }
                        }
                    }
                    else
                    {
                        if (iPrice > 0)
                            MaxNum = ownerCostNumber / iPrice;
                    }
                }
            }
            else
            {
                var activityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY);
                if (activityData != null)
                {
                    //折扣之后的价格
                    int tempDisPrice = (int)Math.Floor(Convert.ToDecimal(iPrice * (int)activityData.parm * 1.0f / 100));
                    if (ownerCostNumber >= tempDisPrice * 1)
                        //可以折扣+不能折扣的价格
                        MaxNum = 1 + (ownerCostNumber - tempDisPrice * 1) / iPrice;
                    else
                        MaxNum = 0;
                }
            }
        }
        int limitnum = 0;
        bool isDailyLimit = false;
        limitnum = Utility.GetLeftLimitNum(mMallItemData, ref isDailyLimit);
        if (mMallItemData.limit > 0 && MaxNum > limitnum)
        {
            MaxNum = limitnum;
        }
        //账号限购
        if (mMallItemData != null && mMallItemData.accountLimitBuyNum > 0)
        {
            if (MaxNum > mMallItemData.accountRestBuyNum)
                MaxNum = (int)mMallItemData.accountRestBuyNum;
        }
        //叠加数
        int itemId = mMallItemData.itemid != 0 ? (int)mMallItemData.itemid : (int)mMallItemData.giftItems[0].id;
        ItemTable ItemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
        if (MaxNum > ItemTableData.MaxNum && ItemTableData.MaxNum != 0)
        {
            MaxNum = ItemTableData.MaxNum;
        }
        //最大可拥有数量
        if (ItemTableData.GetLimitNum != 0 && MaxNum > ItemTableData.GetLimitNum - ItemDataManager.GetInstance().GetItemCountInPackage(ItemTableData.ID))
        {
            MaxNum = ItemTableData.GetLimitNum - ItemDataManager.GetInstance().GetItemCountInPackage(ItemTableData.ID);
        }
        return MaxNum;
    }

    //获取商城道具的限时类型
    public static EItemLimitType GetMallItemLimitType(MallItemInfo item)
    {
        var type = (Utility.EItemLimitType)item.accountRefreshType;
        //如果账号限购为0 可能是角色限购
        if (type == Utility.EItemLimitType.None && 0 != item.limit)
        {
            if (item.limit == (byte)LimitTimeGift.ELimitiTimeGiftDataLimitType.Refresh)
                type = EItemLimitType.Daily;
            else if (item.limit == (byte)LimitTimeGift.ELimitiTimeGiftDataLimitType.Week)
                type = EItemLimitType.Weekly;
        }
        return EItemLimitType.None;
    }

    //获取商店道具的限时类型
    public static EItemLimitType GetShopItemLimitType(int shopId)
    {
        var shopItemTable  = TableManager.GetInstance().GetTableItem<ShopItemTable>(shopId);
        var isCanRefresh = false;
        var refreshIndex = -1;
        var shopTable = TableManager.GetInstance().GetTableItem<ShopTable>(shopId);
        if (shopTable == null)
            return EItemLimitType.None;

        if ((shopTable.Refresh == 2 || shopTable.Refresh == 1)
            && (shopTable.SubType.Count == shopTable.NeedRefreshTabs.Count)
            && (shopTable.SubType.Count == shopTable.RefreshCycle.Count))
        {
            for (var i = 0; i < shopTable.SubType.Count; i++)
            {
                if ((int)shopTable.SubType[i] == (int)shopItemTable.SubType)
                {
                    refreshIndex = i;
                    break;
                }
            }
        }
        if (refreshIndex != -1 && shopTable.NeedRefreshTabs[refreshIndex] == 1)
            isCanRefresh = true;
        if (isCanRefresh == true)
        {
            switch (shopTable.RefreshCycle[refreshIndex])
            {
                case ShopTable.eRefreshCycle.REFRESH_CYCLE_NONE:
                    return EItemLimitType.None;
                case ShopTable.eRefreshCycle.REFRESH_CYCLE_DAILY:
                    if (shopTable.Refresh == 1)
                        return EItemLimitType.Mystery;
                    return EItemLimitType.Daily;
                case ShopTable.eRefreshCycle.REFRESH_CYCLE_WEEK:
                    return EItemLimitType.Weekly;
                case ShopTable.eRefreshCycle.REFRESH_CYCLE_MONTH:
                    return EItemLimitType.Monthly;
                case ShopTable.eRefreshCycle.REFRESH_CYCLE_ACTIVITY:
                    return EItemLimitType.Activityly;
            }
        }
        return EItemLimitType.None;
    }

    //获取账号商店道具的限时类型
    public static EItemLimitType GetAccountShopItemLimitType(AccountShopItemInfo info)
    {
        switch ((AccountShopRefreshType)info.roleRefreshType)
            {
                case AccountShopRefreshType.EachDay:
                    return EItemLimitType.Daily;
                case AccountShopRefreshType.EachWeekend:
                    return EItemLimitType.Weekly;
                case AccountShopRefreshType.EachMonth:
                    return EItemLimitType.Monthly;
                case AccountShopRefreshType.EachSeason:
                    return EItemLimitType.Season;
                default:
                    return EItemLimitType.None;
            }
    }

    /// <summary>
    /// 获取第一个有效的<VIP等级, 对应值>
    /// </summary>
    public static KeyValuePair<int, float> GetFirstValidVipLevelPrivilegeData(VipPrivilegeTable.eType PrivilegeType)
    {
        VipPrivilegeTable VipPrivilegeData = TableManager.GetInstance().GetTableItem<VipPrivilegeTable>((int)PrivilegeType);
        var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_VIPLEVEL_MAX);

        KeyValuePair<int, float> kvp = new KeyValuePair<int, float>(-1, 0.0f);

        if (null != VipPrivilegeData && SystemValueTableData != null)
        {
            for (int i = 1; i <= SystemValueTableData.Value; ++i)
            {
                PropertyInfo info = VipPrivilegeData.GetType().GetProperty(string.Format("VIP{0}", i), (BindingFlags.Instance | BindingFlags.Public));

                if (info != null)
                {
                    int iOriData = (int)info.GetValue(VipPrivilegeData, null);

                    if (iOriData > 0)
                    {
                        if (VipPrivilegeData.DataType == VipPrivilegeTable.eDataType.FLOAT)
                        {
                            kvp = new KeyValuePair<int, float>(i, iOriData / 1000.0f);
                        }
                        else
                        {
                            kvp = new KeyValuePair<int, float>(i, iOriData);
                        }

                        break;
                    }
                }
            }
        }

        return kvp;
    }

    public static float GetCurVipLevelPrivilegeData(VipPrivilegeTable.eType PrivilegeType)
    {
        VipPrivilegeTable VipPrivilegeData = TableManager.GetInstance().GetTableItem<VipPrivilegeTable>((int)PrivilegeType);

        if (VipPrivilegeData == null)
        {
            return 0.0f;
        }

        var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_VIPLEVEL_MAX);

        if (PlayerBaseData.GetInstance().VipLevel > SystemValueTableData.Value)
        {
            return 0.0f;
        }

        PropertyInfo info = VipPrivilegeData.GetType().GetProperty(string.Format("VIP{0}", PlayerBaseData.GetInstance().VipLevel), (BindingFlags.Instance | BindingFlags.Public));
        if (info == null)
        {
            return 0.0f;
        }

        int iOriData = (int)info.GetValue(VipPrivilegeData, null);

        if (iOriData <= 0)
        {
            return 0.0f;
        }

        if (VipPrivilegeData.DataType == VipPrivilegeTable.eDataType.FLOAT)
        {
            return (iOriData / 1000.0f);
        }
        else
        {
            return iOriData;
        }
    }

    public static float GetCurVipLevelPrivilegeData(int iID)
    {
        VipPrivilegeTable VipPrivilegeData = TableManager.GetInstance().GetTableItem<VipPrivilegeTable>(iID);

        if (VipPrivilegeData == null)
        {
            return 0.0f;
        }

        var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_VIPLEVEL_MAX);

        if (PlayerBaseData.GetInstance().VipLevel > SystemValueTableData.Value)
        {
            return 0.0f;
        }

        PropertyInfo info = VipPrivilegeData.GetType().GetProperty(string.Format("VIP{0}", PlayerBaseData.GetInstance().VipLevel), (BindingFlags.Instance | BindingFlags.Public));
        if (info == null)
        {
            return 0.0f;
        }

        int iOriData = (int)info.GetValue(VipPrivilegeData, null);

        if (iOriData <= 0)
        {
            return 0.0f;
        }

        if (VipPrivilegeData.DataType == VipPrivilegeTable.eDataType.FLOAT)
        {
            return (iOriData / 1000.0f);
        }
        else
        {
            return iOriData;
        }
    }

    public static List<string> GetPrivilegeDescListByVipLevel(int VipLevel)
    {
        List<string> PrivilegeDescList = new List<string>();

        var SystemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_VIPLEVEL_MAX);

        if (VipLevel > SystemValueTableData.Value)
        {
            return PrivilegeDescList;
        }

        var VipPrivilegeTableData = TableManager.GetInstance().GetTable<VipPrivilegeTable>();
        var enumerator = VipPrivilegeTableData.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var Item = enumerator.Current.Value as VipPrivilegeTable;

            PropertyInfo info = Item.GetType().GetProperty(string.Format("VIP{0}", VipLevel), (BindingFlags.Instance | BindingFlags.Public));
            if (info == null)
            {
                continue;
            }

            int iOriData = (int)info.GetValue(Item, null);

            if (iOriData <= 0)
            {
                continue;
            }

            if (VipLevel > 0)
            {
                if (Item.Type == VipPrivilegeTable.eType.PK_MONEY_LIMIT)
                {
                    SystemValueTable valuedata = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PKCOIN_MAX);
                    if (valuedata != null)
                    {
                        iOriData += valuedata.Value;
                    }
                }
                else if (Item.Type == VipPrivilegeTable.eType.MYSTERIOUS_SHOP_REFRESH_NUM)
                {
                    SystemValueTable valuedata = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_SHOP_REFRESH);
                    if (valuedata != null)
                    {
                        iOriData += valuedata.Value;
                    }
                }
            }

            if (Item.DataType == VipPrivilegeTable.eDataType.FLOAT)
            {
                PrivilegeDescList.Add(string.Format(Item.Description, iOriData / 10.0f) + "%");
            }
            else
            {
                PrivilegeDescList.Add(string.Format(Item.Description, iOriData));
            }
        }

        return PrivilegeDescList;
    }

    public static ChangeJobState GetChangeJobState()
    {
        //         if(CheckCanChangeJob())
        //         {
        //             return ChangeJobState.CanChangeJob;
        //         }
        // 
        //         if(CheckShowChangeJobTaskBtn())
        //         {
        //             return ChangeJobState.DoChangeJobTask;
        //         }
        // 
        //         if(CheckCanAwake())
        //         {
        //             return ChangeJobState.CanAwake;
        //         }
        // 
        //         if(CheckShowAwakeTaskBtn())
        //         {
        //             return ChangeJobState.DoAwakeJobTask;
        //         }

        return ChangeJobState.JobState_Null;
    }

    public static bool IsFunctionOpen(string ButtonPath)
    {
        var FunctionUnLockData = TableManager.GetInstance().GetTable<FunctionUnLock>();
        var enumerator = FunctionUnLockData.GetEnumerator();

        bool bOpen = false;
        bool bFindBtn = false;

        while (enumerator.MoveNext())
        {
            FunctionUnLock FunctionUnLockItem = enumerator.Current.Value as FunctionUnLock;

            if (FunctionUnLockItem.TargetBtnPos == "" || FunctionUnLockItem.TargetBtnPos == "-")
            {
                continue;
            }

            if (FunctionUnLockItem.TargetBtnPos != ButtonPath)
            {
                continue;
            }

            bFindBtn = true;

            if (PlayerBaseData.GetInstance().Level >= FunctionUnLockItem.FinishLevel)
            {
                bOpen = true;
            }

            break;
        }

        if (!bFindBtn)
        {
            bOpen = true;
        }

        return bOpen;
    }

    public static string GetStringByFloat(float fdata)
    {
        string sdata = "";

        int iParam = (int)fdata;
        int iParam2 = (int)(fdata * 10);

        float fParam = fdata * 100;

        if (Math.Abs(fdata - iParam) < 0.01)
        {
            sdata = fdata.ToString("f0");
        }
        else if (Math.Abs(fParam - iParam2 * 10) <= 10)
        {
            sdata = fdata.ToString("f1");
        }
        else if (fParam > iParam * 100)
        {
            sdata = fdata.ToString("f2");
        }

        return sdata;
    }

    // 这个函数只是用来判断某个功能是否可以解锁，至于是否真的解锁是由PlayerBaseData.GetInstance().UnlockFuncList来决定
    public static bool IsFunctionCanUnlock(FunctionUnLock.eFuncType funcType)
    {
        // dd: 临时代码，去掉PK
        if (!SwitchFunctionUtility.IsPKOpen && funcType == FunctionUnLock.eFuncType.Duel)
        {
            return false;
        }

        var FuncUnlockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)funcType);

        if (FuncUnlockData == null)
        {
            //Logger.LogErrorFormat("功能解锁表找不到{0}的数据", funcType);
            return true;
        }

        return (PlayerBaseData.GetInstance().Level >= FuncUnlockData.FinishLevel);
    }

    // 这个函数只是用来判断某个功能是否可以解锁，至于是否真的解锁是由PlayerBaseData.GetInstance().UnlockFuncList来决定
    public static bool IsFunctionCanUnlock(FunctionUnLock.eFuncType funcType,int iCurLv)
    {
        var FuncUnlockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)funcType);

        if (FuncUnlockData == null)
        {
            //Logger.LogErrorFormat("功能解锁表找不到{0}的数据", funcType);
            return true;
        }

        return (iCurLv >= FuncUnlockData.FinishLevel);
    }

    public static int GetFunctionUnlockLevel(FunctionUnLock.eFuncType funcType)
    {
        var FuncUnlockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)funcType);

        if (FuncUnlockData == null)
        {
            //Logger.LogErrorFormat("功能解锁表找不到{0}的数据", funcType);
            return 1;
        }

        return FuncUnlockData.FinishLevel;
    }

    public enum StrengthOperateResult
    {
        SOR_HAS_NO_ITEM = 0,         //无道具
        SOR_UNCOLOR,                //无色晶体不足
        SOR_COLOR,                 //有色晶体不足
        SOR_GOLD,                 //金币不足
        SOR_OK,                  //可以强化（增幅）
        SOR_COST_ITEM_NOT_EXIST, //找不到消耗品
        SOR_HAS_NO_PROTECTED,   //没有强化保护券
        SOR_Paradoxicalcrystal, //矛盾的结晶体
        SOR_Has_No_GrowthProtectd, //没有增幅保护券
        SOR_Strengthen_Implement,  //没有一次性强化器（一次性激化券）
    }

    public static StrengthOperateResult CheckGrowthItem(ItemData data, bool bUseProtected = false)
    {
        StrengthOperateResult eStrengthOperateResult = StrengthOperateResult.SOR_HAS_NO_ITEM;
        if (data == null)
        {
            return eStrengthOperateResult;
        }

        var materialList = EquipGrowthDataManager.GetInstance().GetGrowthCosts(data);
        if (materialList == null || materialList.Count <= 0)
        {
            eStrengthOperateResult = StrengthOperateResult.SOR_COST_ITEM_NOT_EXIST;
        }

        eStrengthOperateResult = StrengthOperateResult.SOR_OK;
        for (int i = 0; i < materialList.Count; i++)
        {
            var item = materialList[i];
            int iHasCount = 0;
            int iNeedCount = 0;
            ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(item.ItemID);
            if (itemTable == null)
            {
                continue;
            }

            iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(item.ItemID);
            iNeedCount = item.Count;

            if (itemTable.SubType == ItemTable.eSubType.BindGOLD)
            {
                if (iNeedCount > iHasCount)
                {
                    eStrengthOperateResult = StrengthOperateResult.SOR_GOLD;
                    break;
                }
            }
            else if (itemTable.ID == (int)ItemData.IncomeType.IT_COLOR)
            {
                if (iNeedCount > iHasCount)
                {
                    eStrengthOperateResult = StrengthOperateResult.SOR_COLOR;
                    break;
                }
            }
            else if (itemTable.SubType == ItemTable.eSubType.ST_ZENGFU_MATERIAL)
            {
                if (iNeedCount > iHasCount)
                {
                    eStrengthOperateResult = StrengthOperateResult.SOR_Paradoxicalcrystal;
                    break;
                }
            }
        }

        int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_GROWTHPROTECTED);
         if (bUseProtected && iProtectedNum <= 0 && data.StrengthenLevel >= 10)
        {
            eStrengthOperateResult = StrengthOperateResult.SOR_HAS_NO_PROTECTED;
        }

        return eStrengthOperateResult;
    }

    public static StrengthOperateResult CheckStrengthenItem(ItemData data, bool bUseProtected = false)
    {
        StrengthOperateResult eStrengthOperateResult = StrengthOperateResult.SOR_HAS_NO_ITEM;
        if (data == null)
        {
            return eStrengthOperateResult;
        }
        int iHasCount0 = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_UNCOLOR);
        int iHasCount1 = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_COLOR);
        int iHasCount2 = ItemDataManager.GetInstance().GetOwnedItemCount(ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindGOLD));
        int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_PROTECTED);

        StrengthenCost curCost = new StrengthenCost();
        if (!StrengthenDataManager.GetInstance().GetCost(data.StrengthenLevel, data.LevelLimit, data.Quality, ref curCost))
        {
            eStrengthOperateResult = StrengthOperateResult.SOR_COST_ITEM_NOT_EXIST;
        }

        if (data.SubType == (int)ProtoTable.ItemTable.eSubType.WEAPON)
        {
            float fRadio = 1.0f;
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_WP_COST_MOD);
            if (SystemValueTableData != null)
            {
                fRadio = SystemValueTableData.Value / 10.0f;
            }

            curCost.ColorCost = Utility.ceil(curCost.ColorCost * fRadio);
            curCost.UnColorCost = Utility.ceil(curCost.UnColorCost * fRadio);
            curCost.GoldCost = Utility.ceil(curCost.GoldCost * fRadio);
        }
        else if (data.SubType >= (int)ProtoTable.ItemTable.eSubType.HEAD && data.SubType <= (int)ProtoTable.ItemTable.eSubType.BOOT)
        {
            float fRadio = 1.0f;
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_DF_COST_MOD);
            if (SystemValueTableData != null)
            {
                fRadio = SystemValueTableData.Value / 10.0f;
            }

            curCost.ColorCost = Utility.ceil(curCost.ColorCost * fRadio);
            curCost.UnColorCost = Utility.ceil(curCost.UnColorCost * fRadio);
            curCost.GoldCost = Utility.ceil(curCost.GoldCost * fRadio);
        }
        else if (data.SubType >= (int)ProtoTable.ItemTable.eSubType.RING && data.SubType <= (int)ProtoTable.ItemTable.eSubType.BRACELET)
        {
            float fRadio = 1.0f;
            var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_JW_COST_MOD);
            if (SystemValueTableData != null)
            {
                fRadio = SystemValueTableData.Value / 10.0f;
            }

            curCost.ColorCost = Utility.ceil(curCost.ColorCost * fRadio);
            curCost.UnColorCost = Utility.ceil(curCost.UnColorCost * fRadio);
            curCost.GoldCost = Utility.ceil(curCost.GoldCost * fRadio);
        }

        eStrengthOperateResult = StrengthOperateResult.SOR_OK;
        if (curCost.UnColorCost > iHasCount0)
        {
            eStrengthOperateResult = StrengthOperateResult.SOR_UNCOLOR;
        }
        else if (curCost.ColorCost > iHasCount1)
        {
            eStrengthOperateResult = StrengthOperateResult.SOR_COLOR;
        }
        else if (curCost.GoldCost > iHasCount2)
        {
            eStrengthOperateResult = StrengthOperateResult.SOR_GOLD;
        }
        else if (bUseProtected && iProtectedNum <= 0 && data.StrengthenLevel >= 10)
        {
            eStrengthOperateResult = StrengthOperateResult.SOR_HAS_NO_PROTECTED;
        }

        return eStrengthOperateResult;
    }
    public static void OnPopupStrengthenResultMsg(StrengthOperateResult eStrengthOperateResult)
    {
        if (eStrengthOperateResult != StrengthOperateResult.SOR_OK)
        {
            switch (eStrengthOperateResult)
            {
                case StrengthOperateResult.SOR_UNCOLOR:
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_uncolor_not_enough"));
                    }
                    break;
                case StrengthOperateResult.SOR_COLOR:
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_color_not_enough"));
                    }
                    break;
                case StrengthOperateResult.SOR_GOLD:
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_gold_not_enough"));
                    }
                    break;
                case StrengthOperateResult.SOR_HAS_NO_ITEM:
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_item_not_enough"));
                    }
                    break;
                case StrengthOperateResult.SOR_COST_ITEM_NOT_EXIST:
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_cost_tablie_item_not_enough"));
                    }
                    break;
                case StrengthOperateResult.SOR_HAS_NO_PROTECTED:
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_has_not_protected"));
                    }
                    break;
                case StrengthOperateResult.SOR_Paradoxicalcrystal:
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("growth_paradoxicalcrystal")); 
                    }
                    break;
                case StrengthOperateResult.SOR_Has_No_GrowthProtectd:
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("growth_has_not_protected")); 
                    }
                    break;
            }
            return;
        }
    }

    public static string GetValueByTableName(float fValue, string propname)
    {
        if (propname == "maxHp" || propname == "maxMp" || propname == "defence" || propname == "magicDefence")
        {
            return GetStringByFloat(fValue);
        }
        else if (propname == "attackSpeed" || propname == "dex" || propname == "spellSpeed" || propname == "dodge")
        {
            return GetStringByFloat(fValue / 10f) + "%";
        }
        else
        {
            return GetStringByFloat(fValue / 1000f);
        }
    }

    public static void SwitchToPkWaitingRoom(PkRoomType ePkRoomType = PkRoomType.TraditionPk)
    {
        // dd: 临时代码，去掉PK
        if (!SwitchFunctionUtility.IsPKOpen)
        {
            return;
        }

        ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
        if (systemTown == null)
        {
            return;
        }

        CitySceneTable TownTableData = TableManager.instance.GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
        if (TownTableData == null)
        {
            return;
        }

        if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
        {
            ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
            Townframe.SetForbidFadeIn(true);
        }

        //if (ClientSystemManager.GetInstance().IsFrameOpen<FunctionFrame>())
        //{
        //    ClientSystemManager.GetInstance().CloseFrame<FunctionFrame>(null, true);
        //}

        int iSceneId = TownTableData.TraditionSceneID;

        if (ePkRoomType == PkRoomType.BudoPk)
        {
            iSceneId = TownTableData.BudoSceneID;
        }

        GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(
            new SceneParams
            {
                currSceneID = systemTown.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = iSceneId,
                targetDoorID = 0,
            }));
    }

    public static void SwitchToChiJiRoom()
    {
        ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
        if(systemTown != null)
        {
            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(
                new SceneParams
                {
                    currSceneID = systemTown.CurrentSceneID,
                    currDoorID = 0,
                    targetSceneID = 10100,
                    targetDoorID = 0,
                }));
        }
    }

    public static void SwitchToChijiWaittingRoom()
    {
        ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
        if (systemTown == null)
        {
            return;
        }

        ClientSystemManager.GetInstance().SwitchSystem<ClientSystemGameBattle>();
    }

    /// <summary>
    /// 获得屏幕矩形区域
    /// </summary>
    /// <returns></returns>
    public static Rect GetScreenRect()
    {
        var rect = new Rect(-Screen.width / 2,
            -Screen.height / 2,
            Screen.width,
            Screen.height);

        GameObject ui2DRoot = GameObject.Find("UIRoot/UI2DRoot");
        if (ui2DRoot != null)
        {
            CanvasScaler canvasScaler = ui2DRoot.GetComponent<CanvasScaler>();
            if (canvasScaler != null)
            {
                float scale = canvasScaler.matchWidthOrHeight == 1 ?
            canvasScaler.referenceResolution.y / (float)Screen.height :
            canvasScaler.referenceResolution.x / (float)Screen.width;

                rect = new Rect(rect.x * scale,
                                rect.y * scale,
                                rect.width * scale,
                                rect.height * scale);
            }
        }

        return rect;
    }

    public static void AddUICanvasCom(GameObject obj, int sortingOrder = 1, int layer = 5, bool r = false)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = layer;

        Canvas cam = obj.GetComponent<Canvas>();
        if (null == cam)
        {
            cam = obj.AddComponent<Canvas>();
            cam.overrideSorting = true;
            cam.sortingOrder = sortingOrder;

        }
        else
        {
            cam.overrideSorting = true;
            cam.sortingOrder = sortingOrder;
        }

        if (r)
        {
            for (int i = 0; i < obj.transform.childCount; ++i)
            {
                AddUICanvasCom(obj.transform.GetChild(i).gameObject, sortingOrder, layer, r);
            }
        }
    }

    public static bool SetUIArea(RectTransform target, Rect area, Transform canvas)
    {
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas, target);

        if (null == area)
        {
            return false;
        }

        Vector2 delta = default(Vector2);
        if (bounds.center.x - bounds.extents.x < area.x)//target超出area的左边框
        {
            delta.x += Mathf.Abs(bounds.center.x - bounds.extents.x - area.x);
        }
        else if (bounds.center.x + bounds.extents.x > area.width / 2)//target超出area的右边框
        {
            delta.x -= Mathf.Abs(bounds.center.x + bounds.extents.x - area.width / 2);
        }

        if (bounds.center.y - bounds.extents.y < area.y)//target超出area上边框
        {
            delta.y += Mathf.Abs(bounds.center.y - bounds.extents.y - area.y);
        }
        else if (bounds.center.y + bounds.extents.y > area.height / 2)//target超出area的下边框
        {
            delta.y -= Mathf.Abs(bounds.center.y + bounds.extents.y - area.height / 2);
        }

        //加上偏移位置算出在屏幕内的坐标
        target.anchoredPosition += delta;

        return delta != default(Vector2);
    }

    public static UInt64 GetRoleTotalExp()
    {
        Dictionary<int, object> ExpInfo = TableManager.GetInstance().GetTable<ExpTable>();

        UInt64 iExp = 0;
        foreach (var expdata in ExpInfo)
        {
            ExpTable data = expdata.Value as ExpTable;

            if (data.ID < PlayerBaseData.GetInstance().Level)
            {
                iExp += (ulong)data.TotalExp;
            }
            else
            {
                break;
            }
        }

        iExp += PlayerBaseData.GetInstance().CurExp;

        return iExp;
    }

    public static void SetInitTownSystemState()
    {
        if (!PlayerBaseData.GetInstance().bInitializeTownSystem)
        {
            PlayerBaseData.GetInstance().bInitializeTownSystem = true;
            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.InitializeTownSystem);
        }
    }

    public static bool IsShowDailyProveRedPoint()
    {
        var missionList = MissionManager.GetInstance().taskGroup.Values.ToList();

        for (int i = 0; i < missionList.Count; ++i)
        {
            MissionTable missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)missionList[i].taskID);

            if (missionItem == null || missionItem.TaskType != MissionTable.eTaskType.TT_DIALY || missionItem.SubType != MissionTable.eSubType.Daily_Prove)
            {
                continue;
            }

            if (missionList[i].status >= (int)TaskStatus.TASK_FINISHED && missionList[i].status < (int)TaskStatus.TASK_OVER)
            {
                return true;
            }
        }

        return false;
    }

    // 同一种物品ID的多个格子上的个数总和
    public static int GetItemNumByTableID(int iItemID)
    {
        int iNum = 0;

        Dictionary<ulong, ItemData> AllItems = ItemDataManager.GetInstance().GetAllPackageItems();
        if (AllItems == null)
        {
            return iNum;
        }

        foreach (var key in AllItems.Keys)
        {
            ItemData kItem = ItemDataManager.GetInstance().GetItem(key);

            if (kItem == null)
            {
                continue;
            }

            if (kItem.TableID != iItemID)
            {
                continue;
            }

            iNum += kItem.Count;
        }

        return iNum;
    }

    public static void SwitchSelectTextColor(Text text, Color clo, bool bIsSelect = true)
    {
        if (bIsSelect)
        {
            text.color = clo;

            Outline com = text.GetComponent<Outline>();
            if (com != null)
            {
                com.enabled = false;
            }
        }
        else
        {
            text.color = clo;

            Outline com = text.GetComponent<Outline>();
            if (com != null)
            {
                com.enabled = true;
            }
        }
    }

    private static ProtoTable.QuickBuyTable _getQuickBuyItemTable(ProtoTable.ItemTable.eSubType type)
    {
        int id = GameClient.ItemDataManager.GetInstance().GetMoneyIDByType(type);
        ProtoTable.QuickBuyTable quicktable = TableManager.instance.GetTableItem<ProtoTable.QuickBuyTable>(id);

        if (null == quicktable)
        {
            Logger.LogErrorFormat(" {0} 不是货币类型", type);
        }

        return quicktable;
    }

    /// <summary>
    /// 获取 快速购买 type 类型物品所需要的物品数量
    /// </summary>
    public static int GetQuickBuyCostCount(ProtoTable.ItemTable.eSubType type)
    {
        ProtoTable.QuickBuyTable quicktable = _getQuickBuyItemTable(type);

        if (null != quicktable)
        {
            return quicktable.CostNum;
        }

        return -1;
    }


    /// <summary>
    /// 获取 快速购买 type 类型物品所需要的物品ID
    /// </summary>
    public static int GetQuickBuyCostItemID(ProtoTable.ItemTable.eSubType type)
    {
        ProtoTable.QuickBuyTable quicktable = _getQuickBuyItemTable(type);

        if (null != quicktable)
        {
            return quicktable.CostItemID;
        }

        return -1;
    }


    /// <summary>
    /// 获取 能否快速购买 type 类型物品
    /// </summary>
    public static bool CanQuickBuyItem(ProtoTable.ItemTable.eSubType type)
    {
        ProtoTable.QuickBuyTable quicktable = _getQuickBuyItemTable(type);
        if (null == quicktable)
        {
            return false;
        }

        ProtoTable.ItemTable itemTable = TableManager.instance.GetTableItem<ProtoTable.ItemTable>(quicktable.CostItemID);

        if (null == itemTable)
        {
            return false;
        }

        int costCount = quicktable.CostNum;

        switch (itemTable.SubType)
        {
            case ProtoTable.ItemTable.eSubType.POINT:
            case ProtoTable.ItemTable.eSubType.BindPOINT:
                {
                    return PlayerBaseData.GetInstance().CanUseTicket((ulong)costCount);
                }
                break;
            case ProtoTable.ItemTable.eSubType.GOLD:
            case ProtoTable.ItemTable.eSubType.BindGOLD:
                {
                    return PlayerBaseData.GetInstance().CanUseGold((ulong)costCount);
                }
                break;
            default:
                Logger.LogErrorFormat("货币类型 {0} 未实现快速购买工具函数", type);
                break;
        }

        return false;
    }

    /// <summary>
    /// 立即执行协程，用于改异步为同步执行
    /// </summary>
    public static void IterCoroutineImm(IEnumerator iter)
    {
        if (null != iter)
        {
            IEnumerator citer = null;

            while (iter.MoveNext())
            {
                citer = iter.Current as IEnumerator;
                if (null != citer)
                {
                    IterCoroutineImm(citer);
                }
            }
        }
    }

    public static string GetMissionIcon(ProtoTable.MissionTable.eTaskType eTaskType)
    {
        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN)
        {
            return "UI/Image/Packed/p_UI_Task.png:UI_Renwu_Tubiao_Zhuxian";
        }

        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_BRANCH)
        {
            return "UI/Image/Packed/p_UI_Task.png:UI_Renwu_Tubiao_Zhixian";
        }

        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT)
        {
            return "UI/Image/Packed/p_UI_Task.png:UI_Renwu_Tubiao_Renwu";
        }

        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_DIALY)
        {
            return "UI/Image/Packed/p_UI_Task.png:UI_Renwu_Tubiao_Renwu";
        }

        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_CYCLE)
        {
            return "UI/Image/Packed/p_UI_Task.png:UI_Renwu_Tubiao_Xunhuan";
        }

        if (eTaskType == MissionTable.eTaskType.TT_CHANGEJOB)
        {
            return "UI/Image/Packed/p_UI_Task.png:UI_Renwu_Tubiao_Zhuanzhi";
        }

        if (eTaskType == MissionTable.eTaskType.TT_TITLE)
        {
            return "UI/Image/Packed/p_UI_Task.png:UI_Renwu_Tubiao_Chenghao";
        }

        if (eTaskType == MissionTable.eTaskType.TT_AWAKEN)
        {
            return "UI/Image/Packed/p_UI_Task.png:UI_Renwu_Tubiao_Juexing";
        }

        return "UI/Image/Packed/p_UI_Task.png:UI_Renwu_Tubiao_Zhuxian";
    }
    
    public static string GetBattleMissionIcon(ProtoTable.MissionTable.eTaskType eTaskType)
    {
        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN)
        {
            return "UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Task_Icon_Zhuxian";
        }

        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_BRANCH)
        {
            return "UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Task_Icon_Zhixian";
        }

        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT)
        {
            return "UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Task_Icon_Renwu";
        }

        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_DIALY)
        {
            return "UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Task_Icon_Renwu";
        }

        if (eTaskType == ProtoTable.MissionTable.eTaskType.TT_CYCLE)
        {
            return "UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Task_Icon_Xunhuan";
        }

        if (eTaskType == MissionTable.eTaskType.TT_CHANGEJOB)
        {
            return "UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Task_Icon_Zhuanzhi";
        }

        if (eTaskType == MissionTable.eTaskType.TT_TITLE)
        {
            return "UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Task_Icon_Chenghao";
        }

        if (eTaskType == MissionTable.eTaskType.TT_AWAKEN)
        {
            return "UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Task_Icon_Juexing";
        }

        return "UI/Image/NewPacked/NewBattle/Battle_Pve.png:Battle_Pve_Task_Icon_Zhuxian";
    }

    public static bool IsPlayerLevelFull(int iLevel)
    {
        var functionItem = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PLAYER_MAX_LEVEL_LIMIT);
        if (functionItem != null)
        {
            if (functionItem.Value <= iLevel)
            {
                return true;
            }
        }
        return false;
    }

     public static MethodInfo GetMethodInfoFromString(string methodStr)
    {
        string[] types = methodStr.Split(new char[] { '.' });
        if (types != null && types.Length >= 2)
        {
            string funcStr = types[types.Length - 1];
            string typeStr = "";
            for (int i = 0; i < types.Length - 1; i++)
            {
                if (i != 0)
                {
                    typeStr += ".";
                }
                typeStr += types[i];
            }
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            Type type = assembly.GetType(typeStr);
            MethodInfo mf = type.GetMethod(funcStr);
            return mf;
        }
        return null;
    }
    public static string GetEquipProCeilValueDesc(EEquipProp eEEquipProp, int attrValue)
    {
        string strValue = "";

        if ((eEEquipProp >= EEquipProp.Strenth && eEEquipProp <= EEquipProp.Stamina) || eEEquipProp == EEquipProp.Independence)
        {
            strValue = string.Format("+{0}", (float)attrValue / 1000);
        }
        else if (eEEquipProp == EEquipProp.AbormalResist || eEEquipProp >= EEquipProp.LightAttack && eEEquipProp <= EEquipProp.DarkDefence)
        {
            strValue = string.Format("+{0}", attrValue);
        }
        else if (eEEquipProp >= EEquipProp.AttackSpeedRate && eEEquipProp <= EEquipProp.MagicCritRate)
        {
            float va = attrValue * 1.0f / 10;
            string fk = string.Empty;

            if (va >= 0.0f)
            {
                fk = TR.Value("tip_rate_2_level_format_up", Utility.ConvertItemDataRateValue2IntLevel(va), va);
            }
            else
            {
                fk = TR.Value("tip_rate_2_level_format_down", Utility.ConvertItemDataRateValue2IntLevel(va), va);
            }

            strValue = string.Format("+{0}", fk);
        }
        else
        {
            strValue = string.Format("+{0}", attrValue);
        }
        return strValue;
    }

    public static string GetEEquipProDesc(EEquipProp eEEquipProp, int attrValue, string inner = "")
    {
        var curProp = Utility.GetEnumAttribute<EEquipProp, PropAttribute>(eEEquipProp);
        string strValue = curProp.desc;

        if ((eEEquipProp >= EEquipProp.Strenth && eEEquipProp <= EEquipProp.Stamina) || eEEquipProp == EEquipProp.Independence)
        {
            strValue = string.Format("{0}{2}+{1}", strValue, (float)attrValue / 1000, inner);
        }
        else if (eEEquipProp == EEquipProp.Elements)
        {
            strValue = string.Format("{0}{2}:{1}", strValue, Utility.GetEnumDescription((MagicElementType)attrValue), inner);
        }
        else if (eEEquipProp == EEquipProp.AbormalResist || eEEquipProp >= EEquipProp.LightAttack && eEEquipProp <= EEquipProp.DarkDefence)
        {
            strValue = string.Format("{0}{2}+{1}", strValue, attrValue, inner);
        }
        else if (eEEquipProp >= EEquipProp.AttackSpeedRate && eEEquipProp <= EEquipProp.MagicCritRate)
        {
            float va  = attrValue * 1.0f / 10;
            string fk = string.Empty;

            if (va >= 0.0f)
            {
                fk = TR.Value("tip_rate_2_level_format_up", Utility.ConvertItemDataRateValue2IntLevel(va), va);
            }
            else
            {
                fk = TR.Value("tip_rate_2_level_format_down", Utility.ConvertItemDataRateValue2IntLevel(va), va);
            }

            strValue = string.Format("{0}{1}+{2}", strValue, inner, fk);
        }
        else
        {
            strValue = string.Format("{0}{2}+{1}", strValue, attrValue, inner);
        }
        return strValue;
    }

    public static void OnItemClicked(GameObject obj, ItemData item)
    {
        if (null != item)
        {
            ItemTipManager.GetInstance().ShowTip(item);
        }
    }

    public static void OnItemClicked(GameObject obj, IItemDataModel item)
    {
        ItemTipManager.GetInstance().ShowTip(item as ItemData);
    }

    public static int GetSystemValueFromTable(SystemValueTable.eType eType)
    {
        var table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)eType);
        if(table != null)
        {
            return table.Value;
        }
        return 0;
    }
    public static int GetSystemValueFromTable(SystemValueTable.eType2 eType2)
    {
        var table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)eType2);
        if (table != null)
        {
            return table.Value;
        }
        return 0;
    }

    public static int GetSystemValueFromTable(SystemValueTable.eType3 eType3,int defaultValue = 0)
    {
        var table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)eType3);
        if (table != null)
        {
            return table.Value;
        }

        return defaultValue;
    }

    public static List<AwardItemData> ParseAwardItemDatas(string awardString,string tokenPrimary = "|",string tokenSecondary = "_")
    {
        if(string.IsNullOrEmpty(awardString))
        {
            return null;
        }
        List<AwardItemData> awardItemDatas = new List<AwardItemData>();
        if(awardItemDatas == null)
        {
            return null;
        }
        string[] rewards = awardString.Split(tokenPrimary.ToCharArray());
        if(rewards == null)
        {
            return awardItemDatas;
        }
        for(int i = 0;i < rewards.Length;i++)
        {
            string rewardItem = rewards[i];
            if(string.IsNullOrEmpty(rewardItem))
            {
                continue;
            }
            string[] item = rewardItem.Split(tokenSecondary.ToCharArray());
            if(item == null)
            {
                continue;
            }
            if (item.Length >= 2)
            {
                int id = int.Parse(item[0]);
                int iCount = int.Parse(item[1]);
                awardItemDatas.Add(new AwardItemData()
                {
                    ID = id,
                    Num = iCount,
                });
            }
        }        
        return awardItemDatas;
    }
    public static void PathfindingYiJieMap()
    {
        string mapPath = "<type=mapid value=3101000>";
        ActiveManager.GetInstance().OnClickLinkInfo(mapPath);

        if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>())
        {
            ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
        }
    }

    public static string GetHexColor(Color color)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>", (int)(255 * color.r), (int)(255 * color.g), (int)(255 * color.b), (int)(255 * color.a));
    }

    //     public static List<int> GetOpenTeamDungeonList()
    //     {
    //         List<int> TeamDungeonIdList = new List<int>();
    // 
    //         var DungeonTableData = TableManager.GetInstance().GetTable<DungeonTable>();
    //         var enumerator = DungeonTableData.GetEnumerator();
    // 
    //         while (enumerator.MoveNext())
    //         {
    //             var Item = enumerator.Current.Value as DungeonTable;
    // 
    //             if (Item.Type != DungeonTable.eType.L_NORMAL)
    //             {
    //                 continue;
    //             }
    // 
    //             if (Item.MinLevel > PlayerBaseData.GetInstance().Level)
    //             {
    //                 continue;
    //             }
    // 
    //             bool isOpen;
    //             int hard;
    // 
    //             if (!BattleDataManager.GetInstance().ChapterInfo.IsOpen(Item.ID, out isOpen, out hard))
    //             {
    //                 continue;
    //             }
    // 
    //             if (hard < (int)Item.Hard)
    //             {
    //                 continue;
    //             }
    // 
    //             TeamDungeonIdList.Add(Item.ID);
    //         }
    // 
    //         return TeamDungeonIdList;
    //     }

    public static void UseOneKeySuitWear(int suitId, int strength,int type = 0,int attr = 0)
    {
        OneKeyWearTable onKeyWearData = TableManager.instance.GetTableItem<OneKeyWearTable>(suitId);
        if (onKeyWearData != null && ChatManager.GetInstance() != null)
        {
            for (int i = 0; i < onKeyWearData.EquipList.Count; i++)
            {
                int equipId = TableManager.GetValueFromUnionCell(onKeyWearData.EquipList[i], 1);
                if (type > 0)
                {
                    ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!addee id={0} num={1} ql={2} str={3} et={4} ent={5}", equipId, 1, "100", strength, type, attr));
                }
                else
                {
                    ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1} str={2} ql={3}", equipId, 1, strength, "100"));
                }
            }

            for (int i = 0; i < onKeyWearData.FashionList.Count; i++)
            {
                int fashionId = TableManager.GetValueFromUnionCell(onKeyWearData.FashionList[i], 1);
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1} str={2} ql={3}", fashionId, 1, strength, "100"));
            }
        }
    }

    public static void ClearChild(GameObject go)
    {
        int childCount = go.transform.childCount;
        if (childCount <= 0)
            return;
        for(int i = 0; i < childCount; i++)
        {
            var child = go.transform.GetChild(i).gameObject;
            if (child != null)
                GameObject.Destroy(child);
        }
    }

    public static void ClearChild(GameObject parent, int index)
    {
        int childCount = parent.transform.childCount;
        if (childCount <= 0)
        {
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            if (i == index)
            {
                var child = parent.transform.GetChild(i).gameObject;

                if (child != null)
                {
                    GameObject.Destroy(child);
                }

                break;
            }
        }
    }

    /// <summary>
    /// 得到buff祈福活动特效路径
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string GetBuffPrayActivityEffectPath(int index)
    {
        string mEffectPath = "";
        switch (index)
        {
            case 1:
                mEffectPath = TR.Value("effUI_qianghuaquanhecheng_fuwen01");
                break;
            case 2:
                mEffectPath = TR.Value("effUI_qianghuaquanhecheng_fuwen02");
                break;
            case 3:
                mEffectPath = TR.Value("effUI_qianghuaquanhecheng_fuwen03");
                break;
            case 4:
                mEffectPath = TR.Value("effUI_qianghuaquanhecheng_fuwen04");
                break;
            case 5:
                mEffectPath = TR.Value("effUI_qianghuaquanhecheng_fuwen05");
                break;
            default:
                mEffectPath = TR.Value("effUI_qianghuaquanhecheng_fuwen01");
                break;
        }

        return mEffectPath;
    }

    /// <summary>
    /// 根据宠物蛋ID得到宠物ID
    /// </summary>
    /// <param name="petEggID"></param>
    /// <returns></returns>
    public static int GetPetID(int petEggID)
    {
        int mPetId = 0;
        var mPetDic = TableManager.GetInstance().GetTable<PetTable>().GetEnumerator();
        while (mPetDic.MoveNext())
        {
            var mPetTable = mPetDic.Current.Value as PetTable;
            if (mPetTable.PetEggID != petEggID)
            {
                continue;
            }

            mPetId = mPetTable.ID;
            return mPetId;
        }

        return mPetId;
    }

    //在模型身上指定节点挂载一个模型
    public static GameObject AddModelToActor(string resPath,GeActorEx geActor,GameObject attachNode)
    {
#if !LOGIC_SERVER
        var model = AssetLoader.instance.LoadResAsGameObject(resPath);
        AttachTo(model, attachNode);
        return model;
#else
        return null;
#endif
    }

    public static string Combine(string path1, string path2)
    {
        string combinedPath = System.IO.Path.Combine(path1, path2);
        return Normalize(combinedPath);
    }

    /// <summary>
    /// 获取标准化路径。
    /// </summary>
    /// <param name="path">要标准化的路径。</param>
    /// <returns>标准化后的路径。</returns>
    public static string Normalize(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        return path.Replace('\\', '/');
    }

    /// <summary>
    /// 上报点击某界面的按钮埋点
    /// </summary>
    /// <param name="sFrameName"></param>
    /// <param name="sButtonName"></param>
    public static void DoStartFrameOperation(string sFrameName, string sButtonName)
    {
        string sCurrentTime = Function.GetDateTime((int)TimeManager.GetInstance().GetServerTime());
        GameStatisticManager.GetInstance().DoStartFrameOperation(sFrameName, sButtonName, sCurrentTime);
    }

    public static float CalculateDifference(float selfValue,float otherValue)
    {
        return selfValue - otherValue;
    }

    public static void SetPersonalInfo(DisplayAttribute selfAttribute, DisplayAttribute otherAttribute,GameObject content)
    {
        for (int i = 0; i < (int)AttributeType.COUNT; ++i)
        {
            AttributeType at = (AttributeType)i;
            if (at == AttributeType.abnormalResist)
            {
                continue;
            }

            string childName = Global.GetAttributeString((AttributeType)i);

            GameObject goChild = Utility.FindGameObject(content, childName, false);
            if (goChild == null)
            {
                continue;
            }

            var selfValueGo = Utility.FindGameObject(goChild, "SelfValue");
            if (selfValueGo == null)
            {
                continue;
            }

            var otherValueGo = Utility.FindGameObject(goChild, "OtherValue");
            if (otherValueGo == null)
            {
                continue;
            }

            var differenceValueGo = Utility.FindGameObject(goChild, "DifferenceValue");
            if (differenceValueGo == null)
            {
                continue;
            }

            var selfFieldInfo = selfAttribute.GetType().GetField(childName);

            float selfValue = 0;
            if (selfFieldInfo != null)
            {
                selfValue = (float)selfFieldInfo.GetValue(selfAttribute);
            }

            var otherFieldInfo = otherAttribute.GetType().GetField(childName);

            float otherValue = 0;
            if (otherFieldInfo != null)
            {
                otherValue = (float)otherFieldInfo.GetValue(otherAttribute);
            }

            float differenceValue = CalculateDifference(selfValue,otherValue);
            
            Text selfValueText = selfValueGo.GetComponent<Text>();
            Text otherValueText = otherValueGo.GetComponent<Text>();
            Text differenceValueText = differenceValueGo.GetComponent<Text>();

            if (at == AttributeType.attackSpeed ||
            at == AttributeType.moveSpeed ||
            at == AttributeType.spellSpeed ||
            at == AttributeType.ciriticalAttack ||
            at == AttributeType.ciriticalMagicAttack ||
            at == AttributeType.dex ||
            at == AttributeType.dodge)
            {
                string format = "{0:F1}%";

                if (selfValue > 0 || otherValue > 0)
                {
                    format = "+" + format;
                }

                selfValueText.text = string.Format(format, selfValue);
                otherValueText.text = string.Format(format, otherValue);

                format = "{0:F1}%";
                string tr;
                if(differenceValue > 0)
                {
                    format = "+" + format;
                    tr = "ckxi_color_green";
                }
                else if(differenceValue == 0)
                {
                    tr = "ckxi_color_normal";
                }
                else
                {
                    tr = "ckxi_color_red";
                }

                differenceValueText.text = TR.Value(tr, string.Format(format, differenceValue));
            }
            else
            {
                selfValueText.text = string.Format("{0}", selfValue);
                otherValueText.text = string.Format("{0}", otherValue);

                string format = "{0}";
                string tr;
                if (differenceValue > 0)
                {
                    format = "+" + format;
                    tr = "ckxi_color_green";
                }
                else if (differenceValue == 0)
                {
                    tr = "ckxi_color_normal";
                }
                else
                {
                    tr = "ckxi_color_red";
                }

                differenceValueText.text = TR.Value(tr, string.Format(format, differenceValue));
            }
        }

        for (int i = 0; i < (int)AttributeType2.count; i++)
        {
            AttributeType2 att = (AttributeType2)i;
            string childName = Enum.GetName(typeof(AttributeType2), att);

            GameObject goChild = Utility.FindGameObject(content, childName, false);
            if (goChild == null)
            {
                continue;
            }

            var selfValueGo = Utility.FindGameObject(goChild, "SelfValue");
            if (selfValueGo == null)
            {
                continue;
            }

            var otherValueGo = Utility.FindGameObject(goChild, "OtherValue");
            if (otherValueGo == null)
            {
                continue;
            }

            var differenceValueGo = Utility.FindGameObject(goChild, "DifferenceValue");
            if (differenceValueGo == null)
            {
                continue;
            }

            var selfFieldInfo = selfAttribute.GetType().GetField(childName);

            float selfValue = 0;
            if (selfFieldInfo != null)
            {
                selfValue = (float)selfFieldInfo.GetValue(selfAttribute);
            }

            var otherFieldInfo = otherAttribute.GetType().GetField(childName);

            float otherValue = 0;
            if (otherFieldInfo != null)
            {
                otherValue = (float)otherFieldInfo.GetValue(otherAttribute);
            }

            float differenceValue = CalculateDifference(selfValue, otherValue);

            Text selfValueText = selfValueGo.GetComponent<Text>();
            Text otherValueText = otherValueGo.GetComponent<Text>();
            Text differenceValueText = differenceValueGo.GetComponent<Text>();

            selfValueText.text = string.Format("{0}", selfValue);
            otherValueText.text = string.Format("{0}", otherValue);

            string format = "{0}";
            string tr;
            if (differenceValue > 0)
            {
                format = "+" + format;
                tr = "ckxi_color_green";
            }
            else if (differenceValue == 0)
            {
                tr = "ckxi_color_normal";
            }
            else
            {
                tr = "ckxi_color_red";
            }

            differenceValueText.text = TR.Value(tr, string.Format(format, differenceValue));
        }
    }

    public static void SetPersonalInfo(DisplayAttribute attribute, GameObject objLeftRoot, GameObject objRightRoot)
    {
        for (int i = 0; i < (int)AttributeType.COUNT; ++i)
        {
            AttributeType at = (AttributeType)i;
            if (at == AttributeType.abnormalResist)
            {
                continue;
            }

            //string childName = Enum.GetName(typeof(AttributeType), (AttributeType)i);
            string childName = Global.GetAttributeString((AttributeType)i);

            GameObject goChild = Utility.FindGameObject(objLeftRoot, childName, false);
            if (goChild == null)
            {
                goChild = Utility.FindGameObject(objRightRoot, childName, false);
                if (goChild == null)
                {
                    continue;
                }
            }

            var tmp = Utility.FindGameObject(goChild, "Value");
            if (tmp == null)
            {
                continue;
            }

            var fieldInfo = attribute.GetType().GetField(childName);

            float value = 0;
            if (fieldInfo != null)
            {
                value = (float)fieldInfo.GetValue(attribute);
            }

            Text compText = tmp.GetComponent<Text>();
            compText.color = Color.white;

            if (attribute.attachValue.ContainsKey(childName))
            {
                if (attribute.attachValue[childName] > 0)
                {
                    compText.color = Color.green;
                }
                else if (attribute.attachValue[childName] < 0)
                {
                    compText.color = Color.red;
                }
            }

            if (at == AttributeType.attackSpeed ||
                at == AttributeType.moveSpeed ||
                at == AttributeType.spellSpeed ||
                at == AttributeType.ciriticalAttack ||
                at == AttributeType.ciriticalMagicAttack ||
                at == AttributeType.dex ||
                at == AttributeType.dodge)
            {
                string format = "{0:F1}%";

                if (value > 0)
                {
                    format = "+" + format;
                }

                compText.text = string.Format(format, value);
            }
            else
            {
                compText.text = string.Format("{0}", value);
            }
        }
    }

    public static ItemData _TryAddFashionItem(ulong guid)
    {
        ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
        if (itemData == null || itemData.Type != ProtoTable.ItemTable.eType.FASHION ||
            itemData.PackageType == EPackageType.Storage || itemData.PackageType == EPackageType.RoleStorage)
        {
            return null;
        }

        return itemData;
    }

    public static int _SortFashion(ItemData left, ItemData right)
    {
        if (left.FashionWearSlotType != right.FashionWearSlotType)
        {
            return left.FashionWearSlotType - right.FashionWearSlotType;
        }

        if (left.Quality != right.Quality)
        {
            return left.Quality - right.Quality;
        }

        return (int)(left.GUID - right.GUID);
    }

    public static ItemData _TryAddMagicCard(ulong guid)
    {
        ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
        if (itemData != null && itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE &&
            itemData.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard &&
            itemData.PackageType != EPackageType.Storage && itemData.PackageType != EPackageType.RoleStorage)
        {
            return itemData;
        }
        return null;
    }

    public static bool _CheckFashionCanMerge(ulong guid)
    {
        ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
        if (null == itemData)
        {
            return false;
        }

        if (itemData.bFashionItemLocked)
        {
            return false;
        }

        if (itemData.Type != ProtoTable.ItemTable.eType.FASHION)
        {
            return false;
        }

        if (itemData.PackageType != EPackageType.Fashion)
        {
            return false;
        }

        if (itemData.PackageType == EPackageType.Storage || itemData.PackageType == EPackageType.RoleStorage)
        {
            return false;
        }

        if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
        {
            var specialItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(itemData.TableID);
            if (specialItem != null)
            {
                if (specialItem.Type < (int)FashionType.FT_NATIONALDAY)
                {
                    return false;
                }
            }
        }
        else
        {
            var specialItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(itemData.TableID);
            if (specialItem != null)
            {
                return false;
            }
        }

        //wind is not allowed to merge
        if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.FASHION_HAIR)
        {
            return false;
        }

        //时装武器不能合成
        if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.FASHION_WEAPON)
        {
            return false;
        }

        if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.FASHION_AURAS)
        {
            return false;
        }

        if (itemData.Quality == ItemTable.eColor.PINK)
        {
            return false;
        }

        var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemData.TableID);
        if (null == itemTable || itemTable.TimeLeft > 0 && itemData.DeadTimestamp != 0)
        {
            return false;
        }

        return true;
    }
    public static int BinarySearch(IList<int> nums, int val)
    {
        int lo = 0;
        int hi = nums.Count - 1;
        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
            if (nums[mid] > val)
                hi = mid - 1;
            else if (nums[mid] < val)
                lo = mid + 1;
            else
                return mid;
        }
        return lo;
    }
    public static int GetClientIntValue(ClientConstValueTable.eKey type, int defaultValue = 0)
    {
        ClientConstValueTable table = TableManager.GetInstance().GetTableItem<ClientConstValueTable>((int)type);
        if (table == null || table.IntParams.Length == 0) return defaultValue;
        return table.IntParams[0];
    }

    public static int GetSystemIntValueByType1(SystemValueTable.eType type, int defaultValue = 0)
    {
        SystemValueTable table = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)type);
        if (table == null) return defaultValue;
        return table.Value;
    }

    /// <summary>
    /// 播放动画列表
    /// </summary>
    /// <param name="dOTweenAnimations"></param>
    public static void PlayAniList(List<DOTweenAnimation> dOTweenAnimations, bool value = true)
    {
        if (dOTweenAnimations == null)
        {
            return;
        }

        foreach (var aniItem in dOTweenAnimations)
        {
            if (aniItem == null)
            {
                continue;
            }

            if (value)
            {
                if (string.IsNullOrEmpty(aniItem.id))       //动画id为空时不能调用DORestartById这样会把所有id为空的动画都播放一遍了
                {
                    aniItem.DORestart();
                }
                else
                {
                    aniItem.DORestartById(aniItem.id);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(aniItem.id))
                {
                    aniItem.DORewind();
                }
                else
                {
                    aniItem.DORewindAllById(aniItem.id);
                }
            }
        }
    }

    /// <summary>
    /// 获取列表中delay+duration最长的动画
    /// </summary>
    /// <param name="dOTweenAnimations"></param>
    /// <returns></returns>
    public static DOTweenAnimation GetMostDurableAni(List<DOTweenAnimation> dOTweenAnimations)
    {
        if (dOTweenAnimations == null)
        {
            return null;
        }

        DOTweenAnimation dOTweenAnimation = null;
        foreach (var aniItem in dOTweenAnimations)
        {
            if (aniItem == null)
            {
                continue;
            }

            if (dOTweenAnimation == null)
            {
                dOTweenAnimation = aniItem;
            }
            else if (dOTweenAnimation.duration + dOTweenAnimation.delay < aniItem.delay + aniItem.duration)
            {
                dOTweenAnimation = aniItem;
            }
        }

        return dOTweenAnimation;
    }

    /// <summary>
    /// 将回调设置给列表中耗时最长的动画
    /// </summary>
    /// <param name="dOTweenAnimations"></param>
    /// <param name="complete"></param>
    public static void AddComplete(List<DOTweenAnimation> dOTweenAnimations, UnityEngine.Events.UnityAction complete)
    {
        DOTweenAnimation ani = GetMostDurableAni(dOTweenAnimations);
        if (ani != null)
        {
            ani.tween.onComplete = () =>
            {
                if (complete != null)
                {
                    complete();
                }
            };

            ani.CreateTween();
        }
    }

    /// <summary>
    /// 立即结束动画动画（没有播放过程）needReverse 是否需要回复原来的设置
    /// </summary>
    /// <param name="dOTweenAnimations"></param>
    /// <param name="needReverse"></param>
    public static void FinishAnis(List<DOTweenAnimation> dOTweenAnimations, bool needReverse = false)
    {
        if (dOTweenAnimations == null)
        {
            return;
        }

        foreach (var v in dOTweenAnimations)
        {
            if (v == null)
            {
                continue;
            }

            v.DOComplete();
        }
    }

    /// <summary>
    /// 编辑器状态下用来创建枚举下拉框的属性的接口（可以替换原来枚举里的名字）
    /// </summary>
    /// <param name="title"></param>
    /// <param name="selected"></param>
    /// <param name="strs"></param>
    /// <param name="isStrReplace"></param>
    /// <returns></returns>
    public static object EnumPopup(string title, Enum selected, List<string> strs, bool isStrReplace = true)
    {
#if UNITY_EDITOR
        int index = 1;
        var array = Enum.GetValues(selected.GetType());
        int length = 0;
        if (array == null)
        {
            return null;
        }

        length = array.Length;

        string[] enumString = new string[length];
        for (int i = 0; i < length; i++)
        {
            string name = array.GetValue(i).ToString();
            if (strs == null || strs.Count <= i)
            {
                enumString[i] = name;
                continue;
            }

            if (isStrReplace)
            {
                enumString[i] = strs[i];
            }
            else
            {
                enumString[i] = string.Format("{0}({1})", name, strs[i]);
            }
        }

        UnityEditor.EditorGUILayout.BeginHorizontal();
        UnityEditor.EditorGUILayout.PrefixLabel(title);
        index = UnityEditor.EditorGUILayout.Popup(selected.GetHashCode(), enumString);
        UnityEditor.EditorGUILayout.EndHorizontal();

        return Enum.ToObject(selected.GetType(), index);
#else
        return null;
#endif
    }
}
