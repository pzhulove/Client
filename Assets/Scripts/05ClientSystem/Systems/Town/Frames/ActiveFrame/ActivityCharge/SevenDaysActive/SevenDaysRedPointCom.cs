using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class SevenDaysRedPointCom : MonoBehaviour
    {
        public enum EventType
        {
            Null,
            Login,
            Target,
            Charge,
            Gift,
        }

        [SerializeField] private List<EventType> mEventTypes = null;
        [SerializeField] private List<GameObject> mGoRedPoints = null;
        [SerializeField] private List<CanvasGroup> mCanvasRedPoints = null;

        private void Start()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SevenDaysActivityUpdate, _OnSevenDaysLoginDatasUpdate);
            _OnSevenDaysLoginDatasUpdate(null);
        }

        private void _OnSevenDaysLoginDatasUpdate(UIEvent uIEvent)
        {
            bool isShowRedPoint = false;
            if (mEventTypes != null)
            {
                foreach (var eventType in mEventTypes)
                {
                    switch (eventType)
                    {
                        case EventType.Login:
                            {
                                isShowRedPoint = isShowRedPoint || SevendaysDataManager.GetInstance().IsLoginShowRedPoint();
                            }
                            break;
                        case EventType.Target:
                            {
                                isShowRedPoint = isShowRedPoint || SevendaysDataManager.GetInstance().IsTargetShowRedPoint();
                            }
                            break;
                        case EventType.Charge:
                            {
                                isShowRedPoint = isShowRedPoint || SevendaysDataManager.GetInstance().IsChargeShowRedPoint();
                            }
                            break;
                        case EventType.Gift:
                            {
                                isShowRedPoint = isShowRedPoint || SevendaysDataManager.GetInstance().IsGiftShowRedPoint();
                            }
                            break;
                    }
                }
            }

            if (mGoRedPoints != null)
            {
                foreach(var go in mGoRedPoints)
                {
                    go.CustomActive(isShowRedPoint);
                }
            }

            if (mCanvasRedPoints != null)
            {
                foreach(var canvas in mCanvasRedPoints)
                {
                    canvas.CustomActive(isShowRedPoint);
                }
            }
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SevenDaysActivityUpdate, _OnSevenDaysLoginDatasUpdate);
        }

    }
}
