using UnityEngine;
using UnityEngine.UI;

public class ComChangeFontSize : MonoBehaviour
{
    public int NormalSize;
    public int ClickSize;
    public Text text;

    public void SetFontSize(bool b)
    {
        if (text != null)
        {
            text.fontSize = b ? ClickSize : NormalSize;
        }  
    }
}