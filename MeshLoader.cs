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

    public static void LoadMeshFromFile(string name)
    {
      string path = Directory.GetCurrentDirectory() + "\\" + name;
      if (File.Exists(path))
      {
        using (var stream = File.OpenRead(path))
        {
          TriangleContent[] vectors = Load(stream);
        }
      }
    }

    public struct TriangleContent
    {
      public Vector3 A;
      public Vector3 B;
      public Vector3 C;
    }

    public static void ReadVector3(BinaryReader reader, out Vector3 v)
    {
      v = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }


    static TriangleContent[] Load(Stream stream)
    {
      using (BinaryReader reader = new BinaryReader(stream))
      {
        // var count = reader.ReadInt32();
        var count = 8;
        var triangles = new TriangleContent[count];

        for (int i = 0; i < count; i++)
        {
          ref var triangle = ref triangles[i];
          ReadVector3(reader, out triangle.A);
          ReadVector3(reader, out triangle.B);
          ReadVector3(reader, out triangle.C);
        }
        return triangles;
      }
    }

  }
}