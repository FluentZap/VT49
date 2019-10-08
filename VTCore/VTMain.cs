using System;
using Force.Crc32;
using System.Drawing;
using System.IO.Ports;
using System.Collections.Generic;
using Consistent_Overhead_Byte_Stuffing;

using static SDL2.SDL;

namespace VT49
{
  class VTMain : IDisposable
  {
    // 900 x 1440
    const int SCREEN_WIDTH = 900, SCREEN_HEIGHT = 1440, SCREEN_FPS = 60;
    const double SCREEN_TICKS_PER_FRAME = 1000 / SCREEN_FPS;
    const double SERIAL_TICKS_PER_FRAME = 1000 / 60;
    bool quit = false;
    long fpsTicks, fpsStart, spsTicks, spsStart;

    SWSimulation _sws;
    VTRender _render;
    VTNetwork _network;
    VTPhysics _physics;
    VTSerial _serial;
    VTController _controller;
    VTSimulation _simulation;
    VTHolocron _holocron;

    public void Start()
    {
      if (Init())
      {
        int fps = 0;
        int sps = 0;
        fpsTicks = SDL_GetTicks();
        spsTicks = SDL_GetTicks();
        SDL_Event e;
        while (!quit)
        {
          while (SDL_PollEvent(out e) != 0)
          {
            switch (e.type)
            {
              case SDL_EventType.SDL_QUIT:
                quit = true;
                break;
              case SDL_EventType.SDL_KEYDOWN:
                switch (e.key.keysym.sym)
                {
                  case SDL_Keycode.SDLK_ESCAPE:
                    quit = true;
                    break;
                  case SDL_Keycode.SDLK_LEFT:
                    _sws.PCShip.Left = true;
                    break;
                  case SDL_Keycode.SDLK_RIGHT:
                    _sws.PCShip.Right = true;
                    break;
                  case SDL_Keycode.SDLK_UP:
                    _sws.PCShip.Up = true;
                    break;
                  case SDL_Keycode.SDLK_DOWN:
                    _sws.PCShip.Down = true;
                    break;
                }
                break;
              case SDL_EventType.SDL_KEYUP:
                switch (e.key.keysym.sym)
                {
                  case SDL_Keycode.SDLK_LEFT:
                    _sws.PCShip.Left = false;
                    break;
                  case SDL_Keycode.SDLK_RIGHT:
                    _sws.PCShip.Right = false;
                    break;
                  case SDL_Keycode.SDLK_UP:
                    _sws.PCShip.Up = false;
                    break;
                  case SDL_Keycode.SDLK_DOWN:
                    _sws.PCShip.Down = false;
                    break;
                }
                break;
              case SDL_EventType.SDL_JOYAXISMOTION:
                if (e.jaxis.which == 0)
                {
                  // System.Console.WriteLine(e.jaxis.axisValue.ToString());
                }
                break;
            }
          }
          //HandleUI(e);        


          if (spsTicks + SERIAL_TICKS_PER_FRAME <= SDL_GetTicks())
          {
            if (_sws.ConsoleInput.Buttons.Triggered(ListOf_ConsoleInputs.ControlLED1))
            {
              // byte[] sendBuffer = new byte[16];
              // sendBuffer[0] = 2;
              // Crc32Algorithm.ComputeAndWriteToEnd(sendBuffer);              

              // byte[] encodedBuffer = new byte[255];
              // var size = COBS.cobs_encode(ref sendBuffer, 16, ref encodedBuffer);
              // encodedBuffer[size] = 0;
              // _serial.sCon[ListOf_Panels.Center].Port.Write(encodedBuffer, 0, size + 1);

              //   _serial.sendUpdate = true;
              // for (int i = 0; i < 50; i++)
              // {
              //   _sws.RightInput.RGB_LEDIndex[i] = 0;
              // }
              // Random rnd = new Random();
              // _sws.RightInput.rgbLed. [_sws.test] = 1;
              // _sws.RightInput.rgbLed.ColorIndex[0] = Color.FromArgb(0, 128, 128);  //on
              // _sws.RightInput.rgbLed.ColorIndex[1] = Color.FromArgb(0, 0, 0);  //off
              // _sws.test++;
            }

            //Serial_Write();
            _serial.sendUpdate = true;
            _serial.Update();
            spsTicks = SDL_GetTicks();
          }

          // if (1 == 1)
          if (fpsTicks + SCREEN_TICKS_PER_FRAME <= SDL_GetTicks())
          {
            // if (_serial->InputDown(Typeof_ConsoleInputs::FlightStickUP))
            // {
            //   _sws->testFlag = true;
            // }
            // else
            // {
            //   _sws->testFlag = false;
            // }


            _render.Render();
            // _network.Update();
            // _controller.Update();
            // _physics.Update();
            // _simulation.Update();

            fps++;
            fpsTicks = SDL_GetTicks();
          }

          if (fpsStart + 1000 < SDL_GetTicks())
          {

            _sws.FPS = fps;
            fps = 0;

            _sws.SPS[0] = _sws.sps_ticks[0];
            _sws.SPS[1] = _sws.sps_ticks[1];
            _sws.SPS[2] = _sws.sps_ticks[2];
            _sws.sps_ticks[0] = 0;
            _sws.sps_ticks[1] = 0;
            _sws.sps_ticks[2] = 0;
            fpsStart = SDL_GetTicks();
          }
        }
      }
      else System.Console.WriteLine("Failed to initialize!\n");
    }


    bool Init()
    {
      //Init
      _sws = new SWSimulation();
      _render = new VTRender(ref _sws);
      // _network = new VTNetwork(ref _sws, "0.0.0.0", 4949);
      // _physics = new VTPhysics(ref _sws);
      _serial = new VTSerial(ref _sws);
      // _simulation = new VTSimulation(ref _sws);

      // _holocron = new VTHolocron(ref _sws);
      // _holocron.Init();      

      // SDL_SetHint(SDL_HINT_XINPUT_ENABLED, "0");

      _render.Init(SCREEN_HEIGHT, SCREEN_WIDTH, 0);

      SDL_SetHint(SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS, "1");
      // _controller = new VTController(_sws);
      // _controller.Init();      

      if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) == true)
      {
        // _serial.StartConnection(ListOf_Panels.Right, "COM6", 115200, 16);
        // _serial.StartConnection(ListOf_Panels.RightAnalog, "COM10", 115200, 10);

        // _serial.StartConnection(ListOf_Panels.Left, "COM3", 115200, 16);
        // _serial.StartConnection(ListOf_Panels.LeftAnalog, "COM8", 115200, 10);

        // _serial.StartConnection(ListOf_Panels.Center, "COM7", 115200, 13);
        // _serial.StartConnection(ListOf_Panels.CenterAnalog, "COM9", 115200, 8);

      }

      if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) == true)
      {
        try
        {
          // _serial.StartConnection(ListOf_Panels.Center, "/dev/ttyACM0", 115200, 13);
        }
        catch (UnauthorizedAccessException) { }
        try
        {
          // _serial.StartConnection(ListOf_Panels.CenterAnalog, "/dev/ttyUSB0", 115200, 8);
        }
        catch (UnauthorizedAccessException) { }

        try
        {
          // _serial.StartConnection(ListOf_Panels.Left, "/dev/ttyACM1", 115200, 16);
        }
        catch (UnauthorizedAccessException) { }
        try
        {
          // _serial.StartConnection(ListOf_Panels.LeftAnalog, "/dev/ttyUSB1", 115200, 10);
        }
        catch (UnauthorizedAccessException) { }

        try
        {
          _serial.StartConnection(ListOf_Panels.Right, "/dev/ttyACM0", 115200, 16);          
          // _serial.StartConnection(ListOf_Panels.Right, "/dev/ttyACM2", 115200, 16);
        }
        catch (UnauthorizedAccessException) { }
        try
        {
          _serial.StartConnection(ListOf_Panels.RightAnalog, "/dev/ttyUSB0", 115200, 10);
          // _serial.StartConnection(ListOf_Panels.RightAnalog, "/dev/ttyUSB2", 115200, 10);
        }
        catch (UnauthorizedAccessException) { }
      }
      return true;
    }


    public void Dispose()
    {
      //Dispose
      _serial.Dispose();
    }

  }
}