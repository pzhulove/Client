using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace TPImporter
{
	public class TexturePackerImporter : AssetPostprocessor
	{
		public static SpritesheetCollection spriteSheets = new SpritesheetCollection();

		private static bool createMaterials = false;

		private static bool runningInDelayedCall = false;

		[CompilerGenerated]
		private static EditorApplication.CallbackFunction callback;

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			List<string> list = new List<string>(deletedAssets);
			list.AddRange(movedFromAssetPaths);
			foreach (string current in list)
			{
				if (Path.GetExtension(current).Equals(".tpsheet"))
				{
					Dbg.Log("OnPostprocessAllAssets: removing sprite sheet data " + current);
					TexturePackerImporter.spriteSheets.unloadSheetData(current);
				}
			}
			List<string> list2 = new List<string>(importedAssets);
			list2.AddRange(movedAssets);
			foreach (string current2 in list2)
			{
				if (Path.GetExtension(current2).Equals(".tpsheet"))
				{
					string str = (!SettingsDatabase.getInstance().containsDataFile(current2)) ? "adding " : "updating ";
					Dbg.Log("OnPostprocessAllAssets: " + str + " sprite sheet data " + current2);
					if (TexturePackerImporter.spriteSheets.loadSheetData(current2) == LoaderResult.Loaded)
					{
						SettingsDatabase instance = SettingsDatabase.getInstance();
						string text = instance.spriteFileForDataFile(current2);
						AssetDatabase.ImportAsset(text, ImportAssetOptions.ForceUpdate);
						string text2 = instance.normalsFileForDataFile(current2);
						if (text2 != null)
						{
							AssetDatabase.ImportAsset(text2, ImportAssetOptions.ForceUpdate);
						}
					}
				}
			}
			if (TexturePackerImporter.createMaterials)
			{
				Delegate arg_175_0 = EditorApplication.delayCall;
				if (TexturePackerImporter.callback == null)
				{
					TexturePackerImporter.callback = new EditorApplication.CallbackFunction(TexturePackerImporter.createAllMaterials);
				}
				EditorApplication.delayCall = (EditorApplication.CallbackFunction)Delegate.Combine(arg_175_0, TexturePackerImporter.callback);
			}
		}

		private static void GetOrigImageSize(Texture2D texture, TextureImporter importer, out int width, out int height)
		{
			try
			{
				object[] array = new object[]
				{
					0,
					0
				};
				MethodInfo method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
				{
					typeof(int).MakeByRefType(),
					typeof(int).MakeByRefType()
				}, null);
				if (object.Equals(null, method))
				{
					throw new Exception("Method GetWidthAndHeight(int,int) not found");
				}
				method.Invoke(importer, array);
				width = (int)array[0];
				height = (int)array[1];
			}
			catch (Exception ex)
			{
				Debug.LogError("Invoking TextureImporter.GetWidthAndHeight() failed: " + ex.ToString());
				width = texture.width;
				height = texture.height;
			}
		}

		private void OnPreprocessTexture()
		{
			TextureImporter textureImporter = base.assetImporter as TextureImporter;
			SettingsDatabase instance = SettingsDatabase.getInstance();
			if (instance.isSpriteSheet(base.assetPath))
			{
				textureImporter.textureType = TextureImporterType.Sprite;
			}
			else if (instance.isNormalmapSheet(base.assetPath))
			{
				textureImporter.textureType = TextureImporterType.NormalMap;
			}
		}

		public void OnPostprocessTexture(Texture2D texture)
		{
			TextureImporter textureImporter = base.assetImporter as TextureImporter;
			int num;
			int num2;
			TexturePackerImporter.GetOrigImageSize(texture, textureImporter, out num, out num2);
			Dbg.Log(string.Concat(new object[]
			{
				"OnPostprocessTexture(",
				base.assetPath,
				"), ",
				texture.width,
				"x",
				texture.height,
				", orig:",
				num,
				"x",
				num2
			}));
			SettingsDatabase instance = SettingsDatabase.getInstance();
			if (!instance.isSpriteSheet(base.assetPath))
			{
				string text = Path.ChangeExtension(base.assetPath, ".tpsheet");
				if (File.Exists(text))
				{
					Dbg.Log("No tpsheet loaded for " + base.assetPath + ", loading fallback: " + text);
					if (TexturePackerImporter.spriteSheets.loadSheetData(text) == LoaderResult.Error)
					{
						Dbg.Log("Failed to load sprite sheet data " + text);
					}
					else
					{
						string text2 = instance.normalsFileForDataFile(text);
						if (text2 != null)
						{
							AssetDatabase.ImportAsset(text2, ImportAssetOptions.ForceUpdate);
						}
					}
				}
			}
			if (instance.isSpriteSheet(base.assetPath))
			{
				SheetInfo sheetInfo = TexturePackerImporter.spriteSheets.sheetInfoForSpriteFile(base.assetPath);
				if (sheetInfo.width > 0 && sheetInfo.height > 0 && (sheetInfo.width != num || sheetInfo.height != num2))
				{
					Dbg.Log("Inconsistent sheet size in png/tpsheet");
				}
				else if (textureImporter.textureType != TextureImporterType.Sprite)
				{
					Dbg.Log("Sprite has type " + textureImporter.textureType + ", re-importing it as 'Sprite'");
					textureImporter.textureType = TextureImporterType.Sprite;
					textureImporter.SaveAndReimport();
				}
				else
				{
					textureImporter.alphaIsTransparency = sheetInfo.alphaIsTransparency;
					TexturePackerImporter.setSpriteMeshType(textureImporter, (!sheetInfo.polygonsEnabled) ? SpriteMeshType.FullRect : SpriteMeshType.Tight);
					TexturePackerImporter.updateSpriteMetaData(textureImporter, sheetInfo, texture);
				}
			}
			else if (instance.isNormalmapSheet(base.assetPath))
			{
				textureImporter.textureType = TextureImporterType.NormalMap;
				TexturePackerImporter.createMaterials = true;
			}
			else
			{
				Dbg.Log("No tpsheet file loaded for " + base.assetPath + ", skipping");
			}
		}

		public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
		{
			TexturePackerImporter.spriteSheets.assignGeometries(base.assetPath, sprites);
		}

		private static void setSpriteMeshType(TextureImporter importer, SpriteMeshType type)
		{
			TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
			importer.ReadTextureSettings(textureImporterSettings);
			textureImporterSettings.spriteMeshType = type;
			importer.SetTextureSettings(textureImporterSettings);
		}

		private static void updateSpriteMetaData(TextureImporter importer, SheetInfo sheetInfo, Texture2D texture)
		{
			Dbg.Log("PNG has type " + importer.textureType);
			if (importer.spriteImportMode != SpriteImportMode.Multiple)
			{
				Dbg.Log("changing sprite import mode to Multiple");
				importer.spriteImportMode = SpriteImportMode.Multiple;
				importer.SaveAndReimport();
			}
			SpriteMetaData[] metadata = sheetInfo.metadata;
			bool copyPivotPoints = !sheetInfo.pivotPointsEnabled || !SettingsDatabase.getInstance().importPivotPoints();
			bool copyBorders = !sheetInfo.bordersEnabled;
			MetaDataUpdate.copyOldAttributes(importer.spritesheet, metadata, copyPivotPoints, copyBorders);
			if (!MetaDataUpdate.areEqual(importer.spritesheet, metadata))
			{
				Dbg.Log(string.Concat(new object[]
				{
					"Set new meta data with ",
					metadata.Length,
					" sprites on ",
					importer.assetPath
				}));
				importer.spritesheet = metadata;
				EditorUtility.SetDirty(importer);
				EditorUtility.SetDirty(texture);
				AssetDatabase.WriteImportSettingsIfDirty(importer.assetPath);
				if (!TexturePackerImporter.runningInDelayedCall)
				{
					EditorApplication.delayCall = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.delayCall, new EditorApplication.CallbackFunction(delegate
					{
						TexturePackerImporter.runningInDelayedCall = true;
						AssetDatabase.Refresh();
						TexturePackerImporter.runningInDelayedCall = false;
					}));
				}
			}
			else
			{
				Dbg.Log("Meta data hasn't changed");
			}
		}

		private static void createAllMaterials()
		{
			Dbg.Log("Creating material files");
			SettingsDatabase instance = SettingsDatabase.getInstance();
			List<string> list = instance.allDataFiles();
			foreach (string current in list)
			{
				string normalSheet = instance.normalsFileForDataFile(current);
				string spriteSheet = instance.spriteFileForDataFile(current);
				TexturePackerImporter.createMaterialForNormalmap(normalSheet, spriteSheet);
			}
			TexturePackerImporter.createMaterials = false;
		}

		private static void createMaterialForNormalmap(string normalSheet, string spriteSheet)
		{
			string text = Path.ChangeExtension(spriteSheet, ".mat");
			if (!File.Exists(text))
			{
				Texture2D texture2D = AssetDatabase.LoadAssetAtPath(spriteSheet, typeof(Texture2D)) as Texture2D;
				Texture2D texture2D2 = AssetDatabase.LoadAssetAtPath(normalSheet, typeof(Texture2D)) as Texture2D;
				if (texture2D == null || texture2D2 == null)
				{
					Dbg.Log("Create material: sheet or normals not yet available");
					return;
				}
				bool flag = true;
				Shader shader = Shader.Find("Standard");
				if (shader == null)
				{
					shader = Shader.Find("Transparent/Bumped Diffuse");
					flag = false;
				}
				Material material = new Material(shader);
				material.SetTexture("_MainTex", texture2D);
				material.SetTexture("_BumpMap", texture2D2);
				if (flag)
				{
					material.SetFloat("_Mode", 2f);
					material.SetInt("_SrcBlend", 5);
					material.SetInt("_DstBlend", 10);
					material.SetInt("_ZWrite", 0);
					material.DisableKeyword("_ALPHATEST_ON");
					material.EnableKeyword("_ALPHABLEND_ON");
					material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					material.renderQueue = 3000;
				}
				Dbg.Log("Create material file " + text);
				AssetDatabase.CreateAsset(material, text);
				EditorUtility.SetDirty(material);
			}
		}
	}
}
