using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{
    public class NpcDialogSecondView : MonoBehaviour
    {
        [SerializeField] private NpcDialogSecondItem mItem;

        private List<NpcInteractionData> mNpcDataList = null;

        public void Init(List<NpcInteractionData> npcInteractions, UnityAction<bool> unityAction)
        {
            mNpcDataList = npcInteractions;

            for (int i = 0; i < mNpcDataList.Count; i++)
            {
                NpcDialogSecondItem item = _GetChildItem(i);
                if (item != null)
                {
                    item.Init(mNpcDataList[i], unityAction);
                }
            }
        }

        private NpcDialogSecondItem _GetChildItem(int index)
        {
            Transform tr;
            if (index >= transform.childCount)
            {
                tr = Instantiate(mItem, transform).transform;
            }
            else
            {
                tr = transform.GetChild(index);
            }

            if (tr == null)
            {
                tr = Instantiate(mItem, transform).transform;
            }

            NpcDialogSecondItem npcDialogSecondItem = tr.GetComponent<NpcDialogSecondItem>();

            return npcDialogSecondItem;
        }
    }
}