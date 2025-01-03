using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;

public class OptimizeModelTool
{
	static List<ModelImporter> _ModelImporterList = new List<ModelImporter> ();
	static List<string> _Errors = new List<string>();
	static int _Index = 0;
	static int modifiedCount = 0;

	[MenuItem("Assets/Optimize/FBX -> isReadable[false], importBlendShapes[false], MediumCompression")]
	public static void Optimize()
	{
		_ModelImporterList = FindModelImporters ();
		if (_ModelImporterList.Count > 0)
		{
			_Index = 0;
			modifiedCount = 0;
			_Errors.Clear ();
			EditorApplication.update = ProcessModelImporter;
		}
	}

	static void ProcessModelImporter()
	{
		ModelImporter importer = _ModelImporterList[_Index];
		bool isCancel = EditorUtility.DisplayCancelableProgressBar("优化ModelImporter", importer.assetPath , (float)_Index / (float)_ModelImporterList.Count);

		{
			if (importer.isReadable || importer.importBlendShapes || importer.meshCompression != ModelImporterMeshCompression.Medium) 
			{
				importer.isReadable = false;
				importer.importBlendShapes = false;
				importer.meshCompression = ModelImporterMeshCompression.Medium;
				importer.SaveAndReimport ();

				modifiedCount++;
				Debug.LogFormat (importer.assetPath);
			}

		}

		_Index++;
		if (isCancel || _Index >= _ModelImporterList.Count)
		{
			EditorUtility.ClearProgressBar();
			Debug.Log(string.Format("--优化完成--    错误数量: {0}    总数量: {1}/{2}    错误信息↓:\n{3}\n----------输出完毕----------", _Errors.Count, modifiedCount, _ModelImporterList.Count, string.Join(string.Empty, _Errors.ToArray())));
			Resources.UnloadUnusedAssets();
			GC.Collect();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorApplication.update = null;
			_ModelImporterList.Clear();
			_Index = 0;
			modifiedCount = 0;
		}
	}

	static List<ModelImporter> FindModelImporters()
	{
		string[] guids = null;
		List<string> paths = new List<string>();
		List<ModelImporter> assets = new List<ModelImporter> ();
		UnityEngine.Object[] objs = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
		if (objs.Length > 0)
		{
			for(int i = 0; i < objs.Length; i++)
			{
				string path = AssetDatabase.GetAssetPath (objs [i]);

				if (File.Exists (path) && path.ToLower().EndsWith(".fbx")) 
				{
					ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
					if (importer != null && importer.clipAnimations.Length == 0)
						assets.Add (importer);
				}
				else
					paths.Add(path);
			}
			if(paths.Count > 0)
				guids = AssetDatabase.FindAssets ("t:Model", paths.ToArray());
			else
				guids = new string[]{};
		}
		for(int i = 0; i < guids.Length; i++)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath (guids [i]);
			ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
			if (importer != null && importer.clipAnimations.Length == 0)
				assets.Add (importer);
		}
		return assets;
	}
}


