using System;
using System.Collections.Generic;
using Quat = BepuUtilities.Quaternion;
using System.Numerics;

namespace VT49
{

  public enum ListOf_CollisionMesh
  {
    VT49,
    XQ6,
    Asteroid_Type1,
    Asteroid_Type2,
    Asteroid_Type3,
  }

  class SWSystem
  {
    public Dictionary<Guid, Station> Stations = new Dictionary<Guid, Station>();
    public Dictionary<Guid, Starship> Starships = new Dictionary<Guid, Starship>();

    public SWSystem()
    {

    }
  }

  public class MeshObject
  {
    public Vector3 Location;
    public Quat Rotation;
    public ListOf_CollisionMesh CollisionMesh;
  }

  public class ShipObject : MeshObject
  {
    public string Callsign;
    public string TransponderID;
  }


  public class Station : ShipObject
  {

  }


  public class Starship : ShipObject
  {
    public bool Left, Right, Up, Down;    
  };


}