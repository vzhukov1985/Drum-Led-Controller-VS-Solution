// 
// 
// 

#include "Controls.h"

void ControlsClass::init()
{
	pinMode(STATUS_LED_RED_PIN, OUTPUT);
	pinMode(STATUS_LED_GREEN_PIN, OUTPUT);
}

void ControlsClass::ledTurnOnRed()
{
	digitalWrite(STATUS_LED_RED_PIN, LOW);
	digitalWrite(STATUS_LED_GREEN_PIN, HIGH);
}

void ControlsClass::ledTurnOnGreen()
{
	digitalWrite(STATUS_LED_RED_PIN, HIGH);
	digitalWrite(STATUS_LED_GREEN_PIN, LOW);
}


