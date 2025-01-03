
namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Path
        {
            /// <summary>
            /// 获取标准化路径。
            /// </summary>
            /// <param name="path">要标准化的路径。</param>
            /// <returns>标准化后的路径。</returns>
            public static string Normalize(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return path;

                return path.Replace('\\', '/');
            }

            public static string Combine(string path1, string path2)
            {
                string combinedPath = System.IO.Path.Combine(path1, path2);
                return Normalize(combinedPath);
            }

            public static string Combine(params string[] paths)
            {
                if (null == paths || paths.Length < 1)
                    return string.Empty;

                string combinedPath = paths[0];
                for (int i = 1, icnt = paths.Length; i < icnt; ++i)
                    combinedPath = System.IO.Path.Combine(combinedPath, paths[i]);

                return Normalize(combinedPath);
            }

            public static string ChangeExtension(string path, string dstExt)
            {
                string changedPath = System.IO.Path.ChangeExtension(path, dstExt);
                return Normalize(changedPath);
            }

            public static string GetFileName(string path)
            {
                return System.IO.Path.GetFileName(path);
            }

            public static string GetFileNameWithoutExtension(string path)
            {
                return System.IO.Path.GetFileNameWithoutExtension(path);
            }

            public static string GetDirectoryName(string path)
            {
                return System.IO.Path.GetDirectoryName(path);
            }

            public static string GetExtension(string path)
            {
                return System.IO.Path.GetExtension(path);
            }

            public static bool HasExtension(string path)
            {
                return System.IO.Path.HasExtension(path);
            }
        }
    }
}

