# MorseCode-Translator
A small morsecode translator written in C# for general morsecode translation for a console app.

## Description
This is the first version of version of my MorseCode-Translator containing simple convertions.
It makes use of a flexible MorseCharCollection in which you can chose from plenty of characters to display.
You can either start with the whole alphabet, only a few of them or completely new ones that are not defined yet (e.g dollar sign $).

## Installation
To install the library, you can use the NuGet-package market:
```console
dotnet add package MorseCodeTranslator --version 1.0.1
```
Or the build in version in Visual Studio:
```console
NuGet\Install-Package MorseCodeTranslator -Version 1.0.1
```

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
//input: Hello World
//
```
