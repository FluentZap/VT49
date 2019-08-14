using System;
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
  
}