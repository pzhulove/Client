using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    public abstract class TMModuleComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            TMModuleComponentManager.RegisterComponent(this);
        }
    }
}

