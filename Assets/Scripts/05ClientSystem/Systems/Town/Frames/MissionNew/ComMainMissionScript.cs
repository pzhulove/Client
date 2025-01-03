using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComMainMissionScript : MonoBehaviour
    {
        public static MissionManager.SingleMissionInfo ms_selected = null;
        public Image kIcon;
        public Text kName;
        public GameObject goCheckMark;
        public GameObject goArrow;

        MissionManager.SingleMissionInfo value;
        public MissionManager.SingleMissionInfo Value
        {
            get
            {
                return value;
            }
        }

        bool bAcquired;
        public bool Acquired
        {
            get
            {
                return bAcquired;
            }
        }

        Utility.ContentProcess kContentProcess;
        MissionFrameNew frame;

        public static bool IsLegalMainMission(MissionManager.SingleMissionInfo value)
        {
            if (value == null || value.missionItem == null)
            {
                return false;
            }

            if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_BRANCH)
            {
                return true;
            }

            if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN)
            {
                return true;
            }

            if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_CYCLE)
            {
                return true;
            }

            if(value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_TITLE)
            {
                return true;
            }

            return false;
        }

        void OnDestroy()
        {
            kIcon = null;
            kName = null;
            goCheckMark = null;
            goArrow = null;

            value = null;
            bAcquired = false;
            kContentProcess = null;
            frame = null;
        }

        public void OnVisible(MissionManager.SingleMissionInfo data,ClientFrame clientFrame)
        {
            value = data;
            frame = clientFrame as MissionFrameNew;
            var missionItem = value.missionItem;

            gameObject.name = missionItem.ID.ToString();
            // kIcon.sprite = AssetLoader.instance.LoadRes(Utility.GetMissionIcon(value.missionItem.TaskType), typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref kIcon, Utility.GetMissionIcon(value.missionItem.TaskType));
            kName.text = MissionManager.GetInstance().GetMissionName((uint)missionItem.ID) + MissionManager.GetInstance().GetMissionNameAppendBystatus(value.status,value.missionItem.ID);
            bAcquired = value.status == (int)Protocol.TaskStatus.TASK_OVER;

            kContentProcess = Utility.ParseMissionProcess((int)value.taskID, true);
        }

        public void OnDisplayChange(bool bSelected)
        {
            goCheckMark.CustomActive(bSelected);
            goArrow.CustomActive(bSelected);
        }
    }
}