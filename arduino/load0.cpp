#include <SPI.h>
#include <GD.h>

#define VALID   5
#define READY   4 

static PROGMEM prog_uchar box[] = {
   0x00, 0x03,
   0x00, 0x0f,
   0x00, 0x3f,
   0x00, 0xff,
   0x03, 0xff,
   0x0f, 0xff,
   0x3f, 0xff,
   0xff, 0xff };

static uint16_t pal[16] = {
    RGB(0,0,0),
    RGB(0,0,0xaa),
    RGB(0,0xaa,0),
    RGB(0,0xaa,0xaa),
    RGB(0xaa,0,0),
    RGB(0xaa,0,0xaa),
    RGB(0xaa,0xaa,0),
    RGB(0xaa,0xaa,0xaa),
    RGB(0x55,0x55,0x55),
    RGB(0x55,0x55,0xff),
    RGB(0x55,0xff,0x55),
    RGB(0x55,0xff,0xff),
    RGB(0xff,0x55,0x55),
    RGB(0xff,0x55,0xff),
    RGB(0xff,0xff,0x55),
    RGB(0xff,0xff,0xff)
};

static void plot(byte i, byte c)
{
  byte x = 9 + (i & 31);
  byte y = 13 + (i >> 5);
  GD.wr(x + (y << 6), c);
}

void setup()
{
  Serial.begin(115200L);
  GD.begin();

  for (int i = 0; i < 256; i++) {
    GD.copy(RAM_CHR + (16 * i), box, sizeof(box));
    GD.wr16(RAM_PAL + (8 * i), pal[i & 15]);
    GD.wr16(RAM_PAL + (8 * i) + 6, pal[(i>>4) & 15]);
  }
  // paint a stripe pattern
  for (int i = 0; i < 256; i++) {
    byte p = 1 & (i ^ (i >> 5));
    plot(i, p ? 0xe1 : 0x1e);
  }

  // See http://arduino.cc/en/Hacking/PinMapping168

  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(A2, INPUT);
  pinMode(A3, INPUT);
  pinMode(A4, OUTPUT);
  pinMode(A5, OUTPUT);
  pinMode(6, OUTPUT);
  pinMode(7, OUTPUT);

  pinMode(VALID, OUTPUT);
  digitalWrite(VALID, LOW);
  pinMode(READY, INPUT);
}

static uint8_t buf[256];

static byte get()
{
  int i = 0;
  while (i < 256)
    if (Serial.available()) {
      byte c = Serial.read();
      plot(i, c);
      buf[i++] = c;
    }
}

static void opendrain(byte pin, byte val)
{
  if (val) {
    pinMode(pin, INPUT);
    digitalWrite(pin, HIGH);
  } else {
    digitalWrite(pin, LOW);
    pinMode(pin, OUTPUT);
  }
}

void loop()
{
  get();
  byte s = 0;
  for (int i = 0; i < 256; i++) {
    byte c = buf[i];
    s += c;
    c ^= 0x88;  // invert the signals that PC LPT inverts
    // opendrain(A0, (c >> 0) & 1);
    // digitalWrite(A1, ((c >> 1) & 1));
    // opendrain(A2, ((c >> 2) & 1));
    // opendrain(A3, ((c >> 3) & 1));

    digitalWrite(A4, ((c >> 4) & 1));
    digitalWrite(A5, ((c >> 5) & 1));
    digitalWrite(6,  ((c >> 6) & 1));
    digitalWrite(7,  ((c >> 7) & 1));

    digitalWrite(VALID, HIGH);
    while (digitalRead(READY) == 0) ;
    digitalWrite(VALID, LOW);
    while (digitalRead(READY) == 1) ;

    digitalWrite(A4, ((c >> 0) & 1));
    digitalWrite(A5, ((c >> 1) & 1));
    digitalWrite(6,  ((c >> 2) & 1));
    digitalWrite(7,  ((c >> 3) & 1));

    digitalWrite(VALID, HIGH);
    while (digitalRead(READY) == 0) ;
    digitalWrite(VALID, LOW);
    while (digitalRead(READY) == 1) ;

    plot(i, 0x80);
  }
  Serial.write(s);
}
