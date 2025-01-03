using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

class ComLevelGiftEle : MonoBehaviour
{
    public GameObject IconRoot;
    public Image icon0;
    public Image icon1;
    public Image icon2;
    public GameObject TextRoot;
    public Text text0;
    public Text text1;
    public Text text2;
    public GameObject Accomplish;
    public GameObject Uncomplete;
    public Text UncompleteText;
    public Button Receive;
    public Text ReceiveText;
    public int ElementIndex;
    public UIGray ReceiveGray;
    public Text AccomplishText;
    public Text LevelLimit;
    public Text LevelTitle;
    
    private RectTransform rt;
    

    public RectTransform mrt
    {
        get { return rt; }
        set { rt = value; }
    }
}