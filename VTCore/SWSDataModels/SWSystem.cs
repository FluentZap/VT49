using System;
using System.Collections.Generic;
using BepuUtilities;
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

  public class SWSystem
  {
    public Dictionary<Guid, Station> Stations = new Dictionary<Guid, Station>();
    public Dictionary<Guid, Starship> Starships = new Dictionary<Guid, Starship>();

    public Dictionary<Guid, SpaceObject> Objects = new Dictionary<Guid, SpaceObject>();

    public SWSystem()
    {
      Random rnd = new Random();
      GenerateObjects(rnd);

    }

    void GenerateObjects(Random rnd)
    {
      UInt16 Id = 0;
      float RndDeg()
      {
        return MathHelper.ToRadians(rnd.Next(0, 360));
      }
      int count = rnd.Next(30, 30);
      for (int i = 0; i < count; i++)
      {
        Vector3 location = new Vector3
        (
          rnd.Next(-500, 500),
          rnd.Next(-200, 200),
          rnd.Next(-500, 500)
        );
        Quat rotation = Quat.CreateFromYawPitchRoll
        (
          RndDeg(),
          RndDeg(),
          RndDeg()
        );
        float scale;
        if (rnd.Next(5) == 0)
        {
          scale = 10 + (float)(rnd.NextDouble() * 20);
        }
        else
        {
          scale = 1 + (float)(rnd.NextDouble() * 4);
        }
        int ObjectType = rnd.Next(2, 5);

        Objects.Add(Guid.NewGuid(), new SpaceObject()
        {
          CollisionMesh = (ListOf_CollisionMesh)ObjectType,
          Location = location,
          Rotation = rotation,
          Scale = new Vector3(scale),
          // Scale = new Vector3(1),
          Id = Id++
        });
      }

      Objects.Add(Guid.NewGuid(), new SpaceObject()
      {
        CollisionMesh = ListOf_CollisionMesh.XQ6,
        Location = new Vector3(0, 0, 1000),
        Rotation = Quat.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(45)),
        Scale = Vector3.One,
        Id = Id++
      });


    }
  }

  public class MeshObject
  {
    public UInt16 Id;
    public bool PhysicsUpdated = false;
    public int PhysicsId;
    public Vector3 Location;
    public Quat Rotation;
    public ListOf_CollisionMesh CollisionMesh;
  }

  public class ShipObject : MeshObject
  {
    public string Callsign;
    public string TransponderID;
  }

  public class SpaceObject : MeshObject
  {
    public Vector3 Scale;
  }


  public class Station : ShipObject
  {

  }


  public class Starship : ShipObject
  {
    public bool Left, Right, Up, Down;
    public float EngineSpeed = 0;
  };


}