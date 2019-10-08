#include <ResponsiveAnalogRead.h>
#include <PacketSerial.h>
#include "FastCRC.h"

#define UPDATES_PER_SECOND 60

// make a ResponsiveAnalogRead object, pass in the pin, and either true or false depending on if you want sleep enabled
// enabling sleep will cause values to take less time to stop changing and potentially stop changing more abruptly,
//   where as disabling sleep will cause values to ease into their correct position smoothly and more accurately
ResponsiveAnalogRead analog1(A0, true);
ResponsiveAnalogRead analog2(A1, true);
ResponsiveAnalogRead analog3(A2, true);
ResponsiveAnalogRead analog4(A3, true);

// the next optional argument is snapMultiplier, which is set to 0.01 by default
// you can pass it a value from 0 to 1 that controls the amount of easing
// increase this to lessen the amount of easing (such as 0.1) and make the responsive values more responsive
// but doing so may cause more noise to seep through if sleep is not enabled

PacketSerial myPacketSerial;
FastCRC32 CRC32;

void BuildBuffer();
long LastUpdate = 0;

byte SendBuffer[8] = {};

void setup()
{
  myPacketSerial.begin(115200);
}

void loop()
{
  // update the ResponsiveAnalogRead object every loop
  analog1.update();
  analog2.update();
  analog3.update();
  analog4.update();
  myPacketSerial.update();

//  if(analog1.hasChanged() || analog2.hasChanged() || analog3.hasChanged() || analog4.hasChanged())
//  {
   if (millis() > (LastUpdate + 1000 / UPDATES_PER_SECOND))
    {
      BuildBuffer();
      myPacketSerial.send(SendBuffer, 8);
      LastUpdate = millis();
    } 
//  }
}

void BuildBuffer()
{
  SendBuffer[0] = analog1.getValue() / 4;
  SendBuffer[1] = analog2.getValue() / 4;
  SendBuffer[2] = analog3.getValue() / 4;
  SendBuffer[3] = analog4.getValue() / 4;

  uint32_t crcLong = CRC32.crc32(SendBuffer, 4);
  SendBuffer[4] = (byte)crcLong;
  SendBuffer[5] = (byte)(crcLong >> 8);
  SendBuffer[6] = (byte)(crcLong >> 16);
  SendBuffer[7] = (byte)(crcLong >> 24);
}
