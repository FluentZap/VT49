#include <SPI.h>
#include <Wire.h>
//#include <Adafruit_GFX.h>
//#include <Adafruit_SSD1306.h>
//#include <Aurebesh6p.h>
//#include <Fonts/FreeSans9pt7b.h>
#include <PacketSerial.h>
//#define WITHOUT_BUTTON 1;
#include <ClickEncoder.h>
#include <TimerOne.h>
#include "LedControl.h"
#include "FastLED.h"
#include "FastCRC.h"

FASTLED_USING_NAMESPACE

#ifndef _BV
  #define _BV(bit) (1<<(bit))
#endif

PacketSerial myPacketSerial;
FastCRC32 CRC32;

ClickEncoder *encoder1;
ClickEncoder *encoder2;
ClickEncoder *encoder3;
ClickEncoder *encoder4;
ClickEncoder *encoder5;
ClickEncoder *encoder6;

void timerIsr() {
  encoder1->service();
  encoder2->service();
  encoder3->service();
  encoder4->service();
  encoder5->service();
  encoder6->service();
}

#define DATA_PIN    5
#define LED_TYPE    WS2811
#define COLOR_ORDER RGB
#define NUM_LEDS    50
CRGB leds[NUM_LEDS];

#define BRIGHTNESS        64
#define FRAMES_PER_SECOND  60
#define UPDATES_PER_SECOND  60


#define OLED_RESET 4

//Adafruit_LEDBackpack matrix = Adafruit_LEDBackpack();
//Adafruit_SSD1306 OLEDdisplay(OLED_RESET);

LedControl matrix=LedControl(A15, A13, A14, 4);
LedControl seg=LedControl(34, 38, 36, 2);


#define ThrottleLEDButton1      29
#define ThrottleLEDButton2      32
#define ThrottleLEDButton3      35

#define ThrottleLEDToggle       34
#define MatrixLEDButton1        23
#define MatrixLEDButton2        24
#define MatrixRotButton1        53
#define MatrixRotButton2        52
#define MatrixRotButton3        66
#define MatrixDoubleTog_Up      26
#define MatrixDoubleTog_Down    27

#define ControlLED1Button       40
#define ControlLED2Button       44
#define ControlLED3Button       45
#define ControlLED4Button       43
#define ControlLED5Button       46

#define TargetRotButton1        19
#define TargetRotButton2        56
#define TargetDoubleTog_Up      37
#define TargetDoubleTog_Down    36

#define EightRotButton          16
#define EightLEDToggle          3
#define EightDoubleTog_Up       63
#define EightDoubleTog_Down     62
#define EightToggle1            7
#define EightToggle2            6
#define EightToggle3            2
#define EightToggle4            61
#define EightToggle5            60
#define EightToggle6            58
#define EightToggle7            59
#define EightToggle8            57

#define CONTROL_LED_1           41
#define CONTROL_LED_2           47
#define CONTROL_LED_3           39
#define CONTROL_LED_4           42
#define CONTROL_LED_5           38

#define MATRIX_LED_1            22
#define MATRIX_LED_2            25

#define THROTTLE_LED_1          33
#define THROTTLE_LED_2          30
#define THROTTLE_LED_3          31

#define THROTTLE_LED_TOGGLE_LED 28
#define EIGHT_LED_TOGGLE_LED    4

#define SendBufferSize 16
#define ReceiveBufferSize 40

void BuildBuffer(byte packet);
void ProcessBuffer(char* B);
bool CheckBuffer(byte[SendBufferSize], byte[SendBufferSize]);
void CopyBuffer(byte[SendBufferSize], byte[SendBufferSize]);

long LastRender;
long LastUpdate;

byte SendBuffer[SendBufferSize] = {};
byte LastSendBuffer[SendBufferSize] = {};

bool ThrottleLED1 = false;
bool ThrottleLED2 = false;
bool ThrottleLED3 = false;
bool ThrottleLEDToggleLED = false;
bool MatrixLED1 = false;
bool MatrixLED2 = false;

bool ControlLED1 = false;
bool ControlLED2 = false;
bool ControlLED3 = false;
bool ControlLED4 = false;
bool ControlLED5 = false;

bool EightLEDToggleLED = false;

bool TargetMAT[256];
bool EightSEG[128];

bool TargetMAT_Last[256];
bool EightSEG_Last[128];

int SendTimer = 0;

struct splitLong
{
  union {
    long value;
    char split[4];
  } __attribute__((packed));
};

void setup() {
  delay(3000); // 3 second delay for recovery
  
  // tell FastLED about the LED strip configuration
  FastLED.addLeds<LED_TYPE,DATA_PIN,COLOR_ORDER>(leds, NUM_LEDS).setCorrection(TypicalLEDStrip);
  
  // set master brightness control
  FastLED.setBrightness(BRIGHTNESS);

  matrix.shutdown(0, false);
  matrix.shutdown(1, false);
  matrix.shutdown(2, false);
  matrix.shutdown(3, false);
  
  seg.shutdown(0, false);
  seg.shutdown(1, false);
  
  matrix.setIntensity(0, 8);
  matrix.setIntensity(1, 8);
  matrix.setIntensity(2, 8);
  matrix.setIntensity(3, 8);
  
  seg.setIntensity(0, 8);
  seg.setIntensity(1, 8);

  matrix.clearDisplay(0);
  matrix.clearDisplay(1);
  matrix.clearDisplay(2);
  matrix.clearDisplay(3);

  seg.clearDisplay(0);
  seg.clearDisplay(1);

  encoder1 = new ClickEncoder(51, 49, 1);
  encoder2 = new ClickEncoder(50, 48, 1);
  encoder3 = new ClickEncoder(64, 65, 1);
  encoder4 = new ClickEncoder(17, 18, 1);
  encoder5 = new ClickEncoder(54, 55, 1);
  encoder6 = new ClickEncoder(15, 14, 1);
  Timer1.initialize(1000);
  Timer1.attachInterrupt(timerIsr);


 //BUTTON LED 32, 34, 36

LedControl matrix=LedControl(A15, A13, A14, 4);
LedControl seg=LedControl(34, 38, 36, 2);

  for ( int id = 2; id <= 68; id++)
  {
    if (
      id != 4 &&
      id != 5 &&
      id != 34 &&
      id != 36 &&
      id != 38 &&
      id != A13 &&
      id != A14 &&
      id != A15
    )
      pinMode(id, INPUT_PULLUP);
  }

//  pinMode(3, INPUT);
//  pinMode(34, INPUT);

//  pinMode(4, OUTPUT);
  pinMode(CONTROL_LED_1, OUTPUT);
  pinMode(CONTROL_LED_2, OUTPUT);
  pinMode(CONTROL_LED_3, OUTPUT);
  pinMode(CONTROL_LED_4, OUTPUT);
  pinMode(CONTROL_LED_5, OUTPUT);
  pinMode(MATRIX_LED_1, OUTPUT); //GreenLed
  pinMode(MATRIX_LED_2, OUTPUT);
  pinMode(THROTTLE_LED_1, OUTPUT);
  pinMode(THROTTLE_LED_2, OUTPUT);
  pinMode(THROTTLE_LED_3, OUTPUT); //GreenLedLight  

  digitalWrite(CONTROL_LED_1, LOW);
  digitalWrite(CONTROL_LED_2, LOW);
  digitalWrite(CONTROL_LED_3, LOW);
  digitalWrite(CONTROL_LED_4, LOW);
  digitalWrite(CONTROL_LED_5, LOW);
  digitalWrite(MATRIX_LED_1, LOW); //GreenLed
  digitalWrite(MATRIX_LED_2, LOW);
  digitalWrite(THROTTLE_LED_1, LOW);
  digitalWrite(THROTTLE_LED_2, LOW);
  digitalWrite(THROTTLE_LED_3, LOW); //GreenLedLight

  pinMode(ThrottleLEDToggle, INPUT);
  pinMode(EightLEDToggle, INPUT);
  
  pinMode(THROTTLE_LED_TOGGLE_LED, OUTPUT);     //LED and Switch Power
  pinMode(EIGHT_LED_TOGGLE_LED, OUTPUT);    //LED and Switch Power

  digitalWrite(THROTTLE_LED_TOGGLE_LED, HIGH);
  digitalWrite(EIGHT_LED_TOGGLE_LED, HIGH);

  
  //3   LED Tog
  //4   LED Tog
  //28  LED Tog
  //34  LED Tog
  
  //pinMode(27, OUTPUT);
  
  //Leds
  //pinMode(24, OUTPUT);
  //pinMode(32, OUTPUT);
  //pinMode(34, OUTPUT);
  //pinMode(36, OUTPUT);

  //pinMode(A4, OUTPUT);
  //digitalWrite(A4, HIGH);

  for (int i = 0; i < 256; i++)
  {
    TargetMAT[i] = false;
  }
  for (int i = 0; i < 128; i++)
  {
    EightSEG[i] = false;
  }

  myPacketSerial.begin(115200);
  myPacketSerial.setPacketHandler(&onPacketReceived);  
    
  //OLEDdisplay.begin(SSD1306_SWITCHCAPVCC, 0x3C);  // initialize with the I2C addr 0x3D (for the 128x64)
  //OLEDdisplay.setFont(&FreeSans9pt7b);  
  
}

int16_t rot1Val = 0;
int16_t rot2Val = 0;
int16_t rot3Val = 0;
int16_t rot4Val = 0;
int16_t rot5Val = 0;
int16_t rot6Val = 0;

void loop()
{
  rot1Val += encoder1->getValue();
  rot2Val += encoder2->getValue();
  rot3Val += encoder3->getValue();
  rot4Val += encoder4->getValue();
  rot5Val += encoder5->getValue();
  rot6Val += encoder6->getValue();

  myPacketSerial.update();

    
/*
    BuildBuffer();
    if (CheckBuffer(TempBB, SendBuffer))
    {
      CopyBuffer(TempBB, SendBuffer);
      SendByteBuffer(TempBB);
    }
*/

  //if (digitalRead(26) == LOW) fill_rainbow( leds, NUM_LEDS, gHue, 7);
  //if (digitalRead(22) == LOW) fadeToBlackBy( leds, NUM_LEDS, 10);
  if (millis() > (LastRender + 1000 / FRAMES_PER_SECOND))
  {
    Render();
    LastRender = millis();
  }

  if (millis() > (LastUpdate + 1000 / UPDATES_PER_SECOND))
  {
    BuildBuffer(1);
    // myPacketSerial.send(SendBuffer, SendBufferSize);
    if (SendTimer > 30 || CheckBuffer(SendBuffer, LastSendBuffer))
    {
      CopyBuffer(SendBuffer, LastSendBuffer);
      myPacketSerial.send(SendBuffer, SendBufferSize);
      SendTimer = 0;
    }
    else
    {
      SendTimer++;
    }

    LastUpdate = millis();
  }
}

void onPacketReceived(const uint8_t *buffer, size_t size)
{

  // Make a temporary buffer.
  if (size == 40)
  {
    if (CRC32.crc32(buffer, 40) == 0x2144DF1C)
    {
      //uint8_t tempBuffer[size];
      // Copy the packet into our temporary buffer.
      //memcpy(tempBuffer, buffer, size);
      ProcessBuffer(buffer);
    }
  }
}

bool CheckBuffer(byte bb[SendBufferSize], byte bb2[SendBufferSize])
{
  for (int x = 0; x < SendBufferSize; x++)
  {
    if (!(bb[x] == bb2[x]))
    {
      return true;
    }
  }
  return false;
}

void CopyBuffer(byte BufferSource[SendBufferSize], byte BufferDest[SendBufferSize])
{
  memcpy(BufferDest, BufferSource, SendBufferSize);
}

void BuildBuffer(byte packet)
{
  byte i = 1;
  SendBuffer[0] = packet;

  if (packet == 1)
  {
    //Throttle
    bitWrite(SendBuffer[i], 0, (digitalRead(ThrottleLEDButton1) == LOW));
    bitWrite(SendBuffer[i], 1, (digitalRead(ThrottleLEDButton2) == LOW));
    bitWrite(SendBuffer[i], 2, (digitalRead(ThrottleLEDButton3) == LOW));
    bitWrite(SendBuffer[i], 3, (digitalRead(ThrottleLEDToggle) == HIGH));
    bitWrite(SendBuffer[i], 4, (digitalRead(ControlLED1Button) == LOW));
    bitWrite(SendBuffer[i], 5, (digitalRead(ControlLED2Button) == LOW));
    bitWrite(SendBuffer[i], 6, (digitalRead(ControlLED3Button) == LOW));
    bitWrite(SendBuffer[i], 7, (digitalRead(ControlLED4Button) == LOW));

    i++;
    bitWrite(SendBuffer[i], 0, (digitalRead(ControlLED5Button) == LOW));
    bitWrite(SendBuffer[i], 1, (digitalRead(MatrixLEDButton1) == LOW));
    bitWrite(SendBuffer[i], 2, (digitalRead(MatrixLEDButton2) == LOW));
    bitWrite(SendBuffer[i], 3, (digitalRead(MatrixRotButton1) == LOW));
    bitWrite(SendBuffer[i], 4, (digitalRead(MatrixRotButton2) == LOW));
    bitWrite(SendBuffer[i], 5, (digitalRead(MatrixRotButton3) == LOW));
    bitWrite(SendBuffer[i], 6, (digitalRead(MatrixDoubleTog_Up) == LOW));
    bitWrite(SendBuffer[i], 7, (digitalRead(MatrixDoubleTog_Down) == LOW));

    i++;
    bitWrite(SendBuffer[i], 0, (digitalRead(TargetRotButton1) == LOW));
    bitWrite(SendBuffer[i], 1, (digitalRead(TargetRotButton2) == LOW));
    bitWrite(SendBuffer[i], 2, (digitalRead(TargetDoubleTog_Up) == LOW));
    bitWrite(SendBuffer[i], 3, (digitalRead(TargetDoubleTog_Down) == LOW));
    bitWrite(SendBuffer[i], 4, (digitalRead(EightRotButton) == LOW));
    bitWrite(SendBuffer[i], 5, (digitalRead(EightLEDToggle) == HIGH));
    bitWrite(SendBuffer[i], 6, (digitalRead(EightDoubleTog_Up) == LOW));
    bitWrite(SendBuffer[i], 7, (digitalRead(EightDoubleTog_Down) == LOW));

    i++;
    bitWrite(SendBuffer[i], 0, (digitalRead(EightToggle1) == LOW));
    bitWrite(SendBuffer[i], 1, (digitalRead(EightToggle2) == LOW));
    bitWrite(SendBuffer[i], 2, (digitalRead(EightToggle3) == LOW));
    bitWrite(SendBuffer[i], 3, (digitalRead(EightToggle4) == LOW));
    bitWrite(SendBuffer[i], 4, (digitalRead(EightToggle5) == LOW));
    bitWrite(SendBuffer[i], 5, (digitalRead(EightToggle6) == LOW));
    bitWrite(SendBuffer[i], 6, (digitalRead(EightToggle7) == LOW));
    bitWrite(SendBuffer[i], 7, (digitalRead(EightToggle8) == LOW));

    i++;
    SendBuffer[i] = (char)rot1Val;
    i++;
    SendBuffer[i] = (char)rot2Val;
    i++;
    SendBuffer[i] = (char)rot3Val;
    i++;
    SendBuffer[i] = (char)rot4Val;
    i++;
    SendBuffer[i] = (char)rot5Val;
    i++;
    SendBuffer[i] = (char)rot6Val;

    SendBuffer[10] = 0;
    SendBuffer[11] = 0;

    uint32_t crcLong = CRC32.crc32(SendBuffer, 12);
    SendBuffer[12] = (byte)crcLong;
    SendBuffer[13] = (byte)(crcLong >> 8);
    SendBuffer[14] = (byte)(crcLong >> 16);
    SendBuffer[15] = (byte)(crcLong >> 24);

    rot1Val = 0;
    rot2Val = 0;
    rot3Val = 0;
    rot4Val = 0;
    rot5Val = 0;
    rot6Val = 0;
  }
}

void ProcessBuffer(const uint8_t *B)
{
  byte Header = B[0];

  if (Header == 1 || Header == 2)
  {
    ThrottleLED1 = bitRead(B[1], 0);
    ThrottleLED2 = bitRead(B[1], 1);
    ThrottleLED3 = bitRead(B[1], 2);
    ThrottleLEDToggleLED = bitRead(B[1], 3);
    MatrixLED1 = bitRead(B[1], 4);
    MatrixLED2 = bitRead(B[1], 5);

    ControlLED1 = bitRead(B[1], 6);
    ControlLED2 = bitRead(B[1], 7);
    ControlLED3 = bitRead(B[2], 0);
    ControlLED4 = bitRead(B[2], 1);
    ControlLED5 = bitRead(B[2], 2);

    EightLEDToggleLED = bitRead(B[2], 3);
  }
  //35 Bytes
  if (Header == 1)
  {
    for (int i = 0; i < 256; i++)
    {
      TargetMAT[i] = bitRead(B[3 + (i / 8)], i % 8);
    }
  }
  //19 Bytes
  if (Header == 2)
  {
    for (int i = 0; i < 128; i++)
    {
      EightSEG[i] = bitRead(B[3 + (i / 8)], i % 8);
    }
  }
  //17 Bytes Upping to 19
  if (Header == 10)
  {
    for (int x = 0; x < 50; x++)
    {
      if (bitRead(B[9 + (x / 8)], x % 8))
      {
        //Light is on
        leds[x].r = B[3];
        leds[x].g = B[4];
        leds[x].b = B[5];
      }
      else
      {
        //Light is Off
        leds[x].r = B[6];
        leds[x].g = B[7];
        leds[x].b = B[8];
      }
    }
  }

  if (Header > 99)
  {
    int num = (Header - 100) * 5;
    for (int x = 0; x < 5; x++)
    {
      leds[num].r = B[(x * 3) + 1];
      leds[num].g = B[(x * 3) + 2];
      leds[num].b = B[(x * 3) + 3];
      num++;
    }
  }
}

void Render()
{
  //  fill_rainbow(leds, NUM_LEDS, 12, 7);
  FastLED.show();
  for (int panel = 0; panel < 4; panel++)
  {
    for (int i = 0; i < 64; i++)
    {
      if (TargetMAT[panel * 64 + i] != TargetMAT_Last[panel * 64 + i])
      {
        matrix.setLed(panel, i % 8, i / 8, TargetMAT[panel * 64 + i]);
        TargetMAT_Last[panel * 64 + i] = TargetMAT[panel * 64 + i];
      }
    }
  }

  for (int panel = 0; panel < 2; panel++)
  {
    for (int i = 0; i < 64; i++)
    {
      if (EightSEG[panel * 64 + i] != EightSEG_Last[panel * 64 + i])
      {
        seg.setLed(panel, i % 8, i / 8, EightSEG[panel * 64 + i]);
        EightSEG_Last[panel * 64 + i] = EightSEG[panel * 64 + i];
      }
    }
  }

   digitalWrite(THROTTLE_LED_TOGGLE_LED, ThrottleLEDToggleLED == true ? HIGH : LOW);
   digitalWrite(EIGHT_LED_TOGGLE_LED, EightLEDToggleLED == true ? HIGH : LOW);

  digitalWrite(THROTTLE_LED_1, ThrottleLED1 == true ? HIGH : LOW);
  digitalWrite(THROTTLE_LED_2, ThrottleLED2 == true ? HIGH : LOW);
  digitalWrite(THROTTLE_LED_3, ThrottleLED3 == true ? HIGH : LOW);
  digitalWrite(MATRIX_LED_1, MatrixLED1 == true ? HIGH : LOW);
  digitalWrite(MATRIX_LED_2, MatrixLED2 == true ? HIGH : LOW);
  digitalWrite(CONTROL_LED_1, ControlLED1 == true ? HIGH : LOW);
  digitalWrite(CONTROL_LED_2, ControlLED2 == true ? HIGH : LOW);
  digitalWrite(CONTROL_LED_3, ControlLED3 == true ? HIGH : LOW);
  digitalWrite(CONTROL_LED_4, ControlLED4 == true ? HIGH : LOW);
  digitalWrite(CONTROL_LED_5, ControlLED5 == true ? HIGH : LOW);
}
