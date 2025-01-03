using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Protocol;

namespace GameClient
{
    class ComSealEquipment : MonoBehaviour
    {
        public static ItemData ms_selected = null;
        public Text Name;
        public Text mGradeLevel;
        public GameObject equiptedMark;
        public GameObject goItemParent;
        public GameObject goCheckMark;
        public GameObject goInscriptionHoleRoot;
        public string mEquipmentInscriptionHolePath = "UIFlatten/Prefabs/SmithShop/InscriptionFrame/InscriptionHole";
        ComItemNew comItem;
        SmithShopNewTabType eFunctionType = SmithShopNewTabType.SSNTT_ADJUST;
        private List<GameObject> mGoInscriptionHoles = new List<GameObject>();
        public ItemData ItemData
        {
            get
            {
                return itemData;
            }
        }

        private ItemData itemData;

        private void Awake()
        {
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
        }

        public static bool CheckCanSeal(ItemData x)
        {
            return (x.iMaxPackTime > 0 && !x.Packing && x.RePackTime > 0 && !x.isInSidePack);
        }
        
        public void OnItemChangeDisplay(bool bSelected)
        {
            goCheckMark.CustomActive(bSelected);
        }
        
        public void OnItemVisible(ItemData itemData,SmithShopNewTabType eFunctionType,bool isShowIncriptionHolInfo = false)
        {
            if (itemData == null)
                return;

            this.itemData = itemData;

            this.eFunctionType = eFunctionType;
            if (comItem == null)
            {
                comItem = ComItemManager.CreateNew(goItemParent);
            }
            comItem.Setup(itemData, Utility.OnItemClicked);
            
            Name.text = itemData.GetColorName();

            RefreshGradeInfo();

            equiptedMark.CustomActive(itemData.PackageType == EPackageType.WearEquip);
            gameObject.name = itemData.TableID.ToString();
            
            if (isShowIncriptionHolInfo == true)
            {
                if (mGoInscriptionHoles != null && mGoInscriptionHoles.Count > 0)
                {
                    for (int i = 0; i < mGoInscriptionHoles.Count; i++)
                    {
                        mGoInscriptionHoles[i].CustomActive(false);
                    }
                }

                bool isShowInscriptionHoleInfo = false;
                
                if (itemData.InscriptionHoles != null)
                {
                    for (int i = 0; i < itemData.InscriptionHoles.Count; i++)
                    {
                        var inscriptionHoleData = itemData.InscriptionHoles[i];
                        if (inscriptionHoleData == null)
                        {
                            continue;
                        }

                        if (inscriptionHoleData.InscriptionId <= 0)
                        {
                            continue;
                        }

                        isShowInscriptionHoleInfo = true;

                        if (i < mGoInscriptionHoles.Count)
                        {
                            mGoInscriptionHoles[i].CustomActive(true);
                            EquipmentInscriptionHole inscriptionHole = mGoInscriptionHoles[i].GetComponent<EquipmentInscriptionHole>();
                            if (inscriptionHole != null)
                            {
                                inscriptionHole.OnItemVisiable(itemData.InscriptionHoles[i]);
                            }
                        }
                        else
                        {
                            var go = AssetLoader.GetInstance().LoadResAsGameObject(mEquipmentInscriptionHolePath);
                            if (go != null)
                            {
                                Utility.AttachTo(go, goInscriptionHoleRoot);
                                EquipmentInscriptionHole inscriptionHole = go.GetComponent<EquipmentInscriptionHole>();
                                if (inscriptionHole != null)
                                {
                                    inscriptionHole.OnItemVisiable(itemData.InscriptionHoles[i]);
                                }

                                mGoInscriptionHoles.Add(go);
                            }
                        }
                    }

                    if (isShowInscriptionHoleInfo == true)
                    {
                        Name.rectTransform.anchoredPosition = new UnityEngine.Vector2(Name.rectTransform.anchoredPosition.x, 30f);
                    }
                    else
                    {
                        Name.rectTransform.anchoredPosition = new UnityEngine.Vector2(Name.rectTransform.anchoredPosition.x, 0);
                    }
                }
            }
        }

        private void RefreshGradeInfo()
        {
            if (mGradeLevel != null)
            {
                mGradeLevel.text = itemData.GetEquipmentGradeDesc();
            }
        }

        void OnDestroy()
        {
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            if (comItem != null)
            {
                //comItem.imgBackGround.enabled = true;
                comItem = null;
            }
        }

        private void OnUpdateItem(List<Item> items)
        {
            if (this.itemData == null)
                return;

            if (items == null)
            {
                return;
            }
            
            for (int i = 0; i < items.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }
                
                if(itemData.GUID == this.itemData.GUID)
                {
                    this.itemData.SubQuality = itemData.SubQuality;
                    RefreshGradeInfo();
                }
            }
        }
    }
}