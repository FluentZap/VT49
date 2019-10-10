using System;
using System.Drawing;
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
      _sws.time++;

      _sws.PCShip.LeftControlInterface.Alerts.Add(ListOf_ControlInterfaceCategory.White_Yellow);

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

      ClearLights();

      if (
          _sws.PCShip.LeftControlInterface.LoginID != "" ||
          _sws.PCShip.RightControlInterface.LoginID != "" ||
          _sws.PCShip.CenterControlInterface.LoginID != "")
      {
        SetPanelLeds();
        HandleConsole();
      }

      HandleLogin();
    }

    void HandleLogin()
    {
      if (_sws.ConsoleInput.Buttons.IsDown(ListOf_ConsoleInputs.LeftBoxTog4) &&
          _sws.ConsoleInput.Buttons.Triggered(ListOf_ConsoleInputs.ControlLED2))
      {
        _sws.ConsoleInput.CylinderLogin = true;
      }
    }


    public void ClearLights()
    {
      SideControl L = _sws.LeftInput;
      SideControl R = _sws.RightInput;
      ConsoleControl C = _sws.ConsoleInput;
      lock (L)
      {
        L.LEDs.Clear();
      }
      lock (R)
      {
        R.LEDs.Clear();
      }
      lock (C)
      {
        C.LEDs.Clear();
      }

      RgbLedControl.clearLED(L.rgbLed.EightControlLED);
      RgbLedControl.clearLED(L.rgbLed.MatrixControlLED);
      RgbLedControl.clearLED(L.rgbLed.MatrixGuideLED);
      RgbLedControl.clearLED(L.rgbLed.MatrixLED);
      RgbLedControl.clearLED(L.rgbLed.TargetControlLED);
      RgbLedControl.clearLED(L.rgbLed.ThrottleLED);

      RgbLedControl.clearLED(R.rgbLed.EightControlLED);
      RgbLedControl.clearLED(R.rgbLed.MatrixControlLED);
      RgbLedControl.clearLED(R.rgbLed.MatrixGuideLED);
      RgbLedControl.clearLED(R.rgbLed.MatrixLED);
      RgbLedControl.clearLED(R.rgbLed.TargetControlLED);
      RgbLedControl.clearLED(R.rgbLed.ThrottleLED);

      RgbLedControl.clearLED(C.rgbLed.CenterToggleLeftLED);
      RgbLedControl.clearLED(C.rgbLed.CenterToggleRightLED);

      RgbLedControl.clearLED(C.rgbLed.PowerFarLeftLED);
      RgbLedControl.clearLED(C.rgbLed.PowerFarRightLED);
      RgbLedControl.clearLED(C.rgbLed.PowerNearLeftLED);
      RgbLedControl.clearLED(C.rgbLed.PowerNearRightLED);
      RgbLedControl.clearLED(C.rgbLed.RotLeftLED);
      RgbLedControl.clearLED(C.rgbLed.RotRightLED);
      RgbLedControl.clearLED(C.rgbLed.TopLeftToggleLED);
      RgbLedControl.clearLED(C.rgbLed.TopRightToggleLED);
    }

    void HandleConsole()
    {
      SideControl L = _sws.LeftInput;
      SideControl R = _sws.RightInput;
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

      if (C.Buttons.IsUp(ListOf_ConsoleInputs.TopLeftToggle1))
      {
        R.LEDs.SetOn(ListOf_SideOutputs.ThrottleLED1);
        R.LEDs.SetOn(ListOf_SideOutputs.ThrottleLED2);
        R.LEDs.SetOn(ListOf_SideOutputs.ThrottleLEDToggle);
        S.RightFlightControl = true;
      }
      else
      {
        L.LEDs.SetOn(ListOf_SideOutputs.ThrottleLED1);
        L.LEDs.SetOn(ListOf_SideOutputs.ThrottleLED2);
        L.LEDs.SetOn(ListOf_SideOutputs.ThrottleLEDToggle);
        S.RightFlightControl = false;
      }

      if (C.Buttons.IsDown(ListOf_ConsoleInputs.FlightStickUP))
        _sws.NavMapScroll.Y++;
      if (C.Buttons.IsDown(ListOf_ConsoleInputs.FlightStickDOWN))
        _sws.NavMapScroll.Y--;
      if (C.Buttons.IsDown(ListOf_ConsoleInputs.FlightStickLEFT))
        _sws.NavMapScroll.X++;
      if (C.Buttons.IsDown(ListOf_ConsoleInputs.FlightStickRIGHT))
        _sws.NavMapScroll.X--;

      _sws.NavMapScroll.Y = Math.Clamp(_sws.NavMapScroll.Y, -500, 500);
      _sws.NavMapScroll.X = Math.Clamp(_sws.NavMapScroll.X, -500, 500);

      SideControl Side;
      if (_sws.PCShip.RightFlightControl)
      {
        Side = _sws.RightInput;
      }
      else
      {
        Side = _sws.LeftInput;
      }
      
      if(Side.Buttons.Triggered(ListOf_SideInputs.ThrottleLEDButton1))
      {
        _sws.PCShip.EngineSpeed = 300;
      }

      if (Side.Buttons.Triggered(ListOf_SideInputs.ThrottleLEDButton3))
      {
        _sws.PCShip.EngineSpeed = 0;
      }

    }

    void SetPanelLeds()
    {
      SideControl L = _sws.LeftInput;
      SideControl R = _sws.RightInput;
      ConsoleControl C = _sws.ConsoleInput;
      Starship S = _sws.PCShip;

      C.LEDs.SetOn(ListOf_ConsoleOutputs.FlightStickLED);

      //Panel Control Toggle
      if (C.Buttons.IsDown(ListOf_ConsoleInputs.TopLeftToggle1))
        C.rgbLed.TopLeftToggleLED[0] = 1;
      else
        C.rgbLed.TopLeftToggleLED[1] = 1;
      if (C.Buttons.IsDown(ListOf_ConsoleInputs.TopLeftToggle2))
        C.rgbLed.TopLeftToggleLED[2] = 1;
      else
        C.rgbLed.TopLeftToggleLED[3] = 1;

      if (C.Buttons.IsDown(ListOf_ConsoleInputs.TopRightToggle1))
        C.rgbLed.TopRightToggleLED[0] = 1;
      else
        C.rgbLed.TopRightToggleLED[1] = 1;
      if (C.Buttons.IsDown(ListOf_ConsoleInputs.TopRightToggle2))
        C.rgbLed.TopRightToggleLED[2] = 1;
      else
        C.rgbLed.TopRightToggleLED[3] = 1;

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


      //Set Led on and off Color
      C.rgbLed.ColorIndex[0] = Color.FromArgb(C.AnalogInput(0), C.AnalogInput(1), C.AnalogInput(2));
      C.rgbLed.ColorIndex[1] = Color.FromArgb(0, 0, 0);

      L.rgbLed.ColorIndex[0] = Color.FromArgb(C.AnalogInput(0), C.AnalogInput(1), C.AnalogInput(2));
      L.rgbLed.ColorIndex[1] = Color.FromArgb(0, 0, 0);

      R.rgbLed.ColorIndex[0] = Color.FromArgb(C.AnalogInput(0), C.AnalogInput(1), C.AnalogInput(2));
      R.rgbLed.ColorIndex[1] = Color.FromArgb(0, 0, 0);

      //Set Engine Speed
      for (int i = 0; i < _sws.PCShip.EngineSpeed / 30; i++)
      {
        if (i < 5)
        {
          L.rgbLed.ThrottleLED[i] = 1;
          R.rgbLed.ThrottleLED[i] = 1;
        }
      }
      SetAlerts(S.LeftControlInterface, L);
      SetAlerts(S.RightControlInterface, R);
      SetAlerts(S.CenterControlInterface, null, C);


    }

    void SetAlerts(ControlInterface control, SideControl side, ConsoleControl console = null)
    {
      int timeBracket = (int)(_sws.time % 180);
      if (timeBracket < 45)
      {
        if (control.Alerts.Contains(ListOf_ControlInterfaceCategory.Red_Red) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Red_White) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Red_Yellow) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Red_Green))
        {
          if (side != null)
          {
            side.LEDs.SetOn(ListOf_SideOutputs.ControlLED1);
          }
          else
          {
            console.LEDs.SetOn(ListOf_ConsoleOutputs.ControlLED1);
          }
        }
        if (control.Alerts.Contains(ListOf_ControlInterfaceCategory.White_Red) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.White_White) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.White_Yellow) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.White_Green))
        {
          if (side != null)
          {
            side.LEDs.SetOn(ListOf_SideOutputs.ControlLED2);
          }
          else
          {
            console.LEDs.SetOn(ListOf_ConsoleOutputs.ControlLED2);
          }
        }
        if (control.Alerts.Contains(ListOf_ControlInterfaceCategory.Yellow_Red) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Yellow_White) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Yellow_Yellow) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Yellow_Green))
        {
          if (side != null)
          {
            side.LEDs.SetOn(ListOf_SideOutputs.ControlLED3);
          }
          else
          {
            console.LEDs.SetOn(ListOf_ConsoleOutputs.ControlLED3);
          }
        }
        if (control.Alerts.Contains(ListOf_ControlInterfaceCategory.Green_Red) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Green_White) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Green_Yellow) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Green_Green))
        {
          if (side != null)
          {
            side.LEDs.SetOn(ListOf_SideOutputs.ControlLED4);
          }
          else
          {
            console.LEDs.SetOn(ListOf_ConsoleOutputs.ControlLED4);
          }
        }
      }
      else if (timeBracket > 45 && timeBracket < 90)
      {
        if (control.Alerts.Contains(ListOf_ControlInterfaceCategory.Red_Red) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.White_Red) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Yellow_Red) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Green_Red))
        {
          if (side != null)
          {
            side.LEDs.SetOn(ListOf_SideOutputs.ControlLED1);
          }
          else
          {
            console.LEDs.SetOn(ListOf_ConsoleOutputs.ControlLED1);
          }
        }
        if (control.Alerts.Contains(ListOf_ControlInterfaceCategory.Red_White) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.White_White) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Yellow_White) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Green_White))
        {
          if (side != null)
          {
            side.LEDs.SetOn(ListOf_SideOutputs.ControlLED2);
          }
          else
          {
            console.LEDs.SetOn(ListOf_ConsoleOutputs.ControlLED2);
          }
        }
        if (control.Alerts.Contains(ListOf_ControlInterfaceCategory.Red_Yellow) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.White_Yellow) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Yellow_Yellow) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Green_Yellow))
        {
          if (side != null)
          {
            side.LEDs.SetOn(ListOf_SideOutputs.ControlLED3);
          }
          else
          {
            console.LEDs.SetOn(ListOf_ConsoleOutputs.ControlLED3);
          }
        }
        if (control.Alerts.Contains(ListOf_ControlInterfaceCategory.Red_Green) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.White_Green) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Yellow_Green) ||
            control.Alerts.Contains(ListOf_ControlInterfaceCategory.Green_Green))
        {
          if (side != null)
          {
            side.LEDs.SetOn(ListOf_SideOutputs.ControlLED4);
          }
          else
          {
            console.LEDs.SetOn(ListOf_ConsoleOutputs.ControlLED4);
          }
        }
      }
    }


  }
}
