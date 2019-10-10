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

    IntPtr AurabeshLargeFont = IntPtr.Zero;
    IntPtr TeutonLargeFont = IntPtr.Zero;
    FC_Font TeutonLarge;
    FC_Font AurabeshLarge;

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


      gRenderer = SDL_CreateRenderer(gWindow, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE);
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
      AurabeshLargeFont = TTF_OpenFont(FileLoader.LoadFont("englbesh.ttf"), 24);
      TeutonLargeFont = TTF_OpenFont(FileLoader.LoadFont("TeutonMager.otf"), 24);
      TeutonLarge = new FC_Font(TeutonLargeFont, gRenderer, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
      AurabeshLarge = new FC_Font(AurabeshLargeFont, gRenderer, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });

      UITexture = LoadTexture(gRenderer, FileLoader.LoadImage("UI2.png"));

    }

    public void Render()
    {
      SDL_SetRenderDrawColor(gRenderer, 10, 10, 18, 255);
      SDL_RenderClear(gRenderer);

      if (_sws.DiagnosticMode)
      {
        DrawConsoleDiagnostics();
        // SDL_Rect destRect = new SDL_Rect() { x = 0, y = 0, h = TeutonLarge.height * 12, w = TeutonLarge.height * 12 };
        // SDL_RenderCopy(gRenderer, TeutonLarge.FontTexture, IntPtr.Zero, ref destRect);

        // TeutonLarge.FC_DrawText(gRenderer, "Hello Galaxy", 100, 600);
      }
      else
      {
        //Draw Background
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

        // myRect = new SDL_Rect()
        // {
        //   x = _sws.RightInput.rotaryValue[0],
        //   y = _sws.RightInput.rotaryValue[1],
        //   h = _sws.RightInput.rotaryValue[2],
        //   w = _sws.RightInput.rotaryValue[3]
        // };
        // SDL_RenderDrawRect(gRenderer, ref myRect);

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


        // for (int x = 0; x < 16; x++)
        // for (int y = 0; y < 16; y++)
        // {
        // _sws.RightInput.Matrix[x, y] = true;
        // }
        // _sws.RightInput.Matrix[_sws.test % 16, _sws.test / 16] = true;
        //_sws.test++;

        
        _sws.RightInput.ClearMatrix();
        int step = 4;
        _sws.RightInput.Matrix[
          Math.Clamp(_sws.RightInput.rotaryValue[3] / step, 0, 15),
          Math.Clamp(_sws.RightInput.rotaryValue[4] / step, 0, 15)
          ] = true;

        // _sws.RightInput.Seg[0,
        // Math.Clamp(_sws.RightInput.rotaryValue[3] / step, 0, 7),
        // Math.Clamp(_sws.RightInput.rotaryValue[4] / step, 0, 7)
        // ] = true;
        
        _sws.LeftInput.ClearMatrix();
        _sws.LeftInput.Matrix[
          Math.Clamp(_sws.LeftInput.rotaryValue[3] / step, 0, 15),
          Math.Clamp(_sws.LeftInput.rotaryValue[4] / step, 0, 15)
          ] = true;

        // _sws.LeftInput.Seg[0,
        // Math.Clamp(_sws.LeftInput.rotaryValue[3] / step, 0, 7),
        // Math.Clamp(_sws.LeftInput.rotaryValue[4] / step, 0, 7)
        // ] = true;

        // string speed = _sws.PCShip.EngineSpeed.ToString();                

        // SDL_FPoint[] points = new SDL_FPoint[_sws.StationVectors.Count];

        // for (int i = 0; i < _sws.StationVectors.Count; i++)
        // {
        //   points[i].x = ((_sws.StationVectors[i].X + _sws.Station.Location.X) * scale) + offset.X;
        //   points[i].y = (_sws.StationVectors[i].Z + _sws.Station.Location.Z + 100) * scale + offset.Y;
        // }

        // var n9 = _sws.galaxyMap.ArchivePlanetInfo.Where(x => x.grid == "N9");
        // foreach (var item in _sws.galaxyMap.ArchivePlanetInfo)
        // {
        //   SDL_RenderDrawPoint(gRenderer, (int)item.x / 100 + SCREEN_WIDTH / 2, (int)item.y / 100 + SCREEN_HEIGHT / 2);
        // }

        // SDL_RenderDrawPointsF(gRenderer, points, _sws.StationVectors.Count);



        // TeutonLarge.FC_DrawText(gRenderer, 0, 200, _sws.ConsoleInput.rgbLed.ColorIndex[0].ToString());
        // TeutonLarge.FC_DrawText(gRenderer, 0, 250, _sws.ConsoleInput.rgbLed.ColorIndex[1].ToString());

        // TeutonLarge.FC_DrawText(gRenderer, 0, 90, $"Flight Control {_sws.PCShip.FlightControl}");


        int timeAdvance = (int)(_sws.time % 360);
        if (timeAdvance > 180)
        {
          timeAdvance = (byte)((360 - timeAdvance));
        }
        byte aniTime = (byte)(timeAdvance * 1.416);

        // TeutonLarge.FC_DrawText(gRenderer, 100, 100, $"Time {aniTime.ToString()}");
        if (_sws.PCShip.LeftControlInterface.LoginID == "" &&
        _sws.PCShip.RightControlInterface.LoginID == "" &&
        _sws.PCShip.CenterControlInterface.LoginID == "")
        {
          AurabeshLarge.FC_DrawText(gRenderer, 280, 600, "System Locked", Color.FromArgb(aniTime, 255, 255, 255));
          AurabeshLarge.FC_DrawText(gRenderer, 310, 660, "Insert Code", Color.FromArgb(aniTime, 255, 255, 255));
        }

        if (_sws.PCShip.CenterControlInterface.LoginID == "Chirump")
          TeutonLarge.FC_DrawText(gRenderer, 280, 40, "Login: Chirump", Color.FromArgb(255, 255, 165, 0));

        if (_sws.PCShip.CenterControlInterface.LoginID == "SN-34k-")
          TeutonLarge.FC_DrawText(gRenderer, 280, 40, "Login: SN-34k-", Color.FromArgb(255, 118, 91, 255));

        if (_sws.PCShip.CenterControlInterface.LoginID == "Craw---")
          TeutonLarge.FC_DrawText(gRenderer, 280, 40, "Login: Craw", Color.FromArgb(255, 40, 120, 255));

        if (_sws.PCShip.CenterControlInterface.LoginID == "Snow---")
          TeutonLarge.FC_DrawText(gRenderer, 280, 40, "Login: Snow", Color.FromArgb(255, 255, 40, 40));


        if (_sws.PCShip.LeftControlInterface.LoginID != "" ||
                _sws.PCShip.RightControlInterface.LoginID != "" ||
                _sws.PCShip.CenterControlInterface.LoginID != "")
        {
          RenderStarMap();

        }


      }

      // TeutonLarge.FC_DrawText(gRenderer, 0, 0, $"FPS {_sws.FPS}");
      // TeutonLarge.FC_DrawText(gRenderer, 0, 30, $"SPS {_sws.SPSSend[0]}");

      SDL_RenderPresent(gRenderer);
    }


    public void RenderStarMap()
    {

      // int x = char.ToUpper('N') - 64; // =1
      int x = Encoding.ASCII.GetBytes("N")[0];
      int y = 9;
      if (_sws.LoadedSystem == null)
      {
        _sws.LoadedSystem = _sws.galaxyMap.ArchivePlanetInfo.Where(x => x.grid == "N9");
      }

      double adjDistanceX = x * 3.5921014243681D * 7;
      double adjDistanceY = y * 3.5921014243681D * 7;

      foreach (var item in _sws.LoadedSystem)
      {
        // SDL_Rect myRect = new SDL_Rect()
        // {
        //   x = _sws.RightInput.rotaryValue[0],
        //   y = _sws.RightInput.rotaryValue[1],
        //   h = _sws.RightInput.rotaryValue[2],
        //   w = _sws.RightInput.rotaryValue[3]
        // };
        int xPos = (int)(item.x / 2 - adjDistanceX) + (SCREEN_WIDTH / 2) + (int)_sws.NavMapScroll.X * 3;
        int yPos = (int)(item.y / 2 - adjDistanceY) + (SCREEN_HEIGHT / 2) + (int)_sws.NavMapScroll.Y * 3;

        // SDL_RenderDrawRect(gRenderer, ref myRect);
        if (xPos > 240 && xPos < 660 && yPos > 540 && yPos < 1260)
        {
          SDL_RenderDrawPoint(gRenderer, xPos, yPos);
          TeutonLarge.FC_DrawText(gRenderer, xPos, yPos - 30, item.Name);
        }
      }

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

    void DrawConsoleDiagnostics()
    {
      int index = 0;
      TeutonLarge.FC_DrawText(gRenderer, 0, 0, $"FPS {_sws.FPS}");
      TeutonLarge.FC_DrawText(gRenderer, 0, 30, $"Left SPS {_sws.SPSSend[0]}");
      TeutonLarge.FC_DrawText(gRenderer, 0, 60, $"Right SPS {_sws.SPSSend[2]}");
      TeutonLarge.FC_DrawText(gRenderer, 0, 90, $"Center SPS {_sws.SPSSend[1]}");

      TeutonLarge.FC_DrawText(gRenderer, 600, 0 + 0 * 30, $"C Received{_sws.SPSReceive[0]}");
      TeutonLarge.FC_DrawText(gRenderer, 600, 0 + 1 * 30, $"CA Received{_sws.SPSReceive[1]}");
      TeutonLarge.FC_DrawText(gRenderer, 600, 0 + 2 * 30, $"L Received{_sws.SPSReceive[2]}");
      TeutonLarge.FC_DrawText(gRenderer, 600, 0 + 3 * 30, $"LA Received{_sws.SPSReceive[3]}");
      TeutonLarge.FC_DrawText(gRenderer, 600, 0 + 4 * 30, $"R Received{_sws.SPSReceive[4]}");
      TeutonLarge.FC_DrawText(gRenderer, 600, 0 + 5 * 30, $"RA Received{_sws.SPSReceive[5]}");


      foreach (var name in SerialPort.GetPortNames())
      {
        TeutonLarge.FC_DrawText(gRenderer, 300, index * 30, name);
        index++;
      }

      //Analogs
      for (int i = 0; i < 6; i++)
      {
        TeutonLarge.FC_DrawText(gRenderer, 0, 300 + i * 30, $"LCon Alog {i.ToString()}: {_sws.LeftInput.AnalogInput(i)}");
      }

      for (int i = 0; i < 6; i++)
      {
        TeutonLarge.FC_DrawText(gRenderer, 600, 300 + i * 30, $"RCon Alog {i.ToString()}: {_sws.RightInput.AnalogInput(i)}");
      }

      for (int i = 0; i < 4; i++)
      {
        TeutonLarge.FC_DrawText(gRenderer, 300, 300 + i * 30, $"CCon Alog {i.ToString()}: {_sws.ConsoleInput.AnalogInput(i)}");
      }

      //buttons
      index = 0;
      foreach (var item in _sws.ConsoleInput.Buttons.ToList())
      {
        TeutonLarge.FC_DrawText(gRenderer, 300, 600 + index * 30, $"CCon {item.ToString()}");
        index++;
      }

      index = 0;
      foreach (var item in _sws.LeftInput.Buttons.ToList())
      {
        TeutonLarge.FC_DrawText(gRenderer, 0, 600 + index * 30, $"LCon {item.ToString()}");
        index++;
      }

      index = 0;
      foreach (var item in _sws.RightInput.Buttons.ToList())
      {
        TeutonLarge.FC_DrawText(gRenderer, 600, 600 + index * 30, $"RCon {item.ToString()}");
        index++;
      }

      //Rotaries
      for (int i = 0; i < 2; i++)
      {
        TeutonLarge.FC_DrawText(gRenderer, 300, 800 + i * 30, $"CCon Rot {i.ToString()}: {_sws.ConsoleInput.rotaryValue[i]}");
      }

      for (int i = 0; i < 6; i++)
      {
        TeutonLarge.FC_DrawText(gRenderer, 0, 800 + i * 30, $"LCon Rot {i.ToString()}: {_sws.LeftInput.rotaryValue[i]}");
      }

      for (int i = 0; i < 6; i++)
      {
        TeutonLarge.FC_DrawText(gRenderer, 600, 800 + i * 30, $"RCon Rot {i.ToString()}: {_sws.RightInput.rotaryValue[i]}");
      }
    }
  }
}
