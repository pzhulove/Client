using System;
using System.Collections.Generic;
///////É¾³ýlinq
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using UnityEngine;

class NetSocket
{
    public delegate void NetWorkStatusCallback(bool isDone, string errInfo);
    public delegate void NetWorkReceiveCallback(bool isDone, int bytesRead, string errInfo);

    public bool Connect(string addr, int port, int MaxtimeOut, NetWorkStatusCallback cb = null)
    {
        return true;
    }
}
