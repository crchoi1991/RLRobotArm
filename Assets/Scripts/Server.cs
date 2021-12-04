using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;

public class Server : MonoBehaviour
{
    TcpClient client;
    Motor r1, r2, r3;
    VisionCam visionCam;
    void Awake()
    {
        var thread = new Thread(new ThreadStart(ListenProc));
        thread.IsBackground = true;
        thread.Start();
    }

	private void Start()
	{
        r1 = GameObject.Find("R1").GetComponent<Motor>();
        r1.Rotate(40.0f);
        r2 = GameObject.Find("R2").GetComponent<Motor>();
        r2.Rotate(40.0f);
        r3 = GameObject.Find("R3").GetComponent<Motor>();
        r3.Rotate(70.0f);
        visionCam = GameObject.FindObjectOfType<VisionCam>();
	}

	void ListenProc()
    {
        try
        {
            var listner = new TcpListener(IPAddress.Any, 9187);
            listner.Start();
            while(true)
            {
                client = listner.AcceptTcpClient();
                Debug.LogFormat("New connection from {0}", client.ToString());
                var thread = new Thread(new ThreadStart(RecvProc));
                thread.IsBackground = true;
                thread.Start();
            }
        }
        catch(SocketException e)
        {
            Debug.LogErrorFormat("Socket Exception : {0}", e.ToString());
        }
    }

    void RecvProc()
    {
        try
        {
            var stream = client.GetStream();
            byte[] lb = new byte[4];
            while(true)
            {
                //  Read header
                stream.Read(lb, 0, 4);
                var str = Encoding.UTF8.GetString(lb);
                Debug.LogFormat("Incomming : {0}", str);
                //  Process message
                var len = int.Parse(str);
                byte[] bytes = new byte[len];
                stream.Read(bytes, 0, len);
                str = Encoding.UTF8.GetString(bytes);
                Debug.LogFormat("Message : {0}", str);
                var ss = str.Split();
                if(ss[0].Equals("r1"))
                    r1.Rotate(float.Parse(ss[1]));
                else if(ss[0].Equals("r2"))
                    r2.Rotate(float.Parse(ss[1]));
                else if(ss[0].Equals("r3"))
                    r3.Rotate(float.Parse(ss[1]));
                len = visionCam.camImage.Length;
                var slen = string.Format("{0:04}", len*6);
                stream.Write(Encoding.UTF8.GetBytes(slen), 0, 4);
                string img = "";
                for(int i = 0; i < len; i++)
                {
                    Color32 c = visionCam.camImage[i];
                    img += string.Format("{0:x}{1:x}{2:x}", c.r, c.g, c.b);
                }
                stream.Write(Encoding.UTF8.GetBytes(img), 0, len*2);
            }
        }
        catch(SocketException e)
        {
            Debug.LogErrorFormat("Socket Exception : {0}", e.ToString());
        }
    }
}
