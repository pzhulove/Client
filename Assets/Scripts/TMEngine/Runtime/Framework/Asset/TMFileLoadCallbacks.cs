

namespace Tenmove.Runtime
{
    public delegate void OnFileLoadSuccess(string path,byte[] data,int requestID,float duration,object userData);
    public delegate void OnFileLoadFailure(string path, string message,object userData);
    public delegate void OnFileLoadUpdate(string path,float progress,object userData);

    public class FileLoadCallbacks
    {
        readonly OnFileLoadSuccess m_OnFileLoadSuccess;
        readonly OnFileLoadFailure m_OnFileLoadFailure;
        readonly OnFileLoadUpdate  m_OnFileLoadUpdate;

        public FileLoadCallbacks(OnFileLoadSuccess onSuccess,OnFileLoadFailure onFailure)
            : this( onSuccess,  onFailure,  null)
        {

        }

        public FileLoadCallbacks(OnFileLoadSuccess onSuccess, OnFileLoadFailure onFailure,OnFileLoadUpdate onUpdate)
        {
            Debugger.Assert(null != onSuccess, "On success callback can not be null!");
            Debugger.Assert(null != onFailure, "On failure callback can not be null!");

            m_OnFileLoadSuccess = onSuccess;
            m_OnFileLoadFailure = onFailure;
            m_OnFileLoadUpdate = onUpdate;
        }

        public OnFileLoadSuccess OnFileLoadSuccess
        {
            get { return m_OnFileLoadSuccess; }
        }

        public OnFileLoadFailure OnFileLoadFailure
        {
            get { return m_OnFileLoadFailure; }
        }

        public OnFileLoadUpdate OnFileLoadUpdate
        {
            get { return m_OnFileLoadUpdate; }
        }
    }

}