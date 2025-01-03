

namespace Tenmove.Runtime
{
    public delegate void OnAssetLoadSuccess<T>(string path, T asset, int taskGrpID, float duration, object userData) where T : class;
    public delegate void OnAssetLoadFailure(string path,int taskGrpID, AssetLoadErrorCode errorCode, string message,object userData);
    public delegate void OnAssetLoadUpdate(string path, int taskGrpID, float progress,object userData);

    public class AssetLoadCallbacks<T> where T : class
    {
        readonly OnAssetLoadSuccess<T> m_OnAssetLoadSuccess;
        readonly OnAssetLoadFailure m_OnAssetLoadFailure;
        readonly OnAssetLoadUpdate m_OnAssetLoadUpdate;

        public AssetLoadCallbacks(OnAssetLoadSuccess<T> onSuccess,OnAssetLoadFailure onFailure)
            : this( onSuccess,  onFailure,  null)
        {

        }

        public AssetLoadCallbacks(OnAssetLoadSuccess<T> onSuccess, OnAssetLoadFailure onFailure,OnAssetLoadUpdate onUpdate)
        {
            Debugger.Assert(null != onSuccess, "On success callback can not be null!");
            Debugger.Assert(null != onFailure, "On failure callback can not be null!");

            m_OnAssetLoadSuccess = onSuccess;
            m_OnAssetLoadFailure = onFailure;
            m_OnAssetLoadUpdate = onUpdate;
        }

        public OnAssetLoadSuccess<T> OnAssetLoadSuccess
        {
            get { return m_OnAssetLoadSuccess; }
        }

        public OnAssetLoadFailure OnAssetLoadFailure
        {
            get { return m_OnAssetLoadFailure; }
        }

        public OnAssetLoadUpdate OnAssetLoadUpdate
        {
            get { return m_OnAssetLoadUpdate; }
        }
    }

}