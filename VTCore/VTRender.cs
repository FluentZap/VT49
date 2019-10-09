using System;
using System.IO.Ports;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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
    FC_Font fontTest;

    IntPtr AurabeshLarge = IntPtr.Zero;
    IntPtr TeutonLarge = IntPtr.Zero;

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
      // SDL_SetWindowPosition(gWindow, DispayBounds.x + (DispayBounds.w - 900) / 2, DispayBounds.y + (DispayBounds.h - 1440) / 2);
      SDL_SetWindowPosition(gWindow, DispayBounds.x + (DispayBounds.w - 900) / 2, DispayBounds.y);


      gRenderer = SDL_CreateRenderer(gWindow, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
      if (gRenderer == null)
      {
        System.Console.WriteLine("Renderer could not be created! SDL_Error: %s\n", SDL_GetError());
        return false;
      }
      SDL_GL_SetSwapInterval(0);

      gScreenSurface = SDL_GetWindowSurface(gWindow);
      SDL_ShowCursor(SDL_DISABLE);
      TTF_Init();

      LoadResources();
      return true;
    }

    void LoadResources()
    {
      AurabeshLarge = TTF_OpenFont(FileLoader.LoadFont("englbesh.ttf"), 48);
      TeutonLarge = TTF_OpenFont(FileLoader.LoadFont("TeutonMager.otf"), 24);
      UITexture = LoadTexture(gRenderer, FileLoader.LoadImage("UI2.png"));

      fontTest = new FC_Font(AurabeshLarge, gRenderer, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });

    }

    public void Render()
    {
      SDL_SetRenderDrawColor(gRenderer, 10, 10, 18, 255);
      SDL_RenderClear(gRenderer);

      if (_sws.DiagnosticMode)
      {
        // DrawConsoleDiagnostics();
        SDL_Rect destRect = new SDL_Rect() { x = 0, y = 0, h = fontTest.height * 12, w = fontTest.height * 12 };
        SDL_RenderCopy(gRenderer, fontTest.FontTexture, IntPtr.Zero, ref destRect);
      }
      else
      {
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
        SDL_RenderDrawRect(gRenderer, ref myRect);

        // System.Console.WriteLine("1: " + _sws.ConsoleAnalogValue[0]);
        // System.Console.WriteLine("2: " + _sws.ConsoleAnalogValue[1]);
        // System.Console.WriteLine("3: " + _sws.ConsoleAnalogValue[2]);
        // System.Console.WriteLine("4: " + _sws.ConsoleAnalogValue[3]);

        // for (int i = 0; i < 4; i++)
        // {
        //   RenderText(gRenderer, 50, 400 + i * 50, i.ToString() + ": " + _sws.ConsoleInput.AnalogInput(i), font, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });        
        // }


        // foreach (var item in _sws.LeftInput.Buttons.ToList())
        // {
        //   System.Console.WriteLine(item.ToString());
        // }

        // System.Console.WriteLine(_sws.ConsoleInput.rotaryValue[0]);
        // System.Console.WriteLine(_sws.ConsoleInput.rotaryValue[1]);

        // System.Console.WriteLine(Encoding.UTF8.GetString(_sws.ConsoleInput.CylinderCode, 0, _sws.ConsoleInput.CylinderCode.Length));


        // _sws.RightInput.LEDs.SetOn(ListOf_SideOutputs.EightLEDToggle);
        // _sws.RightInput.LEDs.SetOn(ListOf_SideOutputs.ThrottleLEDToggle);

        //_sws.RightInput.LEDs.SetOn((ListOf_SideOutputs)_sws.test);
        //System.Console.WriteLine(Enum.GetName(typeof(ListOf_SideOutputs), _sws.test));        

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
          //_sws.test++;
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

        // string speed = _sws.PCShip.EngineSpeed.ToString();
        string speed = _sws.test.ToString();
        // System.Console.WriteLine(speed[0]);
        int number_os = 0;
        for (int i = 0; i < speed.Length; i++)
        {
          if (i + 1 < speed.Length && speed[i + 1] == '.')
          {
            _sws.RightInput.SetSegDigit(1, i - number_os, speed[i], true);
            number_os++;
            i++;
          }
          else
          {
            _sws.RightInput.SetSegDigit(1, i - number_os, speed[i]);
          }
        }

        // _sws.RightInput.SetSegDigit(1, 0, speed[0]);

        // _sws.RightInput.Seg[1,
        // Math.Clamp(_sws.RightInput.rotaryValue[3] / step, 0, 7),
        // Math.Clamp(_sws.RightInput.rotaryValue[4] / step, 0, 7)
        // ] = true;


        // if (_sws.RightInput.Buttons.IsDown(ListOf_SideInputs.EightToggle1))
        //   _sws.RightInput.Seg[1, 0, 0] = true;
        // else
        //   _sws.RightInput.Seg[1, 0, 3] = true;
        // _sws.RightInput.Seg[1, 0, 6] = true;


        // System.Console.WriteLine(_sws.RightInput.rotaryValue[3]);
        // System.Console.WriteLine(_sws.PCShip.EngineSpeed);
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
        // System.Console.WriteLine(_sws.SPS);
        // System.Console.WriteLine(_sws.FPS);
        // RenderText(gRenderer, 50, 500, _sws.FPS.ToString(), font, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
        // System.Console.WriteLine(_sws.test);

        RgbLedControl.clearLED(_sws.RightInput.rgbLed.ThrottleLED);

        _sws.RightInput.rgbLed.ColorIndex[0] = Color.FromArgb
        (
          _sws.RightInput.AnalogInput(0),
          _sws.RightInput.AnalogInput(1),
          _sws.RightInput.AnalogInput(2)
        );

        _sws.RightInput.rgbLed.ColorIndex[1] = Color.FromArgb
        (
          _sws.RightInput.AnalogInput(3),
          _sws.RightInput.AnalogInput(4),
          _sws.RightInput.AnalogInput(5)
        );

        for (int i = 0; i < _sws.PCShip.EngineSpeed / 30; i++)
        {
          if (i < 5)
          {
            _sws.RightInput.rgbLed.ThrottleLED[i] = 1;
          }
        }
      }
      // RenderText(gRenderer, 0, 0, $"FPS {_sws.FPS}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      // RenderText(gRenderer, 0, 30, $"SPS {_sws.SPSSend[0]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
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

    void RenderText(IntPtr renderer, int x, int y, string text, IntPtr font, SDL_Color color)
    {
      if (text != "")
      {

        // IntPtr surface;
        // IntPtr texture;
        SDL_Rect rect;
        IntPtr surfaceP = TTF_RenderText_Solid(font, text, color);
        IntPtr textureP = SDL_CreateTextureFromSurface(renderer, surfaceP);
        SDL_Surface surface = Marshal.PtrToStructure<SDL_Surface>(surfaceP);
        // Marshal.FreeHGlobal(surfaceP);        
        rect.x = x;
        rect.y = y;
        rect.w = surface.w;
        rect.h = surface.h;
        SDL_RenderCopy(renderer, textureP, IntPtr.Zero, ref rect);
        SDL_FreeSurface(surfaceP);
        SDL_DestroyTexture(textureP);
      }
    }



    void DrawConsoleDiagnostics()
    {
      int index = 0;
      RenderText(gRenderer, 0, 0, $"FPS {_sws.FPS}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      RenderText(gRenderer, 0, 30, $"Left SPS {_sws.SPSSend[0]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      RenderText(gRenderer, 0, 60, $"Right SPS {_sws.SPSSend[2]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      RenderText(gRenderer, 0, 90, $"Center SPS {_sws.SPSSend[1]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });

      RenderText(gRenderer, 600, 0 + 0 * 30, $"C Received{_sws.SPSReceive[0]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      RenderText(gRenderer, 600, 0 + 1 * 30, $"CA Received{_sws.SPSReceive[1]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      RenderText(gRenderer, 600, 0 + 2 * 30, $"L Received{_sws.SPSReceive[2]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      RenderText(gRenderer, 600, 0 + 3 * 30, $"LA Received{_sws.SPSReceive[3]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      RenderText(gRenderer, 600, 0 + 4 * 30, $"R Received{_sws.SPSReceive[4]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      RenderText(gRenderer, 600, 0 + 5 * 30, $"RA Received{_sws.SPSReceive[5]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });

      foreach (var name in SerialPort.GetPortNames())
      {
        RenderText(gRenderer, 300, index * 30, name, TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
        index++;
      }

      for (int i = 0; i < 6; i++)
      {
        RenderText(gRenderer, 0, 300 + i * 30, $"LCon Alog {i.ToString()}: {_sws.LeftInput.AnalogInput(i)}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      }

      for (int i = 0; i < 6; i++)
      {
        RenderText(gRenderer, 600, 300 + i * 30, $"RCon Alog {i.ToString()}: {_sws.RightInput.AnalogInput(i)}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      }

      for (int i = 0; i < 4; i++)
      {
        RenderText(gRenderer, 300, 300 + i * 30, $"CCon Alog {i.ToString()}: {_sws.ConsoleInput.AnalogInput(i)}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      }

      //buttons
      index = 0;
      foreach (var item in _sws.ConsoleInput.Buttons.ToList())
      {
        RenderText(gRenderer, 300, 600 + index * 30, $"CCon {item.ToString()}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
        index++;
      }

      index = 0;
      foreach (var item in _sws.LeftInput.Buttons.ToList())
      {
        RenderText(gRenderer, 0, 600 + index * 30, $"LCon {item.ToString()}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
        index++;
      }

      index = 0;
      foreach (var item in _sws.RightInput.Buttons.ToList())
      {
        RenderText(gRenderer, 600, 600 + index * 30, $"RCon {item.ToString()}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
        index++;
      }

      for (int i = 0; i < 2; i++)
      {
        RenderText(gRenderer, 300, 800 + i * 30, $"CCon Rot {i.ToString()}: {_sws.ConsoleInput.rotaryValue[i]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      }

      for (int i = 0; i < 6; i++)
      {
        RenderText(gRenderer, 0, 800 + i * 30, $"LCon Rot {i.ToString()}: {_sws.LeftInput.rotaryValue[i]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      }

      for (int i = 0; i < 6; i++)
      {
        RenderText(gRenderer, 600, 800 + i * 30, $"RCon Rot {i.ToString()}: {_sws.RightInput.rotaryValue[i]}", TeutonLarge, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      }
    }
  }
}