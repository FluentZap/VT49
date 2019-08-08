using System;
using System.Net;
using System.Net.Sockets;

namespace VT49
{
  public class VTNetwork
  {
    TcpListener server = null;
    TcpClient client = null;
    SWSimulation _sws = null;

    public VTNetwork(ref SWSimulation sws, string ip, int port)    
    {
      _sws = sws;
      IPAddress localAddr = IPAddress.Parse(ip);
      server = new TcpListener(localAddr, port);
      server.Start();      
    }


    public void Update()
    {
      int floatsize = sizeof(float);
      byte[] data = new byte[floatsize * 3];

      if (client == null)
      {
        client = server.AcceptTcpClient();
      }      

      if (client != null)
      {
        var stream = client.GetStream();
        byte[] x = BitConverter.GetBytes(_sws.PCShip.x);
        byte[] y = BitConverter.GetBytes(_sws.PCShip.y);
        byte[] z = BitConverter.GetBytes(_sws.PCShip.z);

        for (int i = 0; i != sizeof(float); i++)
        {
          data[i + floatsize * 0] = x[i];
          data[i + floatsize * 1] = y[i];
          data[i + floatsize * 2] = z[i];
        }
        stream.Write(data, 0, floatsize * 3);
      }
    }
  }
}