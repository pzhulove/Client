using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace GameClient
{
    public class ProcessUnit 
    {
        private static int        sDeep       = 0;
        private List<IEnumerator> mItors      = new List<IEnumerator>();
        private List<UnityEngine.Coroutine>   mCoroutines = new List<UnityEngine.Coroutine>();


        public ProcessUnit Append(IEnumerator it)
        {
            Logger.LogProcessFormat("[ProcessUnit] 添加一个任务");
            mItors.Add(it);
            return this;
        }

        public IEnumerator Parallel()
        {
            sDeep++;
            Logger.LogProcessFormat("[ProcessUnit] {0} {1} 个并行任务 : 开始", sDeep, mItors.Count);

            for (int i = 0; i < mItors.Count; ++i)
            {
                mCoroutines.Add(GameFrameWork.instance.StartCoroutine(mItors[i]));
            }

            bool   isEnd     = false;
            bool[] itorsFlag = new bool[mItors.Count];

            while (!isEnd)
            {
                isEnd = true;

                yield return Yielders.EndOfFrame;

                for (int i = 0; i < mItors.Count; ++i)
                {
                    if (mItors[i].Current != null)
                    {
                        isEnd = false;
                        break;
                    }
                    else if (!itorsFlag[i])
                    {
                        itorsFlag[i] = true;
                        Logger.LogProcessFormat("[ProcessUnit] {0} 第 {1} 个并行任务 : 结束", sDeep, mCoroutines.Count);
                    }
                }
            }

            mItors.Clear();

            sDeep--;
            Logger.LogProcessFormat("[ProcessUnit] {0} 开始清除 {1} 个结束的并行任务", sDeep, mCoroutines.Count);

            for (int i = 0; i < mCoroutines.Count; i++ )
            {
                GameFrameWork.instance.StopCoroutine(mCoroutines[i]);
            }

            mCoroutines.Clear();

        }

        public IEnumerator Sequence()
        {
            Logger.LogProcessFormat("[ProcessUnit] {0} 开始 {1} 个串行任务", sDeep, mItors.Count);
            
            for (int i = 0; i < mItors.Count; ++i)
            {
                sDeep++;
                Logger.LogProcessFormat("[ProcessUnit] {0} 第 {1} 个串行任务 : 开始", sDeep, i);
                yield return mItors[i];
                sDeep--;
                Logger.LogProcessFormat("[ProcessUnit] {0} 第 {1} 个串行任务 : 结束", sDeep, i);
            }

            mItors.Clear();

            Logger.LogProcessFormat("[ProcessUnit] {0} 结束所有串行任务", sDeep);

            // dd: 这里默认返回一个null，用于Paralleld的结束判断
            yield return null;

            Logger.LogProcessFormat("[ProcessUnit] {0} 结束所有串行任务之后的一帧", sDeep);
        }
    }
}
