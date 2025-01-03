using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChaosAddtionItem : MonoBehaviour, IDisposable
    {

        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private Text des;

        public void Init(int id)
        {
            InitItemes(id);
        }

        void InitItemes(int id)
        {
            var buffRandomTable = TableManager.GetInstance().GetTableItem<BuffRandomTable>(id);
            if (buffRandomTable == null)
            {
                return;
            }

            var buffTable = TableManager.GetInstance().GetTableItem<BuffTable>(buffRandomTable.BuffId);
            if (buffRandomTable == null)
            {
                return;
            }
            ETCImageLoader.LoadSprite(ref iconImage, buffTable.Icon);
            des.SafeSetText(buffTable.Name);
        }
        public void Dispose()
        {

        }
    }
}
