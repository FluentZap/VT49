using System;
using System.Linq;
using System.Numerics;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace VT49
{
  class VTRender : IDisposable
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

    IntPtr font = IntPtr.Zero;
    // TTF_Font* Sans = TTF_OpenFont("Sans.ttf", 24); //this opens a font style and sets a size


    int SCREEN_WIDTH, SCREEN_HEIGHT;

    Random rnd = new Random();

    public bool Init(int screen_height, int screen_width, int display)
    {
      SCREEN_HEIGHT = screen_height;
      SCREEN_WIDTH = screen_width;
      // if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) < 0)
      if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_JOYSTICK) < 0)
      {
        System.Console.WriteLine("SDL could not initialize! SDL_Error: %s\n", SDL_GetError());
        return false;
      }

      gWindow = SDL_CreateWindow("VT49", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL_WindowFlags.SDL_WINDOW_BORDERLESS | SDL_WindowFlags.SDL_WINDOW_SHOWN);
      if (gWindow == null)
      {
        System.Console.WriteLine("Window could not be created! SDL_Error: %s\n", SDL_GetError());
      }

      SDL_Rect DispayBounds;
      SDL_GetDisplayBounds(display, out DispayBounds);
      SDL_SetWindowPosition(gWindow, DispayBounds.x + ( DispayBounds.w - 900 ) / 2, DispayBounds.y + ( DispayBounds.h - 1440 ) / 2);      
      // SDL_SetWindowPosition(gWindow, DispayBounds.x + (DispayBounds.w - 900) / 2, DispayBounds.y);


      gRenderer = SDL_CreateRenderer(gWindow, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
      if (gRenderer == null)
      {
        System.Console.WriteLine("Renderer could not be created! SDL_Error: %s\n", SDL_GetError());
        return false;
      }

      gScreenSurface = SDL_GetWindowSurface(gWindow);
      SDL_ShowCursor(SDL_DISABLE);
      TTF_Init();

      LoadResources();
      return true;
    }

    void LoadResources()
    {
      font = TTF_OpenFont(FileLoader.LoadFont("englbesh.ttf"), 24);
      UITexture = LoadTexture(gRenderer, FileLoader.LoadImage("UI2.png"));
    }

    public void Render()
    {
      SDL_SetRenderDrawColor(gRenderer, 10, 10, 18, 255);
      SDL_RenderClear(gRenderer);


      // SDL_Rect rect = new SDL_Rect() {x = 0, y = 0, h = SCREEN_WIDTH, w = SCREEN_HEIGHT};
      SDL_RenderCopy(gRenderer, UITexture, IntPtr.Zero, IntPtr.Zero);

      // for (int x = 0; x < 10; x++)
      // {
      //   SDL_SetRenderDrawColor(gRenderer, (byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255), 255);
      //   SDL_Rect myRect = new SDL_Rect() { x = rnd.Next(1, 500), y = rnd.Next(1, 500), h = 50, w = 50 };
      //   SDL_RenderDrawRect(gRenderer, ref myRect);
      // }
      Vector2 offset = new Vector2(600, 0);
      float scale = 5f;

      SDL_SetRenderDrawColor(gRenderer, 255, 255, 255, 255);
      SDL_Rect myRect = new SDL_Rect()
      {
        x = (int)((_sws.PCShip.Location.X * scale) - (1f * scale) + offset.X),
        y = (int)((_sws.PCShip.Location.Z * scale) - (1f * scale) + offset.Y),
        h = (int)(1f * scale),
        w = (int)(1f * scale)
      };

      // SDL_RenderDrawRect(gRenderer, ref myRect);

      SDL_Color White = new SDL_Color();
      White.a = 255;
      White.r = 255;
      White.g = 255;
      White.b = 255;
      IntPtr surfaceMessage = TTF_RenderText_Solid(font, _sws.LeftInput.AnalogInput(1).ToString(), White); // as TTF_RenderText_Solid could only be used on SDL_Surface then you have to create the surface first

      IntPtr Message = SDL_CreateTextureFromSurface(gRenderer, surfaceMessage); //now you can convert it into a texture      

      SDL_Rect Message_rect; //create a rect
      Message_rect.x = 0;  //controls the rect's x coordinate 
      Message_rect.y = 0; // controls the rect's y coordinate
      Message_rect.w = 500; // controls the width of the rect
      Message_rect.h = 100; // controls the height of the rect

      //Mind you that (0,0) is on the top left of the window/screen, think a rect as the text's box, that way it would be very simple to understance

      //Now since it's a texture, you have to put RenderCopy in your game loop area, the area where the whole code executes

      // SDL_RenderCopy(gRenderer, Message, IntPtr.Zero, ref Message_rect); //you put the renderer's name first, the Message, the crop size(you can ignore this if you don't want to dabble with cropping), and the rect which is the size and coordinate of your texture


      // SDL_SetRenderDrawColor(gRenderer, 200, 0, 0, 255);
      // myRect = new SDL_Rect() { x = (int)_sws.Station.Location.X, y = (int)_sws.Station.Location.Y, h = 1, w = 500 };
      // SDL_RenderDrawRect(gRenderer, ref myRect);

      myRect = new SDL_Rect()
      {
        x = _sws.RightInput.rotaryValue[0],
        y = _sws.RightInput.rotaryValue[1],
        h = _sws.RightInput.rotaryValue[2],
        w = _sws.RightInput.rotaryValue[3]
      };
      // SDL_RenderDrawRect(gRenderer, ref myRect);

      // System.Console.WriteLine("1: " + _sws.RightInput.AnalogInput(0));
      // System.Console.WriteLine("2: " + _sws.RightInput.AnalogInput(1));
      // System.Console.WriteLine("3: " + _sws.RightInput.AnalogInput(2));
      // System.Console.WriteLine("4: " + _sws.RightInput.AnalogInput(3));
      // System.Console.WriteLine("5: " + _sws.RightInput.AnalogInput(4));
      // System.Console.WriteLine("6: " + _sws.RightInput.AnalogInput(5));
      // foreach (var item in _sws.RightInput.Buttons.ToList())
      // {        
      //   System.Console.WriteLine(item.ToString());
      // }
      if (_sws.RightInput.Buttons.Triggered(ListOf_SideInputs.ControlLED1))
      {
        // _sws.RightInput.LEDs.SetOn((ListOf_SideOutputs)_sws.test);
        //   System.Console.WriteLine(_sws.test.ToString() + " " + Enum.GetName(typeof(ListOf_SideOutputs), _sws.test));
        //   _sws.test++;
        //   rnd = new Random();        
        //   for (int i = 0; i < 4; i++)
        //   {
        //     _sws.RightInput.Matrix[rnd.Next(0, 15), rnd.Next(0, 15)] = true;
        //   }        

        // for (int x = 0; x < 16; x++)
        // for (int y = 0; y < 16; y++)
        // {
        // _sws.RightInput.Matrix[x, y] = true;
        // }
        // _sws.RightInput.Matrix[_sws.test % 16, _sws.test / 16] = true;
        // _sws.test++;
      }
      for (int i = 0; i < 256; i++)
        _sws.RightInput.Matrix[i % 16, i / 16] = false;

      for (int s = 0; s < 2; s++)
        for (int i = 0; i < 64; i++)
          _sws.RightInput.Seg[s, i % 8, i / 8] = false;

      int step = 4;
      _sws.RightInput.Matrix[
        Math.Clamp(_sws.RightInput.rotaryValue[3] / step, 0, 15),
        Math.Clamp(_sws.RightInput.rotaryValue[4] / step, 0, 15)
        ] = true;

      // _sws.RightInput.Seg[0,
      // Math.Clamp(_sws.RightInput.rotaryValue[3] / step, 0, 7),
      // Math.Clamp(_sws.RightInput.rotaryValue[4] / step, 0, 7)
      // ] = true;

      // _sws.RightInput.Seg[1,
      // Math.Clamp(_sws.RightInput.rotaryValue[3] / step, 0, 7),
      // Math.Clamp(_sws.RightInput.rotaryValue[4] / step, 0, 7)
      // ] = true;


      if (_sws.RightInput.Buttons.IsDown(ListOf_SideInputs.EightToggle1))
        _sws.RightInput.Seg[1, 0, 0] = true;
      else
        _sws.RightInput.Seg[1, 0, 3] = true;
      // _sws.RightInput.Seg[1, 0, 6] = true;


      // System.Console.WriteLine(_sws.RightInput.rotaryValue[3]);

      // _sws.test++;
      // if (_sws.test > 254)
      // _sws.test = 0;

      // System.Console.WriteLine(_sws.RightInput.rotaryValue[0]);

      // SDL_FPoint[] points = new SDL_FPoint[_sws.StationVectors.Count];

      // for (int i = 0; i < _sws.StationVectors.Count; i++)
      // {
      //   points[i].x = ((_sws.StationVectors[i].X + _sws.Station.LocationOffset.X) * scale) + offset.X;
      //   points[i].y = (_sws.StationVectors[i].Z + _sws.Station.LocationOffset.Z + 100) * scale + offset.Y;
      // }

      // var n9 = _sws.galaxyMap.ArchivePlanetInfo.Where(x => x.grid == "N9");
      // foreach (var item in _sws.galaxyMap.ArchivePlanetInfo)
      // {
      //   SDL_RenderDrawPoint(gRenderer, (int)item.x / 100 + SCREEN_WIDTH / 2, (int)item.y / 100 + SCREEN_HEIGHT / 2);
      // }

      // SDL_RenderDrawPointsF(gRenderer, points, _sws.StationVectors.Count);
      // System.Console.WriteLine(_sws.FPS);
      SDL_RenderPresent(gRenderer);
    }

    public void Dispose()
    {
      SDL_Quit();            
    }

    private static IntPtr LoadTexture(IntPtr gRenderer, string path)
    {
      IntPtr newTexture = IntPtr.Zero;      
      IntPtr surface = IMG_Load(path);
      if (surface == IntPtr.Zero)
        return IntPtr.Zero;

      newTexture = SDL_CreateTextureFromSurface(gRenderer, surface);
      if (newTexture == IntPtr.Zero)
      {
        System.Console.WriteLine("Unable to load image %s! SDL Error: %s\n", path, SDL_GetError());
        SDL_FreeSurface(surface);
        return IntPtr.Zero;
      }
      SDL_FreeSurface(surface);
      return newTexture;
    }

  }


}