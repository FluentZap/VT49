using System;
using System.Threading;
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

    public void ReadFromPanel()
    {

    }
  }


  struct PanelPacket
  {
    public ListOf_Panels Panel;
    public byte[] Data;
    public PanelPacket(ListOf_Panels panel, byte[] data)
    {
      Panel= panel;
      Data = data;
    }
  }

  public class VTSerial
  {
    SWSimulation _sws;
    Dictionary<ListOf_Panels, PanelConnection> sCon = new System.Collections.Generic.Dictionary<ListOf_Panels, PanelConnection>();
    List<PanelPacket> PacketQueue = new List<PanelPacket>();

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
          PanelConnection con = new PanelConnection(newConnection, packetSize);
          Thread thread = new Thread(new ThreadStart(con.ReadFromPanel));          
          sCon.Add(panel, con);

          thread.Start();
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
              Decode_CenterAnalog(buffer);
              break;
            case ListOf_Panels.Center:
              Decode_Center(buffer);
              break;
          }
        }
      }
    }

    void Decode_CenterAnalog(byte[] buffer)
    {
      _sws.ConsoleAnalogValue[0] = buffer[2];
      _sws.ConsoleAnalogValue[1] = buffer[1];
      _sws.ConsoleAnalogValue[2] = buffer[0];
      _sws.ConsoleAnalogValue[3] = buffer[3];
    }

    void Decode_Center(byte[] buffer)
    {
      if (buffer[0] == 1)
      {
        ConsoleInput c = _sws.ConsoleControls;
        byte
        DoubleTog = buffer[1],
        LEDTog = buffer[2],
        TopTog = buffer[3],
        LEDButton = buffer[4],
        LeftBoxTog = buffer[5],
        RightBoxTog = buffer[6],
        FlightStick = buffer[7];
        
        c.Set(ListOf_ConsoleInputs.DoubleTog1_UP, BitCheck(DoubleTog, 0));
        c.Set(ListOf_ConsoleInputs.DoubleTog1_DOWN, BitCheck(DoubleTog, 1));
        c.Set(ListOf_ConsoleInputs.DoubleTog2_UP, BitCheck(DoubleTog, 2));
        c.Set(ListOf_ConsoleInputs.DoubleTog2_DOWN, BitCheck(DoubleTog, 3));
        c.Set(ListOf_ConsoleInputs.DoubleTog3_UP, BitCheck(DoubleTog, 4));
        c.Set(ListOf_ConsoleInputs.DoubleTog3_DOWN, BitCheck(DoubleTog, 5));
        c.Set(ListOf_ConsoleInputs.DoubleTog4_UP, BitCheck(DoubleTog, 6));
        c.Set(ListOf_ConsoleInputs.DoubleTog4_DOWN, BitCheck(DoubleTog, 7));

        c.Set(ListOf_ConsoleInputs.LEDToggle1, BitCheck(LEDTog, 0));
        c.Set(ListOf_ConsoleInputs.LEDToggle2, BitCheck(LEDTog, 1));
        c.Set(ListOf_ConsoleInputs.LEDToggle3, BitCheck(LEDTog, 2));
        c.Set(ListOf_ConsoleInputs.LEDToggle4, BitCheck(LEDTog, 3));
        c.Set(ListOf_ConsoleInputs.LEDToggle5, BitCheck(LEDTog, 4));

        c.Set(ListOf_ConsoleInputs.TopLeftToggle1, BitCheck(TopTog, 0));
        c.Set(ListOf_ConsoleInputs.TopLeftToggle2, BitCheck(TopTog, 1));
        c.Set(ListOf_ConsoleInputs.TopRightToggle1, BitCheck(TopTog, 2));
        c.Set(ListOf_ConsoleInputs.TopRightToggle2, BitCheck(TopTog, 3));

        c.Set(ListOf_ConsoleInputs.PotButton1, BitCheck(TopTog, 4));
        c.Set(ListOf_ConsoleInputs.PotButton2, BitCheck(TopTog, 5));

        c.Set(ListOf_ConsoleInputs.LEDButton1, BitCheck(LEDButton, 0));
        c.Set(ListOf_ConsoleInputs.LEDButton2, BitCheck(LEDButton, 1));
        c.Set(ListOf_ConsoleInputs.LEDButton3, BitCheck(LEDButton, 2));
        c.Set(ListOf_ConsoleInputs.LEDButton4, BitCheck(LEDButton, 3));

        c.Set(ListOf_ConsoleInputs.LeftBoxTog1, BitCheck(LeftBoxTog, 0));
        c.Set(ListOf_ConsoleInputs.LeftBoxTog2, BitCheck(LeftBoxTog, 1));
        c.Set(ListOf_ConsoleInputs.LeftBoxTog3, BitCheck(LeftBoxTog, 2));
        c.Set(ListOf_ConsoleInputs.LeftBoxTog4, BitCheck(LeftBoxTog, 3));
        c.Set(ListOf_ConsoleInputs.LeftBoxTog5, BitCheck(LeftBoxTog, 4));
        c.Set(ListOf_ConsoleInputs.LeftBoxTog6, BitCheck(LeftBoxTog, 5));
        c.Set(ListOf_ConsoleInputs.LeftBoxTog7, BitCheck(LeftBoxTog, 6));
        c.Set(ListOf_ConsoleInputs.LeftBoxTog8, BitCheck(LeftBoxTog, 7));

        c.Set(ListOf_ConsoleInputs.RightBoxTog1, BitCheck(RightBoxTog, 0));
        c.Set(ListOf_ConsoleInputs.RightBoxTog2, BitCheck(RightBoxTog, 1));
        c.Set(ListOf_ConsoleInputs.RightBoxTog3, BitCheck(RightBoxTog, 2));
        c.Set(ListOf_ConsoleInputs.RightBoxTog4, BitCheck(RightBoxTog, 3));
        c.Set(ListOf_ConsoleInputs.RightBoxTog5, BitCheck(RightBoxTog, 4));
        c.Set(ListOf_ConsoleInputs.RightBoxTog6, BitCheck(RightBoxTog, 5));
        c.Set(ListOf_ConsoleInputs.RightBoxTog7, BitCheck(RightBoxTog, 6));
        c.Set(ListOf_ConsoleInputs.RightBoxTog8, BitCheck(RightBoxTog, 7));

        c.Set(ListOf_ConsoleInputs.FlightStickUP, BitCheck(FlightStick, 0));
        c.Set(ListOf_ConsoleInputs.FlightStickDOWN, BitCheck(FlightStick, 1));
        c.Set(ListOf_ConsoleInputs.FlightStickLEFT, BitCheck(FlightStick, 2));
        c.Set(ListOf_ConsoleInputs.FlightStickRIGHT, BitCheck(FlightStick, 3));
        
        System.Console.WriteLine(c.IsDown(ListOf_ConsoleInputs.FlightStickUP));        
      }

      if (buffer[0] == 2)
      {
        for (int x = 0; x < 15; x++)
        {
          _sws.CylinderCode[x] = buffer[x + 1];
        }
      }      
    }

    bool BitCheck(byte b, int pos)
    {
      return ((b & (1 << pos)) != 0);
    }

    public void ReadPanel(ListOf_Panels panel)
    {

    }



    static byte[] ReadAvailable(PanelConnection con)
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
