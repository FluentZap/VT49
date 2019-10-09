using System;
using System.Linq;
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
    public ReactorControl reactorControl = new ReactorControl();

  };


  public class ReactorControl
  {
    public PowerRouting powerRouting = new PowerRouting();

  }

  public class PowerRouting
  {

    public byte CM { get => _cm; }
    public byte Tact { get => _tact; }
    public byte Prop { get => _prop; }
    public byte Aux { get => _aux; }

    byte _cm = 2;
    byte _tact = 2;
    byte _prop = 2;
    byte _aux = 2;


    public void IncreeseProp()
    {
      if (_prop >= 5)
        return;
      _prop++;
      if (_aux >= _cm && _aux > 0)
      {
        _aux--;
        return;
      }
      if (_cm > _aux && _cm > 0)
      {
        _cm--;
        return;
      }
      if (_tact > 0)
      {
        _tact--;
        return;
      }
    }

    public void IncreeseTact()
    {
      if (_tact >= 5)
        return;
      _tact++;
      if (_aux >= _cm && _aux > 0)
      {
        _aux--;
        return;
      }
      if (_cm > _aux && _cm > 0)
      {
        _cm--;
        return;
      }
      if (_prop > 0)
      {
        _prop--;
        return;
      }
    }

    public void IncreeseCm()
    {
      if (_cm >= 5)
        return;
      _cm++;
      if (_aux > 0)
      {
        _aux--;
        return;
      }
      if (_tact >= _prop && _tact > 0)
      {
        _tact--;
        return;
      }
      if (_prop > _tact && _prop > 0)
      {
        _prop--;
        return;
      }

    }

    public void IncreeseAux()
    {
      if (_aux >= 5)
        return;
      _aux++;
      if (_cm > 0)
      {
        _cm--;
        return;
      }
      if (_tact >= _prop && _tact > 0)
      {
        _tact--;
        return;
      }
      if (_prop > _tact && _prop > 0)
      {
        _prop--;
        return;
      }
    }

    public void DecreeseProp()
    {
      if (_prop <= 0)
        return;
      _prop--;
      if (_tact < 5)
      {
        _tact++;
        return;
      }
      if (_cm <= _aux && _cm < 5)
      {
        _cm++;
        return;
      }
      if (_aux < _cm && _aux < 5)
      {
        _aux++;
        return;
      }
    }

    public void DecreeseTact()
    {
      if (_tact <= 0)
        return;
      _tact--;
      if (_prop < 5)
      {
        _prop++;
        return;
      }
      if (_cm <= _aux && _cm < 5)
      {
        _cm++;
        return;
      }
      if (_aux < _cm && _aux < 5)
      {
        _aux++;
        return;
      }
    }

    public void DecreeseCm()
    {
      if (_cm <= 0)
        return;
      _cm--;
      if (_prop <= _tact && _prop < 5)
      {
        _prop++;
        return;
      }
      if (_tact < _prop && _tact < 5)
      {
        _tact++;
        return;
      }

      if (_aux < 5)
      {
        _aux++;
        return;
      }
    }

    public void DecreeseAux()
    {
      if (_aux <= 0)
        return;
      _aux--;
      if (_prop <= _tact && _prop < 5)
      {
        _prop++;
        return;
      }
      if (_tact < _prop && _tact < 5)
      {
        _tact++;
        return;
      }

      if (_cm < 5)
      {
        _cm++;
        return;
      }
    }
  }


}