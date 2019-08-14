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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ObjLoader.Loader.Loaders;

namespace VT49
{
  static class MeshLoader
  {
    static string GetPath()
    {
      return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    }

    public static Stream GetFileStream(string name)
    {
      string path = Path.Combine(GetPath(), name);
      System.Console.WriteLine(path);
      if (File.Exists(path))
      {
        return new FileStream(path, FileMode.Open);
      }
      else
      {
        return null;
      }
    }

    public static List<Vector3> LoadPointsFromFile(string name)
    {
      string path = Path.Combine(GetPath(), name);
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
        return vectors;
      }
      else
      {
        return new List<Vector3>();
      }
    }

    public static Buffer<Vector3> LoadPointsFromFile(BufferPool pool, string name)
    {
      string path = Path.Combine(GetPath(), name);
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

    class MaterialStubLoader : IMaterialStreamProvider
    {
      public Stream Open(string materialFilePath)
      {
        return null;
      }
    }

    public static Mesh LoadTriangleMesh(BufferPool pool, string name, Vector3 scale)
    {
      var triangles = new List<Triangle>();
      var result = new ObjLoaderFactory().Create(new MaterialStubLoader()).Load(GetFileStream(name));

      for (int i = 0; i < result.Groups.Count; ++i)
      {
        var group = result.Groups[i];
        for (int j = 0; j < group.Faces.Count; ++j)
        {
          var face = group.Faces[j];
          var a = result.Vertices[face[0].VertexIndex - 1];          
          for (int k = 1; k < face.Count - 1; ++k)
          {
            var b = result.Vertices[face[k].VertexIndex - 1];
            var c = result.Vertices[face[k + 1].VertexIndex - 1];
            triangles.Add(new Triangle
            {
              A = new Vector3(a.X, a.Y, a.Z),
              B = new Vector3(b.X, b.Y, b.Z),
              C = new Vector3(c.X, c.Y, c.Z)
            });
          }
        }
      }

      pool.Take<Triangle>(triangles.Count, out var meshTriangles);
      for (int i = 0; i < triangles.Count; ++i)
      {
        meshTriangles[i] = triangles[i];
      }
      Mesh mesh = new Mesh(meshTriangles, scale, pool);
      return mesh;
    }
  }
}