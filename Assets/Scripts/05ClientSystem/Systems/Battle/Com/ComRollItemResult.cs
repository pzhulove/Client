using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
//单个roll的物品的结算界面
public class ComRollItemResult : MonoBehaviour {

    // Use this for initialization
    public GameObject[] playerRoot = new GameObject[3];
    public Text[] playerName = new Text[3];
    public Text[] score = new Text[3];
    public Image[] winBG = new Image[3];
    public Image[] hum = new Image[3];
    public Image[] roll = new Image[3];
    public Image[] win = new Image[3];
    public GameObject item = null;
    public Text itemName = null;
    private GameClient.ItemData curItemData = null;
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
    private void SetPlayerInfo(int index, Protocol.RollDropResultItem player)
    {
        if (index < 0 || index >= 3) return;
        if(player.opType == (byte)Protocol.RollOpTypeEnum.RIE_NEED)
        {
            if(hum[index] != null && hum[index].gameObject != null)
            {
                hum[index].gameObject.CustomActive(false);
            }
            if (roll[index] != null && roll[index].gameObject != null)
            {
                roll[index].gameObject.CustomActive(true);
            }
        }
        else if(player.opType == (byte)Protocol.RollOpTypeEnum.RIE_MODEST)
        {
            if (hum[index] != null && hum[index].gameObject != null)
            {
                hum[index].gameObject.CustomActive(true);
            }
            if (roll[index] != null && roll[index].gameObject != null)
            {
                roll[index].gameObject.CustomActive(false);
            }
        }
        else
        {
            if (hum[index] != null && hum[index].gameObject != null)
            {
                hum[index].gameObject.CustomActive(false);
            }
            if (roll[index] != null && roll[index].gameObject != null)
            {
                roll[index].gameObject.CustomActive(false);
            }
        }
        if(score[index] != null)
        {
            score[index].text = string.Format("{0}点", player.point);
        }
    }
    public void Init(GameClient.ItemData itemData, List<Protocol.RollDropResultItem> playerInfos)
    {
        curItemData = itemData;
        if (comItem == null)
        {
            comItem = GameClient.ComItemManager.Create(item);
        }

        if(curItemData != null && itemName != null)
        {
            var quality = curItemData.GetQualityInfo();
            if (quality != null)
            {
                itemName.color = quality.Col;
            }
            itemName.text = curItemData.Name;
        }

        if (comItem != null)
        {
            comItem.Setup(curItemData, OnItemClick);
       
        }
        int winIndex = 0;
        int maxScore = 0;
        for (int i = 0; i < playerRoot.Length; i++)
        {
            if (playerRoot[i] == null) continue;
            if (i < playerInfos.Count)
            {
                var curPlayer = playerInfos[i];
                if (curPlayer == null)
                {
                    if (playerRoot[i].gameObject != null)
                        playerRoot[i].gameObject.CustomActive(false);
                    continue;
                }
                else
                {
                    if (playerRoot[i].gameObject != null)
                        playerRoot[i].gameObject.CustomActive(true);
                }
                int opType = 0;
                if (curPlayer.opType <= 2 &&
                    curPlayer.opType >= 0)
                {
                    opType = (int)curPlayer.opType;
                }
                SetPlayerInfo(i, curPlayer);
                if (maxScore < (int)curPlayer.point)
                {
                    winIndex = i;
                    maxScore = (int)curPlayer.point;
                }
            }
            else
            {
                if (playerRoot[i].gameObject != null)
                    playerRoot[i].gameObject.CustomActive(false);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            Protocol.RollDropResultItem curPlayer = null;
            if(i < playerInfos.Count)
            {
                curPlayer = playerInfos[i];
            }
            if (curPlayer == null) continue;
            if (i == winIndex)
            {
                if (playerName[i] != null)
                {
                    playerName[i].text = string.Format("<color=#ffe23cf6>{0}</color>", curPlayer.playerName);
                }
                if (winBG[i] != null && winBG[i].gameObject != null)
                {
                    winBG[i].gameObject.CustomActive(true);
                }
                if(win[i] != null && win[i].gameObject != null)
                {
                    win[i].gameObject.CustomActive(true);
                }
            }
            else
            {
                if (playerName[i] != null)
                {
                    playerName[i].text = string.Format("<color=#ffffffff>{0}</color>", curPlayer.playerName);
                }
                if (winBG[i] != null && winBG[i].gameObject != null)
                {
                    winBG[i].gameObject.CustomActive(false);
                }
                if (win[i] != null && win[i].gameObject != null)
                {
                    win[i].gameObject.CustomActive(false);
                }
            } 
        }

    }
    private void OnItemClick(GameObject obj, GameClient.ItemData itemData)
    {
        if (itemData == null)
            return;

        GameClient.ItemTipManager.GetInstance().ShowTip(itemData);
    }

}
