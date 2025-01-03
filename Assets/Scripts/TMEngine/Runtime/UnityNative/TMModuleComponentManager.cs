using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    public static class TMModuleComponentManager 
    {
        private const string CONST_UNITY_FRAMEWORK_VERSION = "1.0.0";

        private static readonly LinkedList<TMModuleComponent> s_ModuleComponentList = new LinkedList<TMModuleComponent>();

        public static string Version
        {
            get { return CONST_UNITY_FRAMEWORK_VERSION; }
        }

        public static T GetComponent<T>() where T: TMModuleComponent
        {
            return (_GetComponent(typeof(T)) as T);
        }

        private static TMModuleComponent _GetComponent(Type type)
        {
            LinkedListNode<TMModuleComponent> cur = s_ModuleComponentList.First;
            while(null != cur)
            {
                if (cur.Value.GetType() == type)
                    return cur.Value;

                cur = cur.Next;
            }

            return null;
        }

        internal static void RegisterComponent(TMModuleComponent component)
        {
            if (null == component)
            {
                Debugger.AssertFailed("Module component can not be null!");
                return;
            }

            Type type = component.GetType();
            LinkedListNode<TMModuleComponent> cur = s_ModuleComponentList.First;
            while(null != cur)
            {
                if(cur.Value.GetType() == type)
                {
                    Debugger.LogWarning("Module component type '{0}' is already exist!", type.FullName);
                    return;
                }

                cur = cur.Next;
            }

            s_ModuleComponentList.AddLast(component);
        }
    }
}

