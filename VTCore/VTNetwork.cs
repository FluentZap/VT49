using System;
using System.Numerics;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Quat = BepuUtilities.Quaternion;

namespace VT49
{

  class PacketEncoder
  {
    byte[] Buffer;
    int index = 0;
    static int Float_S = sizeof(float);
    static int UInt16_S = sizeof(UInt16);

    public PacketEncoder(int size)
    {
      Buffer = new byte[size];
    }

    public void Write(float value)
    {
      Array.Copy(BitConverter.GetBytes(value), 0, Buffer, index, Float_S);
      index += Float_S;
    }
    public void Write(UInt16 value)
    {
      Array.Copy(BitConverter.GetBytes(value), 0, Buffer, index, UInt16_S);
      index += UInt16_S;
    }
    public void Write(byte value)
    {
      Buffer[index] = value;
      index += 1;
    }

    public void Write(Vector3 value)
    {
      Write(value.X);
      Write(value.Y);
      Write(value.Z);
    }

    public void Write(Quat value)
    {
      Write(value.X);
      Write(value.Y);
      Write(value.Z);
      Write(value.W);
    }


    public byte[] GetBuffer()
    {
      return Buffer;
    }
  }

  enum ListOf_ClientSendFlags
  {
    FirstUpdate = 0,
    ShipUpdate = 1
  }

  class ClientConnection
  {
    public TcpClient TCP;
    public ListOf_ClientSendFlags SendFlags;
  }


  public class VTNetwork
  {
    TcpListener server = null;
    // Dictionary<Guid, TcpClient> clients = new Dictionary<Guid, TcpClient>();
    Dictionary<Guid, ClientConnection> clients = new Dictionary<Guid, ClientConnection>();

    // TcpClient client = null;
    SWSimulation _sws = null;

    int MeshFirstPacketSize =
        sizeof(UInt16) +   //ObjectId
        sizeof(UInt16) +   //ObjectType
        sizeof(float) * 10; //Position Data
    
    int MeshUpdatePacketSize =
        sizeof(UInt16) +   //ObjectId        
        sizeof(float) * 7; //Position Data

    int ShipPacketSize =
        sizeof(UInt16) +   //ObjectId
        sizeof(float) * 7; //Position Data


    public VTNetwork(ref SWSimulation sws, string ip, int port)
    {
      _sws = sws;
      IPAddress localAddr = IPAddress.Parse(ip);
      server = new TcpListener(localAddr, port);
      server.Start();
    }


    byte[] BuildShipsUpdateBuffer()
    {
      List<MeshObject> UpdateList = new List<MeshObject>();
      foreach ((var key, var value) in _sws.swSystem.Objects)
      {
        if (value.PhysicsUpdated)
        {
          UpdateList.Add(value);
          value.PhysicsUpdated = false;
        }
      }
      int size = 1 + sizeof(UInt16) + ShipPacketSize + (UpdateList.Count * MeshUpdatePacketSize);
      PacketEncoder packet = new PacketEncoder(size);
      packet.Write((byte)ListOf_ClientSendFlags.ShipUpdate);
      packet.Write((UInt16)UpdateList.Count);

      packet.Write(_sws.PCShip.Id);
      packet.Write(_sws.PCShip.Location);
      packet.Write(_sws.PCShip.Rotation);
            
      foreach (var item in UpdateList)
      {
        packet.Write(item.Id);
        packet.Write(item.Location);
        packet.Write(item.Rotation);          
      }
      return packet.GetBuffer();
    }

    byte[] BuildFirstUpdateBuffer()
    {
      UInt16 count = (UInt16)_sws.swSystem.Objects.Count;
      int size = 1 +       //Header
      sizeof(UInt16) +     //Count of Objects      
      count * MeshFirstPacketSize;

      PacketEncoder packet = new PacketEncoder(size);
      packet.Write((byte)ListOf_ClientSendFlags.FirstUpdate);
      packet.Write(count);
      foreach (var item in _sws.swSystem.Objects)
      {
        packet.Write(item.Value.Id);
        packet.Write((UInt16)item.Value.CollisionMesh);
        packet.Write(item.Value.Location);
        packet.Write(item.Value.Scale);
        packet.Write(item.Value.Rotation);
      }
      return packet.GetBuffer();
    }


    async void SendShipsUpdate()
    {
      List<Guid> RemoveList = new List<Guid>();

      if (clients.Count > 0)
      {
        byte[] data = BuildShipsUpdateBuffer();
        byte[] firstData = BuildFirstUpdateBuffer();

        foreach ((var uid, var client) in clients)
        {
          if (client.TCP.Client.Connected)
          {
            var stream = client.TCP.GetStream();
            try
            {
              if (client.SendFlags == ListOf_ClientSendFlags.FirstUpdate)
              {
                await stream.WriteAsync(firstData, 0, firstData.Length);
                client.SendFlags = ListOf_ClientSendFlags.ShipUpdate;
              }

              if (client.SendFlags == ListOf_ClientSendFlags.ShipUpdate)
              {
                await stream.WriteAsync(data, 0, data.Length);
              }
            }
            catch (System.IO.IOException exception)
            {
              if (exception.HResult != -2146232800)
              {
                System.Console.WriteLine(exception);
              }
              RemoveList.Add(uid);
            }
          }
        }
        foreach (var uid in RemoveList)
        {
          clients[uid].TCP.Close();
          clients[uid].TCP.Dispose();
          System.Console.WriteLine($"Client {uid.ToString()} has disconnected");
          clients.Remove(uid);
        }
      }
    }


    async public void Update()
    {
      if (server.Pending())
      {
        ClientConnection client = new ClientConnection();
        client.TCP = await server.AcceptTcpClientAsync();
        client.SendFlags = ListOf_ClientSendFlags.FirstUpdate;
        Guid clientId = Guid.NewGuid();
        System.Console.WriteLine($"Client {clientId.ToString()} has connected");
        clients.Add(clientId, client);
      }

      SendShipsUpdate();
    }
  }
}
