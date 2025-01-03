using UnityEngine;
using System.Collections;

public class ComMutexToggle : MonoBehaviour {

    public GameObject one;
    public GameObject two;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetMutexActive(bool b)
    {
        one.SetActive(b);
        two.SetActive(!b);
    }
}
