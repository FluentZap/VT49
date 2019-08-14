using System;

namespace VT49
{
  class Program
  {
    static void Main(string[] args)
    {
      VTMain vtMain = new VTMain();
      vtMain.Start();
      vtMain.Dispose();
    }
  }
}
