![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)

# MorseCode-Translator
A small morsecode translator written in C# for general morsecode translation for a console app.

## Description
This is one the very first versions of my MorseCode-Translator containing simple convertions.
It makes use of a flexible MorseCharCollection in which you can chose from plenty of characters to display.
You can either start with the whole alphabet, only a few of them or completely new ones that are not defined yet (e.g dollar sign $).

The library offers:
- Basic alphabet convertions from text-to-morse/morse-to-text
- Basic alphabet soundfile creation aswell as output to console
- Custom morsecharacter convertions from text-to-morse/morse-to-text
- Custom morsecharacter soundfile creation aswell as output to console

Whats still in my toDo:
+ Morse-soundfile-to-text-conversion (Soundfile -> MorseChar)

## Installation
To install the library, you can use the NuGet-package market through visual studio or per NuGet package installer:
```console
dotnet add package MorseCodeTranslator --version xxx
```
Or the build in version in Visual Studio:
```console
NuGet\Install-Package MorseCodeTranslator -Version xxx
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
Once you've created a collection or a translator, you can now start using their methods for MorseConvertions or SoundFile convertions
Keep in minde that if you chose to use only a handful of MorseChars, the MorseCodeTranslator wont have access to the specific soundfiles of the alphabet.

However, if there are only e.g. 3 chars inside the MorseCodeTranslator and you use any kind of convertion method, the MorseCodeTranslator refers back to the original MorseRepresentations (See examples).

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

More documentation follows later...

### Known issues
- ConvertStringToMorse() and ConvertCharToMorse() dont lookup the alphabet as wished. Maybe implementing a bool flag for disabling/enabling alphabet lookups in general.
- Some documentations are not correct/missing.
 
