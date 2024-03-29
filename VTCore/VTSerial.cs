using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.IO.Ports;
using Force.Crc32;
using Consistent_Overhead_Byte_Stuffing;

namespace VT49
{
  public enum ListOf_Panels
  {
    None,
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
    public byte[] Buffer = new byte[257];
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

    public void ReadFromPanel(object sender, SerialDataReceivedEventArgs e)
    {
      // while (Queue != null && !quit)
      // {
      // Thread.Sleep(10);
      while (Port.IsOpen && Port.BytesToRead > 0)
      {
        byte[] buffer = VTSerial.ReadAvailable(this);
        if (buffer.Length > 0)
        {
          Queue.Enqueue(new PanelPacket(Panel, buffer));
        }
      }
      // Thread.Sleep(16);
      // }
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
    Thread sendThread;
    public bool quit = false;
    public bool sendUpdate = false;
    public Dictionary<ListOf_Panels, PanelConnection> sCon = new System.Collections.Generic.Dictionary<ListOf_Panels, PanelConnection>();
    Queue<PanelPacket> PacketQueue = new Queue<PanelPacket>();

    public VTSerial(ref SWSimulation sws)
    {
      _sws = sws;
      // sendThread = new Thread(new ThreadStart(SendToPanels));
      // sendThread.Start();
    }

    public bool StartConnection(ListOf_Panels panel, string port, int baud, int packetSize)
    {
      if (!sCon.ContainsKey(panel))
      {
        SerialPort newConnection = new SerialPort(port, baud);
        newConnection.DtrEnable = true;
        newConnection.RtsEnable = true;
        newConnection.Parity = Parity.None;
        newConnection.StopBits = StopBits.One;
        newConnection.DataBits = 8;
        newConnection.Handshake = Handshake.None;
        newConnection.ReadTimeout = 500;
        newConnection.WriteTimeout = 500;

        PanelConnection con = new PanelConnection(panel, newConnection, packetSize, ref this.PacketQueue);
        con.Port.DataReceived += new SerialDataReceivedEventHandler(con.ReadFromPanel);
        newConnection.Open();
        sCon.Add(panel, con);
        return true;
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
            _sws.SPSReceive_ticks[1]++;
            break;
          case ListOf_Panels.Center:
            Decode_Center(packet.Data);
            _sws.SPSReceive_ticks[0]++;
            break;
          case ListOf_Panels.LeftAnalog:
            Decode_LeftAnalog(packet.Data);
            _sws.SPSReceive_ticks[3]++;
            break;
          case ListOf_Panels.RightAnalog:
            Decode_RightAnalog(packet.Data);
            _sws.SPSReceive_ticks[5]++;
            break;
          case ListOf_Panels.Left:
            Decode_Side(packet.Data, _sws.LeftInput);
            _sws.SPSReceive_ticks[2]++;
            break;
          case ListOf_Panels.Right:
            Decode_Side(packet.Data, _sws.RightInput);
            _sws.SPSReceive_ticks[4]++;
            break;
        }
      }

      // Send_Center();
      // Send_Side(_sws.RightInput, ListOf_Panels.Right);
      // Send_Side(_sws.RightInput, ListOf_Panels.Right);
    }

    public void SendToPanels()
    {
      // while (!quit)
      // {
      // if (sendUpdate == true)
      // {

      if (sCon.ContainsKey(ListOf_Panels.Right) && sCon[ListOf_Panels.Right].Port.IsOpen)
      {
        Send_Side(_sws.RightInput, ListOf_Panels.Right, _sws.SideHeader);
      }
      if (sCon.ContainsKey(ListOf_Panels.Left) && sCon[ListOf_Panels.Left].Port.IsOpen)
      {
        Send_Side(_sws.LeftInput, ListOf_Panels.Left, _sws.SideHeader);
      }
      if (sCon.ContainsKey(ListOf_Panels.Center) && sCon[ListOf_Panels.Center].Port.IsOpen)
      {
        Send_Center();
      }

      _sws.SPSSend_ticks[0]++;
      _sws.SideHeader++;
      if (_sws.SideHeader == 3)
      {
        _sws.SideHeader = 10;
      }

      if (_sws.SideHeader > 10)
      {
        _sws.SideHeader = 0;
      }


    }

    public void Send_Center()
    {
      if (sCon[ListOf_Panels.Center].Port.IsOpen)
      {
        ConsoleControl C = _sws.ConsoleInput;
        byte[] sendBuffer = new byte[24];
        if (_sws.ConsoleInput.CylinderLogin)
        {
          sendBuffer[0] = 2;
          Crc32Algorithm.ComputeAndWriteToEnd(sendBuffer);
          _sws.ConsoleInput.CylinderLogin = false;
        }
        else
        {
          sendBuffer[0] = 1;
          if (C.LEDs.IsOn(ListOf_ConsoleOutputs.ControlLED1)) sendBuffer[1] |= 0x1 << 0;
          if (C.LEDs.IsOn(ListOf_ConsoleOutputs.ControlLED2)) sendBuffer[1] |= 0x1 << 1;
          if (C.LEDs.IsOn(ListOf_ConsoleOutputs.ControlLED3)) sendBuffer[1] |= 0x1 << 2;
          if (C.LEDs.IsOn(ListOf_ConsoleOutputs.ControlLED4)) sendBuffer[1] |= 0x1 << 3;
          if (C.LEDs.IsOn(ListOf_ConsoleOutputs.FlightStickLED)) sendBuffer[1] |= 0x1 << 4;

          sendBuffer[2] = C.rgbLed.ColorIndex[0].R;
          sendBuffer[3] = C.rgbLed.ColorIndex[0].G;
          sendBuffer[4] = C.rgbLed.ColorIndex[0].B;

          sendBuffer[5] = C.rgbLed.ColorIndex[1].R;
          sendBuffer[6] = C.rgbLed.ColorIndex[1].G;
          sendBuffer[7] = C.rgbLed.ColorIndex[1].B;

          for (int x = 0; x < 50; x++)
          {
            byte[] b = _sws.ConsoleInput.rgbLed.GetLeds();
            if (b[x] == 1)
            {
              int val = sendBuffer[8 + (x / 8)];
              sendBuffer[8 + (x / 8)] = (byte)(val |= 0x1 << (x % 8));
            }

          }
          sendBuffer[16] = C.Target;
          sendBuffer[17] = 0;
          sendBuffer[18] = 0;
          sendBuffer[19] = 0;
        }

        Crc32Algorithm.ComputeAndWriteToEnd(sendBuffer);

        byte[] encodedBuffer = new byte[255];

        var size = COBS.cobs_encode(ref sendBuffer, 24, ref encodedBuffer);
        encodedBuffer[size] = 0;
        sCon[ListOf_Panels.Center].Port.Write(encodedBuffer, 0, size + 1);

      }
    }

    public void Send_Side(SideControl side, ListOf_Panels panel, byte header)
    {
      if (sCon[panel].Port.IsOpen)
      {
        byte[] sendBuffer = new byte[40];
        sendBuffer[0] = header;
        lock (side)
        {
          if (side.LEDs.IsOn(ListOf_SideOutputs.ThrottleLED1)) sendBuffer[1] |= 0x1 << 0;
          if (side.LEDs.IsOn(ListOf_SideOutputs.ThrottleLED2)) sendBuffer[1] |= 0x1 << 1;
          if (side.LEDs.IsOn(ListOf_SideOutputs.ThrottleLED3)) sendBuffer[1] |= 0x1 << 2;
          if (side.LEDs.IsOn(ListOf_SideOutputs.ThrottleLEDToggle)) sendBuffer[1] |= 0x1 << 3;
          if (side.LEDs.IsOn(ListOf_SideOutputs.MatrixLED1)) sendBuffer[1] |= 0x1 << 4;
          if (side.LEDs.IsOn(ListOf_SideOutputs.MatrixLED2)) sendBuffer[1] |= 0x1 << 5;

          if (side.LEDs.IsOn(ListOf_SideOutputs.ControlLED1)) sendBuffer[1] |= 0x1 << 6;
          if (side.LEDs.IsOn(ListOf_SideOutputs.ControlLED2)) sendBuffer[1] |= 0x1 << 7;
          if (side.LEDs.IsOn(ListOf_SideOutputs.ControlLED3)) sendBuffer[2] |= 0x1 << 0;
          if (side.LEDs.IsOn(ListOf_SideOutputs.ControlLED4)) sendBuffer[2] |= 0x1 << 1;
          if (side.LEDs.IsOn(ListOf_SideOutputs.ControlLED5)) sendBuffer[2] |= 0x1 << 2;
          if (side.LEDs.IsOn(ListOf_SideOutputs.EightLEDToggle)) sendBuffer[2] |= 0x1 << 3;

          if (sendBuffer[0] == 1)
          {
            //Matrix Conversion
            for (int mat = 0; mat < 4; mat++)
              for (int i = 0; i < 64; i++)
              {
                int x = 0, y = 0;
                if (mat == 0)
                {
                  x = 7 - i / 8;
                  y = 7 - i % 8;
                }
                if (mat == 1)
                {
                  x = 15 - i / 8;
                  y = 7 - i % 8;
                }
                if (mat == 2)
                {
                  x = 7 - i / 8;
                  y = 15 - i % 8;
                }
                if (mat == 3)
                {
                  x = 15 - i / 8;
                  y = 15 - i % 8;
                }

                if (side.Matrix[x, y])
                {
                  int var = sendBuffer[3 + i / 8 + 8 * mat];
                  sendBuffer[3 + i / 8 + 8 * mat] = (byte)(var |= 0x1 << i % 8);
                }
              }
          }

          if (sendBuffer[0] == 2)
          {
            for (int seg = 0; seg < 2; seg++)
              for (int i = 0; i < 64; i++)
              {
                int digit = i / 8, led = i % 8;
                if (side.Seg[seg, 7 - led, 7 - digit])
                {
                  int var = sendBuffer[3 + i / 8 + 8 * seg];
                  sendBuffer[3 + i / 8 + 8 * seg] = (byte)(var |= 0x1 << i % 8);
                }
              }
          }

          if (sendBuffer[0] == 10)
          {
            sendBuffer[3] = side.rgbLed.ColorIndex[0].R;
            sendBuffer[4] = side.rgbLed.ColorIndex[0].G;
            sendBuffer[5] = side.rgbLed.ColorIndex[0].B;

            sendBuffer[6] = side.rgbLed.ColorIndex[1].R;
            sendBuffer[7] = side.rgbLed.ColorIndex[1].G;
            sendBuffer[8] = side.rgbLed.ColorIndex[1].B;

            for (int x = 0; x < 50; x++)
            {
              bool isOn = false;
              if (x >= 0 && x < 5)
              {
                isOn = side.rgbLed.TargetControlLED[x] == 1;
              }
              else if (x >= 5 && x < 30)
              {
                isOn = side.rgbLed.MatrixLED[x - 5] == 1;
              }
              else if (x >= 30 && x < 35)
              {
                isOn = side.rgbLed.MatrixGuideLED[x - 30] == 1;
              }
              else if (x >= 35 && x < 40)
              {
                isOn = side.rgbLed.MatrixControlLED[x - 35] == 1;
              }
              else if (x >= 40 && x < 45)
              {
                isOn = side.rgbLed.ThrottleLED[4 - (x - 40)] == 1;
              }
              else if (x >= 45 && x < 50)
              {
                isOn = side.rgbLed.EightControlLED[x - 45] == 1;
              }
              if (isOn)
              {
                int val = sendBuffer[9 + (x / 8)];
                sendBuffer[9 + (x / 8)] = (byte)(val |= 0x1 << (x % 8));
              }
            }
          }
        }
        Crc32Algorithm.ComputeAndWriteToEnd(sendBuffer);

        byte[] encodedBuffer = new byte[255];
        var size = COBS.cobs_encode(ref sendBuffer, 40, ref encodedBuffer);
        encodedBuffer[size] = 0;
        // sCon[panel].Port.Write(encodedBuffer, 0, size + 1);
        // sCon[panel].Port.
        sCon[panel].Port.Write(encodedBuffer, 0, size + 1);
      }
    }

    void Decode_CenterAnalog(byte[] buffer)
    {
      if (Crc32Algorithm.IsValidWithCrcAtEnd(buffer, 0, 8))
      {
        _sws.ConsoleInput.analogInputRaw[0] = buffer[2];
        _sws.ConsoleInput.analogInputRaw[1] = buffer[1];
        _sws.ConsoleInput.analogInputRaw[2] = buffer[0];
        _sws.ConsoleInput.analogInputRaw[3] = buffer[3];
      }
    }

    void Decode_Center(byte[] buffer)
    {
      if (Crc32Algorithm.IsValidWithCrcAtEnd(buffer, 0, 13))
      {
        if (buffer[0] == 1)
        {
          var c = _sws.ConsoleInput.Buttons;
          byte
          DoubleTog = buffer[1],
          LEDTog = buffer[2],
          TopTog = buffer[3],
          LEDButton = buffer[4],
          LeftBoxTog = buffer[5],
          RightBoxTog = buffer[6];

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

          c.Set(ListOf_ConsoleInputs.ControlLED1, BitCheck(LEDButton, 0));
          c.Set(ListOf_ConsoleInputs.ControlLED2, BitCheck(LEDButton, 1));
          c.Set(ListOf_ConsoleInputs.ControlLED3, BitCheck(LEDButton, 2));
          c.Set(ListOf_ConsoleInputs.ControlLED4, BitCheck(LEDButton, 3));

          c.Set(ListOf_ConsoleInputs.FlightStickUP, BitCheck(LEDButton, 4));
          c.Set(ListOf_ConsoleInputs.FlightStickDOWN, BitCheck(LEDButton, 5));
          c.Set(ListOf_ConsoleInputs.FlightStickLEFT, BitCheck(LEDButton, 6));
          c.Set(ListOf_ConsoleInputs.FlightStickRIGHT, BitCheck(LEDButton, 7));

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

          for (int i = 0; i < 2; i++)
          {
            _sws.ConsoleInput.rotaryValue[i] += (SByte)buffer[7 + i];
          }
        }
        if (buffer[0] == 2)
        {
          for (int x = 0; x < 7; x++)
          {
            _sws.ConsoleInput.CylinderCode[x] = buffer[x + 1];            
          }
          _sws.PCShip.CenterControlInterface.LoginID = Encoding.UTF8.GetString(_sws.ConsoleInput.CylinderCode, 0, _sws.ConsoleInput.CylinderCode.Length);          
        }
      }
    }

    void Decode_Side(byte[] buffer, SideControl side)
    {
      // System.Console.WriteLine(Crc32Algorithm.Compute(buffer, 0, 16));
      if (Crc32Algorithm.IsValidWithCrcAtEnd(buffer, 0, 16))
      {
        if (buffer[0] == 1)
        {
          var c = side.Buttons;
          byte
          Throttle = buffer[1],
          Matrix = buffer[2],
          Target = buffer[3],
          Eight = buffer[4];

          c.Set(ListOf_SideInputs.ThrottleLEDButton1, BitCheck(Throttle, 0));
          c.Set(ListOf_SideInputs.ThrottleLEDButton2, BitCheck(Throttle, 1));
          c.Set(ListOf_SideInputs.ThrottleLEDButton3, BitCheck(Throttle, 2));
          c.Set(ListOf_SideInputs.ThrottleLEDToggle, BitCheck(Throttle, 3));
          c.Set(ListOf_SideInputs.ControlLED1, BitCheck(Throttle, 4));
          c.Set(ListOf_SideInputs.ControlLED2, BitCheck(Throttle, 5));
          c.Set(ListOf_SideInputs.ControlLED3, BitCheck(Throttle, 6));
          c.Set(ListOf_SideInputs.ControlLED4, BitCheck(Throttle, 7));

          c.Set(ListOf_SideInputs.ControlLED5, BitCheck(Matrix, 0));
          c.Set(ListOf_SideInputs.MatrixLEDButton1, BitCheck(Matrix, 1));
          c.Set(ListOf_SideInputs.MatrixLEDButton2, BitCheck(Matrix, 2));
          c.Set(ListOf_SideInputs.MatrixRotButton1, BitCheck(Matrix, 3));
          c.Set(ListOf_SideInputs.MatrixRotButton2, BitCheck(Matrix, 4));
          c.Set(ListOf_SideInputs.MatrixRotButton3, BitCheck(Matrix, 5));
          c.Set(ListOf_SideInputs.MatrixDoubleTog_Up, BitCheck(Matrix, 6));
          c.Set(ListOf_SideInputs.MatrixDoubleTog_Down, BitCheck(Matrix, 7));

          c.Set(ListOf_SideInputs.TargetRotButton1, BitCheck(Target, 0));
          c.Set(ListOf_SideInputs.TargetRotButton2, BitCheck(Target, 1));
          c.Set(ListOf_SideInputs.TargetDoubleTog_Up, BitCheck(Target, 2));
          c.Set(ListOf_SideInputs.TargetDoubleTog_Down, BitCheck(Target, 3));
          c.Set(ListOf_SideInputs.EightRotButton, BitCheck(Target, 4));
          c.Set(ListOf_SideInputs.EightLEDToggle, BitCheck(Target, 5));
          c.Set(ListOf_SideInputs.EightDoubleTog_Up, BitCheck(Target, 6));
          c.Set(ListOf_SideInputs.EightDoubleTog_Down, BitCheck(Target, 7));

          c.Set(ListOf_SideInputs.EightToggle1, BitCheck(Eight, 0));
          c.Set(ListOf_SideInputs.EightToggle2, BitCheck(Eight, 1));
          c.Set(ListOf_SideInputs.EightToggle3, BitCheck(Eight, 2));
          c.Set(ListOf_SideInputs.EightToggle4, BitCheck(Eight, 3));
          c.Set(ListOf_SideInputs.EightToggle5, BitCheck(Eight, 4));
          c.Set(ListOf_SideInputs.EightToggle6, BitCheck(Eight, 5));
          c.Set(ListOf_SideInputs.EightToggle7, BitCheck(Eight, 6));
          c.Set(ListOf_SideInputs.EightToggle8, BitCheck(Eight, 7));

          for (int i = 0; i < 6; i++)
          {
            side.rotaryValue[i] += (SByte)buffer[5 + i];
          }
        }
      }
    }

    void Decode_LeftAnalog(byte[] buffer)
    {
      if (Crc32Algorithm.IsValidWithCrcAtEnd(buffer, 0, 10))
      {
        _sws.LeftInput.analogInputRaw[0] = buffer[5];
        _sws.LeftInput.analogInputRaw[1] = buffer[4];
        _sws.LeftInput.analogInputRaw[2] = buffer[3];
        _sws.LeftInput.analogInputRaw[3] = buffer[2];
        _sws.LeftInput.analogInputRaw[4] = buffer[1];
        _sws.LeftInput.analogInputRaw[5] = buffer[0];
      }
    }

    void Decode_RightAnalog(byte[] buffer)
    {
      if (Crc32Algorithm.IsValidWithCrcAtEnd(buffer, 0, 10))
      {
        _sws.RightInput.analogInputRaw[0] = buffer[1];
        _sws.RightInput.analogInputRaw[1] = buffer[2];
        _sws.RightInput.analogInputRaw[2] = buffer[3];
        _sws.RightInput.analogInputRaw[3] = buffer[4];
        _sws.RightInput.analogInputRaw[4] = buffer[5];
        _sws.RightInput.analogInputRaw[5] = buffer[0];
      }
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
      quit = true;
      foreach (PanelConnection con in sCon.Values)
      {
        con.quit = true;
      }
    }
  }
}