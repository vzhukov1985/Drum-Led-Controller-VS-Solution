// WirelessHC.h

#ifndef _WIRELESSHC_h
#define _WIRELESSHC_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include "Types.h"

#define WIRELESSHC_SET_PIN 11

typedef void (*OnOffFunc) (void);
typedef void (*NextFunc) (void);

class WirelessHCClass
{
private:
	CmdStatus cmdStatus = Cmd_WaitHeader;
	char cmdHeader[3];

	void receiveCmdHeader();
	void receiveCmdType();
 protected:


 public:
	 OnOffFunc onOffFunction;
	 NextFunc nextFunction;

	 void init();
	 void process();
};

extern WirelessHCClass WirelessHC;

#endif

