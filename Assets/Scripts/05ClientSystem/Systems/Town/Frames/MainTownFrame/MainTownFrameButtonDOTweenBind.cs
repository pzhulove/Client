using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MainTownFrameButtonDOTweenBind : MonoBehaviour
    {
        [Header("主界面 - 按钮 - 当按钮在动画状态下 - 需要等待动画播完的时间")]
        public float doTweeningDelayTime;
        [Space(10)]
        [Header("按钮父节点绑定的DOTweenAnim")]
        [SerializeField]
        private DOTweenAnimation parentDOTweenAnim;

        void Awake()
        {
            if (parentDOTweenAnim != null)
            {
                //延迟时间应该大于等于DOTween动画播放时间
                if(doTweeningDelayTime < parentDOTweenAnim.duration)
                {
                    doTweeningDelayTime = parentDOTweenAnim.duration;
                }
            }
        }
    }
}