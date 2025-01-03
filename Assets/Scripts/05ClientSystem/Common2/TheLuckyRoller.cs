using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
///////删除linq
using Protocol;
using ProtoTable;

namespace GameClient
{
    class TheLuckyRoller : MonoBehaviour
    {
        /// <summary>
        /// 幸运转盘指针父对象的Transform
        /// </summary>
        private Transform roolPointer;

        /// <summary>
        /// button事件，控制指针
        /// </summary>
        private Button button;
        
        public List<AnimationCurve> animationCurves; //动画曲线列表 
        public float speed = 1.1f;
        private bool spinning;  //是否在旋转中
        private float anglePerItem;  //每个item角度(360/item个数)
        private int randomTime;  //旋转时间
        private int itemNumber;  //item个数

        private bool rotateCommand = false; //旋转命令
        private int targetItemIndex; //目标item索引(从0开始)
        private bool CW = true; //是否顺时针
        private System.Action EndCallBack; //旋转结束回调

        void Start()
        {
            transform.eulerAngles = new UnityEngine.Vector3(0, 0, 0);
            //获取幸运转盘指针父对象的Transform
            roolPointer = GetComponent<Transform>();
            spinning = false;
            //避免没有预设曲线报错(这里建一条先慢再快再慢的动画曲线)
            if (animationCurves.Count <= 0)
            {
                Keyframe[] ks = new Keyframe[2];
                ks[0] = new Keyframe(0, 0);
                ks[0].inTangent = 2;
                ks[0].outTangent = 2;
                ks[1] = new Keyframe(1, 1);
                ks[1].inTangent = 0;
                ks[1].outTangent = 0;
                AnimationCurve animationCurve = new AnimationCurve(ks);
                animationCurves.Add(animationCurve);
            }
        }

        /// <summary>
        /// 开启旋转调用(外部调用)
        /// </summary>
        /// <param name="itemNum">item总个数</param>
        /// <param name="itemIndex">目标item索引，从0开始</param>
        /// <param name="cw">是否顺时针</param>
        /// <param name="callback">结束回调</param>
        public void RotateUp(int itemNum, int itemIndex, bool cw, System.Action callback)
        {
            itemNumber = itemNum;
            anglePerItem = 360 / itemNumber;
            targetItemIndex = itemIndex;
            CW = cw;
            EndCallBack = callback;
            rotateCommand = true;
        }

        void Update()
        {
            if (rotateCommand && !spinning)
            {

                randomTime = 2;  //随机获取旋转全角的次数 

                float maxAngle = -(360 * randomTime + (targetItemIndex * anglePerItem));  //需要旋转的角度
                rotateCommand = false;
                StartCoroutine(SpinTheWheel(randomTime, maxAngle));
            }
        }
   
        IEnumerator SpinTheWheel(float time, float maxAngle)
        {
            spinning = true;

            float timer =0.0f;
            float startAngle = transform.eulerAngles.z;
            //减去相对于0位置的偏移角度
            maxAngle = maxAngle - startAngle;
            //根据顺时针逆时针不同，不同处理
            int cw_value = 1;
            if (CW)
            {
                cw_value = 1;
            }
            int animationCurveNumber = Random.Range(0, animationCurves.Count);  //获取一个随机索引
            if(animationCurveNumber>= animationCurves.Count|| animationCurveNumber<0)
            {
                Logger.LogError(string.Format("数组索引越界，当前数组大小:{0}，当前索引值:{1}", animationCurves.Count, animationCurveNumber));
                yield break;
            }
            AnimationCurve animationCurve= animationCurves[animationCurveNumber];
            if(animationCurve==null)
            {
                Logger.LogError("animationCurve is null");
                yield break;
            }
            while (timer < time)
            {
                //计算旋转,动画曲线的Evaluate函数返回了给定时间下曲线上的值：从0到1逐渐变化，速度又每个位置的切线斜率决定。
                float angle = maxAngle * animationCurve.Evaluate(timer / time);
                //得到的angle从0到最大角度逐渐变化 速度可变,让给加到旋转物角度上实现逐渐旋转 速度可变
                transform.eulerAngles = new Vector3(0.0f, 0.0f, cw_value * angle + startAngle);
                timer += Time.deltaTime*speed;
                yield return 0;
            }

            //避免旋转有误，最终确保其在该在的位置
            transform.eulerAngles = new Vector3(0.0f, 0.0f, cw_value * maxAngle + startAngle);
            //执行回调 
            if (EndCallBack != null)
            {
                EndCallBack();
                EndCallBack = null;
            }
            spinning = false;
        }

        //获取相对角度
        private float GetFitAngle(float angle)
        {
            if (angle > 0)
            {
                if (angle - 360 > 0)
                {
                    return GetFitAngle(angle - 360);
                }
                else
                {
                    return angle;
                }
            }
            else
            {
                if (angle + 360 < 0)
                {
                    return GetFitAngle(angle + 360);
                }
                else
                {
                    return angle;
                }
            }
        }
    }
}


