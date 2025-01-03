using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    public class ComTimeTick : MonoBehaviour
    {
        public UnityAction<uint> onTick;
        uint stamp;
        public void SetEndTime(uint stamp)
        {
            this.stamp = stamp;
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            uint cur = TimeManager.GetInstance().GetServerTime();
            uint delta = cur >= stamp ? 0 : stamp - cur;
            if(null != onTick)
            {
                onTick.Invoke(delta);
            }
        }
    }
}