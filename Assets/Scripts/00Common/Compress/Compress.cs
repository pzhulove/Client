using System;

//namespace Utility

    public class CompressHelper
    {
        private static byte[] buffer = new byte[1024 * 1024];//1M

        public static byte[] Compress(byte[] data, int length)
        {
            long compressedLength = (long)buffer.Length;
            Snappy.Compress(data, length, buffer, ref compressedLength);

            byte[] output = new byte[compressedLength];
            for (int i = 0; i < compressedLength; i++)
            {
                output[i] = buffer[i];
            }

            return output;
        }

        public static byte[] Uncompress(byte[] data, int length)
        {
            long uncompressedLength = (long)buffer.Length;
            uint ret = Snappy.Uncompress(data, length, buffer, ref uncompressedLength);
            if (ret != 0)
            {
                Logger.LogError("uncompress failed, reason:" + ret);
                return null;
            }

            byte[] output = new byte[uncompressedLength];
            for(int i = 0; i < uncompressedLength; i++)
            {
                output[i] = buffer[i];
            }

            return output;
        }
    }
