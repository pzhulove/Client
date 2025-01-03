using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    [System.Serializable]
    class ConsumeData
    {
        public ComCommonConsume.eType eType = ComCommonConsume.eType.Item;
        public ComCommonConsume.eCountType eCountType = ComCommonConsume.eCountType.Fatigue;
        public int itemID = 0;
    }

    class ComFullScreenTitle : MonoBehaviour
    {
        private enum FullScreenTitleType
        {
            IncludeBgType,
            NoBgType,
        }

        [SerializeField]
        ComCommonConsume[] consumes = new ComCommonConsume[0];       

        [SerializeField]
        Button btnChat = null;

        [SerializeField]
        Button btnSet = null;

        [SerializeField]
        CanvasGroup canvasBg = null;

        [SerializeField]
        FullScreenTitleType fullScreenTitleType = FullScreenTitleType.IncludeBgType;

        private void Start()
        {
            _InitTitleType();

            btnChat.SafeSetOnClickListener(() => 
            {

            });

            btnSet.SafeSetOnClickListener(() => 
            {

            });
        }

        private void OnDestroy()
        {
            
        }

        private void _InitTitleType()
        {
            switch (fullScreenTitleType)
            {
                case FullScreenTitleType.IncludeBgType:
                    {
                        canvasBg.CustomActive(true);
                    }
                    break;
                case FullScreenTitleType.NoBgType:
                    {
                        canvasBg.CustomActive(false);
                    }
                    break;
                default:
                    {
                        canvasBg.CustomActive(true);
                    }
                    break;
            }
        }


        public void Init(ConsumeData[] datas,bool showChatBtn,bool showSetBtn)
        {   
            if(consumes != null && datas != null)
            {
                for(int i = 0;i < consumes.Length && i < datas.Length;i++)
                {
                    if(consumes[i] != null)
                    {
                        consumes[i].SetData(datas[i].eType, datas[i].eCountType, datas[i].itemID);
                    }                    
                }
            }

            btnChat.CustomActive(showChatBtn);
            btnSet.CustomActive(showSetBtn);
        }
    }
}