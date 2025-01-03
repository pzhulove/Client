using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

namespace GameClient
{
    public class RaycastObject : MonoBehaviour
    {
        public enum RaycastObjectType
        {
            ROT_INVALID = 0,
            ROT_NPC,
            ROT_TOWNPLAYER,
        }
        Int32 iParam0 = 0;
        RaycastObjectType eRaycastObjectType = RaycastObjectType.ROT_INVALID;
        object data = null;

        public void Initialize(Int32 iID, RaycastObjectType eRaycastObjectType, object data)
        {
            this.iParam0 = iID;
            this.eRaycastObjectType = eRaycastObjectType;
            this.data = data;
        }

        public object Data
        {
            get { return this.data; }
        }

        public RaycastObjectType ObjectType
        {
            get { return this.eRaycastObjectType; }
        }

        public Int32 ID
        {
            get { return iParam0; }
        }
    }
}