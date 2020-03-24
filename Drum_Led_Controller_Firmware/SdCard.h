// SdCard.h

#ifndef _SDCARD_h
#define _SDCARD_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

#include <SysCall.h>
#include <SdFs.h>
#include <MinimumSerial.h>
#include <FsVolume.h>
#include <FsFile.h>
#include <FsConfig.h>
#include <FreeStack.h>
#include <BlockDeviceInterface.h>
#include <BlockDevice.h>

#include "Types.h"
#include "LedStrip.h"


class SdCardClass
{
private:
	SdFs sd;
	uint64_t framesDataPos = 0;
	uint8_t frameLength = 0;
	uint16_t frameCount = 0;
	uint8_t curFrame = 0;

	uint16_t presetsCount = 0;
	uint16_t curPreset = 0;

 protected:


 public:
	 FsFile settingsFile;
	 FsFile presetslistFile;
	 FsFile presetFile;

	bool init();

	bool openPresetsList();

	bool openNextPreset();
	void readStripInfo(LedStripClass* ledStrip);
	uint8_t readFps();
	uint8_t readTrigBrightness();
	uint8_t readProgBrightness();
	void readFramesInfo();
	void readFrame(uint8_t *frameBuffer);
};

#endif

