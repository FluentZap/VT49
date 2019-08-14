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

  public class Starship
  {
    // string Callsign;
    // string TransponderID;
    // Type_PowerDistribution PowerDistribution;
    public Vector3 Location;
    public Vector3 LocationOffset;
    public Quat Rotation;
    public bool Left, Right, Up, Down;
    // void UpdateConsole(VTSerialParser * parser);
    // SDL_Point Sector_Location;    
  };

  public class SWSimulation
  {
    public int FPS;
    public int SPS;
    public Starship PCShip = new Starship();
    public Starship Station = new Starship() { Location = new Vector3(), Rotation = new Quat() };

    public List<Vector3> StationVectors = new List<Vector3>();    
    public ButtonSet<ListOf_ConsoleInputs> ConsoleInput = new ButtonSet<ListOf_ConsoleInputs>();

    public byte[] CylinderCode = new byte[15];
    
    // public HashSet<int> ConsolePressed = new HashSet<int>();
    // public HashSet<int> ConsoleKeyPressed = new HashSet<int>();

    public byte[] ConsoleAnalogValue = new byte[4];
    public byte[] LeftAnalogInput = new byte[6];
    public SideControl LeftInput = new SideControl(new [] 
    {
      new AnalogRange(0, 231),
      new AnalogRange(0, 237),
      new AnalogRange(0, 232),
      new AnalogRange(0, 238),
      new AnalogRange(0, 238),
      new AnalogRange(0, 238) 
    });

    public int inc;
    public byte col;

  }







}