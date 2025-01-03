using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace GameClient
{
    public class ComLegendOpenNotify : MonoBehaviour
    {
        public UnityEvent onNotify;
        public UnityEvent onCancelNotify;

        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            _CheckNotify();
        }

        void _CheckNotify()
        {
            bool bNotify = false;
            if (true)// (iPreLv > 0 && iPreLv < iCurLv)
            {
                var legendMainTable = TableManager.GetInstance().GetTable<ProtoTable.LegendMainTable>();
                var enumerator = legendMainTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var lengendMainItem = enumerator.Current.Value as ProtoTable.LegendMainTable;
                    if (Utility.IsLegendSeriesOver(lengendMainItem.ID))
                    {
                        continue;
                    }

                    //bool bPreOpen = iPreLv >= lengendMainItem.UnLockLevel;
                    bool bCurOpen = PlayerBaseData.GetInstance().Level >= lengendMainItem.UnLockLevel;
                    if (bCurOpen /*&& !bPreOpen*/)
                    {
                        var find = MissionManager.GetInstance().DicLegendNotifies.Find(x =>
                        {
                            return (null != x && x.iNotifyID == lengendMainItem.ID);
                        });

                        if(null == find)
                        {
                            MissionManager.GetInstance().DicLegendNotifies.Add(new LegendNotifyData
                            {
                                bNotify = true,
                                iNotifyID = lengendMainItem.ID,
                            });
                        }
                    }
                }
            }

            for(int i = 0; i < MissionManager.GetInstance().DicLegendNotifies.Count; ++i)
            {
                if(MissionManager.GetInstance().DicLegendNotifies[i].bNotify)
                {
                    bNotify = true;
                    break;
                }
            }

            if (bNotify)
            {
                _Notify();
            }
            else
            {
                _CancelNotify();
            }
        }

        // Use this for initialization
        void Start()
        {
            _RemoveListeners();
            _AddListeners();
            _CheckNotify();
        }

        // Update is called once per frame
        void OnDestroy()
        {
            _RemoveListeners();
            onNotify = null;
            onCancelNotify = null;
        }

        void _RemoveListeners()
        {
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
        }

        void _AddListeners()
        {
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
        }

        public void CancelNotify()
        {
            MissionManager.GetInstance().ClearLegendNotifies();
            _CancelNotify();
        }

        void _CancelNotify()
        {
            if (null != onCancelNotify)
            {
                onCancelNotify.Invoke();
            }
        }

        public void Notify()
        {
            _Notify();
        }

        void _Notify()
        {
            if (null != onNotify)
            {
                onNotify.Invoke();
            }
        }
    }
}