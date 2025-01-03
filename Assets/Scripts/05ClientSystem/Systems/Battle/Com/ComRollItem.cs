using UnityEngine;
using UnityEngine.UI;
//roll单个道具基本显示逻辑，显示道具tip 和时间轴，和按钮的显隐，显示roll点 以及谦让
public class ComRollItem : MonoBehaviour {

    public Text itemName = null;
    public Image humDesc = null;
    public Text scoreDesc = null;
    public GameObject scoreRoot = null;
    public GameObject item = null;
    public Slider timeProgress = null;
    public Button btnRoll = null;
    public Button btnHum = null;
    private GameClient.ItemData curItemData = null;
    private float curTime = 10.0f;
    private float totalTime = 10.0f;
    private GameClient.ComItem comItem = null;
	void Start () {
		
	}
    void OnDestroy()
    {
        if (comItem != null)
        {
            GameClient.ComItemManager.Destroy(comItem);
            comItem = null;
        }
    }

    public void Init(GameClient.ItemData itemData,float time,float totalTime,int score, GameClient.DungeonRollFrame.ROLLITEM_STAT stat)
    {
        curItemData = itemData;
        if(comItem == null)
        {
            comItem = GameClient.ComItemManager.Create(item);
        }
        if (comItem != null)
        {
            comItem.Setup(curItemData, OnItemClick);
        }
        curTime = time;
        this.totalTime = totalTime;
        if(curItemData != null && itemName != null)
        {
            var quality = curItemData.GetQualityInfo();
            if (quality != null)
            {
                itemName.color = quality.Col;
            }
            itemName.text = curItemData.Name;
        }

        if(stat == GameClient.DungeonRollFrame.ROLLITEM_STAT.HUM)
        {
            if (humDesc != null)
            {
                humDesc.CustomActive(true);
            }
            if (scoreRoot != null)
            {
                scoreRoot.CustomActive(false);
            }
            if (scoreDesc != null)
            {
                scoreDesc.CustomActive(false);
            }
            if(btnRoll != null && btnRoll.gameObject != null)
            {
                btnRoll.gameObject.CustomActive(false);
            }
            if (btnHum != null && btnHum.gameObject != null)
            {
                btnHum.gameObject.CustomActive(false);
            }
        }
        else if(stat == GameClient.DungeonRollFrame.ROLLITEM_STAT.SCORE)
        {
            if (humDesc != null)
            {
                humDesc.CustomActive(false);
            }
            if (scoreRoot != null)
            {
                scoreRoot.CustomActive(true);
            }
            if (scoreDesc != null)
            {
                scoreDesc.CustomActive(true);
                scoreDesc.text = score.ToString();
            }
            if (btnRoll != null && btnRoll.gameObject != null)
            {
                btnRoll.gameObject.CustomActive(false);
            }
            if (btnHum != null && btnHum.gameObject != null)
            {
                btnHum.gameObject.CustomActive(false);
            }
        }
        else
        {
            if (humDesc != null)
            {
                humDesc.CustomActive(false);
            }
            if (scoreRoot != null)
            {
                scoreRoot.CustomActive(false);
            }
            if (scoreDesc != null)
            {
                scoreDesc.CustomActive(false);
            }
            if (btnRoll != null && btnRoll.gameObject != null)
            {
                btnRoll.gameObject.CustomActive(true);
            }
            if (btnHum != null && btnHum.gameObject != null)
            {
                btnHum.gameObject.CustomActive(true);
            }
        }
       
        if (timeProgress != null)
        {
            if (totalTime != 0.0f)
            {
                timeProgress.value = curTime / totalTime;
            }
            else
            {
                timeProgress.value = 0.0f;
            }
        }
    }
    void Update()
    {
        if (curTime < 0.0f) return;
        if(curTime < 0.0f)
        {
            curTime = 0.0f;
        }
        if (timeProgress != null)
        {
            if(totalTime != 0.0f)
            { 
                timeProgress.value = curTime / totalTime;
            }
            else
            {
                timeProgress.value = 0.0f;
            }
        }
        curTime -= Time.deltaTime;
    }
    private void OnItemClick(GameObject obj, GameClient.ItemData itemData)
    {
        if (itemData == null)
            return;

        GameClient.ItemTipManager.GetInstance().ShowTip(itemData);
    }
    // Update is called once per frame

}
