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
    public Thread processThread;
    public bool quit = false;
    public SerialPort Port;
    public byte[] Buffer = new byte[32];
    public int PacketSize = 0;
    public int Index = 0;
    ListOf_Panels Panel;
    Queue<PanelPacket> Queue;

    public PanelConnection(ListOf_Panels panel, SerialPort port, int packetSize, ref Queue<PanelPacket> queue)
    {
      Panel = panel;
      PacketSize = packetSize;
      Port = port;
      Queue = queue;
    }

    public void ReadFromPanel()
    {
      while (Queue != null && !quit)
      {
        byte[] buffer = VTSerial.ReadAvailable(this);
        if (buffer.Length > 0)
        {
          Queue.Enqueue(new PanelPacket(Panel, buffer));
        }
      }
    }
  }


  public struct PanelPacket
  {
    public ListOf_Panels Panel;
    public byte[] Data;
    public PanelPacket(ListOf_Panels panel, byte[] data)
    {
      Panel = panel;
      Data = data;
    }
  }

  public class VTSerial : IDisposable
  {
    SWSimulation _sws;
    Dictionary<ListOf_Panels, PanelConnection> sCon = new System.Collections.Generic.Dictionary<ListOf_Panels, PanelConnection>();
    Queue<PanelPacket> PacketQueue = new Queue<PanelPacket>();

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
          PanelConnection con = new PanelConnection(panel, newConnection, packetSize, ref this.PacketQueue);
          con.processThread = new Thread(new ThreadStart(con.ReadFromPanel));
          con.processThread.Start();
          sCon.Add(panel, con);
          return true;
        }
      }
      return false;
    }

    public void Update()
    {
      while (PacketQueue.Count > 0)
      {
        PanelPacket packet = PacketQueue.Dequeue();
        switch (packet.Panel)
        {
          case ListOf_Panels.CenterAnalog:
            Decode_CenterAnalog(packet.Data);
            break;
          case ListOf_Panels.Center:
            Decode_Center(packet.Data);
            break;
          case ListOf_Panels.LeftAnalog:
            Decode_LeftAnalog(packet.Data);
            break;
          case ListOf_Panels.RightAnalog:
            Decode_RightAnalog(packet.Data);
            break;
        }
      }

      // Send_Center();
    }

    public void Send_Center()
    {
      byte[] sendBuffer = new byte[16];
      sendBuffer[0] = 1;
      if (true) sendBuffer[1] |= 0x1 << 0;
      if (true) sendBuffer[1] |= 0x1 << 1;
      if (true) sendBuffer[1] |= 0x1 << 2;
      if (true) sendBuffer[1] |= 0x1 << 3;

      if (true) sendBuffer[1] |= 0x1 << 4;

      for (int x = 0; x < 50; x++)
      {
        if (true)
        {
          int val = sendBuffer[2 + (x / 8)];
          sendBuffer[2 + (x / 8)] = (byte)(val |= 0x1 << (x % 8));
          // sendBuffer[2 + (x / 8)] |= 0x1 << (x % 8);
          // sendBuffer[2 + (x / 8)] = 255;
        }
      }

      sendBuffer[9] = _sws.ConsoleAnalogValue[0];
      sendBuffer[10] = _sws.ConsoleAnalogValue[1];
      sendBuffer[11] = _sws.ConsoleAnalogValue[2];

      sendBuffer[12] = 30;
      sendBuffer[13] = 0;
      sendBuffer[14] = 0;

      sendBuffer[15] = _sws.ConsoleAnalogValue[0];
      byte[] encodedBuffer = new byte[255];

      var size = COBS.cobs_encode(ref sendBuffer, 16, ref encodedBuffer);
      encodedBuffer[size] = 0;
      sCon[ListOf_Panels.Center].Port.Write(encodedBuffer, 0, size + 1);
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
        var c = _sws.ConsoleInput;
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

        // System.Console.WriteLine(c.IsDown(ListOf_ConsoleInputs.FlightStickUP));
      }

      if (buffer[0] == 2)
      {
        for (int x = 0; x < 15; x++)
        {
          _sws.CylinderCode[x] = buffer[x + 1];
        }
      }
    }


    void Decode_Side(byte[] buffer)
    {
      if (buffer[0] == 1)
      {
        var c = _sws.ConsoleInput;
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

        // System.Console.WriteLine(c.IsDown(ListOf_ConsoleInputs.FlightStickUP));
      }

      if (buffer[0] == 2)
      {
        for (int x = 0; x < 15; x++)
        {
          _sws.CylinderCode[x] = buffer[x + 1];
        }
      }
    }


    void Decode_LeftAnalog(byte[] buffer)
    {
      _sws.LeftInput.analogInputRaw[0] = buffer[5];
      _sws.LeftInput.analogInputRaw[1] = buffer[4];
      _sws.LeftInput.analogInputRaw[2] = buffer[3];
      _sws.LeftInput.analogInputRaw[3] = buffer[2];
      _sws.LeftInput.analogInputRaw[4] = buffer[1];
      _sws.LeftInput.analogInputRaw[5] = buffer[0];      
    }

    void Decode_RightAnalog(byte[] buffer)
    {
      _sws.RightInput.analogInputRaw[0] = buffer[1];
      _sws.RightInput.analogInputRaw[1] = buffer[2];
      _sws.RightInput.analogInputRaw[2] = buffer[3];
      _sws.RightInput.analogInputRaw[3] = buffer[4];
      _sws.RightInput.analogInputRaw[4] = buffer[5];
      _sws.RightInput.analogInputRaw[5] = buffer[0];
    }

    static bool BitCheck(byte b, int pos)
    {
      return ((b & (1 << pos)) != 0);
    }

    public static byte[] ReadAvailable(PanelConnection con)
    {
      do
      {
        byte data = (byte)con.Port.ReadByte();
        if (data == 0)
        {
          byte[] decodeBuffer = new byte[255];
          int decodedLength = COBS.cobs_decode(ref con.Buffer, con.Index, ref decodeBuffer);

          if (decodedLength == con.PacketSize)
          {
            con.Index = 0;
            return decodeBuffer;
          }
          else
          {
            con.Index = 0;
          }
        }
        else
        {
          if (con.Index + 1 < 255)
          {
            con.Buffer[con.Index++] = data;
          }
          else
          {
            con.Index = 0;
          }
        }
      } while (con.Port.BytesToRead > 0);
      return new byte[0];
    }

    public void Dispose()
    {
      foreach (PanelConnection con in sCon.Values)
      {
        con.quit = true;
      }
    }
  }
}
