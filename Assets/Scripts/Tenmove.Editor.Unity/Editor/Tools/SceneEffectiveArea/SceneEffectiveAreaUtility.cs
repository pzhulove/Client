using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tenmove.Editor.Unity
{
    public static class SceneEffectiveAreaUtility
    {
        public static int[] R16TextureBytes2Int(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            if((bytes.Length % 2) != 0)
            {
                Debug.LogError("Texture data length is odd.");
                return null;
            }

            int[] data = new int[bytes.Length / 2];


            for(int i = 0;i < data.Length;++i)
            {
                data[i] = bytes[i * 2 + 1] + (bytes[i * 2] << 8);
            }

            return data;
        }


        const int CONST_R_MASK = 0xFF0000;
        const int CONST_G_MASK = 0x00FF00;
        const int CONST_B_MASK = 0x0000FF;
        public static Color EncodeObjectID(int objectID)
        {
            int r = (objectID & CONST_R_MASK) >> 16;
            int g = (objectID & CONST_G_MASK) >> 8;
            int b = (objectID & CONST_B_MASK);

            return new Color(r / 255f, g / 255f, b / 255f);
        }

        public static int DecodeObjectID(Color color)
        {
            int r = (int)(color.r * 255 + 0.5f);
            int g = (int)(color.g * 255 + 0.5f);
            int b = (int)(color.b * 255 + 0.5f);

            return (r << 16) + (g << 8) + b;
        }
    }
}
