using System.IO;
using System.Reflection;

namespace VT49
{
  public static class FileLoader
  {

#if DEBUG
    static string LoadDirectory = Directory.GetCurrentDirectory();
#else
      static string LoadDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif


    public static string LoadMap(string fileName)
    {
      return Path.Combine(LoadDirectory, "SWMaps", fileName);
    }

    public static string LoadMesh(string fileName)
    {
      return Path.Combine(LoadDirectory, "assets", "meshes", fileName);
    }

    public static string LoadImage(string fileName)
    {
      return Path.Combine(LoadDirectory, "assets", "images", fileName);
    }

    public static string LoadFont(string fileName)
    {
      return Path.Combine(LoadDirectory, "assets", "fonts", fileName);
    }

  }
}