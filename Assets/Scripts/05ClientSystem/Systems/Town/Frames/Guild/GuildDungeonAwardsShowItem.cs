using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;

namespace GameClient
{
    public class GuildDungeonAwardsShowItem : MonoBehaviour
    {
        public enum AwardType
        {
            Kill3ShiTu,
            NotKillBigBoss,            
            TypeMax,
        }

        [SerializeField]
        GameObject payAwardItem = null;

        [SerializeField]
        GameObject itemsParent = null;

        [SerializeField]
        Text awardDesc = null;

        string[] txtDescs = new string[(int)AwardType.TypeMax]            
        {
           TR.Value("kill3ShiTu"),
           TR.Value("notKillBoss"),
        };

        // Use this for initialization
        void Start()
        {

        }

        private void OnDestroy()
        {
           
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Vector2 GetContentSize(List<AwardItemData> items)
        {
            Vector2 vec = new Vector2();
            if(items == null)
            {
                return vec;
            }

            if(awardDesc != null)
            {
                vec.y += awardDesc.gameObject.GetComponent<RectTransform>().rect.height;
            }

            if(itemsParent != null)
            {
                GridLayoutGroup grid = itemsParent.GetComponent<GridLayoutGroup>();
                if(grid != null)
                {
                    int iRow = 0;
                    if (items.Count % grid.constraintCount == 0)
                    {
                        iRow = (items.Count / grid.constraintCount);
                    }
                    else
                    {
                        iRow = items.Count / grid.constraintCount + 1;
                    }

                    vec.y += (grid.cellSize.y * iRow) + (iRow > 0 ? grid.spacing.y * (iRow - 1) : 0);
                }
            }

            vec.x = gameObject.GetComponent<RectTransform>().rect.width;
            Rect rect = gameObject.GetComponent<RectTransform>().rect;

            Vector2 vec2 = gameObject.GetComponent<RectTransform>().sizeDelta;

            return vec;
        }

        public void SetUp(ClientFrame frame, AwardType awardType,List<AwardItemData> items)
        {
            if(awardType >= AwardType.TypeMax || items == null)
            {
                return;
            }

            if(txtDescs == null)
            {
                return;
            }

            int iIndex = (int)awardType;
            if(iIndex < txtDescs.Length && iIndex >= 0)
            {
                if(awardDesc != null)
                {
                    awardDesc.text = txtDescs[iIndex];
                }       

                if (itemsParent != null && payAwardItem != null)
                {
                    for (int i = 0; i < itemsParent.transform.childCount; ++i)
                    {
                        GameObject go = itemsParent.transform.GetChild(i).gameObject;
                        GameObject.Destroy(go);
                    }

                    for (int i = 0; i < items.Count; i++)
                    {
                        ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(items[i].ID);
                        if (itemDetailData == null)
                        {
                            continue;
                        }

                        itemDetailData.Count = items[i].Num;

                        GameObject goCurrent = GameObject.Instantiate(payAwardItem.gameObject);
                        Utility.AttachTo(goCurrent, itemsParent);
                        goCurrent.CustomActive(true);

                        PayRewardItem payItem = goCurrent.GetComponent<PayRewardItem>();               
                        if (payItem != null)
                        {
                            payItem.Initialize(frame, itemDetailData, true, false);
                            payItem.RefreshView();
                        }
                    }
                }
            }
        }
    }
}


