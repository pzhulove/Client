using UnityEngine;
using System.Collections.Generic;
using System;

namespace GameClient
{
    public class IStatus
    {
        public enum IStatusFuntion
        {
            ISF_INVALID = -1,
            ISF_POSITION = 0,
            ISF_VISIBLE,
        }
        protected IStatusFuntion m_eIStatusFuntion = IStatusFuntion.ISF_INVALID;
        public IStatusFuntion StatusFuntion
        {
            get
            {
                return m_eIStatusFuntion;
            }
        }
        public virtual void DoStatus()
        {

        }
    }

    [Serializable]
    public class PositionStatus : IStatus
    {
        public PositionStatus()
        {
            m_eIStatusFuntion = IStatusFuntion.ISF_POSITION;
        }
        public Vector3 position;
        public GameObject target;

        public override void DoStatus()
        {
            if(target != null)
            {
                target.transform.localPosition = position;
            }
        }
    }

    [Serializable]
    public class VisibleStatus : IStatus
    {
        public VisibleStatus()
        {
            m_eIStatusFuntion = IStatusFuntion.ISF_VISIBLE;
        }
        public bool bVisible;
        public GameObject target;

        public override void DoStatus()
        {
            if (target != null)
            {
                target.CustomActive(bVisible);
            }
        }
    }

    [Serializable]
    public class StatusGroup
    {
        public int iTag;
        public List<PositionStatus> m_akPostions = null;
        public List<VisibleStatus> m_akVisibles = null;
        public void DoStatus()
        {
            for (int i = 0; m_akPostions != null && i < m_akPostions.Count; ++i)
            {
                m_akPostions[i].DoStatus();
            }

            for (int i = 0; m_akVisibles != null && i < m_akVisibles.Count; ++i)
            {
                m_akVisibles[i].DoStatus();
            }
        }
    }

    [ExecuteAlways]
    public class StatusBinder : MonoBehaviour
    {
        public int iDefaultStatus = 0;
        public List<StatusGroup> m_akGroups = new List<StatusGroup>();

        int iPreData = -1;

        // Use this for initialization
        void Start()
        {
            //ChangeStatus(iDefaultStatus);

            //InvokeRepeating("_Update", 0.0f, 1.0f);
        }

        public void ChangeStatus(int iStatus)
        {
            iDefaultStatus = iStatus;
            var find = m_akGroups.Find(x =>
            {
                return x.iTag == iStatus;
            });

            if(find != null)
            {
                find.DoStatus();
            }
        }

        void OnDestroy()
        {
            //CancelInvoke("_Update");
        }

        void _Update()
        {
            if(iPreData != iDefaultStatus)
            {
                iPreData = iDefaultStatus;
                ChangeStatus(iPreData);
            }
        }
    }
}