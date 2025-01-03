using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class ComFashionMergeResultDataBinder : MonoBehaviour
    {
        public StateController mState;
        public GameObject goNormalItemParent;
        public Text normalName;
        public GameObject goSpecialParent;
        public Text specialName;
        public string normalExpress = string.Empty;
        public string specialExpress = string.Empty;
        public string activityFashionNormalExpress = string.Empty;
        public string activityFashionSpecialExpress = string.Empty;
        public Text title;
        public float lockTime = 1.0f;

        ComItem comNormalItem;
        ComItem comSpecialItem;

        public Vector3 GetSkyWorldPosition()
        {
            if(null != goSpecialParent)
            {
                return goSpecialParent.transform.position;
            }
            return Vector3.zero;
        }

        public void SetValue(FashionResultData result)
        {
            if(null != mState)
            {
                switch(result.eFashionMergeResultType)
                {
                    case FashionMergeResultType.FMRT_NORMAL:
                        {
                            if(null == comNormalItem)
                            {
                                comNormalItem = ComItemManager.Create(goNormalItemParent);
                            }

                            if(null != comNormalItem)
                            {
                                ItemData value = (null == result.datas || result.datas.Count < 1)? null : result.datas[0];
                                comNormalItem.Setup(value, null);
                            }

                            if (null != normalName)
                            {
                                if (null != result.datas && result.datas.Count > 0)
                                {
                                    normalName.text = result.datas[0].GetColorName();
                                }
                            }

                            mState.Key = "normal";
                            if(null != title)
                            {
                                if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
                                {
                                    title.text = normalExpress;
                                }
                                else
                                {
                                    title.text = activityFashionNormalExpress;
                                } 
                            }
                        }
                        break;
                    case FashionMergeResultType.FMRT_SPECIAL:
                        {
                            if (null == comNormalItem)
                            {
                                comNormalItem = ComItemManager.Create(goNormalItemParent);
                            }

                            if (null != comNormalItem)
                            {
                                ItemData value = (null == result.datas || result.datas.Count < 1) ? null : result.datas[0];
                                comNormalItem.Setup(value, null);
                            }

                            if (null != normalName)
                            {
                                if (null != result.datas && result.datas.Count > 0)
                                {
                                    normalName.text = result.datas[0].GetColorName();
                                }
                            }

                            if (null == comSpecialItem)
                            {
                                comSpecialItem = ComItemManager.Create(goSpecialParent);
                            }

                            if (null != comSpecialItem)
                            {
                                ItemData value = (null == result.datas || result.datas.Count < 2) ? null : result.datas[1];
                                comSpecialItem.Setup(value, null);
                            }

                            if (null != specialName)
                            {
                                if (null != result.datas && result.datas.Count > 1)
                                {
                                    specialName.text = result.datas[1].GetColorName();
                                }
                            }

                            mState.Key = "special";

                            if(null != title)
                            {
                                string skyName = string.Empty;
                                if (null != result.datas && result.datas.Count > 1)
                                {
                                    skyName = result.datas[1].GetColorName();
                                }

                                if (!FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
                                {
                                    title.text = string.Format(specialExpress, skyName);
                                }
                                else
                                {
                                    title.text = string.Format(activityFashionSpecialExpress, skyName);
                                } 
                            }
                        }
                        break;
                }
            }
        }

        void OnDestroy()
        {
            if (null != comNormalItem)
            {
                ComItemManager.Destroy(comNormalItem);
                comNormalItem = null;
            }
            if (null != comSpecialItem)
            {
                ComItemManager.Destroy(comSpecialItem);
                comSpecialItem = null;
            }
        }
    }
}
