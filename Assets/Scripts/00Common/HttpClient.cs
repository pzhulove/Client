using UnityEngine;
using System.Collections;

public class HttpClient : MonoBehaviour
{
	public static HttpClient 	Instance = null ;
	public delegate void 		deleHttpRequest ( WWW wwwReq ) ;

	protected WWWForm 			wwwForm = null ;

	void Awake ()
	{
		Instance = this ;
	}

	//GET-REQUEST METHOD
	public void 		GetRequest ( string url , deleHttpRequest deleFunc )
	{
		WWW www = new WWW( url ) ;
		//ExceptionManager.Instance ().LogInfo ( www.url );
		StartCoroutine( WaitForGetRequest( www , deleFunc ) ) ;
	}

	//POST-REQUEST METHOD
	public void 		BeginPostRequest ()
	{
		wwwForm = new WWWForm() ;
	}

	public void 		AddField ( string fieldName , string fieldValue )
	{
		if ( wwwForm == null )
		{
			//ExceptionManager.Instance().LogError( "WWWForm is not initialized, call BeginPostRequest() First." ) ;
			return ;
		}

		wwwForm.AddField( fieldName , fieldValue ) ;
	}

	public void 		AddBinary ( string fieldName , byte[] content , string fileName = null )
	{
		if ( wwwForm == null )
		{
			//ExceptionManager.Instance().LogError( "WWWForm is not initialized, call BeginPostRequest() First." ) ;
			return ;
		}

		wwwForm.AddBinaryData( fieldName , content , fileName , null ) ;
	}

	public void 		PostRequest ( string url , deleHttpRequest deleFunc )
	{
		if ( wwwForm == null )
		{
			//ExceptionManager.Instance().LogError( "WWWForm is not initialized, call BeginPostRequest() First." ) ;
			return ;
		}

		WWW www = new WWW( url , wwwForm ) ;
		//ExceptionManager.Instance ().LogInfo ( www.url );
		StartCoroutine( WaitForPostRequest( www , deleFunc ) ) ;
	}

	IEnumerator 		WaitForGetRequest ( WWW www , deleHttpRequest deleFunc )
	{
		yield return www ;
		if ( www.isDone == false )
		{
			//ExceptionManager.Instance().LogError( "Http Request Failed: " + www.error ) ;
			yield break ;
		}

		if ( deleFunc != null )
		{
			deleFunc ( www ) ;
		}
		else
		{
			if ( www.error != null )
			{
				//ExceptionManager.Instance().LogError( "Http Request Error: " + www.error ) ;
			}
		} 

		www.Dispose() ;
		//ExceptionManager.Instance ().LogInfo ( "Http Request Done!" );
	}

	IEnumerator 		WaitForPostRequest ( WWW www , deleHttpRequest deleFunc )
	{
		yield return www ;
		if ( www.isDone == false )
		{
            Logger.LogWarningFormat("Http Request Failed: " + www.error);
			//ExceptionManager.Instance().LogError( "Http Request Failed: " + www.error ) ;
			yield break ;
		}

		if ( deleFunc != null )
		{
			deleFunc ( www ) ;
		}
		else
		{
			if ( www.error != null )
			{
                Logger.LogWarningFormat("Http Request Failed: " + www.error);
                //ExceptionManager.Instance().LogError( "Http Request Error: " + www.error ) ;
            }
		} 

		www.Dispose() ;
		wwwForm = null ;
        Logger.LogWarningFormat("Http Request Done!");
        //ExceptionManager.Instance().LogInfo( "Http Request Done!" ) ;
    }

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
