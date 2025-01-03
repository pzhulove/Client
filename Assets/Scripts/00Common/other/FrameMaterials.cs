/*
 * FrameMaterials
 * Create by Fengyueyun
 * Date 2013.01.25
 */

using UnityEngine;
using System.Collections;

public class FrameMaterials : MonoBehaviour 
{
    public float StartTime = 0;
    public int TileX = 1;
	public int TileY=1;
	public int FPS=12;
	public int StartFrame=0;
	public bool Loop=true;
	
	float XTiling,YTiling,XOffset,YOffset;
	float UpdateTime;
	float TimeFlg;
	int index=0;
	Material UseMaterial;
	// Use this for initialization
	public void Start () 
	{
		index=StartFrame;
		UpdateTime=1.0f/FPS;
		UseMaterial=GetComponent<Renderer>().material;
		XTiling=1.0f/TileX;
		YTiling=1.0f/TileY;
		TimeFlg=Time.time+StartTime;
		
		UseMaterial.SetTextureScale("_MainTex", new Vector2(XTiling, YTiling));
        XOffset = 0.0f;
        YOffset = (TileY - 1) * YTiling;
        UseMaterial.SetTextureOffset("_MainTex",new Vector2(XOffset,YOffset));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time-TimeFlg>UpdateTime)
		{
            index++;
            if (index >= TileX * TileY)
            {
                if (Loop)
                {
                    index = 0;
                }
                else
                {
                    index = TileX * TileY - 1;
                }
            }

            XOffset = (index % TileX) * XTiling;
            YOffset = (TileY - 1 - (index / TileX)) * YTiling;

			TimeFlg=Time.time;
			UseMaterial.SetTextureOffset("_MainTex",new Vector2(XOffset,YOffset));
		}
	}
	
    public float Duration()
    {
        UpdateTime = 1.0f / FPS;
        float time = StartTime + (TileX * TileY) * UpdateTime;
        return time;
    }

	public void Sampler(float time)
    {
        if(UseMaterial == null)
        {
            UpdateTime = 1.0f / FPS;
            UseMaterial = GetComponent<Renderer>().sharedMaterial;
            XTiling = 1.0f / TileX;
            YTiling = 1.0f / TileY;
            if(UseMaterial)
            UseMaterial.SetTextureScale("_MainTex", new Vector2(XTiling, YTiling));
        }

        if(UseMaterial)
        {
            if(time <= StartTime)
            {
                index = StartFrame;
                XOffset = 0.0f;
                YOffset = (TileY - 1) * YTiling;
            }
            else
            {
                float timepass = time - StartTime;
                timepass /= UpdateTime;
                index = Mathf.FloorToInt(timepass);
                
                if (index >= TileX * TileY)
                {
                    if (Loop)
                    {
                        index = 0;
                    }
                    else
                    {
                        index = TileX * TileY - 1;
                    }
                }

                XOffset = (index % TileX) * XTiling;
                YOffset = (TileY - 1 - (index / TileX)) * YTiling;

                TimeFlg = time;
                UseMaterial.SetTextureOffset("_MainTex", new Vector2(XOffset, YOffset));
            }
        }

    }
}
