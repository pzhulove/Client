using UnityEngine;
using System.Collections;
using Network;
using Protocol;
using System;
using System.Text;

public class TestNetWork : MonoBehaviour {
    // Use this for initialization
    void Start () {
        NetManager.Instance().Init();
        NetManager.Instance().ConnectToAdmainServer("192.168.0.222", 843, 0);
    }
	
  
	// Update is called once per frame
	void Update () {
        
    }
}

public class TestNetWorkLogicServer
{
    void Start(string ip, ushort port, uint timeout)
    {
        var sw = new  System.IO.StreamWriter(Console.OpenStandardOutput());
        sw.AutoFlush = true;
        System.Console.SetOut(sw);
        System.Console.Write("Start ip123123 =" +ip + "\n"); 
        NetworkSocket cli2AdminServer = new NetworkSocket(ServerType.ADMIN_SERVER);
        cli2AdminServer.ConnectToServer(ip, port, (int)timeout);
    }

    void TestMath()
    {
        Vector3 v = new Vector3(1,1,1);
        v = v.normalized;

        float a = Mathf.Cos(1.0f);

        
    }

	void SetBuffer(System.IntPtr ptr,int len)
	{

	}
}