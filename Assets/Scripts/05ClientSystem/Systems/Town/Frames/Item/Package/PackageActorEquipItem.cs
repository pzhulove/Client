using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class PackageActorEquipItem : MonoBehaviour
    {
        [SerializeField] private EEquipWearSlotType mSlotType;
        [SerializeField] private EFashionWearNewSlotType mFashionSlotType;
        [SerializeField] private ComItemNew mComItem;

        public EEquipWearSlotType SlotType
        {
            get
            {
                return mSlotType;
            }
        }

        public EFashionWearNewSlotType FashionSlotType
        {
            get
            {
                return mFashionSlotType;
            }
        }

        public ComItemNew GetComItem()
        {
            return mComItem;
        }
    }
}