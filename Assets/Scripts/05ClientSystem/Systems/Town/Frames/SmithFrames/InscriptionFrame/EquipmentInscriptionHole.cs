using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class EquipmentInscriptionHole : MonoBehaviour
    {
        [SerializeField] private Image mHoleBackGround;//孔边框
        [SerializeField] private Image mCanOpenHoleBackGround;
        [SerializeField] private Image mHasBeenSetBackGround;
        [SerializeField] private Image mHasBeenSetIncriptionIcon;
        [SerializeField] private Text mHoleName;
        [SerializeField] private Text mInscriptionArrt;
        [SerializeField] private StateController mStateCotrol;
        [SerializeField] private Button mIncriptionIconBtn;

        private InscriptionHoleData mData;
        private InscriptionMosaicState mState;
        private void OnDestroy()
        {
            mData = null;
            mState = InscriptionMosaicState.None;
        }

        public void OnItemVisiable(InscriptionHoleData holeData)
        {
            mData = holeData;
            UpdateState();
            InitInterface();
        }

        private void UpdateState()
        {
            //未开孔
            if (mData.IsOpenHole == false)
            {
                mStateCotrol.Key = "CanOpenHole";
                mState = InscriptionMosaicState.CanOpenHole;
            }
            else
            {
                if (mData.InscriptionId == 0)
                {
                    mStateCotrol.Key = "CanBeSet";
                    mState = InscriptionMosaicState.CanBeSet;
                }
                else if (mData.InscriptionId > 0)
                {
                    mStateCotrol.Key = "HasBeenSet";
                    mState = InscriptionMosaicState.HasBeenSet;
                }
            }
        }

        private void InitInterface()
        {
            var inscriptionHoleSetTable = TableManager.GetInstance().GetTableItem<InscriptionHoleSetTable>(mData.Type);
            if (inscriptionHoleSetTable == null)
            {
                return;
            }

            switch (mState)
            {
                case InscriptionMosaicState.None:
                    break;
                case InscriptionMosaicState.CanOpenHole:
                    {
                        if (mCanOpenHoleBackGround != null)
                        {
                            ETCImageLoader.LoadSprite(ref mCanOpenHoleBackGround, inscriptionHoleSetTable.InscriptionHolePicture);
                            mCanOpenHoleBackGround.SetNativeSize();
                        }
                    }
                    break;
                case InscriptionMosaicState.CanBeSet:
                    {
                        if (inscriptionHoleSetTable != null)
                        {
                            if (inscriptionHoleSetTable.InscriptionHolePicture != "")
                            {
                                ETCImageLoader.LoadSprite(ref mHoleBackGround, inscriptionHoleSetTable.InscriptionHolePicture);
                                mHoleBackGround.SetNativeSize();
                            }
                            
                            if (mHoleName != null)
                            {
                                mHoleName.text = InscriptionMosaicDataManager.GetInstance().GetInscriptionHoleName(mData.Type);
                            }
                        }
                    }
                    break;
                case InscriptionMosaicState.HasBeenSet:
                    {
                        ItemData itemData = ItemDataManager.CreateItemDataFromTable(mData.InscriptionId);
                        if (itemData != null)
                        {
                            if (mHasBeenSetBackGround != null)
                            {
                                mHasBeenSetBackGround.color = itemData.GetQualityInfo().Col;
                            }

                            if (mHasBeenSetIncriptionIcon != null)
                            {
                                ETCImageLoader.LoadSprite(ref mHasBeenSetIncriptionIcon, itemData.Icon);
                            }
                           
                            if (mIncriptionIconBtn != null)
                            {
                                mIncriptionIconBtn.onClick.RemoveAllListeners();
                                mIncriptionIconBtn.onClick.AddListener(()=> { ItemTipManager.GetInstance().ShowTip(itemData); });
                            }
                        }

                        if (mInscriptionArrt != null)
                            mInscriptionArrt.text = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(mData.InscriptionId);
                    }
                    break;
            }
        }
    }
}