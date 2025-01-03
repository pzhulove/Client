
namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Directory
        {
            static public bool Exists(string path)
            {
                return System.IO.Directory.Exists(path);
            }

            static public System.IO.DirectoryInfo CreateDirectory(string path)
            {
                return System.IO.Directory.CreateDirectory(path);
            }

            static public void Delete(string path)
            {
                System.IO.Directory.Delete(path, true);
            }

            static public void Move(string srcPath,string dstPath)
            {
                System.IO.Directory.Move(srcPath, dstPath);
            }

            static public string[] GetFiles(string path,string searchPattern,bool topOnly)
            {
                return System.IO.Directory.GetFiles(path, searchPattern, topOnly ? System.IO.SearchOption.TopDirectoryOnly : System.IO.SearchOption.AllDirectories);
            }
        }
    }
}
