using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace GameClient
{
    class ComFashionEquipDataBinder : MonoBehaviour
    {
        public Text suitName;
        public GameObject[] mRoots = new GameObject[6];
        public GameObject[] mItemParents = new GameObject[6];
        public Text[] mItemNames = new Text[6];
        ComItem[] mComItems = new ComItem[6];
        public DOTweenAnimation dotween = null;
        public float[] mDelays = new float[6];
        public ComEffectPrcess comEffectProcess;
        public float mShakeDelay = 3.0f;
        public ComScreenShake comScreenShake = null;
        public string suitFormatString = string.Empty;

        public GameObject[] mActivityFashionRoots = new GameObject[5];
        public GameObject[] mActivityFashionItemParents = new GameObject[5];
        public Text[] mActivityFashionItemNames = new Text[5];
        ComItem[] mActivityFashionComItems = new ComItem[5];
        public ComEffectPrcess mActivityFashionComEffectProcess;
        public DOTweenAnimation mActivityFashionDotween = null;

        FashionType mFashionType;
        public void SetSuit(FashionType eFashionType,int occu,int suitId)
        {
            mFashionType = eFashionType;
            if (eFashionType != FashionType.FT_NATIONALDAY)
            {
                SetSuit(eFashionType, occu, suitId, mItemParents, mComItems, mItemNames);
            }
            else
            {
                SetSuit(eFashionType, occu, suitId, mActivityFashionItemParents, mActivityFashionComItems, mActivityFashionItemNames);
            }
        }

        void SetSuit(FashionType eFashionType, int occu, int suitId,GameObject[] mItemParents,ComItem[] mComItems, Text[] mItemNames)
        {
            int iSuitId = 0;
            List<int> ids = GamePool.ListPool<int>.Get();
            FashionMergeManager.GetInstance().GetFashionItemsByTypeAndOccu(eFashionType, occu, suitId, ref ids);
            for (int i = 0; i < ids.Count; ++i)
            {
                var item = ItemDataManager.GetInstance().GetCommonItemTableDataByID(ids[i]);
                if (null == item)
                {
                    continue;
                }

                if (mComItems[i] == null)
                {
                    mComItems[i] = ComItemManager.Create(mItemParents[i]);
                }

                if (null != mComItems[i])
                {
                    mComItems[i].Setup(item, null);
                }

                if (null != mItemNames[i])
                {
                    mItemNames[i].text = item.GetColorName();
                }

                iSuitId = item.SuitID;
            }
            GamePool.ListPool<int>.Release(ids);

            if (null != suitName)
            {
                var id = FashionMergeManager.GetInstance().GetFashionByKey(eFashionType, occu, suitId, ProtoTable.ItemTable.eSubType.FASHION_HEAD);
                var suitItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(id);
                if (null != suitItem)
                {
                    suitName.text = string.Format(suitFormatString, suitItem.SuitName);
                }
            }
        }

        public void Play()
        {
            if(null != dotween)
            {
                for(int i = 0; i < mItemParents.Length; ++i)
                {
                    float f = mDelays[i];
                    int k = i;
                    InvokeMethod.Invoke(this,f, () =>
                    {
                        mRoots[k].CustomActive(true);
                        dotween.DOPlayAllById(k.ToString());
                    });
                }
            }
            InvokeMethod.Invoke(this, mShakeDelay, () =>
            {
                dotween.DOPlayAllById("8");
            });
            if (null != comEffectProcess)
            {
                comEffectProcess.Play();
            }
            /*
            InvokeMethod.Invoke(this, mShakeDelay, () => 
            {
                if(null != comScreenShake)
                {
                    comScreenShake.Shake();
                }
            });
            */
        }

        void ActivityFashionPlay()
        {
            if (null != mActivityFashionDotween)
            {
                for (int i = 0; i < mActivityFashionItemParents.Length; ++i)
                {
                    float f = mDelays[i];
                    int k = i;
                    InvokeMethod.Invoke(this, f, () =>
                    {
                        mActivityFashionRoots[k].CustomActive(true);
                        mActivityFashionDotween.DOPlayAllById(k.ToString());
                    });
                }
            }
            InvokeMethod.Invoke(this, mShakeDelay, () =>
            {
                dotween.DOPlayAllById("8");
            });
            if (null != mActivityFashionComEffectProcess)
            {
                mActivityFashionComEffectProcess.Play();
            }
        }

        void ItemParentDoPlay(int count)
        {
             for(int i = 0; i < mItemParents.Length; ++i)
                {
                    float f = mDelays[i];
                    int k = i;
                    InvokeMethod.Invoke(this,f, () =>
                    {
                        mRoots[k].CustomActive(true);
                        dotween.DOPlayAllById(k.ToString());
                    });
                }
        }

        void Start()
        {
            if (mFashionType != FashionType.FT_NATIONALDAY)
            {
                Play();
            }
            else
            {
                ActivityFashionPlay();
            }
                
        }


        void OnDestroy()
        {
            InvokeMethod.RemoveInvokeCall(this);
        }
    }
}
