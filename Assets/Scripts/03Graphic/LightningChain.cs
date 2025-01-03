using UnityEngine;
using System.Collections;
 
public class LightningChain : MonoBehaviour {
    public GameObject target;
    private LineRenderer lineRend;
    public float arcLength = 1.0f;
	public float arcVariation = 1.0f;
	public float inaccuracy = 0.5f;
	public float timeOfZap = 0.25f * 1000000000;
    private float zapTimer;
    //private LightningTrace lightTrace;
    public Vector3 offect = Vector3.zero;
 	
	float timeAcc = 0f;
	public float interval = 1.0f;

    void Start () {
        lineRend = gameObject.GetComponent<LineRenderer> ();
        zapTimer = 0;
        lineRend.SetVertexCount (1);

        zapTimer = timeOfZap;

		timeAcc = interval;
        //lightTrace = gameObject.GetComponent <LightningTrace> ();
    }
 	


    void Update() {
		timeAcc += Time.deltaTime;
		if (timeAcc >= interval)
		{
			timeAcc -= interval;

		}
		else {
			return;
		}


        if (zapTimer > 0 && target != null) {
            //UnityEngine.Debug.Log("Update!!!");
                        Vector3 lastPoint = transform.position;
                        int i = 1;
                        lineRend.SetPosition (0, transform.position + offect);//make the origin of the LR the same as the transform
                       /* while (Vector3.Distance(target.transform.position, lastPoint) > 3f) {//was the last arc not touching the target?
                                lineRend.SetVertexCount (i + 1);//then we need a new vertex in our line renderer
                                Vector3 fwd = target.transform.position - lastPoint;//gives the direction to our target from the end of the last arc
                                fwd.Normalize ();//makes the direction to scale
                                //fwd = Randomize (fwd, inaccuracy);//we don't want a straight line to the target though
								//if (arcLength != 0)
                                //	fwd *= Random.Range (arcLength * arcVariation, arcLength);//nature is never too uniform
                                fwd += lastPoint;//point + distance * direction = new point. this is where our new arc ends
                                lineRend.SetPosition (i, fwd);//this tells the line renderer where to draw to
                                i++;
                                lastPoint = fwd;//so we know where we are starting from for the next arc
                        }*/
                        lineRend.SetVertexCount (i + 1);
                        lineRend.SetPosition (i, target.transform.position + offect);
                        //lightTrace.TraceLight (gameObject.transform.position, target.transform.position);
						zapTimer = zapTimer - interval;
                } 
		else
		{
			if (lineRend != null)
				lineRend.SetVertexCount (1);
		}
			
 
    }
 
    private Vector3 Randomize (Vector3 newVector, float devation) {
        newVector += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * devation;
        newVector.Normalize();
        return newVector;
    }
 
    public void ZapTarget( GameObject newTarget){
        //print ("zap called");
        target = newTarget;
        zapTimer = timeOfZap;
    }

	public void SetVertexCount(int count)
	{
		//lineRend.SetVertexCount (count);
	}

	public void ForceUpdate()
	{
		timeAcc = interval;
		Update();
	}
}
