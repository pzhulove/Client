using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemDrugSetting : MonoBehaviour {

    public Image background;
    public Image icon;
    public Text des;
    public GameObject item;
    public GameObject empty;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetItemInfo(int id,int percent, PlayerBaseData.PotionSlotType type)
    {
        ItemData itemdata = ItemDataManager.CreateItemDataFromTable(id);
        item.CustomActive(id != 0);
        empty.CustomActive(id == 0);

        if (itemdata == null)
        {
            return;
        }
        if (null != des)
            des.CustomActive(itemdata.SubType != 62);
        if (background != null)
        {
            ETCImageLoader.LoadSprite(ref background, itemdata.GetQualityInfo().Background);
        }

        if (icon != null)
        {
            ETCImageLoader.LoadSprite(ref icon, itemdata.Icon);
        }
        if (null != des)
        {
            var colorSet = des.GetComponent<ComSetTextColor>();
            if (percent <= 0)
            {
                des.text = TR.Value("potion_set_un_ues");
                colorSet.SetColor(0);
                // des.color = new Color(195.0f / 255, 195.0f / 255, 195.0f / 255, 1.0f);
            }
            else
            {
                des.text = TR.Value("potion_set_ues_rate", percent);//string.Format("HP低于{0}%自动使用", percent);
                colorSet.SetColor(1);
                // des.color = Color.green;
            }

            if (type == PlayerBaseData.PotionSlotType.SlotMain)
            {
                if (!PlayerBaseData.GetInstance().IsPotionSlotMainSwitchOn())
                {
                    des.text = TR.Value("potion_set_un_ues");
                    colorSet.SetColor(0);
                    // des.color = new Color(195.0f / 255, 195.0f / 255, 195.0f / 255, 1.0f);
                }
            }
        }
    }
      
}
