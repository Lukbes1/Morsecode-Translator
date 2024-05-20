![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![NuGet Downloads](https://img.shields.io/nuget/dt/L.MorseCodeTranslator.svg)

![alt text](https://github.com/Lukbes1/Morsecode-Translator/blob/main/MorseIcon.png?raw=true)
# MorseCode-Translator
A small morsecode translator written in C# for general morsecode translation for the .NET Framework 

## Description
This is one of the very first versions of my MorseCode-Translator containing simple convertions.
It makes use of a flexible MorseCharCollection in which you can chose from plenty of characters to display.
You can either start with the whole alphabet, only a few of them or completely new ones that are not defined yet (e.g dollar sign $).

The library offers:
- Basic alphabet convertions from text-to-morse/morse-to-text
- Basic alphabet soundfile creation aswell as output to console
- Custom morsecharacter convertions from text-to-morse/morse-to-text
- Custom morsecharacter soundfile creation aswell as output to console
- Morse-soundfile-to-text-conversion as in Soundfile -> MorseChar (Only works with the morse soundfiles created within this project or those similiar)
  
## Installation
To install the library, you can use the NuGet-package market through visual studio or per NuGet package installer:
```console
dotnet add package L.MorseCodeTranslator --version xxx
```
Or the build in version in Visual Studio:
```console
NuGet\Install-Package L.MorseCodeTranslator -Version xxx
```
Newest version is listed here: https://www.nuget.org/packages/L.MorseCodeTranslator/
## Usage
Depending on what you want to use you can go different routes. 

### Getting started
To make complete use of the MorseCode-Translator start by creating an instance of a MorseCodeTranslator:
```csharp
MorseCodeTranslator translator = new MorseCodeTranslator();
```
The constructor with no parameters initializes all MorseChars from the alphabet inside of a MorseCharCollection.
Alternatively you can create only a handful of MorseChars with custom MorseRepresentations:
```csharp
List<MorseChar> morseList = new List<MorseChar>
{
    new MorseChar('n', "..."),
    new MorseChar('a',".--"),
    new MorseChar('h',".-.-")
};

MorseCharCollection morseCharCollection = new MorseCharCollection(morseList);
MorseCodeTranslator translator = new MorseCodeTranslator(morseCharCollection);
//use translator and or the collection from here on...
```
Once you've created a collection or a translator, you can now start using their methods for MorseConvertions or SoundFile convertions.
Keep in mind that if you chose to use only a handful of MorseChars, the MorseCodeTranslator wont have access to the specific soundfiles of the alphabet.
In any case, the programm will create the soundfiles for each char specified.

However, if there are only e.g. 3 chars inside the MorseCodeTranslator and you use any kind of convertion method, the MorseCodeTranslator refers back to the original MorseRepresentations from the alphabet (See examples).

### MorseConvertions
First of all, you can convert the whole MorseCharCollection back into its MorseRepresentations and use it for example like so:
```csharp
string[] morseRepresentations = morseCharCollection.ConvertToMorseCharRepresentation();
for (int i = 0; i < morseRepresentations.Length; i++)
{
    Console.WriteLine(morseRepresentations[i]);
}	
//Output with example MorseCharCollection from before
//...
//.--
//.-.-
```
Moreover, you can use the translator to for example read a string input and convert it to the MorseCharRepresentations:
```csharp
MorseCodeTranslator translator = new MorseCodeTranslator();
while (true)
{
    Console.WriteLine("Type in some text: ");
    string input = Console.ReadLine();
    try
    {
        string[] inputToMorse = translator.ConvertStringToMorse(input);
        for (int i = 0; i < inputToMorse.Length; i++)
        {
            Console.WriteLine(inputToMorse[i]);
        }
    }
    catch (MorseCharNotFoundException ex)
    {
        Console.WriteLine("Wrong input: " + ex.Message);
    }
}
//input: hello world
/*output: ....
          .
          .-..
          .-..
          ---
        
          .--
          ---
          .-.
          .-..
          -.. */
```
If we were to type in hello world with any upper case letters we'd get an expected exception. This is because the methods are all case sensitive, meaning you can have a different morse representation for 'a' and 'A' respectively.
```csharp
//input Hello World
//Wrong input: Error: cannot convert text into morse-representation because there are characters that were not defined yet
```
There are many more Morse/text-based convertions including MorseToChar, CharToMorse, StringToMorse, MorseToString etc.. 

### SoundConversions
There are many incorporated methods that help you realise the sound aspect of morse code within this project.
First of all, you can play the morse sounds to the console (or most other application) by using the PlayMorse methods.

```csharp
MorseCodeTranslator translator = new MorseCodeTranslator();
while (true)
{
    Console.WriteLine("Type in some text: ");
    string input = Console.ReadLine();
    try
    {
        string[] inputToMorse = translator.ConvertStringToMorse(input);
        for (int i = 0; i < inputToMorse.Length; i++)
        {
            Console.WriteLine(inputToMorse[i]);
        }
        translator.PlayMorseFromString(input);
    }
    catch (MorseCharNotFoundException ex)
    {
        Console.WriteLine("Wrong input: " + ex.Message);
    }
}
``` 
For the PlayMorseFromString version, you can chose to implement a delay between each char.
```csharp
translator.PlayMorseFromString(input,TimeSpan.FromSeconds(1));
```

Moreover, you can use the MorseCodeTranslator's (in some cases static) methods to encode and decode soundfiles.
Encode and then decode like so:

```csharp
MorseCodeTranslator translator = new MorseCodeTranslator();
var morseFromString = translator.ConvertStringToMorse("csharp is the best language ever");

MorseCodeTranslator.EncodeMorseToSoundFile(morseFromString, "CSHARP");

var morseFromSoundFile = MorseCodeTranslator.DecodeSoundFileToMorse(@"MorseSoundFiles\CSHARP.wav");
var textFromSoundFile = translator.DecodeSoundFileToText(@"MorseSoundFiles\CSHARP.wav");
foreach (var morse in morseFromSoundFile)
{
    Console.WriteLine("MorseFromSoundFile: " + morse);
}
Console.WriteLine("TextFromConversion: " + translator.ConvertMorseToString(morseFromSoundFile));
Console.WriteLine("TextFromSoundFile: "+ textFromSoundFile);
//Output:
/*
MorseFromSoundFile: -.-.
MorseFromSoundFile: ...
MorseFromSoundFile: ....
MorseFromSoundFile: .-
MorseFromSoundFile: .-.
MorseFromSoundFile: .--.
MorseFromSoundFile:
MorseFromSoundFile: ..
MorseFromSoundFile: ...
MorseFromSoundFile:
MorseFromSoundFile: -
MorseFromSoundFile: ....
MorseFromSoundFile: .
MorseFromSoundFile:
MorseFromSoundFile: -...
MorseFromSoundFile: .
MorseFromSoundFile: ...
MorseFromSoundFile: -
MorseFromSoundFile:
MorseFromSoundFile: .-..
MorseFromSoundFile: .-
MorseFromSoundFile: -.
MorseFromSoundFile: --.
MorseFromSoundFile: ..-
MorseFromSoundFile: .-
MorseFromSoundFile: --.
MorseFromSoundFile: .
MorseFromSoundFile:
MorseFromSoundFile: .
MorseFromSoundFile: ...-
MorseFromSoundFile: .
MorseFromSoundFile: .-.
TextFromConversion: csharp is the best language ever
TextFromSoundFile: csharp is the best language ever
*/
```
The reading/decoding algorithm is based upon the assumption that the moments of silence are 0 when normalized as a float sample and that the waveformat is the same as this: 32 bit IEEFloat: 8000Hz 1 channel. Other waveformats won't work.
If your custom wavfiles match this pattern, but your beeps/silences are longer/shorter than my prefabs (see MorseCodeAudio dir) then try changing the static value sample_difference_threashold_factor around. 

### Changing the directory of located files
The default directories for the MorseSoundFiles and MorseCodeAudios are in either the executable directory or two directories behind that. 
If you would like to change where the generated files are put, use the MorseCharCollection's static property and for the pre defined audios (beep short/long and silence) do the same as follows:
```csharp
	MorseCharCollection.MorseSoundFilesDir = @"C:\exampleDir\MorseCodeFiles";
	MorseCharCollection.MorseCodeAudioDir = @"C:\exampleDir\MorseCodeAudio";
```
Whenever you generate files from this point on, the code will look for these two directories.
Note that files created beforehand (those located in MorseCodeFiles) will not be moved. However, every MorseChar created before and after setting this property will automatically be located in the new directory.
For the MorseCodeAudio, you have to make sure the files are actually located where you said they would be after changing the property.
### Known issues
- Some documentations are not correct/missing.
- Inconsistent use of alphabet lookup/not clear what methods only use the internal MorseCharCollection and what methods also use the alphabet.
- Unit tests done for:
   + [x] MorseCodeTranslator
   + [ ] MorseChar
   + [ ] MorseCharCollection (Started)
   + [ ] MorseAudioReader
   + [ ] 
### Last note
This is just a side project i made for fun, don't expect too much.
Please let me know if you would like any specific changes made or if there are any bugs, thanks to everyone using my package!
