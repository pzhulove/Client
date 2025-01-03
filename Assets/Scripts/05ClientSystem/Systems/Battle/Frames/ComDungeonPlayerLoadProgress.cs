using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProtoTable;

namespace GameClient
{
    public class ComDungeonPlayerLoadProgress : MonoBehaviour 
    {
        public Text     name;
        public Text     level;
        public Image    icon;
        public Slider   slider;
        public Text     progress;
        public Text     job;
        public Image    rankIcon;
        public Image    rankIconNumber;
        public Text     rankLevel;
        public ReplaceHeadPortraitFrame replaceHeadPortraitFrame;

        private byte    mSeat = byte.MaxValue;


        public void Awake()
        {
            _bindEvent();
        }

        public void OnDestroy()
        {
            _unbindEvent();
        }

        private void _bindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DungeonPlayerLoadProgressChanged, _updatePlayerLoadProcess);

        }

        private void _unbindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DungeonPlayerLoadProgressChanged, _updatePlayerLoadProcess);
        }

        public void SetSeat(byte seat)
        {
            mSeat = seat;

            _updatePlayerBaseInfo();
            _updatePlayerBaseLoadProcess();
        }

        private void _updatePlayerBaseInfo()
        {
            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);
            if (null == player)
            {
                return ;
            }

            if (null != name)
            {
                name.text = player.playerInfo.name;
            }

            if (null != level)
            {
                level.text = player.playerInfo.level.ToString();
            }

            if (null != icon)
            {
                // icon.sprite = _getHeadIcon();
                _getHeadIcon(ref icon);
            }

            if (null != job)
            {
                job.text = Utility.GetJobName(player.playerInfo.occupation, 0);
            }

            if (null != rankIcon)
            {
                string path = SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon((int)player.playerInfo.seasonLevel);
                ETCImageLoader.LoadSprite(ref rankIcon, path);
            }

            if (null != rankIconNumber)
            {
                ETCImageLoader.LoadSprite(ref rankIconNumber, SeasonDataManager.GetInstance().GetSubSeasonLevelIcon((int)player.playerInfo.seasonLevel));
                rankIconNumber.SetNativeSize();
            }


            if (null != rankLevel)
            {
                rankLevel.text = SeasonDataManager.GetInstance().GetRankName((int)player.playerInfo.seasonLevel);
            }

            if (replaceHeadPortraitFrame != null)
            {
                if (player.playerInfo.playerLabelInfo.headFrame != 0)
                {
                    replaceHeadPortraitFrame.ReplacePhotoFrame((int)player.playerInfo.playerLabelInfo.headFrame);
                }
                else
                {
                    replaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                }
            }
        }

        private void _getHeadIcon(ref Image image)
        {
            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);
            if (null == player)
            {
                return;
            }

            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>((int)player.playerInfo.occupation);
            if (null == jobData)
            {
                return;
            }

            ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
            if (null == resData)
            {
                return;
            }

            // return AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref image, resData.IconPath);
        }


        private void _updatePlayerBaseLoadProcess()
        {
            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);
            if (null == player)
            {
                return ;
            }

            if (null == slider)
            {
                return ;
            }

            slider.value = (player.loadRate / 100.0f);

            if (null != progress)
            {
                progress.text = player.loadRate.ToString();
            }
        }

        private void _updatePlayerLoadProcess(UIEvent ui)
        {
            _updatePlayerBaseLoadProcess();
        }
    }
}
