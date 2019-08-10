using System;
using System.Collections.Generic;

using static SDL2.SDL;

namespace VT49
{
  class VTMain : IDisposable
  {
    const int SCREEN_WIDTH = 1920, SCREEN_HEIGHT = 1080, SCREEN_FPS = 60;
    const double SCREEN_TICKS_PER_FRAME = 1000 / SCREEN_FPS;
    const double SERIAL_TICKS_PER_FRAME = 1000 / 120;
    bool quit = false;
    long fpsTicks, fpsStart, spsTicks, spsStart;

    SWSimulation _sws;
    VTRender _render;
    VTNetwork _network;
    VTPhysics _physics;
    VTSerial _serial;

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
            }
          }
          //HandleUI(e);        


          if (spsTicks + SERIAL_TICKS_PER_FRAME <= SDL_GetTicks())
          {            
            //Serial_Write();
            _serial.Update();
            sps++;
            spsTicks = SDL_GetTicks();
          }

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
            // _physics->Update();
            _render.Render();
            _network.Update();
            _physics.Update();

            fps++;
            fpsTicks = SDL_GetTicks();
          }          

          if (fpsStart + 1000 < SDL_GetTicks())
          {
            _sws.FPS = fps;
            fps = 0;

            _sws.SPS = sps;
            sps = 0;
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
      _network = new VTNetwork(ref _sws, "0.0.0.0", 4949);
      _physics = new VTPhysics(ref _sws);
      _serial = new VTSerial(_sws);


      _render.Init(SCREEN_HEIGHT, SCREEN_WIDTH, 0);
      _serial.StartConnection(ListOf_Panels.CenterAnalog, "COM6", 115200, 4);
      return true;
    }


    public void Dispose()
    {
      //Dispose
    }

  }
}