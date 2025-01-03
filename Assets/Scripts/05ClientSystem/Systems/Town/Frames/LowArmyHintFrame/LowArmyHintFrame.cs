using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace GameClient
{
    class LowArmyHintItemData
    {
        public ItemData itemData;
    }

    class LowArmyHintItem : CachedNormalObject<LowArmyHintItemData>
    {
        Text title;
        GameObject goLPrefab;
        GameObject goLParent;
        Regex m_regex = new Regex(@"([\+\-]*\d+\.*\d*%*$)");
        public override void Initialize()
        {
            title = Utility.FindComponent<Text>(goLocal, "Record/Title");
            goLParent = Utility.FindChild(goLocal, "Record/Attributes");
            goLPrefab = Utility.FindChild(goLocal, "Record/Attributes/Attribute");
            goLPrefab.CustomActive(false);
        }

        public override void UnInitialize()
        {
            title = null;
            goLParent = null;
            goLPrefab = null;
        }

        public override void OnUpdate()
        {
            if(null != Value && null != Value.itemData)
            {
                if(null != title)
                {
                    string strContent = string.Empty;
                    ItemData.QualityInfo qualityInfo = Value.itemData.GetQualityInfo();
                    if(null != qualityInfo)
                    {
                        if (Value.itemData.StrengthenLevel > 0)
                        {
                            strContent = TR.Value("super_link_item_name", Value.itemData.StrengthenLevel, Value.itemData.Name);
                        }
                        else
                        {
                            strContent = Value.itemData.Name;
                        }
                        title.text = string.Format("<color={0}>[{1}]</color>", qualityInfo.ColStr, strContent);
                    }
                }

                var attrs = Value.itemData.GetMasterAttrDescs(false);
                for(int i = 0; i < attrs.Count; ++i)
                {
                    GameObject goCurrent = GameObject.Instantiate(goLPrefab);
                    Utility.AttachTo(goCurrent, goLParent);
                    goCurrent.CustomActive(true);

                    Text value = goCurrent.GetComponent<Text>();
                    if(null != value)
                    {
                        var match = m_regex.Match(attrs[i]);
                        if(match.Success)
                        {
                            value.text = string.Format("{0}{1}{2}{3}",attrs[i].Substring(0,match.Index),"<color=#ff0000ff>",match.Groups[1].Value,"</color>");
                        }
                        else
                        {
                            value.text = attrs[i];
                        }
                    }
                }
            }
        }
    }

    class LowArmyHintFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LowArmyHint/LowArmyHintFrame";
        }

        [UIObject("Records/Scroll View/Viewport/Content")]
        GameObject goParent;

        [UIObject("Records/Scroll View/Viewport/Content/Prefab")]
        GameObject goPrefab;

        CachedObjectListManager<LowArmyHintItem> m_akLowArmyHintItems = new CachedObjectListManager<LowArmyHintItem>();

        protected override void _OnOpenFrame()
        {
            goPrefab.CustomActive(false);

            _AddButton("Close", () =>
             {
                 frameMgr.CloseFrame(this);
             });

            var equips = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            for (int i = 0; i < equips.Count; ++i)
            {
                var item = ItemDataManager.GetInstance().GetItem(equips[i]);
                if (null == item)
                {
                    continue;
                }

                int iEquipedMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)item.Quality, (int)item.EquipWearSlotType, (int)item.ThirdType);
                if (iEquipedMasterPriority == 2)
                {
                    m_akLowArmyHintItems.Create(new object[]
                    {
                        goParent,
                        goPrefab,
                        new LowArmyHintItemData
                        {
                            itemData = item
                        },
                        false
                    });
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            m_akLowArmyHintItems.DestroyAllObjects();
        }
    }
}