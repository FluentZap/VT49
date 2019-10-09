using System;
using System.Collections.Generic;
using System.Linq;

namespace VT49
{
  class VTSimulation
  {
    SWSimulation _sws = null;

    public VTSimulation(ref SWSimulation sws)
    {
      _sws = sws;
    }


    public void Update()
    {
      // List<SWPlanetInfo> planets = _sws.galaxyMap.ArchivePlanetInfo.Where(x => x.grid == "K9").ToList();
      // System.Console.WriteLine(planets);
      // _sws.RightInput.SetSegDigit

      switch (_sws.DiagnosticModeUnlock)
      {
        case 0:
          if (_sws.ConsoleInput.Buttons.IsDown(ListOf_ConsoleInputs.ControlLED3))
          {
            _sws.DiagnosticModeUnlock = 1;
          }
          break;
        case 1:
          if (_sws.ConsoleInput.Buttons.IsDown(ListOf_ConsoleInputs.ControlLED1))
          {
            _sws.DiagnosticModeUnlock = 2;
          }
          break;
        case 2:
          if (_sws.ConsoleInput.Buttons.IsDown(ListOf_ConsoleInputs.ControlLED4))
          {
            _sws.DiagnosticModeUnlock = 3;
          }
          break;
        case 3:
          if (_sws.ConsoleInput.Buttons.IsDown(ListOf_ConsoleInputs.ControlLED2))
          {
            _sws.DiagnosticModeUnlock = 0;
            _sws.DiagnosticMode = !_sws.DiagnosticMode;
          }
          break;
      }

      SetPanelLeds();

      HandleConsole();

    }

    void HandleConsole()
    {
      ConsoleControl C = _sws.ConsoleInput;
      Starship S = _sws.PCShip;
      if (C.Buttons.Triggered(ListOf_ConsoleInputs.DoubleTog1_UP))
        S.reactorControl.powerRouting.IncreeseCm();
      if (C.Buttons.Triggered(ListOf_ConsoleInputs.DoubleTog1_DOWN))
        S.reactorControl.powerRouting.DecreeseCm();
      
      if (C.Buttons.Triggered(ListOf_ConsoleInputs.DoubleTog2_UP))
        S.reactorControl.powerRouting.IncreeseTact();
      if (C.Buttons.Triggered(ListOf_ConsoleInputs.DoubleTog2_DOWN))
        S.reactorControl.powerRouting.DecreeseTact();
      
      if (C.Buttons.Triggered(ListOf_ConsoleInputs.DoubleTog3_UP))
        S.reactorControl.powerRouting.IncreeseProp();
      if (C.Buttons.Triggered(ListOf_ConsoleInputs.DoubleTog3_DOWN))
        S.reactorControl.powerRouting.DecreeseProp();
      
      if (C.Buttons.Triggered(ListOf_ConsoleInputs.DoubleTog4_UP))
        S.reactorControl.powerRouting.IncreeseAux();
      if (C.Buttons.Triggered(ListOf_ConsoleInputs.DoubleTog4_DOWN))
        S.reactorControl.powerRouting.DecreeseAux();
    }


    void SetPanelLeds()
    {
      SideControl L = _sws.LeftInput;
      SideControl R = _sws.LeftInput;
      ConsoleControl C = _sws.ConsoleInput;
      Starship S = _sws.PCShip;
      if (C.Buttons.IsDown(ListOf_ConsoleInputs.TopLeftToggle1))
      {
        C.rgbLed.TopLeftToggleLED[0] = 1;
        C.rgbLed.TopLeftToggleLED[1] = 0;
      }
      else
      {
        C.rgbLed.TopLeftToggleLED[0] = 0;
        C.rgbLed.TopLeftToggleLED[1] = 1;
      }
      if (C.Buttons.IsDown(ListOf_ConsoleInputs.TopLeftToggle2))
      {
        C.rgbLed.TopLeftToggleLED[2] = 1;
        C.rgbLed.TopLeftToggleLED[3] = 0;
      }
      else
      {
        C.rgbLed.TopLeftToggleLED[2] = 0;
        C.rgbLed.TopLeftToggleLED[3] = 1;
      }

      if (C.Buttons.IsDown(ListOf_ConsoleInputs.TopRightToggle1))
      {
        C.rgbLed.TopRightToggleLED[0] = 1;
        C.rgbLed.TopRightToggleLED[1] = 0;
      }
      else
      {
        C.rgbLed.TopRightToggleLED[0] = 0;
        C.rgbLed.TopRightToggleLED[1] = 1;
      }
      if (C.Buttons.IsDown(ListOf_ConsoleInputs.TopRightToggle2))
      {
        C.rgbLed.TopRightToggleLED[2] = 1;
        C.rgbLed.TopRightToggleLED[3] = 0;
      }
      else
      {
        C.rgbLed.TopRightToggleLED[2] = 0;
        C.rgbLed.TopRightToggleLED[3] = 1;
      }

      RgbLedControl.clearLED(C.rgbLed.PowerFarLeftLED, 5);
      RgbLedControl.clearLED(C.rgbLed.PowerNearLeftLED, 5);
      RgbLedControl.clearLED(C.rgbLed.PowerFarRightLED, 5);
      RgbLedControl.clearLED(C.rgbLed.PowerNearRightLED, 5);

      for (int i = 0; i < S.reactorControl.powerRouting.CM; i++)
      {
        C.rgbLed.PowerFarLeftLED[i] = 1;
      }
      for (int i = 0; i < S.reactorControl.powerRouting.Tact; i++)
      {
        C.rgbLed.PowerNearLeftLED[i] = 1;
      }
      for (int i = 0; i < S.reactorControl.powerRouting.Prop; i++)
      {
        C.rgbLed.PowerNearRightLED[i] = 1;
      }
      for (int i = 0; i < S.reactorControl.powerRouting.Aux; i++)
      {
        C.rgbLed.PowerFarRightLED[i] = 1;
      }



    }

  }

}



