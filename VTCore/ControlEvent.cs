using System;
using System.Numerics;
using System.Collections.Generic;

namespace VT49
{
  public enum ListOf_ConsoleInputs : int
  {
    DoubleTog1_UP,
    DoubleTog1_DOWN,
    DoubleTog2_UP,
    DoubleTog2_DOWN,
    DoubleTog3_UP,
    DoubleTog3_DOWN,
    DoubleTog4_UP,
    DoubleTog4_DOWN,
    LEDToggle1,
    LEDToggle2,
    LEDToggle3,
    LEDToggle4,
    LEDToggle5,
    TopLeftToggle1,
    TopLeftToggle2,
    TopRightToggle1,
    TopRightToggle2,
    PotButton1,
    PotButton2,
    LEDButton1,
    LEDButton2,
    LEDButton3,
    LEDButton4,
    LeftBoxTog1,
    LeftBoxTog2,
    LeftBoxTog3,
    LeftBoxTog4,
    LeftBoxTog5,
    LeftBoxTog6,
    LeftBoxTog7,
    LeftBoxTog8,
    RightBoxTog1,
    RightBoxTog2,
    RightBoxTog3,
    RightBoxTog4,
    RightBoxTog5,
    RightBoxTog6,
    RightBoxTog7,
    RightBoxTog8,
    FlightStickUP,
    FlightStickDOWN,
    FlightStickLEFT,
    FlightStickRIGHT
  };

  public enum ListOf_SideInputs : int
  {
    DoubleTog1_UP,
    DoubleTog1_DOWN,
    DoubleTog2_UP,
    DoubleTog2_DOWN,
    DoubleTog3_UP,
    DoubleTog3_DOWN,
    DoubleTog4_UP,
    DoubleTog4_DOWN,
    LEDToggle1,
    LEDToggle2,
    LEDToggle3,
    LEDToggle4,
    LEDToggle5,
    TopLeftToggle1,
    TopLeftToggle2,
    TopRightToggle1,
    TopRightToggle2,
    PotButton1,
    PotButton2,
    LEDButton1,
    LEDButton2,
    LEDButton3,
    LEDButton4,
    LeftBoxTog1,
    LeftBoxTog2,
    LeftBoxTog3,
    LeftBoxTog4,
    LeftBoxTog5,
    LeftBoxTog6,
    LeftBoxTog7,
    LeftBoxTog8,
    RightBoxTog1,
    RightBoxTog2,
    RightBoxTog3,
    RightBoxTog4,
    RightBoxTog5,
    RightBoxTog6,
    RightBoxTog7,
    RightBoxTog8,
    FlightStickUP,
    FlightStickDOWN,
    FlightStickLEFT,
    FlightStickRIGHT
  };

  public class ButtonSet<T>
  {
    HashSet<T> _triggered = new HashSet<T>();
    HashSet<T> _down = new HashSet<T>();

    public void Set(T key, bool value)
    {
      if (value)
      {
        SetDown(key);
      }
      else
      {
        SetUp(key);
      }
    }

    public void SetDown(T key)
    {
      if (!_down.Contains(key))
      {
        _triggered.Add(key);
        _down.Add(key);
      }
    }

    public void SetUp(T key)
    {
      _down.Remove(key);
    }

    public bool IsDown(T key)
    {
      return _down.Contains(key);
    }

    public bool IsUp(T key)
    {
      return !_down.Contains(key);
    }

    public bool Triggered(T key, bool keep = false)
    {
      if (keep)
      {
        return _triggered.Contains(key);
      }
      else
      {
        return _triggered.Remove(key);
      }
    }
  }
  

  public struct AnalogRange
  {
    public int Lower { get; set; }
    public int Upper { get; set; }
    public AnalogRange(int lower, int upper)
    {
      Lower = lower;
      Upper = upper;
    }
  }

  public class SideControl
  {
    public ButtonSet<ListOf_SideInputs> Buttons = new ButtonSet<ListOf_SideInputs>();
    public byte[] analogInputRaw = new byte[6];
    AnalogRange[] analogRange;

    public FlightStickControl FlightStick = new FlightStickControl();

    public SideControl(AnalogRange[] range)
    {
      analogRange = range;
    }

    public byte AnalogInput(int id)
    {
      if (id >= 0 &&  id < analogInputRaw.Length)
      {
        //Calculate Pot Deadzone
        return (byte)Math.Clamp((
          (255f / (analogRange[id].Upper - analogRange[id].Lower)) * (analogInputRaw[id] - analogRange[id].Lower)),
           0, 255);
      }
      return 0;      
    }

  }  


  public class FlightStickControl
  {
    public Vector3 Axis = new Vector3();
    public int Throttle = -32767;
    public byte HAT;
  }



}