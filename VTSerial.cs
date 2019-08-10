using System;
using System.Collections.Generic;
using System.IO.Ports;
using Consistent_Overhead_Byte_Stuffing;

namespace VT49
{
  public enum ListOf_Panels
  {
    Left,
    Right,
    Center,
    LeftAnalog,
    RightAnalog,
    CenterAnalog
  }

  public class PanelConnection
  {
    public SerialPort port;
    public byte[] buffer = new byte[255];
    public int packetSize = 0;
    public int index = 0;

    public PanelConnection(SerialPort port, int packetSize)
    {
      this.packetSize = packetSize;
      this.port = port;
    }
  }


  public class VTSerial
  {
    SWSimulation _sws;
    Dictionary<ListOf_Panels, PanelConnection> sCon = new System.Collections.Generic.Dictionary<ListOf_Panels, PanelConnection>();

    public VTSerial(SWSimulation sws)
    {
      _sws = sws;
    }

    public bool StartConnection(ListOf_Panels panel, string port, int baud, int packetSize)
    {
      if (!sCon.ContainsKey(panel))
      {
        SerialPort newConnection = new SerialPort(port, baud);
        newConnection.Open();
        if (newConnection.IsOpen)
        {
          sCon.Add(panel, new PanelConnection(newConnection, packetSize));
          return true;
        }
      }
      return false;
    }


    public void Update()
    {
      foreach (ListOf_Panels name in sCon.Keys)
      {        
        byte[] buffer = ReadAvailable(sCon[name]);
        if (buffer.Length > 0)
        {          
          switch (name)
          {
            case ListOf_Panels.CenterAnalog:
              _sws.ConsoleAnalogValue[0] = buffer[2];
              _sws.ConsoleAnalogValue[1] = buffer[1];
              _sws.ConsoleAnalogValue[2] = buffer[0];
              _sws.ConsoleAnalogValue[3] = buffer[3];              
              break;            
          }
        }
      }
    }


    byte[] ReadAvailable(PanelConnection con)
    {
      do
      {
        byte data = (byte)con.port.ReadByte();
        if (data == 0)
        {
          byte[] decodeBuffer = new byte[255];
          int decodedLength = COBS.cobs_decode(ref con.buffer, con.index, ref decodeBuffer);

          if (decodedLength == con.packetSize)
          {
            con.index = 0;
            return decodeBuffer;
          }
          else
          {
            con.index = 0;
          }
        }
        else
        {
          if (con.index + 1 < 255)
          {
            con.buffer[con.index++] = data;
          }
          else
          {
            con.index = 0;
          }
        }
      } while (con.port.BytesToRead > 0);
      return new byte[0];
    }
  }
}
