using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using System;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace VT49
{
  static class MeshLoader
  { 
    public static Buffer<Vector3> LoadPointsFromFile(BufferPool pool, string name)
    {
      string path = Directory.GetCurrentDirectory() + "\\" + name;
      if (File.Exists(path))
      {
        string[] lines = File.ReadAllLines(path);
        List<Vector3> vectors = new List<Vector3>();
        
        foreach (string line in lines)
        {
          if (ReadVector3(line, out var newVector))
          {
            vectors.Add(newVector);
          }
        }

        pool.Take<Vector3>(vectors.Count, out var points);
        for (int i = 0; i < vectors.Count; i++)
        {
          points[i] = vectors[i];
        }        
        return points;
      }
      else
      {
        return new Buffer<Vector3>();
      }

    }

    static bool ReadVector3 (string line, out Vector3 vec)
    {      
      string[] points = line.Split(' ');
      float x, y, z;
      if (points[0] == "v")
      {
        float.TryParse(points[1], out x);
        float.TryParse(points[2], out y);
        float.TryParse(points[3], out z);
        vec = new Vector3(x, y, z);
        return true;
      }
      else
      {
        vec = Vector3.Zero;
        return false;
      }
    }



    public static Buffer<Vector3> LoadVT49(BufferPool pool)
    {
      pool.Take<Vector3>(35, out var points);
      int i = 0;
      points[i++] = new Vector3(-0.392225f, 0.445773f, 1.298571f);
      points[i++] = new Vector3(-0.321504f, 0.142857f, 1.354454f);
      points[i++] = new Vector3(0.315040f, 0.475524f, 1.303680f);
      points[i++] = new Vector3(0.319171f, 0.153855f, 1.362038f);
      points[i++] = new Vector3(-0.044715f, 0.603622f, 1.234881f);
      points[i++] = new Vector3(-0.044715f, 0.039295f, 1.342264f);
      points[i++] = new Vector3(-1.273956f, 0.500396f, -0.978231f);
      points[i++] = new Vector3(-1.822653f, -0.135699f, 0.512451f);
      points[i++] = new Vector3(-1.028348f, -0.146549f, 1.103464f);
      points[i++] = new Vector3(-1.395651f, -0.015939f, -2.999074f);
      points[i++] = new Vector3(-0.759244f, 0.186102f, -4.263649f);
      points[i++] = new Vector3(1.387395f, -0.077494f, -2.993922f);
      points[i++] = new Vector3(0.755864f, 0.104236f, -4.252594f);
      points[i++] = new Vector3(1.019557f, -0.163134f, 1.116892f);
      points[i++] = new Vector3(1.785758f, -0.009637f, 0.526791f);
      points[i++] = new Vector3(-0.278269f, -0.284762f, -2.699013f);
      points[i++] = new Vector3(0.269602f, -0.281323f, -2.698989f);
      points[i++] = new Vector3(-1.815870f, -0.016918f, 0.265320f);
      points[i++] = new Vector3(-1.745630f, -0.127614f, -0.279237f);
      points[i++] = new Vector3(1.733191f, -0.135700f, -0.287202f);
      points[i++] = new Vector3(1.731873f, 0.084479f, -0.266485f);
      points[i++] = new Vector3(1.822581f, -0.016918f, 0.235001f);
      points[i++] = new Vector3(-0.011835f, 0.900874f, 0.502585f);
      points[i++] = new Vector3(-0.786501f, 0.700257f, 0.634768f);
      points[i++] = new Vector3(-0.786501f, 0.700257f, 0.173596f);
      points[i++] = new Vector3(-0.718642f, 0.704622f, 1.062089f);
      points[i++] = new Vector3(0.718991f, 0.713707f, 0.138557f);
      points[i++] = new Vector3(0.718643f, 0.718318f, 1.062088f);
      points[i++] = new Vector3(-0.018844f, -0.528379f, -0.095989f);
      points[i++] = new Vector3(-1.542503f, -0.054984f, 0.973085f);
      points[i++] = new Vector3(-1.290621f, -0.039728f, 1.157194f);
      points[i++] = new Vector3(1.300392f, 0.025738f, 1.157193f);
      points[i++] = new Vector3(1.531108f, -0.008693f, 0.973085f);
      points[i++] = new Vector3(-1.120558f, -0.039700f, 1.268800f);
      points[i++] = new Vector3(1.071557f, 0.048009f, 1.305767f);
      return points;
    }
  }
}