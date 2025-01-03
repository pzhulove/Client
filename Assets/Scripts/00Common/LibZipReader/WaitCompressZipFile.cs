namespace Tenmove
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using GameClient;
    using LibZip;

    public class WaitCompressFile : BaseCustomEnum<WaitCompressFile.State>, IEnumerator
    {
        public string FilesRoot { get; set; }
        public string ZipFilePath { get; set; }

        private IList<string> mFiles = null;
        private IntPtr mZipFilePtr = IntPtr.Zero;
        private int mCurrentFileIdx = 0;

        //public ILogicAction<float> OnRateChanged


        public WaitCompressFile(string target, string root, string[] files)
        {
            ZipFilePath = target;
            FilesRoot = root;

            mFiles = files;

            Logger.LogProcessFormat("[Zip] {0} {1} {2}", ZipFilePath, FilesRoot, mFiles.Count);
            _StartProgress();
        }

        public WaitCompressFile(string target, string root, string searchOption)
        {
            ZipFilePath = target;
            FilesRoot = root;
            mFiles = TMFile.GetFiles(root, searchOption);

            Logger.LogProcessFormat("[Zip] {0} {1} {2}", ZipFilePath, FilesRoot, mFiles.Count);
            _StartProgress();
        }

        private bool _StartProgress()
        {
            if (null == mFiles || 0 == mFiles.Count)
            {
                Logger.LogErrorFormat("[Zip] CompressFiles file list is empty");
                SetResult(State.Error);
                return false;
            }

            TMPathUtil.MakeParentRootExist(ZipFilePath);

            mCurrentFileIdx = 0;
            _OpenZipFile();
            SetResult(State.Progress);
            return true;
        }

        public object Current
        {
            get { return null; }
        }

        private int FileCounts
        {
            get
            {
                if (null == mFiles)
                {
                    return 1;
                }

                return mFiles.Count;
            }
        }

        private void _OpenZipFile()
        {
            mZipFilePtr = LibZip.zip_open(ZipFilePath, 1, IntPtr.Zero);
        }

        private void _CloseZipFile()
        {
            if (IntPtr.Zero != mZipFilePtr)
            {
                LibZip.zip_close(mZipFilePtr);
            }

            mZipFilePtr = IntPtr.Zero;
        }


        public bool MoveNext()
        {
            switch (GetResult())
            {
                case State.None:
                    _OpenZipFile();
                    SetResult(State.Progress);

                    break;
                case State.Progress:
                    {
                        if (mCurrentFileIdx >= FileCounts)
                        {
                            SetResult(State.Finish);
                        }
                        else
                        {
                            string currentCompressFile = mFiles[mCurrentFileIdx];

                            if (!TMZipFile.WriteOneFile2ZipSource(mZipFilePtr, FilesRoot, currentCompressFile))
                            {
                                Logger.LogErrorFormat("[Zip] CompressFiles Open File fail {0}", currentCompressFile);
                            }

                            mCurrentFileIdx++;
                        }
                    }
                    break;
                case State.Error:
                case State.Finish:
                    _CloseZipFile();
                    return false;
            }

            return true;
        }

        public void Reset()
        {
        }

        public void DeleteCompressedFiles()
        {
            for (int i = 0; i < mFiles.Count; ++i)
            {
                try 
                {
                    System.IO.File.Delete(mFiles[i]);
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat(e.ToString());
                }
            }


        }

        public bool IsSuccessFinish()
        {
            if (GetResult() != State.Finish)
            {
                return false;
            }

            if (!TMFile.FileExist(ZipFilePath))
            {
                return false;
            }

            return true;
        }

        public void Abort()
        {
            if (GetResult() == State.Progress)
            {
                _CloseZipFile();
            }
        }

        public enum State
        {
            None,
            Progress,
            Finish,
            Error,
        }

        public float Rate
        {
            get
            {
                switch (GetResult())
                {
                    case State.Progress:
                        return 1.0f * mCurrentFileIdx / FileCounts;
                    case State.Error:
                    case State.Finish:
                        return 1.0f;
                }

                return 0.0f;
            }
        }
    }
}
