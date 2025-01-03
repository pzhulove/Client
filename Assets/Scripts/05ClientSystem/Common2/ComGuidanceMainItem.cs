using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ComData = ProtoTable.GuidanceMainItemTable;

namespace GameClient
{
    class ComGuidanceMainItem : MonoBehaviour
    {
        public Image kIcon;
        public Text kDesc;
        public Transform kStart;

		public const string TO_CHAPTER = "<type=sceneid value=6002>";
		public const string LINK_KING_LEVEL = "888888";
		public const string LINK_BOX = "999999";
		public const int LINK_BOX_INT = 999999;

        public void OnClickLink()
        {
            if(value != null)
            {

				var tmpLinkInfo = value.LinkInfo;

				//获取最近打过的王者关卡ID
				if (tmpLinkInfo.Contains(LINK_KING_LEVEL))
				{
					int dungeonID = ChapterUtility.GetLastedDungeonIDByDiff(3);
					if (dungeonID == -1)
					{
						tmpLinkInfo = TO_CHAPTER;
					}
					else {
						tmpLinkInfo.Replace(LINK_KING_LEVEL, ""+dungeonID);
					}
				}
				else if (tmpLinkInfo.Contains(LINK_BOX))
				{
					ChapterSelectFrame.SetDungeonID(LINK_BOX_INT);
					tmpLinkInfo = TO_CHAPTER;
				}

                if (value.bCloseFrame == 1 && frame != null)
                {
                    frame.Close(true);
                    frame = null;
                }
                ActiveManager.GetInstance().OnClickLinkInfo(tmpLinkInfo);
            }
        }

        ComData value;
        public ComData Value
        {
            get
            {
                return value;
            }
        }

        DevelopGuidanceMainFrame frame;

        void OnDestroy()
        {
            kIcon = null;
            kDesc = null;

            value = null;
            frame = null;
        }

        public void OnVisible(ComData data, ClientFrame clientFrame)
        {
            value = data;
            frame = clientFrame as DevelopGuidanceMainFrame;

            if(value != null)
            {
                // kIcon.sprite = AssetLoader.instance.LoadRes(value.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref kIcon, value.Icon);
                kDesc.text = value.Desc;
                foreach (Transform child in kStart.transform)
                {
                    child.gameObject.CustomActive(false);
                }
                for (int i = 0; i < value.recommandLv; i++)
                {
                    kStart.GetChild(i).gameObject.CustomActive(true);
                }
            }
        }
    }
}