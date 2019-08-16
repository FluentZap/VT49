using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;

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
    static XNamespace og = "http://www.opengis.net/kml/2.2";

    public List<SWPlanetInfo> ArchivePlanetInfo = new List<SWPlanetInfo>();

    public GalaxyMap(string name)
    {
      string path = FileLoader.LoadMap(name);
      XElement Planets = XElement.Load($"{path}");
      var list = Planets.Element(og + "Document").Element(og + "Folder").Elements(og + "Placemark");
      
      foreach (var planet in list)
      {
        ArchivePlanetInfo.Add( new SWPlanetInfo(){
          Name = GetName(planet),
          sector = GetString(planet, "sector"),
          objectid = GetNumber(planet, "objectid"),
          uid = GetNumber(planet, "uid"),
          link = GetString(planet, "link"),
          grid = GetString(planet, "grid"),
          x = GetFloat(planet, "x"),
          y = GetFloat(planet, "y"),
          cartodb_id =  GetNumber(planet, "cartodb_id"),
          region = GetString(planet, "region"),
          canon = GetNumber(planet, "canon") > 0,
          zm = GetNumber(planet, "zm"),
          name0 = GetString(planet, "name0"),
          name1 = GetString(planet, "name1"),
          name2 = GetString(planet, "name2"),
          lat = GetDouble(planet, "lat"),
          lon = GetDouble(planet, "long"),
          namede = GetString(planet, "namede"),
          linkde = GetString(planet, "linkde"),
        });
      }
    }

    static string GetString(XElement element, string key)
    {
      var value = element.Element(og + "ExtendedData").Element(og + "SchemaData").Elements().Where(x => x.Attribute("name").Value == key).FirstOrDefault();
      
      if (value != null)
      {
        return value.Value;
      }      
      return "";      
    }

    static int GetNumber(XElement element, string key)
    {
      var value = element.Element(og + "ExtendedData").Element(og + "SchemaData").Elements().Where(x => x.Attribute("name").Value == key).FirstOrDefault();
      if (value != null && int.TryParse(value.Value, out int returnValue))
      {
        return returnValue;
      }
      return 0;
    }

    static float GetFloat(XElement element, string key)
    {
      var value = element.Element(og + "ExtendedData").Element(og + "SchemaData").Elements().Where(x => x.Attribute("name").Value == key).FirstOrDefault();
      
      if (value != null && float.TryParse(value.Value, out float returnValue))
      {
        return returnValue;
      }      
      return 0f;
    }

    static double GetDouble(XElement element, string key)
    {
      var value = element.Element(og + "ExtendedData").Element(og + "SchemaData").Elements().Where(x => x.Attribute("name").Value == key).FirstOrDefault();
      if (value != null && double.TryParse(value.Value, out double returnValue))
      {
        return returnValue;
      }
      return 0f;
    }

    static string GetName(XElement element)
    {
      return element.Element(og + "name").Value;
    }

  }
}