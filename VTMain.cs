using System;
using System.Collections.Generic;

using static SDL2.SDL;

namespace VT49
{
  class VTMain : IDisposable
  {
    const int SCREEN_WIDTH = 900, SCREEN_HEIGHT = 1440, SCREEN_FPS = 60;    
    const double SCREEN_TICKS_PER_FRAME = 1000 / SCREEN_FPS;
    const double SERIAL_TICKS_PER_FRAME = 1000 / 60;
    bool quit = false;    
    long fpsTicks, fpsStart, spsTicks, spsStart;            
    
    SWSimulation _sws;
    VTRender _render;
    VTNetwork _network;

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
            //HandleUI(e);
          }


          if (spsTicks + SERIAL_TICKS_PER_FRAME <= SDL_GetTicks())
          {
            // _serial->Update();
            //Serial_Write();            
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
            fps++;
            fpsTicks = SDL_GetTicks();
          }          

          if (fpsStart + 1000 < SDL_GetTicks())
          {            
            // _sws.FPS = fps;
            fps = 0;

            // _sws.SPS = sps;
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


      _render.Init(SCREEN_HEIGHT, SCREEN_WIDTH, 3);
      return true;
    }


    public void Dispose()
    {
      //Dispose
    }

  }
}