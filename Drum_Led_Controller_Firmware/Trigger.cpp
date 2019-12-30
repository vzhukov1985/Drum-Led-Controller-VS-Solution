// 
// 
// 

#include "Trigger.h"

void TriggerClass::init(uint8_t setPin)
{
	pin = setPin;
}

uint16_t TriggerClass::processDetection()
{
	uint16_t val = analogRead(pin);

	if (val > lowThreshold)
	{
		isDetectStarted = true;
		detectorMillis = 0;
		if (val > maxVal)
			maxVal = val;
	}

	if ((detectorMillis > detectPeriod) && isDetectStarted)
	{
		int velocity = map(maxVal, lowThreshold, highThreshold, 1, 255);

		isDetectStarted = false;
		maxVal = 0;
		return velocity;
	}
	return 0;
}


