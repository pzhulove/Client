using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ShowCountDownComponent : MonoBehaviour
    {
        public ComCommonBind mBind;
        protected int m_Count_Time = 0;
        protected Coroutine m_Coroutine_Count = null;
        private Text mCountDownText = null;

        public void Awake()
        {
            mBind = GetComponent<ComCommonBind>();
            mCountDownText = mBind.GetCom<Text>("CountDownText");
        }

        //初始化数据
        public void InitData(int time)
        {
            m_Count_Time = time;
            mCountDownText.text = m_Count_Time.ToString();
            m_Coroutine_Count = StartCoroutine(CountDown());
        }

        protected IEnumerator CountDown()
        {
            bool isDone = false;
            while (!isDone)
            {
                yield return Yielders.GetWaitForSeconds(1);
                m_Count_Time--;
                mCountDownText.text = m_Count_Time.ToString();
                if (m_Count_Time<=0)
                {
                    isDone = true;
                }
            }
            CGameObjectPool.instance.RecycleGameObject(gameObject);
        }

        void OnDestroy()
        {
            mCountDownText = null;
            if (m_Coroutine_Count != null)
            {
                StopCoroutine(m_Coroutine_Count);
            }
        }
    }
}
