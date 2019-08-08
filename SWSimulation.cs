using System.Collections.Generic;

namespace VT49
{
  class PlayerStats
  {
    string name;
    byte brawn, agility, intelect, cunning, willpower, presence;
    byte astrogation, athletics, brawl, charm, coercion, computers, cool, coordination, coreWorlds,
    deception, discipline, education, gunnery, leadership, lightsaber, lore, mechanics, medicine,
    melee, negotiation, outerRim, perception, pilotingPlanetary, pilotingSpace, rangedHeavy,
    rangedLight, resilience, skulduggery, stealth, streetwise, survival, underworld, vigilance,
    xenology, warfare, cybernetics;
  }

  public class Starship
  {
    // string Callsign;
    // string TransponderID;
    // Type_PowerDistribution PowerDistribution;
    public float x, y, z;
    // void UpdateConsole(VTSerialParser * parser);
    // SDL_Point Sector_Location;
  };

  public class SWSimulation
  {
    public int FPS;
    public int SPS;
    public Starship PCShip = new Starship();
    public HashSet<int> ConsolePressed = new HashSet<int>();
    public HashSet<int> ConsoleKeyPressed = new HashSet<int>();

    public int inc;
    public byte col;

  }

}