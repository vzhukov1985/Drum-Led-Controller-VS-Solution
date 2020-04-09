// 
// 
// 

#include "SdCard.h"

void SdCardClass::readStripInfo(LedStripClass* ledStrip)
{
	ledStrip->ledsConnectionType = (LedsConnectionType)presetFile.read();
	ledStrip->hasAdditionalStrip = (bool)presetFile.read();
	ledStrip->additionalStripBehaviour = (AdditionalStripBehaviour)presetFile.read();
	ledStrip->triggerColorMode = (TriggerColorMode)presetFile.read();
	ledStrip->triggerColorR = (uint8_t)presetFile.read();
	ledStrip->triggerColorG = (uint8_t)presetFile.read();
	ledStrip->triggerColorB = (uint8_t)presetFile.read();
	ledStrip->trigLedsCount = (uint8_t)presetFile.read();
	ledStrip->progLedsCount = (uint8_t)presetFile.read();

	ledStrip->updateStripData();
}

void SdCardClass::readFramesInfo()
{
	frameLength = presetFile.read();
	presetFile.read(&frameCount, 2);
	curFrame = 0;
}

void SdCardClass::readFrame(uint8_t* frameBuffer)
{
	if (curFrame == 0)
		framesDataPos = presetFile.curPosition();

	if (curFrame == frameCount)
	{
		presetFile.seek(framesDataPos);
		curFrame = 0;
	}
	presetFile.read(frameBuffer, frameLength * 3); //3 colors per pixel
	curFrame++;
}

uint8_t SdCardClass::readFps()
{
	return presetFile.read();
}

uint8_t SdCardClass::readProgBrightness()
{
	return presetFile.read();
}

uint8_t SdCardClass::readTrigBrightness()
{
	return presetFile.read();
}

bool SdCardClass::init()
{
	bool res = false;

	//Initializing SD
	if (sd.begin(SdioConfig(FIFO_SDIO)))
		res = true;

	return res;
}

bool SdCardClass::openPresetsList()
{
	if (!sd.exists("PresetsList.ini"))
		return false;

	presetslistFile.open("PresetsList.ini", O_RDWR);
	presetslistFile.read(&presetsCount, 2);
	return true;
}

bool SdCardClass::openNextPreset()
{
	presetFile.close();


	curPreset++;
	if (curPreset > presetsCount)
	{
		curPreset = 1;
		presetslistFile.seek(2);
	}

	uint8_t fileNameLength = presetslistFile.read();
	char presetFileName[255];

	for (int i = 0; i < fileNameLength; i++)
	{
		presetFileName[i] = presetslistFile.read();
	}
	presetFileName[fileNameLength] = '\0';
	strcat(presetFileName, ".dlp");

	if (!sd.exists(presetFileName))
		return false;

	presetFile.open(presetFileName);
}

void SdCardClass::addPresetFile(Stream* serialStream)
{
	char fileName[255];
	byte presetNameLength = serialStream->read();
	serialStream->readBytes(fileName, presetNameLength);
	fileName[presetNameLength] = '\0';
	strcat(fileName, ".dlp");

	FsFile presetNewFile;

	if (sd.exists(fileName))
		sd.remove(fileName);

	presetNewFile.open(fileName, O_CREAT | O_WRITE);
	uint32_t fileDataLength;
	serialStream->readBytes((byte*)&fileDataLength, 4);
	for (int i = 0; i < fileDataLength; i++)
	{
		presetNewFile.write((uint8_t)serialStream->read());
	}
	presetNewFile.close();
}

void SdCardClass::updatePresetsListFile(Stream* serialStream)
{
	curPreset = 0;

	presetslistFile.seek(0);
	presetslistFile.truncate(0);
	uint32_t fileLength;
	serialStream->readBytes((byte*)&fileLength, 4);
	for (int i = 0; i < fileLength; i++)
	{
		presetslistFile.write((uint8_t)serialStream->read());
	}
}

void SdCardClass::removePresetFile(Stream* serialStream)
{
	char fileName[255];
	byte presetNameLength = serialStream->read();
	serialStream->readBytes(fileName, presetNameLength);
	fileName[presetNameLength] = '\0';
	strcat(fileName, ".dlp");
	sd.remove(fileName);
}


