using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MorseCode
{
	internal class Program
	{
		static void Main(string[] args)
		{

			//List<MorseChar> morse = new List<MorseChar>
			//{
			//	new MorseChar('S',".-.-",@"C:\Users\lukbe\source\repos\MorseCode\MorseCode\bin\Debug\MorseSoundFiles\Salsa.wav")
			//};
			MorseCodeTranslator translator = new MorseCodeTranslator(new MorseCharCollection());
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
         
			Console.ReadKey();
			List<MorseChar> morse = new List<MorseChar>
			{
				new MorseChar('n', "..."),
				new MorseChar('ä',".--"),
				new MorseChar('h',".-.-")
			};

			MorseCharCollection morseChars = new MorseCharCollection(morse);
			MorseCodeTranslator morseTranslator = new MorseCodeTranslator(morseChars);
			//while (true) 
			//{
			//	morseTranslator.PlayMorseFromChar('S');
			//	Thread.Sleep(1000);
			//}

			string[] morseRepresentations = morseTranslator.ConvertStringToMorse("näh");
			do
			{
				for (int i = 0; i < morseRepresentations.Length; i++)
				{
					Console.WriteLine(morseRepresentations[i]);
				}
			} while (true == false);

			MorseCodeTranslator.DecodeMorseToSoundFile(".-.-", "Salsa");




			//ToDo methoden documenten, besser documenten, speed zwischen PlayMorse, Alle Converts einbauen, Morse zu Datei umwandeln.

		}

	}
}
