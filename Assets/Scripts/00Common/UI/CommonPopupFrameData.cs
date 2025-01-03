using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 通用弹出式界面的配置数据
[RequireComponent(typeof(CommonPopupFrame))]
public class CommonPopupFrameData : MonoBehaviour
{
    [HeaderAttribute("界面标题")]
    public Text frameTitle = null;

    [HeaderAttribute("帮助按钮")]
    public GameObject frameHelp = null;

    [HeaderAttribute("关闭按钮")]
    public GameObject frameClose = null;


    [HeaderAttribute("各种大小的列表")]
    public List<Vector2> sizeList = new List<Vector2>()
    {
        new Vector2(1420,820),
        new Vector2(1300,740),
        new Vector2(1620,820),
        new Vector2(620,820),
        new Vector2(920,520),
        new Vector2(1550,820),
        new Vector2(1010,820),
    };    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }    
}


