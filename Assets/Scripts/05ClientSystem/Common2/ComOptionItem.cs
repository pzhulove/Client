using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    class ComOptionItem : MonoBehaviour
    {
        public Image itemBG;
        public Text itemRealName;
        public Text itemRealAttribute;
        public GameObject itemParent;
        public Text itemCount;
        public Text itemName;
        public Text itemHint;
        public StateController stateController;
        public Button buttonAdd;
        public Text acquiredHint;

        OptionItemData data = null;
        public OptionItemData Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public void OnClickRemove()
        {
            Value.onItemRemove(Value.itemData);
        }

        public void OnClickAdd()
        {
            Value.onOpenEquipList(Value.isLeft);
        }

        public void OnClickBG()
        {
            Value.onOpenEquipList(Value.isLeft);
        }
    }
}