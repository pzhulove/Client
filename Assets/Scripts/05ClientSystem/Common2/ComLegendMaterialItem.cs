using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    public class ComLegendMaterialItemData
    {
        public ItemData itemData;
        public MissionManager.ParseObject parseObject;
    }

    class ComLegendMaterialItem : MonoBehaviour
    {
        public GameObject goItemParent;
        public Text process;
        public StateController comStateController;
        ComItem comItem;
        ComLegendMaterialItemData data = null;


        private int mGoldItemId = 600000001;
        public static string[] statusKeys = new string[]
        {
            "normal_not_enough",
            "gold_not_enough",
            "normal_enough",
            "gold_enough",
            "task_over",
        };

        public void SetItemData(ComLegendMaterialItemData data)
        {
            this.data = data;

            if(null == comItem)
            {
                if(null != goItemParent)
                comItem = ComItemManager.Create(goItemParent);
            }

            if(null != comItem)
            {
                comItem.Setup(data.itemData, (GameObject obj, ItemData item)=>
                {
                    ItemTipManager.GetInstance().ShowTip(item);
                });
            }

            var find = data.parseObject.tokens.Find(x => 
            {
                return x.eMaterialRegexType == MissionManager.MaterialRegexType.MRT_KEY_VALUE;
            });
            if(data.itemData.TableID==mGoldItemId)
            {
                process.text = data.parseObject.content;
            }
            else
            {
                int count = ItemDataManager.GetInstance().GetItemCountInPackage(data.itemData.TableID);
                string content = data.parseObject.content;
                string[] strArry = content.Split('/');
                int totalNum = 0;
                if (strArry != null && strArry.Length >= 2)
                {
                    int.TryParse(strArry[1], out totalNum);
                }
                process.text = string.Format("{0}/{1}", count, totalNum);
            }


            if (null != find)
            {
                int iPre = (int)find.param0;
                int iAft = (int)find.param1;
                bool bTaskOver = (bool)find.param2;
                iPre = IntMath.Min(iPre, iAft);

                bool bIncome = false;
                if(null != data && null != data.itemData)
                {
                    var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(data.itemData.TableID);
                    if(null != itemTable)
                    {
                        bIncome = itemTable.Type == ProtoTable.ItemTable.eType.INCOME;
                    }
                }

                if(null != comStateController)
                {
                    int mask = 0;
                    mask |= (bIncome ? (1 << 0) : 0);
                    mask |= (iPre >= iAft ? (1 << 1) : 0);

                    if (bTaskOver)
                    {
                        comStateController.Key = statusKeys[4];
                    }
                    else
                    {
                        comStateController.Key = statusKeys[mask];
                    }
                }
            }
        }

        void OnDestroy()
        {
            if(null != comItem)
            {
                ComItemManager.Destroy(comItem);
                comItem = null;
            }
            data = null;
        }
    }
}