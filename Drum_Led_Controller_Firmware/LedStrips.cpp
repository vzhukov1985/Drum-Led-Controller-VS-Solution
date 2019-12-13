// 
// 
// 

#include "LedStrips.h"

void LedStripsClass::init(OctoWS2811* ledsEngine, uint16_t ledsPerStripMaxUsed)
{
	ledsEngine->begin();
	strip[0].init(ledsEngine, ledsPerStripMaxUsed, 0, 9);
	for (int i = 1; i < LED_STRIPS_COUNT; i++)
	{
		strip[i].init(ledsEngine, ledsPerStripMaxUsed, i, 0);
	}

	ledsEngine->show();
}

void LedStripsClass::updateProgLeds(uint8_t* frameBuffer)
{
	for (int i = 0; i < LED_STRIPS_COUNT; i++)
	{
		strip[i].updateProgLeds(frameBuffer, progBrightness);
	}
}

void LedStripsClass::triggersHitProcess(uint8_t brightness, uint16_t speed)
{
	for (int i = 0; i < LED_STRIPS_COUNT; i++)
	{
		strip[i].triggerHitProcess(trigBrightness, speed);
	}
}

void LedStripsClass::turnStripsOff()
{
	for (int i = 0; i < LED_STRIPS_COUNT; i++)
	{
		strip[i].turnStripOff();
	}
}
