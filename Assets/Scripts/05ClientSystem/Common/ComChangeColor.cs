using UnityEngine;
using UnityEngine.UI;

public class ComChangeColor : MonoBehaviour
{
    public Color NormalColor = new Color();
    public Color ClickColor = new Color();
    public Text text;

    [Space(10)]
    [Header("��������Ҫ����Ч�������Ŀ��Text�ϵĽ�������������")]
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