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
      List<SWPlanetInfo> planets = _sws.galaxyMap.ArchivePlanetInfo.Where(x => x.grid == "K9").ToList();
      // System.Console.WriteLine(planets);      
    }





    // public static 

  }
}