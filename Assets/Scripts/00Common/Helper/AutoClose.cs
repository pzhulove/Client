using UnityEngine;
using System.Collections;

public class AutoClose : MonoBehaviour {

	public float CloseTime = 2.0f ;
	private GameObject self = null ;

	// Use this for initialization
	void Start () 
    {
        Invoke("CloseSelf", CloseTime);
        self = gameObject;
	}

    public bool SelfExist()
    {
        if (self == null)
            return false;
        else
            return true;
    }

    void CloseSelf()
	{
		if(self != null)
		{
			GameObject.Destroy( self ) ;
			self = null ;
		}
	}
}
