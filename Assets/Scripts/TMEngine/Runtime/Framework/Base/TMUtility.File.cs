
namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class File
        {
            static public bool Exists(string path)
            {
                return System.IO.File.Exists(path);
            }

            static public System.IO.FileStream OpenRead(string path)
            {
                return System.IO.File.OpenRead(path);
            }

            static public System.IO.FileStream OpenWrite(string path, bool overwrite)
            {
                return new System.IO.FileStream(path, overwrite ? System.IO.FileMode.Create : System.IO.FileMode.Append, System.IO.FileAccess.Write);
            }

            static public long GetByteSize(string path)
            {
                if (System.IO.File.Exists(path))
                {
                    using (System.IO.FileStream file = OpenRead(path))
                    {
                        if (null != file)
                            return file.Length;
                    }
                }
                return ~0;
            }

            static public void Copy(string srcFilePath,string dstFilePath)
            {
                System.IO.File.Copy(srcFilePath,dstFilePath);
            }

            static public void Copy(string srcFilePath, string dstFilePath,bool overwrite)
            {
                System.IO.File.Copy(srcFilePath, dstFilePath, overwrite);
            }

            static public void Move(string srcFilePath, string dstFilePath)
            {
                System.IO.File.Move(srcFilePath, dstFilePath);
            }

            static public void Delete(string path)
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
        }
    }
}
