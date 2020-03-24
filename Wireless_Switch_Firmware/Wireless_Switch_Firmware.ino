#include <OneButton.h>
#include <SoftwareSerial.h>

SoftwareSerial HC12(10, 11);

OneButton bt1(5, false, false);
OneButton bt2(6, false, false);

void bt1Click()
{
    HC12.write('D');
    HC12.write('C');
    HC12.write(0x01);
}

void bt2Click()
{
    HC12.write('D');
    HC12.write('C');
    HC12.write(0x02);
}


void setup() {
    Serial.begin(9600);
    HC12.begin(9600);
    bt1.attachClick(bt1Click);
    bt2.attachClick(bt2Click);

    pinMode(2, OUTPUT);
    digitalWrite(2, HIGH);
}

void loop() {
    bt1.tick();
    bt2.tick();
}

