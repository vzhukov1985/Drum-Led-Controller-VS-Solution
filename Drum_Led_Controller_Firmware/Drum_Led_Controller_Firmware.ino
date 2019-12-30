#include <OctoWS2811.h>
#include <elapsedMillis.h>
#include <Bounce.h>

#include "Types.h"
#include "Trigger.h"
#include "SdCard.h"
#include "Types.h"
#include "Controls.h"
#include "LedStrips.h"

#define LEDS_PER_STRIP_MAX 200
#define TRIGGERS_COUNT 5

#define BUTTON_ON_OFF 24		//On/Off Button
#define BUTTON_NEXT 25			//Next Button


ControlsClass controls; //Buttons & StatusLed
SdCardClass sd;
LedStripsClass ledStrips;
TriggerClass triggers[TRIGGERS_COUNT];

DMAMEM int displayMemory[LEDS_PER_STRIP_MAX * 6];
int drawingMemory[LEDS_PER_STRIP_MAX * 6];
const int config = WS2811_GRB | WS2811_800kHz;
OctoWS2811 ledStripsEngine(LEDS_PER_STRIP_MAX, displayMemory, drawingMemory, config);

Bounce buttonOnOff = Bounce(BUTTON_ON_OFF, 10);
Bounce buttonNext = Bounce(BUTTON_NEXT, 10);

uint16_t curPreset = 0;
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
	}
}

void setup() {
	controls.init();

	pinMode(9, OUTPUT);
	analogWrite(9, 1023);

	pinMode(BUTTON_ON_OFF, INPUT_PULLUP);
	pinMode(BUTTON_NEXT, INPUT_PULLUP);

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

	//Reading settings.ini
	readSettings();

	if (sd.openPreset(curPreset))
	{
		controls.ledTurnOnGreen();
		readCurrentPresetInfo();
	}
	else
	{
		controls.ledTurnOnRed();
	}
}

void loop() {
	ProcessSerial();

	if ((settingsFileUpdated == false) && settingsFileUpdaterTimer > 1000)
	{
		writeSettings();
		settingsFileUpdated = true;
	}

	if (buttonOnOff.update() && buttonOnOff.risingEdge() && !firstRun)
	{
		enabled = !enabled;

		for (int i = 0; i < LED_STRIPS_COUNT; i++)
		{
			ledStrips.strip[i].updateStripData();
		}

		if (!enabled)
			ledStrips.turnStripsOff();
	}
	if (buttonNext.update() && buttonNext.risingEdge() && !firstRun)
	{
		curPreset++;
		if (!sd.openPreset(curPreset))
		{
			curPreset = 0;
			sd.openPreset(curPreset);
		}
		readCurrentPresetInfo();
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
