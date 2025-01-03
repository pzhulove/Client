
using UnityEngine;
using Tenmove.Runtime;
using System.IO;
using Tenmove.Runtime.Unity;
using System;
using System.Collections.Generic;
using GameClient;

namespace Tenmove.Runtime
{
#if UNITY_EDITOR
    using UnityEditor;

    public class UnitTest_2020_08_28_CheckUIFrameUpperLowerCase : MonoBehaviour
    {
        bool alreadyCheck = false;

        private void Start()
        {

        }

        void Update()
        {
            if (!alreadyCheck)
                _Check();
        }

        void _Check()
        {

            alreadyCheck = true;
            Type[] frameTypes = Utility.Assembly.GetTypesOf(typeof(ClientFrame));
            List<string> pathInScript = new List<string>();
            for (int i = 0, icnt = frameTypes.Length; i < icnt; ++i)
            {
                ClientFrame cur = Utility.Assembly.CreateInstance(frameTypes[i]) as ClientFrame;
                string prefabPath = cur.GetPrefabPath();
                if (string.IsNullOrEmpty(prefabPath))
                    continue;
                pathInScript.Add(Utility.Path.Combine("Assets/Resources", Utility.Path.ChangeExtension(prefabPath, "prefab")));
            }

            using (Stream file = Utility.File.OpenWrite("./UperLowerCaseMissingMatch.txt", true))
            {
                StreamWriter sw = new StreamWriter(file);
                for (int i = 0, icnt = pathInScript.Count; i < icnt; ++i)
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathInScript[i]);
                    string pathInAssets = AssetDatabase.GetAssetPath(prefab);
                    if (pathInAssets != pathInScript[i])
                    {
                        string line = string.Format("Path in script '{0}' case is missing match [Asset:{1}]!", pathInScript[i], pathInAssets);
                        sw.WriteLine(line);
                    }
                }

                sw.Flush();
            }
        
        }
    }
#endif
}