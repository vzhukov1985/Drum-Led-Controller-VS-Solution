// 
// 
// 

#include "LedStrip.h"

void LedStripClass::init(OctoWS2811* ledsEngine, uint16_t ledsPerStripMaxUsed, uint8_t ledStripOutNum, uint8_t setAdditionalStripPin = 0)
{
    leds = ledsEngine;
    additionalStripPin = setAdditionalStripPin;
    ledsPerStripMax = ledsPerStripMaxUsed;



    startAddress = ledsPerStripMax * ledStripOutNum;
    uint16_t endMaxAddress = startAddress + ledsPerStripMax - 1;

    for (int i = startAddress; i <= endMaxAddress; i++)
    {
        leds->setPixel(i, 0, 0, 0);
    }
}

void LedStripClass::updateStripData()
{
    if (ledsConnectionType == TrigProg)
    {
        trigLedsStartAddress = startAddress;
        progLedsStartAddress = startAddress + trigLedsCount;
    }
    if (ledsConnectionType == ProgTrig)
    {
        progLedsStartAddress = startAddress;
        trigLedsStartAddress = startAddress + progLedsCount;
    }

 
}

void LedStripClass::updateProgLeds(uint8_t *frameBuffer, uint8_t brightness)
{
    float brightnessMult = map((float)brightness, 0, 255, 0, 1);
    
    for (int i = 0; i < progLedsCount; i++)
    {
        leds->setPixel(progLedsStartAddress + i, (uint8_t) frameBuffer[3*i] * brightnessMult, (uint8_t) frameBuffer[3*i + 1] * brightnessMult, (uint8_t) frameBuffer[3*i + 2] * brightnessMult);
    }
}

void LedStripClass::triggerHit(uint8_t velocity)
{
    trigLedsHitBrightness = 1000;
    trigHitVelocity = velocity;

    if (triggerColorMode == Random)
    {
        //0-Red, 1-Green, 2 - Blue, 3 - Yellow(RG), 4 - Cyan(GB), 5 - Magenta (RB), 6 - White (RGB)
        uint8_t randColor = random(0, 7);
        switch (randColor)
        {
        case 0:
            triggerColorR = 255;
            triggerColorG = 0;
            triggerColorB = 0;
            break;
        case 1:
            triggerColorR = 0;
            triggerColorG = 255;
            triggerColorB = 0;
            break;
        case 2:
            triggerColorR = 0;
            triggerColorG = 0;
            triggerColorB = 255;
            break;
        case 3:
            triggerColorR = 255;
            triggerColorG = 255;
            triggerColorB = 0;
            break;
        case 4:
            triggerColorR = 0;
            triggerColorG = 255;
            triggerColorB = 255;
            break;
        case 5:
            triggerColorR = 255;
            triggerColorG = 0;
            triggerColorB = 255;
            break;
        case 6:
            triggerColorR = 255;
            triggerColorG = 255;
            triggerColorB = 255;
            break;
        default:
            break;
        }
    }
}

void LedStripClass::triggerHitProcess(uint8_t brightness, uint16_t speed)
{
    if (trigLedsHitBrightness < 0)
        trigLedsHitBrightness = 0;

    if (trigLedsHitBrightness > 0)
        trigLedsHitBrightness -= speed;

    float brightnessMult = map((float)trigHitVelocity, 0, 255, 0, 1) * map((float)brightness, 0, 255, 0, 1) * map((float)trigLedsHitBrightness, 0, 1000, 0, 1);

    for (int i = 0; i < trigLedsCount; i++)
    {
        leds->setPixel(trigLedsStartAddress + i, (uint8_t)triggerColorR * brightnessMult, (uint8_t)triggerColorG * brightnessMult, (uint8_t)triggerColorB * brightnessMult);
    }

}

void LedStripClass::turnStripOff()
{
    for (int i = startAddress; i <= (startAddress + progLedsCount + trigLedsCount); i++)
    {
        leds->setPixel(i, 0, 0, 0);
    }
}

