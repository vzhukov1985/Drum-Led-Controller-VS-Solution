// LedStrip.h

#ifndef _LEDSTRIP_h
#define _LEDSTRIP_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <OctoWS2811.h>
#include <elapsedMillis.h>

#include "Types.h" 

#define TRIGGER_HIT_LENGTH 30

class LedStripClass
{
private:
	OctoWS2811* leds;
	uint16_t startAddress = 0;
	uint16_t ledsPerStripMax = 0;
	uint16_t progLedsStartAddress = 0;
	uint16_t trigLedsStartAddress = 0;
	int16_t trigLedsHitBrightness = 0;
	uint8_t trigHitVelocity = 0;
	uint8_t additionalStripPin = 0;

 protected:


 public:
	 LedsConnectionType ledsConnectionType = TrigProg;
	 bool hasAdditionalStrip = false;
	 AdditionalStripBehaviour additionalStripBehaviour = AlwaysOff;
	 TriggerColorMode triggerColorMode = Constant;
	 uint8_t triggerColorR = 0;
	 uint8_t triggerColorG = 0;
	 uint8_t triggerColorB = 0;
	 uint8_t trigLedsCount = 0;
	 uint8_t progLedsCount = 0;


	void init(OctoWS2811 *ledsEngine, uint16_t ledsPerStripMaxUsed, uint8_t ledStripOutNum, uint8_t setAdditionalStripPin);
	void updateStripData();
	void updateProgLeds(uint8_t *frameBuffer, uint8_t brightness);
	void triggerHit(uint8_t velocity);
	void triggerHitProcess(uint8_t brightness, uint16_t speed);
	void turnStripOff();
};

#endif

