using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class ComMapRoad : MonoBehaviour
    {
        public ComMapScene Scene0;
        public ComMapScene Scene1;

        bool m_bInited = false;
        ComMapScene[] m_scenes = new ComMapScene[2];

        //public void Awake()
        //{
        //    m_scenes[0] = Scene0;
        //    m_scenes[1] = Scene1;

        //    for (int i = 0; i < m_scenes.Length; ++i)
        //    {
        //        if (m_scenes[i] != null)
        //        {
        //            if (m_scenes[i].Roads.Contains(this) == false)
        //            {
        //                m_scenes[i].Roads.Add(this);
        //            }
        //        }
        //    }
        //}

        //public void Initialize()
        //{
        //    for (int i = 0; i < m_scenes.Length; ++i)
        //    {
        //        if (m_scenes[i] == null)
        //        {
        //            Logger.LogErrorFormat("ComMapRoad Scenes has null scene!!");
        //            gameObject.SetActive(false);
        //            m_bInited = false;
        //            return;
        //        }
        //        else
        //        {
        //            if (m_scenes[i].IsActive() == false)
        //            {
        //                gameObject.SetActive(false);
        //                m_bInited = true;
        //                return;
        //            }
        //        }
        //    }

        //    gameObject.SetActive(true);
        //    m_bInited = true;
        //}

        //public void TryActiveRoad()
        //{
        //    if (m_bInited == false)
        //    {
        //        return;
        //    }

        //    for (int i = 0; i < m_scenes.Length; ++i)
        //    {
        //        if (m_scenes[i].IsActive() == false)
        //        {
        //            return;
        //        }
        //    }

        //    gameObject.SetActive(true);
        //}
    }
}
