#include <OctoWS2811.h>
#include <elapsedMillis.h>
#include <Bounce.h>

#include "Types.h"
#include "Trigger.h"
#include "SdCard.h"
#include "Types.h"
#include "Controls.h"
#include "LedStrips.h"
#include "WirelessHC.h"

#define LEDS_PER_STRIP_MAX 200
#define TRIGGERS_COUNT 5


ControlsClass controls; //Buttons & StatusLed
SdCardClass sd;
LedStripsClass ledStrips;
TriggerClass triggers[TRIGGERS_COUNT];
WirelessHCClass wirelessHC;

DMAMEM int displayMemory[LEDS_PER_STRIP_MAX * 6];
int drawingMemory[LEDS_PER_STRIP_MAX * 6];
const int config = WS2811_GRB | WS2811_800kHz;
OctoWS2811 ledStripsEngine(LEDS_PER_STRIP_MAX, displayMemory, drawingMemory, config);

uint8_t frameBuffer[LEDS_PER_STRIP_MAX * 3];
uint32_t microsPerFrame;
elapsedMicros frameElapsedMicros;
elapsedMillis settingsFileUpdaterTimer;
bool settingsFileUpdated = true;
bool enabled = false;
bool firstRun = true;

void readSettings()
{
	sd.settingsFile.open("Settings.ini", O_RDWR);
	sd.settingsFile.read(&ledStrips.trigHitShowSpeed, 2);

	for (int i = 0; i < 5; i++)
	{
		sd.settingsFile.read(&triggers[i].lowThreshold, 2);
		sd.settingsFile.read(&triggers[i].highThreshold, 2);
		sd.settingsFile.read(&triggers[i].detectPeriod, 2);
	}
}

void writeSettings()
{
	sd.settingsFile.seek(0);
	sd.settingsFile.write(&ledStrips.trigHitShowSpeed, 2);

	for (int i = 0; i < 5; i++)
	{
		sd.settingsFile.write(&triggers[i].lowThreshold, 2);
		sd.settingsFile.write(&triggers[i].highThreshold, 2);
		sd.settingsFile.write(&triggers[i].detectPeriod, 2);
	}
}

void readCurrentPresetInfo()
{
	//Reading each strip's info from preset
	for (int i = 0; i < LED_STRIPS_COUNT; i++)
	{
		sd.readStripInfo(&ledStrips.strip[i]);
	}
	//Reading frames Info
	sd.readFramesInfo();
	ledStrips.fps = sd.readFps();
	microsPerFrame = (uint32_t)1000000 / ledStrips.fps;
	ledStrips.trigBrightness = sd.readTrigBrightness();
	ledStrips.progBrightness = sd.readProgBrightness();
}

void ProcessSerial()
{
	char cmdHeader[7];
	cmdHeader[6] = '\0';
	while (Serial.available())
	{
		Serial.readBytes(cmdHeader, 6);

		//Если послан запрос от контроллера - выдать настройки
		if (strcmp(cmdHeader, "PCREQS") == 0)
		{
			//Sending settings
			char outHeader[7] = "CPSETS";
			Serial.write(outHeader);
			Serial.write((byte *)&ledStrips.trigHitShowSpeed, 2);
			for (int i = 0; i < TRIGGERS_COUNT; i++)
			{
				Serial.write((byte*)&triggers[i].detectPeriod, 2);
				Serial.write((byte*)&triggers[i].lowThreshold, 2);
				Serial.write((byte*)&triggers[i].highThreshold, 2);
			}
		}

		//Если послан запрос от контроллера - обновить настройки
		if (strcmp(cmdHeader, "PCSETS") == 0)
		{
			//Receiving settings
			Serial.readBytes((char *) &ledStrips.trigHitShowSpeed, 2);
			for (int i = 0; i < TRIGGERS_COUNT; i++)
			{
				Serial.readBytes((char*)&triggers[i].detectPeriod, 2);
				Serial.readBytes((char*)&triggers[i].lowThreshold, 2);
				Serial.readBytes((char*)&triggers[i].highThreshold, 2);
			}

			settingsFileUpdated = false;
			settingsFileUpdaterTimer = 0;
		}

		//Если послан запрос от контроллера - выдать список презетов
		if (strcmp(cmdHeader, "PCRQPL") == 0)
		{
			char outHeader[7] = "CPRQPL";
			Serial.write(outHeader);
			Serial.write((byte *)&sd.presetsCount, 2);
			uint64_t presetsListFilePos = sd.presetslistFile.curPosition();
			sd.presetslistFile.seekSet(2);
			for (int i = 0; i < (sd.presetslistFile.fileSize() - 2); i++)
				Serial.write((byte)sd.presetslistFile.read());
			sd.presetslistFile.seekSet(presetsListFilePos);
		}
	}
}

void setup() {
	controls.init();

	pinMode(9, OUTPUT);
	analogWrite(9, 1023);

	//Initializing Sd Card
	if (sd.init())
		controls.ledTurnOnGreen();

	//Initializing LED Strips
	ledStrips.init(&ledStripsEngine, LEDS_PER_STRIP_MAX);

	//Initializing Triggers
	triggers[0].init(A1);
	triggers[1].init(A2);
	triggers[2].init(A3);
	triggers[3].init(A4);
	triggers[4].init(A5);

	//Initializing Wireless HC-12
	wirelessHC.init();
	wirelessHC.nextFunction = SwitchToNextPreset;
	wirelessHC.onOffFunction = OnOffStrips;

	//Reading settings.ini
	readSettings();

	if (sd.openPresetsList())
	{
		controls.ledTurnOnGreen();
	}
	else
	{
		controls.ledTurnOnRed();
	}


	if (sd.openNextPreset())
	{
		controls.ledTurnOnGreen();
		readCurrentPresetInfo();
	}
	else
	{
		controls.ledTurnOnRed();
	}


	ledStrips.turnStripsOff();
}

void OnOffStrips()
{
	enabled = !enabled;

	for (int i = 0; i < LED_STRIPS_COUNT; i++)
	{
		ledStrips.strip[i].updateStripData();
	}

	if (!enabled)
		ledStrips.turnStripsOff();
}

void SwitchToNextPreset()
{
	sd.openNextPreset();
	readCurrentPresetInfo();
}

void loop() {
	ProcessSerial();
	wirelessHC.process();

	if ((settingsFileUpdated == false) && settingsFileUpdaterTimer > 1000)
	{
		writeSettings();
		settingsFileUpdated = true;
	}

	if (enabled)
	{
		for (int i = 0; i < TRIGGERS_COUNT; i++)
		{
			uint16_t velocity = triggers[i].processDetection();
			if (velocity > 0)
				ledStrips.strip[i].triggerHit(velocity);
		}

		ledStrips.triggersHitProcess(ledStrips.trigBrightness);

		if (frameElapsedMicros > microsPerFrame)
		{
			sd.readFrame(frameBuffer);
			ledStrips.updateProgLeds(frameBuffer);
			frameElapsedMicros = 0;
		}
	}
	ledStripsEngine.show();

	firstRun = false;
}
