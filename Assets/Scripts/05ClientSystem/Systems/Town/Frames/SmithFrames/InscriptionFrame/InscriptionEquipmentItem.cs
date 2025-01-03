using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class InscriptionEquipmentItem : MonoBehaviour
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mName;
        [SerializeField] private GameObject mHoleRoot;
        [SerializeField] private GameObject mCheckMark;
        [SerializeField] private GameObject mEquipMark;
        [SerializeField] private string mEquipmentInscriptionHolePath = "UIFlatten/Prefabs/SmithShop/InscriptionFrame/InscriptionHole";

        public static ItemData mSelectItemData = null;
        private ItemData mItemData;
        private List<GameObject> mGoInscriptionHoles = new List<GameObject>();
        public ItemData CurrentItemData
        {
            get { return mItemData; }
            set { mItemData = value; }
        }
        private ComItemNew mComItem;
        public void OnitemVisiable(ItemData itemData)
        {
            mItemData = itemData;
            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(mItemData, Utility.OnItemClicked);

            if (mName != null)
            {
                mName.text = mItemData.GetColorName();
            }

            if (mEquipMark != null)
            {
                mEquipMark.CustomActive(mItemData.PackageType == EPackageType.WearEquip);
            }
           
            if (mGoInscriptionHoles != null && mGoInscriptionHoles.Count > 0)
            {
                for (int i = 0; i < mGoInscriptionHoles.Count; i++)
                {
                    mGoInscriptionHoles[i].CustomActive(false);
                }
            }

            if (itemData.InscriptionHoles.Count > 0)
            {
                for (int i = 0; i < itemData.InscriptionHoles.Count; i++)
                {
                    if ( i< mGoInscriptionHoles.Count)
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
                            Utility.AttachTo(go, mHoleRoot);
                            EquipmentInscriptionHole inscriptionHole = go.GetComponent<EquipmentInscriptionHole>();
                            if (inscriptionHole != null)
                            {
                                inscriptionHole.OnItemVisiable(itemData.InscriptionHoles[i]);
                            }

                            mGoInscriptionHoles.Add(go);
                        }
                    }
                }
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            if (mCheckMark != null)
            {
                mCheckMark.CustomActive(bSelected);
            }
        }

        private void OnDestroy()
        {
            mItemData = null;
            mComItem = null;
            mGoInscriptionHoles.Clear();
        }
    }
}