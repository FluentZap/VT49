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
  #define _BV(bit) (1<<(bit))
#endif

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


#define ThrottleLEDButton1      29;
#define ThrottleLEDButton2      32;
#define ThrottleLEDButton3      35;
//#define ThrottleLEDToggle       
#define MatrixLEDButton1        23;
#define MatrixLEDButton2        24;
#define MatrixRotButton1        53;
#define MatrixRotButton2        52;
#define MatrixRotButton3        66;
#define MatrixDoubleTog_Up      26;
#define MatrixDoubleTog_Down    27;
#define ControlLED1             40;
#define ControlLED2             44;
#define ControlLED3             45;
#define ControlLED4             43;
#define ControlLED5             46;
#define TargetRotButton1        19;
#define TargetRotButton2        56;
#define TargetDoubleTog_Up      37;
#define TargetDoubleTog_Down    36;

#define EightRotButton          16;
//#define EightLEDToggle          
#define EightDoubleTog_Up       63;
#define EightDoubleTog_Down     62;
#define EightToggle1            7;
#define EightToggle2            6;
//#define EightToggle3            
#define EightToggle4            61;
#define EightToggle5            60;
#define EightToggle6            58;
#define EightToggle7            59;
#define EightToggle8            57;


#define ThrottleLEDButton1LED   57;
#define ThrottleLEDButton2LED   57;
#define ThrottleLEDButton3LED   57;
#define ThrottleLEDToggleLED    57;

#define MatrixLEDButton1LED     23;
#define MatrixLEDButton2LED     24;

#define ControlLED1LED          40;
#define ControlLED2LED          44;
#define ControlLED3LED          45;
#define ControlLED4LED          43;
#define ControlLED5LED          46;

//#define EightLEDToggleLED       

void BuildBuffer();
void ProcessBuffer(char* B);
bool CheckBuffer(byte[12], byte[12]);
void CopyBuffer(byte[12], byte[12]);
void SendByteBuffer(byte[12]);


long LastRender;
long LastUpdate;

byte SendBuffer[12];
byte TempBB[12];
char *buff;


int lastCount = 50;

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
  //seg.shutdown(0, false);
  //seg.shutdown(1, false);
  
  matrix.setIntensity(0,8);
  matrix.setIntensity(1,8);
  matrix.setIntensity(2,8);
  matrix.setIntensity(3,8);
  
  //seg.setIntensity(0,8);
  //seg.setIntensity(1,8);

  matrix.clearDisplay(0);
  matrix.clearDisplay(1);
  matrix.clearDisplay(2);
  matrix.clearDisplay(3);

  //seg.clearDisplay(0);
  //seg.clearDisplay(1);
  matrix.setRow(0,1,B10110000);
  matrix.setRow(0,2,B10110000);
  matrix.setRow(0,3,B10110000);
  matrix.setRow(0,4,B10110000);

  encoder1 = new ClickEncoder(51, 49, 1);
  encoder2 = new ClickEncoder(50, 48, 1);
  encoder3 = new ClickEncoder(64, 65, 1);
  encoder4 = new ClickEncoder(17, 18, 1);
  encoder5 = new ClickEncoder(54, 55, 1);
  encoder6 = new ClickEncoder(15, 14, 1);
  Timer1.initialize(1000);
  Timer1.attachInterrupt(timerIsr);


 //BUTTON LED 32, 34, 36

  for ( int id = 2; id <= 68; id++)
  {
    if (id != 5)pinMode(id, INPUT_PULLUP);
  }

//  pinMode(3, INPUT);
//  pinMode(34, INPUT);

//  pinMode(4, OUTPUT);
  pinMode(22, OUTPUT);
  pinMode(25, OUTPUT);
  pinMode(28, OUTPUT);
  pinMode(30, OUTPUT);
  pinMode(31, OUTPUT);
  pinMode(33, OUTPUT);    //GreenLed
  pinMode(38, OUTPUT);
  pinMode(39, OUTPUT);
  pinMode(41, OUTPUT);
  pinMode(42, OUTPUT);    //GreenLedLight
  pinMode(47, OUTPUT);
  
//  digitalWrite(4, LOW);
//  digitalWrite(4, HIGH);
  
  digitalWrite(22, HIGH);
  digitalWrite(25, HIGH);
  
//  digitalWrite(28, LOW);
//  digitalWrite(28, HIGH);
  
  digitalWrite(30, HIGH);
  digitalWrite(31, HIGH);
  digitalWrite(33, HIGH);
  digitalWrite(38, HIGH);
  digitalWrite(39, HIGH);
  digitalWrite(41, HIGH);
  digitalWrite(42, HIGH);
  digitalWrite(47, HIGH);

  pinMode(3, INPUT);
  pinMode(34, INPUT);
  
  pinMode(4, OUTPUT);     //LED and Switch Power
  pinMode(28, OUTPUT);    //LED and Switch Power

  digitalWrite(4, HIGH);
  digitalWrite(28, HIGH);

  
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
  
  byte SendBuffer[12];
  
  Serial.begin(115200);
    
  //OLEDdisplay.begin(SSD1306_SWITCHCAPVCC, 0x3C);  // initialize with the I2C addr 0x3D (for the 128x64)
  //OLEDdisplay.setFont(&FreeSans9pt7b);  
  
}

int16_t value;

void loop()
{ 
// value += encoder->getValue();
// Serial.println(value);


  for ( int id = 1; id <= 53; id++)
  {    
    if (digitalRead(id) == LOW && id != 5 && id != 3 && id != 34)
    {
      Serial.println(id);
      Serial.println("IS ON");
      delay(500);
    }
  }
  if (digitalRead(3) == HIGH)
    {
      Serial.println(3);
      Serial.println("IS ON");
      delay(500);
    }
    if (digitalRead(34) == HIGH)
    {
      Serial.println(34);
      Serial.println("IS ON");
      delay(500);
    }

  for ( int id = 0; id <= 14; id++)
    {    
      if (digitalRead(id+54) == LOW)
      {
        Serial.println(id+54);
        Serial.println("IS ON");       
        delay(500);
      }
    }
    
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
      //SendByteBuffer(TempBB);      
      LastUpdate = millis();
  }
}


void SendByteBuffer(byte bb[12])
{
  Serial.print("VT");
  Serial.write(bb, 13);
}


void CopyBuffer(byte bb[12], byte bb2[12])
{
  for (int x = 0; x < 12; x++)
  {
    bb2[x] = bb[x];
  }  
}



bool CheckBuffer(byte bb[12], byte bb2[12])
{
  bool change = false;
  for (int x = 0; x < 12; x++)
  {
    if (!(bb[x] == bb2[x])) change = true;
  }
  return change;
}


void BuildBuffer()
{
  TempBB[0] = 0;
  TempBB[1] = 0;
  TempBB[4] = 0;
  TempBB[5] = 0;
  TempBB[6] = 0;
  TempBB[7] = 0;
  TempBB[8] = 0;
  TempBB[9] = 0;
  TempBB[10] = 0;
  TempBB[11] = 0;
  TempBB[12] = 0;  
}

void Render ()
{
  fill_rainbow( leds, NUM_LEDS, 12, 7);
  FastLED.show();
}
