using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
///////删除linq
using Protocol;
using ProtoTable;

namespace GameClient
{
    /// <summary>
    /// 用于控制某种东西按某一种动画移动
    /// </summary>
    class SliderChange : MonoBehaviour
    {

        /// <summary>
        /// 需要被移动的节点
        /// </summary>
        private RectTransform root;

        public List<AnimationCurve> animationCurves; //动画曲线列表 
        public float speed = 1.0f;
        public Slider slider = null;//跟谁特效移动的slider

        private bool isRemove;  //是否在移动中
        private int randomTime;  //旋转时间
        private Vector2 startPosition; //开始的时候横坐标
        private Vector2 endPosition; //结束的时候的横坐标
        private bool removeCommand = false; //开始命令
        private System.Action EndCallBack; //旋转结束回调
        private float sliderWidth;
        void Start()
        {
            isRemove = false;
            //避免没有预设曲线报错(这里建一条先慢再快再慢的动画曲线)
            if (animationCurves.Count <= 0)
            {
                Keyframe[] ks = new Keyframe[2];
                ks[0] = new Keyframe(0, 0)
                {
                    inTangent = 2,
                    outTangent = 2
                };
                ks[1] = new Keyframe(1, 1)
                {
                    inTangent = 0,
                    outTangent = 0
                };
                AnimationCurve animationCurve = new AnimationCurve(ks);
                animationCurves.Add(animationCurve);
            }
            if(slider != null)
            {
                sliderWidth = slider.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
            }
        }

        /// <summary>
        /// 外部调用，开始移动
        /// </summary>
        /// <param name="beginPos"></param>开始位置
        /// <param name="endPos"></param>结束位置
        /// <param name="callback"></param>回调
        public void StartRemove(Vector2 beginPos, Vector2 endPos, System.Action callback)
        {
            removeCommand = true;
            startPosition = beginPos;
            //避免误差，开始确保其在该在的位置
            RectTransform thisRectTransform = this.GetComponent<RectTransform>();
            if(thisRectTransform == null || slider == null)
            {
                removeCommand = false;
            }
            thisRectTransform.anchoredPosition = startPosition;


            endPosition = endPos;
            if(callback != null)
            {
                EndCallBack = callback;
            }
        }

        void Update()
        {
            if (removeCommand && !isRemove)
            {
                int time = 2;
                removeCommand = false;
                StartCoroutine(SpinTheWheel(time));
            }
        }

        IEnumerator SpinTheWheel(float time)
        {
            isRemove = true;

            float timer = 0.0f;
            int animationCurveNumber = Random.Range(0, animationCurves.Count);  //获取一个随机动画

            RectTransform thisRectTransform = this.GetComponent<RectTransform>();
            while (timer < time)
            {
                float tempX = (endPosition.x - startPosition.x) * animationCurves[animationCurveNumber].Evaluate(timer / time) + startPosition.x;
                Vector2 tempVec = new Vector2(tempX, thisRectTransform.anchoredPosition3D.y);
                thisRectTransform.anchoredPosition = tempVec;

                slider.value = tempX / sliderWidth;

                timer += Time.deltaTime * speed;
                yield return 0;
            }

            //避免误差，最终确保其在该在的位置
            thisRectTransform.anchoredPosition = endPosition;
            //执行回调 
            if (EndCallBack != null && ClientSystemManager.GetInstance().IsFrameOpen<EquipRecoveryFrame>())
            {
                EndCallBack();
                EndCallBack = null;
            }
            isRemove = false;
        }
    }
}


