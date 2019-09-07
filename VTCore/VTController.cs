using System;
using static SDL2.SDL;

namespace VT49
{
  public class VTController : IDisposable
  {
    SWSimulation _sws;

    const int JOYSTICK_DEAD_ZONE = 8000;
    IntPtr Joystick1 = IntPtr.Zero;
    int startThrottle;
    bool checkThrottle = false;


    public VTController(SWSimulation sws)
    {
      _sws = sws;
    }

    public void Init()
    {
      int stickNumber = SDL_NumJoysticks();
      Joystick1 = SDL_JoystickOpen(0);
      startThrottle = -SDL_JoystickGetAxis(Joystick1, 2);
    }

    public void Update()
    {
      _sws.LeftInput.FlightStick.Axis.Y = -SDL_JoystickGetAxis(Joystick1, 0);  //Y
      _sws.LeftInput.FlightStick.Axis.X = SDL_JoystickGetAxis(Joystick1, 1);  //X
      _sws.LeftInput.FlightStick.Axis.Z = -SDL_JoystickGetAxis(Joystick1, 3);
      throttleCheck();
      _sws.LeftInput.FlightStick.HAT = SDL_JoystickGetHat(Joystick1, 0);

      for (int i = 0; i < 5; i++)
      {
        _sws.LeftInput.FlightStick.Buttons.Set(i, SDL_JoystickGetButton(Joystick1, i) > 0);
      }
    }

    public void throttleCheck()
    {
      if (checkThrottle == true)
      {
        _sws.LeftInput.FlightStick.Throttle = -SDL_JoystickGetAxis(Joystick1, 2);  //Throttle
      }
      else
      {
        if (startThrottle != -SDL_JoystickGetAxis(Joystick1, 2))
        {
          checkThrottle = true;
        }
      }
    }

    public void Dispose()
    {
      SDL_JoystickClose(Joystick1);
    }

  }
}