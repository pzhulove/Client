using UnityEngine.UI;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    class AwakeAwardFrame : ClientFrame
    {
        const int MaxItemNum = 6;

        int TaskID = 0;

        protected override void _OnOpenFrame()
        {
            TaskID = (int)userData;

            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        public override string GetPrefabPath()
        {
            return "UI/Prefabs/AwakeAwardFrame";
        }

        void ClearData()
        {
            TaskID = 0;
        }

        [UIEventHandle("btGoOnChallenge")]
        void OnGoOnChallenge()
        {
            MissionTable MissionData = TableManager.GetInstance().GetTableItem<MissionTable>(TaskID);
            if(MissionData == null)
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<AwakeTaskFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AwakeTaskFrame>();
            }

            if (PlayerBaseData.GetInstance().AwakeState < 1 && MissionData.AfterID != 0)
            {
                ClientSystemManager.GetInstance().OpenFrame<AwakeTaskFrame>(FrameLayer.Middle);
            }

            if(PlayerBaseData.GetInstance().AwakeState == 1)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.AwakeChanged);
            }

            frameMgr.CloseFrame(this);
        }

        void InitInterface()
        {
            MissionTable MissionItemData = TableManager.GetInstance().GetTableItem<MissionTable>(TaskID);
            if (MissionItemData == null)
            {
                return;
            }

            TaskName.text = MissionItemData.TaskName;

            string[] awards = MissionItemData.Award.Split(new char[] { ',' });

            for (int i = 0; i < MaxItemNum && i < awards.Length; i++)
            {
                var award = awards[i].Split(new char[] { '_' });
                if (award.Length == 2)
                {
                    ItemData data = ItemDataManager.CreateItemDataFromTable(int.Parse(award[0]));
                    if (data == null)
                    {
                        continue;
                    }

                    data.Count = int.Parse(award[1]);

                    ComItem ShowItem = CreateComItem(pos[i].gameObject);
                    ShowItem.Setup(data, null);
                }
            }

            if(MissionItemData.AfterID == 0)
            {
                Challenge.text = "完成";
            }         
        }

        [UIControl("TaskName")]
        protected Text TaskName;

        [UIControl("pos/pos{0}", typeof(RectTransform), 1)]
        protected RectTransform[] pos = new RectTransform[MaxItemNum];

        [UIControl("btGoOnChallenge/Text")]
        protected Text Challenge;
    }
}