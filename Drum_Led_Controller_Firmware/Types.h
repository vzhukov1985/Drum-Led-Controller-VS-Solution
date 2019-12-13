// Types.h

#ifndef _TYPES_h
#define _TYPES_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

enum LedsConnectionType
{
	TrigProg,
	ProgTrig
};

enum AdditionalStripBehaviour
{
	AlwaysOff,
	AlwaysOn,
	Trigger
};

enum TriggerColorMode
{
	Constant,
	Random
};

#endif

