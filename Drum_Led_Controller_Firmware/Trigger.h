// Trigger.h

#ifndef _TRIGGER_h
#define _TRIGGER_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class TriggerClass
{
private:
	uint8_t pin;
	uint16_t maxVal;
	elapsedMillis detectorMillis;
	bool isDetectStarted = false;

 protected:


 public:
	 uint16_t lowThreshold = 10;
	 uint16_t highThreshold = 1023;
	 uint32_t detectPeriod = 20;

	 void init(uint8_t setPin);
	 uint16_t processDetection();
};


#endif

