using System;
using UnityEditor;

namespace TPImporter
{
	public class MetaDataUpdate
	{
		public static bool areEqual(SpriteMetaData[] meta1, SpriteMetaData[] meta2)
		{
			bool flag = meta1.Length == meta2.Length;
			int num = 0;
			while (flag && num < meta1.Length)
			{
				flag = (flag && meta1[num].name == meta2[num].name);
				flag = (flag && meta1[num].rect == meta2[num].rect);
				flag = (flag && meta1[num].border == meta2[num].border);
				flag = (flag && meta1[num].pivot == meta2[num].pivot);
				flag = (flag && meta1[num].alignment == meta2[num].alignment);
				num++;
			}
			return flag;
		}

		public static void copyOldAttributes(SpriteMetaData[] oldMeta, SpriteMetaData[] newMeta, bool copyPivotPoints, bool copyBorders)
		{
			for (int i = 0; i < newMeta.Length; i++)
			{
				for (int j = 0; j < oldMeta.Length; j++)
				{
					SpriteMetaData spriteMetaData = oldMeta[j];
					if (spriteMetaData.name == newMeta[i].name)
					{
						if (copyPivotPoints)
						{
							newMeta[i].pivot = spriteMetaData.pivot;
							newMeta[i].alignment = spriteMetaData.alignment;
						}
						if (copyBorders)
						{
							newMeta[i].border = spriteMetaData.border;
						}
						break;
					}
				}
			}
		}
	}
}
