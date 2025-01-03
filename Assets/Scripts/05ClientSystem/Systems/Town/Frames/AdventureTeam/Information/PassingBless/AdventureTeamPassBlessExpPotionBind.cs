using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamPassBlessExpPotionBind : MonoBehaviour
    {
        private const string EFFUI_EXP_POTION_FILL_UP_RES_PATH = "Effects/UI/Prefab/EffUI_yongbingtuan/Prefab/EffUI_yongbingtuan_xiaopin";

        [SerializeField]
        private UIGray drugGray;
        [SerializeField]
        private Text drugIndex;
        [SerializeField]
        private GameObject emptyExpFlyGo;
        [SerializeField]
        [Header("经验激活一个的动画时长")]
        private float mEffectExpFillupDuration = 3f;

        
        //[Header("经验飞行轨迹 依赖于终点配置")]
        //[Header("1.经验飞行时间\n 2.经验飞行轨迹弧度\n 3.经验飞行轨迹点数量，不包括开始点，包括结束点\n")]
        //[SerializeField]
        //private AdventureTeamExpFlyEffectConfig expFlyConfig;

        //是否为空
        private bool isEmpty = false;
        private GameObject fillupEffectGo = null;

        private UnityEngine.Coroutine waitToPlayExpFillupEffect = null;

        void Awake()
        {
            if (fillupEffectGo == null)
            {
                fillupEffectGo = AssetLoader.GetInstance().LoadResAsGameObject(EFFUI_EXP_POTION_FILL_UP_RES_PATH);
                fillupEffectGo.CustomActive(false);
                Utility.AttachTo(fillupEffectGo, emptyExpFlyGo);
            }
        }

        void OnDestroy()
        {
            isEmpty = false;
            fillupEffectGo = null;

            if (waitToPlayExpFillupEffect != null)
            {
                StopCoroutine(waitToPlayExpFillupEffect);
                waitToPlayExpFillupEffect = null;
            }
        }

        public void InitView(string indexStr, bool bEmpty)
        {
            SetDrugIndex(indexStr);
            SetDrugStatus(bEmpty);
        }

        public void Useup()
        {
            if (!isEmpty)
            {
                SetEffectShow(true);
                SetDrugStatus(true);
            }
        }

        public void Fillup()
        {
            if (isEmpty)
            {
                SetEffectShow(true);
                SetDrugStatus(false);
            }
        }

        public void SetEffectShow(bool bShow)
        {
            if (fillupEffectGo)
            {
                if (!bShow)
                {
                    fillupEffectGo.CustomActive(false);
                }
                else
                {
                    if (waitToPlayExpFillupEffect != null)
                    {
                        StopCoroutine(waitToPlayExpFillupEffect);
                    }
                    waitToPlayExpFillupEffect = StartCoroutine(_WaitToPlayExpFillupEffect());
                }
            }
        }

        IEnumerator _WaitToPlayExpFillupEffect()
        {
            fillupEffectGo.CustomActive(false);
            fillupEffectGo.CustomActive(true);
            yield return Yielders.GetWaitForSeconds(mEffectExpFillupDuration);
            fillupEffectGo.CustomActive(false);
        }

        public bool GetDrugIsEmpty()
        {
            return isEmpty;
        }

        public void SetDrugStatus(bool bEmpty)
        {
            if (drugGray)
            {
                drugGray.enabled = bEmpty;
            }
            isEmpty = bEmpty;
        }

        public void SetDrugIndex(string index)
        {
            if (drugIndex)
            {
                drugIndex.text = index;
            }
        }

        public GameObject GetEmptyExpFlyTarget()
        {
            return emptyExpFlyGo;
        }
    }
}