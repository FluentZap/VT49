#include "FastLED.h"
#include "LedControl.h"
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

FASTLED_USING_NAMESPACE

#ifndef _BV
#define _BV(bit) (1 << (bit))
#endif

PacketSerial myPacketSerial;

ClickEncoder *encoder1;
ClickEncoder *encoder2;
ClickEncoder *encoder3;
ClickEncoder *encoder4;
ClickEncoder *encoder5;
ClickEncoder *encoder6;

void timerIsr()
{
  encoder1->service();
  encoder2->service();
  encoder3->service();
  encoder4->service();
  encoder5->service();
  encoder6->service();
}

#define DATA_PIN 5
#define LED_TYPE WS2811
#define COLOR_ORDER RGB
#define NUM_LEDS 50
CRGB leds[NUM_LEDS];

#define BRIGHTNESS 64
#define FRAMES_PER_SECOND 60
#define UPDATES_PER_SECOND 60

#define OLED_RESET 4

//Adafruit_LEDBackpack matrix = Adafruit_LEDBackpack();
//Adafruit_SSD1306 OLEDdisplay(OLED_RESET);

LedControl matrix = LedControl(28, 32, 30, 4);
LedControl seg = LedControl(34, 38, 36, 2);

#define ThrottleLEDButton1 27
#define ThrottleLEDButton2 29
#define ThrottleLEDButton3 41
#define ThrottleLEDToggle A15
#define MatrixLEDButton1 45
#define MatrixLEDButton2 44
#define MatrixRotButton1 16
#define MatrixRotButton2 24
#define MatrixRotButton3 19
#define MatrixDoubleTog_Up 49
#define MatrixDoubleTog_Down 48
#define ControlLED1 57
#define ControlLED2 56
#define ControlLED3 55
#define ControlLED4 54
#define ControlLED5 39
#define TargetRotButton1 10
#define TargetRotButton2 13
#define TargetDoubleTog_Up 53
#define TargetDoubleTog_Down 52

#define EightRotButton 2
#define EightLEDToggle 33
#define EightDoubleTog_Up 50
#define EightDoubleTog_Down 51
#define EightToggle1 62
#define EightToggle2 63
#define EightToggle3 64
#define EightToggle4 65
#define EightToggle5 66
#define EightToggle6 67
#define EightToggle7 68
#define EightToggle8 42

#define ThrottleLEDButton1LED 57
#define ThrottleLEDButton2LED 57
#define ThrottleLEDButton3LED 57
#define ThrottleLEDToggleLED 57

#define MatrixLEDButton1LED 23
#define MatrixLEDButton2LED 24

#define SendBufferSize 16
#define ReceiveBufferSize 16

//enum LED_ID
//{
//ToggleLit_1LED_1 = 14,
//ToggleLit_1LED_2 = 15,
//ToggleLit_2LED_1 = 13,
//ToggleLit_2LED_2 = 16
//};

void BuildBuffer(byte packet);
void ProcessBuffer(char *B);
bool CheckBuffer(byte[SendBufferSize], byte[SendBufferSize]);
void CopyBuffer(byte[SendBufferSize], byte[SendBufferSize]);

long LastRender = 0;
long LastUpdate = 0;

byte SendBuffer[SendBufferSize] = {};
byte LastSendBuffer[SendBufferSize] = {};

int lastCount = 50;

void setup()
{
  delay(3000); // 3 second delay for recovery

  // tell FastLED about the LED strip configuration
  FastLED.addLeds<LED_TYPE, DATA_PIN, COLOR_ORDER>(leds, NUM_LEDS).setCorrection(TypicalLEDStrip);

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

  encoder1 = new ClickEncoder(15, 24, 1);
  encoder2 = new ClickEncoder(18, 17, 1);
  encoder3 = new ClickEncoder(22, 23, 1);
  encoder4 = new ClickEncoder(11, 12, 1);
  encoder5 = new ClickEncoder(9, 8, 1);
  encoder6 = new ClickEncoder(3, 4, 1);
  Timer1.initialize(1000);
  Timer1.attachInterrupt(timerIsr);

  //BUTTON LED 32, 34, 36

  for (int id = 2; id <= 68; id++)
  {
    if (id != 5)
      pinMode(id, INPUT_PULLUP);
  }

  pinMode(25, OUTPUT);
  pinMode(26, OUTPUT);
  pinMode(40, OUTPUT);
  pinMode(43, OUTPUT);
  pinMode(46, OUTPUT);
  pinMode(47, OUTPUT);
  pinMode(58, OUTPUT);
  pinMode(59, OUTPUT);
  pinMode(60, OUTPUT);
  pinMode(61, OUTPUT);

  digitalWrite(25, HIGH);
  digitalWrite(26, HIGH);
  digitalWrite(40, HIGH);
  digitalWrite(43, HIGH);
  digitalWrite(46, HIGH);
  digitalWrite(47, HIGH);
  digitalWrite(58, HIGH);
  digitalWrite(59, HIGH);
  digitalWrite(60, HIGH);
  digitalWrite(61, HIGH);
  //
  //37
  //A15 pulldo
  //
  //35
  //33 pulldo

  pinMode(A15, INPUT);
  pinMode(33, INPUT);

  pinMode(35, OUTPUT); //LED and Switch Power
  pinMode(37, OUTPUT); //LED and Switch Power

  digitalWrite(35, HIGH);
  digitalWrite(37, HIGH);

  //pinMode(27, OUTPUT);

  //Leds
  //pinMode(24, OUTPUT);
  //pinMode(32, OUTPUT);
  //pinMode(34, OUTPUT);
  //pinMode(36, OUTPUT);

  //pinMode(A4, OUTPUT);
  //digitalWrite(A4, HIGH);

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

  if (millis() > (LastRender + 1000 / FRAMES_PER_SECOND))
  {
    Render();
    LastRender = millis();
  }

  if (millis() > (LastUpdate + 1000 / UPDATES_PER_SECOND))
  {
    BuildBuffer(1);
    myPacketSerial.send(SendBuffer, SendBufferSize);

    //      if (CheckBuffer(SendBuffer, LastSendBuffer))
    //      {
    //        CopyBuffer(SendBuffer, LastSendBuffer);
    //        myPacketSerial.send(SendBuffer, SendBufferSize);
    //      }

    LastUpdate = millis();
  }
}

void onPacketReceived(const uint8_t *buffer, size_t size)
{

  // Make a temporary buffer.
  if (size == 16)
  {
    //uint8_t tempBuffer[size];
    // Copy the packet into our temporary buffer.
    //memcpy(tempBuffer, buffer, size);
    // ProcessBuffer(buffer);
  }
}

bool CheckBuffer(byte bb[16], byte bb2[16])
{
  bool change = false;
  for (int x = 0; x < 16; x++)
  {
    if (!(bb[x] == bb2[x]))
      change = true;
  }
  return change;
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
    bitWrite(SendBuffer[i], 4, (digitalRead(ControlLED1) == LOW));
    bitWrite(SendBuffer[i], 5, (digitalRead(ControlLED2) == LOW));
    bitWrite(SendBuffer[i], 6, (digitalRead(ControlLED3) == LOW));
    bitWrite(SendBuffer[i], 7, (digitalRead(ControlLED4) == LOW));

    i++;
    bitWrite(SendBuffer[i], 0, (digitalRead(ControlLED5) == LOW));
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
    rot1Val = 0;
    rot2Val = 0;
    rot3Val = 0;
    rot4Val = 0;
    rot5Val = 0;
    rot6Val = 0;
  }
}

void ProcessBuffer(char *B)
{
//  byte Header = B[0];
//  if (Header == 1)
//  {
//    digitalWrite(Button_LED1, bitRead(B[1], 0));
//    digitalWrite(Button_LED2, bitRead(B[1], 1));
//    digitalWrite(Button_LED3, bitRead(B[1], 2));
//    digitalWrite(Button_LED4, bitRead(B[1], 3));
//    digitalWrite(FightStick_LED, bitRead(B[1], 4));
//
//    for (int x = 0; x < 50; x++)
//    {
//      leds[x].r = B[12];
//      leds[x].g = B[13];
//      leds[x].b = B[14];
//
//      //If the Light is set to on
//      if (bitRead(B[2 + (x / 8)], x % 8))
//      {
//        leds[x].r = B[9];
//        leds[x].g = B[10];
//        leds[x].b = B[11];
//      }
//    }
//
//    Target = B[15];
//  }
//    
//  if (Header > 99)
//  {
//    int num = (Header - 100) * 5;
//    for (int x = 0; x < 5; x++)
//    {
//      leds[num].r = B[(x * 3) + 1];
//      leds[num].g = B[(x * 3) + 2];
//      leds[num].b = B[(x * 3) + 3];
//      num++;
//    }
//  }
}

void SendByteBuffer(byte bb[12])
{
  Serial.print("VT");
  Serial.write(bb, 13);
}

void Render()
{
  fill_rainbow(leds, NUM_LEDS, 12, 7);
  FastLED.show();
}
