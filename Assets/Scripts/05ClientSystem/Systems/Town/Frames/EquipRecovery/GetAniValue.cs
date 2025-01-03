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
    /// 用于得到一个不规则还有可能被改变曲线的值，函数模拟成AnimationCurve给策划配置
    /// </summary>
    class GetAniValue : MonoBehaviour
    {
        public AnimationCurve animationCurves; //动画曲线列表 
        void Start()
        {
            //避免没有预设曲线报错
            if (animationCurves == null)
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
                animationCurves = animationCurve;
            }
        }

        /// <summary>
        /// 外部调用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float GetValue(float key)
        {
            return animationCurves.Evaluate(key);
        }
    }
}