using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using ProtoTable;
using UnityEditor;

namespace GameClient
{
    public class GeTownDoorEffect : MonoBehaviour
    {
        public GameObject opendEffect;
        public GameObject closedEffect;
        public int levelLimit = 1;
        public int unLockFunctionID = -1; // -1的含义是不需要这个条件
        float m_fMaxUpdateTime = 0.5f;
        float m_fTime = 0.0f;
        [HeaderAttribute("地图城镇表id")]
        public int mapTableId;
        public string mapName;
        
        public void SetDoorOpen(bool a_bOpend)
        {
            if (opendEffect != null)
            {
                opendEffect.SetActive(a_bOpend);
            }
            if (closedEffect != null)
            {
                closedEffect.SetActive(!a_bOpend);
            }
        }

        public void Update()
        {
            m_fTime -= Time.deltaTime;
            if (m_fTime <= 0.0f)
            {
                m_fTime = m_fMaxUpdateTime;

                if(PlayerBaseData.GetInstance().Level >= levelLimit)
                {
                    if(unLockFunctionID == -1)
                    {
                        SetDoorOpen(true);
                    }
                    else
                    {
                        FunctionUnLock unLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>(unLockFunctionID);
                        if(unLockData == null)
                        {
                            SetDoorOpen(false);
                        }

                        if (Utility.IsUnLockArea(unLockData.AreaID))
                        {
                            SetDoorOpen(true);
                        }
                        else
                        {
                            SetDoorOpen(false);
                        }     
                    }
                }
                else
                {
                    SetDoorOpen(false);
                }          
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GeTownDoorEffect))]
    public class GeTownDoorEffectInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("check",GUILayout.Width(200)))
            {
                _LoadData(target as GeTownDoorEffect);
            }
        }

        private void _LoadData(GeTownDoorEffect door)
        {
            if (null == door)
                return;
            var table = TableManager.GetInstance().GetTableItem<CitySceneTable>(door.mapTableId);
            if (null == table)
            {
                Logger.LogError("地图城镇表中找不到该数据");
                return;
            }
            door.levelLimit = table.LevelLimit;
            door.mapName = table.Desc;
        }
    }
#endif
}
