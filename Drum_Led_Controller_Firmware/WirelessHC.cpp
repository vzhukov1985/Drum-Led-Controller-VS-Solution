// 
// 
// 

#include "WirelessHC.h"

void WirelessHCClass::receiveCmdHeader()
{
	int cmdLen = strlen(cmdHeader);
	cmdHeader[cmdLen] = Serial1.read();

	if ((cmdLen + 1) == 2)
	{
		if (strcmp(cmdHeader, "DC") == 0)
			cmdStatus = Cmd_Wait_Type;
		cmdLen = -1;
	}
	cmdHeader[cmdLen + 1] = '\0';
}

void WirelessHCClass::receiveCmdType()
{
	byte cmdType = Serial1.read();
	if (cmdType == 0x01)
	{
		onOffFunction();
	}
	if (cmdType == 0x02)
	{
		nextFunction();
	}
	cmdStatus = Cmd_WaitHeader;
}

void WirelessHCClass::init()
{
	Serial1.begin(9600);
	pinMode(WIRELESSHC_SET_PIN, OUTPUT);


	digitalWrite(WIRELESSHC_SET_PIN, HIGH);
	delay(80);

	strcpy(cmdHeader, "");
}

void WirelessHCClass::process()
{
	if (Serial1.available())
	{
		switch (cmdStatus)
		{
		case (Cmd_WaitHeader):
			receiveCmdHeader();
			break;
		case (Cmd_Wait_Type):
			receiveCmdType();
			break;
		}
	}
}

