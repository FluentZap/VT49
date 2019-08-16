using System;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Schemas;

namespace VT49
{ 

  public class SWPlanetInfo
  {
    public string Name;
    public string sector;
		public int objectid;
		public int uid;
		public string link;
		public string grid;
		public float x;
		public float y;
		public int cartodb_id;
		public string region;
		public bool canon;
		public int zm;
		public string name0;
		public string name1;
		public string name2;
		public double lat;
		public double lon;
		public string namede;
		public string linkde;
  }


  public class GalaxyMap
  {

    public GalaxyMap(string name)
    {
      XNamespace og = "http://www.opengis.net/kml/2.2";
      string path = FileLoader.LoadMap(name);
      XElement Planets = XElement.Load($"{path}");

      // IEnumerable<XElement> list = Planets.Element("kml").Element("Document").Element("Folder").Elements("Placemark");
      var list = Planets.Element(og + "Document").Element(og + "Folder").Elements();
      List<SWPlanetInfo> PlanetInfo = new List<SWPlanetInfo>();

      foreach (var planet in list)
      {
        PlanetInfo.Add( new SWPlanetInfo(){
          Name = planet.Element(og + "name").ToString(),
          // sector = planet.Elements(og + "SimpleData").Attributes(og + "sector").ToString(),
          // objectid = int.Parse(planet.Element(og + "SimpleData").Attributes(og + "objectid").ToString()),
          // uid = int.Parse(planet.Element(og + "SimpleData").Attributes(og + "uid").ToString()),
          // link = planet.Element(og + "SimpleData").Attributes(og + "link").ToString(),
          // grid = planet.Element(og + "SimpleData").Attributes(og + "grid").ToString(),
          // x = float.Parse(planet.Element(og + "SimpleData").Attributes(og + "x").ToString()),
          // y = float.Parse(planet.Element(og + "SimpleData").Attributes(og + "y").ToString()),
          // cartodb_id = int.Parse(planet.Element(og + "SimpleData").Attributes(og + "cartodb_id").ToString()),
          // region = planet.Element(og + "SimpleData").Attributes(og + "region").ToString(),
          // canon = bool.Parse(planet.Element(og + "SimpleData").Attributes(og + "canon").ToString()),
          // zm =int.Parse(planet.Element(og + "SimpleData").Attributes(og + "zm").ToString()),
          // name0 = planet.Element(og + "SimpleData").Attributes(og + "name0").ToString(),
          // name1 = planet.Element(og + "SimpleData").Attributes(og + "name1").ToString(),
          // name2 = planet.Element(og + "SimpleData").Attributes(og + "name2").ToString(),
          // lat = double.Parse(planet.Element(og + "SimpleData").Attributes(og + "lat").ToString()),
          // lon = double.Parse(planet.Element(og + "SimpleData").Attributes(og + "long").ToString()),
          // namede = planet.Element(og + "SimpleData").Attributes(og + "namede").ToString(),
          // linkde = planet.Element(og + "SimpleData").Attributes(og + "linkde").ToString(),
        });        
      }

    }
  }
}