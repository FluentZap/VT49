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

LedControl matrix=LedControl(28, 32, 30, 4);
LedControl seg=LedControl(34, 38, 36, 2);


#define ThrottleLEDButton1      27;
#define ThrottleLEDButton2      29;
#define ThrottleLEDButton3      41;
//#define ThrottleLEDToggle       
#define MatrixLEDButton1        45;
#define MatrixLEDButton2        44;
#define MatrixRotButton1        16;
#define MatrixRotButton2        24;
#define MatrixRotButton3        19;
#define MatrixDoubleTog_Up      49;
#define MatrixDoubleTog_Down    48;
#define ControlLED1             57;
#define ControlLED2             56;
#define ControlLED3             55;
#define ControlLED4             54;
#define ControlLED5             39;
#define TargetRotButton1        10;
#define TargetRotButton2        13;
#define TargetDoubleTog_Up      53;
#define TargetDoubleTog_Down    52;

#define EightRotButton          2;
//#define EightLEDToggle          
#define EightDoubleTog_Up       50;
#define EightDoubleTog_Down     51;
#define EightToggle1            62;
#define EightToggle2            63;
#define EightToggle3            64  
#define EightToggle4            65;
#define EightToggle5            66;
#define EightToggle6            67;
#define EightToggle7            68;
#define EightToggle8            42;



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


//enum LED_ID
//{
//ToggleLit_1LED_1 = 14,
//ToggleLit_1LED_2 = 15,
//ToggleLit_2LED_1 = 13,
//ToggleLit_2LED_2 = 16
//};

void BuildBuffer();
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
  seg.shutdown(0, false);
  seg.shutdown(1, false);
  
  matrix.setIntensity(0,8);
  matrix.setIntensity(1,8);
  matrix.setIntensity(2,8);
  matrix.setIntensity(3,8);
  
  seg.setIntensity(0,8);
  seg.setIntensity(1,8);

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

  for ( int id = 2; id <= 68; id++)
  {
    if (id != 5)pinMode(id, INPUT_PULLUP);
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
  
  pinMode(35, OUTPUT);     //LED and Switch Power
  pinMode(37, OUTPUT);    //LED and Switch Power

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
  
  byte SendBuffer[12];
  Serial.begin(115200);
    
  //OLEDdisplay.begin(SSD1306_SWITCHCAPVCC, 0x3C);  // initialize with the I2C addr 0x3D (for the 128x64)
  //OLEDdisplay.setFont(&FreeSans9pt7b);  

}

int16_t value;

void loop()
{ 

  value += encoder1->getValue();
  Serial.println(value);
//  for ( int id = 2; id <= 53; id++)
//  {    
//    if (digitalRead(id) == LOW && id != 5 && id != 33)
//    {
//      Serial.println(id);
//      Serial.println("IS ON");       
//      delay(500);
//    }
//  }
//
//  for ( int id = 0; id <= 14; id++)
//    {    
//      if (digitalRead(id+54) == LOW)
//      {
//        Serial.println(id+54);
//        Serial.println("IS ON");       
//        delay(500);
//      }
//    }
//     if (digitalRead(A15) == HIGH)
//    {
//      Serial.println(A15);
//      Serial.println("IS ON");
//      delay(500);
//    }
//    if (digitalRead(33) == HIGH)
//    {
//      Serial.println(33);
//      Serial.println("IS ON");
//      delay(500);
//    }
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
