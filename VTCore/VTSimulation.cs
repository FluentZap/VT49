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


    }

  }

}


