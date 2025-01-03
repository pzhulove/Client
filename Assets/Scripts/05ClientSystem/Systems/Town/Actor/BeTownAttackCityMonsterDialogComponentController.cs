using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    public class BeTownAttackCityMonsterDialogComponentController : MonoBehaviour
    {
        private int _npcId = -1;
        private NpcTable _npcItem = null;
        private const float DisappearInterval = 25.0f;
        private float _disappearInterval = 0.0f;      //不显示时间（默认时间+随机时间）20-30s
        private float _appearInterval = -1.0f;            //显示时间（表格配置)

        private float _lastInterval = 0.0f;                 //持续的时间
        private float _periodTime = 0.0f;                   //一个显示周期时间

        public void InitController(int npcId)
        {
            _npcId = npcId;
            _npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(_npcId);

            if (_npcItem != null)
            {
                _appearInterval = _npcItem.Interval * 1.0f / 1000.0f;
                _disappearInterval = DisappearInterval + UnityEngine.Random.Range(0.0f, 10.0f) - 5.0f;
            }

            _lastInterval = 0.0f;
            _periodTime = _appearInterval + _disappearInterval;
        }

        public void OnUpdate(float timeElapsed)
        {
            if (_appearInterval <= 0)
            {
                return;
            }

            _lastInterval += timeElapsed;
            if (_lastInterval >= 0.0f && _lastInterval <= _appearInterval)
            {
                //显示阶段
                transform.gameObject.CustomActive(true);
            }
            else if (_lastInterval > _appearInterval && _lastInterval <= _periodTime)
            {
                //隐藏阶段
                transform.gameObject.CustomActive(false);
            }
            else
            {
                //超过一个周期，时间重置
                _lastInterval = 0.0f;
                _disappearInterval = DisappearInterval + UnityEngine.Random.Range(0.0f, 10.0f) - 5.0f;
                _periodTime = _appearInterval + _disappearInterval;
            }
        }

        private void OnDestroy()
        {
            _npcId = -1;
            _npcItem = null;
            _appearInterval = -1.0f;
        }

    }
}