using System;
using static SDL2.SDL;

namespace VT49
{
  public class VTController : IDisposable
  {
    SWSimulation _sws;
    
    const int JOYSTICK_DEAD_ZONE = 8000;
    IntPtr Joystick1 = IntPtr.Zero;
    
    
    
    public VTController(SWSimulation sws)
    {
      _sws = sws;          
    }

    public void Init()
    {
      int stickNumber = SDL_NumJoysticks();
      Joystick1 = SDL_JoystickOpen(0);
    }
    
    public void Update()
    {
      _sws.LeftInput.FlightStick.Axis.X = SDL_JoystickGetAxis(Joystick1, 1);  //X
      _sws.LeftInput.FlightStick.Axis.Y = -SDL_JoystickGetAxis(Joystick1, 0);  //Y
      // _sws.LeftInput.FlightStick.Axis.Z = SDL_JoystickGetAxis(Joystick1, 2);  //Throttle
      _sws.LeftInput.FlightStick.Axis.Z = -SDL_JoystickGetAxis(Joystick1, 3);
      // System.Console.WriteLine(_sws.LeftInput.FlightStick.Axis.Z.ToString());
    }

    public void Dispose()
    {
      SDL_JoystickClose(Joystick1);
    }

  }
}