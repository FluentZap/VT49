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


    async public void Update()
    {
      int floatsize = sizeof(float);
      byte[] data = new byte[floatsize * 7];

      if (client == null || !client.Client.Connected)
      {
        client = await server.AcceptTcpClientAsync();
      }

      if (client != null && client.Client.Connected)
      {
        var stream = client.GetStream();
        byte[] x = BitConverter.GetBytes(_sws.PCShip.Location.X);
        byte[] y = BitConverter.GetBytes(_sws.PCShip.Location.Y);
        byte[] z = BitConverter.GetBytes(_sws.PCShip.Location.Z);
        byte[] qX = BitConverter.GetBytes(_sws.PCShip.Rotation.X);
        byte[] qY = BitConverter.GetBytes(_sws.PCShip.Rotation.Y);
        byte[] qZ = BitConverter.GetBytes(_sws.PCShip.Rotation.Z);
        byte[] qW = BitConverter.GetBytes(_sws.PCShip.Rotation.W);

        for (int i = 0; i != sizeof(float); i++)
        {
          data[i + floatsize * 0] = x[i];
          data[i + floatsize * 1] = y[i];
          data[i + floatsize * 2] = z[i];
          
          data[i + floatsize * 3] = qX[i];
          data[i + floatsize * 4] = qY[i];
          data[i + floatsize * 5] = qZ[i];
          data[i + floatsize * 6] = qW[i];
        }
        try {
          await stream.WriteAsync(data, 0, floatsize * 7);        
        }
        catch (System.IO.IOException exception)
        {
          System.Console.WriteLine(exception);
        }
      }
    }
  }
}