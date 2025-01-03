using System;
using UnityEditor;

namespace TPImporter
{
	public class SheetInfo
	{
		public SpriteMetaData[] metadata = null;

		public int width = -1;

		public int height = -1;

		public bool pivotPointsEnabled = true;

		public bool bordersEnabled = false;

		public bool polygonsEnabled = false;

		public bool alphaIsTransparency = true;

		public string sheetHash;
	}
}
