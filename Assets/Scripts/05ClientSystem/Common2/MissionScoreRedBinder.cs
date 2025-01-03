using UnityEngine;
using System.Collections;

namespace GameClient
{
    class MissionScoreRedBinder : MonoBehaviour
    {
        public GameObject target;
        public int iLinkID;
        public int LinkID
        {
            set
            {
                iLinkID = value;
                _Update();
            }
        }

        // Use this for initialization
        void Start()
        {
            _Update();

            MissionManager.GetInstance().onDailyScoreChanged += OnDailyScoreChanged;
            MissionManager.GetInstance().onChestIdsChanged += OnChestIdsChanged;
        }

        void OnDestroy()
        {
            MissionManager.GetInstance().onDailyScoreChanged -= OnDailyScoreChanged;
            MissionManager.GetInstance().onChestIdsChanged -= OnChestIdsChanged;

            target = null;
        }

        void OnDailyScoreChanged(int score)
        {
            _Update();
        }

        void OnChestIdsChanged()
        {
            _Update();
        }

        void _Update()
        {
            var data = MissionManager.GetInstance().MissionScoreDatas.Find(x =>
            {
                return x.missionScoreItem.ID == iLinkID;
            });
            bool bShow = false;
            if(data != null)
            {
                bShow = data.missionScoreItem.Score <= MissionManager.GetInstance().Score && !MissionManager.GetInstance().AcquiredChestIDs.Contains(data.missionScoreItem.ID);
            }
            if (target == null)
            {
                target = gameObject;
            }
            target.CustomActive(bShow);
        }
    }
}