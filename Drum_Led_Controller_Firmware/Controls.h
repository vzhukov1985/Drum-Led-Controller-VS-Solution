// Controls.h

#ifndef _CONTROLS_h
#define _CONTROLS_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <Bounce.h>

#define STATUS_LED_RED_PIN 3	//Red Color of Status LED
#define STATUS_LED_GREEN_PIN 4	//Green Color of LED

class ControlsClass
{
 protected:

 public:
	 

	 void init();

	void ledTurnOnRed();
	void ledTurnOnGreen();
};

#endif

