using UnityEngine;
using UnityEngine.UI;

public class ComChangeColor : MonoBehaviour
{
    public Color NormalColor = new Color();
    public Color ClickColor = new Color();
    public Text text;

    [Space(10)]
    [Header("如果点击需要渐变效果，则把目标Text上的渐变组件添加上来")]
    [SerializeField]
    private GameClient.ComGradient comGradient;

    public void SetColor(bool b)
    {
        if (text != null)
        {
            text.color = b ? ClickColor : NormalColor;
        }

        if (comGradient != null)
        {
            comGradient.enabled = b;
        }
    }
}