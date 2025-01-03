

using System.Collections.Generic;
using Tenmove.Runtime;
using UnityEditor;
using UnityEngine;

namespace Tenmove.Editor.Unity
{
    public partial class MiscTool
    {
        public class CheckWeaponMesh : UnityEditor.Editor
        {
            [MenuItem("Assets/Check Weapon")]
            public static void Function()
            {
                Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);                
                for (int i = 0; i < selection.Length; ++i)
                {
                    GameObject curPrefab = selection[i] as GameObject;// AssetDatabase.LoadAssetAtPath<GameObject>(prefabList[i]);
                    if (null == curPrefab)
                        continue;

                    string path = AssetDatabase.GetAssetPath(curPrefab);
                    if (path.Contains("/Weapon/Weapon_") && path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                    {
                        ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
                        if (null != importer && !importer.isReadable)
                        {
                            Debug.LogFormat("### Weapon mesh:'{0}' is not readable!", path);
                            importer.isReadable = true;
                            importer.SaveAndReimport();
                        }
                    }
                }
            }
        }
    }
}