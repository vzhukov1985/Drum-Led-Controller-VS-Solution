// LedStrips.h

#ifndef _LEDSTRIPS_h
#define _LEDSTRIPS_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <OctoWS2811.h>

#include "LedStrip.h"

#define LED_STRIPS_COUNT 5

class LedStripsClass
{
 protected:


 public:
	LedStripClass strip[LED_STRIPS_COUNT];
	uint8_t fps = 30;
	uint8_t trigBrightness = 255;
	uint8_t progBrightness = 255;

	void init(OctoWS2811* ledsEngine, uint16_t ledsPerStripMaxUsed);
	void updateProgLeds(uint8_t* frameBuffer);
	void triggersHitProcess(uint8_t brightness, uint16_t speed);
	void turnStripsOff();
};

#endif

