using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlFrame : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> image_list = new List<Sprite>();
	public GameObject go;
	public void Play()
    {
		if (go != null) {
			var fm = go.GetComponent<FramesAnimation>();
			if(fm.image_list != null)
				fm.AnimPlay(image_list, true);
		}
    }
}
