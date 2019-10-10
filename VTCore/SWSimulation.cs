using System.Collections.Generic;
using BepuUtilities;
using Quat = BepuUtilities.Quaternion;
using System.Numerics;

namespace VT49
{
  // class PlayerStats
  // {
  //   string name;
  //   byte brawn, agility, intelect, cunning, willpower, presence;
  //   byte astrogation, athletics, brawl, charm, coercion, computers, cool, coordination, coreWorlds,
  //   deception, discipline, education, gunnery, leadership, lightsaber, lore, mechanics, medicine,
  //   melee, negotiation, outerRim, perception, pilotingPlanetary, pilotingSpace, rangedHeavy,
  //   rangedLight, resilience, skulduggery, stealth, streetwise, survival, underworld, vigilance,
  //   xenology, warfare, cybernetics;
  // }  

  // public class Starship
  // {
  //   // string Callsign;
  //   // string TransponderID;
  //   // Type_PowerDistribution PowerDistribution;
  //   public Vector3 Location;
  //   public Vector3 LocationOffset;
  //   public Quat Rotation;
  //   public bool Left, Right, Up, Down;
  //   // void UpdateConsole(VTSerialParser * parser);
  //   // SDL_Point Sector_Location;    
  // };

  public class SWSimulation
  {
    public ulong time = 0;
    public GalaxyMap galaxyMap = new GalaxyMap("planets.kml");

    public int DiagnosticModeUnlock = 0;
    public bool DiagnosticMode = false;

    public int test = 0;
    public int test2 = 0;

    public byte SideHeader = 1;

    public int FPS;
    public int[] SPSSend = new int[3];
    public int[] SPSSend_ticks = new int[3];

    public int[] SPSReceive = new int[6];
    public int[] SPSReceive_ticks = new int[6];
    public SWSystem swSystem = new SWSystem();

    public Starship PCShip = new Starship();
    public Starship Station = new Starship() { Location = new Vector3(), Rotation = new Quat() };

    public List<Vector3> StationVectors = new List<Vector3>();

    public Vector2 NavMapScroll = new Vector2();

    public IEnumerable<SWPlanetInfo> LoadedSystem;

    // public ButtonSet<ListOf_ConsoleInputs> ConsoleInput = new ButtonSet<ListOf_ConsoleInputs>();    

    // public HashSet<int> ConsolePressed = new HashSet<int>();
    // public HashSet<int> ConsoleKeyPressed = new HashSet<int>();

    // public byte[] ConsoleAnalogValue = new byte[4];
    // public byte[] LeftAnalogInput = new byte[6];

    public SideControl LeftInput = new SideControl(new[]
    {
      new AnalogRange(0, 255),
      new AnalogRange(0, 255),
      new AnalogRange(0, 249),
      new AnalogRange(0, 255),
      new AnalogRange(0, 255),
      new AnalogRange(8, 255),
    });

    public SideControl RightInput = new SideControl(new[]
    {
      new AnalogRange(0, 253),
      new AnalogRange(0, 253),
      new AnalogRange(0, 253),
      new AnalogRange(0, 253),
      new AnalogRange(0, 253),
      new AnalogRange(2, 253),
    });

    public ConsoleControl ConsoleInput = new ConsoleControl(new[]
    {
      new AnalogRange(0, 255),
      new AnalogRange(0, 255),
      new AnalogRange(0, 255),
      new AnalogRange(0, 252),
    });

    public int inc;
    public byte col;

  }







}