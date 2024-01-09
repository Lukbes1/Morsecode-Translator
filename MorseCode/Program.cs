using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseCode
{
	internal class Program
	{
		static void Main(string[] args)
		{


			PlayMorseAsync();


            Console.ReadKey();

			//ToDo methoden documenten, besser documenten, speed zwischen PlayMorse, Alle Converts einbauen, Morse zu Datei umwandeln.

		}

		public static void PlayMorseAsync()
		{
			string path = @"MorseCodeAudio\";
			MorseCodeTranslator morseTranslator = new MorseCodeTranslator(path);

			Console.WriteLine("Type in your sentence: ");
			string answer = Console.ReadLine();

		
			string[] morseRepresentation = morseTranslator.ConvertStringToMorse(answer, true);
			List<MorseChar> morseChars = new List<MorseChar>();
			for (int i = 0; i < morseRepresentation.Length; i++)
			{
				morseTranslator.PlayMorseFromChar(morseTranslator.ConvertMorseToChar(morseRepresentation[i]));
                Console.WriteLine(morseTranslator.ConvertMorseToChar(morseRepresentation[i]));
                //morseChars.Add(morseTranslator.MorseCodes.Find(morseTranslator.ConvertMorseToChar(morseRepresentation[i])));
			}

			//foreach (MorseChar morseChar in morseChars)
			//{
   //             Console.WriteLine("Char: " + morseChar.Character);
			//	Console.WriteLine("Representation: " + morseChar.MorseRepresentation);
			//	Console.WriteLine("Soundfile: " + morseChar.SoundFile);
			//	Console.WriteLine("---------------------------");
			//}	
		}
	}
}
