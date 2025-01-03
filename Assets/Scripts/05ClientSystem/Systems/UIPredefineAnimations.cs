using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    [Serializable]
    public class StringResWrapper
    {
        //[AssetPath.Attribute(typeof(GameObject))]
        public string m_Res;
    }

    public class UIPredefineAnimations : MonoBehaviour
    {
        [SerializeField]
        public List<StringResWrapper> m_PredefineAnimations = new List<StringResWrapper>();
    }
}
