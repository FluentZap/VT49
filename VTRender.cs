using System;
using System.Numerics;
using static SDL2.SDL;

namespace VT49
{
  class VTRender
  {
    SWSimulation _sws;

    public VTRender(ref SWSimulation sws)
    {
      _sws = sws;
    }




    IntPtr gWindow = IntPtr.Zero;
    IntPtr gRenderer = IntPtr.Zero;
    IntPtr gScreenSurface = IntPtr.Zero;
    SDL_Surface gXOut;
    IntPtr gTexture = IntPtr.Zero;
    IntPtr UITexture = IntPtr.Zero;

    int SCREEN_WIDTH, SCREEN_HEIGHT;

    Random rnd = new Random();

    public bool Init(int screen_height, int screen_width, int display)
    {
      SCREEN_HEIGHT = screen_height;
      SCREEN_WIDTH = screen_width;
      if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) < 0)
      {
        System.Console.WriteLine("SDL could not initialize! SDL_Error: %s\n", SDL_GetError());
        return false;
      }

      gWindow = SDL_CreateWindow("VT49", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
      if (gWindow == null)
      {
        System.Console.WriteLine("Window could not be created! SDL_Error: %s\n", SDL_GetError());
      }

      SDL_Rect DispayBounds;
      SDL_GetDisplayBounds(display, out DispayBounds);
      // SDL_SetWindowPosition(gWindow, DispayBounds.x + ( DispayBounds.w - 900 ) / 2, DispayBounds.y + ( DispayBounds.h - 1440 ) / 2);      
      // SDL_SetWindowPosition(gWindow, DispayBounds.x + (DispayBounds.w - 900) / 2, DispayBounds.y);


      gRenderer = SDL_CreateRenderer(gWindow, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
      if (gRenderer == null)
      {
        System.Console.WriteLine("Renderer could not be created! SDL_Error: %s\n", SDL_GetError());
        return false;
      }

      gScreenSurface = SDL_GetWindowSurface(gWindow);
      SDL_ShowCursor(SDL_DISABLE);

      LoadResources();
      return true;
    }

    void LoadResources()
    {

    }


    public void Render()
    {
      SDL_SetRenderDrawColor(gRenderer, 10, 10, 10, 255);
      SDL_RenderClear(gRenderer);

      // for (int x = 0; x < 10; x++)
      // {
      //   SDL_SetRenderDrawColor(gRenderer, (byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255), 255);
      //   SDL_Rect myRect = new SDL_Rect() { x = rnd.Next(1, 500), y = rnd.Next(1, 500), h = 50, w = 50 };
      //   SDL_RenderDrawRect(gRenderer, ref myRect);
      // }

      SDL_SetRenderDrawColor(gRenderer, 255, 255, 255, 255);
      SDL_Rect myRect = new SDL_Rect() { x = (int)_sws.PCShip.Location.X - 25, y = (int)_sws.PCShip.Location.Y - 25, h = 50, w = 50 };
      SDL_RenderDrawRect(gRenderer, ref myRect);

      SDL_SetRenderDrawColor(gRenderer, 200, 0, 0, 255);
      myRect = new SDL_Rect() { x = (int)_sws.Station.Location.X, y = (int)_sws.Station.Location.Y, h = 1, w = 500 };
      SDL_RenderDrawRect(gRenderer, ref myRect);

      myRect = new SDL_Rect()
      {
        x = _sws.ConsoleAnalogValue[0],
        y = _sws.ConsoleAnalogValue[1],
        h = _sws.ConsoleAnalogValue[2],
        w = _sws.ConsoleAnalogValue[3]
      };
      SDL_RenderDrawRect(gRenderer, ref myRect);


      System.Console.WriteLine(_sws.FPS);
      SDL_RenderPresent(gRenderer);
    }

  }


}