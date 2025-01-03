using UnityEngine;
using UnityEngine.UI;

public class ComponentFPS : MonoSingleton<ComponentFPS>
{
	void Start()
	{
		GameObject.DontDestroyOnLoad(gameObject);
	}
	
	void Update () 
    {
        UpdateFPS();
    }

	public int watchFrames = 10;//10秒
	public int averageFrame = 30;
	public int lowFrameTown = 10;//(int)Global.Settings.startVel.x; //10;
	public int lowFrameBattle = 10;//(int)Global.Settings.startVel.x;//10;
	//
	public int frameCount = 0;
	public int fpsSum = 0;

    //FPS
    float updateInterval = 1.0f;
    private float accum = 0.0f;
    private float frames = 0;
    private float timeleft;
    private float fps;
    private string FPSAAA;
    [Range(0, 150)]
    public int MaxFPS;
    void UpdateFPS()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            fps = accum / frames;
            //FPSAAA = "FPS: " + fps.ToString("0.0");
            timeleft = 1.0f;
            accum = 0.0f;
            frames = 0;

			frameCount++;
			fpsSum += (int)fps;

			if (frameCount >= watchFrames)
			{
				averageFrame = (int)(fpsSum / (float)frameCount);

				frameCount = 0;
				fpsSum = 0;
			}
        }
    }

	public int GetFPS()
	{
		return (int)fps;
	}

	public int GetLastAverageFPS()
	{
		return averageFrame;
	}
}
