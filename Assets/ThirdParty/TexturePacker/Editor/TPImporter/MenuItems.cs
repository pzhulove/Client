using System;
using System.Collections.Generic;
using UnityEditor;

namespace TPImporter
{
	public class MenuItems
	{
		[MenuItem("Assets/TexturePacker/Reload sprite sheets")]
		private static void ReloadSpriteSheets()
		{
			SettingsDatabase instance = SettingsDatabase.getInstance();
			List<string> list = instance.allDataFiles();
			foreach (string current in list)
			{
				SheetInfo sheetInfo = TexturePackerImporter.spriteSheets.sheetInfoForDataFile(current);
				sheetInfo.sheetHash = "";
				AssetDatabase.ImportAsset(current, ImportAssetOptions.ForceUpdate);
			}
		}
	}
}
