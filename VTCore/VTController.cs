using System;
using static SDL2.SDL;

namespace VT49
{

  public class Joystick
  {
    const int JOYSTICK_DEAD_ZONE = 8000;
    public IntPtr Pointer = IntPtr.Zero;
    public int startThrottle;
    public bool checkThrottle = false;
  }



  public class VTController : IDisposable
  {
    SWSimulation _sws;

    const int JOYSTICK_DEAD_ZONE = 8000;
    Joystick Joystick1 = new Joystick();
    Joystick Joystick2 = new Joystick();

    public VTController(SWSimulation sws)
    {
      _sws = sws;
    }

    public void Init()
    {
      int stickNumber = SDL_NumJoysticks();
      Joystick1.Pointer = SDL_JoystickOpen(0);
      Joystick2.Pointer = SDL_JoystickOpen(1);
      Joystick1.startThrottle = -SDL_JoystickGetAxis(Joystick1.Pointer, 2);
      Joystick2.startThrottle = -SDL_JoystickGetAxis(Joystick2.Pointer, 2);
    }

    public void Update()
    {
      UpdateJoystick(Joystick1, _sws.RightInput.FlightStick);
      UpdateJoystick(Joystick2, _sws.LeftInput.FlightStick);
    }

    void UpdateJoystick(Joystick joystick, FlightStickControl flightStick)
    {
      flightStick.Axis.Y = -SDL_JoystickGetAxis(joystick.Pointer, 0);  //Y
      flightStick.Axis.X = SDL_JoystickGetAxis(joystick.Pointer, 1);  //X
      flightStick.Axis.Z = -SDL_JoystickGetAxis(joystick.Pointer, 3);
      throttleCheck(joystick, flightStick);
      flightStick.HAT = SDL_JoystickGetHat(joystick.Pointer, 0);

      for (int i = 0; i < 5; i++)
      {
        flightStick.Buttons.Set(i, SDL_JoystickGetButton(joystick.Pointer, i) > 0);
      }

    }

    public void throttleCheck(Joystick Js, FlightStickControl flightStick)
    {
      if (Js.checkThrottle == true)
      {
        flightStick.Throttle = -SDL_JoystickGetAxis(Js.Pointer, 2);  //Throttle
      }
      else
      {
        if (Js.startThrottle != -SDL_JoystickGetAxis(Js.Pointer, 2))
        {
          Js.checkThrottle = true;
        }
      }
    }

    public void Dispose()
    {
      SDL_JoystickClose(Joystick1.Pointer);
      SDL_JoystickClose(Joystick2.Pointer);
    }

  }
}