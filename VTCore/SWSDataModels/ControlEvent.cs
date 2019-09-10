using System;
using System.Linq;
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
    ThrottleLEDButton1,
    ThrottleLEDButton2,
    ThrottleLEDButton3,
    ThrottleLEDToggle,
    MatrixLEDButton1,
    MatrixLEDButton2,
    MatrixRotButton1,
    MatrixRotButton2,
    MatrixRotButton3,
    MatrixDoubleTog_Up,
    MatrixDoubleTog_Down,
    ControlLED1,
    ControlLED2,
    ControlLED3,
    ControlLED4,
    ControlLED5,
    TargetRotButton1,
    TargetRotButton2,
    TargetDoubleTog_Up,
    TargetDoubleTog_Down,
    EightRotButton,
    EightDoubleTog_Up,
    EightDoubleTog_Down,
    EightLEDToggle,
    EightToggle1,
    EightToggle2,
    EightToggle3,
    EightToggle4,
    EightToggle5,
    EightToggle6,
    EightToggle7,
    EightToggle8
  };

  public enum ListOf_SideOutputs : int
  {
    ThrottleLED1,
    ThrottleLED2,
    ThrottleLED3,
    ThrottleLEDToggle,
    MatrixLED1,
    MatrixLED2,    
    ControlLED1,
    ControlLED2,
    ControlLED3,
    ControlLED4,
    ControlLED5,        
    EightLEDToggle,    
  };

  public class ButtonSet<T>
  {
    HashSet<T> _triggered = new HashSet<T>();
    HashSet<T> _down = new HashSet<T>();


    public List<T> ToList()
    {
      return _down.ToList();
    }

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


  public class LedOutput<T>
  {    
    HashSet<T> _on = new HashSet<T>();

    public List<T> ToList()
    {
      return _on.ToList();
    }

    public void Set(T led, bool value)
    {
      if (value)
      {
        SetOn(led);
      }
      else
      {
        SetOff(led);
      }
    }

    public void SetOn(T led)
    {
      if (!_on.Contains(led))
      {
        _on.Add(led);
      }
    }

    public void SetOff(T led)
    {
      _on.Remove(led);
    }

    public bool IsOff(T led)
    {
      return !_on.Contains(led);
    }

    public bool IsOn(T led)
    {
      return _on.Contains(led);
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
    public LedOutput<ListOf_SideOutputs> LEDs = new LedOutput<ListOf_SideOutputs>();
    public bool[,] Matrix = new bool[16, 16];
    public byte[] analogInputRaw = new byte[6];
    AnalogRange[] analogRange;
    public int[] rotaryValue = new int[6];

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
    public ButtonSet<int> Buttons = new ButtonSet<int>();
  }  
}